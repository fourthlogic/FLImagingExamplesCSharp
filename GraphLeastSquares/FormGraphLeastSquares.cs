using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace GraphLeastSquares
{
    public partial class FormGraphLeastSquares : Form
    {
        public void ErrorMessageBox(CResult cResult, string str)
        {
            string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

            if (str.Length > 1)
                strMessage += str;

            MessageBox.Show(strMessage, "Error");
        }

        public FormGraphLeastSquares()
        {
            InitializeComponent();

            this.buttonAdd.Click += new System.EventHandler(this.ClickButtonAdd);
            this.buttonClear.Click += new System.EventHandler(this.ClickButtonClear);

            this.Load += new System.EventHandler(this.FormGraphViewLoad);
            this.CenterToScreen();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private void ClickButtonAdd(object sender, EventArgs e)
        {
            string strInfo = "";
            Random rand = new Random();

            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                EColor eColor = new EColor();
                eColor = (EColor)((uint)(((char)(rand.Next() % 255) | ((uint)((char)(rand.Next() % 255)) << 8)) | (((uint)(char)(rand.Next() % 255)) << 16)));

                string strName = textBoxName.Text;
                string strDegree = textBoxDegree.Text;

                int i32Degree = 0; 
                int.TryParse(strDegree, out i32Degree);

                // 랜덤으로 100개의 데이터를 생성한다.
                const int i32DataCount = 100;
                double[] arrF64DataX = new double[i32DataCount];
                double[] arrF64DataY = new double[i32DataCount];

                double f64PrevX = 0;
                double f64PrevY = 0;

                for (int i = 0; i < i32DataCount; ++i)
                {
                    arrF64DataX[i] = f64PrevX + ((rand.Next() % 100) / 10);
                    if (rand.Next() % 2 != 0)
                        arrF64DataY[i] = f64PrevY + ((rand.Next() % 100) / 10);
                    else
                        arrF64DataY[i] = f64PrevY - ((rand.Next() % 100) / 10);

                    f64PrevX = arrF64DataX[i];
                    f64PrevY = arrF64DataY[i];
                }

                // 그래프에 생성한 데이터를 추가한다.
                m_viewGraph.Plot(arrF64DataX, arrF64DataY, i32DataCount, EChartType.Scatter, eColor, strName);

                if (i32Degree == 0)
                {
                    strInfo = "Please check the degree.";
                    break;
                }

                // LeastSquares<double> 객체 생성 // Create LeastSquares<double> object
                CLeastSquares<double> ls = new CLeastSquares<double>();
                // 데이터를 할당
                ls.Assign(arrF64DataX, arrF64DataY, i32DataCount);

                // 계수 값을 받기 위해 List 생성
                List<double> listOutput = new List<double>();
                // R square 값을 받기 위해double 생성
                double f64TRSqr = 0;

                // 다항식 계수를 얻는다.
                ls.GetPoly(i32Degree, out listOutput, out f64TRSqr);

                string strEquation = "";

                long i64Count = listOutput.Count;
                if (i64Count == 0)
                {
                    strInfo = "Empty result";
                    break;
                }

                // 차수가 높아질수록 계수의 정도를 높인다.
                // 예를 들어 4차식인 경우, 4 + 12 = 16, 즉 소수점 16째 자리까지 계수를 표현한다.
                int i32Precision = i32Degree + 12;
                string strPrecision = String.Format("F{0}", i32Precision);
                strPrecision = "{0:" + strPrecision + "}";

                // 얻어온 계수로 다항식을 만든다.
                for (int i = 0; i < (int)i64Count; ++i)
                {
                    double f64Coef = listOutput[i];
                    if (f64Coef == 0)
                        continue;
                      
                    if (strEquation != "" && f64Coef > 0)
                        strEquation += " + ";

                    string strFormat = "";

                    if (i == (i64Count - 2))
                        strFormat = String.Format(strPrecision + "*x", f64Coef);
                    else if (i == (i64Count - 1))
                        strFormat = String.Format(strPrecision, f64Coef);
                    else
                        strFormat = String.Format(strPrecision+"*x^{1:0}", f64Coef, i64Count - 1 - i);

                    strEquation += strFormat;
                }

                if (strEquation == "")
                    break;

                strInfo = String.Format("R square value: {0}", f64TRSqr);

                // 수식 객체 생성 // Create 수식 object
                CExpression exp = new CExpression();

                // 수식 문자열을 설정한다
                exp.SetExpression(strEquation);

                // 그래프 뷰에 수식 데이터를 추가한다
                m_viewGraph.Plot(exp, eColor);

                // 그래프 뷰를 갱신한다
                m_viewGraph.Invalidate();
            }
            while (false);

            richTextBoxInfo.Text = strInfo;
        }
        private void ClickButtonClear(object sender, EventArgs e)
        {
            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                // 그래프 뷰의 데이터를 초기화한다
                m_viewGraph.Clear();

                // 그래프 뷰를 갱신한다
                m_viewGraph.Invalidate();
            }
            while (false);
        }
        private void FormGraphViewLoad(object sender, EventArgs e)
        {
            m_timer = new Timer();
            m_timer.Tick += new System.EventHandler(this.TimerTick);
            m_timer.Interval = 100;
            m_timer.Start();

            DockGraphViewToThis();
            ClickButtonAdd(sender, e);
            UpdateControls();
        }
        private void DockGraphViewToThis()
        {
            m_viewGraph = new CGUIViewGraph();

            // 그래프 뷰 생성
            CResult eResult = m_viewGraph.Create(0, 0, 540, 435);

            if(eResult.IsFail())
                ErrorMessageBox(eResult, "");

            // 그래프 뷰의 윈도우를 얻어온다.
            ulong hWndGraphView = m_viewGraph.GetWindowHandle();

            if (hWndGraphView != 0)
            {
                // 현재 Form 을 Graph view 의 parent 로 설정한다.
                SetParent((IntPtr)hWndGraphView, this.Handle);

                // Graph view 의 Form 내에서의 위치를 이동한다.
                MoveWindow((IntPtr)hWndGraphView, 10, 15, 540, 435, true);
            }
        }
        private void UpdateControls()
        {
            // 그래프 뷰 유효성 체크
            buttonAdd.Enabled = m_viewGraph.IsAvailable();

            // 그래프 뷰 유효성 체크, 그래프 뷰 데이터 존재 여부 체크
            buttonClear.Enabled = (m_viewGraph.IsAvailable() && m_viewGraph.DoesGraphExist());
        }
        private void TimerTick(object sender, EventArgs e)
        {
            this.UpdateControls();
        }

        private CGUIViewGraph m_viewGraph;
        private Timer m_timer;
    }
}

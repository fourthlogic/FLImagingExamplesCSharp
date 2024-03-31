using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace GraphView
{
    public partial class FormGraphView : Form
    {
        public void ErrorMessageBox(CResult cResult, string str)
        {
            string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

            if (str.Length > 1)
                strMessage += str;

            MessageBox.Show(strMessage, "Error");
        }

        public FormGraphView()
        {
            InitializeComponent();

            this.buttonOpenView.Click += new System.EventHandler(this.ClickButtonOpenView);
            this.buttonTerminateView.Click += new System.EventHandler(this.ClickButtonTerminateView);
            this.buttonLoadGraph.Click += new System.EventHandler(this.ClickButtonLoadGraph);
            this.buttonSaveGraph.Click += new System.EventHandler(this.ClickButtonSaveGraph);
            this.buttonAdd.Click += new System.EventHandler(this.ClickButtonChartAdd);
            this.buttonClear.Click += new System.EventHandler(this.ClickButtonClear);

            this.Load += new System.EventHandler(this.FormGraphViewLoad);
            this.CenterToScreen();
        }
        private void FormGraphViewLoad(object sender, EventArgs e)
        {
            m_timer = new Timer();
            m_timer.Tick += new System.EventHandler(this.TimerTick);
            m_timer.Interval = 100;
            m_timer.Start();

            m_viewGraph = new CGUIViewGraph();

            UpdateControls();
        }
        private void ClickButtonOpenView(object sender, EventArgs e)
        {
            do
            {
                // 그래프 뷰 유효성 체크
                if (m_viewGraph.IsAvailable())
                    break;

                // 그래프 뷰 생성
                CResult res = m_viewGraph.Create(0, 0, 500, 500);

                if (res.IsFail())
                    ErrorMessageBox(res, "");
            }
            while (false);
        }
        private void ClickButtonTerminateView(object sender, EventArgs e)
        {
            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                // 그래프 뷰를 종료한다.
                CResult res = m_viewGraph.Destroy();

                if (res.IsFail())
                    ErrorMessageBox(res, "");
            }
            while (false);
        }
        private void ClickButtonLoadGraph(object sender, EventArgs e)
        {
            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                LockControls(true);

                // 그래프 파일 로드 다이얼로그를 활성화 시킨다.
                // 가장 마지막 파라미터로 로드 옵션을 지정한다.
                // ex) EViewGraphLoadOption.Load -> 그래프 파일
                //     EViewGraphLoadOption.Load | EViewGraphLoadOption.OpenDialog 그래프 파일 로드 다이얼로그 활성화
                m_viewGraph.Load(null, EViewGraphLoadOption.Load | EViewGraphLoadOption.OpenDialog);

                // 그래프 뷰를 갱신 한다.
                m_viewGraph.Invalidate();

                LockControls(false);
            }
            while (false);
        }
        private void ClickButtonSaveGraph(object sender, EventArgs e)
        {
            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                LockControls(true);

                // 그래프 저장 다이얼로그를 활성화 시킨다.
                m_viewGraph.Save();

                LockControls(false);
            }
            while (false);
        }
        private void ClickButtonChartAdd(object sender, EventArgs e)
        {
            // TODO: Add your control notification handler code here
            Random rand = new Random();

            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                // 입력한 차트이름을 얻어온다.
                string strChartName = textBoxChartName.Text;

                if (strChartName == "")
                    strChartName = "Chart";

                // 선택한 차트타입을 얻어온다.
                EChartType eChartType = (EChartType)comboBoxChartType.SelectedIndex ;

                // 랜덤으로 10개의 데이터를 생성한다.
                const int i32DataCount = 10;
                List<double> arrF64DataX1 = new List<double>();
                List<double> arrF64DataY1 = new List<double>();

                for (int i = 0; i < i32DataCount; ++i)
                {
                    arrF64DataX1.Add((double)(rand.Next() % 100));
                    arrF64DataY1.Add((double)(rand.Next() % 100));
                }

                // 그래프에 생성한 데이터를 추가한다.
                EColor eColor = new EColor();
                eColor = (EColor)((uint)(((char)(rand.Next() % 255) | ((uint)((char)(rand.Next() % 255)) << 8)) | (((uint)(char)(rand.Next() % 255)) << 16)));

                m_viewGraph.Plot(arrF64DataX1, arrF64DataY1, eChartType, eColor, strChartName);

                // 그래프 뷰를 갱신 한다.
                m_viewGraph.Invalidate();
            }
            while (false);
        }
        private void ClickButtonClear(object sender, EventArgs e)
        {
            do
            {
                // 그래프 뷰 유효성 체크
                if (!m_viewGraph.IsAvailable())
                    break;

                // 그래프의 데이터를 클리어한다.
                m_viewGraph.Clear();

                // 그래프 뷰를 갱신 한다.
                m_viewGraph.Invalidate();
            }
            while (false);
        }
        private void LockControls(bool bLock)
        {
            m_bLockControls = bLock;
            UpdateControls();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            this.UpdateControls();
        }
        private void UpdateControls()
        {
            if (m_bLockControls)
            {
                buttonOpenView.Enabled = false;
                buttonTerminateView.Enabled = false;

                buttonLoadGraph.Enabled = false;
                buttonSaveGraph.Enabled = false;

                textBoxChartName.Enabled = false;
                comboBoxChartType.Enabled = false;
                buttonAdd.Enabled = false;
                buttonClear.Enabled = false;
            }
            // 그래프 뷰 유효성 체크
            else if (!m_viewGraph.IsAvailable())
            {
                buttonOpenView.Enabled = true;
               buttonTerminateView.Enabled = false;

                buttonLoadGraph.Enabled = false;
                buttonSaveGraph.Enabled = false;

                textBoxChartName.Enabled = false;
                comboBoxChartType.Enabled = false;
                buttonAdd.Enabled = false;
                buttonClear.Enabled = false;
            }
            else
            {
                buttonOpenView.Enabled = false;
               buttonTerminateView.Enabled = true;

                buttonLoadGraph.Enabled = true;

                // 그래프 차트 데이터/수식 데이터의 존재 유무를 얻어 온다.
                if (m_viewGraph.DoesGraphExist())
                {
                    buttonSaveGraph.Enabled = true;
                    buttonClear.Enabled = true;
                }
                else
                {
                    buttonSaveGraph.Enabled = false;
                    buttonClear.Enabled = false;
                }

                textBoxChartName.Enabled = true;
                comboBoxChartType.Enabled = true;
                buttonAdd.Enabled = true;
            }
        }

        private CGUIViewGraph m_viewGraph;
        private Timer m_timer;
        private bool m_bLockControls;
    }
}

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

namespace GraphView
{
	public partial class FormGraphViewIntoDialog : Form
	{
		public void ErrorMessageBox(CResult cResult, string str)
		{
			string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

			if(str.Length > 1)
				strMessage += str;

			MessageBox.Show(strMessage, "Error");
		}

		public FormGraphViewIntoDialog()
		{
			InitializeComponent();

			this.buttonAdd.Click += new System.EventHandler(this.ClickButtonAdd);
			this.buttonClear.Click += new System.EventHandler(this.ClickButtonClear);

			this.Load += new System.EventHandler(this.FormGraphViewLoad);
			this.CenterToScreen();
		}

		private void ClickButtonAdd(object sender, EventArgs e)
		{
			string strInfo = "";
			Random rand = new Random();

			do
			{
				// 그래프 뷰 유효성 체크
				if(!m_viewGraph.IsAvailable())
					break;

				System.Windows.Forms.TextBox[] textBoxCoeff = new System.Windows.Forms.TextBox[5];

				textBoxCoeff[0] = textBoxA;
				textBoxCoeff[1] = textBoxB;
				textBoxCoeff[2] = textBoxC;
				textBoxCoeff[3] = textBoxD;
				textBoxCoeff[4] = textBoxE;

				double[] arrF64Coef = new double[5];
				System.Numerics.Complex[] arrCpxCoef = new System.Numerics.Complex[5];
				string strEquation = "";

				for(int i = 0; i < 5; ++i)
				{
					string strCoef = textBoxCoeff[i].Text;
					Double.TryParse(strCoef, out arrF64Coef[i]);
					if(arrF64Coef[i] == 0)
						continue;

					if(strEquation != "" && arrF64Coef[i] > 0)
						strEquation += " + ";

					string strFormat = "";
					if(i == 3)
					{
						strFormat = String.Format("{0}*x", arrF64Coef[i]);
					}
					else if(i == 4)
					{
						strFormat = String.Format("{0}", arrF64Coef[i]);
					}
					else
					{
						strFormat = String.Format("{0}*x^{1}", arrF64Coef[i], 4 - i);
					}

					arrCpxCoef[i] = new System.Numerics.Complex(arrF64Coef[i], 0);
					strEquation += strFormat;
				}

				if(strEquation == "")
					break;

				// 방정식의 해를 얻어올 객체 생성 // Create 방정식의 해를 얻어올 object
				List<System.Numerics.Complex> listEquationResult = new List<System.Numerics.Complex>();

				CResult cResult = CEquation.Quartic(arrCpxCoef[0], arrCpxCoef[1], arrCpxCoef[2], arrCpxCoef[3], arrCpxCoef[4], ref listEquationResult);

				if(cResult.IsOK())
				{
					for(int i = 0; i < listEquationResult.Count; ++i)
					{
						System.Numerics.Complex cp = listEquationResult[i];
						string strCP = "";
						if(cp.Imaginary == 0)
							strCP = String.Format("{0}", cp.Real);
						else if(cp.Imaginary > 0)
							strCP = String.Format("{0}+{1}i", cp.Real, cp.Imaginary);
						else
							strCP = String.Format("{0}{1}i", cp.Real, cp.Imaginary);

						strInfo += strCP + "\r\n\r\n";
					}
				}

				// 수식 객체 생성 // Create 수식 object
				CExpression exp = new CExpression();

				// 수식 문자열을 설정한다
				exp.SetExpression(strEquation);

				// 그래프 뷰에 수식 데이터를 추가한다
				EColor eColor = new EColor();
				eColor = (EColor)((uint)(((char)(rand.Next() % 255) | ((uint)((char)(rand.Next() % 255)) << 8)) | (((uint)(char)(rand.Next() % 255)) << 16)));

				m_viewGraph.Plot(exp, eColor);

				// 그래프 뷰를 갱신한다
				m_viewGraph.Invalidate();
			}
			while(false);

			strInfo = "[root]\r\n" + strInfo;
			richTextBoxInfo.Text = strInfo;
		}
		private void ClickButtonClear(object sender, EventArgs e)
		{
			do
			{
				// 그래프 뷰 유효성 체크
				if(!m_viewGraph.IsAvailable())
					break;

				// 그래프 뷰의 데이터를 초기화한다
				m_viewGraph.Clear();

				// 그래프 뷰를 갱신한다
				m_viewGraph.Invalidate();
			}
			while(false);
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
			CResult res = m_viewGraph.CreateAndFitParent((ulong)pictureBoxView.Handle);

			if(res.IsFail())
				ErrorMessageBox(res, "");
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

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

namespace FLImagingExamplesCSharp
{
	public partial class FormView3DIntoDialog : Form
	{
		public void ErrorMessageBox(CResult cResult, string str)
		{
			string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

			if(str.Length > 1)
				strMessage += str;

			MessageBox.Show(strMessage, "Error");
		}

		public FormView3DIntoDialog()
		{
			InitializeComponent();

			this.buttonGetHeightProfile.Click += new System.EventHandler(this.ClickButtonGetHeightProfile);

			this.Load += new System.EventHandler(this.FormImageViewLoad);
			this.CenterToScreen();
		}

		private void FormImageViewLoad(object sender, EventArgs e)
		{
			DockView3DToThis();
		}
		private void ClickButtonGetHeightProfile(object sender, EventArgs e)
		{
			do
			{
				// 3D 뷰 유효성 체크
				if(!m_view3D.IsAvailable())
					break;

				// 높이 프로파일의 좌표를 Edit box 로부터 얻어 와 지정한다.
				long i64StartX = long.Parse(this.textBoxStartX.Text);
				long i64StartY = long.Parse(this.textBoxStartY.Text);
				long i64EndX = long.Parse(this.textBoxEndX.Text);
				long i64EndY = long.Parse(this.textBoxEndY.Text);

				CFLPoint<long> flpStart = new CFLPoint<long>(i64StartX, i64StartY);
				CFLPoint<long> flpEnd = new CFLPoint<long>(i64EndX, i64EndY);
				List<double> listF64HP = new List<double>();

				// 높이 프로파일 정보를 얻어 온다.
				CResult gr = m_view3D.GetHeightProfile(flpStart, flpEnd, ref listF64HP);

				if(gr.IsOK())
				{
					string str = "";

					for(int i = 0; i < listF64HP.Count(); ++i)
					{
						str += String.Format("[{0}] {1}\n", i, listF64HP[i]);
					}

					this.richTextBoxInfo.Text = str;
				}
				else if(gr.GetResult() == EResult.OutOfRange)
				{
					this.richTextBoxInfo.Text = "Error - OutOfRange";
				}
				else
				{
					this.richTextBoxInfo.Text = "Error";
				}

				m_view3D.Invalidate();
			}
			while(false);
		}

		private void DockView3DToThis()
		{
			m_view3D = new CGUIView3D();

			// 3D 뷰 생성
			CResult res = m_view3D.CreateAndFitParent((ulong)pictureBoxView.Handle);

			if(res.IsFail())
				ErrorMessageBox(res, "");

			// 높이 맵 이미지와 텍스쳐 로드 // Load height map image and texture
			m_view3D.Load("../../ExampleImages/View3D/mountain.flif", "../../ExampleImages/View3D/mountain_texture.flif");
		}

		private CGUIView3D m_view3D;
	}
}

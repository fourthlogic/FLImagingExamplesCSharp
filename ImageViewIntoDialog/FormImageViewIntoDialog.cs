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
	public partial class FormImageViewIntoDialog : Form
	{
		public void ErrorMessageBox(CResult cResult, string str)
		{
			string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

			if(str.Length > 1)
				strMessage += str;

			MessageBox.Show(strMessage, "Error");
		}

		public FormImageViewIntoDialog()
		{
			InitializeComponent();

			this.buttonCreate.Click += new System.EventHandler(this.ClickButtonCreate);
			this.buttonPopFront.Click += new System.EventHandler(this.ClickButtonPopFront);

			this.Load += new System.EventHandler(this.FormImageViewLoad);
			this.CenterToScreen();
		}

		private void FormImageViewLoad(object sender, EventArgs e)
		{
			m_timer = new Timer();
			m_timer.Tick += new System.EventHandler(this.TimerTick);
			m_timer.Interval = 100;
			m_timer.Start();
			DockImageViewToThis();
			UpdateControls();
		}
		private void ClickButtonCreate(object sender, EventArgs e)
		{
			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				// 이미지 뷰의 캔버스 영역을 얻어온다.
				CFLRect<int> flrlCanvas = m_viewImage.GetClientRectCanvasRegion();

				// 캔버스 영역의 좌표계를 이미지 영역의 좌표계로 변환한다.
				CFLRect<double> flrdImage = m_viewImage.ConvertCanvasCoordToImageCoord(flrlCanvas);

				// 이미지 영역을 기준으로 생성될 Figure 의 크기와 모양을 사각형으로 설정한다.
				double f64Width = flrdImage.GetWidth() / (double)10;
				double f64Height = flrdImage.GetHeight() / (double)10;
				double f64Size = Math.Min(f64Width, f64Height);

				CFLPoint<double> flpdCenter = new CFLPoint<double>(0, 0);
				flrdImage.GetCenter(ref flpdCenter);

				CFLRect<double> flrdFigure = new CFLRect<double>(flpdCenter.x - f64Size, flpdCenter.y - f64Size, flpdCenter.x + f64Size, flpdCenter.y + f64Size);

				// 이미지 뷰에 Figure object 를 생성한다.
				// 가장 마지막 파라미터는 활성화 되는 메뉴의 구성이며, EAvailableFigureContextMenu.All 가 기본 메뉴를 활성화 한다.
				// 활성화 하고자 하는 메뉴를 추가 혹은 제거 하기 위해서는 enum 값을 비트 연산으로 넣어주면 된다.
				// ex) EAvailableFigureContextMenu.None -> 활성화 되는 메뉴 없음
				//     EAvailableFigureContextMenu.All -> 전체 메뉴 활성화
				//     EAvailableFigureContextMenu.DeclType | EAvailableFigureContextMenu.TemplateType -> Decl Type, Template Type 변환 메뉴 활성화
				m_viewImage.PushBackFigureObject(flrdFigure, EAvailableFigureContextMenu.All);
			}
			while(false);
		}
		private void ClickButtonPopFront(object sender, EventArgs e)
		{
			CFLFigure flFigure = null;
			string strFigureInfo = "Error";

			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				// 이미지 뷰의 맨 앞의 Figure 를 제거하면서 얻어온다.
				flFigure = m_viewImage.PopFrontFigureObject();
				if(flFigure == null)
					break;

				// Figure 를 문자열로 얻어온다.
				string strFigure = CFigureUtilities.ConvertFigureObjectToString(flFigure);

				strFigureInfo = strFigure;
			}
			while(false);

			richTextBoxFigureInfo.Text = strFigureInfo;
		}
		private void DockImageViewToThis()
		{
			m_viewImage = new CGUIViewImage();

			// 이미지 뷰 생성 // Create image view
			CResult res = m_viewImage.CreateAndFitParent((ulong)pictureBoxView.Handle);

			if(res.IsFail())
				ErrorMessageBox(res, "");
		}
		private void UpdateControls()
		{
			bool bEnable = false;

			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				// 이미지 뷰의 Figure object 개수를 얻어온다.
				if(m_viewImage.GetFigureObjectCount() == 0)
					break;

				bEnable = true;
			}
			while(false);

			buttonPopFront.Enabled = bEnable;
		}
		private void TimerTick(object sender, EventArgs e)
		{
			this.UpdateControls();
		}

		private CGUIViewImage m_viewImage;
		private Timer m_timer;
	}
}

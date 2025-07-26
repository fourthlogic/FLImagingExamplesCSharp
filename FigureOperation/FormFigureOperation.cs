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
	public partial class FormFigureOperation : Form
	{
		public void ErrorMessageBox(CResult cResult, string str)
		{
			string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

			if(str.Length > 1)
				strMessage += str;

			MessageBox.Show(strMessage, "Error");
		}

		public FormFigureOperation()
		{
			InitializeComponent();

			m_timer = null;
			this.buttonCreate.Click += new System.EventHandler(this.ClickButtonCreate);
			this.buttonClear.Click += new System.EventHandler(this.ClickButtonClear);
			this.comboBoxSrc.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChangedComboBoxSrcFigure);
			this.comboBoxDst.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChangedComboBoxDstFigure);
			this.buttonExecute.Click += new System.EventHandler(this.ClickButtonExecute);

			this.Load += new System.EventHandler(this.FormImageViewLoad);
			this.CenterToScreen();
		}

		private void FormImageViewLoad(object sender, EventArgs e)
		{
			m_timer = new Timer();
			m_timer.Tick += new System.EventHandler(this.TimerTick);
			m_timer.Interval = 30;
			m_timer.Start();

			DockImageViewToThis();
			UpdateControls();
		}
		private void ClickButtonCreate(object sender, EventArgs e)
		{
			CFLFigure flFigure = null;

			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				EFigureTemplateType eTemplateType = EFigureTemplateType.Double;

				switch(comboBoxTemplateType.SelectedIndex)
				{
				case 0: // Int32
					eTemplateType = EFigureTemplateType.Int32;
					break;
				case 1: // Int64
					eTemplateType = EFigureTemplateType.Int64;
					break;
				case 2: // Float
					eTemplateType = EFigureTemplateType.Float;
					break;
				case 3: // Double
					eTemplateType = EFigureTemplateType.Double;
					break;
				}

				// 이미지 뷰의 캔버스 영역을 얻어온다.
				CFLRect<int> flrlCanvas = m_viewImage.GetClientRectCanvasRegion();

				// 캔버스 영역의 좌표계를 이미지 영역의 좌표계로 변환한다.
				CFLRect<double> flrdImage = m_viewImage.ConvertCanvasCoordToImageCoord(flrlCanvas);

				// 이미지 영역을 기준으로 생성될 Figure 의 크기와 모양을 사각형으로 설정한다.
				double f64Width = flrdImage.GetWidth() / 10;
				double f64Height = flrdImage.GetHeight() / 10;
				double f64Size = Math.Min(f64Width, f64Height);

				CFLPoint<double> flpdCenter = new CFLPoint<double>(0, 0);
				flrdImage.GetCenter(ref flpdCenter);

				CFLRect<double> flrdFigureShape = new CFLRect<double>(flpdCenter.x - f64Size, flpdCenter.y - f64Size, flpdCenter.x + f64Size, flpdCenter.y + f64Size);

				// 선택한 Decl Type, Template Type 으로 Figure 를 생성한다.
				// CubicSpline, ComplexRegion 같은 경우에는 Template Type 이 double 형으로 고정이다.
				switch(SelectedDeclType())
				{
				case EFigureDeclType.Point:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLPoint<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLPoint<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLPoint<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLPoint<double>();
							break;

						default:
							break;
						}
					}
					break;

				case EFigureDeclType.Line:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLLine<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLLine<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLLine<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLLine<double>();
							break;

						default:
							break;
						}
					}
					break;

				case EFigureDeclType.Rect:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLRect<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLRect<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLRect<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLRect<double>();
							break;

						default:
							break;
						}
					}
					break;

				case EFigureDeclType.Quad:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLQuad<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLQuad<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLQuad<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLQuad<double>();
							break;

						default:
							break;
						}
					}
					break;

				case EFigureDeclType.Circle:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLCircle<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLCircle<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLCircle<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLCircle<double>();
							break;

						default:
							break;
						}
					}
					break;

				case EFigureDeclType.Ellipse:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLEllipse<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLEllipse<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLEllipse<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLEllipse<double>();
							break;

						default:
							break;
						}
					}
					break;

				case EFigureDeclType.CubicSpline:
					{
						flFigure = new CFLCubicSpline();
					}
					break;

				case EFigureDeclType.Region:
					{
						flFigure = new CFLRegion();
					}
					break;

				case EFigureDeclType.ComplexRegion:
					{
						flFigure = new CFLComplexRegion();
					}
					break;

				case EFigureDeclType.Doughnut:
					{
						switch(eTemplateType)
						{
						case EFigureTemplateType.Int32:
							flFigure = new CFLDoughnut<int>();
							break;

						case EFigureTemplateType.Int64:
							flFigure = new CFLDoughnut<long>();
							break;

						case EFigureTemplateType.Float:
							flFigure = new CFLDoughnut<float>();
							break;

						case EFigureTemplateType.Double:
							flFigure = new CFLDoughnut<double>();
							break;

						default:
							break;
						}
					}
					break;

				default:
					break;
				}

				if(flFigure == null)
					break;

				// 생성된 Figure 에 사각형을 설정함으로써 각 형상에 맞게 구성한다.
				flFigure.Set(flrdFigureShape);

				// 이미지 뷰에 Figure object 를 생성한다.
				// 가장 마지막 파라미터는 활성화 되는 메뉴의 구성이며, EAvailableFigureContextMenu.All 가 기본 메뉴를 활성화 한다.
				// 활성화 하고자 하는 메뉴를 추가 혹은 제거 하기 위해서는 enum 값을 비트 연산으로 넣어주면 된다.
				// ex) EAvailableFigureContextMenu.None . 활성화 되는 메뉴 없음
				//     EAvailableFigureContextMenu.All . 전체 메뉴 활성화
				//     EAvailableFigureContextMenu.DeclType | EAvailableFigureContextMenu.TemplateType . Decl Type, Template Type 변환 메뉴 활성화
				m_viewImage.PushBackFigureObject(flFigure, EAvailableFigureContextMenu.All);

				// 이미지 뷰를 갱신한다. // Update the image view.
				m_viewImage.Invalidate(true);

				// 콤보 박스에 Figure Object 항목을 설정한다.
				UpdateFigureObjectList();
			}
			while(false);
		}
		private void ClickButtonClear(object sender, EventArgs e)
		{
			do
			{
				if(!m_viewImage.IsAvailable())
					break;

				// 현재 이미지 뷰에 있는 Figure Objects 를 제거한다.
				m_viewImage.ClearFigureObject();

				// 이미지 뷰를 갱신한다. // Update the image view.
				m_viewImage.Invalidate(true);

				// 콤보 박스에 Figure Object 항목을 설정한다.
				UpdateFigureObjectList();
			}
			while(false);
		}
		private void SelectedIndexChangedComboBoxSrcFigure(object sender, EventArgs e)
		{
			DrawSelectedFigure();
		}
		private void SelectedIndexChangedComboBoxDstFigure(object sender, EventArgs e)
		{
			DrawSelectedFigure();
		}
		private void ClickButtonExecute(object sender, EventArgs e)
		{
			CFLFigure flFigure1 = null;
			CFLFigure flFigure2 = null;
			CResult cResult = new CResult(EResult.UnknownError);

			do
			{
				// 선택된 Figure Object 를 얻어온다.
				flFigure1 = GetSelectedFigure1();
				flFigure2 = GetSelectedFigure2();

				if(flFigure1 == null || flFigure2 == null)
				{
					cResult.Assign(EResult.InvalidObject);
					break;
				}

				// Operation 결과를 얻기 위해 FLFigureArray 를 생성한다.
				CFLFigureArray flfaRes = new CFLFigureArray();

				switch(comboBoxOperation.SelectedIndex)
				{
				case 0:
					// Intersection Operation 수행
					cResult = flFigure1.GetRegionOfIntersection(flFigure2, ref flfaRes);
					break;

				case 1:
					// Union Operation 수행
					cResult = flFigure1.GetRegionOfUnion(flFigure2, ref flfaRes);
					break;

				case 2:
					// Subtraction Operation 수행
					cResult = flFigure1.GetRegionOfSubtraction(flFigure2, ref flfaRes);
					break;

				case 3:
					// Exclusive Or Operation 수행
					cResult = flFigure1.GetRegionOfExclusiveOr(flFigure2, ref flfaRes);
					break;
				}

				// 수행 결과를 확인한다.
				if(cResult.IsFail())
					break;

				CFLFigureArray flfa = new CFLFigureArray(flfaRes);

				if(flfa.GetCount() == 0)
					break;

				// 이미지 뷰에 Figure object 를 생성한다.
				if(flfa.GetCount() == 1)
					m_viewImage.PushBackFigureObject(flfa.GetAt(0), EAvailableFigureContextMenu.All);
				else
					m_viewImage.PushBackFigureObject(flfa, EAvailableFigureContextMenu.All);

				// 이미지 뷰를 갱신한다. // Update the image view.
				m_viewImage.Invalidate(true);

				// 콤보 박스에 Figure Object 항목을 설정한다.
				UpdateFigureObjectList();
			}
			while(false);

			// 수행 결과 메세지를 표시한다.
			DisplayMessage(cResult.GetString());
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
			// 이미지 뷰 유효성 체크
			if(!m_viewImage.IsAvailable())
			{
				comboBoxDeclType.Enabled = false;
				comboBoxTemplateType.Enabled = false;
				buttonCreate.Enabled = false;
				buttonClear.Enabled = false;
				comboBoxSrc.Enabled = false;
				comboBoxDst.Enabled = false;
				comboBoxOperation.Enabled = false;
				buttonExecute.Enabled = false;
			}
			else
			{
				comboBoxDeclType.Enabled = true;

				if(comboBoxDeclType.DroppedDown == false && (SelectedDeclType() == EFigureDeclType.CubicSpline || SelectedDeclType() == EFigureDeclType.Region || SelectedDeclType() == EFigureDeclType.ComplexRegion))
				{
					if(comboBoxTemplateType.SelectedIndex != 0)
						comboBoxTemplateType.SelectedIndex = 3; // Double

					if(comboBoxTemplateType.Enabled == true)
						comboBoxTemplateType.Enabled = false;
				}
				else if(comboBoxTemplateType.Enabled == false)
					comboBoxTemplateType.Enabled = true;

				buttonCreate.Enabled = true;

				// 이미지 뷰의 Figure object 개수를 얻어온다.
				buttonClear.Enabled = (m_viewImage.GetFigureObjectCount() == 0 ? false : true);

				comboBoxSrc.Enabled = true;
				comboBoxDst.Enabled = true;
				comboBoxOperation.Enabled = true;
				buttonExecute.Enabled = true;
			}

			do
			{
				// 이미지 뷰의 Figure object 개수를 얻어온다.
				int i32Count = m_viewImage.GetFigureObjectCount();

				if(comboBoxSrc.Items.Count == i32Count && comboBoxDst.Items.Count == i32Count)
					break;

				// 콤보 박스에 Figure Object 항목을 설정한다.
				UpdateFigureObjectList();
			}
			while(false);
		}
		private void DrawSelectedFigure()
		{   // 선택된 Figure Object 를 얻어온다.
			CFLFigure flFigure1 = GetSelectedFigure1();
			CFLFigure flFigure2 = GetSelectedFigure2();

			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = m_viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				if(flFigure1 == null && flFigure2 == null)
					break;

				if(flFigure1 != null)
				{
					// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
					// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
					// 여기서 0.5, 0.3이므로 옅은 반투명 상태라고 볼 수 있다.
					// 파라미터 순서 : 레이어 . Figure 객체 . 선 색 . 선 두께 . 면 색 . 펜 스타일 . 선 알파값(불투명도) . 면 알파값 (불투명도)
					layer.DrawFigureImage(flFigure1, EColor.RED, 5, EColor.RED, EGUIViewImagePenStyle.Solid, 0.5f, 0.3f);
				}

				if(flFigure2 != null)
				{
					// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
					// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
					// 여기서 0.5, 0.3이므로 옅은 반투명 상태라고 볼 수 있다.
					// 파라미터 순서 : 레이어 . Figure 객체 . 선 색 . 선 두께 . 면 색 . 펜 스타일 . 선 알파값(불투명도) . 면 알파값 (불투명도)
					layer.DrawFigureImage(flFigure2, EColor.BLUE, 5, EColor.BLUE, EGUIViewImagePenStyle.Solid, 0.5f, 0.3f);
				}
			}
			while(false);

			// 이미지 뷰를 갱신한다. // Update the image view.
			m_viewImage.Invalidate(true);
		}
		private EFigureDeclType SelectedDeclType()
		{
			// 현재 선택된 DeclType 을 얻어온다.            
			EFigureDeclType eReturn = EFigureDeclType.Unknown;

			do
			{
				int i32CurSel = comboBoxDeclType.SelectedIndex;

				switch(i32CurSel)
				{
				case 0:
					eReturn = EFigureDeclType.Point;
					break;

				case 1:
					eReturn = EFigureDeclType.Line;
					break;

				case 2:
					eReturn = EFigureDeclType.Rect;
					break;

				case 3:
					eReturn = EFigureDeclType.Quad;
					break;

				case 4:
					eReturn = EFigureDeclType.Circle;
					break;

				case 5:
					eReturn = EFigureDeclType.Ellipse;
					break;

				case 6:
					eReturn = EFigureDeclType.CubicSpline;
					break;

				case 7:
					eReturn = EFigureDeclType.Region;
					break;

				case 8:
					eReturn = EFigureDeclType.ComplexRegion;
					break;

				case 9:
					eReturn = EFigureDeclType.Doughnut;
					break;

				default:
					break;
				}
			}
			while(false);

			return eReturn;
		}

		private void UpdateFigureObjectList()
		{
			do
			{
				int i32Selected1 = Math.Max(0, comboBoxSrc.SelectedIndex);
				int i32Selected2 = Math.Max(0, comboBoxDst.SelectedIndex);

				comboBoxSrc.Items.Clear();
				comboBoxDst.Items.Clear();

				// 이미지 뷰의 Figure object 개수를 얻어온다.
				int i32Count = m_viewImage.GetFigureObjectCount();

				if(i32Count == 0)
					break;

				for(int i = 0; i < i32Count; ++i)
				{
					// 해당 인덱스의 Figure Object 를 얻어온다.
					CFLFigure flFigure = m_viewImage.GetFigureObject((int)i);
					if(flFigure == null)
						continue;

					// Figure Object 의 이름을 설정한다.
					string strFigureName = MakeFigureObjectName((int)i, flFigure);
					if(strFigureName == "")
						break;

					// 콤보 박스에 항목을 추가한다.                    
					comboBoxSrc.Items.Add(strFigureName);
					comboBoxDst.Items.Add(strFigureName);
				}

				if(comboBoxSrc.Items.Count > i32Selected1)
					comboBoxSrc.SelectedIndex = i32Selected1;
				if(comboBoxDst.Items.Count > i32Selected2)
					comboBoxDst.SelectedIndex = i32Selected2;
			}
			while(false);
		}

		private string MakeFigureObjectName(int i32Index, CFLFigure flFigure)
		{
			string strReturn = "";
			bool bError = false;

			do
			{
				if(flFigure == null)
					break;


				strReturn = "[" + i32Index.ToString() + "] ";

				// Figure 의 DeclType 에 따른 이름 설정
				switch(flFigure.GetDeclType())
				{
				case EFigureDeclType.Point:
					strReturn += "Point";
					break;

				case EFigureDeclType.Line:
					strReturn += "Line";
					break;

				case EFigureDeclType.Rect:
					strReturn += "Rect";
					break;

				case EFigureDeclType.Quad:
					strReturn += "Quad";
					break;

				case EFigureDeclType.Circle:
					strReturn += "Circle";
					break;

				case EFigureDeclType.Ellipse:
					strReturn += "Ellipse";
					break;

				case EFigureDeclType.CubicSpline:
					strReturn += "CubicSpline";
					break;

				case EFigureDeclType.Region:
					strReturn += "Region";
					break;

				case EFigureDeclType.ComplexRegion:
					strReturn += "ComplexRegion";
					break;

				case EFigureDeclType.Array:
					strReturn += "Array";
					break;

				case EFigureDeclType.Doughnut:
					strReturn += "Doughnut";
					break;

				default:
					bError = true;
					break;
				}

				if(bError)
					break;

				// Figure 의 Template Type 에 따른 이름 설정
				switch(flFigure.GetTemplateType())
				{
				case EFigureTemplateType.Int32:
					strReturn += "(Int32)";
					break;

				case EFigureTemplateType.Int64:
					strReturn += "(Int64)";
					break;

				case EFigureTemplateType.Float:
					strReturn += "(Float)";
					break;

				case EFigureTemplateType.Double:
					strReturn += "(Double)";
					break;

				default:
					bError = true;
					break;
				}
			}
			while(false);

			if(bError)
				strReturn = "";

			return strReturn;
		}
		private CFLFigure GetSelectedFigure1()
		{
			CFLFigure flfReturn = null;

			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				int i32Selected = comboBoxSrc.SelectedIndex;

				if(i32Selected < 0)
					break;

				// 해당 인덱스의 Figure Object 를 얻어온다.
				flfReturn = m_viewImage.GetFigureObject(i32Selected);
			}
			while(false);

			return flfReturn;
		}
		private CFLFigure GetSelectedFigure2()
		{
			CFLFigure flfReturn = null;

			do
			{
				// 이미지 뷰 유효성 체크
				if(!m_viewImage.IsAvailable())
					break;

				int i32Selected = comboBoxDst.SelectedIndex;

				if(i32Selected < 0)
					break;

				// 해당 인덱스의 Figure Object 를 얻어온다.
				flfReturn = m_viewImage.GetFigureObject(i32Selected);
			}
			while(false);

			return flfReturn;
		}
		private void DisplayMessage(string strMessage)
		{
			richTextBoxMessage.Text = strMessage;
		}
		private void TimerTick(object sender, EventArgs e)
		{
			this.UpdateControls();
			this.DrawSelectedFigure();
		}

		private CGUIViewImage m_viewImage;
		private Timer m_timer;
	}
}

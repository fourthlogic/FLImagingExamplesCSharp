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

namespace ImageView
{
    public partial class FormImageView : Form
	{
        public void ErrorMessageBox(CResult cResult, string str)
		{
            string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

            if (str.Length > 1)
                strMessage += str;

            MessageBox.Show(strMessage, "Error");
        }

        public FormImageView()
		{
            InitializeComponent();

            this.buttonOpenView.Click += new System.EventHandler(this.ClickButtonOpenView);
            this.buttonTerminateView.Click += new System.EventHandler(this.ClickButtonTerminateView);
            this.buttonLoadImage.Click += new System.EventHandler(this.ClickButtonLoadImage);
            this.buttonSaveImage.Click += new System.EventHandler(this.ClickButtonSaveImage);
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

            m_viewImage = new CGUIViewImage();

            UpdateControls();
        }
        private void ClickButtonOpenView(object sender, EventArgs e)
		{
            do
			{
                // 이미지 뷰 유효성 체크
                if (m_viewImage.IsAvailable())
                    break;

                // 이미지 뷰 생성 // Create image view
                CResult res = m_viewImage.Create(0, 0, 500, 500);

                if (res.IsFail())
                    ErrorMessageBox(res, "");
            }
            while (false);
        }
        private void ClickButtonTerminateView(object sender, EventArgs e)
		{
            do
			{
                // 이미지 뷰 유효성 체크
                if (!m_viewImage.IsAvailable())
                    break;

                // 이미지 뷰를 종료한다.
                CResult res = m_viewImage.Destroy();

                if (res.IsFail())
                    ErrorMessageBox(res, "");
            }
            while (false);
        }
        private void ClickButtonLoadImage(object sender, EventArgs e)
		{
            do
			{
                // 이미지 뷰 유효성 체크
                if (!m_viewImage.IsAvailable())
                    break;

                LockControls(true);

                // 이미지 파일 로드 다이얼로그를 활성화 시킨다.
                // 가장 마지막 파라미터로 로드 옵션을 지정한다.
                // ex) EViewImageLoadOption.Load -> 이미지 파일/폴더 로드
                //     EViewImageLoadOption.OpenDialog | EViewImageLoadOption.DialogTypeFile 이미지 파일 로드 다이얼로그 활성화
                //     EViewImageLoadOption.OpenDialog | EViewImageLoadOption.DialogTypeFolder 폴더 로드 다이얼로그 활성화(폴더 내부의 이미지 파일들을 로드)
                m_viewImage.Load("", EViewImageLoadOption.Load);

                LockControls(false);
            }
            while (false);
        }
        private void ClickButtonSaveImage(object sender, EventArgs e)
		{
            do
			{
                // 이미지 뷰 유효성 체크
                if (!m_viewImage.IsAvailable())
                    break;

                // 이미지 뷰의 이미지 버퍼가 존재하는지 체크
                if (!m_viewImage.DoesFLImageBufferExist())
                    break;
                
                LockControls(true);

                // 이미지 저장 다이얼로그를 활성화 시킨다.
                m_viewImage.Save("", false);

                LockControls(false);
            }
            while (false);
        }
        private void ClickButtonCreate(object sender, EventArgs e)
		{
            CFLFigure flFigure = null;

            do
			{
                // 이미지 뷰 유효성 체크
                if (!m_viewImage.IsAvailable())
                    break;

                EFigureTemplateType eTemplateType = EFigureTemplateType.Unknown;

				switch(comboBoxTemplateType.SelectedIndex)
				{
				case 0:
                    eTemplateType = EFigureTemplateType.Int32;
					break;
				case 1:
					eTemplateType = EFigureTemplateType.Int64;
					break;
				case 2:
					eTemplateType = EFigureTemplateType.Float;
					break;
                case 3:
					eTemplateType = EFigureTemplateType.Double;
					break;
				}

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

                CFLRect<double> flrdFigureShape = new CFLRect<double>(flpdCenter.x -f64Size, flpdCenter.y - f64Size, flpdCenter.x + f64Size, flpdCenter.y + f64Size);

                // 선택한 Decl Type, Template Type 으로 Figure 를 생성한다.
                // CubicSpline, ComplexRegion 같은 경우에는 Template Type 이 double 형으로 고정이다.
                switch (SelectedDeclType())
				{
                    case EFigureDeclType.Point:
						{
                            switch (eTemplateType)
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
                            switch (eTemplateType)
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
                            switch (eTemplateType)
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
                            switch (eTemplateType)
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
                            switch (eTemplateType)
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
                            switch (eTemplateType)
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
                            switch (eTemplateType)
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

                if (flFigure == null)
                    break;

                // 생성된 Figure 에 사각형을 설정함으로써 각 형상에 맞게 구성한다.
                flFigure.Set(flrdFigureShape);

                // 이미지 뷰에 Figure object 를 생성한다.
                // 가장 마지막 파라미터는 활성화 되는 메뉴의 구성이며, EAvailableFigureContextMenu.All 가 기본 메뉴를 활성화 한다.
                // 활성화 하고자 하는 메뉴를 추가 혹은 제거 하기 위해서는 enum 값을 비트 연산으로 넣어주면 된다.
                // ex) EAvailableFigureContextMenu.None -> 활성화 되는 메뉴 없음
                //     EAvailableFigureContextMenu.All -> 전체 메뉴 활성화
                //     EAvailableFigureContextMenu.DeclType | EAvailableFigureContextMenu.TemplateType -> Decl Type, Template Type 변환 메뉴 활성화
                m_viewImage.PushBackFigureObject(flFigure, EAvailableFigureContextMenu.All);

            }
            while (false);
        }
        private void ClickButtonPopFront(object sender, EventArgs e)
		{
            CFLFigure flFigure = null;
            string strFigureInfo = "Error";

            do
			{
                // 이미지 뷰 유효성 체크
                if (!m_viewImage.IsAvailable())
                    break;

                // 이미지 뷰의 맨 앞의 Figure 를 제거하면서 얻어온다.
                flFigure = m_viewImage.PopFrontFigureObject();
                if (flFigure == null)
                    break;

                // Figure 를 문자열로 얻어온다.
                string strFigure = CFigureUtilities.ConvertFigureObjectToString(flFigure);

                strFigureInfo = strFigure;
            }
            while (false);

            richTextBoxInfo.Text = strFigureInfo;
        }
        private void LockControls(bool bLock)
		{
            m_bLockControls = bLock;
            UpdateControls();
        }
        private EFigureDeclType SelectedDeclType()
		{
            EFigureDeclType eReturn = EFigureDeclType.Unknown;

            do
			{
                int i32CurSel = comboBoxDeclType.SelectedIndex;

                switch (i32CurSel)
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
            while (false);

            return eReturn;
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
                buttonLoadImage.Enabled = false;
                buttonSaveImage.Enabled = false;
                comboBoxDeclType.Enabled = false;
                comboBoxTemplateType.Enabled = false;
                buttonCreate.Enabled = false;
                buttonPopFront.Enabled = false;
            }
            // 이미지 뷰 유효성 체크
            else if (!m_viewImage.IsAvailable())
			{
                buttonOpenView.Enabled = true;

                buttonTerminateView.Enabled = false;
                buttonLoadImage.Enabled = false;
                buttonSaveImage.Enabled = false;
                comboBoxDeclType.Enabled = false;
                comboBoxTemplateType.Enabled = false;
                buttonCreate.Enabled = false;
                buttonPopFront.Enabled = false;
            }
            else
			{
                buttonOpenView.Enabled = false;

                buttonTerminateView.Enabled = true;
                buttonLoadImage.Enabled = true;
                comboBoxDeclType.Enabled = true;
                buttonCreate.Enabled = true;

                // 이미지 뷰의 이미지 버퍼가 존재하는지 체크
                if (!m_viewImage.DoesFLImageBufferExist())
				{
					buttonSaveImage.Enabled = false;
				}
				else
				{
					buttonSaveImage.Enabled = true;
				}

				if(comboBoxDeclType.DroppedDown == false)
				{
                    if (SelectedDeclType() == EFigureDeclType.CubicSpline || SelectedDeclType() == EFigureDeclType.Region || SelectedDeclType() == EFigureDeclType.ComplexRegion)
					{
                        comboBoxTemplateType.SelectedIndex = comboBoxTemplateType.Items.Count - 1;
						comboBoxTemplateType.Enabled = false;
					}
					else
					{
						comboBoxTemplateType.Enabled = true;
					}
				}
				else
				{
					comboBoxTemplateType.Enabled = true;
				}

				// 이미지 뷰의 Figure object 개수를 얻어온다.
				if (m_viewImage.GetFigureObjectCount() == 0)
				{
                    buttonPopFront.Enabled = false;
				}
				else
				{
					buttonPopFront.Enabled = true;
				}
			}
        }

        private CGUIViewImage m_viewImage;
        private Timer m_timer;
        private bool m_bLockControls;
    }
}

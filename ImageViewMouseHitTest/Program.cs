using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace ImageViewMouseHitTest
{
	// 메세지를 전달 받기 위해 CFLBase 를 상속 받아서 구현
	public class CMessageReceiver : CFLBase
	{
		// CMessageReceiver 생성자
		public CMessageReceiver()
		{
			// 메세지를 전달 받기 위해 CBroadcastManager 에 구독 등록
			CBroadcastManager.Subscribe(this);
		}

		// CMessageReceiver 소멸자
		~CMessageReceiver()
		{
			// 객체가 소멸할때 메세지 수신을 중단하기 위해 구독을 해제한다.
			CBroadcastManager.Unsubscribe(this);
		}

		// 메세지가 들어오면 호출되는 함수 OnReceiveBroadcast 오버라이드 하여 구현
		public override void OnReceiveBroadcast(CBroadcastMessage message)
		{
			do
			{
				// message 가 null 인지 확인
				if(message == null)
					break;

				// 메세지의 채널을 확인
				switch(message.GetChannel())
				{
				case (uint)EGUIBroadcast.ViewImage_PostMouseMove:
					{
						// message 객체를 CBroadcastMessage_GUI_ViewImage_MouseEvent 로 캐스팅
						CBroadcastMessage_GUI_ViewImage_MouseEvent msgMouseEvent = message as CBroadcastMessage_GUI_ViewImage_MouseEvent;

						// msgMouseEvent 가 null 인지 확인
						if(msgMouseEvent == null)
							break;

						// 메세지를 호출한 객체를 CGUIViewImage 로 캐스팅
						CGUIViewImage viewImage = msgMouseEvent.GetCaller() as CGUIViewImage;

						// viewImage 가 null 인지 확인
						if(viewImage == null)
							break;

						// 현재 마우스가 위치한 영역을 표시할 문자열 생성
						String strHitArea;
						String str;

						// 현재 마우스가 위치한 영역을 얻어 옵니다.
						EGUIViewImageHitArea eHitArea = viewImage.GetHitArea();

						if(eHitArea == EGUIViewImageHitArea.None)
							strHitArea = "None";
						else
						{
							strHitArea = "Mouse is ";
							str = "on ";

							if(eHitArea.HasFlag(EGUIViewImageHitArea.MiniMap))
							{
								strHitArea += str + "MiniMap";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.MiniMapDisplayingArea))
							{
								strHitArea += str + "MiniMapDisplayingArea";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.ThumbnailView))
							{
								strHitArea += str + "ThumbnailView";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.ThumbnailViewTop))
							{
								strHitArea += str + "ThumbnailViewTop";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.Figure))
							{
								strHitArea += str + "Figure";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.MultiFigures))
							{
								strHitArea += str + "MultiFigures";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.ImageFigure))
							{
								strHitArea += str + "ImageROI";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.StatusBar))
							{
								strHitArea += str + "StatusBar";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.PageIndex))
							{
								strHitArea += str + "PageIndex";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.PrevPageArrow))
							{
								strHitArea += str + "PrevPageArrow";
								str = "and ";
							}

							if(eHitArea.HasFlag(EGUIViewImageHitArea.NextPageArrow))
							{
								strHitArea += str + "NextPageArrow";
								str = "and ";
							}
							strHitArea += ".";
						}

						// 이미지뷰의 0번 레이어 가져오기
						CGUIViewImageLayer layer = viewImage.GetLayer(0);

						// 기존에 Layer 에 그려진 도형들을 삭제
						layer.Clear();

						// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
						// 색상 파라미터를 EColor.TRANSPARENCY 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다.
						// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
						//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
						// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
						//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
						layer.DrawTextCanvas(new CFLPoint<double>(80, 10), strHitArea, EColor.LIME, EColor.BLACK);

						// 이미지뷰를 갱신
						viewImage.Invalidate();
					}
					break;
				}
			}
			while(false);
		}
	}

	class Program
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}
		[STAThread]
		static void Main(string[] args)
		{
			// 메세지를 전달 받을 CMessageReceiver 객체 생성 // Create 메세지를 전달 받을 CMessageReceiver object
			CMessageReceiver msgReceiver = new CMessageReceiver();

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();

			do
			{
				CResult res;

				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/PagePooling/Multiple File_Min.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(300, 0, 300 + 520, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}


				// 이미지 뷰의 캔버스 영역을 얻어온다.
				CFLRect<int> flrlCanvas = viewImage.GetClientRectCanvasRegion();

				// 캔버스 영역의 좌표계를 이미지 영역의 좌표계로 변환한다.
				CFLRect<double> flrdImage = viewImage.ConvertCanvasCoordToImageCoord(flrlCanvas);

				// 이미지 영역을 기준으로 생성될 Figure 의 크기와 모양을 사각형으로 설정한다.
				double f64Width = flrdImage.GetWidth() / 10;
				double f64Height = flrdImage.GetHeight() / 10;
				double f64Size = Math.Min(f64Width, f64Height);

				CFLPoint<double> flpdCenter = new CFLPoint<double>(0, 0);
				flrdImage.GetCenter(out flpdCenter);

				CFLRect<double> flrdFigureShape = new CFLRect<double>(flpdCenter.x - f64Size, flpdCenter.y - f64Size, flpdCenter.x + f64Size, flpdCenter.y + f64Size);

				// 이미지 뷰에 Figure object 를 생성한다.
				// 가장 마지막 파라미터는 활성화 되는 메뉴의 구성이며, EAvailableFigureContextMenu.All 가 기본 메뉴를 활성화 한다.
				// 활성화 하고자 하는 메뉴를 추가 혹은 제거 하기 위해서는 enum 값을 비트 연산으로 넣어주면 된다.
				// ex) EAvailableFigureContextMenu.None . 활성화 되는 메뉴 없음
				//     EAvailableFigureContextMenu.All . 전체 메뉴 활성화
				//     EAvailableFigureContextMenu.DeclType | EAvailableFigureContextMenu.TemplateType . Decl Type, Template Type 변환 메뉴 활성화
				viewImage.PushBackFigureObject(flrdFigureShape, EAvailableFigureContextMenu.None);


				// 이미지 뷰를 갱신 // Update image view
				viewImage.Invalidate(true);

				// 다중 페이지 이미지의 썸네일 미리보기 뷰를 고정
				viewImage.SetFixThumbnailView(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

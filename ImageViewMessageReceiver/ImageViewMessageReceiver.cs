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

namespace FLImagingExamplesCSharp
{
	// 메세지를 전달 받기 위해 CFLBase 를 상속 받아서 구현
	public class CMessageReceiver : CFLBase
	{
		// CMessageReceiver 생성자
		public CMessageReceiver(ref CGUIViewImage viewImage)
		{
            m_viewImage = viewImage;

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

                // GetCaller() 가 등록한 이미지뷰인지 확인
                if (message.GetCaller() != m_viewImage)
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

						// 이미지뷰의 0번 레이어 가져오기
						CGUIViewImageLayer layer = viewImage.GetLayer(0);

						// 기존에 Layer 에 그려진 도형들을 삭제
						layer.Clear();

						// 마우스 좌표를 표시할 문자열 생성
						String strPosition = String.Format("Move X: {0}, Y: {1}", msgMouseEvent.m_i32CursorX, msgMouseEvent.m_i32CursorY);

						// 아래 함수 DrawTextCanvas는 스크린 좌표를 기준으로 문자열을 뷰어에 출력한다.
						// The function DrawTextCanvas displays a string on the viewer using screen coordinates.
						// 파라미터 순서 : 기준 좌표 Figure 객체 -> 문자열 -> 텍스트 색 -> 텍스트 테두리 색 -> 폰트 크기 -> 실제 크기로 출력 유무 -> 각도 -> 정렬 -> 폰트 이름 -> 텍스트 알파값(불투명도) -> 텍스트 테두리 알파값 (불투명도) -> 폰트 두께 -> 폰트 이탤릭 여부
						// Parameter order: reference coordinate (Figure object) -> text string -> text color -> text outline color -> font size -> render in real-world size (bool) -> angle -> alignment -> font name -> text alpha (opacity) -> text outline alpha (opacity) -> font thickness -> italic font (bool)
						layer.DrawTextCanvas(new CFLPoint<double>(10, 10), strPosition, EColor.LIME, EColor.BLACK);

						// 이미지뷰를 갱신
						viewImage.Invalidate();
					}
					break;

				case (uint)EGUIBroadcast.ViewImage_PostLButtonDown:
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

						// 이미지뷰의 0번 레이어 가져오기
						CGUIViewImageLayer layer = viewImage.GetLayer(0);

						// 기존에 Layer 에 그려진 도형들을 삭제
						layer.Clear();

						// 마우스 좌표를 표시할 문자열 생성
						String strPosition = String.Format("LButtonDown X: {0}, Y: {1}", msgMouseEvent.m_i32CursorX, msgMouseEvent.m_i32CursorY);

						// 아래 함수 DrawTextCanvas는 스크린 좌표를 기준으로 문자열을 뷰어에 출력한다.
						// The function DrawTextCanvas displays a string on the viewer using screen coordinates.
						// 파라미터 순서 : 기준 좌표 Figure 객체 -> 문자열 -> 텍스트 색 -> 텍스트 테두리 색 -> 폰트 크기 -> 실제 크기로 출력 유무 -> 각도 -> 정렬 -> 폰트 이름 -> 텍스트 알파값(불투명도) -> 텍스트 테두리 알파값 (불투명도) -> 폰트 두께 -> 폰트 이탤릭 여부
						// Parameter order: reference coordinate (Figure object) -> text string -> text color -> text outline color -> font size -> render in real-world size (bool) -> angle -> alignment -> font name -> text alpha (opacity) -> text outline alpha (opacity) -> font thickness -> italic font (bool)
						layer.DrawTextCanvas(new CFLPoint<double>(10, 10), strPosition, EColor.RED, EColor.BLACK);

						// 이미지뷰를 갱신
						viewImage.Invalidate();
					}
					break;

				case (uint)EGUIBroadcast.ViewImage_PostLButtonUp:
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

						// 이미지뷰의 0번 레이어 가져오기
						CGUIViewImageLayer layer = viewImage.GetLayer(0);

						// 기존에 Layer 에 그려진 도형들을 삭제
						layer.Clear();

						// 마우스 좌표를 표시할 문자열 생성
						String strPosition = String.Format("LButtonUp X: {0}, Y: {1}", msgMouseEvent.m_i32CursorX, msgMouseEvent.m_i32CursorY);

						// 아래 함수 DrawTextCanvas는 스크린 좌표를 기준으로 문자열을 뷰어에 출력한다.
						// The function DrawTextCanvas displays a string on the viewer using screen coordinates.
						// 파라미터 순서 : 기준 좌표 Figure 객체 -> 문자열 -> 텍스트 색 -> 텍스트 테두리 색 -> 폰트 크기 -> 실제 크기로 출력 유무 -> 각도 -> 정렬 -> 폰트 이름 -> 텍스트 알파값(불투명도) -> 텍스트 테두리 알파값 (불투명도) -> 폰트 두께 -> 폰트 이탤릭 여부
						// Parameter order: reference coordinate (Figure object) -> text string -> text color -> text outline color -> font size -> render in real-world size (bool) -> angle -> alignment -> font name -> text alpha (opacity) -> text outline alpha (opacity) -> font thickness -> italic font (bool)
						layer.DrawTextCanvas(new CFLPoint<double>(10, 10), strPosition, EColor.BLUE, EColor.BLACK);

						// 이미지뷰를 갱신
						viewImage.Invalidate();
					}
					break;
				}
			}
			while(false);
		}

        CGUIViewImage m_viewImage;
	}

	class ImageViewMessageReceiver
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
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[2];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();

            // 메세지를 전달 받을 CMessageReceiver 객체 생성 // Create 메세지를 전달 받을 CMessageReceiver object
            CMessageReceiver msgReceiver = new CMessageReceiver(ref viewImage[0]);

			do
			{
				CResult res;

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(300, 0, 300 + 520, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[1].Create(300 + 520, 0, 300 + 520 * 2, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 뷰의 시점을 동기화 한다
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

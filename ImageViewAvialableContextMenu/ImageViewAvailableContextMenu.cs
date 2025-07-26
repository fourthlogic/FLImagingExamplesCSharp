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

namespace ImageView
{
	class ImageViewAvailableContextMenu
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


				// 이미지 뷰에서 이용 가능한 컨텍스트 메뉴를 설정합니다. 
				// EAvailableViewImageContextMenu.All 이 기본값이며, 이 값으로 설정하면 모든 메뉴를 사용 가능한 상태가 됩니다.
				// 아래와 같이 EAvailableViewImageContextMenu.None 으로 설정할 경우 모든 메뉴가 비활성화됩니다.
				viewImage[0].SetAvailableViewImageContextMenu(EAvailableViewImageContextMenu.None);

				// 이미지뷰의 0번 레이어 가져오기
				CGUIViewImageLayer layer = viewImage[0].GetLayer(0);

				// 기존에 Layer 에 그려진 도형들을 삭제
				layer.Clear();

				// 안내 문자열 생성
				String strInformation = "RIGHT BUTTON CLICK ON MOUSE AND SEE THE CONTEXT MENU\n";
				String strInformation2 = "Option : EAvailableViewImageContextMenu.None";

				// 아래 함수 DrawTextCanvas는 스크린 좌표를 기준으로 문자열을 뷰어에 출력한다.
				// The function DrawTextCanvas displays a string on the viewer using screen coordinates.
				// 파라미터 순서 : 기준 좌표 Figure 객체 -> 문자열 -> 텍스트 색 -> 텍스트 테두리 색 -> 폰트 크기 -> 실제 크기로 출력 유무 -> 각도 -> 정렬 -> 폰트 이름 -> 텍스트 알파값(불투명도) -> 텍스트 테두리 알파값 (불투명도) -> 폰트 두께 -> 폰트 이탤릭 여부
				// Parameter order: reference coordinate (Figure object) -> text string -> text color -> text outline color -> font size -> render in real-world size (bool) -> angle -> alignment -> font name -> text alpha (opacity) -> text outline alpha (opacity) -> font thickness -> italic font (bool)
				layer.DrawTextCanvas(new CFLPoint<double>(10, 10), strInformation, EColor.LIME, EColor.BLACK, 15);
				layer.DrawTextCanvas(new CFLPoint<double>(10, 30), strInformation2, EColor.CYAN, EColor.BLACK, 15);


				// 이미지 뷰에서 이용 가능한 컨텍스트 메뉴를 설정합니다. 
				// EAvailableViewImageContextMenu.All 이 기본값이며, 이 값으로 설정하면 모든 메뉴를 사용 가능한 상태가 됩니다.
				// 아래와 같이 여러 조합을 이용하여 설정할 수 있으며, 
				// EAvailableViewImageContextMenu.All & ~(EAvailableViewImageContextMenu.Load | EAvailableViewImageContextMenu.ClearFile | EAvailableViewImageContextMenu.Save | EAvailableViewImageContextMenu.CreateImage) 으로 설정할 경우 
				// 파일 열기, 닫기, 저장, 이미지 생성 메뉴가 비활성화됩니다.
				viewImage[1].SetAvailableViewImageContextMenu(EAvailableViewImageContextMenu.All & ~(EAvailableViewImageContextMenu.Load | EAvailableViewImageContextMenu.ClearFile | EAvailableViewImageContextMenu.Save | EAvailableViewImageContextMenu.CreateImage));


				// 이미지뷰의 0번 레이어 가져오기
				layer = viewImage[1].GetLayer(0);

				// 기존에 Layer 에 그려진 도형들을 삭제
				layer.Clear();

				// 안내 문자열 지정
				strInformation = "RIGHT BUTTON CLICK ON MOUSE AND SEE THE CONTEXT MENU\n";
				strInformation2 = "Option: EAvailableViewImageContextMenu.All & \n           ~(EAvailableViewImageContextMenu.Load | \n              EAvailableViewImageContextMenu.ClearFile | \n              EAvailableViewImageContextMenu.Save | \n              EAvailableViewImageContextMenu.CreateImage)";

				// 아래 함수 DrawTextCanvas는 스크린 좌표를 기준으로 문자열을 뷰어에 출력한다.
				// The function DrawTextCanvas displays a string on the viewer using screen coordinates.
				// 파라미터 순서 : 기준 좌표 Figure 객체 -> 문자열 -> 텍스트 색 -> 텍스트 테두리 색 -> 폰트 크기 -> 실제 크기로 출력 유무 -> 각도 -> 정렬 -> 폰트 이름 -> 텍스트 알파값(불투명도) -> 텍스트 테두리 알파값 (불투명도) -> 폰트 두께 -> 폰트 이탤릭 여부
				// Parameter order: reference coordinate (Figure object) -> text string -> text color -> text outline color -> font size -> render in real-world size (bool) -> angle -> alignment -> font name -> text alpha (opacity) -> text outline alpha (opacity) -> font thickness -> italic font (bool)
				layer.DrawTextCanvas(new CFLPoint<double>(10, 10), strInformation, EColor.LIME, EColor.BLACK, 15);
				layer.DrawTextCanvas(new CFLPoint<double>(10, 30), strInformation2, EColor.CYAN, EColor.BLACK, 15);

				for(int i = 0; i < 2; i++)
					viewImage[i].Invalidate();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

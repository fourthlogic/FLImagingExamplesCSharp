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

				// 뷰의 시점을 동기화 한다. // Synchronizes the view point.
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
				// 기본값은 모든 메뉴가 사용 가능한 상태이며,
				// 아래와 같이 EnableAvailableViewImageContextMenuAll(false) 를 호출하면 모든 메뉴가 비활성화됩니다.
				// Sets the context menus available in the image view.
				// By default all context menu items are enabled.
				// As shown below, calling EnableAvailableViewImageContextMenuAll(false) disables all context menu items.
				viewImage[0].EnableAvailableViewImageContextMenuAll(false);

				// 이미지뷰의 0번 레이어 가져오기 // Retrieves layer 0 from the image view.
				CGUIViewImageLayer layer = viewImage[0].GetLayer(0);
				// 기존에 Layer 에 그려진 도형들을 삭제 // Clear all figures previously drawn on the layer.
				layer.Clear();

				// 안내 문자열 생성 // Creates a guidance message.
				String strInformation = "RIGHT BUTTON CLICK ON MOUSE AND SEE THE CONTEXT MENU\n";
				String strInformation2 = "Option : EnableAvailableViewImageContextMenuAll(false)";

				// 아래 함수 DrawTextCanvas는 스크린 좌표를 기준으로 문자열을 뷰어에 출력한다.
				// The function DrawTextCanvas displays a string on the viewer using screen coordinates.
				// 파라미터 순서 : 기준 좌표 Figure 객체 -> 문자열 -> 텍스트 색 -> 텍스트 테두리 색 -> 폰트 크기 -> 실제 크기로 출력 유무 -> 각도 -> 정렬 -> 폰트 이름 -> 텍스트 알파값(불투명도) -> 텍스트 테두리 알파값 (불투명도) -> 폰트 두께 -> 폰트 이탤릭 여부
				// Parameter order: reference coordinate (Figure object) -> text string -> text color -> text outline color -> font size -> render in real-world size (bool) -> angle -> alignment -> font name -> text alpha (opacity) -> text outline alpha (opacity) -> font thickness -> italic font (bool)
				layer.DrawTextCanvas(new CFLPoint<double>(10, 10), strInformation, EColor.LIME, EColor.BLACK, 15);
				layer.DrawTextCanvas(new CFLPoint<double>(10, 30), strInformation2, EColor.CYAN, EColor.BLACK, 15);

				// 이미지 뷰에서 이용 가능한 컨텍스트 메뉴를 설정합니다.
				// 기본값은 모든 메뉴가 사용 가능한 상태이며, EnableAvailableViewImageContextMenuAll(true) 로 언제든 전체 활성화할 수 있습니다.
				// 아래와 같이 EMenuItem 리스트를 만들어 RemoveAvailableViewImageContextMenu 를 호출하면
				// 전달한 항목들만 비활성화됩니다.
				// 아래 예제에서는 파일 열기, 닫기, 저장, 이미지 생성 관련 메뉴가 비활성화됩니다.
				// Sets the context menus available in the image view.
				// By default all context menu items are enabled; call EnableAvailableViewImageContextMenuAll(true) to enable them all at any time.
				// As shown below, build an EMenuItem list and pass it to RemoveAvailableViewImageContextMenu
				// to disable only the items you list.
				// The example below disables the Open File, Close File, Save, and Create Image menu items.
				var listRemoveMenu = new List<EMenuItem>
				{
					// Load
					EMenuItem.LoadFile,
					EMenuItem.LoadFile_Raw,
					EMenuItem.LoadFolder,
					EMenuItem.AppendFile,
					EMenuItem.InsertFile,
					EMenuItem.AppendFolder,
					EMenuItem.InsertFolder,

					// ClearFile
					EMenuItem.ClearFile,
					EMenuItem.ClearSelectedPage,

					// Save
					EMenuItem.Save,
					EMenuItem.SavePages,
					EMenuItem.SaveCurrentPage,
					EMenuItem.SaveCurrentPageWithLayers,

					// CreateImage
					EMenuItem.CreateImage,
					EMenuItem.InsertPage,
					EMenuItem.AppendPage,
				};

				viewImage[1].EnableAvailableViewImageContextMenuAll(true); // 전체 메뉴 활성화 // Enable all menu items
				viewImage[1].RemoveAvailableViewImageContextMenu(listRemoveMenu);

				// 이미지뷰의 0번 레이어 가져오기 // Retrieves layer 0 from the image view.
				layer = viewImage[1].GetLayer(0);

				// 기존에 Layer 에 그려진 도형들을 삭제 // Clear all figures previously drawn on the layer.
				layer.Clear();

				// 안내 문자열 지정 // Sets the guidance message.
				strInformation = "RIGHT BUTTON CLICK ON MOUSE AND SEE THE CONTEXT MENU\n";
				strInformation2 = "Option: RemoveAvailableViewImageContextMenu\n           (LoadFile, LoadFile_Raw, LoadFolder\n           ClearFile, Save, CreateImage, ...)";
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

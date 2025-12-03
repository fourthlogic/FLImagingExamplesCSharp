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

using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
	class RandomPageShuffle
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

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliSrcAfterImage = new CFLImage();
			CFLImage fliSrcBeforeImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageAfterSrc = new CGUIViewImage();
			CGUIViewImage viewImageBeforeSrc = new CGUIViewImage();

			do
			{
				CResult res;

				// 알고리즘을 수행 할 Source 이미지 로드 // Load Source image to execute algorithm
				if((res = fliSrcAfterImage.Load("../../ExampleImages/RandomPageShuffle/Landscape.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 알고리즘 수행 결과 비교를 위한 Source 이미지 로드 // Load Source image for Comparing algorithm result
				if((res = fliSrcBeforeImage.Load("../../ExampleImages/RandomPageShuffle/Landscape.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// After 이미지 뷰 생성 // Create After image view
				if((res = viewImageAfterSrc.Create(912, 0, 1424, 612)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageAfterSrc.SetImagePtr(ref fliSrcAfterImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Before 이미지 뷰 생성 // Create the Before image view
				if((res = viewImageBeforeSrc.Create(400, 0, 912, 612)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Before 이미지 뷰에 이미지를 디스플레이 // Display the image in the Before image view
				if((res = viewImageBeforeSrc.SetImagePtr(ref fliSrcBeforeImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = viewImageAfterSrc.SynchronizeWindow(ref viewImageBeforeSrc)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageAfterSrc.SynchronizePointOfView(ref viewImageBeforeSrc)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰의 페이지 색인을 동기화 한다 // Synchronize the page index of the two image views
				if((res = viewImageAfterSrc.SynchronizePageIndex(ref viewImageBeforeSrc)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageAfterSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// Random Page Shuffle 객체 생성 // Create Random Page Shuffle object
				CRandomPageShuffle randomPageShuffle = new CRandomPageShuffle();

				// 처리할 이미지 설정 // Set the image to process
				randomPageShuffle.SetSourceImage(ref fliSrcAfterImage);

				// 랜덤하게 섞을 페이지의 시작 인덱스와 페이지 개수 설정 // Set the start page index and  page counts to shuffle pages
				randomPageShuffle.SetSelection(0, 5);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = randomPageShuffle.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Random Page Shuffle.");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageBeforeSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerAfterSrc = viewImageAfterSrc.GetLayer(0);
				CGUIViewImageLayer layerBeforeSrc = viewImageBeforeSrc.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerAfterSrc.Clear();
				layerBeforeSrc.Clear();

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layerBeforeSrc.DrawTextCanvas(flp, ("Source Image Before Shuffle"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerAfterSrc.DrawTextCanvas(flp, ("Source Image After Shuffle"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageAfterSrc.Invalidate(true);
				viewImageBeforeSrc.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageAfterSrc.IsAvailable() && viewImageBeforeSrc.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

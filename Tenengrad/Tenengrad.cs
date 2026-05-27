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
	class Tenengrad
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
			CFLImage fliSrcImage1 = new CFLImage();
			CFLImage fliSrcImage2 = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc1 = new CGUIViewImage();
			CGUIViewImage viewImageSrc2 = new CGUIViewImage();

			do
			{
				CResult res;

				// Source 이미지 로드 // Load the source image
				if((res = fliSrcImage1.Load("../../ExampleImages/FocusMeasurement/Focus1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliSrcImage2.Load("../../ExampleImages/FocusMeasurement/Focus2.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}


				// Source 이미지 뷰 생성 // Create the source image view
				if((res = viewImageSrc1.Create(400, 0, 912, 612)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageSrc2.Create(912, 0, 1424, 612)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImageSrc1.SetImagePtr(ref fliSrcImage1)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImageSrc2.SetImagePtr(ref fliSrcImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc1.SynchronizePointOfView(ref viewImageSrc2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = viewImageSrc1.SynchronizeWindow(ref viewImageSrc2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Tenengrad 객체 생성 // Create Tenengrad object
				CTenengrad tenengrad = new CTenengrad();

				// Source 이미지 1 설정 // Set the source1 image
				tenengrad.SetSourceImage(ref fliSrcImage1);

				//Threshold 설정 //Set Threshold
				tenengrad.SetThreshold(5.0);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = tenengrad.Execute()).IsFail())
				{
                    ErrorPrint(res, "Failed to execute tenengrad.");
					break;
				}

				// 결과 점수 획득 // Get Result Score
				double f64Score1 = tenengrad.GetResultScore();

				// Source 이미지 2 설정 // Set the source2 image
				tenengrad.SetSourceImage(ref fliSrcImage2);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = tenengrad.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute tenengrad.");
					break;
				}

				// 결과 점수 획득 // Get Result Score
				double f64Score2 = tenengrad.GetResultScore();

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc1 = viewImageSrc1.GetLayer(0);
				CGUIViewImageLayer layerSrc2 = viewImageSrc2.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc1.Clear();
				layerSrc2.Clear();

				// 점수를 디스플레이 한다. // Display score
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				
				if((res = layerSrc1.DrawTextCanvas(new CFLPoint<double>(0, 0), "Score : " + f64Score1, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerSrc2.DrawTextCanvas(new CFLPoint<double>(0, 0), "Score : " + f64Score2, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc1.Invalidate(true);
				viewImageSrc2.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc1.IsAvailable() && viewImageSrc2.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
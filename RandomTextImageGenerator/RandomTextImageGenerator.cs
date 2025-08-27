using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;
using FLImagingCLR.ThreeDim;
using FLImagingCLR.AI;
using System.Net.NetworkInformation;

namespace FLImagingExamplesCSharp
{
	class RandomTextImageGenerator
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
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliResultImage1 = new CFLImage();
			CFLImage fliResultImage2 = new CFLImage();

			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageResult1 = new CGUIViewImage();
			CGUIViewImage viewImageResult2 = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 라이브러리가 완전히 로드 될 때까지 기다림 // Wait for the library to fully load
				Thread.Sleep(1000);

				// 이미지 로드 // Load image
                if ((res = fliSourceImage.Load("../../ExampleImages/Filter/SilverGrass.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSource.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageResult1.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageResult2.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageResult1.SetImagePtr(ref fliResultImage1)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageResult2.SetImagePtr(ref fliResultImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 두 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSource.SynchronizeWindow(ref viewImageResult1)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageResult2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerResult1 = viewImageResult1.GetLayer(0);
				CGUIViewImageLayer layerResult2 = viewImageResult2.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerResult1.Clear();
				layerResult2.Clear();
		
				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = layerLearn.DrawTextCanvas(flpPoint, "ORIGINAL", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}
				
				if((res = layerResult1.DrawTextCanvas(flpPoint, "RESULT (String)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResult2.DrawTextCanvas(flpPoint, "RESULT (Character)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// CRandomTextImageGeneratorDL 객체 생성 // Create CRandomTextImageGeneratorDL object
				CRandomTextImageGeneratorDL randomTextImageGeneratorDL = new CRandomTextImageGeneratorDL();
				
				// Source 이미지 설정 // Set source image
				randomTextImageGeneratorDL.SetSourceImage(ref fliSourceImage);
				// Destination 이미지 설정 // Set destination image
				randomTextImageGeneratorDL.SetDestinationImage(ref fliResultImage1);

				// 파라미터 값 설정 // Set parameter value
				randomTextImageGeneratorDL.SetFontSize(20.0f, 60.0f);
				randomTextImageGeneratorDL.SetTextCount(3, 10);
				randomTextImageGeneratorDL.SetRotationAngle(-30.0, 30.0);

				// RandomTextImageGenerator 실행 // RandomTextImageGenerator Execute 
				if((res = randomTextImageGeneratorDL.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to process\n");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				randomTextImageGeneratorDL.SetDestinationImage(ref fliResultImage2);

				// 파라미터 값 설정 // Set parameter value
				randomTextImageGeneratorDL.SetLabelFigureType(CRandomTextImageGeneratorDL.ELabelFigureType.Character);

				// RandomTextImageGenerator 실행 // RandomTextImageGenerator Execute 
				if((res = randomTextImageGeneratorDL.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to process\n");
					break;
				}

				// 두 개의 이미지 뷰 윈도우의 페이지 인덱스를 동기화 한다 // Synchronize the page index of the two image view windows
				if((res = viewImageSource.SynchronizePageIndex(ref viewImageResult1)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index. \n");
					break;
				}

				if((res = viewImageSource.SynchronizePageIndex(ref viewImageResult2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index. \n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageSource.ZoomFit();
				viewImageResult1.ZoomFit();
				viewImageResult2.ZoomFit();
				viewImageSource.RedrawWindow();
				viewImageResult1.RedrawWindow();
				viewImageResult2.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageResult1.IsAvailable() && viewImageResult2.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

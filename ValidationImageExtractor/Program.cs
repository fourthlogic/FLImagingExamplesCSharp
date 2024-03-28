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

namespace SemanticSegmentation
{
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
			// 이미지 객체 선언 // Declare the image object
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliResultLearnImage = new CFLImage();
			CFLImage fliResultValidationImage = new CFLImage();

			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageResultLearn = new CGUIViewImage();
			CGUIViewImage viewImageResultValidation = new CGUIViewImage();

			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((eResult = fliSourceImage.Load("../../ExampleImages/SemanticSegmentation/Train.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file. \n");
					break;
				}

				if((eResult = fliResultLearnImage.Load("../../ExampleImages/SemanticSegmentation/Validation.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImageSource.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view. \n");
					break;
				}

				if((eResult = viewImageResultLearn.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				if((eResult = viewImageResultValidation.Create(1100, 0, 1700, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}


				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((eResult = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				if((eResult = viewImageResultLearn.SetImagePtr(ref fliResultLearnImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				if((eResult = viewImageResultValidation.SetImagePtr(ref fliResultValidationImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				// 세 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the three image view windows
				if((eResult = viewImageSource.SynchronizeWindow(ref viewImageResultLearn)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				if((eResult = viewImageSource.SynchronizeWindow(ref viewImageResultValidation)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerValidation = viewImageResultLearn.GetLayer(0);
				CGUIViewImageLayer layerResultLabel = viewImageResultValidation.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerValidation.Clear();
				layerResultLabel.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((eResult = layerLearn.DrawTextCanvas(flpPoint, "SOURCE", EColor.RED, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}
				
				if((eResult = layerValidation.DrawTextCanvas(flpPoint, "TRAIN(EXTRACTED (4/6))", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}

				if((eResult = layerResultLabel.DrawTextCanvas(flpPoint, "VALIDATION(EXTRACTED (2/6))", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}

				// Validation Image 비율 설정 // Set ratio of validation image
				float f32Ratio = 0.4f;
				// Dataset type 설정 // Set the data set type
				CValidationImageExtractorDL.EDatasetType eDatasetType = CValidationImageExtractorDL.EDatasetType.SemanticSegmentation;
				// Validation Set에 최소한 몇 개의 클래스가 1개 이상 씩 포함될 것인지 설정 // Set how many classes each will be included in the Validation Set
				int i32MinimumClassIncluded = 2;

				if((eResult = CValidationImageExtractorDL.Extract(ref fliSourceImage, f32Ratio, eDatasetType, out fliResultLearnImage, out fliResultValidationImage, i32MinimumClassIncluded)).IsFail())
				{
					ErrorPrint(eResult, "Failed to process\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageResultLearn.ZoomFit();
				viewImageResultValidation.ZoomFit();
				viewImageSource.RedrawWindow();
				viewImageResultLearn.RedrawWindow();
				viewImageResultValidation.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageResultLearn.IsAvailable() && viewImageResultValidation.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

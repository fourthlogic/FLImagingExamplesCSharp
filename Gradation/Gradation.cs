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

namespace Gradation
{
	class Gradation
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
			CFLImage fliISrcImage = new CFLImage();
			CFLImage fliIDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[2];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/Gradation/House.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(300, 0, 300 + 520, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(300 + 520, 0, 300 + 520 * 2, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[0].SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[1].SetImagePtr(ref fliIDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Gradation  객체 생성 // Create Gradation object
				CGradation Gradation = new CGradation();

				// Source 이미지 설정 // Set source image 
				Gradation.SetSourceImage(ref fliISrcImage);

				// Destination 이미지 설정 // Set destination image
				Gradation.SetDestinationImage(ref fliIDstImage);

				// 시작 Alpha 값 설정 // Set start alpha value
				CMultiVar<double> mvStartAlpha = new CMultiVar<double>(0, 0, 0);
				Gradation.SetStartAlpha(mvStartAlpha);

				// 끝 Alpha 값 설정 // Set end alpha value
				CMultiVar<double> mvEndAlpha = new CMultiVar<double>(0.1, 0.6, 0.9);
				Gradation.SetEndAlpha(mvEndAlpha);

				// Gradation Start Value 설정(3Ch) // Set Gradation Start Value(3Ch)
				CMultiVar<double> mvStartValue = new CMultiVar<double>(255, 0, 0);
				Gradation.SetStartValue(mvStartValue);

				// Gradation End Value 설정(3Ch) // Set Gradation End Value(3Ch)
				CMultiVar<double> mvEndValue = new CMultiVar<double>(0, 0, 255);
				Gradation.SetEndValue(mvEndValue);

				// Gradation Vector Figure 객체 // Gradation Vector Figure object
				CFLLine<double> fllVector = new CFLLine<double>();
				fllVector.Load("../../ExampleImages/Gradation/Vector.fig");
				Gradation.SetVector(fllVector);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (Gradation.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Gradation.");
					break;
				}

				// 레이어는 따로 해제하지 않아도 View가 해제 될 때 같이 해제된다. // The layer is released together when View is released without releasing it separately.
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);

				// Draw Figure 객체 // Gradation Vector Figure object
				CFLFigureArray fllDrawVector = new CFLFigureArray();
				fllDrawVector.Load("../../ExampleImages/Gradation/DrawVector.fig");

				CFLRect<double> fllRect1 = new CFLRect<double>();
				CFLRect<double> fllRect2 = new CFLRect<double>();

				fllRect1.left = fllVector.flpPoints[0].x - 15;
				fllRect1.top = fllVector.flpPoints[0].y - 15;
				fllRect1.right = fllVector.flpPoints[0].x + 15;
				fllRect1.bottom = fllVector.flpPoints[0].y + 15;

				fllRect2.left = fllVector.flpPoints[1].x - 15;
				fllRect2.top = fllVector.flpPoints[1].y - 15;
				fllRect2.right = fllVector.flpPoints[1].x + 15;
				fllRect2.bottom = fllVector.flpPoints[1].y + 15;

				if((res = layer1.DrawFigureImage(fllRect1, EColor.BLUE, 5, EColor.BLUE, EGUIViewImagePenStyle.Solid)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layer1.DrawFigureImage(fllRect2, EColor.RED, 5, EColor.RED, EGUIViewImagePenStyle.Solid)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// Gradation Vector 출력 // Draw gradation vector
				if((res = layer1.DrawFigureImage(fllDrawVector, EColor.BLACK, 5)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layer1.DrawFigureImage(fllDrawVector, EColor.LIME, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// text를 출력합니다. // Display text.
				if((res = layer1.DrawTextImage(fllVector.flpPoints[0], "Start Value(255, 0, 0)/Start Alpha(0, 0, 0)", EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.RIGHT)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layer1.DrawTextImage(fllVector.flpPoints[1], "End(0, 0, 255)/Start Alpha(0.1, 0.6, 0.9)", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// Text 출력 // Display Text
				if((res = layer1.DrawTextImage(new CFLPoint<double>(50, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(new CFLPoint<double>(0, 0), "Gradation Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

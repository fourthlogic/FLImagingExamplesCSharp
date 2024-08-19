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

namespace RadialGradation
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
				if((res = fliISrcImage.Load("../../ExampleImages/RadialGradation/Moon.flif")).IsFail())
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
				if((res = viewImage[0].SynchronizePointOfView(viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(viewImage[1])).IsFail())
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

				// Load Radial Gradation Region 객체 // Load Radial Gradation Region Figure object
				CFLCircle<double> flcRadialRegion = new CFLCircle<double>();
				flcRadialRegion.Load("../../ExampleImages/RadialGradation/RadialRegion.fig");

				// RadialGradation  객체 생성 // Create RadialGradation object
				CRadialGradation RadialGradation = new CRadialGradation();

				// Source 이미지 설정 // Set source image 
				RadialGradation.SetSourceImage(ref fliISrcImage);

				// Destination 이미지 설정 // Set destination image
				RadialGradation.SetDestinationImage(ref fliIDstImage);

				// Source ROI 설정 // Set source ROI 
				RadialGradation.SetSourceROI(flcRadialRegion);

				// 시작 Alpha 값 설정 // Set start alpha value
				CMultiVar<double> mvStartAlpha = new CMultiVar<double>(1.0, 0.3, 0.3);
				RadialGradation.SetStartAlpha(mvStartAlpha);

				// 끝 Alpha 값 설정 // Set start alpha value
				CMultiVar<double> mvEndAlpha = new CMultiVar<double>(0.1, 0.5, 0.5);
				RadialGradation.SetEndAlpha(mvEndAlpha);

				// RadialGradation Start Value 설정(3Ch) // Set RadialGradation Start Value(3Ch)
				CMultiVar<double> mvStartValue = new CMultiVar<double>(0, 0, 0);
				RadialGradation.SetStartValue(mvStartValue);

				// RadialGradation End Value 설정(3Ch) // Set RadialGradation End Value(3Ch)
				CMultiVar<double> mvEndValue = new CMultiVar<double>(100, 255, 255);
				RadialGradation.SetEndValue(mvEndValue);

				// RadialGradation Region 설정 // Set RadialGradation Region 
				RadialGradation.SetRadialRegion(flcRadialRegion);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (RadialGradation.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute RadialGradation.");
					break;
				}

				// 레이어는 따로 해제하지 않아도 View가 해제 될 때 같이 해제된다. // The layer is released together when View is released without releasing it separately.
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);

				// Draw Figure 객체 // RadialGradation Vector Figure object
				CFLFigureArray flfaDrawArrow;
				CFLLine<double> fllArrow = new CFLLine<double>();
				CFLPoint<double> flpCenter = new CFLPoint<double>();

				flpCenter.Set(flcRadialRegion.GetCenter());
				flpCenter.y += flcRadialRegion.radius - 10;
				fllArrow.flpPoints[0].Set(flcRadialRegion.GetCenter());
				fllArrow.flpPoints[1].Set(flcRadialRegion.GetCenter());
				fllArrow.flpPoints[1].y += flcRadialRegion.radius;
				flfaDrawArrow = fllArrow.MakeArrowWithLength(5);

				// Arrow Figure 를 출력합니다. // Display Arrow Figure.
				if((res = layer1.DrawFigureImage(flfaDrawArrow, EColor.RED, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw Figure.\n");
					break;
				}

				// text를 출력합니다. // Display text.
				if((res = layer1.DrawTextImage(flcRadialRegion.GetCenter(), "Start Value(255, 0, 0)\nStart Alpha(1.0, 0.3, 0.3)", EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layer1.DrawTextImage(flpCenter, "End Value(100, 255, 255)\nEnd Alpha(0.1, 0.5, 0.5)", EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// Text 출력 // Display Text
				if((res = layer1.DrawTextImage(new CFLPoint<double>(50, 0), "Source Image", EColor.RED)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(new CFLPoint<double>(50, 0), "Destination Image", EColor.RED)).IsFail())
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

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

namespace Fit_Circle
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
			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

			CResult res;

			do
			{
				// View 1 생성 // Create View 1
				if((res = (viewImage[0].Create(200, 0, 700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// View 2 생성 // Create View 2
				if((res = (viewImage[1].Create(700, 0, 1200, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// View 3 생성 // Create View 3
				if((res = (viewImage[2].Create(1200, 0, 1700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 각 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each image view.
				if((res = (viewImage[0].SynchronizePointOfView(ref viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				if((res = (viewImage[1].SynchronizePointOfView(ref viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = (viewImage[0].SynchronizeWindow(ref viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}
				if((res = (viewImage[1].SynchronizeWindow(ref viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] layer = { viewImage[0].GetLayer(0), viewImage[1].GetLayer(0), viewImage[2].GetLayer(0) };

				// 화면상 좌표(고정 좌표)에 View 의 이름을 표시
				// Indicates view name on screen coordinates (fixed coordinates)
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 0), "Default", EColor.YELLOW, EColor.BLACK, 30);
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 0), "Parameter 1", EColor.YELLOW, EColor.BLACK, 30);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 0), "Parameter 2", EColor.YELLOW, EColor.BLACK, 30);

				// (x, y) = (250, 250) , r = 150 원 생성 // (x, y) = (250, 250) , r = 150 Create circle
				CFLCircle<double> flc = new CFLCircle<double>(250, 250, 150);
				CFLPointArray flpaSrc = new CFLPointArray();
				// 원 모양의 PointArray 설정 // Set a circle-shaped PointArray
				flpaSrc.Set(flc);

				// Noise 가 추가된 PointArray 생성 // Create a PointArray with noise added
				CFLPointArray flpaNoise = new CFLPointArray();
				double f64Epsilon = 10.0;

				for(long i = 0; i < flpaSrc.GetCount(); ++i)
				{
					double f64RandomVal = CRandomGenerator.Double(-f64Epsilon, f64Epsilon);
					flpaNoise.PushBack(new CFLPoint<double>(flpaSrc.GetAt(i).x + f64RandomVal, flpaSrc.GetAt(i).y + f64RandomVal));
				}


				CFLCircle<double> flcResult1 = new CFLCircle<double>();
				long i64OutlierThresholdCount1 = 0;

				// Fit 함수 실행 (Default parameter) // Fit function execution (Default parameter)
				if((res = flcResult1.Fit(flpaNoise)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// 0번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 0
				layer[0].DrawFigureImage(flcResult1, EColor.BLACK, 5);
				layer[0].DrawFigureImage(flcResult1, EColor.CYAN, 3);
				layer[0].DrawFigureImage(flpaNoise, EColor.BLACK, 3);
				layer[0].DrawFigureImage(flpaNoise, EColor.LIME, 1);
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 40), String.Format("Outlier Threshold Count : {0}", i64OutlierThresholdCount1), EColor.YELLOW, EColor.BLACK, 15);
				layer[0].DrawTextImage(flcResult1.GetCenter(), String.Format("Center : ({0:.000}, {1:.000})\r\nRadius : {2:.000}", flcResult1.GetCenter().x, flcResult1.GetCenter().y, flcResult1.GetRadius()), EColor.YELLOW, EColor.BLACK, 13, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);


				CFLCircle<double> flcResult2 = new CFLCircle<double>();
				long i64OutlierThresholdCount2 = 1;
				double f64OutlierThreshold2 = 2.0;
				List<long> listOutlierIndices2;
				CFLPointArray flpaOutlier2 = new CFLPointArray();

				// Fit 함수 실행 (Parameter1) // Fit function execution (Parameter1)
				if((res = flcResult2.Fit(flpaNoise, i64OutlierThresholdCount2, f64OutlierThreshold2, out listOutlierIndices2)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Outlier 인덱스로 Outlier PointArray 추가 // Add Outlier PointArray as Outlier Index
				for(int i = 0; i < listOutlierIndices2.Count; ++i)
					flpaOutlier2.PushBack(flpaNoise.GetAt(listOutlierIndices2[i]));

				// 1번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 1
				layer[1].DrawFigureImage(flcResult2, EColor.BLACK, 5);
				layer[1].DrawFigureImage(flcResult2, EColor.CYAN, 3);
				layer[1].DrawFigureImage(flpaNoise, EColor.BLACK, 3);
				layer[1].DrawFigureImage(flpaNoise, EColor.LIME, 1);
				layer[1].DrawFigureImage(flpaOutlier2, EColor.RED, 1);
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 40), String.Format("Outlier Threshold Count : {0}\r\nOutlier Threshold : {1:.000}", i64OutlierThresholdCount2, f64OutlierThreshold2), EColor.YELLOW, EColor.BLACK, 15);
				layer[1].DrawTextImage(flcResult2.GetCenter(), String.Format("Center : ({0:.000}, {1:.000})\r\nRadius : {2:.000}", flcResult2.GetCenter().x, flcResult2.GetCenter().y, flcResult2.GetRadius()), EColor.YELLOW, EColor.BLACK, 13, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);

				CFLCircle<double> flcResult3 = new CFLCircle<double>();
				long i64OutlierThresholdCount3 = 3;
				double f64OutlierThreshold3 = 1.0;
				List<long> listOutlierIndices3;
				CFLPointArray flpaOutlier3 = new CFLPointArray();

				// Fit 함수 실행 (Parameter2) // Fit function execution (Parameter2)
				if((res = flcResult3.Fit(flpaNoise, i64OutlierThresholdCount3, f64OutlierThreshold3, out listOutlierIndices3)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Outlier 인덱스로 Outlier PointArray 추가 // Add Outlier PointArray as Outlier Index
				for(int i = 0; i < listOutlierIndices3.Count(); ++i)
					flpaOutlier3.PushBack(flpaNoise.GetAt(listOutlierIndices3[i]));

				// 2번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 2
				layer[2].DrawFigureImage(flcResult3, EColor.BLACK, 5);
				layer[2].DrawFigureImage(flcResult3, EColor.CYAN, 3);
				layer[2].DrawFigureImage(flpaNoise, EColor.BLACK, 3);
				layer[2].DrawFigureImage(flpaNoise, EColor.LIME, 1);
				layer[2].DrawFigureImage(flpaOutlier3, EColor.RED, 1);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 40), String.Format("Outlier Threshold Count : {0}\r\nOutlier Threshold : {1:.000}", i64OutlierThresholdCount3, f64OutlierThreshold3), EColor.YELLOW, EColor.BLACK, 15);
				layer[2].DrawTextImage(flcResult3.GetCenter(), String.Format("Center : ({0:.000}, {1:.000})\r\nRadius : {2:.000}", flcResult3.GetCenter().x, flcResult3.GetCenter().y, flcResult3.GetRadius()), EColor.YELLOW, EColor.BLACK, 13, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);


				// Console 출력 // Console output
				Console.Write("Source Points (Without noise)\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaSrc));

				Console.Write("Source Points (With noise)\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaNoise));

				Console.Write("[Default parameter]\n");
				Console.Write("Outlier Threshold Count : {0}\n", i64OutlierThresholdCount1);
				Console.Write("Result Circle : \n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcResult1));

				Console.Write("[Parameter 1]\n");
				Console.Write("Outlier Threshold Count : {0}\n", i64OutlierThresholdCount2);
				Console.Write("Outlier Threshold : {0:.000}\n", f64OutlierThreshold2);
				Console.Write("Result Circle : \n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcResult2));

				Console.Write("[Parameter 2]\n");
				Console.Write("Outlier Threshold Count : {0}\n", i64OutlierThresholdCount3);
				Console.Write("Outlier Threshold : {0:.000}\n", f64OutlierThreshold3);
				Console.Write("Result Circle : \n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcResult3));


				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 3; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 셋중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the three image views are turned off
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

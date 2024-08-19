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

namespace Fit_Line
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
				if((res = (viewImage[0].SynchronizePointOfView(viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				if((res = (viewImage[1].SynchronizePointOfView(viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = (viewImage[0].SynchronizeWindow(viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}
				if((res = (viewImage[1].SynchronizeWindow(viewImage[2]))).IsFail())
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

				// (x1, y1) = (100, 400), (x2, y2) = (400, 100)  선 생성
				// (x1, y1) = (100, 400), (x2, y2) = (400, 100)  Create Line
				CFLLine<double> fll = new CFLLine<double>(100, 400, 400, 100);

				// 선 위의 점들을 추출 // Sample points on a line
				CFLFigureArray flfaSrc;
				if((res = fll.GetSamplingPointsOnSegment(10, out flfaSrc)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Noise 가 추가된 PointArray 생성 // Create a PointArray with noise added
				CFLPointArray flpaNoise = new CFLPointArray();
				double f64Epsilon = 10.0;

				for(long i = 0; i < flfaSrc.GetCount(); ++i)
				{
					double f64RandomVal = CRandomGenerator.Double(-f64Epsilon, f64Epsilon);
					flpaNoise.PushBack(new CFLPoint<double>(flfaSrc.GetAt(i).GetCenter().x + f64RandomVal, flfaSrc.GetAt(i).GetCenter().y + f64RandomVal));
				}


				CFLLine<double> fllResult1 = new CFLLine<double>();
				long i64OutlierThresholdCount1 = 0;

				// Fit 함수 실행 (Default parameter) // Fit function execution (Default parameter)
				if((res = fllResult1.Fit(flpaNoise)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// 0번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 0
				layer[0].DrawFigureImage(fllResult1, EColor.BLACK, 5);
				layer[0].DrawFigureImage(fllResult1, EColor.CYAN, 3);
				layer[0].DrawFigureImage(flpaNoise, EColor.BLACK, 3);
				layer[0].DrawFigureImage(flpaNoise, EColor.LIME, 1);
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 40), String.Format("Outlier Threshold Count : {0}", i64OutlierThresholdCount1), EColor.YELLOW, EColor.BLACK, 15);
				layer[0].DrawTextImage(new CFLPoint<int>(350, 350), String.Format("(x1, y1) = ({0:.000},{1:.000})\r\n(x2, y2) = ({2:.000},{3:.000})\r\nAngle = {4:.000}", fllResult1.flpPoints[0].x, fllResult1.flpPoints[0].y, fllResult1.flpPoints[1].x, fllResult1.flpPoints[1].y, fllResult1.GetAngle()), EColor.YELLOW, EColor.BLACK, 13, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);


				CFLLine<double> fllResult2 = new CFLLine<double>();
				long i64OutlierThresholdCount2 = 1;
				double f64OutlierThreshold2 = 1.7;
				List<long> listOutlierIndices2;
				CFLPointArray flpaOutlier2 = new CFLPointArray();

				// Fit 함수 실행 (Parameter1) // Fit function execution (Parameter1)
				if((res = fllResult2.Fit(flpaNoise, i64OutlierThresholdCount2, f64OutlierThreshold2, out listOutlierIndices2)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Outlier 인덱스로 Outlier PointArray 추가 // Add Outlier PointArray as Outlier Index
				for(int i = 0; i < listOutlierIndices2.Count; ++i)
					flpaOutlier2.PushBack(flpaNoise.GetAt(listOutlierIndices2[i]));

				// 1번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 1
				layer[1].DrawFigureImage(fllResult2, EColor.BLACK, 5);
				layer[1].DrawFigureImage(fllResult2, EColor.CYAN, 3);
				layer[1].DrawFigureImage(flpaNoise, EColor.BLACK, 3);
				layer[1].DrawFigureImage(flpaNoise, EColor.LIME, 1);
				layer[1].DrawFigureImage(flpaOutlier2, EColor.RED, 1);
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 40), String.Format("Outlier Threshold Count : {0}\r\nOutlier Threshold : {1:.000}", i64OutlierThresholdCount2, f64OutlierThreshold2), EColor.YELLOW, EColor.BLACK, 15);
				layer[1].DrawTextImage(new CFLPoint<int>(350, 350), String.Format("(x1, y1) = ({0:.000},{1:.000})\r\n(x2, y2) = ({2:.000},{3:.000})\r\nAngle = {4:.000}", fllResult2.flpPoints[0].x, fllResult2.flpPoints[0].y, fllResult2.flpPoints[1].x, fllResult2.flpPoints[1].y, fllResult2.GetAngle()), EColor.YELLOW, EColor.BLACK, 13, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);

				CFLLine<double> fllResult3 = new CFLLine<double>();
				long i64OutlierThresholdCount3 = 3;
				double f64OutlierThreshold3 = 1.0;
				List<long> listOutlierIndices3;
				CFLPointArray flpaOutlier3 = new CFLPointArray();

				// Fit 함수 실행 (Parameter2) // Fit function execution (Parameter2)
				if((res = fllResult3.Fit(flpaNoise, i64OutlierThresholdCount3, f64OutlierThreshold3, out listOutlierIndices3)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Outlier 인덱스로 Outlier PointArray 추가 // Add Outlier PointArray as Outlier Index
				for(int i = 0; i < listOutlierIndices3.Count(); ++i)
					flpaOutlier3.PushBack(flpaNoise.GetAt(listOutlierIndices3[i]));

				// 2번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 2
				layer[2].DrawFigureImage(fllResult3, EColor.BLACK, 5);
				layer[2].DrawFigureImage(fllResult3, EColor.CYAN, 3);
				layer[2].DrawFigureImage(flpaNoise, EColor.BLACK, 3);
				layer[2].DrawFigureImage(flpaNoise, EColor.LIME, 1);
				layer[2].DrawFigureImage(flpaOutlier3, EColor.RED, 1);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 40), String.Format("Outlier Threshold Count : {0}\r\nOutlier Threshold : {1:.000}", i64OutlierThresholdCount3, f64OutlierThreshold3), EColor.YELLOW, EColor.BLACK, 15);
				layer[2].DrawTextImage(new CFLPoint<int>(350, 350), String.Format("(x1, y1) = ({0:.000},{1:.000})\r\n(x2, y2) = ({2:.000},{3:.000})\r\nAngle = {4:.000}", fllResult3.flpPoints[0].x, fllResult3.flpPoints[0].y, fllResult3.flpPoints[1].x, fllResult3.flpPoints[1].y, fllResult3.GetAngle()), EColor.YELLOW, EColor.BLACK, 13, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);


				// Console 출력 // Console output
				Console.Write("Source Points (Without noise)\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaSrc));

				Console.Write("Source Points (With noise)\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaNoise));

				Console.Write("[Default parameter]\n");
				Console.Write("Outlier Threshold Count : {0}\n", i64OutlierThresholdCount1);
				Console.Write("Result Line : \n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(fllResult1));

				Console.Write("[Parameter 1]\n");
				Console.Write("Outlier Threshold Count : {0}\n", i64OutlierThresholdCount2);
				Console.Write("Outlier Threshold : {0:.000}\n", f64OutlierThreshold2);
				Console.Write("Result Line : \n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(fllResult2));

				Console.Write("[Parameter 2]\n");
				Console.Write("Outlier Threshold Count : {0}\n", i64OutlierThresholdCount3);
				Console.Write("Outlier Threshold : {0:.000}\n", f64OutlierThreshold3);
				Console.Write("Result Line : \n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(fllResult3));


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

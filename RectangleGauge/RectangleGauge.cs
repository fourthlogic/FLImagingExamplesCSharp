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
    class RectangleGauge
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			const int i32ExampleCount = 12;

			string[] arrViewText =
			{
				"Dark To Bright Or Bright To Dark\nBegin 0",
				"Dark To Bright Or Bright To Dark\nBegin 1",
				"Dark To Bright Or Bright To Dark\nBegin 2",
				"Dark To Bright Or Bright To Dark\nLargest Area",
				"Dark To Bright Or Bright To Dark\nEnd 0",
				"Dark To Bright Or Bright To Dark\nEnd 1",
				"Dark To Bright Or Bright To Dark\nEnd 2",
				"Dark To Bright Or Bright To Dark\nLargest Amplitude",
				"Dark To Bright Or Bright To Dark\nClosest",
				"Dark To Bright\nClosest",
				"Bright To Dark\nClosest",
				"Dark To Bright To Dark\nClosest",
			};


			CRectangleGauge.ETransitionType[] arrTransitionType =
			{
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CRectangleGauge.ETransitionType.DarkToBright,
				CRectangleGauge.ETransitionType.BrightToDark,
				CRectangleGauge.ETransitionType.DarkToBrightToDark,
			};

			CRectangleGauge.ETransitionChoice[] arrTransitionChoice =
			{
				CRectangleGauge.ETransitionChoice.Begin,
				CRectangleGauge.ETransitionChoice.Begin,
				CRectangleGauge.ETransitionChoice.Begin,
				CRectangleGauge.ETransitionChoice.LargestArea,
				CRectangleGauge.ETransitionChoice.End,
				CRectangleGauge.ETransitionChoice.End,
				CRectangleGauge.ETransitionChoice.End,
				CRectangleGauge.ETransitionChoice.LargestAmplitude,
				CRectangleGauge.ETransitionChoice.Closest,
				CRectangleGauge.ETransitionChoice.Closest,
				CRectangleGauge.ETransitionChoice.Closest,
				CRectangleGauge.ETransitionChoice.Closest,
			};

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[i32ExampleCount];

			for(int i = 0; i < i32ExampleCount; ++i)
				viewImage[i] = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/Gauge/Rect.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				for(int i = 0; i < i32ExampleCount; ++i)
				{
					int i32X = 300 * (i % 4);
					int i32Y = 300 * (i / 4);

					if((res = viewImage[i].Create(i32X, i32Y, i32X + 300, i32Y + 300)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
					if((res = viewImage[i].SetImagePtr(ref fliImage)).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}

					// 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the all image views. 
					if(i > 0)
					{
						if((res = viewImage[i].SynchronizePointOfView(ref viewImage[0])).IsFail())
						{
							ErrorPrint(res, "Failed to set image object on the image view.\n");
							break;
						}
					}
				}

				// Rectangle Gauge 객체 생성 // Create Rectangle Gauge Object
				CRectangleGauge rectangleGauge = new CRectangleGauge();

				// 처리할 이미지 설정 // Set the image to process
				rectangleGauge.SetSourceImage(ref fliImage);

				// 측정할 영역을 설정합니다. // Set the area to measure.
				CFLRect<double> measureRegion = new CFLRect<double>(213.577428,262.324155,295.020437,348.179290);
				double tolerance = 50.0;
				rectangleGauge.SetMeasurementRegion(measureRegion, tolerance);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.		
				// 사각형을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the rectangle.
				rectangleGauge.SetThreshold(20);
				// 사각형을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the rectangle.
				rectangleGauge.SetMinimumAmplitude(10);
				// 사각형을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the rectangle.
				rectangleGauge.SetThickness(1);
				// 사각형을 추정하기위해 추출할 경계점들의 추출 간격을 설정합니다. // Set the extraction interval of boundary points to be extracted to estimate the rectangle.
				rectangleGauge.SetSamplingStep(1.0);
				// 사각형을 추정하기위해 추출할 경계점들의 이상치 조정을 위한 임계값을 설정합니다. // Set the threshold value for outlier adjustment of the boundary points to be extracted to estimate the rectangle.
				rectangleGauge.SetOutliersThreshold(3.0);
				// 사각형을 추정하기위해 추출할 경계점들의 이상치 조정 횟수을 설정합니다. // Set the number of outlier adjustments for boundary points to be extracted to estimate the rectangle.
				rectangleGauge.SetOutliersThresholdCount(3);
				// 사각형을 추정하기위해 점 클러스터링 처리 유무에 대한 설정을 합니다. // Set whether or not to process point clustering to estimate the rectangle.
				rectangleGauge.EnableClusterMode(true);
				// 사각형을 추정하기위해 마진을 설정합니다. 필요에 따라 각 구역별로 설정가능합니다. // Set the margin to estimate the rectangle. It can be set for each zone as needed.
				rectangleGauge.SetMeasurementMarginRatio(0, CRectangleGauge.EMargin.All);
				// 사각형을 추정하기위한 Tolerance를 설정합니다. 필요에 따라 각 구역별로 설정가능합니다. // Set the Tolerance for estimating the rectangle. It can be set for each zone as needed.
				rectangleGauge.SetTolerance(tolerance, CRectangleGauge.ETolerance.All);

				for(int i = 0; i < i32ExampleCount; ++i)
				{
					// 점을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the point.
					rectangleGauge.SetTransitionType(arrTransitionType[i]);
					// 추출한 경계점 중 최종적으로 얻고자하는 경계점 유형을 선택합니다. // Select the boundary point type you want to finally get among the extracted boundary points.
					rectangleGauge.SetTransitionChoice(arrTransitionChoice[i]);

					// 알고리즘 수행 // Execute the Algoritm
					if((res = rectangleGauge.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute point gauge.");
						break;
					}

					// 실행 결과를 가져옵니다. // Get the execution result.
					CFLRect<double> flrResult = new CFLRect<double>();
					CFLFigureArray flfaResultsValid = new CFLFigureArray();
					CFLFigureArray flfaResultsInvalid = new CFLFigureArray();
					// 추정된 사각형을 가져옵니다. // Get the estimated rectangle.
					res = rectangleGauge.GetMeasuredObject(ref flrResult, i % 4);
					// 추정된 사각형을 추출에 사용된 유효 경계점을 가져옵니다. // Get the valid bounding point used to extract the estimated rectangle.
					rectangleGauge.GetMeasuredValidPoints(ref flfaResultsValid, i % 4);
					// 추정된 사각형을 추출에 사용되지 못한 유효하지 않은 경계점을 가져옵니다. // Get an invalid bounding point that was not used to extract the estimated rectangle.
					rectangleGauge.GetMeasuredInvalidPoints(ref flfaResultsInvalid, i % 4);

					CGUIViewImageLayer layer = viewImage[i].GetLayer(0);

					layer.Clear();

					if((res = layer.DrawTextImage(new CFLPoint<double>(0, 0), arrViewText[i], EColor.YELLOW, EColor.BLUE, 20, true)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					CFLRect<double> flrRegion = rectangleGauge.GetMeasurementRegion();
					List<double> arrTolerance = rectangleGauge.GetTolerance();
					double f64Tolerance = arrTolerance[0];

					double[] arrF64Tolerance = new double[4];
					double f64WidthTolerance = 0, f64HeightTolerance = 0;
					double f64Ratio;

					double f64Height = flrRegion.GetHeight();
					double f64Width = flrRegion.GetWidth();

					// 설정된 ROI에 대해 내부 및 외부 측정영역을 디스플레이 합니다. // Display the inner and outer measurement areas for the set ROI.
					if(f64Height >= f64Width)
					{
						f64Ratio = f64Width / f64Height;

						double f64MinTolerance = f64Height / 2.0;

						if(f64Tolerance >= f64MinTolerance)
							f64Tolerance = f64MinTolerance;

						f64HeightTolerance = f64Tolerance;
						f64WidthTolerance = f64HeightTolerance * f64Ratio;
					}

					if(f64Height < f64Width)
					{
						f64Ratio = f64Height / f64Width;

						double f64MinTolerance = f64Width / 2.0;

						if(f64Tolerance >= f64MinTolerance)
							f64Tolerance = f64MinTolerance;

						f64WidthTolerance = f64Tolerance;
						f64HeightTolerance = f64WidthTolerance * f64Ratio;
					}

					for(int j = 0; j < 2; ++j)
					{
						arrF64Tolerance[2 * j] = f64WidthTolerance;
						arrF64Tolerance[2 * j + 1] = f64HeightTolerance;
					}

					CFLPoint<double> flpCent = flrRegion.GetCenter();
					CFLRect<double> flrInner = flrRegion, flrOuter = flrRegion;

					if(flrInner.GetWidth() / 2.0 <= arrF64Tolerance[0] || flrInner.GetHeight() / 2.0 <= arrF64Tolerance[1])
					{
						if((res = layer.DrawFigureImage(new CFLPoint<double>(flrInner.GetCenter()), EColor.RED)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure\n");
							break;
						}
					}
					else
					{
						flrInner.Offset(-flpCent.x, -flpCent.y);
						flrInner.Multiply(((double)flrRegion.GetWidth() - arrF64Tolerance[0] * 2.0) / (double)flrRegion.GetWidth(), ((double)flrRegion.GetHeight() - arrF64Tolerance[1] * 2.0) / (double)flrRegion.GetHeight());
						flrInner.Offset(flpCent);

						if((res = layer.DrawFigureImage(flrInner, EColor.RED)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure\n");
							break;
						}
					}

					flrOuter.Offset(-flpCent.x, -flpCent.y);
					flrOuter.Multiply(((double)flrRegion.GetWidth() + arrF64Tolerance[0] * 2.0) / (double)flrRegion.GetWidth(), ((double)flrRegion.GetHeight() + arrF64Tolerance[1] * 2.0) / (double)flrRegion.GetHeight());
					flrOuter.Offset(flpCent);

					if((res = layer.DrawFigureImage(flrOuter, EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					if(res.IsOK())
					{
						// 추정된 사각형을 디스플레이 합니다. // Display the estimated rectangle.
						if((res = layer.DrawFigureImage(flrResult, EColor.BLACK, 5)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure\n");
							break;
						}

						if((res = layer.DrawFigureImage(flrResult, EColor.CYAN, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure\n");
							break;
						}

						// 사각형의 정보를 Console창에 출력합니다. // Output the square information to the console window.
						double f64ResultWidth, f64ResultHeight, f64ResultAngle;
						f64ResultWidth = flrResult.GetWidth();
						f64ResultHeight = flrResult.GetHeight();
						f64ResultAngle = flrResult.GetAngle();
						CFLPoint<double> flpLineCenter = flrResult.GetCenter();
						Console.WriteLine("Rectangle Center : ({0:F2}, {1:F2})\nWidth : {2:F2} pixels\nHeight : {3:F2} pixels\nAngle : {4:F2}˚\n", flpLineCenter.x, flpLineCenter.y, f64ResultWidth, f64ResultHeight, f64ResultAngle);
					}

					// 추출된 유효점이 어디인지 알기 위해 디스플레이 한다 // Display to know where the extracted valid point is

					CFLFigureArray flfaValidCrosshair = new CFLFigureArray();
					flfaResultsValid.MakeCrossHairElementwise(ref flfaValidCrosshair, 1, true);

					if((res = layer.DrawFigureImage(flfaValidCrosshair, EColor.LIME)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					CFLFigureArray flfaInvalidCrosshair = new CFLFigureArray();
					flfaResultsInvalid.MakeCrossHairElementwise(ref flfaInvalidCrosshair, 1, true);

					if((res = layer.DrawFigureImage(flfaInvalidCrosshair, EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					if((res = layer.DrawFigureImage(measureRegion, EColor.BLUE)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
						break;
					}

					// 이미지 뷰를 갱신 합니다. // Update the image view.
					viewImage[i].Invalidate(true);
				}

				bool bTerminated = false;
				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(!bTerminated)
				{
					for(int i = 0; i < i32ExampleCount; ++i)
						bTerminated |= !viewImage[i].IsAvailable();

					CThreadUtilities.Sleep(1);
				}

				for(int i = 0; i < i32ExampleCount; ++i)
					viewImage[i].Destroy();
			}
			while(false);
		}
	}
}

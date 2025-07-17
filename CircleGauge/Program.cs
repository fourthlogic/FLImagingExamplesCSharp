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

namespace Gauge
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


			CCircleGauge.ETransitionType[] arrTransitionType =
			{
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CCircleGauge.ETransitionType.DarkToBright,
				CCircleGauge.ETransitionType.BrightToDark,
				CCircleGauge.ETransitionType.DarkToBrightToDark,
			};

			CCircleGauge.ETransitionChoice[] arrTransitionChoice =
			{
				CCircleGauge.ETransitionChoice.Begin,
				CCircleGauge.ETransitionChoice.Begin,
				CCircleGauge.ETransitionChoice.Begin,
				CCircleGauge.ETransitionChoice.LargestArea,
				CCircleGauge.ETransitionChoice.End,
				CCircleGauge.ETransitionChoice.End,
				CCircleGauge.ETransitionChoice.End,
				CCircleGauge.ETransitionChoice.LargestAmplitude,
				CCircleGauge.ETransitionChoice.Closest,
				CCircleGauge.ETransitionChoice.Closest,
				CCircleGauge.ETransitionChoice.Closest,
				CCircleGauge.ETransitionChoice.Closest,
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
				if((res = fliImage.Load("../../ExampleImages/Gauge/Circle.flif")).IsFail())
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

				// Circle Gauge 객체 생성 // Create Circle Gauge Object
				CCircleGauge circleGauge = new CCircleGauge();

				// 처리할 이미지 설정 // Set the image to process
				circleGauge.SetSourceImage(ref fliImage);

				// 측정할 영역을 설정합니다. // Set the area to measure.
				CFLCircle<double> measureRegion = new CFLCircle<double>(260.0, 265.0, 120.0);
				double tolerance = 100;
				circleGauge.SetMeasurementRegion(measureRegion, tolerance);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.				
				// 원을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the circle.
				circleGauge.SetThreshold(20);
				// 원을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the circle.
				circleGauge.SetMinimumAmplitude(10);
				// 원을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the circle.
				circleGauge.SetThickness(3);
				// 원을 추정하기위해 추출할 경계점들의 추출 간격을 설정합니다. // Set the extraction interval of boundary points to be extracted to estimate the circle.
				circleGauge.SetSamplingStep(1);
				// 원을 추정하기위해 추출할 경계점들의 이상치 조정을 위한 임계값을 설정합니다. // Set the threshold value for outlier adjustment of the boundary points to be extracted to estimate the circle.
				circleGauge.SetOutliersThreshold(3);
				// 원을 추정하기위해 추출할 경계점들의 이상치 조정 횟수을 설정합니다. // Set the number of outlier adjustments for boundary points to be extracted to estimate the circle.
				circleGauge.SetOutliersThresholdCount(3);

				for(int i = 0; i < i32ExampleCount; ++i)
				{
					// 원을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the circle.
					circleGauge.SetTransitionType(arrTransitionType[i]);
					// 원을 추정하기위해 추출한 경계점 중 사용할 경계점 유형을 선택합니다. // Select the boundary point type to use among the boundary points extracted to estimate the circle.
					circleGauge.SetTransitionChoice(arrTransitionChoice[i]);

					// 알고리즘 수행 // Execute the Algoritm
					if((res = circleGauge.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute Circle gauge.");
						break;
					}

					// 실행 결과를 가져옵니다. // Get the execution result.
					CFLCircle<double> flcResult = new CFLCircle<double>();
					CFLFigureArray flfaResultsValid = new CFLFigureArray();
					CFLFigureArray flfaResultsInvalid = new CFLFigureArray();

					// 실행 결과를 가져옵니다. // Get the execution result. // Get the execution result.
					res = circleGauge.GetMeasuredObject(ref flcResult, i % 4);
					// 추정된 원을 추출에 사용된 유효 경계점을 가져옵니다. // Get the effective boundary point used to extract the estimated circle.
					circleGauge.GetMeasuredValidPoints(ref flfaResultsValid, i % 4);
					// 추정된 원을 추출에 사용되지 못한 유효하지 않은 경계점을 가져옵니다. // Get an invalid boundary point that is not used to extract the estimated circle.
					circleGauge.GetMeasuredInvalidPoints(ref flfaResultsInvalid, i % 4);

					CGUIViewImageLayer layer = viewImage[i].GetLayer(0);

					layer.Clear();

					if((res = layer.DrawTextImage(new CFLPoint<double>(0, 0), arrViewText[i], EColor.YELLOW, EColor.BLUE, 20, true)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					CFLCircle<double> flcRegion = circleGauge.GetMeasurementRegion();

					CFLCircle<double> flcInner = new CFLCircle<double>();
					CFLCircle<double> flcOuter = new CFLCircle<double>();
					double f64Tolerance;
					f64Tolerance = circleGauge.GetTolerance();

					// 설정된 ROI에 대해 내부 및 외부 측정영역을 디스플레이 합니다. // Display the inner and outer measurement areas for the set ROI.
					if(flcRegion.radius < f64Tolerance)
						flcInner.Set((float)flcRegion.GetCenter().x, (float)flcRegion.GetCenter().y, 0.1f);
					else
						flcInner.Set((float)flcRegion.GetCenter().x, (float)flcRegion.GetCenter().y, flcRegion.radius - f64Tolerance);

					flcOuter.Set((float)flcRegion.GetCenter().x, (float)flcRegion.GetCenter().y, flcRegion.radius + f64Tolerance);

					if((res = layer.DrawFigureImage(flcInner, EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layer.DrawFigureImage(flcOuter, EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if(res.IsOK())
					{
						// 추정된 원을 디스플레이 합니다. // Display the estimated circle.
						if((res = layer.DrawFigureImage(flcResult, EColor.BLACK, 5)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						if((res = layer.DrawFigureImage(flcResult, EColor.CYAN, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						// 원의 정보를 Console창에 출력합니다. // Output the original information to the console window.
						double f64Radius;
						flcResult.GetRadius(ref f64Radius);
						CFLPoint<double> flpLineCenter = new CFLPoint<double>();
						flcResult.GetCenter(ref flpLineCenter);
						Console.WriteLine("Circle Center : ({0}, {1})\nRadius : {2} pixels", flpLineCenter.x, flpLineCenter.y, f64Radius);
					}

					// 추출된 유효점이 어디인지 알기 위해 디스플레이 한다 // Display to know where the extracted valid point is
					for(long i64Index = 0; i64Index < flfaResultsValid.GetCount(); ++i64Index)
					{
						if(flfaResultsValid.GetAt(i64Index).GetDeclType() != EFigureDeclType.Point)
							break;

						CFLPoint<double> pFlp = (CFLPoint<double>)flfaResultsValid.GetAt(i64Index);

						CFLFigureArray flfaPoint = (new CFLPoint<double>(pFlp.x, pFlp.y)).MakeCrossHair(1, true);

						if((res = layer.DrawFigureImage(flfaPoint, EColor.LIME)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}
					}

					// 추출된 유효하지 않은 점이 어디인지 알기 위해 디스플레이 한다 // Display to see where the extracted invalid points are
					for(long i64Index = 0; i64Index < flfaResultsInvalid.GetCount(); ++i64Index)
					{
						if(flfaResultsInvalid.GetAt(i64Index).GetDeclType() != EFigureDeclType.Point)
							break;

						CFLPoint<double> pFlp = (CFLPoint<double>)flfaResultsInvalid.GetAt(i64Index);

						CFLFigureArray flfaPoint = (new CFLPoint<double>(pFlp.x, pFlp.y)).MakeCrossHair(1, true);

						if((res = layer.DrawFigureImage(flfaPoint, EColor.RED)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}
					}

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					if((res = layer.DrawFigureImage(measureRegion, EColor.BLUE)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figures objects on the image view.");
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
			}
			while(false);
		}
    }
}

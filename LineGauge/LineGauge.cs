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
	class LineGauge
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


			CLineGauge.ETransitionType[] arrTransitionType =
			{
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CLineGauge.ETransitionType.DarkToBright,
				CLineGauge.ETransitionType.BrightToDark,
				CLineGauge.ETransitionType.DarkToBrightToDark,
			};

			CLineGauge.ETransitionChoice[] arrTransitionChoice =
			{
				CLineGauge.ETransitionChoice.Begin,
				CLineGauge.ETransitionChoice.Begin,
				CLineGauge.ETransitionChoice.Begin,
				CLineGauge.ETransitionChoice.LargestArea,
				CLineGauge.ETransitionChoice.End,
				CLineGauge.ETransitionChoice.End,
				CLineGauge.ETransitionChoice.End,
				CLineGauge.ETransitionChoice.LargestAmplitude,
				CLineGauge.ETransitionChoice.Closest,
				CLineGauge.ETransitionChoice.Closest,
				CLineGauge.ETransitionChoice.Closest,
				CLineGauge.ETransitionChoice.Closest,
			};

			double[] arrTolerance =
			{
				200.0,
				200.0,
				200.0,
				200.0,
				200.0,
				200.0,
				200.0,
				200.0,
				100.0,
				100.0,
				100.0,
				100.0,
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
				if((res = fliImage.Load("../../ExampleImages/Gauge/stripe.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
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

				// Line Gauge 객체 생성 // Create Line Gauge object
				CLineGauge lineGauge = new CLineGauge();

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.

				// 처리할 이미지 설정 // Set the image to process
				lineGauge.SetSourceImage(ref fliImage);
				// 선을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the line.
				lineGauge.SetThreshold(20);
				// 선을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the line.
				lineGauge.SetMinimumAmplitude(10);
				// 선을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the line.
				lineGauge.SetThickness(1);
				// 선을 추정하기위해 추출할 경계점들의 추출 간격을 설정합니다. // Set the extraction interval of boundary points to be extracted to estimate the line.
				lineGauge.SetSamplingStep(1.0);
				// 선을 추정하기위해 추출할 경계점들의 이상치 조정을 위한 임계값을 설정합니다. // Set the threshold value for outlier adjustment of the boundary points to be extracted to estimate the line.
				lineGauge.SetOutliersThreshold(1.0);
				// 선을 추정하기위해 추출할 경계점들의 이상치 조정 횟수을 설정합니다. // Set the number of outlier adjustments for boundary points to be extracted to estimate the line.
				lineGauge.SetOutliersThresholdCount(3);
				// 선을 추정하기위해 점 클러스터링 처리 유무에 대한 설정을 합니다. // Set whether or not to process point clustering to estimate the line.
				lineGauge.EnableClusterMode(true);

				CFLLine<double> measureRegion = new CFLLine<double>(250.0, 480.0, 250.0, 80.0);

				for(int i = 0; i < i32ExampleCount; ++i)
				{
					// 측정할 영역을 설정합니다. // Set the area to measure.
					lineGauge.SetMeasurementRegion(measureRegion, arrTolerance[i]);

					// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.
					// 선을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the line.
					lineGauge.SetTransitionType(arrTransitionType[i]);
					// 선을 추정하기위해 추출한 경계점 중 사용할 경계점 유형을 선택합니다. // Select the boundary point type to use among the boundary points extracted to estimate the line.
					lineGauge.SetTransitionChoice(arrTransitionChoice[i]);

					// 알고리즘 수행 // Execute the Algoritm
					if((res = lineGauge.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute Line gauge.");
						break;
					}


					// 실행 결과를 가져옵니다. // Get the execution result.
					CFLLine<double> fllResult = new CFLLine<double>();
					CFLFigureArray flfaResultsValid = new CFLFigureArray();
					CFLFigureArray flfaResultsInvalid = new CFLFigureArray();
					// 추정된 선을 가져옵니다. // Get the estimated line.
					lineGauge.GetMeasuredObject(ref fllResult, i % 4);
					// 추정된 선을 추출에 사용된 유효 경계점을 가져옵니다. // Get the effective boundary point used to extract the estimated line.
					lineGauge.GetMeasuredValidPoints(ref flfaResultsValid, i % 4);
					// 추정된 선을 추출에 사용되지 못한 유효하지 않은 경계점을 가져옵니다. // Get an invalid boundary point that is not used to extract the estimated line.
					lineGauge.GetMeasuredInvalidPoints(ref flfaResultsInvalid, i % 4);
					
					CGUIViewImageLayer layer = viewImage[i].GetLayer(0);

					layer.Clear();

					if((res = layer.DrawTextImage(new CFLPoint<double>(0, 0), arrViewText[i], EColor.YELLOW, EColor.BLUE, 20, true)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

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

					if((res = layer.DrawFigureImage(measureRegion, EColor.YELLOW)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
						break;
					}

					// 선의 방향을 디스플레이 합니다. // Display the direction of the line.
					CFLPoint<double> flpCenter = new CFLPoint<double>();
					double f64Angle = new double();
					CFLLine<double> fllCenter = new CFLLine<double>();

					flpCenter = measureRegion.GetCenter();
					f64Angle = measureRegion.GetAngle();

					fllCenter.flpPoints[0].Set(flpCenter);
					fllCenter.flpPoints[1].Set(flpCenter);
					fllCenter.Rotate(f64Angle, flpCenter);

					CFLPoint<double> flpCenter1 = new CFLPoint<double>(flpCenter.x - 1.5, flpCenter.y - Math.Sqrt(1.5) * .5 * 1.5);
					CFLPoint<double> flpCenter2 = new CFLPoint<double>(flpCenter.x + 1.5, flpCenter.y - Math.Sqrt(1.5) * .5 * 1.5);
					CFLPoint<double> flpCenter3 = new CFLPoint<double>(flpCenter.x, flpCenter.y + Math.Sqrt(1.5) * .5 * 1.5);

					CFLComplexRegion flTriangle = new CFLComplexRegion();
					flTriangle.PushBack(flpCenter1);
					flTriangle.PushBack(flpCenter2);
					flTriangle.PushBack(flpCenter3);
					flTriangle.Rotate(f64Angle, flpCenter);

					if((res = layer.DrawFigureImage(fllCenter, EColor.BLUE)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layer.DrawFigureImage(flTriangle, EColor.LIGHTRED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					// 설정된 ROI에 대해 측정영역을 디스플레이 합니다. // Display the measurement area for the set ROI.
					CFLQuad<double> flqDraw = new CFLQuad<double>();
					double f64ToleranceLeft = 0;
					double f64ToleranceRight = 0;
					lineGauge.GetTolerance(ref f64ToleranceLeft, ref f64ToleranceRight);

					CFLPoint<double> fllNorm = measureRegion.GetNormalVector();
					flqDraw.flpPoints[0].x = measureRegion.flpPoints[0].x + fllNorm.x * f64ToleranceLeft;
					flqDraw.flpPoints[0].y = measureRegion.flpPoints[0].y + fllNorm.y * f64ToleranceLeft;
					flqDraw.flpPoints[1].x = measureRegion.flpPoints[1].x + fllNorm.x * f64ToleranceLeft;
					flqDraw.flpPoints[1].y = measureRegion.flpPoints[1].y + fllNorm.y * f64ToleranceLeft;
					flqDraw.flpPoints[2].x = measureRegion.flpPoints[1].x - fllNorm.x * f64ToleranceRight;
					flqDraw.flpPoints[2].y = measureRegion.flpPoints[1].y - fllNorm.y * f64ToleranceRight;
					flqDraw.flpPoints[3].x = measureRegion.flpPoints[0].x - fllNorm.x * f64ToleranceRight;
					flqDraw.flpPoints[3].y = measureRegion.flpPoints[0].y - fllNorm.y * f64ToleranceRight;

					if((res = layer.DrawFigureImage(flqDraw, EColor.BLUE)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res).IsOK())
					{
						// 추정된 선분을 디스플레이 합니다. // Display the estimated line segment.
						if((res = layer.DrawFigureImage(fllResult, EColor.BLACK, 5)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						if((res = layer.DrawFigureImage(fllResult, EColor.CYAN, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						// 선의 정보를 Console창에 출력합니다. // Output the original information to the console window.
						CFLPoint<double> flpLineCenter = fllResult.GetCenter();
						double f64LineAngle = fllResult.GetAngle();
						Console.WriteLine("Line Center : ({0}, {1})\nAngle : {2}˚", flpLineCenter.x, flpLineCenter.y, f64LineAngle);
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

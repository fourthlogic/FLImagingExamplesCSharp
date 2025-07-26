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
	class EllipseGauge
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


			CEllipseGauge.ETransitionType[] arrTransitionType =
			{
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CEllipseGauge.ETransitionType.DarkToBright,
				CEllipseGauge.ETransitionType.BrightToDark,
				CEllipseGauge.ETransitionType.DarkToBrightToDark,
			};

			CEllipseGauge.ETransitionChoice[] arrTransitionChoice =
			{
				CEllipseGauge.ETransitionChoice.Begin,
				CEllipseGauge.ETransitionChoice.Begin,
				CEllipseGauge.ETransitionChoice.Begin,
				CEllipseGauge.ETransitionChoice.LargestArea,
				CEllipseGauge.ETransitionChoice.End,
				CEllipseGauge.ETransitionChoice.End,
				CEllipseGauge.ETransitionChoice.End,
				CEllipseGauge.ETransitionChoice.LargestAmplitude,
				CEllipseGauge.ETransitionChoice.Closest,
				CEllipseGauge.ETransitionChoice.Closest,
				CEllipseGauge.ETransitionChoice.Closest,
				CEllipseGauge.ETransitionChoice.Closest,
			};

			double[] arrTolerance =
			{
				85.0,
				85.0,
				85.0,
				20.0,
				85.0,
				85.0,
				85.0,
				30.0,
				30.0,
				30.0,
				30.0,
				30.0,
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
				if((res = fliImage.Load("../../ExampleImages/Gauge/Ellipse.flif")).IsFail())
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

				// Ellipse Gauge 객체 생성 // Create Ellipse Gauge Object
				CEllipseGauge ellipseGauge = new CEllipseGauge();

				// 처리할 이미지 설정 // Set the image to process
				ellipseGauge.SetSourceImage(ref fliImage);

				// 측정할 영역을 설정합니다. // Set the area to measure.
				CFLEllipse<double> measureRegion = new CFLEllipse<double>(240, 280, 96, 150, 22);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.				
				// 타원을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the ellipse.
				ellipseGauge.SetThreshold(20);
				// 타원을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the ellipse.
				ellipseGauge.SetMinimumAmplitude(10);
				// 타원을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the ellipse.
				ellipseGauge.SetThickness(1);
				// 타원을 추정하기위해 추출할 경계점들의 추출 간격을 설정합니다. // Set the extraction interval of boundary points to be extracted to estimate the ellipse.
				ellipseGauge.SetSamplingStep(1.0);
				// 타원을 추정하기위해 추출할 경계점들의 이상치 조정을 위한 임계값을 설정합니다. // Set the threshold value for outlier adjustment of the boundary points to be extracted to estimate the ellipse.
				ellipseGauge.SetOutliersThreshold(1.0);
				// 타원을 추정하기위해 추출할 경계점들의 이상치 조정 횟수을 설정합니다. // Set the number of outlier adjustments for boundary points to be extracted to estimate the ellipse.
				ellipseGauge.SetOutliersThresholdCount(3);

				for(int i = 0; i < i32ExampleCount; ++i)
				{
					ellipseGauge.SetMeasurementRegion(measureRegion, arrTolerance[i]);
					// 타원을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the ellipse.
					ellipseGauge.SetTransitionType(arrTransitionType[i]);
					// 타원을 추정하기위해 추출한 경계점 중 사용할 경계점 유형을 선택합니다. // Select the boundary point type to use among the boundary points extracted to estimate the ellipse.
					ellipseGauge.SetTransitionChoice(arrTransitionChoice[i]);

					// 알고리즘 수행 // Execute the Algoritm
					if((res = ellipseGauge.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute Ellipse gauge.");
						break;
					}

					// 실행 결과를 가져옵니다. // Get the execution result.
					CFLEllipse<double> flres = new CFLEllipse<double>();
					CFLFigureArray flfaResultsValid = new CFLFigureArray();
					CFLFigureArray flfaResultsInvalid = new CFLFigureArray();
					// 추정된 타원을 가져옵니다. // Get the estimated ellipse.
					ellipseGauge.GetMeasuredObject(ref flres, i % 4);
					// 추정된 타원을 추출에 사용된 유효 경계점을 가져옵니다. // Get the valid boundary point used to extract the estimated ellipse.
					ellipseGauge.GetMeasuredValidPoints(ref flfaResultsValid, i % 4);
					// 추정된 타원을 추출에 사용되지 못한 유효하지 않은 경계점을 가져옵니다. // Get an invalid boundary point that is not used to extract the estimated ellipse.
					ellipseGauge.GetMeasuredInvalidPoints(ref flfaResultsInvalid, i % 4);

					CGUIViewImageLayer layer = viewImage[i].GetLayer(0);

					layer.Clear();

					if((res = layer.DrawTextImage(new CFLPoint<double>(0, 0), arrViewText[i], EColor.YELLOW, EColor.BLUE, 20, true)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					double f64Tolerance = new double();

					CFLEllipse<double> fleRegion = ellipseGauge.GetMeasurementRegion();
					CFLEllipse<double> fleInner = new CFLEllipse<double>(fleRegion);
					CFLEllipse<double> fleOuter = new CFLEllipse<double>(fleRegion);
					float f64Radius1 = 0, f64Radius2 = 0;

					fleInner.GetRadius1(ref f64Radius1);
					fleInner.GetRadius2(ref f64Radius2);

					f64Tolerance = ellipseGauge.GetTolerance();

					double f64Radius1Tolerance = fleRegion.radius1 >= fleRegion.radius2 ? f64Tolerance : f64Tolerance * (fleRegion.radius1 / fleRegion.radius2);
					double f64Radius2Tolerance = fleRegion.radius1 >= fleRegion.radius2 ? f64Tolerance * (fleRegion.radius2 / fleRegion.radius1) : f64Tolerance;

					// 설정된 ROI에 대해 내부 및 외부 측정영역을 디스플레이 합니다. // Display the inner and outer measurement areas for the set ROI.
					if(f64Radius1 < f64Radius1Tolerance || f64Radius2 < f64Radius2Tolerance)
						fleInner.Set((float)fleInner.GetCenter().x, (float)fleInner.GetCenter().y, 0.1f, 0.1f);
					else
					{
						fleInner.radius1 -= f64Radius1Tolerance;
						fleInner.radius2 -= f64Radius2Tolerance;
					}

					fleOuter.radius1 += f64Radius1Tolerance;
					fleOuter.radius2 += f64Radius2Tolerance;

					if((res = layer.DrawFigureImage(fleInner, EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layer.DrawFigureImage(fleOuter, EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res).IsOK())
					{
						// 추정된 타원을 디스플레이 합니다. // Display the estimated ellipse.
						if((res = layer.DrawFigureImage(flres, EColor.BLACK, 5)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						if((res = layer.DrawFigureImage(flres, EColor.CYAN, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						// 타원의 정보를 Console창에 출력합니다. // Output the information of the ellipse to the console window.
						float f64RadiusResult1 = 0, f64RadiusResult2 = 0;
						double f64Anglres = 0;
						flres.GetRadius1(ref f64RadiusResult1);
						flres.GetRadius2(ref f64RadiusResult2);
						f64Anglres = flres.GetAngle();
						CFLPoint<double> flpLineCenter = flres.GetCenter();
						Console.WriteLine("Ellipse Center : ({0}, {1})\nRadius X : {2} pixels\nRadius Y : {3} pixels\nAngle : {4}˚",
							flpLineCenter.x, flpLineCenter.y, f64RadiusResult1, f64RadiusResult2, f64Anglres);
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
			}
			while(false);
		}
	}
}

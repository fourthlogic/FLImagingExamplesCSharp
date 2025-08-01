﻿using System;
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
    class CornerGauge_EachCorner
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

            // 이미지 객체 선언 // Declare the image object
            CFLImage fliImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImage = new CGUIViewImage();
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/Gauge/Rect.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 1424, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Corner Gauge 객체 생성 // Create Corner Gauge Object
				CCornerGauge cornerGauge = new CCornerGauge();

				// 처리할 이미지 설정 // Set the image to process
				cornerGauge.SetSourceImage(ref fliImage);

				// 측정할 영역을 설정합니다. // Set the area to measure.
				CFLRect<double> measureRegion = new CFLRect<double>(213.577428, 262.324155, 295.020437, 348.179290);
				double tolerance = 50;
				cornerGauge.SetMeasurementRegion(measureRegion, tolerance);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.
				// 코너를 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the corner.
				cornerGauge.SetTransitionType(CCornerGauge.ETransitionType.DarkToBrightOrBrightToDark);
				// 코너를 추정하기위해 추출한 경계점 중 사용할 경계점 유형을 선택합니다. // Select the boundary point type to use among the boundary points extracted to estimate the corner.
				cornerGauge.SetTransitionChoice(CCornerGauge.ETransitionChoice.Closest);
				// 코너를 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the corner.
				cornerGauge.SetThreshold(20);
				// 코너를 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the corner.
				cornerGauge.SetMinimumAmplitude(10);
				// 코너를 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the corner.
				cornerGauge.SetThickness(1);
				// 코너를 추정하기위해 추출할 경계점들의 추출 간격을 설정합니다. // Set the extraction interval of boundary points to be extracted to estimate the corner.
				cornerGauge.SetSamplingStep(1.0);
				// 코너를 추정하기위해 추출할 경계점들의 이상치 조정을 위한 임계값을 설정합니다. // Set the threshold value for outlier adjustment of the boundary points to be extracted to estimate the corner.
				cornerGauge.SetOutliersThreshold(1.0);
				// 코너를 추정하기위해 추출할 경계점들의 이상치 조정 횟수을 설정합니다. // Set the number of outlier adjustments for boundary points to be extracted to estimate the corner.
				cornerGauge.SetOutliersThresholdCount(3);
				// 코너를 추정하기위해 점 클러스터링 처리 유무에 대한 설정을 합니다. // Set whether or not to process point clustering to estimate the corner.
				cornerGauge.EnableClusterMode(true);
				// 코너를 추정하기위해 마진을 설정합니다. 필요에 따라 각 구역별로 설정가능합니다. // Set the margin to estimate the corner. It can be set for each zone as needed.
				cornerGauge.SetMeasurementMarginRatio(0, CCornerGauge.EMargin.All);
				// 코너를 추정하기위한 Tolerance를 설정합니다. 필요에 따라 각 구역별로 설정가능합니다. // Set the Tolerance for estimating the corner. It can be set for each zone as needed.
				cornerGauge.SetTolerance(tolerance, CCornerGauge.ETolerance.All);
				// 코너를 측정하기위한 영역을 설정합니다. // Set the area for measuring corners.
				cornerGauge.SetCorner(CCornerGauge.ECorner.All);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = cornerGauge.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Corner gauge.");
					break;
				}

				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				layer.Clear();


				if(res.IsOK())
				{
					CFLFigureArray flfaResultLine = new CFLFigureArray();
					// 추정된 선을 가져옵니다. // Get the estimated line.
					cornerGauge.GetMeasuredLines(ref flfaResultLine);

					layer.DrawFigureImage(flfaResultLine, EColor.BLACK, 5);
					layer.DrawFigureImage(flfaResultLine, EColor.CYAN, 3);

					CFLLine<double>[] arrLines = new CFLLine<double>[2];
					arrLines[0] = (CFLLine<double>)(flfaResultLine.GetAt(0));
					arrLines[1] = (CFLLine<double>)(flfaResultLine.GetAt(1));

					for(int i32CornerIndex = 0; i32CornerIndex < 4; ++i32CornerIndex)
					{
						// 실행 결과를 가져옵니다. // Get the execution result.
						CFLPoint<double> flpResultCorner = new CFLPoint<double>();
						// 추정된 코너를 가져옵니다. // Get the estimated corner.
						cornerGauge.GetMeasuredObject(ref flpResultCorner, i32CornerIndex);

						layer.DrawFigureImage(flpResultCorner, EColor.BLACK, 3);
						layer.DrawFigureImage(flpResultCorner, EColor.CYAN, 1);

						Console.WriteLine("#{0} : Corner X : {1}, Corner Y : {2}", i32CornerIndex, flpResultCorner.x, flpResultCorner.y);
					}

					for(int i = 0; i < 2; ++i)
					{
						double f64ResultAngle;
						f64ResultAngle = arrLines[i].GetAngle();
						Console.WriteLine("Line Angle : {0}˚", f64ResultAngle);
					}
				}

				CFLFigureArray flfaResultsValid = new CFLFigureArray();
				CFLFigureArray flfaResultsInvalid = new CFLFigureArray();
				// 추정된 코너를 추출에 사용된 유효 경계점을 가져옵니다. // Get the effective boundary point used to extract the estimated corner.
				cornerGauge.GetMeasuredValidPoints(ref flfaResultsValid, 0);
				// 추정된 코너를 추출에 사용되지 못한 유효하지 않은 경계점을 가져옵니다. // Get an invalid boundary point that is not used to extract the estimated corner.
				cornerGauge.GetMeasuredInvalidPoints(ref flfaResultsInvalid, 0);

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
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImage.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
    }
}
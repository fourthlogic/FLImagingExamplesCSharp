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

		static void Main(string[] args)
        {
            // 이미지 객체 선언 // Declare the image object
            CFLImage fliImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImage = new CGUIViewImage();
			CResult eResult = new CResult();

			do
			{
                // 이미지 로드 // Load image
                if ((eResult = fliImage.Load("../../ExampleImages/Gauge/CrossImage.flif")).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load the image file.\n");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if ((eResult = viewImage.Create(400, 0, 1424, 768)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.\n");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
                if ((eResult = viewImage.SetImagePtr(ref fliImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view.\n");
                    break;
                }

                // Cross Gauge 객체 생성 // Create Cross Gauge
                CCrossGauge CrossGauge = new CCrossGauge();

                // 처리할 이미지 설정 // Set the image to process
                CrossGauge.SetSourceImage(ref fliImage);

                // 측정할 영역을 설정합니다. // Set the area to measure.
                CFLRect<double> measureRegion = new CFLRect<double>(264.293737, 247.352397, 787.094791, 779.478531);
                double tolerance = 150.0;
                CrossGauge.SetMeasurementRegion(measureRegion, tolerance);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.
				// 십자형을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the crosshair.
				CrossGauge.SetTransitionType(CCrossGauge.ETransitionType.DarkToBrightToDarkOrBrightToDarkToBright);
				// 십자형을 추정하기위해 추출한 경계점 중 사용할 경계점 유형을 선택합니다. // Select the boundary point type to use among the boundary points extracted to estimate the crosshair.
				CrossGauge.SetTransitionChoice(CCrossGauge.ETransitionChoice.Closest);
				// 십자형을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the crosshair.
				CrossGauge.SetThreshold(20);
				// 십자형을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the crosshair.
				CrossGauge.SetMinimumAmplitude(10);
				// 십자형을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the crosshairs.
				CrossGauge.SetThickness(1);
				// 십자형을 추정하기위해 추출할 경계점들의 추출 간격을 설정합니다. // Set the extraction interval of boundary points to be extracted to estimate the crosshair.
				CrossGauge.SetSamplingStep(1.0);
				// 십자형을 추정하기위해 추출할 경계점들의 이상치 조정을 위한 임계값을 설정합니다. // Set the threshold value for outlier adjustment of the boundary points to be extracted to estimate the crosshairs.
				CrossGauge.SetOutliersThreshold(1.0);
				// 십자형을 추정하기위해 추출할 경계점들의 이상치 조정 횟수을 설정합니다. // Set the number of outlier adjustments for boundary points to be extracted to estimate the crosshairs.
				CrossGauge.SetOutliersThresholdCount(3);
				// 십자형을 추정하기위해 점 클러스터링 처리 유무에 대한 설정을 합니다. // Set whether or not to process point clustering to estimate the crosshairs.
				CrossGauge.EnableClusterMode(true);
				// 십자형을 추정하기위해 마진을 설정합니다. 필요에 따라 각 구역별로 설정가능합니다. // Set the margin to estimate the crosshairs. It can be set for each zone as needed.
				CrossGauge.SetMeasurementMarginRatio(0.3, 0.1);

				// 알고리즘 수행 // Execute the Algoritm
				if((eResult = CrossGauge.Execute()).IsFail())
                {
                    ErrorPrint(eResult, "Failed to execute Cross gauge.");
                    break;
                }

                // 실행 결과를 가져옵니다. // Get the execution result.
                CFLPoint<double> resultRegion;
                CFLFigureArray flfaResultsValid, flfaResultsInvalid;
				CFLFigureArray flfaResultLine;
				// 추정을 위한 라인을 가져옵니다. // Get the line for inference.
				CrossGauge.GetMeasuredLines(out flfaResultLine);
				// 추정된 십자형을 가져옵니다. // Get the estimated crosshairs.
				CrossGauge.GetMeasuredObject(out resultRegion);
				// 추정된 십자형을 추출에 사용된 유효 경계점을 가져옵니다. // Get the valid boundary points used to extract the estimated crosshairs.
				CrossGauge.GetMeasuredValidPoints(out flfaResultsValid);
				// 추정된 십자형을 추출에 사용되지 못한 유효하지 않은 경계점을 가져옵니다. // Get invalid boundary points that were not used to extract the estimated crosshairs.
				CrossGauge.GetMeasuredInvalidPoints(out flfaResultsInvalid);

                CGUIViewImageLayer layer = viewImage.GetLayer(0);
                 
                layer.Clear();

				CFLFigureArray flfaResult = resultRegion.MakeCrossHair(25, true);
				layer.DrawFigureImage(flfaResult, EColor.BLACK, 3);
				layer.DrawFigureImage(flfaResult, EColor.CYAN, 1);

				layer.DrawFigureImage(flfaResultLine, EColor.BLACK, 5);
				layer.DrawFigureImage(flfaResultLine, EColor.CYAN, 3);

				if(eResult.IsOK())
                {
					double f64ResultAngle;
					CrossGauge.GetMeasuredAngle(out f64ResultAngle);

                    Console.WriteLine("Cross Center : ({0}, {1})\nAngle : {2}˚", resultRegion.x, resultRegion.y, f64ResultAngle);
                }

                for (long i64Index = 0; i64Index < flfaResultsValid.GetCount(); ++i64Index)
                {
                    if (flfaResultsValid.GetAt(i64Index).GetDeclType() != EFigureDeclType.Point)
                        break;

                    CFLPoint<double> pFlp = (CFLPoint<double>)flfaResultsValid.GetAt(i64Index);

                    CFLFigureArray flfaPoint = (new CFLPoint<double>(pFlp.x, pFlp.y)).MakeCrossHair(1, true);

                    if ((eResult = layer.DrawFigureImage(flfaPoint, EColor.LIME)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw figure");
						break;
					}
				}

				for (long i64Index = 0; i64Index < flfaResultsInvalid.GetCount(); ++i64Index)
                {
                    if (flfaResultsInvalid.GetAt(i64Index).GetDeclType() != EFigureDeclType.Point)
                        break;

                    CFLPoint<double> pFlp = (CFLPoint<double>)flfaResultsInvalid.GetAt(i64Index);

                    CFLFigureArray flfaPoint = (new CFLPoint<double>(pFlp.x, pFlp.y)).MakeCrossHair(1, true);

                    if ((eResult = layer.DrawFigureImage(flfaPoint, EColor.RED)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw figure");
						break;
					}
				}

				if ((eResult = layer.DrawFigureImage(measureRegion, EColor.BLUE)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw figures objects on the image view.\n");
					break;
				}

                // 이미지 뷰를 갱신 합니다. // Update the image view.
                viewImage.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
                while (viewImage.IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

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
                if ((eResult = fliImage.Load("../../ExampleImages/Gauge/Plate.flif")).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load the image file.");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if ((eResult = viewImage.Create(400, 0, 1424, 768)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
                if ((eResult = viewImage.SetImagePtr(ref fliImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view.");
                    break;
                }

                // Point Gauge 객체 생성 // Create Point Gauge Object
                CPointGauge pointGauge = new CPointGauge();

                // 처리할 이미지 설정 // Set the image to process
                pointGauge.SetSourceImage(ref fliImage);

                // 측정할 영역을 설정합니다. // Set the area to measure.
                CFLPoint<double> measureCenter = new CFLPoint<double>(267.481728, 240.846156);
                double tolerance = 400.0;
                double angle = 25.0;
                pointGauge.SetMeasurementRegion(measureCenter, tolerance, angle);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.
				// 점을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the point.
				pointGauge.SetTransitionType(CPointGauge.ETransitionType.DarkToBrightOrBrightToDark);
				// 추출한 경계점 중 최종적으로 얻고자하는 경계점 유형을 선택합니다. // Select the boundary point type you want to finally get among the extracted boundary points.
				pointGauge.SetTransitionChoice(CPointGauge.ETransitionChoice.Begin);
				// 점을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the point.
				pointGauge.SetThreshold(20);
				// 점을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the point.
				pointGauge.SetMinimumAmplitude(10);
				// 점을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the points.
				pointGauge.SetThickness(1);

                // 알고리즘 수행 // Execute the Algoritm
                if ((eResult = pointGauge.Execute()).IsFail())
                {
                    ErrorPrint(eResult, "Failed to execute point gauge.");
                    break;
                }

                // 실행 결과를 가져옵니다. // Get the execution result.
                long i64Count = pointGauge.GetMeasuredObjectCount();
				// 추정과정에 사용된 선을 가져옵니다. // Get the line used in the estimation process.
				CFLLine<double> fllLine = pointGauge.GetMeasurementRegion();

                CGUIViewImageLayer layer = viewImage.GetLayer(0);

                layer.Clear();

                // 추출된 점이 어디인지 알기 위해 디스플레이 한다 // Display to know where the extracted point is
                for (int i32Index = 0; i32Index < (int)i64Count; ++i32Index)
                {
                    CFLPoint<double> flp;

                    if (pointGauge.GetMeasuredObject(out flp, i32Index).IsFail())
                        break;

                    if((eResult = layer.DrawFigureImage(flp.MakeCrossHair(7, true), EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw figure");
						break;
					}

					if((eResult = layer.DrawFigureImage(flp.MakeCrossHair(7, true), EColor.YELLOW)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw figure");
						break;
					}

					Console.WriteLine("Index {0} : ({1}, {2})", i32Index, flp.x, flp.y);
                }
                 
                if ((eResult = layer.DrawFigureImage(fllLine, EColor.BLUE)).IsFail())
                    break;

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

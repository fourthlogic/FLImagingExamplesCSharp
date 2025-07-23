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
    class PointGauge
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


			CPointGauge.ETransitionType[] arrTransitionType =
			{
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBrightOrBrightToDark,
				CPointGauge.ETransitionType.DarkToBright,
				CPointGauge.ETransitionType.BrightToDark,
				CPointGauge.ETransitionType.DarkToBrightToDark,
			};

			CPointGauge.ETransitionChoice[] arrTransitionChoice =
			{
				CPointGauge.ETransitionChoice.Begin,
				CPointGauge.ETransitionChoice.Begin,
				CPointGauge.ETransitionChoice.Begin,
				CPointGauge.ETransitionChoice.LargestArea,
				CPointGauge.ETransitionChoice.End,
				CPointGauge.ETransitionChoice.End,
				CPointGauge.ETransitionChoice.End,
				CPointGauge.ETransitionChoice.LargestAmplitude,
				CPointGauge.ETransitionChoice.Closest,
				CPointGauge.ETransitionChoice.Closest,
				CPointGauge.ETransitionChoice.Closest,
				CPointGauge.ETransitionChoice.Closest,
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

				// Point Gauge 객체 생성 // Create Point Gauge Object
				CPointGauge pointGauge = new CPointGauge();

				// 처리할 이미지 설정 // Set the image to process
				pointGauge.SetSourceImage(ref fliImage);

				// 측정할 영역을 설정합니다. // Set the area to measure.
				CFLPoint<double> measureCenter = new CFLPoint<double>(267.0, 240.0);
				double tolerance = 400.0;
				double angle = 25.0;
				pointGauge.SetMeasurementRegion(measureCenter, tolerance, angle);

				// 추출하기위한 파라미터를 설정합니다. // Set parameters for extraction.				
				// 점을 추정하기위해 추출할 경계점의 변화 임계값에 대해 설정합니다. // Set the threshold change of the boundary point to be extracted to estimate the point.
				pointGauge.SetThreshold(20);
				// 점을 추정하기위해 추출할 경계점의 변화 임계값에 보정값을 설정합니다. // Set the correction value to the threshold change of the boundary point to be extracted to estimate the point.
				pointGauge.SetMinimumAmplitude(10);
				// 점을 추정하기위해 추출할 경계점들의 대표값 표본 개수를 설정합니다. // Set the number of representative sample values ??of the boundary points to be extracted to estimate the points.
				pointGauge.SetThickness(1);

				for(int i = 0; i < i32ExampleCount; ++i)
				{
					// 점을 추정하기위해 추출할 경계점 변화 방향에 대해 설정합니다. // Set the boundary point change direction to extract to estimate the point.
					pointGauge.SetTransitionType(arrTransitionType[i]);
					// 추출한 경계점 중 최종적으로 얻고자하는 경계점 유형을 선택합니다. // Select the boundary point type you want to finally get among the extracted boundary points.
					pointGauge.SetTransitionChoice(arrTransitionChoice[i]);

					// 알고리즘 수행 // Execute the Algoritm
					if((res = pointGauge.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute point gauge.");
						break;
					}

					// 실행 결과를 가져옵니다. // Get the execution result.
					long i64Count = pointGauge.GetMeasuredObjectCount();
					// 추정과정에 사용된 선을 가져옵니다. // Get the line used in the estimation process.
					CFLLine<double> fllLine = pointGauge.GetMeasurementRegion();

					CGUIViewImageLayer layer = viewImage[i].GetLayer(0);

					layer.Clear();

					if((res = layer.DrawTextImage(new CFLPoint<double>(0, 0), arrViewText[i], EColor.YELLOW, EColor.BLUE, 20, true)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					// 측정 중심 위치를 디스플레이한다. // Display the measurement center position.
					if((res = layer.DrawFigureImage(measureCenter.MakeCrossHair(10), EColor.RED)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					// 추출된 점이 어디인지 알기 위해 디스플레이 한다 // Display to know where the extracted point is
					for(int i32Index = 0; i32Index < (int)i64Count; ++i32Index)
					{
						CFLPoint<double> flp = new CFLPoint<double>();

						if(pointGauge.GetMeasuredObject(ref flp, i32Index).IsFail())
							break;

						if((res = layer.DrawFigureImage(flp.MakeCrossHair(10, true), EColor.BLACK, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						EColor col = (arrTransitionChoice[i] == CPointGauge.ETransitionChoice.Begin || arrTransitionChoice[i] == CPointGauge.ETransitionChoice.End) && i32Index != i % 4 ? EColor.YELLOW : EColor.CYAN;

						if((res = layer.DrawFigureImage(flp.MakeCrossHair(10, true), col, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure");
							break;
						}

						Console.WriteLine("Index {0} : ({1}, {2})", i32Index, flp.x, flp.y);
					}

					if((res = layer.DrawFigureImage(fllLine, EColor.BLUE)).IsFail())
						break;

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

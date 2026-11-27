using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
	class LeastSquares
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

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 이미지 뷰 생성 // Create image views
				for(int i = 0; i < 3; ++i)
				{
					if((res = viewImage[i].Create(200 + i * 500, 0, 700 + i * 500, 500)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// 각 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each image view.
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail() || (res = viewImage[1].SynchronizePointOfView(ref viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail() || (res = viewImage[1].SynchronizeWindow(ref viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 곡선 y = 0.004 * (x - 250)^2 + 200 위의 점들에 노이즈를 추가하여 데이터 생성
				// Generate data by adding noise to points on the curve y = 0.004 * (x - 250)^2 + 200
				CFLPointArray flpaData = new CFLPointArray();

				for(int i = 0; i <= 40; ++i)
				{
					double f64X = 50.0 + i * 10.0;
					double f64Y = 0.004 * (f64X - 250.0) * (f64X - 250.0) + 200.0 + CRandomGenerator.Double(-5.0, 5.0);

					// 일부 점은 곡선에서 크게 벗어나도록 만든다. // Make some points deviate greatly from the curve.
					if(i == 6)
						f64Y -= 90.0;
					else if(i == 20)
						f64Y += 110.0;
					else if(i == 33)
						f64Y += 100.0;

					// 일부 점은 곡선에서 조금 벗어나도록 만든다. (임계값이 작을 때만 제외됨)
					// Make some points deviate slightly from the curve. (Excluded only when the threshold is small)
					if(i == 13)
						f64Y -= 25.0;
					else if(i == 27)
						f64Y += 25.0;

					flpaData.PushBack(new CFLPoint<double>(f64X, f64Y));
				}

				// CLeastSquares<double> 객체 생성 // Create CLeastSquares<double> object
				CLeastSquares<double> leastSquares = new CLeastSquares<double>();

				// 데이터를 할당 // Assign data
				if((res = leastSquares.Assign(flpaData)).IsFail())
				{
					ErrorPrint(res, "Failed to assign data.\n");
					break;
				}

				// 피팅할 다항식의 차수 // Degree of the polynomial to fit
				const int i32Degree = 2;

				// 각 뷰에서 사용할 Outlier 파라미터 // Outlier parameters for each view
				string[] arrStrViewName = { "Default", "Parameter 1", "Parameter 2" };
				long[] arrI64OutlierThresholdCount = { 0, 1, 3 };
				double[] arrF64OutlierThreshold = { 3.0, 3.0, 1.5 };

				Console.WriteLine("Source Points");
				Console.WriteLine("{0}\n", CFigureUtilities.ConvertFigureObjectToString(flpaData));

				for(int i = 0; i < 3; ++i)
				{
					// 계수 값을 받기 위해 List 생성 // Create List to receive coefficient values
					List<double> listCoef = new List<double>();
					// R square 값을 받기 위해 double 선언 // Declare double to receive R square value
					double f64RSquare = 0.0;
					// 제외된 Outlier 점들의 인덱스를 받기 위해 List 생성 // Create List to receive the indices of excluded outlier points
					List<long> listOutlierIndices = new List<long>();

					// 다항식 계수를 얻는다. (내림차순 : a, b, c → y = ax^2 + bx + c)
					// Get polynomial coefficients. (Descending order : a, b, c -> y = ax^2 + bx + c)
					if((res = leastSquares.GetPoly(i32Degree, ref listCoef, ref f64RSquare, ECoefficientOrder.Descending, ELeastSqauresAccuracy.TemplateType, arrI64OutlierThresholdCount[i], arrF64OutlierThreshold[i], ref listOutlierIndices)).IsFail())
					{
						ErrorPrint(res, "Failed to calculate.\n");
						break;
					}

					// Outlier 인덱스로 Outlier PointArray 생성 // Create Outlier PointArray from the outlier indices
					CFLPointArray flpaOutlier = new CFLPointArray();

					foreach(long i64Index in listOutlierIndices)
						flpaOutlier.PushBack(flpaData.GetAt(i64Index));

					// 얻어온 계수로 곡선을 계산하여 짧은 선분들로 표현 // Calculate the curve with the obtained coefficients and represent it with short line segments
					CFLFigureArray flfaCurve = new CFLFigureArray();
					double f64PrevX = 0.0;
					double f64PrevY = 0.0;

					for(int x = 30; x <= 470; x += 4)
					{
						double f64X = x;
						double f64Y = 0.0;

						foreach(double f64Coef in listCoef)
							f64Y = f64Y * f64X + f64Coef;

						if(x != 30)
							flfaCurve.PushBack(new CFLLine<double>(f64PrevX, f64PrevY, f64X, f64Y));

						f64PrevX = f64X;
						f64PrevY = f64Y;
					}

					// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					CGUIViewImageLayer layer = viewImage[i].GetLayer(0);

					string strParameter;

					if(arrI64OutlierThresholdCount[i] != 0)
						strParameter = String.Format("Outlier Threshold Count : {0}\r\nOutlier Threshold : {1:F3}", arrI64OutlierThresholdCount[i], arrF64OutlierThreshold[i]);
					else
						strParameter = String.Format("Outlier Threshold Count : {0}", arrI64OutlierThresholdCount[i]);

					string strResult = String.Format("y = {0:F6}x^2 {1:+0.000000;-0.000000}x {2:+0.000000;-0.000000}\r\nR square : {3:F6}\r\nOutlier Count : {4}", listCoef[0], listCoef[1], listCoef[2], f64RSquare, listOutlierIndices.Count);

					// 레이어에 Figure 와 Text 를 출력 // Draw Figure and Text to the layer
					// 피팅 결과 곡선 : CYAN, 데이터 : LIME, 제외된 Outlier 점 : RED
					// Fitted curve : CYAN, Data : LIME, Excluded outlier points : RED
					layer.DrawFigureImage(flfaCurve, EColor.BLACK, 5);
					layer.DrawFigureImage(flfaCurve, EColor.CYAN, 3);
					layer.DrawFigureImage(flpaData, EColor.BLACK, 3);
					layer.DrawFigureImage(flpaData, EColor.LIME, 1);
					layer.DrawFigureImage(flpaOutlier, EColor.RED, 1);
					layer.DrawTextCanvas(new CFLPoint<int>(0, 0), arrStrViewName[i], EColor.YELLOW, EColor.BLACK, 30);
					layer.DrawTextCanvas(new CFLPoint<int>(0, 40), strParameter, EColor.YELLOW, EColor.BLACK, 15);
					layer.DrawTextCanvas(new CFLPoint<int>(0, 80), strResult, EColor.YELLOW, EColor.BLACK, 15);

					// Console 출력 // Console output
					Console.WriteLine("[{0}]", arrStrViewName[i]);
					Console.WriteLine(strParameter);
					Console.WriteLine("{0}\n", strResult);
				}

				if(res.IsFail())
					break;

				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 3; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 셋중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the three image views are turned off
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

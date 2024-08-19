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

namespace Histogram
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
			// 이미지 객체 선언 // Declare the image object
			CFLImage fliISrcImage = new CFLImage();
			CFLImage fliIDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();
			CGUIViewGraph viewGraph = new CGUIViewGraph();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/Histogram/Escherichia coli.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(100, 0, 100 + 440, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((res = viewGraph.Create(100 + 440 * 1, 0, 100 + 440 * 2, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage.SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 윈도우의 위치를 동기화 한다 // / Synchronize the positions of windows
				if((res = viewImage.SynchronizeWindow(ref viewGraph)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Histogram  객체 생성 // Create Histogram object
				CHistogram Histogram = new CHistogram();

				// ROI 지정 // Create ROI
				CFLRect<double> flrSrcROI = new CFLRect<double>(161, 181, 293, 302);

				// Source 이미지 설정 // Set source image 
				Histogram.SetSourceImage(ref fliISrcImage);

				// Source ROI 영역 지정 // set Source ROI 
				Histogram.SetSourceROI(flrSrcROI);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (Histogram.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Histogram.");
					break;
				}

				// Result 결과 갯수 확인 // get result count
				Int64 i64IndexCount = Histogram.GetResultCount();

				// Channel 값 표기를 위한 String 변수 // string variable to indicate Channel value
				string strChannel;
				// 그래프 선 색상 // graph line color
				EColor[] arrColor = new EColor[3];

				arrColor[0] = EColor.BLUE;
				arrColor[1] = EColor.LIGHTRED;
				arrColor[2] = EColor.GREEN;

				// Histogram 결과값 // Histogram result object
				List<uint> listResult = new List<uint>();

				for(Int64 i = 0; i < i64IndexCount; ++i)
				{
					// 이전 데이터 삭제 // data clear
					listResult.Clear();

					// Histogram 결과 값 가져오기 // get histogram result
					if((res = (Histogram.GetResult(i, out listResult))).IsFail())
					{
						Console.WriteLine("No Result.");
						break;
					}

					// 채널 String // Channel String
					strChannel = String.Format("Ch{0}", i);

					// Graph View 데이터 입력 // Input Graph View Data
					viewGraph.Plot(listResult, EChartType.Bar, arrColor[i], strChannel);
				}

				CGUIViewImageLayer layer1 = viewImage.GetLayer(0);

				// ROI 영역 표기 // ROI Area draw
				if((res = layer1.DrawFigureImage(flrSrcROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");

				viewImage.Invalidate(true);

				CMultiVar<double> mvMean = new CMultiVar<double>();
				CMultiVar<double> mvVariance = new CMultiVar<double>();
				CMultiVar<double> mvStdDev = new CMultiVar<double>();
				CMultiVar<double> mvMedian = new CMultiVar<double>();

				// 평균, 분산, 표준편차, 중앙값 받기 // get mean, variance, standard deviation, median
				Histogram.GetResultMean(out mvMean);
				Histogram.GetResultVariance(out mvVariance);
				Histogram.GetResultStdDev(out mvStdDev);
				Histogram.GetResultMedian(out mvMedian);

				// 출력 갯수 // get count
				int i32ResultCount = (int)mvMean.GetCount();

				// 평균, 분산, 표준편차, 중앙값 출력 // display Mean, variance, standard deviation, median
				for(int i = 0; i < i32ResultCount; ++i)
				{
					Console.WriteLine(String.Format("Channel {0}", i));
					Console.WriteLine(String.Format("Mean {0}", mvMean.GetAt(i)));
					Console.WriteLine(String.Format("Variance {0}", mvVariance.GetAt(i)));
					Console.WriteLine(String.Format("Standard Deviation {0}", mvStdDev.GetAt(i)));
					Console.WriteLine(String.Format("Median {0}\n", mvMedian.GetAt(i)));
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

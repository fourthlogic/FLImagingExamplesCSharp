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

namespace Projection
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
				if((res = fliISrcImage.Load("../../ExampleImages/Projection/mountains.flif")).IsFail())
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
				if((res = viewImage.SynchronizeWindow(viewGraph)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Projection 객체 생성 // Create Projection object
				CProjection Projection = new CProjection();

				// Source 이미지 설정 // Set source image 
				Projection.SetSourceImage(ref fliISrcImage);

				// 연산 방향 설정 // Set operation direction
				Projection.SetProjectionMode(CProjection.EProjectionDirection.Column);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (Projection.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Projection.");
					break;
				}

				// Result 결과 갯수 확인 // get result count
				Int64 i64IndexCount = Projection.GetResultCount();

				// Channel 값 표기를 위한 String 변수 // string variable to indicate Channel value
				string strChannel;
				// 그래프 선 색상 // Graph line color
				EColor[] arrColor = new EColor[3];

				arrColor[0] = EColor.BLUE;
				arrColor[1] = EColor.LIGHTRED;
				arrColor[2] = EColor.GREEN;

				// Projection 결과값 // Projection Result Object
				List<double> listResult = new List<double>();

				for(Int64 i = 0; i < i64IndexCount; ++i)
				{
					// 이전 데이터 삭제 // data clear
					listResult.Clear();

					// Projection 결과 값 가져오기 // get projection result
					if((res = (Projection.GetResult(i, out listResult))).IsFail())
					{
						ErrorPrint(res, "Failed to Get Result.");
						break;
					}

					// 채널 String // Channel String
					strChannel = String.Format("Ch{0}", i);

					// Graph View 데이터 입력 // Input Graph View Data
					viewGraph.Plot(listResult, EChartType.Line, arrColor[i], strChannel);
				}

				CGUIViewImageLayer layer = viewImage.GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// Text 출력 // Display Text 
				if((res = layer.DrawTextImage(flpTemp, "Source Image", EColor.RED)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

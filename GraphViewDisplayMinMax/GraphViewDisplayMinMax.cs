using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

using System.Diagnostics;
using System.Runtime.InteropServices;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;
using CResult = FLImagingCLR.CResult;

namespace GraphView
{
	class GraphViewDisplayMinMax
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

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph[] arrViewGraph = new CGUIViewGraph[3];

			for(int i = 0; i < 3; ++i)
			{
				arrViewGraph[i] = new CGUIViewGraph();
			}

			do
			{
				// 동작 결과 // operation result
				CResult res;

				// Graph 뷰 생성 // Create graph view
				if((res = arrViewGraph[0].Create(100, 0, 100 + 440, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the graph view.\n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((res = arrViewGraph[1].Create(100 + 440 * 1, 0, 100 + 440 * 2, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the graph view.\n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((res = arrViewGraph[2].Create(100 + 440 * 2, 0, 100 + 440 * 3, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the graph view.\n");
					break;
				}

				// 윈도우의 위치를 동기화 // / Synchronize the positions of windows
				if((res = arrViewGraph[0].SynchronizeWindow(ref arrViewGraph[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 윈도우의 위치를 동기화 // / Synchronize the positions of windows
				if((res = arrViewGraph[1].SynchronizeWindow(ref arrViewGraph[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}


				// 3 개의 차트를 생성 // Create 3 charts.
				for(int k = 0; k < 3; ++k)
				{
					// 랜덤으로 100개의 데이터를 생성
					Random rand = new Random(k * 2 + Environment.TickCount);
					const int i32DataCount = 100;
					double[] arrF64DataX = new double[i32DataCount];
					double[] arrF64DataY = new double[i32DataCount];

					double f64PrevX = 0;
					double f64PrevY = 0;

					for(int i = 0; i < i32DataCount; ++i)
					{
						arrF64DataX[i] = f64PrevX + ((rand.Next() % 100) / 10);
						if(rand.Next() % 2 != 0)
							arrF64DataY[i] = f64PrevY + ((rand.Next() % 100) / 10);
						else
							arrF64DataY[i] = f64PrevY - ((rand.Next() % 100) / 10);

						f64PrevX = arrF64DataX[i];
						f64PrevY = arrF64DataY[i];
					}

					EColor eColor = new EColor();
					eColor = (EColor)((uint)(((char)(rand.Next() % 255) | ((uint)((char)(rand.Next() % 255)) << 8)) | (((uint)(char)(rand.Next() % 255)) << 16)));

					string strName = string.Format("Chart {0}", k);

					// 그래프에 생성한 데이터를 추가한다. // Plot chart on the graph view.
					arrViewGraph[0].Plot(arrF64DataX, arrF64DataY, i32DataCount, EChartType.Line, eColor, strName);
					arrViewGraph[1].Plot(arrF64DataX, arrF64DataY, i32DataCount, EChartType.Line, eColor, strName);
					arrViewGraph[2].Plot(arrF64DataX, arrF64DataY, i32DataCount, EChartType.Line, eColor, strName);

					Thread.Sleep(5);
				}


				// 전체 차트에 대한 Y 축의 최솟값과 차트 이름 표시 // Display the Y-Axis minimum value for the entire charts and name.
				arrViewGraph[0].IndicateEntireChart(EViewGraphExtrema.MinY, EViewGraphIndicateType.Value | EViewGraphIndicateType.Name | EViewGraphIndicateType.Arrow);

				// 전체 차트에 대한 Y 축의 최댓값과 이름 표시 // Display the Y-Axis maximum value for the entire charts and name.
				arrViewGraph[0].IndicateEntireChart(EViewGraphExtrema.MaxY, EViewGraphIndicateType.Value | EViewGraphIndicateType.Name | EViewGraphIndicateType.Arrow);


				// 모든 차트에 각각 X 축의 최대, 최솟값과 Y 축의 최대, 최솟값을 표시 // Display the range and label for every individual chart.
				arrViewGraph[1].IndicateEveryIndividualChart(EViewGraphExtrema.MinX, EViewGraphIndicateType.All);
				arrViewGraph[1].IndicateEveryIndividualChart(EViewGraphExtrema.MaxX, EViewGraphIndicateType.All);
				arrViewGraph[1].IndicateEveryIndividualChart(EViewGraphExtrema.MinY, EViewGraphIndicateType.All);
				arrViewGraph[1].IndicateEveryIndividualChart(EViewGraphExtrema.MaxY, EViewGraphIndicateType.All);


				// 특정 차트(2번째 차트)에 Y 축의 최대/최소값을 표시 // Display the Y-Axis minimum/maximum value and name for a particular chart(chart index 0).
				int i32ChartIndex = 2;
				arrViewGraph[2].Indicate(i32ChartIndex, EViewGraphExtrema.MinX, EViewGraphIndicateType.None);
				arrViewGraph[2].Indicate(i32ChartIndex, EViewGraphExtrema.MaxX, EViewGraphIndicateType.None);
				arrViewGraph[2].Indicate(i32ChartIndex, EViewGraphExtrema.MinY, EViewGraphIndicateType.All);
				arrViewGraph[2].Indicate(i32ChartIndex, EViewGraphExtrema.MaxY, EViewGraphIndicateType.All);

				// Graph 뷰의 마우스 커서 위치에 십자선 표시 해제 // Set the crosshair on mouse cursor invisible.
				arrViewGraph[0].ShowCrosshair(false);
				arrViewGraph[1].ShowCrosshair(false);
				arrViewGraph[2].ShowCrosshair(false);

				// Graph 뷰의 스케일을 조정 // Sets the scales of the graph view.
				arrViewGraph[0].ZoomFit();
				arrViewGraph[1].ZoomFit();
				arrViewGraph[2].ZoomFit();


				// 그래프 뷰가 종료될 때 까지 기다림 // Wait until the graph view closed
				while(arrViewGraph[0].IsAvailable() && arrViewGraph[1].IsAvailable() && arrViewGraph[2].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

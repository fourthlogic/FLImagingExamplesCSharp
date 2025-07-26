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

namespace FLImagingExamplesCSharp
{
	class GraphViewTheme
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
			CGUIViewGraph viewGraphDark = new CGUIViewGraph();
			CGUIViewGraph viewGraphLight = new CGUIViewGraph();

			do
			{
				// 동작 결과 // operation result
				CResult res;

				// Graph 뷰 생성 // Create graph view
				if((res = viewGraphDark.Create(100, 0, 100 + 440, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the graph view.\n");
					break;
				}
				
				// Graph 뷰 생성 // Create graph view
				if((res = viewGraphLight.Create(100 + 440 * 1, 0, 100 + 440 * 2, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the graph view.\n");
					break;
				}

				// 윈도우의 위치를 동기화 한다 // / Synchronize the positions of windows
				if((res = viewGraphLight.SynchronizeWindow(ref viewGraphDark)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Graph 뷰 테마를 다크모드로 설정 // Sets the theme of the graph view to dark mode.
				viewGraphDark.SetDarkMode();

				// Graph 뷰 테마를 라이트모드로 설정 // Sets the theme of the graph view to light mode.
				viewGraphLight.SetLightMode();

				// 랜덤으로 100개의 데이터를 생성한다.
				Random rand = new Random();
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

				string strName = "Chart";

				// 그래프에 생성한 데이터를 추가한다.
				viewGraphDark.Plot(arrF64DataX, arrF64DataY, i32DataCount, EChartType.Scatter, eColor, strName);
				viewGraphLight.Plot(arrF64DataX, arrF64DataY, i32DataCount, EChartType.Scatter, eColor, strName);

				// Graph 뷰의 스케일을 조정 // Sets the scales of the graph view.
				viewGraphDark.ZoomFit();
				viewGraphLight.ZoomFit();

				// 그래프 뷰가 종료될 때 까지 기다림
				while(viewGraphDark.IsAvailable() && viewGraphLight.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

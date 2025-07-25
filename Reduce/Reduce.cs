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

using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
	class Reduce
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

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

			CResult res;

			do
			{
				// Source View 생성 // Create Source View
				if((res = (viewImage[0].Create(200, 0, 700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Reduce result1 View 생성 // Create Reduce result1 view
				if((res = (viewImage[1].Create(700, 0, 1200, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Reduce result2 View 생성 // Create Reduce result2 view
				if((res = (viewImage[2].Create(1200, 0, 1700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 각 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each image view.
				if((res = (viewImage[0].SynchronizePointOfView(ref viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				if((res = (viewImage[1].SynchronizePointOfView(ref viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = (viewImage[0].SynchronizeWindow(ref viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}
				if((res = (viewImage[1].SynchronizeWindow(ref viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] layer = { viewImage[0].GetLayer(0), viewImage[1].GetLayer(0), viewImage[2].GetLayer(0) };

				// 화면상 좌표(고정 좌표)에 Source Figure View 임을 표시
				// Indicates Source Figure View on screen coordinates (fixed coordinates)
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 0), "Source Figure", EColor.YELLOW, EColor.BLACK, 30);
				// 화면상 좌표(고정 좌표)에 Reduce Result View 임을 표시
				// Indicates Reduce Result View on screen coordinates (fixed coordinates)
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 0), "Reduce Result1", EColor.YELLOW, EColor.BLACK, 30);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 0), "Reduce Result2", EColor.YELLOW, EColor.BLACK, 30);

				CFLRegion flrgSourceFig = new CFLRegion();

				// Source Figure 불러오기 // Load source figure
				if((res = flrgSourceFig.Load("../../ExampleImages/Figure/RegionForReduce.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// 0번 Layer 에 Figure 와 Text 를 출력 // Draw Figure and Text to Layer 0
				layer[0].DrawFigureImage(flrgSourceFig, EColor.LIME, 3);
				layer[0].DrawFigureImage(new CFLPointArray(flrgSourceFig), EColor.BLACK, 1);
				layer[0].DrawTextImage(flrgSourceFig.GetCenter(), String.Format("vertex count : {0}", flrgSourceFig.GetCount()), EColor.LIME, EColor.BLACK, 17, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);


				// Reduce 함수 실행 (Epsilon : 10.0) // Reduce function execution (Epsilon : 10.0)
				CFLRegion flrgResult1 = new CFLRegion();
				double f64Epsilon1 = 10.0;

				if((res = flrgSourceFig.Reduce(f64Epsilon1, true, ref flrgResult1)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Reduce 함수 실행 (Epsilon : 15.0) // Reduce function execution (Epsilon : 15.0)
				CFLRegion flrgResult2 = new CFLRegion();
				double f64Epsilon2 = 15.0;

				if((res = flrgSourceFig.Reduce(f64Epsilon2, true, ref flrgResult2)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// View 에 결과 Region 과 정점 그리기 // Draw the resulting Region and vertices in the View
				layer[1].DrawFigureImage(flrgResult1, EColor.CYAN, 3);
				layer[2].DrawFigureImage(flrgResult2, EColor.YELLOW, 3);
				layer[1].DrawFigureImage(new CFLPointArray(flrgResult1), EColor.BLACK, 1);
				layer[2].DrawFigureImage(new CFLPointArray(flrgResult2), EColor.BLACK, 1);
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 40), "epsilon : 10.0", EColor.YELLOW, EColor.BLACK, 20);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 40), "epsilon : 15.0", EColor.YELLOW, EColor.BLACK, 20);
				layer[1].DrawTextImage(flrgResult1.GetCenter(), String.Format("vertex count : {0}", flrgResult1.GetCount()), EColor.CYAN, EColor.BLACK, 17, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);
				layer[2].DrawTextImage(flrgResult2.GetCenter(), String.Format("vertex count : {0}", flrgResult2.GetCount()), EColor.YELLOW, EColor.BLACK, 17, false, 0.0, EGUIViewImageTextAlignment.CENTER_CENTER);

				// Console 출력 // Console output
				Console.Write("\nSource Region Points : \nvertex count = {0}\n\n", flrgSourceFig.GetCount());

				for(long i = 0; i < flrgSourceFig.GetCount(); ++i)
					Console.Write("[{0}] ({1:.000},{2:.000})\n", i, flrgSourceFig.GetAt(i).x, flrgSourceFig.GetAt(i).y);

				Console.Write("\n\nResult1 Region Points : \nepsilon = {0:.0}\nvertex count = {1}\n\n", f64Epsilon1, flrgResult1.GetCount());

				for(long i = 0; i < flrgResult1.GetCount(); ++i)
					Console.Write("[{0}] ({1:.000},{2:.000}\n", i, flrgResult1.GetAt(i).x, flrgResult1.GetAt(i).y);

				Console.Write("\n\nResult2 Region Points : \nepsilon = {0:.0}\nvertex count = {1}\n\n", f64Epsilon2, flrgResult2.GetCount());

				for(long i = 0; i < flrgResult2.GetCount(); ++i)
					Console.Write("[{0}] ({1:.000},{2:.000}\n", i, flrgResult2.GetAt(i).x, flrgResult2.GetAt(i).y);


				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 3; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 셋중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the three image views are turned off
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

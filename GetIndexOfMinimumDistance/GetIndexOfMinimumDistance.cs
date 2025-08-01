﻿using System;
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
	class GetIndexOfMinimumDistance
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
			CGUIViewImage[] viewImage = new CGUIViewImage[4];

			for(int i = 0; i < 4; ++i)
				viewImage[i] = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(400, 0, 812, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(812, 0, 1224, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[2].Create(400, 384, 812, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[3].Create(812, 384, 1224, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// SourceView, DstView 의 0번 레이어 가져오기 // Get Layer 0 of SourceView, DstView
				CGUIViewImageLayer layerSrc1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layerDst1 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer layerSrc2 = viewImage[2].GetLayer(0);
				CGUIViewImageLayer layerDst2 = viewImage[3].GetLayer(0);

				layerSrc1.DrawTextCanvas(new TPoint<double>(0, 0), "Source Figure 1", EColor.YELLOW, EColor.BLACK, 15);
				layerSrc2.DrawTextCanvas(new TPoint<double>(0, 0), "Source Figure 2", EColor.YELLOW, EColor.BLACK, 15);
				layerDst1.DrawTextCanvas(new TPoint<double>(0, 0), "Result Figure 1", EColor.YELLOW, EColor.BLACK, 15);
				layerDst2.DrawTextCanvas(new TPoint<double>(0, 0), "Result Figure 2", EColor.YELLOW, EColor.BLACK, 15);

				layerDst1.DrawTextCanvas(new TPoint<double>(0, 20), "Index of Minimum Distance", EColor.CYAN, EColor.BLACK);
				layerDst2.DrawTextCanvas(new TPoint<double>(0, 20), "Index of Minimum Distance", EColor.CYAN, EColor.BLACK);

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImage[2].SynchronizePointOfView(ref viewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				for(int i = 1; i < 4; ++i)
				{
					if((res = viewImage[0].SynchronizeWindow(ref viewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						break;
					}
				}

				// Figure 생성 // Create figure
				CFLPointArray flpaSource1 = new CFLPointArray();
				CFLCircle<double> flcDestination1 = new CFLCircle<double>();
				CFLFigureArray flfaSource2 = new CFLFigureArray();
				CFLFigureArray flfaDestination2 = new CFLFigureArray();

				// Source Figure 불러오기 // Load Source figure
				if((res = flpaSource1.Load("../../ExampleImages/Figure/PointArray1.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				if((res = flfaSource2.Load("../../ExampleImages/Figure/various_arrays.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// Destination Figure 불러오기 // Load Destination Figure
				if((res = flcDestination1.Load("../../ExampleImages/Figure/Circle2.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				if((res = flfaDestination2.Load("../../ExampleImages/Figure/Circles2.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// Figure 사이의 최소 거리를 나타내는 인덱스를 추출 // Get the index of representing the minimum distance between figures
				CFLFigureArray flfaResultSrc1 = new CFLFigureArray();

				if((res = flpaSource1.GetIndexOfMinimumDistance(flcDestination1, ref flfaResultSrc1)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				CFLFigureArray flfaResultSrc2 = new CFLFigureArray();
				CFLFigureArray flfaResultDst2 = new CFLFigureArray();

				if((res = flfaSource2.GetIndexOfMinimumDistance(flfaDestination2, ref flfaResultSrc2, true, true, ref flfaResultDst2)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// SourceView1의 0번 레이어에 Source, Destination Figure 그리기 // Draw Source and Destination Figure on Layer 0 of SourceView1
				layerSrc1.DrawFigureImage(flpaSource1, EColor.BLACK, 3);
				layerSrc1.DrawFigureImage(flpaSource1, EColor.LIME);
				layerSrc1.DrawFigureImage(flcDestination1, EColor.BLACK, 3);
				layerSrc1.DrawFigureImage(flcDestination1, EColor.KHAKI);

				for(long i = 0; i < flpaSource1.GetCount(); ++i)
					layerSrc1.DrawTextImage(flpaSource1.GetAt(i).GetCenter(), String.Format("{0}", i), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_BOTTOM);

				// DstView1의 0번 레이어에 결과 그리기 // Draw the result on layer 0 of DstView1
				CFLScalar<long> flvSrc = (CFLScalar<long>)flfaResultSrc1.Front();
				layerDst1.DrawFigureImage(flpaSource1, EColor.BLACK, 3);
				layerDst1.DrawFigureImage(flpaSource1, EColor.LIME);
				layerDst1.DrawFigureImage(flcDestination1, EColor.BLACK, 3);
				layerDst1.DrawFigureImage(flcDestination1, EColor.KHAKI);
				layerDst1.DrawFigureImage(flpaSource1.GetAt(flvSrc.v), EColor.CYAN, 3);
				layerDst1.DrawTextImage(flpaSource1.GetAt(flvSrc.v).GetCenter(), String.Format("{0}", flvSrc.v), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_BOTTOM);
				layerDst1.DrawTextImage(flcDestination1.GetCenter(), String.Format("{0}", flvSrc.v), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);

				// SourceView2의 0번 레이어에 Source, Destination Figure 그리기 // Draw Source and Destination Figure on Layer 0 of SourceView2
				layerSrc2.DrawFigureImage(flfaSource2, EColor.BLACK, 3);
				layerSrc2.DrawFigureImage(flfaSource2, EColor.LIME);
				layerSrc2.DrawFigureImage(flfaDestination2, EColor.BLACK, 3);
				layerSrc2.DrawFigureImage(flfaDestination2, EColor.KHAKI);

				for(long i = 0; i < flfaSource2.GetCount(); ++i)
				{
					CFLFigureArray flfaArrayDepth1 = (CFLFigureArray)flfaSource2.GetAt(i);
					CFLRect<double> flrBoundary = new CFLRect<double>();
					flfaArrayDepth1.GetBoundaryRect(ref flrBoundary);

					layerSrc2.DrawFigureImage(flrBoundary, EColor.LIGHTBLUE, 1);
					layerSrc2.DrawTextImage(flfaArrayDepth1.GetCenter(), String.Format("{0}", i), EColor.LIGHTBLUE, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);

					for(long j = 0; j < flfaArrayDepth1.GetCount(); ++j)
						layerSrc2.DrawTextImage(flfaArrayDepth1.GetAt(j).GetCenter(), String.Format("{0}", j), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
				}

				for(long i = 0; i < flfaDestination2.GetCount(); ++i)
					layerSrc2.DrawTextImage(flfaDestination2.GetAt(i).GetCenter(), String.Format("{0}", i), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);

				// DstView2의 0번 레이어에 결과 그리기 // Draw the result on layer 0 of DstView2
				layerDst2.DrawFigureImage(flfaSource2, EColor.BLACK, 3);
				layerDst2.DrawFigureImage(flfaSource2, EColor.LIME);
				layerDst2.DrawFigureImage(flfaDestination2, EColor.BLACK, 3);
				layerDst2.DrawFigureImage(flfaDestination2, EColor.KHAKI);


				CFLScalar<long> flvSrcDepth1 = (CFLScalar<long>)flfaResultSrc2.GetAt(0);
				CFLScalar<long> flvSrcDepth2 = (CFLScalar<long>)flfaResultSrc2.GetAt(1);

				CFLFigureArray flfaSrcDepth1 = (CFLFigureArray)flfaSource2.GetAt(flvSrcDepth1.v);
				CFLFigure flfSrcDepth1 = flfaSource2.GetAt(flvSrcDepth1.v);
				CFLFigure flfSrcDepth2 = ((CFLFigureArray)flfSrcDepth1).GetAt(flvSrcDepth2.v);

				CFLFigureArray flfaArraySrcDepth1 = (CFLFigureArray)flfaSource2.GetAt(flvSrcDepth1.v);
				CFLRect<double> flrBoundary2 = new CFLRect<double>();
				flfaArraySrcDepth1.GetBoundaryRect(ref flrBoundary2);

				layerDst2.DrawFigureImage(flrBoundary2, EColor.LIGHTORANGE, 1);
				layerDst2.DrawTextImage(flfaArraySrcDepth1.GetCenter(), String.Format("{0}", flvSrcDepth1.v), EColor.LIGHTORANGE, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);

				layerDst2.DrawTextImage(flfaArraySrcDepth1.GetAt(flvSrcDepth2.v).GetCenter(), String.Format("{0}", flvSrcDepth2.v), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
				layerDst2.DrawFigureImage(flfSrcDepth2, EColor.CYAN, 1);

				CFLScalar<long> flvDstDepth1 = (CFLScalar<long>)flfaResultDst2.GetAt(0);

				CFLFigure flfDstDepth1 = flfaDestination2.GetAt(flvDstDepth1.v);

				layerDst2.DrawTextImage(flfDstDepth1.GetCenter(), String.Format("{0}", flvDstDepth1.v), EColor.MAGENTA, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
				layerDst2.DrawFigureImage(flfDstDepth1, EColor.MAGENTA, 1);


				// Console 출력 // Console output
				Console.Write("Source1 CFLPointArray\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaSource1));

				Console.Write("Destination1 CFLCircle<double>\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcDestination1));

				Console.Write("Result1 Index of Minimum distance\n");
				Console.Write("{0}\n\n", flvSrc.v);

				Console.Write("\n\n");

				Console.Write("Source2 CFLFigureArray\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaSource2));

				Console.Write("Destination2 CFLFigureArray\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaDestination2));

				Console.Write("Src Result2 Index of Minimum distance\n");
				Console.Write("Depth1 : {0}\nDepth2 : {1}\n\n", flvSrcDepth1.v, flvSrcDepth2.v);

				Console.Write("Dst Result2 Index of Minimum distance\n");
				Console.Write("Depth1 : {0}\n\n", flvDstDepth1.v);

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < 4; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable() && viewImage[3].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

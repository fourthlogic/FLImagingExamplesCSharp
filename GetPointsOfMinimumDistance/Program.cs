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

namespace GetPointsOfMinimumDistance
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
				CGUIViewImageLayer Src1Layer0 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer Dst1Layer0 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer Src2Layer0 = viewImage[2].GetLayer(0);
				CGUIViewImageLayer Dst2Layer0 = viewImage[3].GetLayer(0);

				Src1Layer0.DrawTextCanvas(new TPoint<double>(0, 0), "Source Figure 1", EColor.YELLOW, EColor.BLACK, 15);
				Src2Layer0.DrawTextCanvas(new TPoint<double>(0, 0), "Source Figure 2", EColor.YELLOW, EColor.BLACK, 15);
				Dst1Layer0.DrawTextCanvas(new TPoint<double>(0, 0), "Result Figure 1", EColor.YELLOW, EColor.BLACK, 15);
				Dst2Layer0.DrawTextCanvas(new TPoint<double>(0, 0), "Result Figure 2", EColor.YELLOW, EColor.BLACK, 15);

				Dst1Layer0.DrawTextCanvas(new TPoint<double>(0, 20), "Minimum Distance", EColor.CYAN, EColor.BLACK);
				Dst2Layer0.DrawTextCanvas(new TPoint<double>(0, 20), "Minimum Distance", EColor.CYAN, EColor.BLACK);

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
				CFLCircle<double> flcSource1 = new CFLCircle<double>();
				CFLQuad<double> flqOperand1 = new CFLQuad<double>();
				CFLFigureArray flfaSource2 = new CFLFigureArray();
				CFLFigureArray flfaOperand2 = new CFLFigureArray();

				// Source Figure 불러오기 // Load source figure
				if((res = flcSource1.Load("../../ExampleImages/Figure/Circle1.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				if((res = flfaSource2.Load("../../ExampleImages/Figure/various shapes_Top.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// Operand Figure 불러오기 // Load Operand Figure
				if((res = flqOperand1.Load("../../ExampleImages/Figure/Quad1.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				if((res = flfaOperand2.Load("../../ExampleImages/Figure/various shapes_Bottom.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// Figure 사이의 최소 거리를 나타내는 점을 추출 // Get the point representing the minimum distance between figures
				CFLPointArray flpaResult1 = new CFLPointArray();

				if((res = flcSource1.GetPointsOfMinimumDistance(flqOperand1, ref flpaResult1)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				CFLPointArray flpaResult2 = new CFLPointArray();

				if((res = flfaSource2.GetPointsOfMinimumDistance(flfaOperand2, ref flpaResult2)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// Figure 사이의 최소 거리를 계산 // Calculate the minimum distance between figures
				double f64MinimumDistance1 = 0;

				if((res = flcSource1.GetMinimumDistance(flqOperand1, ref f64MinimumDistance1)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				double f64MinimumDistance2 = 0;

				if((res = flfaSource2.GetMinimumDistance(flfaOperand2, ref f64MinimumDistance2)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// 두 Point를 잇는 Line을 생성 // Create a line connecting two points
				CFLLine<double> fllMin1 = new CFLLine<double>(flpaResult1.GetAt(0), flpaResult1.GetAt(1));
				CFLLine<double> fllMin2 = new CFLLine<double>(flpaResult2.GetAt(0), flpaResult2.GetAt(1));

				// SourceView1의 0번 레이어에 Source, Operand Figure 그리기 // Draw Source and Operand Figure on Layer 0 of SourceView1
				Src1Layer0.DrawFigureImage(flcSource1, EColor.BLACK, 3);
				Src1Layer0.DrawFigureImage(flcSource1, EColor.KHAKI);
				Src1Layer0.DrawFigureImage(flqOperand1, EColor.BLACK, 3);
				Src1Layer0.DrawFigureImage(flqOperand1, EColor.LIME);

				// DstView1의 0번 레이어에 결과 그리기 // Draw the result on layer 0 of DstView1
				Dst1Layer0.DrawFigureImage(flcSource1, EColor.BLACK, 3);
				Dst1Layer0.DrawFigureImage(flcSource1, EColor.KHAKI);
				Dst1Layer0.DrawFigureImage(flqOperand1, EColor.BLACK, 3);
				Dst1Layer0.DrawFigureImage(flqOperand1, EColor.LIME);
				Dst1Layer0.DrawFigureImage(flpaResult1, EColor.CYAN, 3);
				Dst1Layer0.DrawFigureImage(fllMin1, EColor.CYAN);
				Dst1Layer0.DrawTextImage(fllMin1.GetCenter(), String.Format("{0:.000}", f64MinimumDistance1), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.RIGHT_TOP);

				// SourceView2의 0번 레이어에 Source, Operand Figure 그리기 // Draw Source and Operand Figure on Layer 0 of SourceView2
				Src2Layer0.DrawFigureImage(flfaSource2, EColor.BLACK, 3);
				Src2Layer0.DrawFigureImage(flfaSource2, EColor.KHAKI);
				Src2Layer0.DrawFigureImage(flfaOperand2, EColor.BLACK, 3);
				Src2Layer0.DrawFigureImage(flfaOperand2, EColor.LIME);

				// DstView2의 0번 레이어에 결과 그리기 // Draw the result on layer 0 of DstView2
				Dst2Layer0.DrawFigureImage(flfaSource2, EColor.BLACK, 3);
				Dst2Layer0.DrawFigureImage(flfaSource2, EColor.KHAKI);
				Dst2Layer0.DrawFigureImage(flfaOperand2, EColor.BLACK, 3);
				Dst2Layer0.DrawFigureImage(flfaOperand2, EColor.LIME);
				Dst2Layer0.DrawFigureImage(flpaResult2, EColor.CYAN, 3);
				Dst2Layer0.DrawFigureImage(fllMin2, EColor.CYAN);
				Dst2Layer0.DrawTextImage(fllMin2.GetCenter(), String.Format("{0:.000}", f64MinimumDistance2), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM);

				// Console 출력 // Console output
				Console.Write("Source1 CFLCircle<double>\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcSource1));

				Console.Write("Operand1 CFLQuad<double>\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flqOperand1));

				Console.Write("Result1 Points of Minimum distance\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaResult1));

				Console.Write("Result1 Minimum distance\n");
				Console.Write("{0}\n\n", f64MinimumDistance1);

				Console.Write("\n\n");

				Console.Write("Source2 CFLFigureArray\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaSource2));

				Console.Write("Operand2 CFLFigureArray\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaOperand2));

				Console.Write("Result2 Points of Minimum distance\n");
				Console.Write("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaResult2));

				Console.Write("Result2 Minimum distance\n");
				Console.Write("{0}\n\n", f64MinimumDistance2);

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

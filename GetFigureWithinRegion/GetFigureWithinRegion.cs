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

namespace Figure
{
	class GetFigureWithinRegion
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

			CResult res;

			do
			{
				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(400, 0, 912, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(912, 0, 1424, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[2].Create(400, 400, 912, 794)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[3].Create(912, 400, 1424, 794)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// SourceView, DstView 의 0번 레이어 가져오기 // Get Layer 0 of SourceView, DstView
				CGUIViewImageLayer SrcLayer0 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer DstLayer0 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer SrcLayer1 = viewImage[2].GetLayer(0);
				CGUIViewImageLayer DstLayer1 = viewImage[3].GetLayer(0);

				SrcLayer0.DrawTextCanvas(new TPoint<double>(0, 0), "Source Figure And Region1", EColor.YELLOW, EColor.BLACK, 15);
				DstLayer0.DrawTextCanvas(new TPoint<double>(0, 0), "Get Figure Within Region1", EColor.YELLOW, EColor.BLACK, 15);
				SrcLayer1.DrawTextCanvas(new TPoint<double>(0, 0), "Source Figure And Region2", EColor.YELLOW, EColor.BLACK, 15);
				DstLayer1.DrawTextCanvas(new TPoint<double>(0, 0), "Get Figure Within Region2", EColor.YELLOW, EColor.BLACK, 15);

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				for(int i = 1; i < 4; ++i)
				{
					if((res = viewImage[0].SynchronizePointOfView(ref viewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						break;
					}
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
				CFLLine<double> fll = new CFLLine<double>(76, 300, 130, 210);

				CFLRect<double> flr = new CFLRect<double>(50, 50, 100, 100);

				CFLCircle<double> flc = new CFLCircle<double>(150.0, 100.0, 30.0, 0.0, 0.0, 80.0, EArcClosingMethod.Center);

				CFLEllipse<double> fle = new CFLEllipse<double>(300, 150, 100, 50, 0, 180, 60, EArcClosingMethod.EachOther);

				CFLComplexRegion flcr = new CFLComplexRegion();
				flcr.PushBack(new CFLPoint<double>(270, 100));
				flcr.PushBack(new CFLPoint<double>(420, 160));
				flcr.PushBack(new CFLPoint<double>(300, 200));

				CFLFigureArray flfaSource = new CFLFigureArray();

				flfaSource.PushBack(fll);
				flfaSource.PushBack(flr);
				flfaSource.PushBack(flc);
				flfaSource.PushBack(fle);
				flfaSource.PushBack(flcr);

				// Region 생성 // Create region
				CFLComplexRegion flcrRegion1 = new CFLComplexRegion();

				flcrRegion1.PushBack(new CFLPoint<double>(0, 0));
				flcrRegion1.PushBack(new CFLPoint<double>(220, 50));
				flcrRegion1.PushBack(new CFLPoint<double>(240, 100));
				flcrRegion1.PushBack(new CFLPoint<double>(200, 150));
				flcrRegion1.PushBack(new CFLPoint<double>(110, 170));
				flcrRegion1.PushBack(new CFLPoint<double>(70, 200));

				CFLComplexRegion flcrRegion2 = new CFLComplexRegion();

				flcrRegion2.PushBack(new CFLPoint<double>(150, 100));
				flcrRegion2.PushBack(new CFLPoint<double>(240, 160));
				flcrRegion2.PushBack(new CFLPoint<double>(430, 250));
				flcrRegion2.PushBack(new CFLPoint<double>(300, 400));
				flcrRegion2.PushBack(new CFLPoint<double>(200, 300));
				flcrRegion2.PushBack(new CFLPoint<double>(140, 200));
				flcrRegion2.PushBack(new CFLPoint<double>(110, 80));

				String strFigure;
				Console.WriteLine("Source Figure Array\n");

				strFigure = string.Format("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaSource));
				Console.WriteLine("{0}", strFigure);

				Console.WriteLine("Region1\n");

				strFigure = string.Format("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcrRegion1));
				Console.WriteLine("{0}", strFigure);

				// SourceView1의 0번 레이어에 Source Figure, Region1 그리기 // Draw Source Figure, Region1 on Layer 0 of SourceView1
				SrcLayer0.DrawFigureImage(flfaSource, EColor.CYAN);
				SrcLayer0.DrawFigureImage(flcrRegion1, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1, 0.2f);

				Console.WriteLine("Region2\n");

				strFigure = string.Format("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flcrRegion2));
				Console.WriteLine("{0}", strFigure);

				// SourceView2의 0번 레이어에 Source Figure, Region2 그리기 // Draw Source Figure, Region2 on Layer 0 of SourceView2
				SrcLayer1.DrawFigureImage(flfaSource, EColor.CYAN);
				SrcLayer1.DrawFigureImage(flcrRegion2, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1, 0.2f);

				// Region1과 겹쳐지는 Figure 추출 // Get figure overlapping with Region1
				CFLFigureArray flfaResult1 = new CFLFigureArray();
				flfaSource.GetFigureWithinRegion(flcrRegion1, ref flfaResult1);

				Console.WriteLine("Result Figure Within Region1\n");

				strFigure = string.Format("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaResult1));
				Console.WriteLine("{0}", strFigure);

				// DstView1의 0번 레이어에 결과 그리기 // Draw the result on layer 0 of DstView1
				DstLayer0.DrawFigureImage(flfaSource, EColor.CYAN);
				DstLayer0.DrawFigureImage(flcrRegion1, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1, 0.2f);
				DstLayer0.DrawFigureImage(flfaResult1, EColor.LIME, 3, EColor.LIME, EGUIViewImagePenStyle.Solid, 1, 0.2f);

				// Region2과 겹쳐지는 Figure 추출 // Get figure overlapping with Region2
				CFLFigureArray flfaResult2 = new CFLFigureArray();
				flfaSource.GetFigureWithinRegion(flcrRegion2, ref flfaResult2);

				Console.WriteLine("Result Figure Within Region2\n");

				strFigure = string.Format("{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaResult2));
				Console.WriteLine("{0}", strFigure);

				// DstView1의 0번 레이어에 결과 그리기 // Draw the result on layer 0 of DstView1
				DstLayer1.DrawFigureImage(flfaSource, EColor.CYAN);
				DstLayer1.DrawFigureImage(flcrRegion2, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1, 0.2f);
				DstLayer1.DrawFigureImage(flfaResult2, EColor.LIME, 3, EColor.LIME, EGUIViewImagePenStyle.Solid, 1, 0.2f);

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

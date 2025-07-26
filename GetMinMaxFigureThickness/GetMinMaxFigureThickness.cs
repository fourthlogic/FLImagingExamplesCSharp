using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CResult = FLImagingCLR.CResult;

namespace Figure
{
	class GetMinMaxFigureThickness
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

				// Minimum Thickness View, Maximum Thickness View 의 0번 레이어 가져오기 // Get Layer 0 of Minimum Thickness View, Maximum Thickness View
				CGUIViewImageLayer layerMin0 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layerMax0 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer layerMin1 = viewImage[2].GetLayer(0);
				CGUIViewImageLayer layerMax1 = viewImage[3].GetLayer(0);

				layerMin0.DrawTextCanvas(new TPoint<double>(0, 0), "Minimum Thickness", EColor.YELLOW, EColor.BLACK, 15);
				layerMax0.DrawTextCanvas(new TPoint<double>(0, 0), "Maximum Thickness", EColor.YELLOW, EColor.BLACK, 15);
				layerMin1.DrawTextCanvas(new TPoint<double>(0, 0), "Minimum Thickness", EColor.YELLOW, EColor.BLACK, 15);
				layerMax1.DrawTextCanvas(new TPoint<double>(0, 0), "Maximum Thickness", EColor.YELLOW, EColor.BLACK, 15);
				layerMin0.DrawTextCanvas(new TPoint<double>(0, 20), "Trim Ratio : Default", EColor.YELLOW, EColor.BLACK);
				layerMax0.DrawTextCanvas(new TPoint<double>(0, 20), "Trim Ratio : Default", EColor.YELLOW, EColor.BLACK);
				layerMin1.DrawTextCanvas(new TPoint<double>(0, 20), "Trim Ratio : 0.01", EColor.YELLOW, EColor.BLACK);
				layerMax1.DrawTextCanvas(new TPoint<double>(0, 20), "Trim Ratio : 0.05", EColor.YELLOW, EColor.BLACK);

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

				// 화면상에 잘 보이도록 좌표 0.5배율을 적용 // Apply 0.5 magnification to the coordinates so that they can be seen clearly on the screen
				double f64Scale = 0.5;
				// 화면상에 잘 보이도록 시점 Offset 조정 // Adjust the viewpoint offset so that it can be seen clearly on the screen
				double f64CenterCoordX = 500;
				double f64CenterCoordY = 500;
				viewImage[0].SetViewCenterAndScale(new CFLPoint<double>(f64CenterCoordX, f64CenterCoordY), f64Scale);

				// Source Figure 불러오기 // Load source figure
				CFLFigure flfSource = CFigureUtilities.LoadFigure("../../ExampleImages/Figure/Thickness1.fig");

				// 도형의 최소 두께를 나타내는 점을 얻어옴 // Get a point representing the minimum thickness of the figure
				CFLPointArray flpaResultMinPoints1 = new CFLPointArray();

				if((res = flfSource.GetPointsOfMinimumThickness(ref flpaResultMinPoints1)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// 두께를 측정한 값들에 대해 Trimming 파라미터 적용하여 계산 // Calculated by applying trimming parameters to the measured thickness values
				CFLPointArray flpaResultMinPoints2 = new CFLPointArray();

				if((res = flfSource.GetPointsOfMinimumThickness(ref flpaResultMinPoints2, 0.01)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// 도형의 최대 두께를 나타내는 점을 얻어옴 // Get a point representing the maximum thickness of the figure
				CFLPointArray flpaResultMaxPoints1 = new CFLPointArray();

				if((res = flfSource.GetPointsOfMaximumThickness(ref flpaResultMaxPoints1)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// 두께를 측정한 값들에 대해 Trimming 파라미터 적용하여 계산 // Calculated by applying trimming parameters to the measured thickness values
				CFLPointArray flpaResultMaxPoints2 = new CFLPointArray();

				if((res = flfSource.GetPointsOfMaximumThickness(ref flpaResultMaxPoints2, 0.05)).IsFail())
				{
					ErrorPrint(res, "Failed to process.\n");
					break;
				}

				// 도형의 최소 두께를 얻어옴 // Get the minimum thickness of the figure
				double f64MinimumThickness1 = flfSource.GetMinimumThickness();

				// 두께를 측정한 값들에 대해 Trimming 파라미터 적용하여 계산 // Calculated by applying trimming parameters to the measured thickness values
				double f64MinimumThickness2 = flfSource.GetMinimumThickness(0.01);

				// 도형의 최대 두께를 얻어옴 // Get the minimum thickness of the figure
				double f64MaximumThickness1 = flfSource.GetMaximumThickness();

				// 두께를 측정한 값들에 대해 Trimming 파라미터 적용하여 계산 // Calculated by applying trimming parameters to the measured thickness values
				double f64MaximumThickness2 = flfSource.GetMaximumThickness(0.05);

				// 각각의 레이어에 Source Figure 그리기 // Draw source figure on each layer
				layerMin0.DrawFigureImage(flfSource, EColor.BLACK, 3);
				layerMin0.DrawFigureImage(flfSource, EColor.LIME);
				layerMin1.DrawFigureImage(flfSource, EColor.BLACK, 3);
				layerMin1.DrawFigureImage(flfSource, EColor.LIME);
				layerMax0.DrawFigureImage(flfSource, EColor.BLACK, 3);
				layerMax0.DrawFigureImage(flfSource, EColor.LIME);
				layerMax1.DrawFigureImage(flfSource, EColor.BLACK, 3);
				layerMax1.DrawFigureImage(flfSource, EColor.LIME);


				// 각각의 레이어에 결과 Point Figure 와 거리값 그리기 // Draw the resulting point figure and distance value on each layer.

				CFLPoint<double> flpForDrawMinPoints1 = new CFLPoint<double>(flpaResultMinPoints1.GetCenter());
				CFLPoint<double> flpForDrawMinPoints2 = new CFLPoint<double>(flpaResultMinPoints2.GetCenter());
				CFLPoint<double> flpForDrawMaxPoints1 = new CFLPoint<double>(flpaResultMaxPoints1.GetCenter());
				CFLPoint<double> flpForDrawMaxPoints2 = new CFLPoint<double>(flpaResultMaxPoints2.GetCenter());
				flpForDrawMinPoints1.Offset(0, 20);
				flpForDrawMinPoints2.Offset(0, 20);
				flpForDrawMaxPoints1.Offset(0, 20);
				flpForDrawMaxPoints2.Offset(0, 20);

				layerMin0.DrawFigureImage(flpaResultMinPoints1, EColor.BLACK, 3);
				layerMin0.DrawFigureImage(flpaResultMinPoints1, EColor.MAGENTA);
				layerMin0.DrawTextImage(flpForDrawMinPoints1, String.Format("{0:.000}", f64MinimumThickness1), EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_TOP);

				layerMin1.DrawFigureImage(flpaResultMinPoints2, EColor.BLACK, 3);
				layerMin1.DrawFigureImage(flpaResultMinPoints2, EColor.MAGENTA);
				layerMin1.DrawTextImage(flpForDrawMinPoints2, String.Format("{0:.000}", f64MinimumThickness2), EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_TOP);

				layerMax0.DrawFigureImage(flpaResultMaxPoints1, EColor.BLACK, 3);
				layerMax0.DrawFigureImage(flpaResultMaxPoints1, EColor.CYAN);
				layerMax0.DrawTextImage(flpForDrawMaxPoints1, String.Format("{0:.000}", f64MaximumThickness1), EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_TOP);

				layerMax1.DrawFigureImage(flpaResultMaxPoints2, EColor.BLACK, 3);
				layerMax1.DrawFigureImage(flpaResultMaxPoints2, EColor.CYAN);
				layerMax1.DrawTextImage(flpForDrawMaxPoints2, String.Format("{0:.000}", f64MaximumThickness2), EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_TOP);

				// Console 출력 // Console output
				Console.Write("<Minimum Thickness>\nTrim Ratio : Default\n");
				Console.Write("Result Thickness : {0}\n", f64MinimumThickness1);
				Console.Write("Result Points : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaResultMinPoints1));

				Console.Write("<Maximum Thickness>\nTrim Ratio : Default\n");
				Console.Write("Result Thickness : {0}\n", f64MaximumThickness1);
				Console.Write("Result Points : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaResultMaxPoints1));

				Console.Write("<Minimum Thickness>\nTrim Ratio : 0.01\n");
				Console.Write("Result Thickness : {0}\n", f64MinimumThickness2);
				Console.Write("Result Points : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaResultMinPoints2));

				Console.Write("<Maximum Thickness>\nTrim Ratio : 0.05\n");
				Console.Write("Result Thickness : {0}\n", f64MaximumThickness2);
				Console.Write("Result Points : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flpaResultMaxPoints2));

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

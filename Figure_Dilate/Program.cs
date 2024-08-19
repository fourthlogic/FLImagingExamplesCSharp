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

namespace Figure_Dilate
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
			CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

			CResult res;

			do
			{
				// Source View 생성 // Create Source View
				if((res = (viewImage[0].Create(200, 0, 700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Dilate result1 View 생성 // Create Dilate result1 view
				if((res = (viewImage[1].Create(700, 0, 1200, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Dilate result2 View 생성 // Create Dilate result2 view
				if((res = (viewImage[2].Create(200, 500, 700, 1000))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Dilate result3 View 생성 // Create Dilate result3 view
				if((res = (viewImage[3].Create(700, 500, 1200, 1000))).IsFail())
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
				if((res = (viewImage[0].SynchronizePointOfView(ref viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				if((res = (viewImage[0].SynchronizePointOfView(ref viewImage[3]))).IsFail())
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
				if((res = (viewImage[0].SynchronizeWindow(ref viewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}
				if((res = (viewImage[0].SynchronizeWindow(ref viewImage[3]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] layer = { viewImage[0].GetLayer(0), viewImage[1].GetLayer(0), viewImage[2].GetLayer(0), viewImage[3].GetLayer(0) };

				// 화면상 좌표(고정 좌표)에 Source Figure View 임을 표시
				// Indicates Source Figure View on screen coordinates (fixed coordinates)
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 0), "Source Figure", EColor.YELLOW, EColor.BLACK, 30);
				// 화면상 좌표(고정 좌표)에 Result View 임을 표시
				// Indicates Result View on screen coordinates (fixed coordinates)
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 0), "Dilate Result1", EColor.YELLOW, EColor.BLACK, 30);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 0), "Dilate Result2", EColor.YELLOW, EColor.BLACK, 30);
				layer[3].DrawTextCanvas(new CFLPoint<int>(0, 0), "Dilate Result2", EColor.YELLOW, EColor.BLACK, 30);

				CFLRegion flrgSourceFig = new CFLRegion();

				// Source Figure 불러오기 // Load source figure
				if((res = flrgSourceFig.Load("../../ExampleImages/Figure/RegionForReduce.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// 각 Layer 에 Figure 를 출력 // Draw Figure to each Layers
				for(int i = 0; i < 4; ++i)
				{
					layer[i].DrawFigureImage(flrgSourceFig, EColor.BLACK, 5);
					layer[i].DrawFigureImage(flrgSourceFig, EColor.LIME, 3);
				}

				// Dilate 함수 실행 (HalfSize : 10, default kernel : Rectangle)
				// Dilate function execution (HalfSize : 10, default kernel : Rectangle)
				CFLFigureArray flfaResult1;
				long i64HalfSize = 10;

				if((res = flrgSourceFig.Dilate(i64HalfSize, i64HalfSize, out flfaResult1)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Dilate 함수 실행 (HalfSize : 10, kernel shape : Circle)
				// Dilate function execution (HalfSize : 10, kernel shape : Circle)
				CFLFigureArray flfaResult2;

				if((res = flrgSourceFig.Dilate(i64HalfSize, i64HalfSize, out flfaResult2, EKernelShape.Circle)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Dilate 함수 실행 (Figure Kernel : 반지름이 10인 원)
				// Dilate function execution (Figure Kernel : Circle with radius 10)
				CFLFigureArray flfaResult3;

				CFLEllipse<double> fleKernel = new CFLEllipse<double>(0, 0, 5, 20, 90);
				CFLEllipse<double> fleForDrawing = new CFLEllipse<double>();
				fleForDrawing.Set(fleKernel);

				CFLPoint<double> flpOffset = new CFLPoint<double>(245, 53);
				flpOffset.x -= fleForDrawing.GetCenter().x;
				flpOffset.y -= fleForDrawing.GetCenter().y;

				fleForDrawing.Offset(flpOffset);

				if((res = flrgSourceFig.Dilate(fleKernel, out flfaResult3)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// View 에 결과 FigureArray 그리기 // Draw the resulting FigureArray in the View
				layer[1].DrawFigureImage(flfaResult1, EColor.BLACK, 5);
				layer[1].DrawFigureImage(flfaResult1, EColor.CYAN, 3);
				layer[2].DrawFigureImage(flfaResult2, EColor.BLACK, 5);
				layer[2].DrawFigureImage(flfaResult2, EColor.YELLOW, 3);
				layer[3].DrawFigureImage(flfaResult3, EColor.BLACK, 5);
				layer[3].DrawFigureImage(flfaResult3, EColor.LIGHTRED, 3);
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 40), "Rectangle (HalfSize : 10)", EColor.YELLOW, EColor.BLACK, 20);
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 40), "Circle (HalfSize : 10)", EColor.YELLOW, EColor.BLACK, 20);
				layer[3].DrawTextCanvas(new CFLPoint<int>(0, 40), "User Defined Kernel", EColor.YELLOW, EColor.BLACK, 20);
				layer[3].DrawFigureCanvas(fleForDrawing, EColor.LIGHTRED, 1, EColor.LIGHTRED, EGUIViewImagePenStyle.Solid, 1.0f, 0.5f);

				// Console 출력 // Console output
				Console.Write("\n<Source Figure>\n\n");
				Console.Write("{0}", CFigureUtilities.ConvertFigureObjectToString(flrgSourceFig));

				Console.Write("\n\n<Dilate Result1>\nHalfSize = {0}\nKernel Shape = Default(Rectangle)\n\n", i64HalfSize);
				Console.Write("Result1 Figure : {0}", CFigureUtilities.ConvertFigureObjectToString(flfaResult1));

				Console.Write("\n\n<Dilate Result2>\nHalfSize = {0}\nKernel Shape = Circle\n\n", i64HalfSize);
				Console.Write("Result2 Figure : {0}", CFigureUtilities.ConvertFigureObjectToString(flfaResult2));

				Console.Write("\n\n<Dilate Result3>\nKernel Shape = User Defined Kernel\n");
				Console.Write("Kernel Figure : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(fleKernel));
				Console.Write("Result3 Figure : {0}\n", CFigureUtilities.ConvertFigureObjectToString(flfaResult3));


				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 4; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 하나라도 꺼지면 종료로 간주 // Consider closed when any of image views are turned off
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable() && viewImage[3].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

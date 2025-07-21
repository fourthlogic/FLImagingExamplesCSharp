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

namespace Figure
{
	class ArrayOperation
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
			const int i32ViewCount = 4;

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[i32ViewCount];

			for(int i = 0; i < i32ViewCount; ++i)
				viewImage[i] = new CGUIViewImage();

			CResult res = new CResult(EResult.UnknownError);

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

				if((res = viewImage[2].Create(400, 384, 912, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[3].Create(912, 384, 1424, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				for(int i = 1; i < i32ViewCount; ++i)
				{
					// 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoint of the image view
					if((res = viewImage[0].SynchronizePointOfView(ref viewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						break;
					}

					// 이미지 뷰 윈도우의 위치를 맞춤 // Align the position of the image view window
					if((res = viewImage[0].SynchronizeWindow(ref viewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						break;
					}
				}

				// SourceView, DstView 의 0번 레이어 가져오기 // Get Layer 0 of SourceView, DstView
				CGUIViewImageLayer[] ViewLayer = new CGUIViewImageLayer[i32ViewCount];

				for(int i = 0; i < i32ViewCount; ++i)
					ViewLayer[i] = viewImage[i].GetLayer(0);

				// Figure 생성 // Create figure
				CFLRect<double> flr = new CFLRect<double>(50, 50, 100, 100, 15);
				CFLQuad<double> flq = new CFLQuad<double>(200, 50, 360, 50, 400, 160, 150, 110);
				CFLCircle<double> flc = new CFLCircle<double>(100.0, 150.0, 30.0, 0, 30, 90, EArcClosingMethod.Center);
				CFLEllipse<double> fle = new CFLEllipse<double>(300, 250, 100, 50, 0, 30, 200, EArcClosingMethod.Center);

				CFLFigureArray flfa = new CFLFigureArray();

				flfa.PushBack(flr);
				flfa.PushBack(flq);
				flfa.PushBack(flc);
				flfa.PushBack(fle);

				String strFigure;
				Console.WriteLine("Figure Array\n");

				for(int i = 0; i < flfa.GetCount(); ++i)
				{
					strFigure = String.Format("[{0}]\n {1}\n", i, CFigureUtilities.ConvertFigureObjectToString(flfa.GetAt(i)));
					Console.WriteLine("{0}", strFigure);
				}

				Console.WriteLine("\n");

				// Figure 그리기 // Draw Figure
				for(int i = 0; i < flfa.GetCount(); ++i)
					ViewLayer[i].DrawFigureImage(flfa, EColor.LIME);

				//////////////////////////////// GetCenterElementwise()
				// 중심점 좌표를 담을 FigureArray 생성 // Create a FigureArray to hold the coordinates of the center point
				CFLFigureArray flfaCenter = new CFLFigureArray();

				// Figure Array 각 요소의 중심점 계산 // Calculate the center point of each element of Figure Array
				flfa.GetCenterElementwise(ref flfaCenter);

				// 중심들을 View0의 0번 레이어에 그리기 // Draw the centers on layer 0 of View0
				ViewLayer[0].DrawFigureImage(flfaCenter, EColor.RED);
				ViewLayer[0].DrawTextCanvas(new TPoint<double>(0, 0), "GetCenterElementwise() Result", EColor.YELLOW, EColor.BLACK, 15);

				// 콘솔에 중심 좌표 표시 // Print center coordinates in console
				Console.WriteLine("Center Point\n");

				for(int i = 0; i < flfa.GetCount(); ++i)
				{
					strFigure = String.Format("[{0}]\n {1}\n", i, CFigureUtilities.ConvertFigureObjectToString(flfaCenter.GetAt(i)));
					Console.WriteLine("{0}", strFigure);
				}

				Console.WriteLine("\n");


				//////////////////////////////// GetPerimeterElementwise()
				// 각 둘레의 길이를 저장할 CFLFigureArray 생성 // Create CFLFigureArray to store the length of each perimeter
				CFLFigureArray flfaPerimeter = new CFLFigureArray();

				// Figure Array 각 요소의 둘레 계산 // Calculate the perimeter of each element of the Figure Array
				flfa.GetPerimeterElementwise(ref flfaPerimeter);

				// Figure Array 각 요소의 둘레 표시 // Display perimeter of each element of Figure Array
				for(long i = 0; i < flfaPerimeter.GetCount(); ++i)
				{
					String strPerimeter;
					strPerimeter = String.Format("{0}", ((CFLScalar<double>)flfaPerimeter.GetAt(i)).v);
					ViewLayer[1].DrawTextImage(flfaCenter.GetAt(i), strPerimeter, EColor.BLACK);
				}
				ViewLayer[1].DrawTextCanvas(new TPoint<double>(0, 0), "GetPerimeterElementwise() Result", EColor.YELLOW, EColor.BLACK, 15);

				// 콘솔에 길이 표시 // Display the length in the console
				Console.WriteLine("Perimeter\n");

				for(long i = 0; i < flfaPerimeter.GetCount(); ++i)
				{
					strFigure = String.Format("[{0}]\n {1}\n", i, ((CFLScalar<double>)flfaPerimeter.GetAt(i)).v);
					Console.WriteLine("{0}", strFigure);
				}

				Console.WriteLine("\n");


				//////////////////////////////// GetCenterOfGravityElementwise()
				// 무게중심점 좌표를 담을 FigureArray 생성 // Create a FigureArray to contain the coordinates of the center of gravity
				CFLFigureArray flfaCenterOfGravity = new CFLFigureArray();

				// Figure Array 각 요소의 무게중심점 계산 // Calculate the center of gravity of each element of the Figure Array
				flfa.GetCenterOfGravityElementwise(ref flfaCenterOfGravity);

				// 무게중심들을 View0의 0번 레이어에 그리기 // Draw the centers of gravity on Layer 0 of View0
				ViewLayer[2].DrawFigureImage(flfaCenterOfGravity, EColor.CYAN);
				ViewLayer[2].DrawTextCanvas(new TPoint<double>(0, 0), "GetCenterOfGravityElementwise() Result", EColor.YELLOW, EColor.BLACK, 15);

				// 콘솔에 무게중심 좌표 표시 // Display barycentric coordinates in console
				Console.WriteLine("Center Of Gravity Point\n");

				for(int i = 0; i < flfa.GetCount(); ++i)
				{
					strFigure = String.Format("[{0}]\n {1}\n", i, CFigureUtilities.ConvertFigureObjectToString(flfaCenterOfGravity.GetAt(i)));
					Console.WriteLine("{0}", strFigure);
				}

				Console.WriteLine("\n");


				//////////////////////////////// GetMinimumEnclosingRectangleElementwise()
				// 최소둘레 직사각형을 담을 FigureArray 생성 // Create a FigureArray to contain the minimum enclosing rectangle
				CFLFigureArray flfaMER = new CFLFigureArray();

				// Figure Array 각 요소의 최소둘레 직사각형을 계산 // Calculate the minimum enclosing rectangle of each element of the Figure Array
				flfa.GetMinimumEnclosingRectangleElementwise(ref flfaMER);

				// 최소둘레 직사각형들을 View0의 0번 레이어에 그리기 // Draw the minimum enclosing rectangle on Layer 0 of View0
				ViewLayer[3].DrawFigureImage(flfaMER, EColor.BLUE);
				ViewLayer[3].DrawTextCanvas(new TPoint<double>(0, 0), "GetMinimumEnclosingRectangleElementwise() Result", EColor.YELLOW, EColor.BLACK, 15);

				// 콘솔에 최소둘레 직사각형을 표시 // Display the minimum enclosing rectangle in console
				Console.WriteLine("Minimum Enclosing Rectangle\n");

				for(int i = 0; i < flfa.GetCount(); ++i)
				{
					strFigure = String.Format("[{0}]\n {1}\n", i, CFigureUtilities.ConvertFigureObjectToString(flfaMER.GetAt(i)));
					Console.WriteLine("{0}", strFigure);
				}

				Console.WriteLine("\n");

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < i32ViewCount; ++i)
				{
					viewImage[i].Invalidate(true);
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable() && viewImage[3].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

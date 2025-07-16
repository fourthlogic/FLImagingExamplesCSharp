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

namespace GetSimilarity
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
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

			CResult res;

			do
			{
				// Source Coordinate View 생성 // Create Source Coordinate View
				if((res = (viewImage[0].Create(200, 0, 700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination Coordinate View 생성 // Create Destination Coordinate View
				if((res = (viewImage[1].Create(700, 0, 1200, 500))).IsFail())
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

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = (viewImage[0].SynchronizeWindow(ref viewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] layer = { viewImage[0].GetLayer(0), viewImage[1].GetLayer(0) };

				// 화면상 좌표(고정 좌표)에 Get Similarity Matrix View 임을 표시
				// Indicates Get Similarity Matrix View on screen coordinates (fixed coordinates)
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 0), "Get Similarity Matrix", EColor.YELLOW, EColor.BLACK, 30);
				// 화면상 좌표(고정 좌표)에 Transformed View 임을 표시
				// Indicates Transformed View on screen coordinates (fixed coordinates)
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 0), "Transformed", EColor.YELLOW, EColor.BLACK, 30);

				CFLEllipse<double> fleSourceFig = new CFLEllipse<double>();

				// Source Figure 불러오기 // Load source figure
				if((res = fleSourceFig.Load("../../ExampleImages/Figure/Ellipse1.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.\n");
					break;
				}

				// Source Figure 를 Transformed Figure 에 복사한 후 Affine 변환
				// Affine transformation after copying the Source Figure to the Transformed Figure
				CFLEllipse<double> fleTransformedFig = new CFLEllipse<double>();
				fleTransformedFig.Set(fleSourceFig);
				fleTransformedFig.Scale(fleSourceFig.GetCenter(), 1.8, 1.8);
				fleTransformedFig.Rotate(30, fleSourceFig.GetCenter());
				fleTransformedFig.Offset(-200, 180);

				// Source Figure 와 Transformed Figure 로부터 점을 샘플링
				// Sample points from the Source Figure and Transformed Figure
				CFLFigureArray flfaSource, flfaTransformed;
				fleSourceFig.GetSamplingPointsOnSegment(5, ref flfaSource);
				fleTransformedFig.GetSamplingPointsOnSegment(5, ref flfaTransformed);

				CFLPointArray flpaSource = new CFLPointArray();
				for(long i = 0; i < flfaSource.GetCount(); ++i)
					flpaSource.PushBack(new CFLPoint<double>(flfaSource.GetAt(i)));

				// Sampling 한 Source Points 들을 Transformed Points 로 복사한 후 Figure 와 동일하게 Affine 변환
				// After copying the sampled Source Points to Transformed Points, convert the Affine in the same way as the Figure
				CFLPointArray flpaTransformed = new CFLPointArray(flpaSource);
				flpaTransformed.Scale(fleSourceFig.GetCenter(), 1.8, 1.8);
				flpaTransformed.Rotate(30, fleSourceFig.GetCenter());
				flpaTransformed.Offset(-200, 180);

				// Transformed Points 에 Random Noise 를 추가 // Add Random Noise to Transformed Points
				CFLPointArray flpaTransformedWithNoise = new CFLPointArray();
				for(long i = 0; i < flpaTransformed.GetCount(); ++i)
				{
					CFLPoint<double> flp = new CFLPoint<double>(flpaTransformed.GetAt(i));

					flpaTransformedWithNoise.PushBack(new CFLPoint<double>(flp.x + CRandomGenerator.Double(-5, 5), flp.y + CRandomGenerator.Double(-5, 5)));
				}

				// 0번 Layer 에 Figure 들과 Text 를 출력 // Draw Figures and Text to Layer 0
				layer[0].DrawTextImage(fleSourceFig.GetCenter(), "Source", EColor.LIME, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
				layer[0].DrawTextImage(fleTransformedFig.GetCenter(), "Destination", EColor.CYAN, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);

				layer[0].DrawFigureImage(fleSourceFig, EColor.RED);
				layer[0].DrawFigureImage(flpaSource, EColor.LIME, 1);

				layer[0].DrawFigureImage(fleTransformedFig, EColor.BLUE);
				layer[0].DrawFigureImage(flpaTransformedWithNoise, EColor.CYAN, 1);

				// Similarity 행렬 계산 // Calculate the similarity matrix
				CMatrix<double> matResult;
				if((res = CMatrix<double>.GetSimilarity(flpaSource, flpaTransformedWithNoise, out matResult)).IsFail())
				{
					ErrorPrint(res, "Failed to calculate.\n");
					break;
				}

				// Console 출력 // Console output
				Console.WriteLine("\n[index] Source Ellipse Points -> Target Points with noise");

				for(long i = 0; i < flpaSource.GetCount(); ++i)
					Console.WriteLine("[{0}] ({1:.000},{2:.000}) -> ({3:.000},{4:.000})", i, flpaSource.GetAt(i).x, flpaSource.GetAt(i).y, flpaTransformedWithNoise.GetAt(i).x, flpaTransformedWithNoise.GetAt(i).y);

				Console.WriteLine("\n\nSimilarity Matrix");
				Console.WriteLine("[{0:0.000}, {1:0.000}, {2:0.000}]", matResult.GetValue(0, 0), matResult.GetValue(0, 1), matResult.GetValue(0, 2));
				Console.WriteLine("[{0:0.000}, {1:0.000}, {2:0.000}]", matResult.GetValue(1, 0), matResult.GetValue(1, 1), matResult.GetValue(1, 2));
				Console.WriteLine("[{0:0.000}, {1:0.000}, {2:0.000}]\n\n", matResult.GetValue(2, 0), matResult.GetValue(2, 1), matResult.GetValue(2, 2));

				// 계산된 Similarity 행렬을 사용하여 Affine 변환할 Source Grid Point 생성
				// Create a Source Grid Point to be Affine Transformed using the calculated Similarity Matrix
				CFLPointArray flpaSourceGrid = new CFLPointArray();
				CFLPoint<int> flpGridSize = new CFLPoint<int>(5, 5);
				int i32GridPitch = 20;
				int i32GridOffsetX = 325;
				int i32GridOffsetY = 90;

				for(int y = 0; y < flpGridSize.y; ++y)
				{
					int i32PosY = y * i32GridPitch + i32GridOffsetY;

					for(int x = 0; x < flpGridSize.x; ++x)
					{
						int i32PosX = x * i32GridPitch + i32GridOffsetX;

						flpaSourceGrid.PushBack(new CFLPoint<double>(i32PosX, i32PosY));
					}
				}

				// View 에 Text 출력 // Output text to View
				CFLPoint<int> flpDrawTextPosition = new CFLPoint<int>(flpaSourceGrid.GetBoundaryRect().left -3, flpaSourceGrid.GetBoundaryRect().top - 5);

				layer[1].DrawFigureImage(flpaSourceGrid, EColor.LIME, 3);
				layer[1].DrawTextImage(flpDrawTextPosition, "Source", EColor.LIME, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM);

				// Affine 변환할 Result PointArray 선언 // Declaration of Result PointArray to be converted to Affine
				CFLPointArray flpaResult = new CFLPointArray();
				// Affine 변환에 사용할 Matrix 선언 // Declaration of Matrix to be used for Affine transformation
				CMatrix<double> matA = new CMatrix<double>(3, 1);
				CMatrix<double> matB;

				Console.WriteLine("Affine Transform using Similarity Matrix\n");
				Console.WriteLine("[index] Source Grid -> Transformed Grid");

				// Source Grid Point 를 Affine 변환 // Convert Source Grid Point to Affine
				for(long i = 0; i < flpaSourceGrid.GetCount(); ++i)
				{
					matA.SetValue(0, 0, flpaSourceGrid.GetAt(i).x);
					matA.SetValue(1, 0, flpaSourceGrid.GetAt(i).y);
					matA.SetValue(2, 0, 1);

					if((res = matResult.Multiply(matA, out matB)).IsFail())
					{
						ErrorPrint(res, "Failed to calculate Matrix Operation\n");
						break;
					}

					flpaResult.PushBack(new CFLPoint<double>(matB.GetValue(0, 0), matB.GetValue(1, 0)));

					// Console 출력 // Console output
					Console.WriteLine("[{0}] ({1:.000},{2:.000}) -> ({3:.000},{4:.000})", i, flpaSourceGrid.GetAt(i).x, flpaSourceGrid.GetAt(i).y, flpaResult.GetAt(i).x, flpaResult.GetAt(i).y);
				}

				// View 에 Text 출력 // Output text to View
				flpDrawTextPosition.Scale(flpaSourceGrid.GetCenter(), 1.8, 1.8);
				flpDrawTextPosition.Rotate(30, flpaSourceGrid.GetCenter());
				flpDrawTextPosition.Offset(-200, 180);

				layer[1].DrawFigureImage(flpaResult, EColor.CYAN, 3);
				layer[1].DrawTextImage(flpDrawTextPosition, "Transformed", EColor.CYAN, EColor.BLACK, 15, false, 30, EGUIViewImageTextAlignment.LEFT_BOTTOM);

				layer[0].DrawTextCanvas(new CFLPoint<int>(5, 40), String.Format("[{0:0.000}, {1:0.000}, {2:0.000}]", matResult.GetValue(0, 0), matResult.GetValue(0, 1), matResult.GetValue(0, 2)), EColor.YELLOW, EColor.BLACK, 15);
				layer[0].DrawTextCanvas(new CFLPoint<int>(5, 60), String.Format("[{0:0.000}, {1:0.000}, {2:0.000}]", matResult.GetValue(1, 0), matResult.GetValue(1, 1), matResult.GetValue(1, 2)), EColor.YELLOW, EColor.BLACK, 15);
				layer[0].DrawTextCanvas(new CFLPoint<int>(5, 80), String.Format("[{0:0.000}, {1:0.000}, {2:0.000}]", matResult.GetValue(2, 0), matResult.GetValue(2, 1), matResult.GetValue(2, 2)), EColor.YELLOW, EColor.BLACK, 15);

				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 2; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 셋중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the three image views are turned off
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

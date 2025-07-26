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

namespace FLImagingExamplesCSharp
{
	class PatternMatchSparse_SaveLoad
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

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliFindImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageFind = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/Matching/Pattern2 Single Learn.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				if((res = fliFindImage.Load("../../ExampleImages/Matching/Pattern2 Single Find.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearn.Create(400, 0, 912, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				if((res = viewImageFind.Create(912, 0, 1680, 576)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				if((res = viewImageFind.SetImagePtr(ref fliFindImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageFind)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerFind = viewImageFind.GetLayer(1);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerFind.Clear();

				CFLPoint<double> flp00 = new CFLPoint<double>(0, 0);

				if((res = layerLearn.DrawTextCanvas(flp00, "LEARN", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerFind.DrawTextCanvas(flp00, "FIND", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// Pattern Match 객체 생성 // Create Pattern Match object
				CPatternMatchSparse FLPatternMatchSparseSave = new CPatternMatchSparse();
				CPatternMatchSparse FLPatternMatchSparseLoad = new CPatternMatchSparse();

				// 학습할 이미지 설정 // Set the image to learn
				FLPatternMatchSparseSave.SetLearnImage(ref fliLearnImage);

				// 학습할 영역을 설정합니다. // Set the area to learn.
				CFLRect<double> learnRegion = new CFLRect<double>(150, 150, 760, 840);
				CFLPoint<double> flpLearnPivot = new CFLPoint<double>(learnRegion.GetCenter());
				FLPatternMatchSparseSave.SetLearnROI(learnRegion);
				FLPatternMatchSparseSave.SetLearnPivot(flpLearnPivot);

				// 샘플링 개수를 설정합니다. // Set the sample count.
				FLPatternMatchSparseSave.SetSampleCount(64);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = FLPatternMatchSparseSave.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to Learn.");
					break;
				}

				// 데이터 Save를 진행합니다. // Proceed to save data.
				if((res = FLPatternMatchSparseSave.Save("../../ExampleImages/Matching/Pattern Single Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to save\n");
					break;
				}

				// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
				if((res = layerLearn.DrawFigureImage(learnRegion, EColor.BLACK, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				if((res = layerLearn.DrawFigureImage(learnRegion, EColor.CYAN)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				// 설정된 중심점의 위치를 디스플레이 한다 // Display the position of the set center point
				CFLFigureArray flfaPointPivot = flpLearnPivot.MakeCrossHair(3, false);

				if((res = layerLearn.DrawFigureImage(flfaPointPivot, EColor.BLACK, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				if((res = layerLearn.DrawFigureImage(flfaPointPivot, EColor.LIME)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				// 학습한 정보에 대해 Console창에 출력한다 // Print the learned information to the console window
				Console.WriteLine(" ▷ Learn Information");
				Console.WriteLine("  1. ROI Shape Type : Rectangle");
				Console.WriteLine("    left   : {0}", learnRegion.left);
				Console.WriteLine("    right  : {0}", learnRegion.right);
				Console.WriteLine("    top    : {0}", learnRegion.top);
				Console.WriteLine("    bottom : {0}", learnRegion.bottom);
				Console.WriteLine("    angle  : {0}", learnRegion.angle);
				Console.WriteLine("  2. Interest Pivot : ({0}, {1})", flpLearnPivot.x, flpLearnPivot.y);
				Console.WriteLine("");

				// 데이터 Load를 진행합니다. // Proceed to load data.
				if((res = FLPatternMatchSparseLoad.Load("../../ExampleImages/Matching/Pattern Single Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to load\n");
					break;
				}

				// 검출할 이미지 설정 // Set image to detect
				FLPatternMatchSparseLoad.SetSourceImage(ref fliFindImage);

				// 검출 시 사용될 기본 각도를 설정합니다. // Set the default angle to be used for detection.
				FLPatternMatchSparseLoad.SetAngleBias(0.0);
				// 검출 시 사용될 각도의 탐색범위를 설정합니다. // Set the search range of the angle to be used for detection.
				// 각도는 기본 각도를 기준으로 (기본 각도 - AngleTolerance, 기본 각도 + AngleTolerance)가 최종 탐색범위 // The angle is based on the basic angle (default angle - AngleTolerance, basic angle + AngleTolerance) is the final search range
				FLPatternMatchSparseLoad.SetAngleTolerance(10.0);
				// 검출 시 사용될 최소 탐색점수를 설정합니다. // Set the minimum search score to be used for detection.
				FLPatternMatchSparseLoad.SetMinimumDetectionScore(0.7);
				// 검출 시 사용될 최대 탐색객체 수를 설정합니다. // Set the maximum number of search objects to be used for detection.
				FLPatternMatchSparseLoad.SetMaxObject(1);
				// 검출 시 보간법 사용 유무에 대해 설정합니다. // Set whether to use interpolation when detecting.
				FLPatternMatchSparseLoad.EnableInterpolation(true);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = FLPatternMatchSparseLoad.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}

				// 패턴 검출 결과를 가져옵니다. // Get the pattern detection result.
				long i64ResultCount = FLPatternMatchSparseLoad.GetResultCount();

				Console.WriteLine(" ▶ Find Information");

				for(long i = 0; i < i64ResultCount; ++i)
				{
					CPatternMatchSparse.SResult results = new CPatternMatchMultiSparse.SResult();

					FLPatternMatchSparseLoad.GetResult(i, ref results);

					float f32Score = results.f32Score;
					float f32Angle = results.f32Angle;
					float f32Scale = results.f32Scale;
					CFLPoint<double> flpPivot = new CFLPoint<double>(results.pFlpPivot);
					CFLFigure pFlfRegion = new CFLRect<double>(results.pFlfRegion);
					CFLRect<double> pFlrResultRegion = new CFLRect<double>(pFlfRegion);

					CFLRect<double> flrResultRegion = pFlrResultRegion;

					CFLPoint<double> flpResultRegion = new CFLPoint<double>(flrResultRegion.left, flrResultRegion.top);

					// 패턴 검출 결과를 Console창에 출력합니다. // Output the pattern detection result to the console window.
					Console.WriteLine(" < Instance : {0} >", i);
					Console.WriteLine("  1. ROI Shape Type : Rectangle");
					Console.WriteLine("    left   : {0}", flrResultRegion.left);
					Console.WriteLine("    right  : {0}", flrResultRegion.right);
					Console.WriteLine("    top    : {0}", flrResultRegion.top);
					Console.WriteLine("    bottom : {0}", flrResultRegion.bottom);
					Console.WriteLine("    angle  : {0}", flrResultRegion.angle);
					Console.WriteLine("  2. Interest Pivot : ({0}, {1})", flpResultRegion.x, flpResultRegion.y);
					Console.WriteLine("  3. Score : {0}\n  4. Angle : {1}\n  5. Scale : {2}", f32Score, flrResultRegion.angle, f32Scale);
					Console.WriteLine("");

					// 검출 결과의 중심점을 디스플레이 한다 // Display the center point of the detection result
					CFLFigureArray flfaPoint = flpPivot.MakeCrossHair(3, false);
					flfaPoint.Rotate(f32Angle, flpPivot);

					if((res = layerFind.DrawFigureImage(flfaPoint, EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layerFind.DrawFigureImage(flfaPoint, EColor.LIME)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					// 결과 영역을 디스플레이 한다 // Display the result area
					if((res = layerFind.DrawFigureImage(flrResultRegion, EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layerFind.DrawFigureImage(flrResultRegion, EColor.LIME)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					TPoint<double> tpPosition = new TPoint<double>();
					tpPosition.x = flpPivot.x;
					tpPosition.y = flpPivot.y;

					string strText = String.Format("Score : {0}\nAngle : {1}\nScale : x{2}\n", f32Score, f32Angle, f32Scale);

					if((res = layerFind.DrawTextImage(tpPosition, strText, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_CENTER)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text");
						break;
					}
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImageLearn.Invalidate(true);
				viewImageFind.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImageLearn.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

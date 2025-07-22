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

namespace Match
{
	class GeometricMatch_SaveLoad
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
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliFindImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageFind = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/Matching/Geometric Single Learn.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				if((res = fliFindImage.Load("../../ExampleImages/Matching/Geometric Single Find.flif")).IsFail())
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

				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerFind = viewImageFind.GetLayer(1);

				layerLearn.Clear();
				layerFind.Clear();

				TPoint<double> tpPosition00 = new TPoint<double>(0, 0);

				if((res = layerLearn.DrawTextCanvas(tpPosition00, "LEARN", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerFind.DrawTextCanvas(tpPosition00, "FIND", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// Geometric Match 객체 생성 // Create Geometric Match object
				CGeometricMatch FLGeometricMatchSave = new CGeometricMatch();
				CGeometricMatch FLGeometricMatchLoad = new CGeometricMatch();

				// 학습할 이미지 설정 // Set the image to learn
				FLGeometricMatchSave.SetLearnImage(ref fliLearnImage);

				// 학습할 영역을 설정합니다. // Set the area to learn.
				CFLRect<double> learnRegion = new CFLRect<double>(110.77276, 97.42619, 747.46519, 752.33384);
				CFLPoint<double> flpLearnPivot = new CFLPoint<double>(learnRegion.GetCenter());
				FLGeometricMatchSave.SetLearnROI(learnRegion);
				FLGeometricMatchSave.SetLearnPivot(flpLearnPivot);

				// 학습 파라미터를 설정합니다. // Set the learning parameters.
				// 추출할 특징점 개수를 설정합니다. // Set the number of feature points to be extracted.
				FLGeometricMatchSave.SetFeatureCount(2048);
				// 추출할 특징점 처리과정에서의 노이즈 필터링 정도를 설정합니다. // Set the noise filtering degree in the process of processing the feature points to be extracted.
				FLGeometricMatchSave.SetFeatureFiltering(0.5);
				// 추출할 특징점 처리과정에서의 허용 임계값을 설정합니다. // Set the allowable threshold in the feature point processing process to be extracted.
				FLGeometricMatchSave.SetLearnThresholdCoefficient(1.0);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = FLGeometricMatchSave.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to Learn.");
					break;
				}

				// 데이터 Save를 진행합니다. // Proceed to save data.
				if((res = FLGeometricMatchSave.Save("../../ExampleImages/Matching/Geometric Single Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to save\n");
					break;
				}

				// 학습 영역이 어디인지 알기 위해 디스플레이 한다 // Display to see where the learning area is
				if((res = layerLearn.DrawFigureImage(learnRegion, EColor.BLACK, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerLearn.DrawFigureImage(learnRegion, EColor.CYAN)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 설정된 중심점의 위치를 디스플레이 한다 // Display the position of the set center point
				CFLFigureArray flfaPoint = flpLearnPivot.MakeCrossHair(3, false);

				if((res = layerLearn.DrawFigureImage(flfaPoint, EColor.BLACK, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				if((res = layerLearn.DrawFigureImage(flfaPoint, EColor.LIME)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				// 학습한 특징점을 디스플레이 한다 // Display the learned feature point
				// 학습한 특징점이 찾고자하는 객체를 나타내기에 충분하게 잘 뽑혔는지 확인하고, 그렇지 않다면 학습 파라미터를 재조정함으로써 재확인하면 검출 시 더 효과적입니다. // Check whether the learned feature points are selected well enough to represent the object to be found.
				CFLFigureArray flfaFeaturePoints = new CFLFigureArray();

				if((res = FLGeometricMatchSave.GetLearnedFeature(ref flfaFeaturePoints)).IsFail())
				{
					ErrorPrint(res, "Failed to get learnt features.");
					break;
				}

				layerLearn.DrawFigureImage(flfaFeaturePoints, EColor.BLUE);

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

				// 데이터를 Load합니다. // Load data.
				if((res = FLGeometricMatchLoad.Load("../../ExampleImages/Matching/Geometric Single Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to load\n");
					break;
				}

				// 검출할 이미지 설정 // Set image to detect
				FLGeometricMatchLoad.SetSourceImage(ref fliFindImage);

				// 검출 시 사용될 파라미터를 설정합니다. // Set the parameters to be used for detection.
				// 검출 시 사용될 기본 각도를 설정합니다. // Set the default angle to be used for detection. // Set the default angle to be used for detection.
				FLGeometricMatchLoad.SetAngleBias(0.0);
				// 검출 시 사용될 각도의 탐색범위를 설정합니다. // Set the search range of the angle to be used for detection.
				// 각도는 기본 각도를 기준으로 (기본 각도 - AngleTolerance, 기본 각도 + AngleTolerance)가 최종 탐색범위 // The angle is based on the basic angle (default angle - AngleTolerance, basic angle + AngleTolerance) is the final search range
				FLGeometricMatchLoad.SetAngleTolerance(180.0);
				// 검출 시 사용될 스케일 탐색범위를 설정합니다. // Set the scale search range to be used for detection.
				FLGeometricMatchLoad.SetScaleRange(0.98, 1.02);
				// 검출 시 사용될 최소 탐색점수를 설정합니다. // Set the minimum search score to be used for detection.
				FLGeometricMatchLoad.SetMinimumDetectionScore(0.7);
				// 검출 시 사용될 최대 탐색객체 수를 설정합니다. // Set the maximum number of search objects to be used for detection.
				FLGeometricMatchLoad.SetMaxObject(5);

				// 검출 시 보간법 사용 유무에 대해 설정합니다. // Set whether to use interpolation when detecting.
				FLGeometricMatchLoad.EnableInterpolation(true);
				// 검출 시 최적화 정도에 대해 설정합니다. // Set the degree of optimization for detection.
				FLGeometricMatchLoad.SetOptimizationOption(CGeometricMatch.EOptimizationOption.Fast);
				// 검출 시 대비정도에 대해 설정합니다. // Set the contrast level for detection.
				FLGeometricMatchLoad.SetContrastOption(FLImagingCLR.AdvancedFunctions.EMatchContrastOption.Normal);
				// 검출 시 이미지 영역밖의 탐색 정도를 설정합니다. // Set the degree of search outside the image area when detecting.
				FLGeometricMatchLoad.SetInvisibleRegionEstimation(1.25);
				// 검출 시 처리과정에서의 허용 임계값을 설정합니다. // Set the allowable threshold in the process of detection.
				FLGeometricMatchLoad.SetFindThresholdCoefficient(1.0);
				// 검출 시 겹쳐짐 허용 정도를 설정합니다. // Set the allowable degree of overlap during detection.
				FLGeometricMatchLoad.SetObjectOverlap(0.5);
				// 검출 시 이미지 전처리 유무를 설정합니다. // Set whether or not to pre-process the image during detection.

				// 알고리즘 수행 // Execute the Algoritm
				res = FLGeometricMatchLoad.Execute();

				if(res.IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}

				// 기하학적 패턴 검출 결과를 가져옵니다. // Get the geometric pattern detection result.
				long i64ResultCount = FLGeometricMatchLoad.GetResultCount();

				Console.WriteLine(" ▶ Find Information");

				for(long i = 0; i < i64ResultCount; ++i)
				{
					CGeometricMatch.SResult results = new CGeometricMatch.SResult();
					CFLFigureArray flfaResultPoints = new CFLFigureArray();

					FLGeometricMatchLoad.GetResult(i, ref results);
					FLGeometricMatchLoad.GetResultDetectedFeature(i, ref flfaResultPoints);

					float f32Score = results.f32Score;
					float f32Angle = results.f32Angle;
					float f32Scale = results.f32Scale;
					CFLFigure pFlfRegion = results.pFlfRegion;
					CFLPoint<double> flpLocation = results.pFlpLocation;
					CFLPoint<double> flpPivot = results.pFlpPivot;
					;

					CFLRect<double> pFlrResultRegion = new CFLRect<double>(pFlfRegion);

					// 기하학적 패턴 검출 결과를 Console창에 출력합니다. // Output the geometric pattern detection result to the console window.
					Console.WriteLine(" < Instance : {0} >", i);
					Console.WriteLine("  1. ROI Shape Type : Rectangle");
					Console.WriteLine("    left   : {0}", pFlrResultRegion.left);
					Console.WriteLine("    right  : {0}", pFlrResultRegion.right);
					Console.WriteLine("    top    : {0}", pFlrResultRegion.top);
					Console.WriteLine("    bottom : {0}", pFlrResultRegion.bottom);
					Console.WriteLine("    angle  : {0}", f32Angle);
					Console.WriteLine("  2. Interest Pivot : ({0}, {1})", flpPivot.x, flpPivot.y);
					Console.WriteLine("  3. Score : {0}\n  4. Angle : {1}\n  5. Scale : x{2}", f32Score, f32Angle, f32Scale);

					// 검출 결과의 중심점을 디스플레이 한다 // Display the center point of the detection result
					CFLFigureArray flfaPointPivot = flpPivot.MakeCrossHair(3, false);
					flfaPointPivot.Rotate(f32Angle, flpPivot);

					if((res = layerFind.DrawFigureImage(flfaPointPivot, EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layerFind.DrawFigureImage(flfaPointPivot, EColor.LIME)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					TPoint<double> tpPosition = new TPoint<double>();
					tpPosition.x = flpPivot.x;
					tpPosition.y = flpPivot.y;

					string strText = String.Format("Score : {0}\nAngle : {1}\nScale : x{2}\n", f32Score, f32Angle, f32Scale);

					// 결과 특징점을 디스플레이 한다 // Display the resulting feature point
					layerFind.DrawFigureImage(flfaResultPoints, EColor.LIME);

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

			Console.ReadLine();
		}
	}
}

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
			CFLImage[] fliLearnImage = new CFLImage[3];
			CFLImage fliFindImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImageLearn = new CGUIViewImage[3];
			CGUIViewImage viewImageFind = new CGUIViewImage();

			for(long i = 0; i < 3; i++)
			{
				fliLearnImage[i] = new CFLImage();
				viewImageLearn[i] = new CGUIViewImage();
			}

			CResult res = new CResult();

			// Geometric Match Multi 객체 생성 // Create Geometric Match Multi object
			CGeometricMatchMulti FLGeometricMatchMultiSave = new CGeometricMatchMulti();
			CGeometricMatchMulti FLGeometricMatchMultiLoad = new CGeometricMatchMulti();

			do
			{
				string[] arrPath = new string[3];
				arrPath[0] = "../../ExampleImages/Matching/Geometric Mult Learn_01.flif";
				arrPath[1] = "../../ExampleImages/Matching/Geometric Mult Learn_02.flif";
				arrPath[2] = "../../ExampleImages/Matching/Geometric Mult Learn_03.flif";

				string[] arrClassName = new string[3];
				arrClassName[0] = "A";
				arrClassName[1] = "B";
				arrClassName[2] = "C";

				EColor[] arrColor = new EColor[3];
				arrColor[0] = EColor.LIME;
				arrColor[1] = EColor.RED;
				arrColor[2] = EColor.CYAN;

				Console.WriteLine(" ▷ Learn Information");

				for(long i64DataIdx = 0; i64DataIdx < 3; ++i64DataIdx)
				{
					// 이미지 로드 // Load image
					if((fliLearnImage[i64DataIdx].Load(arrPath[i64DataIdx]).IsFail()))
						break;

					// 이미지 뷰 생성 // Create image view
					if((viewImageLearn[i64DataIdx].Create((int)(400 + 512 * i64DataIdx), 0, (int)(400 + 512 * (i64DataIdx + 1)), 384)).IsFail())
						break;

					// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
					if((viewImageLearn[i64DataIdx].SetImagePtr(fliLearnImage[i64DataIdx]).IsFail()))
						break;

					CGUIViewImageLayer layerLearn = viewImageLearn[i64DataIdx].GetLayer(0);

					layerLearn.Clear();

					// 학습할 이미지 설정 // Set the image to learn
					FLGeometricMatchMultiSave.SetLearnImage(ref fliLearnImage[i64DataIdx]);

					// 학습할 영역을 설정합니다. // Set the area to learn.
					CFLRect<double> learnRegion = new CFLRect<double>();

					if(i64DataIdx == 0)
						learnRegion.Set(33.700864, 230.805616, 213.474082, 407.099352);
					else if(i64DataIdx == 1)
						learnRegion.Set(370.366091, 482.671707, 470.402807, 575.431965);
					else if(i64DataIdx == 2)
						learnRegion.Set(363.564795, 344.259179, 486.333693, 430.323974);

					CFLPoint<double> flpLearnPivot = new CFLPoint<double>(learnRegion.GetCenter());
					FLGeometricMatchMultiSave.SetLearnROI(learnRegion);
					FLGeometricMatchMultiSave.SetLearnPivot(flpLearnPivot);

					// 학습 파라미터를 설정합니다. // Set the learning parameters.
					// 추출할 특징점 개수를 설정합니다. // Set the number of feature points to be extracted.
					FLGeometricMatchMultiSave.SetFeatureCount(2048);
					// 추출할 특징점 처리과정에서의 노이즈 필터링 정도를 설정합니다. // Set the noise filtering degree in the process of processing the feature points to be extracted.
					FLGeometricMatchMultiSave.SetFeatureFiltering(0.5);
					// 추출할 특징점 처리과정에서의 허용 임계값을 설정합니다. // Set the allowable threshold in the feature point processing process to be extracted.
					FLGeometricMatchMultiSave.SetLearnThresholdCoeff(1.0);
					// 추출할 특징점 처리과정에서의 이미지 전처리 유무를 설정합니다. // Set whether or not to pre-process the image in the process of processing the feature points to be extracted.
					FLGeometricMatchMultiSave.EnablePreprocessing(false);

					// 알고리즘 수행 // Execute the Algoritm
					if((FLGeometricMatchMultiSave.Learn(arrClassName[i64DataIdx])).IsFail())
						break;

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					if((res = layerLearn.DrawFigureImage(learnRegion, EColor.BLACK, 3)).IsFail())
						break;

					if((res = layerLearn.DrawFigureImage(learnRegion, arrColor[i64DataIdx])).IsFail())
						break;

					// 설정된 중심점의 위치를 디스플레이 한다 // Display the position of the set center point
					CFLFigureArray flfaPointPivot = flpLearnPivot.MakeCrossHair(3, false);

					layerLearn.DrawFigureImage(flfaPointPivot, EColor.BLACK, 3);

					if((res = layerLearn.DrawFigureImage(flfaPointPivot, EColor.BLACK, 3)).IsFail())
						break;

					if((res = layerLearn.DrawFigureImage(flfaPointPivot, EColor.LIME)).IsFail())
						break;


					// 학습한 특징점을 디스플레이 한다 // Display the learned feature point
					// 학습한 특징점이 찾고자하는 객체를 나타내기에 충분하게 잘 뽑혔는지 확인하고, 그렇지 않다면 학습 파라미터를 재조정함으로써 재확인하면 검출 시 더 효과적입니다. // Check whether the learned feature points are selected well enough to represent the object to be found.
					CFLFigureArray flfaFeaturePoints;
					FLGeometricMatchMultiSave.GetLearntFeature(out flfaFeaturePoints);
					layerLearn.DrawFigureImage(flfaFeaturePoints, arrColor[i64DataIdx]);

					string strStatus;
					strStatus = String.Format("LEARN CLASS {0} ", arrClassName[i64DataIdx]);

					CFLPoint<double> flpPosition00 = new CFLPoint<double>(0, 0);

					if((res = layerLearn.DrawTextCanvas(flpPosition00, strStatus, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
						break;

					// 학습한 정보에 대해 Console창에 출력한다 // Print the learned information to the console window
					Console.WriteLine("  < LEARN CLASS {0} > ", arrClassName[i64DataIdx]);
					Console.WriteLine("  1. ROI Shape Type : Rectangle");
					Console.WriteLine("    left   : {0}", learnRegion.left);
					Console.WriteLine("    right  : {0}", learnRegion.right);
					Console.WriteLine("    top    : {0}", learnRegion.top);
					Console.WriteLine("    bottom : {0}", learnRegion.bottom);
					Console.WriteLine("    angle  : {0}", learnRegion.angle);
					Console.WriteLine("  2. Interest Pivot : ({0}, {1})", flpLearnPivot.x, flpLearnPivot.y);
					Console.WriteLine("");

					// 이미지 뷰를 갱신 합니다. // Update the image view.
					viewImageLearn[i64DataIdx].Invalidate(true);
				}

				// 데이터 추가를 완료 후 Save를 진행합니다.
				if((res = FLGeometricMatchMultiSave.Save("../../ExampleImages/Matching/Geometric Multi Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to save\n");
					break;
				}

				// 이미지 로드 // Load image
				if((res = fliFindImage.Load("../../ExampleImages/Matching/Geometric Mult Find.flif")).IsFail())
					break;

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageFind.Create(400, 384, 1168, 960)).IsFail())
					break;

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageFind.SetImagePtr(fliFindImage)).IsFail())
					break;

				for(long i64DataIdx = 0; i64DataIdx < 3; ++i64DataIdx)
				{
					// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
					if((res = viewImageFind.SynchronizeWindow(viewImageLearn[i64DataIdx])).IsFail())
						break;
				}

				CGUIViewImageLayer layerFind = viewImageFind.GetLayer(1);
				layerFind.Clear();

				CFLPoint<double> flp00 = new CFLPoint<double>(0, 0);

				if((res = layerFind.DrawTextCanvas(flp00, "FIND", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					break;

				// 데이터를 Load합니다.
				if((res = FLGeometricMatchMultiLoad.Load("../../ExampleImages/Matching/Geometric Multi Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to save\n");
					break;
				}

				// 검출할 이미지 설정 // Set image to detect
				FLGeometricMatchMultiLoad.SetSourceImage(ref fliFindImage);

				// 검출 시 사용될 파라미터를 설정합니다. // Set the parameters to be used for detection.
				// 검출 시 사용될 기본 각도를 설정합니다. // Set the default angle to be used for detection.
				FLGeometricMatchMultiLoad.SetAngleBias(0.0);
				// 검출 시 사용될 각도의 탐색범위를 설정합니다. // Set the search range of the angle to be used for detection.
				// 각도는 기본 각도를 기준으로 (기본 각도 - AngleTolerance, 기본 각도 + AngleTolerance)가 최종 탐색범위 // The angle is based on the basic angle (default angle - AngleTolerance, basic angle + AngleTolerance) is the final search range
				FLGeometricMatchMultiLoad.SetAngleTolerance(180.0);
				// 검출 시 사용될 스케일 탐색범위를 설정합니다. // Set the scale search range to be used for detection.
				// 학습 이미지의 크기가 1.00이며, 0.98의 경우 0.98배의 크기를 나타냄
				FLGeometricMatchMultiLoad.SetScaleRange(0.98, 1.02);
				// 검출 시 사용될 최소 탐색점수를 설정합니다. // Set the minimum search score to be used for detection.
				FLGeometricMatchMultiLoad.SetMinimumDetectionScore(0.5);
				// 검출 시 사용될 탐색 방식을 설정합니다. // Set the search method to be used for detection.
				FLGeometricMatchMultiLoad.SetMaxObjectMode(CGeometricMatchMulti.EMaxObjectMode.Total);
				// 검출 시 사용될 최대 탐색객체 수를 설정합니다. // Set the maximum number of search objects to be used for detection.
				FLGeometricMatchMultiLoad.SetMaxObjectTotal(16);

				// 검출 시 보간법 사용 유무에 대해 설정합니다. // Set whether to use interpolation when detecting.
				FLGeometricMatchMultiLoad.EnableInterpolation(true);
				// 검출 시 최적화 정도에 대해 설정합니다. // Set the degree of optimization for detection.
				FLGeometricMatchMultiLoad.SetOptimizationOption(CGeometricMatchMulti.EOptimizationOption.Fastest);
				// 검출 시 대비정도에 대해 설정합니다. // Set the contrast level for detection.
				FLGeometricMatchMultiLoad.SetConstrastOption(FLImagingCLR.AdvancedFunctions.EGeometricMatchMultiConstrastOption.Normal);
				// 검출 시 이미지 영역밖의 탐색 정도를 설정합니다. // Set the degree of search outside the image area when detecting.
				FLGeometricMatchMultiLoad.SetInvisibleRegionEstimation(1.25);
				// 검출 시 처리과정에서의 허용 임계값을 설정합니다. // Set the allowable threshold in the process of detection.
				FLGeometricMatchMultiLoad.SetFindThresholdCoeff(1.0);
				// 검출 시 겹쳐짐 허용 정도를 설정합니다. // Set the allowable degree of overlap during detection.
				FLGeometricMatchMultiLoad.SetObjectOverlap(0.8);
				// 검출 시 이미지 전처리 유무를 설정합니다. // Set whether or not to pre-process the image during detection.
				// 학습과정에서 전처리 유무를 설정과 동일하게 설정하는 것을 추천합니다. // In the learning process, it is recommended to set the pre-processing status the same as the setting.
				FLGeometricMatchMultiLoad.EnablePreprocessing(false);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = FLGeometricMatchMultiLoad.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}

				// 기하학적 패턴 검출 결과를 가져옵니다. // Get the geometric pattern detection result.
				long i64ResultCount = FLGeometricMatchMultiLoad.GetResultCount();

				for(long i = 0; i < i64ResultCount; ++i)
				{
					CGeometricMatchMulti.SResult results;
					CFLFigureArray flfaResultPoints;

					FLGeometricMatchMultiLoad.GetResult(i, out results);
					FLGeometricMatchMultiLoad.GetResultForDetectedFeature(i, out flfaResultPoints);

					bool bInverse = results.bInverse;
					float f32Score = results.f32Score;
					float f32Angle = results.f32Angle;
					float f32Scale = results.f32Scale;
					CFLFigure pFlfRegion = new CFLRect<double>(results.pFlfRegion);
					CFLPoint<double> flpLocation = new CFLPoint<double>(results.pFlpLocation);
					CFLPoint<double> flpPivot = new CFLPoint<double>(results.pFlpPivot);

					string strInverse = bInverse ? "Inverse Type" : "Normal Type";
					string wstrClassName = results.pStrClassName;

					long i64Idx = 0;

					for(long i64ResultIndex = 0; i64ResultIndex < 3; ++i64ResultIndex)
					{
						if(wstrClassName == arrClassName[i64ResultIndex])
						{
							i64Idx = i64ResultIndex;
							break;
						}
					}

					CFLRect<double> pFlrResultRegion = new CFLRect<double>(pFlfRegion);

					// 기하학적 패턴 검출 결과를 Console창에 출력합니다. // Output the geometric pattern detection result to the console window.
					Console.WriteLine(" < Instance : {0} >", i);
					Console.WriteLine(" Class Name); : {0}", wstrClassName);
					Console.WriteLine("  1. ROI Shape Type : Rectangle");
					Console.WriteLine("    left   : {0}", pFlrResultRegion.left);
					Console.WriteLine("    right  : {0}", pFlrResultRegion.right);
					Console.WriteLine("    top    : {0}", pFlrResultRegion.top);
					Console.WriteLine("    bottom : {0}", pFlrResultRegion.bottom);
					Console.WriteLine("    angle  : {0}", pFlrResultRegion.angle);
					Console.WriteLine("  2. Interest Pivot : ({0}, {1})", flpPivot.x, flpPivot.y);
					Console.WriteLine("  3. Score : {0}\n  4. Angle : {1}\n  5. Scale : x{2}", f32Score, f32Angle, f32Scale);

					if(bInverse)
						Console.WriteLine(" Inverse Type");
					else
						Console.WriteLine(" Normal Type");

					Console.WriteLine("");

					// 검출 결과의 중심점을 디스플레이 한다 // Display the center point of the detection result // Display the center point of the detection result
					CFLFigureArray flfaPoint = flpPivot.MakeCrossHair(3, false);
					flfaPoint.Rotate(f32Angle, flpPivot);

					if((res = layerFind.DrawFigureImage(flfaPoint, EColor.BLACK, 3)).IsFail())
						break;

					if((res = layerFind.DrawFigureImage(flfaPoint, EColor.LIME)).IsFail())
						break;

					TPoint<double> tpPosition = new TPoint<double>();
					tpPosition.x = flpPivot.x;
					tpPosition.y = flpPivot.y;

					tpPosition.x += 10;
					string strText = String.Format("Score : {0}\nAngle : {1}\nScale : x{2}\n", f32Score, f32Angle, f32Scale);

					// 결과 특징점을 디스플레이 한다 // Display the resulting feature point
					if((res = layerFind.DrawFigureImage(flfaResultPoints, arrColor[i64Idx])).IsFail())
						break;

					if((res = layerFind.DrawTextImage(tpPosition, strText, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_CENTER)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text");
						break;
					}

					tpPosition.x -= 10;

					if((res = layerFind.DrawTextImage(tpPosition, wstrClassName, EColor.YELLOW, EColor.BLACK, 30, false, 0, EGUIViewImageTextAlignment.CENTER)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text");
						break;
					}
				}

				viewImageFind.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImageLearn[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

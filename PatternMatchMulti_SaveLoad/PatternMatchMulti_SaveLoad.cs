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
	class PatternMatchMulti_SaveLoad
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
			CFLImage[] fliLearnImage = new CFLImage[2];
			CFLImage fliFindImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImageLearn = new CGUIViewImage[2];
			CGUIViewImage viewImageFind = new CGUIViewImage();

			for(int i = 0; i < 2; i++)
			{
				fliLearnImage[i] = new CFLImage();
				viewImageLearn[i] = new CGUIViewImage();
			}

			CResult res = new CResult();

			// Pattern Match Multi 객체 생성 // Create Pattern Match Multi object
			CPatternMatchMulti FLPatternMatchMultiSave = new CPatternMatchMulti();
			CPatternMatchMulti FLPatternMatchMultiLoad = new CPatternMatchMulti();

			do
			{
				string[] arrPath = new string[2];
				arrPath[0] = "../../ExampleImages/Matching/Pattern Multi Learn.flif";
				arrPath[1] = "../../ExampleImages/Matching/Pattern Multi Learn.flif";

				string[] arrClassName = new string[2];
				arrClassName[0] = "A";
				arrClassName[1] = "B";
				EColor[] arrColor = new EColor[2];
				arrColor[0] = EColor.LIME;
				arrColor[1] = EColor.RED;
				CFLRect<double>[] arrLearnRegion = new CFLRect<double>[2];
				arrLearnRegion[0] = new CFLRect<double>(178.9984, 178.9984, 253.9842, 257.2094);
				arrLearnRegion[1] = new CFLRect<double>(110.4629, 109.6566, 182.2236, 178.9984);

				for(long i64DataIdx = 0; i64DataIdx < 2; ++i64DataIdx)
				{
					// 이미지 로드 // Load image
					if((res = fliLearnImage[i64DataIdx].Load(arrPath[i64DataIdx])).IsFail())
					{
						ErrorPrint(res, "Failed to load the image file.");
						break;
					}

					// 이미지 뷰 생성 // Create image view
					if((res = viewImageLearn[i64DataIdx].Create((int)(400 + 512 * i64DataIdx), 0, (int)(400 + 512 * (i64DataIdx + 1)), 384)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.");
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
					if((res = viewImageLearn[i64DataIdx].SetImagePtr(ref fliLearnImage[i64DataIdx])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.");
						break;
					}

					CGUIViewImageLayer layerLearn = viewImageLearn[i64DataIdx].GetLayer(0);

					layerLearn.Clear();

					// 학습할 이미지 설정 // Set the image to learn
					FLPatternMatchMultiSave.SetLearnImage(ref fliLearnImage[i64DataIdx]);

					// 학습할 영역을 설정합니다. // Set the area to learn.
					CFLPoint<double> flpLearnPivot = new CFLPoint<double>(arrLearnRegion[i64DataIdx].GetCenter());
					FLPatternMatchMultiSave.SetLearnROI(arrLearnRegion[i64DataIdx]);
					FLPatternMatchMultiSave.SetLearnPivot(flpLearnPivot);

					// 알고리즘 수행 // Execute the Algoritm
					if((FLPatternMatchMultiSave.Learn(arrClassName[i64DataIdx])).IsFail())
					{
						ErrorPrint(res, "Failed to Learn.");
						break;
					}

					// 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the measurement area is
					if((res = layerLearn.DrawFigureImage(arrLearnRegion[i64DataIdx], EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layerLearn.DrawFigureImage(arrLearnRegion[i64DataIdx], arrColor[i64DataIdx])).IsFail())
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

					string strStatus;
					strStatus = String.Format("LEARN CLASS {0}", arrClassName[i64DataIdx]);

					CFLPoint<double> flpPosition00 = new CFLPoint<double>(0, 0);

					if((res = layerLearn.DrawTextCanvas(flpPosition00, strStatus, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text");
						break;
					}

					// 학습한 정보에 대해 Console창에 출력한다 // Print the learned information to the console window
					Console.WriteLine("  < LEARN CLASS {0} > ", arrClassName[i64DataIdx]);
					Console.WriteLine("  1. ROI Shape Type : Rectangle");
					Console.WriteLine("    left   : {0}", arrLearnRegion[i64DataIdx].left);
					Console.WriteLine("    right  : {0}", arrLearnRegion[i64DataIdx].right);
					Console.WriteLine("    top    : {0}", arrLearnRegion[i64DataIdx].top);
					Console.WriteLine("    bottom : {0}", arrLearnRegion[i64DataIdx].bottom);
					Console.WriteLine("    angle  : {0}", arrLearnRegion[i64DataIdx].angle);
					Console.WriteLine("  2. Interest Pivot : ({0}, {0})", flpLearnPivot.x, flpLearnPivot.y);
					Console.WriteLine("");

					// 이미지 뷰를 갱신 합니다. // Update the image view.
					viewImageLearn[i64DataIdx].Invalidate(true);
				}

				// 데이터 추가를 완료 후 Save를 진행합니다. // After completing data addition, proceed with Save.
				if((res = FLPatternMatchMultiSave.Save("../../ExampleImages/Matching/Pattern Multi Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to save\n");
					break;
				}

				// 이미지 로드 // Load image
				if((res = fliFindImage.Load("../../ExampleImages/Matching/Pattern Multi Find.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageFind.Create(400, 384, 1168, 960)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageFind.SetImagePtr(ref fliFindImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				for(long i64DataIdx = 0; i64DataIdx < 2; ++i64DataIdx)
				{
					// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
					if((res = viewImageFind.SynchronizeWindow(ref viewImageLearn[i64DataIdx])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.");
						break;
					}
				}

				CGUIViewImageLayer layerFind = viewImageFind.GetLayer(1);
				layerFind.Clear();

				CFLPoint<double> flp00 = new CFLPoint<double>(0, 0);

				if((res = layerFind.DrawTextCanvas(flp00, "FIND", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = FLPatternMatchMultiLoad.Load("../../ExampleImages/Matching/Pattern Multi Learn")).IsFail())
				{
					ErrorPrint(res, "Failed to load\n");
					break;
				}

				// 검출할 이미지 설정 // Set image to detect
				FLPatternMatchMultiLoad.SetSourceImage(ref fliFindImage);

				// 검출 시 사용될 파라미터를 설정합니다. // Set the parameters to be used for detection.
				// 검출 시 사용될 스케일 구간을 설정합니다.
				FLPatternMatchMultiLoad.SetScaleRange(1.0, 1.0);
				// 검출 시 사용될 기본 각도를 설정합니다. // Set the default angle to be used for detection.
				FLPatternMatchMultiLoad.SetAngleBias(0.0);
				// 검출 시 사용될 각도의 탐색범위를 설정합니다. // Set the search range of the angle to be used for detection.
				// 각도는 기본 각도를 기준으로 (기본 각도 - AngleTolerance, 기본 각도 + AngleTolerance)가 최종 탐색범위 // The angle is based on the basic angle (default angle - AngleTolerance, basic angle + AngleTolerance) is the final search range
				FLPatternMatchMultiLoad.SetAngleTolerance(15.0);
				// 검출 시 최적화 정도를 설정합니다. // Set the degree of optimization for detection.
				// 0 ~ 1범위에서 0에 가까울수록 정확성은 낮아질 수 있으나, 속도가 상향됩니다. // From 0 to 1, the closer to 0, the lower the accuracy, but the higher the speed.
				FLPatternMatchMultiLoad.SetAccuracy(0.5);
				// 검출 시 사용될 최소 탐색점수를 설정합니다. // Set the minimum search score to be used for detection.
				FLPatternMatchMultiLoad.SetMinimumDetectionScore(0.7);
				// 검출 시 사용될 탐색 방식을 설정합니다. // Set the search method to be used for detection.
				FLPatternMatchMultiLoad.SetMaxObjectMode(CPatternMatchMulti.EMaxObjectMode.Total);
				// 검출 시 사용될 최대 탐색객체 수를 설정합니다. // Set the maximum number of search objects to be used for detection.
				FLPatternMatchMultiLoad.SetMaxObjectTotal(2);
				// 검출 시 보간법 사용 유무에 대해 설정합니다. // Set whether to use interpolation when detecting.
				FLPatternMatchMultiLoad.EnableInterpolation(true);
				// 검출 시 서로 다른 클래스에 대해 영역 중복을 허용 유무에 대해 설정합니다. // Set whether to allow area overlap for different classes during detection.
				FLPatternMatchMultiLoad.SetConflictDetectionMethod(CPatternMatchMulti.EConflictDetectionMethod.HighestScore);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = FLPatternMatchMultiLoad.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}

				// 패턴 검출 결과를 가져옵니다. // Get the pattern detection result.
				long i64ResultCount = FLPatternMatchMultiLoad.GetResultCount();

				for(long i = 0; i < i64ResultCount; ++i)
				{
					CPatternMatchMulti.SResult results = new CPatternMatchMulti.SResult();

					FLPatternMatchMultiLoad.GetResult(i, ref results);

					float f32Score = results.f32Score;
					float f32Angle = results.f32Angle;
					float f32Scale = results.f32Scale;
					CFLPoint<double> flpPivot = new CFLPoint<double>(results.pFlpPivot);
					CFLFigure pFlfRegion = new CFLRect<double>(results.pFlfRegion);
					CFLRect<double> pFlrResultRegion = new CFLRect<double>(pFlfRegion);

					CFLRect<double> flrResultRegion = pFlrResultRegion;

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

					// 패턴 검출 결과를 Console창에 출력합니다. // Output the pattern detection result to the console window.
					Console.WriteLine(" < Instance : {0} >", i);
					Console.WriteLine(" Class Name); : {0}", wstrClassName);
					Console.WriteLine("  1. ROI Shape Type : Rectangle");
					Console.WriteLine("    left   : {0}", flrResultRegion.left);
					Console.WriteLine("    right  : {0}", flrResultRegion.right);
					Console.WriteLine("    top    : {0}", flrResultRegion.top);
					Console.WriteLine("    bottom : {0}", flrResultRegion.bottom);
					Console.WriteLine("    angle  : {0}", flrResultRegion.angle);
					Console.WriteLine("  2. Interest Pivot : ({0}, {1})", flpPivot.x, flpPivot.y);
					Console.WriteLine("  3. Score : {0}\n  4. Angle : {1}\n  5. Scale : {2}", f32Score, f32Angle, f32Scale);

					Console.WriteLine("");

					if((res = layerFind.DrawFigureImage(flrResultRegion, EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

					if((res = layerFind.DrawFigureImage(flrResultRegion, arrColor[i64Idx])).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure");
						break;
					}

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

					TPoint<double> tpPosition = new TPoint<double>();
					tpPosition.x = flpPivot.x;
					tpPosition.y = flpPivot.y;

					// 검출 결과에 해당하는 클래스명을 디스플레이 한다 // Display the class name corresponding to the detection result
					if((res = layerFind.DrawTextImage(tpPosition, wstrClassName, EColor.YELLOW, EColor.BLACK, 30, false, 0, EGUIViewImageTextAlignment.CENTER)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text");
						break;
					}

					tpPosition.x += 10;
					string strText = String.Format("Score : {0}\nAngle : {1}\nScale : x{2}\n", f32Score, f32Angle, f32Scale);

					if((res = layerFind.DrawTextImage(tpPosition, strText, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_CENTER)).IsFail())
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

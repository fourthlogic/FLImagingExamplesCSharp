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

namespace CameraCalibrator
{
	class CameraCalibrator_Chessboard
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		struct SGridDisplay
		{
			public long i64ImageIdx;
			public long i64ObjectIdx;
			public CCameraCalibrator.CCalibratorGridResult sGridData;
		};

		static bool Calibration(CCameraCalibrator sCC, CFLImage fliLearnImage)
		{
			bool bResult = false;


			CResult res = new CResult();

			do
			{
				// Learn 이미지 설정 // Learn image settings
				if((res = sCC.SetCalibrationImage(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image");
					break;
				}

				// Calibator할 대상 종류를 설정합니다. // Set the target type for Calibator.
				sCC.SetGridType(CCameraCalibrator.EGridType.ChessBoard);
				// 결과에 대한 학습률을 설정합니다.
				sCC.SetOptimalSolutionAccuracy(1e-5);

				// Calibration 실행 // Execute Calibration
				if((res = sCC.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Calibration failed");
					break;
				}

				bResult = true;
			}
			while(false);

			return bResult;
		}

		static bool Undistortion(CCameraCalibrator sCC, CFLImage fliSourceImage, CFLImage fliDestinationImage)
		{
			bool bResult = false;


			CResult res = new CResult();

			do
			{
				// Source 이미지 설정 // Set Source image
				if((res = sCC.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to load image");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				if((res = sCC.SetDestinationImage(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to load image");
					break;
				}

				// Interpolation 알고리즘 설정 // Set the Interpolation Algorithm
				if((res = sCC.SetInterpolationMethod(EInterpolationMethod.Bilinear)).IsFail())
				{
					ErrorPrint(res, "Failed to set interpolation method");
					break;
				}

				// Undistortion 실행 // Execute Undistortion
				if((res = sCC.Execute()).IsFail())
				{
					ErrorPrint(res, "Undistortion failed");
					break;
				}

				bResult = true;
			}
			while(false);

			return bResult;
		}

		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare the image object
			CFLImage fliLearnImage = new CFLImage(), fliSourceImage = new CFLImage(), fliDestinationImage = new CFLImage();
			CFLImage fliLearnImageCopy = new CFLImage();
			CFLImage fliDisplay = new CFLImage();
			CFLImage[] arrFliDisplay = new CFLImage[3];
			arrFliDisplay[0] = new CFLImage();
			arrFliDisplay[1] = new CFLImage();
			arrFliDisplay[2] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImageLearn = new CGUIViewImage[3];
			CGUIViewImage viewImageSource = new CGUIViewImage(), viewImageDestination = new CGUIViewImage();
			viewImageLearn[0] = new CGUIViewImage();
			viewImageLearn[1] = new CGUIViewImage();
			viewImageLearn[2] = new CGUIViewImage();

			// Camera Calibrator 객체 생성 // Create Camera Calibrator object
			CCameraCalibrator sCC = new CCameraCalibrator();
			CResult res = new CResult();

			do
			{
				// Learn 이미지 로드 // Load the Learn image
				if((res = fliLearnImage.Load("../../ExampleImages/CameraCalibrator/ChessBoard/Multiple ChessBoard.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				Console.WriteLine("Processing....");

				fliLearnImageCopy.Assign(fliLearnImage, true);
				fliDisplay = fliLearnImageCopy;
				arrFliDisplay[0].SwapPage(fliDisplay, 0);
				arrFliDisplay[1].SwapPage(fliDisplay, 1);
				arrFliDisplay[2].SwapPage(fliDisplay, 2);

				for(int i = 0; i < 3; ++i)
				{
					// Learn 이미지 뷰 생성 // Create the Learn image view
					if((res = viewImageLearn[i].Create(300 + 480 * i, 0, 300 + 480 * (i + 1), 360)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.");
						break;
					}

					// Learn 이미지 뷰에 이미지를 디스플레이 // Display the image in the Learn image view
					if((res = viewImageLearn[i].SetImagePtr(ref arrFliDisplay[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.");
						break;
					}
				}

				if(!Calibration(sCC, fliLearnImage))
					break;

				// Source 이미지 로드 // Load the source image
				if((res = fliSourceImage.Load("../../ExampleImages/CameraCalibrator/ChessBoard/UndistortionImage/Undistortion_Image.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				CMultiVar<long> mvBlank = new CMultiVar<long>(0);

				// Destination 이미지 생성 // Create destination image
				if((res = fliDestinationImage.Create(fliSourceImage.GetWidth(), fliSourceImage.GetHeight(), mvBlank, fliSourceImage.GetPixelFormat())).IsFail())
				{
					ErrorPrint(res, "Failed to create the image file.");
					break;
				}

				if(!Undistortion(sCC, fliSourceImage, fliDestinationImage))
					break;

				// 화면에 격자 탐지 결과 출력 // Output the grid detection result on the screen
				SGridDisplay[] sArrGridDisplay = new SGridDisplay[3];

				for(int i = 0; i < 3; ++i)
				{
					sArrGridDisplay[i] = new SGridDisplay();

					sArrGridDisplay[i].i64ImageIdx = 0;
					sArrGridDisplay[i].i64ObjectIdx = 0;
					sArrGridDisplay[i].sGridData = new CCameraCalibrator.CCalibratorGridResult();
				}

				for(long i64ImgIdx = 0; i64ImgIdx < (long)fliLearnImage.GetPageCount(); ++i64ImgIdx)
				{
					long i64ObjectCount = sCC.GetResultGridPointsObjectCnt(i64ImgIdx);

					for(long i64ObjectIdx = 0; i64ObjectIdx < i64ObjectCount; ++i64ObjectIdx)
					{
						sCC.GetResultGridPoints(i64ObjectIdx, i64ImgIdx, ref sArrGridDisplay[i64ImgIdx].sGridData);
						sArrGridDisplay[i64ImgIdx].i64ImageIdx = i64ImgIdx;
						sArrGridDisplay[i64ImgIdx].i64ObjectIdx = sArrGridDisplay[i64ImgIdx].sGridData.i64ID;
					}
				}

				for(int i = 0; i < 3; ++i)
				{
					CGUIViewImageLayer layerLearn = viewImageLearn[i].GetLayer(0);

					layerLearn.Clear();

					EColor[] colorPool = new EColor[3];
					colorPool[0] = EColor.RED;
					colorPool[1] = EColor.LIME;
					colorPool[2] = EColor.CYAN;

					int i64GridRow = (int)sArrGridDisplay[i].sGridData.i64Rows;
					int i64GridCol = (int)sArrGridDisplay[i].sGridData.i64Columns;

					for(int i64Row = 0; i64Row < i64GridRow; ++i64Row)
					{
						for(int i64Col = 0; i64Col < i64GridCol - 1; ++i64Col)
						{
							int i64GridIdx = i64Row * i64GridCol + i64Col;
							CFLPoint<double> pFlpGridPoint1 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(i64Col));
							CFLPoint<double> pFlpGridPoint2 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(i64Col + 1));
							CFLLine<double> fllDrawLine = new CFLLine<double>(pFlpGridPoint1, pFlpGridPoint2);

							if((res = layerLearn.DrawFigureImage(fllDrawLine, EColor.BLACK, 5)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure");
								break;
							}

							if((res = layerLearn.DrawFigureImage(fllDrawLine, colorPool[i64GridIdx % 3], 3)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure");
								break;
							}
						}

						if(i64Row < i64GridRow - 1)
						{
							CFLPoint<double> pFlpGridPoint1 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(i64GridCol - 1));
							CFLPoint<double> pFlpGridPoint2 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row + 1)).GetAt(0));
							CFLLine<double> fllDrawLine = new CFLLine<double>();
							fllDrawLine.Set(pFlpGridPoint1, pFlpGridPoint2);

							if((res = layerLearn.DrawFigureImage(fllDrawLine, EColor.BLACK, 5)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure.\n");
								break;
							}

							if((res = layerLearn.DrawFigureImage(fllDrawLine, EColor.YELLOW, 3)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure.\n");
								break;
							}
						}
					}

					EColor colorText = EColor.YELLOW;
					colorPool[2] = EColor.CYAN;
					double f64PointDist = 0;
					double f64Dx = 0;
					double f64Dy = 0;

					for(int i64Row = 0; i64Row < i64GridRow; ++i64Row)
					{
						CFLPoint<double> tpGridPoint1 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(0));
						CFLPoint<double> tpGridPoint2 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(1));
						CFLPoint<double> flpGridPoint1 = new CFLPoint<double>(tpGridPoint1.x, tpGridPoint1.y);
						CFLPoint<double> flpGridPoint2 = new CFLPoint<double>(tpGridPoint2.x, tpGridPoint2.y);

						double f64AngleIner = flpGridPoint1.GetAngle(flpGridPoint2);

						for(int i64Col = 0; i64Col < i64GridCol; ++i64Col)
						{
							long i64GridIdx = i64Row * i64GridCol + i64Col;

							if(i64Col < i64GridCol - 1)
							{
								tpGridPoint1 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(i64Col));
								tpGridPoint2 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(i64Col + 1));

								f64Dx = tpGridPoint2.x - tpGridPoint1.x;
								f64Dy = tpGridPoint2.y - tpGridPoint1.y;
								f64PointDist = Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy);
							}

							if(i64Row > 0)
							{
								tpGridPoint1 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row)).GetAt(i64Col));
								tpGridPoint2 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(i64Row - 1)).GetAt(i64Col));

								f64Dx = tpGridPoint2.x - tpGridPoint1.x;
								f64Dy = tpGridPoint2.y - tpGridPoint1.y;
								f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
							}
							else
							{
								tpGridPoint1 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(0)).GetAt(i64Col));
								tpGridPoint2 = (CFLPoint<double>)(((CFLFigureArray)sArrGridDisplay[i].sGridData.flfaGridData.GetAt(1)).GetAt(i64Col));

								f64Dx = tpGridPoint2.x - tpGridPoint1.x;
								f64Dy = tpGridPoint2.y - tpGridPoint1.y;
								f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
							}

							string wstrGridIdx;
							wstrGridIdx = String.Format("{0}", i64GridIdx);
							colorText = colorPool[i64GridIdx % 3];

							if(i64Col == i64GridCol - 1)
								colorText = EColor.YELLOW;

							if((res = layerLearn.DrawTextImage(tpGridPoint1, wstrGridIdx, colorText, EColor.BLACK, (int)(f64PointDist / 2), true, f64AngleIner)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure.\n");
								break;
							}
						}
					}


					CFLQuad<double> flqBoardRegion = sArrGridDisplay[i].sGridData.pFlqBoardRegion;
					CFLPoint<double> flpPoint1 = new CFLPoint<double>(flqBoardRegion.flpPoints[0]);
					CFLPoint<double> flpPoint2 = new CFLPoint<double>(flqBoardRegion.flpPoints[1]);
					double f64Angle = flpPoint1.GetAngle(flpPoint2);
					string wstringData;
					wstringData = string.Format("[{0}] ({1} X {2})", sArrGridDisplay[i].sGridData.i64ID, sArrGridDisplay[i].sGridData.i64Columns, sArrGridDisplay[i].sGridData.i64Rows);

					if((res = layerLearn.DrawFigureImage(flqBoardRegion, EColor.YELLOW, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure.\n");
						break;
					}


					if((res = layerLearn.DrawTextImage(flpPoint1, wstringData, EColor.YELLOW, EColor.BLACK, 15, false, f64Angle, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text.\n");
						break;
					}

					viewImageLearn[i].Invalidate();
				}

				// Source 이미지 뷰 생성 // Create Source image view
				if((res = viewImageSource.Create(300, 360, 780, 720)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the Destination image view
				if((res = viewImageDestination.Create(780, 360, 1260, 720)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the Source ImageView
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the Destination image view
				if((res = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				for(int i = 0; i < 3; ++i)
				{
					// 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.
					if((res = viewImageLearn[i].SynchronizePointOfView(ref viewImageSource)).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view");
						break;
					}

					// 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.
					if((res = viewImageLearn[i].SynchronizePointOfView(ref viewImageDestination)).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view");
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
					if((res = viewImageLearn[i].SynchronizeWindow(ref viewImageSource)).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.");
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
					if((res = viewImageLearn[i].SynchronizeWindow(ref viewImageDestination)).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.");
						break;
					}
				}

				// calibration data 출력 // Output calibration data 
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CCameraCalibrator.CCalibratorIntrinsicParameters sIntrinsicParam = sCC.GetResultIntrinsicParameters();
				CCameraCalibrator.CCalibratorDistortionCoefficients sDistortCoeef = sCC.GetResultDistortionCoefficients();

				string strMatrix = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", sIntrinsicParam.f64FocalLengthX, sIntrinsicParam.f64Skew, sIntrinsicParam.f64PrincipalPointX, 0, sIntrinsicParam.f64FocalLengthY, sIntrinsicParam.f64PrincipalPointY, 0, 0, 1);

				string strDistVal = String.Format("{0}, {1}, {2}, {3}, {4}", sDistortCoeef.f64K1, sDistortCoeef.f64K2, sDistortCoeef.f64P1, sDistortCoeef.f64P2, sDistortCoeef.f64K3);

				Console.WriteLine("Intrinsic parameters");
				Console.WriteLine("{0}", strMatrix);
				Console.WriteLine("Distortion Coefficients");
				Console.WriteLine("{0}", strDistVal);


				TPoint<double> tpScreen = new TPoint<double>(0, 0);

				if((res = layerSource.DrawTextCanvas(tpScreen, "Intrinsic Parameters: ", EColor.YELLOW, EColor.BLACK, 13)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				tpScreen.y += 20;

				if((res = layerSource.DrawTextCanvas(tpScreen, strMatrix, EColor.YELLOW, EColor.BLACK, 13)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				tpScreen.y += 20;

				if((res = layerSource.DrawTextCanvas(tpScreen, "Distortion Coefficients: ", EColor.YELLOW, EColor.BLACK, 13)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				tpScreen.y += 20;

				if((res = layerSource.DrawTextCanvas(tpScreen, strDistVal, EColor.YELLOW, EColor.BLACK, 13)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

				layerDestination.Clear();

				CFLPoint<double> ptTop = new CFLPoint<double>(20, 20);

				if((res = layerDestination.DrawTextImage(ptTop, "Undistortion - Bilinear method", EColor.GREEN, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				viewImageSource.Invalidate();
				viewImageDestination.Invalidate();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImageLearn[0].IsAvailable() && viewImageLearn[1].IsAvailable() && viewImageLearn[2].IsAvailable() && viewImageSource.IsAvailable() && viewImageDestination.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

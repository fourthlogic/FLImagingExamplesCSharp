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

namespace OrthogonalCalibrator
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

		struct SGridDisplay
        {
            public long i64ImageIdx;
            public long i64ObjectIdx;
            public COrthogonalCalibrator.SGridResult sGridData;
        };

        static bool Calibration(COrthogonalCalibrator sCC, CFLImage fliLearnImage)
        {
            bool bResult = false;

            
            CResult eResult = new CResult();
             
            do
            {
                // Learn 이미지 설정 // Learn image settings
                if ((eResult = sCC.SetLearnImageForCameraCalibration(ref fliLearnImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image");
                    break;
                }

				// Calibator할 대상 종류를 설정합니다. // Set the target type for Calibator.
				sCC.SetGridTypeForCameraCalibration(COrthogonalCalibrator.EGridType.GridOfDots);

				// 직교 보정 계산을 할 Learn 이미지 설정 // Learn image settings for orthogonal correction
				if((eResult = sCC.SetLearnImageForOrthogonalCorrection(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image");
					break;
				}

				// 직교 보정할 대상 종류를 설정합니다. // Set the target type for orthogonal correction.
				sCC.SetGridTypeForOrthogonalCorrection(COrthogonalCalibrator.EGridType.GridOfDots);

				// 결과에 대한 학습률을 설정합니다.
				sCC.SetOptimalSolutionAccuracy(1e-5);

                // Calibration 실행 // Execute Calibration
                if ((eResult = sCC.Calibrate()).IsFail())
                {
                    ErrorPrint(eResult, "Calibration failed");
                    break;
                }

                bResult = true;
            }
            while (false);

            return bResult;
        }

        static bool Undistortion(COrthogonalCalibrator sCC, CFLImage fliSourceImage, CFLImage fliDestinationImage)
        {
            bool bResult = false;

            
            CResult eResult = new CResult();

            do
            {
                // Source 이미지 설정 // Set Source image
                if ((eResult = sCC.SetSourceImage(ref fliSourceImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load image");
                    break;
                }

                // Destination 이미지 설정 // Set destination image
                if ((eResult = sCC.SetDestinationImage(ref fliDestinationImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load image");
                    break;
                }

                // Interpolation 알고리즘 설정 // Set the Interpolation Algorithm
                if ((eResult = sCC.SetInterpolationMethod(EInterpolationMethod.Bilinear)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set interpolation method");
                    break;
                }

                // Undistortion 실행 // Execute Undistortion
                if ((eResult = sCC.Execute()).IsFail())
                {
                    ErrorPrint(eResult, "Undistortion failed");
                    break;
                }

                bResult = true;
            }
            while (false);

            return bResult;
        }

        [STAThread]
        static void Main(string[] args)
        {
            // 이미지 객체 선언 // Declare the image object
            CFLImage fliLearnImage = new CFLImage(), fliSourceImage = new CFLImage(), fliDestinationImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageLearn = new CGUIViewImage();
            CGUIViewImage viewImageSource = new CGUIViewImage(), viewImageDestination = new CGUIViewImage();

            // Orthogonal Calibrator 객체 생성 // Create Orthogonal Calibrator object
            COrthogonalCalibrator sCC = new COrthogonalCalibrator();
			CResult eResult = new CResult();

			do
			{
                // Learn 이미지 로드 // Load the Learn image
                if ((eResult = fliLearnImage.Load("../../ExampleImages/OrthogonalCalibrator/Orthogonal_GridOfDots.flif")).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load the image file.");
                    break;
                }

                // Learn 이미지 뷰 생성 // Create the Learn image view
                if ((eResult = viewImageLearn.Create(300, 0, 300 + 480, 360)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.");
                    break;
                }

                // Learn 이미지 뷰에 이미지를 디스플레이 // Display the image in the Learn image view
                if ((eResult = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view.");
                    break;
                }

                Console.WriteLine("Processing....");

                if (!Calibration(sCC, fliLearnImage))
                    break;

                // Source 이미지 로드 // Load the source image
                if ((eResult = fliSourceImage.Load("../../ExampleImages/OrthogonalCalibrator/Orthogonal_GridOfDots.flif")).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load the image file.");
                    break;
                }

                CMultiVar<long> mvBlank = new CMultiVar<long>(0);

                // Destination 이미지 생성 // Create destination image
                if ((eResult = fliDestinationImage.Create(fliSourceImage.GetWidth(), fliSourceImage.GetHeight(), mvBlank, fliSourceImage.GetPixelFormat())).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image file.");
                    break;
                }

                if (!Undistortion(sCC, fliSourceImage, fliDestinationImage))
                    break;

                // 화면에 격자 탐지 결과 출력 // Output the grid detection result on the screen
                SGridDisplay sArrGridDisplay = new SGridDisplay();
                long i64ObjectCount = sCC.GetResultGridPointsObjectCnt(0);

                for (long i64ObjectIdx = 0; i64ObjectIdx < i64ObjectCount; ++i64ObjectIdx)
                {
                    sArrGridDisplay.sGridData = new COrthogonalCalibrator.SGridResult();
                    sCC.GetResultGridPoints(i64ObjectIdx, 0, out sArrGridDisplay.sGridData);
                    sArrGridDisplay.i64ImageIdx = 0;
                    sArrGridDisplay.i64ObjectIdx = sArrGridDisplay.sGridData.i64ID;
                }

                CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);

                layerLearn.Clear();

                EColor[] colorPool = new EColor[3];
                colorPool[0] = EColor.RED;
                colorPool[1] = EColor.LIME;
                colorPool[2] = EColor.BLUE;

                int i64GridRow = (int)sArrGridDisplay.sGridData.i64Rows;
                int i64GridCol = (int)sArrGridDisplay.sGridData.i64Columns;

                for (int i64Row = 0; i64Row < i64GridRow; ++i64Row)
                {
                    for (int i64Col = 0; i64Col < i64GridCol - 1; ++i64Col)
                    {
                        int i64GridIdx = i64Row * i64GridCol + i64Col;
                        TPoint<double> pFlpGridPoint1 = (sArrGridDisplay.sGridData.arrGridData[i64Row][i64Col]);
                        TPoint<double> pFlpGridPoint2 = (sArrGridDisplay.sGridData.arrGridData[i64Row][i64Col + 1]);
                        CFLLine<double> fllDrawLine = new CFLLine<double>(pFlpGridPoint1, pFlpGridPoint2);

                        if ((eResult = layerLearn.DrawFigureImage(fllDrawLine, EColor.BLACK, 5)).IsFail())
                        {
                            ErrorPrint(eResult, "Failed to draw figure");
                            break;
                        }

                        if ((eResult = layerLearn.DrawFigureImage(fllDrawLine, colorPool[i64GridIdx % 3], 3)).IsFail())
                        {
                            ErrorPrint(eResult, "Failed to draw figure");
                            break;
                        }
                    }

                    if (i64Row < i64GridRow - 1)
                    {
                        TPoint<double> pFlpGridPoint1 = (sArrGridDisplay.sGridData.arrGridData[i64Row][i64GridCol - 1]);
                        TPoint<double> pFlpGridPoint2 = (sArrGridDisplay.sGridData.arrGridData[i64Row + 1][0]);
                        CFLLine<double> fllDrawLine = new CFLLine<double>();
                        fllDrawLine.Set(pFlpGridPoint1, pFlpGridPoint2);

                        if ((eResult = layerLearn.DrawFigureImage(fllDrawLine, EColor.BLACK, 5)).IsFail())
                        {
                            ErrorPrint(eResult, "Failed to draw figure.\n");
                            break;
                        }

                        if ((eResult = layerLearn.DrawFigureImage(fllDrawLine, EColor.YELLOW, 3)).IsFail())
                        {
                            ErrorPrint(eResult, "Failed to draw figure.\n");
                            break;
                        }
                    }
                }

                EColor colorText = EColor.YELLOW;
                colorPool[2] = EColor.CYAN;
                double f64PointDist = 0;
                double f64Dx = 0;
                double f64Dy = 0;

                for (int i64Row = 0; i64Row < i64GridRow; ++i64Row)
                {
					TPoint<double> pFlpGridPoint1 = (sArrGridDisplay.sGridData.arrGridData[i64Row][0]);
                    TPoint<double> pFlpGridPoint2 = (sArrGridDisplay.sGridData.arrGridData[i64Row][1]);
					CFLPoint<double> flpGridPoint1 = new CFLPoint<double>(pFlpGridPoint1.x, pFlpGridPoint1.y);
					CFLPoint<double> flpGridPoint2 = new CFLPoint<double>(pFlpGridPoint2.x, pFlpGridPoint2.y);
					double f64AngleIner = flpGridPoint1.GetAngle(flpGridPoint2);

                    for (int i64Col = 0; i64Col < i64GridCol; ++i64Col)
                    {
                        int i64GridIdx = i64Row * i64GridCol + i64Col;

                        if (i64Col < i64GridCol - 1)
                        {
                            pFlpGridPoint1 = (sArrGridDisplay.sGridData.arrGridData[i64Row][i64Col]);
                            pFlpGridPoint2 = (sArrGridDisplay.sGridData.arrGridData[i64Row][i64Col + 1]);

                            f64Dx = pFlpGridPoint2.x - pFlpGridPoint1.x;
                            f64Dy = pFlpGridPoint2.y - pFlpGridPoint1.y;
                            f64PointDist = Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy);
                        }

                        if (i64Row > 0)
                        {
                            pFlpGridPoint1 = (sArrGridDisplay.sGridData.arrGridData[i64Row][i64Col]);
                            pFlpGridPoint2 = (sArrGridDisplay.sGridData.arrGridData[i64Row - 1][i64Col]);

                            f64Dx = pFlpGridPoint2.x - pFlpGridPoint1.x;
                            f64Dy = pFlpGridPoint2.y - pFlpGridPoint1.y;
                            f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
                        }
                        else
                        {
                            pFlpGridPoint1 = (sArrGridDisplay.sGridData.arrGridData[0][i64Col]);
                            pFlpGridPoint2 = (sArrGridDisplay.sGridData.arrGridData[1][i64Col]);

                            f64Dx = pFlpGridPoint2.x - pFlpGridPoint1.x;
                            f64Dy = pFlpGridPoint2.y - pFlpGridPoint1.y;
                            f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
                        }

                        string wstrGridIdx;
                        wstrGridIdx = String.Format("{0}", i64GridIdx);
                        colorText = colorPool[i64GridIdx % 3];

                        if (i64Col == i64GridCol - 1)
                            colorText = EColor.YELLOW;

                        if ((eResult = layerLearn.DrawTextImage(pFlpGridPoint1, wstrGridIdx, colorText, EColor.BLACK, (int)(f64PointDist / 2), true, f64AngleIner)).IsFail())
                        {
                            ErrorPrint(eResult, "Failed to draw figure.\n");
                            break;
                        }
                    }
                }

                CFLQuad<double> flqBoardRegion = sArrGridDisplay.sGridData.pFlqBoardRegion;
                CFLPoint<double> flpPoint1 = new CFLPoint<double>(flqBoardRegion.flpPoints[0]);
                CFLPoint<double> flpPoint2 = new CFLPoint<double>(flqBoardRegion.flpPoints[1]);
                double f64Angle = flpPoint1.GetAngle(flpPoint2);
                string wstringData;
                wstringData = string.Format("[{0}] ({1} X {2})", sArrGridDisplay.sGridData.i64ID, sArrGridDisplay.sGridData.i64Columns, sArrGridDisplay.sGridData.i64Rows);

                if ((eResult = layerLearn.DrawFigureImage(flqBoardRegion, EColor.YELLOW, 3)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw figure.\n");
                    break;
                }


                if ((eResult = layerLearn.DrawTextImage(flpPoint1, wstringData, EColor.YELLOW, EColor.BLACK, 15, false, f64Angle, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                viewImageLearn.Invalidate();


                // Source 이미지 뷰 생성 // Create Source image view
                if ((eResult = viewImageSource.Create(300, 360, 780, 720)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create the Destination image view
                if ((eResult = viewImageDestination.Create(780, 360, 1260, 720)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the Source ImageView
                if ((eResult = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view.");
                    break;
                }

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the Destination image view
                if ((eResult = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view.");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.
                if ((eResult = viewImageLearn.SynchronizePointOfView(ref viewImageSource)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.
                if ((eResult = viewImageLearn.SynchronizePointOfView(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if ((eResult = viewImageLearn.SynchronizeWindow(ref viewImageSource)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize window.");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if ((eResult = viewImageLearn.SynchronizeWindow(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize window.");
                    break;
                }

                // calibration data 출력 // Output calibration data 
                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                COrthogonalCalibrator.SIntrinsicParameters sIntrinsicParam = sCC.GetResultIntrinsicParameters();
                COrthogonalCalibrator.SDistortionCoefficients sDistortCoeef = sCC.GetResultDistortionCoefficients();

                string strMatrix = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", sIntrinsicParam.f64FocalLengthX, sIntrinsicParam.f64Skew, sIntrinsicParam.f64PrincipalPointX, sIntrinsicParam.f64Padding1, sIntrinsicParam.f64FocalLengthY, sIntrinsicParam.f64PrincipalPointY, sIntrinsicParam.f64Padding2, sIntrinsicParam.f64Padding3, sIntrinsicParam.f64Padding4);

                string strDistVal = String.Format("{0}, {1}, {2}, {3}, {4}", sDistortCoeef.f64F1, sDistortCoeef.f64F2, sDistortCoeef.f64P1, sDistortCoeef.f64P2, sDistortCoeef.f64F3);

                Console.WriteLine("Intrinsic parameters");
                Console.WriteLine("{0}", strMatrix);
                Console.WriteLine("Distortion Coefficients");
                Console.WriteLine("{0}", strDistVal);

               
                TPoint<double> tpScreen = new TPoint<double>(0, 0);

                if ((eResult = layerSource.DrawTextCanvas(tpScreen, "Intrinsic Parameters: ", EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                tpScreen.y += 20;

                if ((eResult = layerSource.DrawTextCanvas(tpScreen, strMatrix, EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                tpScreen.y += 20;

                if ((eResult = layerSource.DrawTextCanvas(tpScreen, "Distortion Coefficients: ", EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                tpScreen.y += 20;

                if ((eResult = layerSource.DrawTextCanvas(tpScreen, strDistVal, EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                layerDestination.Clear();

                CFLPoint<double> ptTop = new CFLPoint<double>(20, 20);

                if ((eResult = layerDestination.DrawTextImage(ptTop, "Undistortion - Bilinear method", EColor.GREEN, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                viewImageSource.Invalidate();
                viewImageDestination.Invalidate();

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
                while (viewImageLearn.IsAvailable() && viewImageSource.IsAvailable() && viewImageDestination.IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

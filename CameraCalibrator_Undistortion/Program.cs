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

namespace CircleGauge
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

		static bool Undistortion(CCameraCalibrator sCC, CFLImage fliSourceImage, CFLImage fliDestinationImage, CGUIViewImage viewImageSource, CGUIViewImage viewImageDestination)
        {
            bool bResult = false;

            
            CResult eResult = new CResult();

            do
            {
                // Source 이미지 로드 // Load the source image
                if ((eResult = fliSourceImage.Load("../../ExampleImages/CameraCalibrator/Undistortion.flif")).IsFail())
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

                // Source 이미지 뷰 생성 // Create Source image view
                if ((eResult = viewImageSource.Create(400, 480, 1040, 960)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create the Destination image view
                if ((eResult = viewImageDestination.Create(1040, 480, 1680, 960)).IsFail())
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

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if ((eResult = viewImageSource.SynchronizeWindow(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize window.");
                    break;
                }

                // Interpolation 알고리즘 설정 // Set the Interpolation Algorithm
                if ((eResult = sCC.SetInterpolationMethod(EInterpolationMethod.Bilinear)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set interpolation method");
                    break;
                }

                CPerformanceCounter sPC = new CPerformanceCounter();
                sPC.Start();

                // Undistortion 실행 // Execute Undistortion
                if ((eResult = sCC.Execute()).IsFail())
                {
                    ErrorPrint(eResult, "Undistortion failed");
                    break;
                }

                sPC.CheckPoint();

                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);

                layerSource.Clear();

                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                layerDestination.Clear();

                CFLPoint<double> ptTop = new CFLPoint<double>(20,20);

                if ((eResult = layerDestination.DrawTextImage(ptTop, "Undistortion - Bilinear method", EColor.GREEN, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                double f64ElapsedMS = sPC.GetCheckPointInMilliSecond();

                string strMS = String.Format("elapsed time: {0} ms", f64ElapsedMS);
                CFLPoint<double> ptMS = new CFLPoint<double>(20, 50);

                if ((eResult = layerDestination.DrawTextImage(ptMS, strMS, EColor.GREEN, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                viewImageDestination.Invalidate(false);

                bResult = true;
            }
            while (false);

            return bResult;
        }

        [STAThread]
		static void Main(string[] args)
        {
            
            CResult eResult = new CResult();

            // 이미지 객체 선언 // Declare the image object
            CFLImage fliSourceImage = new CFLImage(), fliDestinationImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageSource = new CGUIViewImage(), viewImageDestination = new CGUIViewImage();

            // Camera Calibrator 객체 생성 // Create Camera Calibrator object
            CCameraCalibrator sCC = new CCameraCalibrator();

			do
			{
                // Setter로 입력 // Input as setter
                double[] arrF64Intrinc = new double[9]{ 605.9413643192689, 0, 325.9133439121233, 0, 605.3834974915350, 234.0647625697701, 0, 0, 1 };
                double[] arrF64Dist = new double[5]{ 0.1748895907714, -1.4909467274276, -0.0070404809103, 0.0017880490098, 5.9363069879613 };
                CCameraCalibrator.SIntrinsicParameters uIntrinc = new CCameraCalibrator.SIntrinsicParameters();
                CCameraCalibrator.SDistortionCoefficients uDist = new CCameraCalibrator.SDistortionCoefficients();

                uIntrinc.f64FocalLengthX = arrF64Intrinc[0];
                uIntrinc.f64Skew = arrF64Intrinc[1];
                uIntrinc.f64PrincipalPointX = arrF64Intrinc[2];
                uIntrinc.f64Padding1 = arrF64Intrinc[3];
                uIntrinc.f64FocalLengthY = arrF64Intrinc[4];
                uIntrinc.f64PrincipalPointY = arrF64Intrinc[5];
                uIntrinc.f64Padding2 = arrF64Intrinc[6];
                uIntrinc.f64Padding3 = arrF64Intrinc[7];
                uIntrinc.f64Padding4 = arrF64Intrinc[8];

                uDist.f64F1 = arrF64Dist[0];
                uDist.f64F2 = arrF64Dist[1];
                uDist.f64P1 = arrF64Dist[2];
                uDist.f64P2 = arrF64Dist[3];
                uDist.f64F3 = arrF64Dist[4];

                if ((eResult = sCC.SetIntrinsicParameters(uIntrinc)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set intrinsic parameters");
                    break;
                }

                if ((eResult = sCC.SetDistortionCoefficients(uDist)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set distortion coefficients");
                    break;
                }

                if (!Undistortion(sCC, fliSourceImage, fliDestinationImage, viewImageSource, viewImageDestination))
                    break;

                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                string strMatrix = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", uIntrinc.f64FocalLengthX, uIntrinc.f64Skew, uIntrinc.f64PrincipalPointX, uIntrinc.f64Padding1, uIntrinc.f64FocalLengthY, uIntrinc.f64PrincipalPointY, uIntrinc.f64Padding2, uIntrinc.f64Padding3, uIntrinc.f64Padding4);
                string strDistVal = String.Format("{0}, {1}, {2}, {3}, {4}", uDist.f64F1, uDist.f64F2, uDist.f64P1, uDist.f64P2, uDist.f64F3);

                TPoint<double> tpPosition = new TPoint<double>(0, 0);

               
                if ((eResult = layerSource.DrawTextCanvas(tpPosition, "Intrinsic Parameters: ", EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                tpPosition.y += 20;

                if ((eResult = layerSource.DrawTextCanvas(tpPosition, strMatrix, EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                tpPosition.y += 20;

                if ((eResult = layerSource.DrawTextCanvas(tpPosition, "Distortion Coefficients: ", EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                tpPosition.y += 20;

                if ((eResult = layerSource.DrawTextCanvas(tpPosition, strDistVal, EColor.YELLOW, EColor.BLACK, 13)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text");
                    break;
                }

                viewImageSource.Invalidate();

                Console.WriteLine("Intrinsic parameters : {0}", strMatrix);
                Console.WriteLine("Distortion Coefficients : {0}", strDistVal);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
                while (viewImageSource.IsAvailable() && viewImageDestination.IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

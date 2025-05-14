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
using FLImagingCLR.ThreeDim;

namespace ColorizedPointCloudGenerator3D
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
			CFLImage fliCaliSrcXYZVImage = new CFLImage();
			CFLImage fliCaliSrcRGBImage = new CFLImage();
			CFLImage fliExecSrcXYZVImage = new CFLImage();
			CFLImage fliExecSrcRGBImage = new CFLImage();
			CFLImage fliExecDstRGBImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageCaliSrcXYZV = new CGUIViewImage();
			CGUIViewImage viewImageCaliSrcRGB = new CGUIViewImage();
			CGUIViewImage viewImageExecSrcXYZV = new CGUIViewImage();
			CGUIViewImage viewImageExecSrcRGB = new CGUIViewImage();
			CGUIViewImage viewImageExecDstRGB = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// Calibration Source XYZV 이미지 로드 // Load the calibration source XYZV image
				if((res = fliCaliSrcXYZVImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/CalibXYZV.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Calibration Source XYZV 이미지 뷰 생성 // Create the calibration source XYZV image view
				if((res = viewImageCaliSrcXYZV.Create(100, 0, 400, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration Source XYZV 이미지 뷰에 이미지를 디스플레이 // Display the image in the calibration source XYZV image view
				if((res = viewImageCaliSrcXYZV.SetImagePtr(ref fliCaliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Calibration Source RGB 이미지 로드 // Load the calibration source RGB image
				if((res = fliCaliSrcRGBImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/CalibRGB.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Calibration Source RGB 이미지 뷰 생성 // Create the calibration source RGB image view
				if((res = viewImageCaliSrcRGB.Create(100, 300, 400, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration Source RGB 이미지 뷰에 이미지를 디스플레이 // Display the image in the calibration source RGB image view
				if((res = viewImageCaliSrcRGB.SetImagePtr(ref fliCaliSrcRGBImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Source XYZV 이미지 로드 // Load the Execution source XYZV image
				if((res = fliExecSrcXYZVImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/ExecXYZV.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Execution Source XYZV 이미지 뷰 생성 // Create the Execution source XYZV image view
				if((res = viewImageExecSrcXYZV.Create(400, 0, 700, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Source XYZV 이미지 뷰에 이미지를 디스플레이 // Display the image in the Execution source XYZV image view
				if((res = viewImageExecSrcXYZV.SetImagePtr(ref fliExecSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Source RGB 이미지 로드 // Load the Execution source RGB image
				if((res = fliExecSrcRGBImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/ExecRGB.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Execution Source RGB 이미지 뷰 생성 // Create the Execution source RGB image view
				if((res = viewImageExecSrcRGB.Create(400, 300, 700, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Source RGB 이미지 뷰에 이미지를 디스플레이 // Display the image in the Execution source RGB image view
				if((res = viewImageExecSrcRGB.SetImagePtr(ref fliExecSrcRGBImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Destination RGB 이미지 뷰 생성 // Create the execution destination RGB image view
				if((res = viewImageExecDstRGB.Create(700, 0, 1000, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the execution destination RGB image view
				if((res = viewImageExecDstRGB.SetImagePtr(ref fliExecDstRGBImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Destination 3D 이미지 뷰 생성 // Create the destination 3D image view
				if((res = view3DDst.Create(700, 300, 1300, 900)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}


				// 두 이미지 뷰 윈도우의 Page를 동기화 한다 // Synchronize the pages of the two image view windows
				if((res = viewImageCaliSrcXYZV.SynchronizePageIndex(ref viewImageCaliSrcRGB)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}


				// ColorizedPointCloudGenerator3D 객체 생성 // Create ColorizedPointCloudGenerator3D object
				CColorizedPointCloudGenerator3D cColorizedPointCloudGenerator = new CColorizedPointCloudGenerator3D();

				// Calibration XYZV 이미지 설정 // Set the calibration XYZV image
				if((res = cColorizedPointCloudGenerator.SetCalibrationImageXYZV(ref fliCaliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration XYZV source.\n");
					break;
				}

				// Calibration RGB 이미지 설정 // Set the calibration RGB image
				if((res = cColorizedPointCloudGenerator.SetCalibrationImageRGB(ref fliCaliSrcRGBImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration RGB source.\n");
					break;
				}

				// Calibration의 Grid Type 설정 // Set the grid type of the calibration
				if((res = cColorizedPointCloudGenerator.SetGridType(FLImagingCLR.AdvancedFunctions.CCameraCalibrator.EGridType.ChessBoard)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration grid type.\n");
					break;
				}

				// Calibration의 최적해 정확도 값 설정 // Set the optimal solution accuracy of the calibration
				if((res = cColorizedPointCloudGenerator.SetOptimalSolutionAccuracy(0.00001)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration optimal solution accuracy.\n");
					break;
				}

				// 알고리즘 Calibration 실행 // Execute calibration of the algorithm
				if((res = cColorizedPointCloudGenerator.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate algorithm.\n");
					break;
				}


				// Calibration 결과 출력 // Print calibration results
				Console.Write(" < Calibration Result >\n\n");

				// RGB 카메라의 Intrinsic Parameter 출력 // Print the intrinsic parameters of the RGB camera
				FLImagingCLR.AdvancedFunctions.CCameraCalibrator.CCalibratorIntrinsicParameters cCalibIntrinsic;

				cCalibIntrinsic = cColorizedPointCloudGenerator.GetIntrinsicParameters();

				Console.Write(" < Intrinsic Parameters >\n");

				Console.Write("Focal Length X ->\t{0}\n", cCalibIntrinsic.f64FocalLengthX);
				Console.Write("Focal Length Y ->\t{0}\n", cCalibIntrinsic.f64FocalLengthY);
				Console.Write("Principal Point X ->\t{0}\n", cCalibIntrinsic.f64PrincipalPointX);
				Console.Write("Principal Point Y ->\t{0}\n", cCalibIntrinsic.f64PrincipalPointY);
				Console.WriteLine("Skew ->\t{0}\n", cCalibIntrinsic.f64Skew);

				// RGB 카메라의 Distortion Coefficient 출력 // Print the distortion coefficients of the RGB camera
				FLImagingCLR.AdvancedFunctions.CCameraCalibrator.CCalibratorDistortionCoefficients cCalibDistortion;

				cCalibDistortion = cColorizedPointCloudGenerator.GetDistortionCoefficients();

				Console.Write(" < Distortion Coefficients >\n");

				Console.Write("K1 ->\t{0}\n", cCalibDistortion.f64K1);
				Console.Write("K2 ->\t{0}\n", cCalibDistortion.f64K2);
				Console.Write("P1 ->\t{0}\n", cCalibDistortion.f64P1);
				Console.Write("P2 ->\t{0}\n", cCalibDistortion.f64P2);
				Console.WriteLine("K3 ->\t{0}\n", cCalibDistortion.f64K3);

				// 두 카메라 간의 회전 행렬 출력 // Print the relative rotation matrix between both cameras
				CMatrix<double> cMatRotation;

				if((res = cColorizedPointCloudGenerator.GetRelativeRotation(out cMatRotation)).IsFail())
				{
					ErrorPrint(res, "Failed to get relative rotation.\n");
					break;
				}

				Console.Write(" < Relative Rotation >\n");

				Console.Write("R00 ->\t{0}\n", cMatRotation.GetValue(0, 0));
				Console.Write("R01 ->\t{0}\n", cMatRotation.GetValue(0, 1));
				Console.Write("R02 ->\t{0}\n", cMatRotation.GetValue(0, 2));
				Console.Write("R10 ->\t{0}\n", cMatRotation.GetValue(1, 0));
				Console.Write("R11 ->\t{0}\n", cMatRotation.GetValue(1, 1));
				Console.Write("R12 ->\t{0}\n", cMatRotation.GetValue(1, 2));
				Console.Write("R20 ->\t{0}\n", cMatRotation.GetValue(2, 0));
				Console.Write("R21 ->\t{0}\n", cMatRotation.GetValue(2, 1));
				Console.WriteLine("R22 ->\t{0}\n", cMatRotation.GetValue(2, 2));

				// 두 카메라 간의 변환 행렬 출력 // Print the relative translation matrix between both cameras
				CMatrix<double> cMatTranslation;

				if((res = cColorizedPointCloudGenerator.GetRelativeTranslation(out cMatTranslation)).IsFail())
				{
					ErrorPrint(res, "Failed to get relative translation.\n");
					break;
				}

				Console.Write(" < Relative Translation >\n");

				Console.Write("TX ->\t{0}\n", cMatTranslation.GetValue(0, 0));
				Console.Write("TY ->\t{0}\n", cMatTranslation.GetValue(1, 0));
				Console.WriteLine("TZ ->\t{0}\n", cMatTranslation.GetValue(2, 0));




				// 출력에 사용되는 3D 객채 생성 // Create 3D object used as output
				CFL3DObject fli3DDstObj = new CFL3DObject();

				// Execution XYZV 이미지 설정 // Set the execution XYZV image
				if((res = cColorizedPointCloudGenerator.SetSourceImageXYZV(ref fliExecSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set execution XYZV source.\n");
					break;
				}

				// Execution RGB 이미지 설정 // Set the execution RGB image
				if((res = cColorizedPointCloudGenerator.SetSourceImageRGB(ref fliExecSrcRGBImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set execution RGB source.\n");
					break;
				}

				// Destination RGB 이미지 설정 // Set the destination RGB image
				if((res = cColorizedPointCloudGenerator.SetDestinationImageRGB(ref fliExecDstRGBImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set destination RGB source.\n");
					break;
				}

				// Destination 3D Object 설정 // Set the destination 3D object
				if((res = cColorizedPointCloudGenerator.SetDestination3DObject(ref fli3DDstObj)).IsFail())
				{
					ErrorPrint(res, "Failed to set destination 3D point cloud.\n");
					break;
				}

				// 알고리즘 실행 // Execute algorithm
				if((res = cColorizedPointCloudGenerator.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute algorithm.\n");
					break;
				}


				// 결과 3D 객체 출력 // Print 3D Object
				if((res = view3DDst.PushObject(fli3DDstObj)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D object");
					break;
				}

				// 3D View 카메라 설정 // Set 3D view camera
				CFL3DCamera fl3DCam = new CFL3DCamera();

				CFLPoint3<float> flP3Dir = new CFLPoint3<float>(0, 0, 1);
				CFLPoint3<float> flP3DirUp = new CFLPoint3<float>(0, -1, 0);
				CFLPoint3<float> flP3Pos = new CFLPoint3<float>(0, 0, -1000);

				fl3DCam.SetDirection(flP3Dir);
				fl3DCam.SetDirectionUp(flP3DirUp);
				fl3DCam.SetPosition(flP3Pos);

				view3DDst.SetCamera(fl3DCam);

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다. // With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				if((res = viewImageExecDstRGB.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit of the image view.\n");
					break;
				}

				// 이미지 뷰와 3D 뷰를 갱신 합니다. // Update image views and 3D view
				viewImageCaliSrcXYZV.Invalidate(true);
				viewImageCaliSrcRGB.Invalidate(true);
				viewImageExecSrcXYZV.Invalidate(true);
				viewImageExecSrcRGB.Invalidate(true);
				viewImageExecDstRGB.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰와 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageCaliSrcXYZV.IsAvailable() && viewImageCaliSrcRGB.IsAvailable() && viewImageExecSrcXYZV.IsAvailable() && viewImageExecSrcRGB.IsAvailable() && viewImageExecDstRGB.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

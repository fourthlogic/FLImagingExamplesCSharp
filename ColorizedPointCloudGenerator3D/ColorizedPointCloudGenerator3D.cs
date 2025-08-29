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

namespace FLImagingExamplesCSharp
{
	class ColorizedPointCloudGenerator3D
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
			CFLImage fliCaliSrcXYZVImage = new CFLImage();
			CFLImage fliCaliSrcColorImage = new CFLImage();
			CFLImage fliExecSrcXYZVImage = new CFLImage();
			CFLImage fliExecSrcColorImage = new CFLImage();
			CFLImage fliExecDstColorImage = new CFLImage();
			CFLImage fliSampDstColorImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageCaliSrcXYZV = new CGUIViewImage();
			CGUIViewImage viewImageCaliSrcColor = new CGUIViewImage();
			CGUIViewImage viewImageExecSrcXYZV = new CGUIViewImage();
			CGUIViewImage viewImageExecSrcColor = new CGUIViewImage();
			CGUIViewImage viewImageExecDstColor = new CGUIViewImage();
			CGUIViewImage viewImageSampDstColor = new CGUIViewImage();
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


				// Calibration Source Color 이미지 로드 // Load the calibration source Color image
				if((res = fliCaliSrcColorImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/CalibRGB.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Calibration Source Color 이미지 뷰 생성 // Create the calibration source Color image view
				if((res = viewImageCaliSrcColor.Create(100, 300, 400, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration Source Color 이미지 뷰에 이미지를 디스플레이 // Display the image in the calibration source Color image view
				if((res = viewImageCaliSrcColor.SetImagePtr(ref fliCaliSrcColorImage)).IsFail())
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


				// Execution Source Color 이미지 로드 // Load the Execution source Color image
				if((res = fliExecSrcColorImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/ExecRGB.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Execution Source Color 이미지 뷰 생성 // Create the Execution source Color image view
				if((res = viewImageExecSrcColor.Create(400, 300, 700, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Source Color 이미지 뷰에 이미지를 디스플레이 // Display the image in the Execution source Color image view
				if((res = viewImageExecSrcColor.SetImagePtr(ref fliExecSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Destination Color 이미지 뷰 생성 // Create the execution destination Color image view
				if((res = viewImageExecDstColor.Create(700, 0, 1000, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the execution destination Color image view
				if((res = viewImageExecDstColor.SetImagePtr(ref fliExecDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Sampled Color 이미지 뷰 생성 // Create the execution destination Color image view
				if((res = viewImageSampDstColor.Create(700, 300, 1000, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the execution destination Color image view
				if((res = viewImageSampDstColor.SetImagePtr(ref fliSampDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}



				// Destination 3D 이미지 뷰 생성 // Create the destination 3D image view
				if((res = view3DDst.Create(1000, 0, 1600, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}


				// 두 이미지 뷰 윈도우의 Page를 동기화 한다 // Synchronize the pages of the two image view windows
				if((res = viewImageCaliSrcXYZV.SynchronizePageIndex(ref viewImageCaliSrcColor)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}


				// ColorizedPointCloudGenerator3D 객체 생성 // Create ColorizedPointCloudGenerator3D object
				CColorizedPointCloudGenerator3D colorizedPointCloudGenerator3D = new CColorizedPointCloudGenerator3D();

				// Calibration XYZV 이미지 설정 // Set the calibration XYZV image
				if((res = colorizedPointCloudGenerator3D.SetCalibrationXYZVImage(ref fliCaliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration XYZV source.\n");
					break;
				}

				// Calibration Color 이미지 설정 // Set the calibration Color image
				if((res = colorizedPointCloudGenerator3D.SetCalibrationColorImage(ref fliCaliSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration Color source.\n");
					break;
				}

				// Calibration의 Grid Type 설정 // Set the grid type of the calibration
				if((res = colorizedPointCloudGenerator3D.SetGridType(FLImagingCLR.AdvancedFunctions.CCameraCalibrator.EGridType.ChessBoard)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration grid type.\n");
					break;
				}

				// Calibration의 최적해 정확도 값 설정 // Set the optimal solution accuracy of the calibration
				if((res = colorizedPointCloudGenerator3D.SetOptimalSolutionAccuracy(0.00001)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration optimal solution accuracy.\n");
					break;
				}

				// Coordinate Adjustment 자동 설정 // Coordinate Adjustment Auto Set Flag
				if((res = colorizedPointCloudGenerator3D.EnableAutoCoordinateAdjustment(true)).IsFail())
				{
					ErrorPrint(res, "Failed to set Coordinate Adjustment Flag.\n");
					break;
				}

				// 알고리즘 Calibration 실행 // Execute calibration of the algorithm
				if((res = colorizedPointCloudGenerator3D.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate algorithm.\n");
					break;
				}


				// Calibration 결과 출력 // Print calibration results
				Console.Write(" < Calibration Result >\n\n");

				// Color 카메라의 Intrinsic Parameter 출력 // Print the intrinsic parameters of the color camera
				FLImagingCLR.AdvancedFunctions.CCameraCalibrator.CCalibratorIntrinsicParameters cCalibIntrinsic;

				cCalibIntrinsic = colorizedPointCloudGenerator3D.GetIntrinsicParameters();

				Console.Write(" < Intrinsic Parameters >\n");

				Console.Write("Focal Length X ->\t{0}\n", cCalibIntrinsic.f64FocalLengthX);
				Console.Write("Focal Length Y ->\t{0}\n", cCalibIntrinsic.f64FocalLengthY);
				Console.Write("Principal Point X ->\t{0}\n", cCalibIntrinsic.f64PrincipalPointX);
				Console.Write("Principal Point Y ->\t{0}\n", cCalibIntrinsic.f64PrincipalPointY);
				Console.WriteLine("Skew ->\t{0}\n", cCalibIntrinsic.f64Skew);

				// Color 카메라의 Distortion Coefficient 출력 // Print the distortion coefficients of the color camera
				FLImagingCLR.AdvancedFunctions.CCameraCalibrator.CCalibratorDistortionCoefficients cCalibDistortion;

				cCalibDistortion = colorizedPointCloudGenerator3D.GetDistortionCoefficients();

				Console.Write(" < Distortion Coefficients >\n");

				Console.Write("K1 ->\t{0}\n", cCalibDistortion.f64K1);
				Console.Write("K2 ->\t{0}\n", cCalibDistortion.f64K2);
				Console.Write("P1 ->\t{0}\n", cCalibDistortion.f64P1);
				Console.Write("P2 ->\t{0}\n", cCalibDistortion.f64P2);
				Console.WriteLine("K3 ->\t{0}\n", cCalibDistortion.f64K3);

				// 두 카메라 간의 회전 행렬 출력 // Print the relative rotation matrix between both cameras
				CMatrix<double> cMatRotation = new CMatrix<double>();

				if((res = colorizedPointCloudGenerator3D.GetRelativeRotation(ref cMatRotation)).IsFail())
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
				CMatrix<double> cMatTranslation = new CMatrix<double>();

				if((res = colorizedPointCloudGenerator3D.GetRelativeTranslation(ref cMatTranslation)).IsFail())
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
				if((res = colorizedPointCloudGenerator3D.SetSourceXYZVImage(ref fliExecSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set execution XYZV source.\n");
					break;
				}

				// Execution Color 이미지 설정 // Set the execution Color image
				if((res = colorizedPointCloudGenerator3D.SetSourceColorImage(ref fliExecSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set execution Color source.\n");
					break;
				}

				// Destination Color 이미지 설정 // Set the destination Color image
				if((res = colorizedPointCloudGenerator3D.SetDestinationColorImage(ref fliExecDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set destination Color source.\n");
					break;
				}

				// Destination Sampled Color 이미지 설정 // Set the destination sampled Color image
				if((res = colorizedPointCloudGenerator3D.SetDestinationSampledColorImage(ref fliSampDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set destination sampled Color source.\n");
					break;
				}

				// Sampled 픽셀 표시 BGR 설정 // Set the color of the sampled pixels in BGR
				if((res = colorizedPointCloudGenerator3D.SetSampledBGRValue(255, 255, 0)).IsFail())
				{
					ErrorPrint(res, "Failed to set sampled Color value.\n");
					break;
				}

				// Destination 3D Object 설정 // Set the destination 3D object
				if((res = colorizedPointCloudGenerator3D.SetDestination3DObject(ref fli3DDstObj)).IsFail())
				{
					ErrorPrint(res, "Failed to set destination 3D point cloud.\n");
					break;
				}

				// 알고리즘 실행 // Execute algorithm
				if((res = colorizedPointCloudGenerator3D.Execute()).IsFail())
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

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layerImageCaliSrcXYZV = viewImageCaliSrcXYZV.GetLayer(0);
				CGUIViewImageLayer layerImageCaliSrcColor = viewImageCaliSrcColor.GetLayer(0);
				CGUIViewImageLayer layerImageExecSrcXYZV = viewImageExecSrcXYZV.GetLayer(0);
				CGUIViewImageLayer layerImageExecSrcColor = viewImageExecSrcColor.GetLayer(0);
				CGUIViewImageLayer layerImageExecDstColor = viewImageExecDstColor.GetLayer(0);
				CGUIViewImageLayer layerImageSampDstColor = viewImageSampDstColor.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layerImageCaliSrcXYZV.Clear();
				layerImageCaliSrcColor.Clear();
				layerImageExecSrcXYZV.Clear();
				layerImageExecSrcColor.Clear();
				layerImageExecDstColor.Clear();
				layerImageSampDstColor.Clear();
				layer3DDst.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas 는 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((res = layerImageCaliSrcXYZV.DrawTextCanvas(new CFLPoint<double>(0, 0), "Calibration Source XYZV Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerImageCaliSrcColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Calibration Source Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerImageExecSrcXYZV.DrawTextCanvas(new CFLPoint<double>(0, 0), "Execution Source XYZV Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerImageExecSrcColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Execution Source Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerImageExecDstColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Execution Destination Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerImageSampDstColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Execution Sampled Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layer3DDst.DrawTextCanvas(new CFLPoint<double>(0, 0), "3D Colored Point Cloud", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}


				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다. // With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				if((res = viewImageExecDstColor.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit of the image view.\n");
					break;
				}

				if((res = viewImageSampDstColor.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit of the image view.\n");
					break;
				}

				// 이미지 뷰와 3D 뷰를 갱신 합니다. // Update image views and 3D view
				viewImageCaliSrcXYZV.Invalidate(true);
				viewImageCaliSrcColor.Invalidate(true);
				viewImageExecSrcXYZV.Invalidate(true);
				viewImageExecSrcColor.Invalidate(true);
				viewImageExecDstColor.Invalidate(true);
				viewImageSampDstColor.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰와 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageCaliSrcXYZV.IsAvailable() && viewImageCaliSrcColor.IsAvailable() && viewImageExecSrcXYZV.IsAvailable() && viewImageExecSrcColor.IsAvailable() && viewImageExecDstColor.IsAvailable() && viewImageSampDstColor.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

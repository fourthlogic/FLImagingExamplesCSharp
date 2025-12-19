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

			// 이미지 객체 선언 // Declare image object
			CFLImage fliCaliSrcXYZVImage = new CFLImage();
			CFLImage fliCaliSrcColorImage = new CFLImage();
			CFLImage fliExecSrcXYZVImage = new CFLImage();
			CFLImage fliExecSrcColorImage = new CFLImage();
			CFLImage fliExecDstColorImage = new CFLImage();
			CFLImage fliSampDstColorImage = new CFLImage();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewCaliSrcXYZVImage = new CGUIViewImage();
			CGUIViewImage viewCaliSrcColorImage = new CGUIViewImage();
			CGUIViewImage viewExecSrcXYZVImage = new CGUIViewImage();
			CGUIViewImage viewExecSrcColorImage = new CGUIViewImage();
			CGUIViewImage viewExecDstColorImage = new CGUIViewImage();
			CGUIViewImage viewSampDstColorImage = new CGUIViewImage();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DDst = new CGUIView3D();

			do
			{
				// 수행 결과 객체 선언 // Declare execution result object
				CResult res = new CResult(EResult.UnknownError);

				// Calibration Source XYZV 이미지 로드 // Load Calibration Source XYZV image
				if((res = fliCaliSrcXYZVImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/CalibXYZV.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Calibration Source XYZV 이미지 뷰 생성 // Create Calibration Source XYZV image view
				if((res = viewCaliSrcXYZVImage.Create(100, 0, 400, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration Source XYZV 이미지 뷰에 이미지를 디스플레이 // Display image in Calibration Source XYZV image view
				if((res = viewCaliSrcXYZVImage.SetImagePtr(ref fliCaliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Calibration Source Color 이미지 로드 // Load Calibration Source Color image
				if((res = fliCaliSrcColorImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/CalibRGB.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Calibration Source Color 이미지 뷰 생성 // Create Calibration Source Color image view
				if((res = viewCaliSrcColorImage.Create(100, 300, 400, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration Source Color 이미지 뷰에 이미지를 디스플레이 // Display image in Calibration Source Color image view
				if((res = viewCaliSrcColorImage.SetImagePtr(ref fliCaliSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Source XYZV 이미지 로드 // Load Execution Source XYZV image
				if((res = fliExecSrcXYZVImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/ExecXYZV.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Execution Source XYZV 이미지 뷰 생성 // Create Execution Source XYZV image view
				if((res = viewExecSrcXYZVImage.Create(400, 0, 700, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Source XYZV 이미지 뷰에 이미지를 디스플레이 // Display image in Execution Source XYZV image view
				if((res = viewExecSrcXYZVImage.SetImagePtr(ref fliExecSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Source Color 이미지 로드 // Load Execution Source Color image
				if((res = fliExecSrcColorImage.Load("../../ExampleImages/ColorizedPointCloudGenerator3D/ExecRGB.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Execution Source Color 이미지 뷰 생성 // Create Execution Source Color image view
				if((res = viewExecSrcColorImage.Create(400, 300, 700, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Source Color 이미지 뷰에 이미지를 디스플레이 // Display image in Execution Source Color image view
				if((res = viewExecSrcColorImage.SetImagePtr(ref fliExecSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Destination Color 이미지 뷰 생성 // Create Execution Destination Color image view
				if((res = viewExecDstColorImage.Create(700, 0, 1000, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Destination Color 이미지 뷰에 이미지를 디스플레이 // Display image in Execution Destination Color image view
				if((res = viewExecDstColorImage.SetImagePtr(ref fliExecDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Execution Sampled Destination 이미지 뷰 생성 // Create Execution Sampled Destination image view
				if((res = viewSampDstColorImage.Create(700, 300, 1000, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Execution Sampled Destination 이미지 뷰에 이미지를 디스플레이 // Display image in Execution Sampled Destination image view
				if((res = viewSampDstColorImage.SetImagePtr(ref fliSampDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// Destination 3D 뷰 생성 // Create Destination 3D view
				if((res = view3DDst.Create(1000, 0, 1600, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}


				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewCaliSrcXYZVImage.SynchronizeWindow(ref viewCaliSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewCaliSrcXYZVImage.SynchronizeWindow(ref viewExecSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewCaliSrcXYZVImage.SynchronizeWindow(ref viewExecSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewCaliSrcXYZVImage.SynchronizeWindow(ref viewExecDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewCaliSrcXYZVImage.SynchronizeWindow(ref viewSampDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewCaliSrcXYZVImage.SynchronizeWindow(ref view3DDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}


				// 두 이미지 뷰 윈도우의 Page를 동기화 한다 // Synchronize pages of two image views
				if((res = viewCaliSrcXYZVImage.SynchronizePageIndex(ref viewCaliSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index between image views.\n");
					break;
				}


				// Colorized Point Cloud Generator 3D 객체 생성 // Create Colorized Point Cloud Generator 3D object
				CColorizedPointCloudGenerator3D colorizedPointCloudGenerator3D = new CColorizedPointCloudGenerator3D();

				// Calibration Source XYZV 이미지 설정 // Set Calibration Source XYZV image
				if((res = colorizedPointCloudGenerator3D.SetCalibrationXYZVImage(ref fliCaliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Calibration Source XYZV image.\n");
					break;
				}

				// Calibration Source Color 이미지 설정 // Set Calibration Source Color image
				if((res = colorizedPointCloudGenerator3D.SetCalibrationColorImage(ref fliCaliSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Calibration Source Color image.\n");
					break;
				}

				// Calibration에 사용되는 Grid Type 설정 // Set grid type used in calibration
				if((res = colorizedPointCloudGenerator3D.SetGridType(FLImagingCLR.AdvancedFunctions.CCameraCalibrator.EGridType.ChessBoard)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration grid type.\n");
					break;
				}

				// Calibration의 최적해 정확도 값 설정 // Set optimal solution accuracy of calibration
				if((res = colorizedPointCloudGenerator3D.SetOptimalSolutionAccuracy(0.00001)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration optimal solution accuracy.\n");
					break;
				}

				// 자동 Coordinate Adjustment 사용 여부 설정 // Set auto coordinate adjustment flag
				if((res = colorizedPointCloudGenerator3D.EnableAutoCoordinateAdjustment(true)).IsFail())
				{
					ErrorPrint(res, "Failed to set coordinate adjustment flag.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 Calibration 수행 // Calibration algorithm according to previously set parameters
				if((res = colorizedPointCloudGenerator3D.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate Colorized Point Cloud Generator 3D.\n");
					break;
				}


				// Calibration 결과 출력 // Print calibration results
				Console.Write(" < Calibration Result >\n\n");

				// Color 카메라의 Intrinsic Parameter 출력 // Print intrinsic parameters of color camera
				FLImagingCLR.AdvancedFunctions.CCameraCalibrator.CCalibratorIntrinsicParameters calibIntrinsic;

				calibIntrinsic = colorizedPointCloudGenerator3D.GetResultIntrinsicParameters();

				Console.Write(" < Intrinsic Parameters >\n");

				Console.Write("Focal Length X ->\t{0:N7}\n", calibIntrinsic.f64FocalLengthX);
				Console.Write("Focal Length Y ->\t{0:N7}\n", calibIntrinsic.f64FocalLengthY);
				Console.Write("Principal Point X ->\t{0:N7}\n", calibIntrinsic.f64PrincipalPointX);
				Console.Write("Principal Point Y ->\t{0:N7}\n", calibIntrinsic.f64PrincipalPointY);
				Console.Write("Skew ->\t{0:N7}\n", calibIntrinsic.f64Skew);

				Console.Write("\n");

				// Color 카메라의 Distortion Coefficient 출력 // Print distortion coefficients of color camera
				FLImagingCLR.AdvancedFunctions.CCameraCalibrator.CCalibratorDistortionCoefficients calibDistortion;

				calibDistortion = colorizedPointCloudGenerator3D.GetResultDistortionCoefficients();

				Console.Write(" < Distortion Coefficients >\n");

				Console.Write("K1 ->\t{0:N7}\n", calibDistortion.f64K1);
				Console.Write("K2 ->\t{0:N7}\n", calibDistortion.f64K2);
				Console.Write("P1 ->\t{0:N7}\n", calibDistortion.f64P1);
				Console.Write("P2 ->\t{0:N7}\n", calibDistortion.f64P2);
				Console.Write("K3 ->\t{0:N7}\n", calibDistortion.f64K3);

				Console.Write("\n");

				// 두 카메라 간의 회전 행렬 출력 // Print relative rotation matrix between both cameras
				CMatrix<double> matRotation = new CMatrix<double>();

				if((res = colorizedPointCloudGenerator3D.GetResultRelativeRotation(ref matRotation)).IsFail())
				{
					ErrorPrint(res, "Failed to get relative rotation.\n");
					break;
				}

				Console.Write(" < Relative Rotation >\n");

				Console.Write("R00 ->\t{0:N7}\n", matRotation.GetValue(0, 0));
				Console.Write("R01 ->\t{0:N7}\n", matRotation.GetValue(0, 1));
				Console.Write("R02 ->\t{0:N7}\n", matRotation.GetValue(0, 2));
				Console.Write("R10 ->\t{0:N7}\n", matRotation.GetValue(1, 0));
				Console.Write("R11 ->\t{0:N7}\n", matRotation.GetValue(1, 1));
				Console.Write("R12 ->\t{0:N7}\n", matRotation.GetValue(1, 2));
				Console.Write("R20 ->\t{0:N7}\n", matRotation.GetValue(2, 0));
				Console.Write("R21 ->\t{0:N7}\n", matRotation.GetValue(2, 1));
				Console.Write("R22 ->\t{0:N7}\n", matRotation.GetValue(2, 2));

				Console.Write("\n");

				// 두 카메라 간의 변환 행렬 출력 // Print relative translation matrix between both cameras
				CMatrix<double> matTranslation = new CMatrix<double>();

				if((res = colorizedPointCloudGenerator3D.GetResultRelativeTranslation(ref matTranslation)).IsFail())
				{
					ErrorPrint(res, "Failed to get relative translation.\n");
					break;
				}

				Console.Write(" < Relative Translation >\n");

				Console.Write("TX ->\t{0:N7}\n", matTranslation.GetValue(0, 0));
				Console.Write("TY ->\t{0:N7}\n", matTranslation.GetValue(1, 0));
				Console.Write("TZ ->\t{0:N7}\n", matTranslation.GetValue(2, 0));

				Console.Write("\n");


				// 출력에 사용되는 3D 객채 생성 // Create 3D object used as output
				CFL3DObject fl3DDstObj = new CFL3DObject();

				// Execution Source XYZV 이미지 설정 // Set Execution Source XYZV image
				if((res = colorizedPointCloudGenerator3D.SetSourceXYZVImage(ref fliExecSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Execution Source XYZV image.\n");
					break;
				}

				// Execution Source Color 이미지 설정 // Set Execution Source Color image
				if((res = colorizedPointCloudGenerator3D.SetSourceColorImage(ref fliExecSrcColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Execution Source Color image.\n");
					break;
				}

				// Execution Destination Color 이미지 설정 // Set Execution Destination Color image
				if((res = colorizedPointCloudGenerator3D.SetDestinationColorImage(ref fliExecDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Execution Destination Color image.\n");
					break;
				}

				// Execution Destination Sampled Color 이미지 설정 // Set Execution Destination Sampled Color image
				if((res = colorizedPointCloudGenerator3D.SetDestinationSampledColorImage(ref fliSampDstColorImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Execution Destination Sampled Color image.\n");
					break;
				}

				// Sampled 픽셀 표시 Color 설정 // Set color of the sampled pixels in BGR
				if((res = colorizedPointCloudGenerator3D.SetSampledBGRValue(255, 255, 0)).IsFail())
				{
					ErrorPrint(res, "Failed to set sampled pixel BGR value.\n");
					break;
				}

				// Execution Destination 3D Object 설정 // Set Execution Destination 3D Object
				if((res = colorizedPointCloudGenerator3D.SetDestination3DObject(ref fl3DDstObj)).IsFail())
				{
					ErrorPrint(res, "Failed to set Execution Destination 3D Object.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = colorizedPointCloudGenerator3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Colorized Point Cloud Generator 3D.\n");
					break;
				}


				// 결과 3D 객체 출력 // Print resulting 3D Object
				if((res = view3DDst.PushObject(fl3DDstObj)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D Object.\n");
					break;
				}

				// 3D View 카메라 설정 // Set 3D view camera
				CFL3DCamera fl3DCam = new CFL3DCamera();

				fl3DCam.SetDirection(new CFLPoint3<float>(0, 0, 1));
				fl3DCam.SetDirectionUp(new CFLPoint3<float>(0, -1, 0));
				fl3DCam.SetPosition(new CFLPoint3<float>(0, 0, -1000));

				view3DDst.SetCamera(fl3DCam);

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerImageCaliSrcXYZV = viewCaliSrcXYZVImage.GetLayer(0);
				CGUIViewImageLayer layerImageCaliSrcColor = viewCaliSrcColorImage.GetLayer(0);
				CGUIViewImageLayer layerImageExecSrcXYZV = viewExecSrcXYZVImage.GetLayer(0);
				CGUIViewImageLayer layerImageExecSrcColor = viewExecSrcColorImage.GetLayer(0);
				CGUIViewImageLayer layerImageExecDstColor = viewExecDstColorImage.GetLayer(0);
				CGUIViewImageLayer layerImageSampDstColor = viewSampDstColorImage.GetLayer(0);

				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an 3D view and does not need to be released
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear figures drawn on existing layer
				layerImageCaliSrcXYZV.Clear();
				layerImageCaliSrcColor.Clear();
				layerImageExecSrcXYZV.Clear();
				layerImageExecSrcColor.Clear();
				layerImageExecDstColor.Clear();
				layerImageSampDstColor.Clear();
				layer3DDst.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerImageCaliSrcXYZV.DrawTextCanvas(new CFLPoint<double>(0, 0), "Calibration Source XYZV Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerImageCaliSrcColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Calibration Source Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerImageExecSrcXYZV.DrawTextCanvas(new CFLPoint<double>(0, 0), "Execution Source XYZV Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerImageExecSrcColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Execution Source Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerImageExecDstColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerImageSampDstColor.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Sampled Color Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 3D 뷰 정보 표시 // Display 3D view information
				if((res = layer3DDst.DrawTextCanvas(new CFLPoint<double>(0, 0), "3D Destination Colored Point Cloud", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}


				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewExecDstColorImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewSampDstColorImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewCaliSrcXYZVImage.Invalidate(true);
				viewCaliSrcColorImage.Invalidate(true);
				viewExecSrcXYZVImage.Invalidate(true);
				viewExecSrcColorImage.Invalidate(true);
				viewExecDstColorImage.Invalidate(true);
				viewSampDstColorImage.Invalidate(true);

				// 3D 뷰를 갱신 // Update 3D view
				view3DDst.Invalidate(true);


				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewCaliSrcXYZVImage.IsAvailable() && viewCaliSrcColorImage.IsAvailable() && viewExecSrcXYZVImage.IsAvailable() && viewExecSrcColorImage.IsAvailable() && viewExecDstColorImage.IsAvailable() && viewSampDstColorImage.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

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
	class PhotometricStereo3D_Calibration
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
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliCalibrationImage = new CFLImage();
			CFLImage fliDestinationImage = new CFLImage();
			CFLImage fliCurvatureImage = new CFLImage();
			CFLImage fliTextureImage = new CFLImage();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewSourceImage = new CGUIViewImage();
			CGUIViewImage viewCalibrationImage = new CGUIViewImage();
			CGUIViewImage viewTextureImage = new CGUIViewImage();
			CGUIViewImage viewCurvatureImage = new CGUIViewImage();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DDst = new CGUIView3D();

			do
			{
				// 수행 결과 객체 선언 // Declare execution result object
				CResult res = new CResult(EResult.UnknownError);

				// Source 이미지 로드 // Load Source image
				if((res = fliSourceImage.Load("../../ExampleImages/PhotometricStereo3D/Source.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create Source image view
				if((res = viewSourceImage.Create(100, 0, 498, 398)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display image in Source image view
				if((res = viewSourceImage.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Calibration 이미지 로드 // Load Calibration image
				if((res = fliCalibrationImage.Load("../../ExampleImages/PhotometricStereo3D/Calibrate.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Calibration 이미지 뷰 생성 // Create Calibration image view
				if((res = viewCalibrationImage.Create(498, 0, 896, 398)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration 이미지 뷰에 이미지를 디스플레이 // Display image in Calibration image view
				if((res = viewCalibrationImage.SetImagePtr(ref fliCalibrationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Texture 이미지 뷰 생성 // Create Texture image view
				if((res = viewTextureImage.Create(100, 398, 498, 796)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Texture 이미지 뷰에 이미지를 디스플레이 // Display image in Texture image view
				if((res = viewTextureImage.SetImagePtr(ref fliTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Curvature 이미지 뷰 생성 // Create Curvature image view
				if((res = viewCurvatureImage.Create(498, 398, 896, 796)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Curvature 이미지 뷰에 이미지를 디스플레이 // Display image in Curvature image view
				if((res = viewCurvatureImage.SetImagePtr(ref fliCurvatureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 3D 뷰 생성 // Create Destination 3D view
				if((res = view3DDst.Create(896, 0, 1692, 796)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 // Synchronize viewpoints of two image views
				if((res = viewSourceImage.SynchronizePointOfView(ref viewTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize point of view between image views.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 // Synchronize viewpoints of two image views
				if((res = viewSourceImage.SynchronizePointOfView(ref viewCurvatureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize point of view between image views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref viewCalibrationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref viewCurvatureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref viewTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref view3DDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// Photometric Stereo 3D 객체 생성 // Create Photometric Stereo 3D object
				CPhotometricStereo3D photometricStereo3D = new CPhotometricStereo3D();

				// 출력에 사용되는 3D Height Map 객채 생성 // Create 3D height map used as output
				CFL3DObject fl3DOHM = new CFL3DObjectHeightMap();

				// Source 이미지 설정 // Set Source image
				if((res = photometricStereo3D.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source image.\n");
					break;
				}

				// Calibration 이미지 설정 // Set Calibration image
				if((res = photometricStereo3D.SetCalibrationImage(ref fliCalibrationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Calibration image.\n");
					break;
				}

				// Destination Height Map 이미지 설정 // Set Destination Height Map image
				if((res = photometricStereo3D.SetDestinationHeightMapImage(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination Height Map image.\n");
					break;
				}

				// Destination Texture 이미지 설정 // Set Destination Texture image
				if((res = photometricStereo3D.SetDestinationTextureImage(ref fliTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination Texture image.\n");
					break;
				}

				// Destination Curvature 이미지 설정 // Set Destination Curvature image
				if((res = photometricStereo3D.SetCurvatureImage(ref fliCurvatureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination Curvature image.\n");
					break;
				}

				// Destination 3D Object 설정 // Set Destination 3D Object 
				if((res = photometricStereo3D.SetDestinationObject(ref fl3DOHM)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination 3D Object.\n");
					break;
				}

				// Calibration Circle ROI 설정 // Set calibration circle ROI settings
				if((res = photometricStereo3D.SetCalibrationCircleROI(new CFLCircle<double>(117.210526, 104.842105, 78.736842, 0.000000, 0.000000, 360.000000, EArcClosingMethod.EachOther))).IsFail())
				{
					ErrorPrint(res, "Failed to set Calibration Circle ROI.\n");
					break;
				}

				// 동작 방식 설정 // Set reconstruction mode
				if((res = photometricStereo3D.SetReconstructionMode(CPhotometricStereo3D.EReconstructionMode.Poisson_FP32)).IsFail())
				{
					ErrorPrint(res, "Failed to set reconstruction mode.\n");
					break;
				}

				// Valid 픽셀의 기준 설정 // Set valid pixel ratio
				if((res = photometricStereo3D.SetValidPixelThreshold(0.25)).IsFail())
				{
					ErrorPrint(res, "Failed to set valid pixel threshold.\n");
					break;
				}

				// Curvature 이미지 Normalization 여부 설정 // Set curvature image normalization option
				if((res = photometricStereo3D.EnableCurvatureNormalization(true)).IsFail())
				{
					ErrorPrint(res, "Failed to set curvature normalization flag.\n");
					break;
				}

				// Angle Degrees 동작 방식으로 설정 // Set operation method as angle degrees
				if((res = photometricStereo3D.SetCalibrationMode(CPhotometricStereo3D.ECalibrationMode.Angle_Degrees)).IsFail())
				{
					ErrorPrint(res, "Failed to set light angle in degrees.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 Calibration 수행 // Calibration algorithm according to previously set parameters
				if((res = photometricStereo3D.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate Photometric Stereo 3D.\n");
					break;
				}

				// Calibrate 된 Angle Degree 데이터 // Calibrated angle degree data
				CMultiVar<double> mvdSlant = new CMultiVar<double>();
				CMultiVar<double> mvdTilt = new CMultiVar<double>();

				// Calibrate 된 Angle Degree 데이터 저장 // Save calibrated angle degree data
				if((res = photometricStereo3D.GetLightAngleDegrees(ref mvdSlant, ref mvdTilt)).IsFail())
				{
					ErrorPrint(res, "Failed to get light angle in degrees.\n");
					break;
				}

				// 위치 데이터 동작 방식으로 설정 // Set operation method as positions
				if((res = photometricStereo3D.SetCalibrationMode(CPhotometricStereo3D.ECalibrationMode.Positions)).IsFail())
				{
					ErrorPrint(res, "Failed to set light positions.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 Calibration 수행 // Calibration algorithm according to previously set parameters
				if((res = photometricStereo3D.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate Photometric Stereo 3D.\n");
					break;
				}

				// Calibrate 된 위치 데이터 // Calibrated position data
				CMatrix<double> matPosition = new CMatrix<double>();

				// Calibrate 된 위치 데이터 저장 // Save calibrated position data
				if((res = photometricStereo3D.GetLightPositions(ref matPosition)).IsFail())
				{
					ErrorPrint(res, "Failed to get light positions.\n");
					break;
				}

				// Calibrate를 실행한 결과를 Console창에 출력 // Output calibration result to console window
				int i32CalibPageNum = fliCalibrationImage.GetPageCount();

				// Angle Degrees 데이터 출력 // Print angle degrees data
				Console.Write(" < Calibration Angle - Degrees >\n");

				for(int i = 0; i < i32CalibPageNum; i++)
					Console.Write("Image {0} ->\tSlant: {1:N7}\tTilt: {2:N7}\n", i, mvdSlant.GetAt(i), mvdTilt.GetAt(i));

				Console.Write("\n");

				// Positions 데이터 출력 // Print positions data
				Console.Write(" < Calibration Positions >\n");

				for(int i = 0; i < i32CalibPageNum; i++)
					Console.Write("Image {0} ->\tX: {1:N7}\tY: {2:N7} \tZ: {3:N7}\n", i, matPosition.GetValue(i, 0), matPosition.GetValue(i, 1), matPosition.GetValue(i, 2));

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = photometricStereo3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Photometric Stereo 3D.\n");
					break;
				}

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerSource = viewSourceImage.GetLayer(0);
				CGUIViewImageLayer layerCalibration = viewCalibrationImage.GetLayer(0);
				CGUIViewImageLayer layerCurvature = viewCurvatureImage.GetLayer(0);
				CGUIViewImageLayer layerTexture = viewTextureImage.GetLayer(0);

				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an 3D view and does not need to be released
				CGUIView3DLayer layer3DDestination = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear figures drawn on existing layer
				layerSource.Clear();
				layerCalibration.Clear();
				layerCurvature.Clear();
				layerTexture.Clear();
				layer3DDestination.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerSource.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerCalibration.DrawTextCanvas(new CFLPoint<double>(0, 0), "Calibration Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerTexture.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Texture Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerCurvature.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Curvature Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 3D View 정보 디스플레이 // Display 3D view information
				float f32CenterX = (float)fliSourceImage.GetWidth() / 2;
				float f32CenterY = (float)fliSourceImage.GetHeight() / 2;
				float f32CenterZ = (float)fliDestinationImage.GetBuffer()[(long)(f32CenterY * fliSourceImage.GetWidth() + f32CenterX)];

				TPoint3<float> tp3dFrom = new TPoint3<float>(f32CenterX, f32CenterY, f32CenterZ);

				float f32MulNum = 800;

				for(long i = 0; i < i32CalibPageNum; i++)
				{
					string strText;

					strText = String.Format("X: {0:N4}lf    \nY: {1:N4}lf    \nZ: {2:N4}lf\n", matPosition.GetValue(i, 0), matPosition.GetValue(i, 1), matPosition.GetValue(i, 2));

					TPoint3<float> tp3dTo = new TPoint3<float>(f32MulNum * (float)matPosition.GetValue(i, 0) + f32CenterX, f32MulNum * (float)matPosition.GetValue(i, 1) + f32CenterY, f32MulNum * (float)matPosition.GetValue(i, 2) + f32CenterZ);

					TPoint3<double> tp3dTod = new TPoint3<double>(f32MulNum * matPosition.GetValue(i, 0) + f32CenterX, f32MulNum * matPosition.GetValue(i, 1) + f32CenterY, f32MulNum * matPosition.GetValue(i, 2) + f32CenterZ);

					CGUIView3DObjectLine cgui3dlineTemp = new CGUIView3DObjectLine(tp3dFrom, tp3dTo, EColor.YELLOW, 1);

					layer3DDestination.DrawText3D(tp3dTod, strText, EColor.BLACK, EColor.YELLOW);
					view3DDst.PushObject(cgui3dlineTemp);
				}

				// 3D Height Map에 Texture 적용 // Apply texture to 3D height map
				if((res = ((CFL3DObjectHeightMap)fl3DOHM).SetTextureImage(fliTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to apply texture to height map.\n");
					break;
				}

				res = ((CFL3DObjectHeightMap)fl3DOHM).ActivateVertexColorTexture(true);

				// 결과 3D 객체 출력 // Print resulting 3D Object
				if((res = view3DDst.PushObject(fl3DOHM)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D Object.\n");
					break;
				}

				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewTextureImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewCurvatureImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 새로 생성한 3D Object를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created 3D object
				if((res = view3DDst.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit 3D view.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewSourceImage.Invalidate(true);
				viewTextureImage.Invalidate(true);
				viewCalibrationImage.Invalidate(true);
				viewCurvatureImage.Invalidate(true);

				// 3D 뷰를 갱신 // Update 3D view
				view3DDst.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewSourceImage.IsAvailable() && viewTextureImage.IsAvailable() && viewCalibrationImage.IsAvailable() && viewCurvatureImage.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

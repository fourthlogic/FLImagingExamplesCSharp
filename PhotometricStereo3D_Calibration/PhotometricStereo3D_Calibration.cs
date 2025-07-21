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

namespace StereoPhotometric
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
			// 이미지 객체 선언 // Declare the image object
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliCalImage = new CFLImage();
			CFLImage fliTxtImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageCal = new CGUIViewImage();
			CGUIViewImage viewImageTxt = new CGUIViewImage();
			CGUIView3D viewImage3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// Source 이미지 로드 // Load source image
				if((res = fliSrcImage.Load("../../ExampleImages/PhotometricStereo3D/Source.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// Source 이미지 뷰 생성 // Create the source image view
				if((res = viewImageSrc.Create(100, 0, 498, 398)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the source image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Calibration 이미지 로드 // Load calibration image
				if((res = fliCalImage.Load("../../ExampleImages/PhotometricStereo3D/Calibrate.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliCalImage.SelectPage(0);

				// Calibration 이미지 뷰 생성 // Create the calibration image view
				if((res = viewImageCal.Create(498, 0, 896, 398)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Calibration 이미지 뷰에 이미지를 디스플레이 // Display the calibration image in an image view
				if((res = viewImageCal.SetImagePtr(ref fliCalImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((res = viewImageTxt.Create(100, 398, 498, 796)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((res = viewImageTxt.SetImagePtr(ref fliTxtImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 3D 뷰 생성
				if((res = viewImage3DDst.Create(896, 0, 1692, 769)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageTxt)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageTxt)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageCal)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
				if((res = viewImageSrc.SynchronizeWindow(ref viewImage3DDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				viewImageSrc.SetFixThumbnailView(true);

				// PhotometricStereo 객체 생성 // Create PhotometricStereo object
				CPhotometricStereo3D photometric = new CPhotometricStereo3D();

				CFL3DObject fl3DOHM = new CFL3DObjectHeightMap();

				// Source 이미지 설정 // Set the source image
				photometric.SetSourceImage(ref fliSrcImage);
				// Calibration 이미지 설정 // Set the calibration image
				photometric.SetCalibrationImage(ref fliCalImage);
				// Destination Height Map 이미지 설정 // Set the destination height map image
				photometric.SetDestinationHeightMapImage(ref fliDstImage);
				// Destination 객체 설정 // Set the destination object
				photometric.SetDestinationObject(ref fl3DOHM);
				// Destination Texture 이미지 설정 // Set the destination texture image
				photometric.SetDestinationTextureImage(ref fliTxtImage);
				// 동작 방식 설정 // Set Operation Mode
				photometric.SetReconstructionMode(CPhotometricStereo3D.EReconstructionMode.Poisson_FP32);
				// Calibration 데이터 설정 // Set Calibration Settings
				photometric.SetCalibrationCircleROI(new CFLCircle<double>(386.439657, 346.491239, 259.998140, 0.000000, 0.000000, 360.000000, EArcClosingMethod.EachOther));
				// Valid 픽셀의 기준 설정 // Set valid pixel ratio
				photometric.SetValidPixelThreshold(0.125);

				CMatrix<double> cmatdTemp = new CMatrix<double>(3, 3);

				// Angle Degrees 동작 방식으로 설정 // Set operation method as angle degrees
				photometric.SetLightAngleDegrees(cmatdTemp);

				// 알고리즘 Calibration 실행 // Execute calibration of the algorithm
				if((res = photometric.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate algorithm.\n");
					break;
				}

				// Calibrate 된 Angle Degree 데이터 저장 // Save calibrated angle degree data
				CMultiVar<double> cmvdSlant = new CMultiVar<double>();
				CMultiVar<double> cmvdTilt = new CMultiVar<double>();

				photometric.GetLightAngleDegrees(ref cmvdSlant, ref cmvdTilt);

				// 위치 데이터 동작 방식으로 설정 // Set operation method as positions
				photometric.SetLightPositions(cmatdTemp);

				// 알고리즘 Calibration 실행 // Execute calibration of the algorithm
				if((res = photometric.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate algorithm.\n");
					break;
				}

				// Calibrate 된 위치 데이터 저장 // Save calibrated position data
				CMatrix<double> cmatdPosition = new CMatrix<double>();

				photometric.GetLightPositions(ref cmatdPosition);

				// Calibrate를 실행한 결과를 Console창에 출력합니다. // Output the calibration result to the console window.
				int i32CalibPageNum = fliCalImage.GetPageCount();

				// Angle Degrees 데이터 출력
				Console.WriteLine(" < Calibration Angle - Degrees >");

				for(int i = 0; i < i32CalibPageNum; i++)
					Console.WriteLine("Image {0} ->\tSlant: {1:#,0.0000000}\tTilt: {2:#,0.0000000}", i, cmvdSlant.GetAt(i), cmvdTilt.GetAt(i));

				Console.WriteLine("\n");

				// Positions 데이터 출력
				Console.WriteLine(" < Calibration Positions >");

				for(int i = 0; i < i32CalibPageNum; i++)
					Console.WriteLine("Image {0} ->\tX: {1:#,0.0000000}\tY: {2:#,0.0000000} \tZ: {3:#,0.0000000}", i, cmatdPosition.GetValue(i, 0), cmatdPosition.GetValue(i, 1), cmatdPosition.GetValue(i, 2));

				// 알고리즘 실행 // Execute algorithm
				if((res = photometric.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate algorithm.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = photometric.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute algorithm.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageTxt.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerCal = viewImageCal.GetLayer(0);
				CGUIViewImageLayer layerTxt = viewImageTxt.GetLayer(0);
				CGUIView3DLayer layer3D = viewImage3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerCal.Clear();
				layerTxt.Clear();
				layer3D.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerCal.DrawTextCanvas(flp, ("Calibration Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerTxt.DrawTextCanvas(flp, ("Destination Texture Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 3D View 정보 디스필레이 // Display 3D view information

				float f32CenterX = (float)fliSrcImage.GetWidth() / 2;
				float f32CenterY = (float)fliSrcImage.GetHeight() / 2;
				float f32CenterZ = (float)fliDstImage.GetBuffer()[(long)(f32CenterY * fliSrcImage.GetWidth() + f32CenterX)];

				TPoint3<float> tp3dFrom = new TPoint3<float>(f32CenterX, f32CenterY, f32CenterZ);

				float f32MulNum = 2000;

				for(long i = 0; i < i32CalibPageNum; i++)
				{
					string strText = "";

					strText += String.Format("X: {0:#,0.0000}    \nY: {1:#,0.0000}    \nZ: {2:#,0.0000}\n", cmatdPosition.GetValue(i, 0), cmatdPosition.GetValue(i, 1), cmatdPosition.GetValue(i, 2));

					TPoint3<float> tp3dTo = new TPoint3<float>(f32MulNum * (float)cmatdPosition.GetValue(i, 0) + f32CenterX, f32MulNum * (float)cmatdPosition.GetValue(i, 1) + f32CenterY, f32MulNum * (float)cmatdPosition.GetValue(i, 2) + f32CenterZ);

					TPoint3<double> tp3dTod = new TPoint3<double>(f32MulNum * cmatdPosition.GetValue(i, 0) + f32CenterX, f32MulNum * cmatdPosition.GetValue(i, 1) + f32CenterY, f32MulNum * cmatdPosition.GetValue(i, 2) + f32CenterZ);

					CGUIView3DObjectLine cgui3dlineTemp = new CGUIView3DObjectLine(tp3dFrom, tp3dTo, EColor.YELLOW, 1);

					layer3D.DrawText3D(tp3dTod, strText, EColor.BLACK, EColor.YELLOW);
					viewImage3DDst.PushObject(cgui3dlineTemp);
				}

				CFL3DObjectHeightMap fl3DObject = photometric.GetDestinationObject() as CFL3DObjectHeightMap;
				fl3DObject.SetTextureImage(fliTxtImage);
				fl3DObject.ActivateVertexColorTexture(false);

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if(viewImage3DDst.PushObject(fl3DObject).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				viewImage3DDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageCal.Invalidate(true);
				viewImageTxt.Invalidate(true);
				viewImage3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageSrc.IsAvailable() && viewImageCal.IsAvailable() && viewImageTxt.IsAvailable() && viewImage3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

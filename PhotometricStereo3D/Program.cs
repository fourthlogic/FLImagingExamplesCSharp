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

namespace PhotometricStereo
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();
			CFLImage fliTxtImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D viewImage3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/PhotometricStereo3D/Source.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// 이미지 뷰 생성 // Create an image view
				if((res = viewImageSrc.Create(100, 0, 548, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((res = viewImageDst.Create(100, 448, 548, 896)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 3D 뷰 생성
				if((res = viewImage3DDst.Create(548, 448, 996, 896)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
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
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImage3DDst)).IsFail())
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
				// Destination Height Map 이미지 설정 // Set the destination height map image
				photometric.SetDestinationHeightMapImage(ref fliDstImage);
				// Destination 객체 설정 // Set the destination object
				photometric.SetDestinationObject(ref fl3DOHM);
				// Destination Texture 이미지 설정 // Set the destination texture image
				photometric.SetDestinationTextureImage(ref fliTxtImage);
				// 동작 방식 설정 // Set Operation Mode
				photometric.SetReconstructionMode(CPhotometricStereo3D.EReconstructionMode.Poisson_FP32);
				// Valid 픽셀의 기준 설정 // Set valid pixel ratio
				photometric.SetValidPixelThreshold(0.125);


				// 각 이미지의 광원 Slant 값 입력
				CMultiVar<double> mvdSlant = new CMultiVar<double>();

				mvdSlant.PushBack(39.831506);
				mvdSlant.PushBack(28.682381);
				mvdSlant.PushBack(20.989625);
				mvdSlant.PushBack(19.346638);
				mvdSlant.PushBack(20.785800);
				mvdSlant.PushBack(26.005273);
				mvdSlant.PushBack(19.038004);
				mvdSlant.PushBack(9.253585);
				mvdSlant.PushBack(16.425454);
				mvdSlant.PushBack(23.712574);
				mvdSlant.PushBack(26.003058);
				mvdSlant.PushBack(19.069500);
				mvdSlant.PushBack(11.801071);
				mvdSlant.PushBack(20.484473);
				mvdSlant.PushBack(25.909730);
				mvdSlant.PushBack(43.055332);
				mvdSlant.PushBack(39.043981);
				mvdSlant.PushBack(30.041029);
				mvdSlant.PushBack(26.067657);
				mvdSlant.PushBack(26.126303);

				// 각 이미지의 광원 Tilt 값 입력
				CMultiVar<double> mvdTilt = new CMultiVar<double>();

				mvdTilt.PushBack(123.359091);
				mvdTilt.PushBack(123.952892);
				mvdTilt.PushBack(154.836215);
				mvdTilt.PushBack(-173.353324);
				mvdTilt.PushBack(-147.483507);
				mvdTilt.PushBack(109.497340);
				mvdTilt.PushBack(115.825606);
				mvdTilt.PushBack(-169.019112);
				mvdTilt.PushBack(-119.343654);
				mvdTilt.PushBack(-109.319167);
				mvdTilt.PushBack(66.944279);
				mvdTilt.PushBack(48.136896);
				mvdTilt.PushBack(-5.157068);
				mvdTilt.PushBack(-54.033519);
				mvdTilt.PushBack(-66.856636);
				mvdTilt.PushBack(60.456870);
				mvdTilt.PushBack(53.388008);
				mvdTilt.PushBack(36.447691);
				mvdTilt.PushBack(13.056294);
				mvdTilt.PushBack(-5.976723);

				photometric.SetLightAngleDegrees(mvdSlant, mvdTilt);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = photometric.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute algorithm.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageDst.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

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

				if((res = layerDst.DrawTextCanvas(flp, ("Destination Height Map Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				CFL3DObjectHeightMap fl3DObject = photometric.GetDestinationObject() as CFL3DObjectHeightMap;
				fl3DObject.SetTextureImage(fliTxtImage);
				fl3DObject.ActivateVertexColorTexture(false);

				// 3D 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if(viewImage3DDst.PushObject(fl3DObject).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				viewImage3DDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);
				viewImage3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable() && viewImage3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

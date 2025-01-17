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

namespace StereoDisparity3D
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
			CFLImage fliSrcImage2 = new CFLImage();
			CFLImage fliDstImage = new CFLImage();
			CFLImage fliTxtImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageSrc2 = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D viewImage3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/StereoDisparity3D/Left.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 로드 // Load image
				if((res = fliSrcImage2.Load("../../ExampleImages/StereoDisparity3D/Right.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);
				fliSrcImage2.SelectPage(0);

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

				// 이미지 뷰 2 생성 // Create the image view 2
				if((res = viewImageSrc2.Create(548, 0, 996, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 2에 이미지를 디스플레이 // Display the image to the image view 2
				if((res = viewImageSrc2.SetImagePtr(ref fliSrcImage2)).IsFail())
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

				// 3D 이미지 뷰 생성
				if((res = viewImage3DDst.Create(548, 448, 996, 896)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageSrc2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
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

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageSrc2.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

                // 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImageSrc2)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view. \n");
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
				viewImageSrc2.SetFixThumbnailView(true);

				// StereoDisparity 객체 생성 // Create StereoDisparity object
				CStereoDisparity3D disparity = new CStereoDisparity3D();

				CFL3DObject fl3DOHM = new CFL3DObjectHeightMap();

				// Source 이미지 설정 // Set the source image
				disparity.SetSourceImage(ref fliSrcImage);
				// Source 이미지 설정 // Set the source image
				disparity.SetSourceImage2(ref fliSrcImage2);
				// Destination Height Map 이미지 설정 // Set the destination height map image
				disparity.SetDestinationHeightMapImage(ref fliDstImage);
				// Destination 객체 설정 // Set the destination object
				disparity.SetDestinationObject(ref fl3DOHM);
				// Destination Texture 이미지 설정 // Set the destination texture image
				disparity.SetDestinationTextureImage(ref fliTxtImage);
				// 최소 허용 Disparity 값 설정 // Set the minimum allowed disparity value
				disparity.SetMinimumDisparity(-20);
				// Disparity 범위 설정 // Set the range of disparity
				disparity.SetMaximumDisparity(0);
				// Matched Block 크기 설정 // Set the matched block size
				disparity.SetMatchBlockSize(5);
				// 좌우 간 최대 허용 차이 값 설정 // Set maximum allowed difference value between left and right
				disparity.SetMaximumDifference(30);
				// 고유비 값 설정 // Set the uniqueness ratio value
				disparity.SetUniquenessRatio(0.0);
				// P1 값 설정 // Set P1 Value
				disparity.SetP1(300);
				// P2 값 설정 // Set P2 Value
				disparity.SetP2(2000);
				// Median Morphology 커널 사이즈 설정 // Set the median morphology kernel size
				disparity.SetMedianKernelSize(5);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = disparity.Execute()).IsFail())
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
				CGUIViewImageLayer layerSrc2 = viewImageSrc2.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerSrc2.Clear();
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

				if((res = layerSrc2.DrawTextCanvas(flp, ("Source Image 2"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				CFL3DObjectHeightMap fl3DObject = disparity.GetDestinationObject() as CFL3DObjectHeightMap;
				fl3DObject.SetTextureImage(fliTxtImage);
				fl3DObject.ActivateVertexColorTexture(true);

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if(viewImage3DDst.PushObject(fl3DObject).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				viewImage3DDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageSrc2.Invalidate(true);
				viewImageDst.Invalidate(true);
				viewImage3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageSrc.IsAvailable() && viewImageSrc2.IsAvailable() && viewImageDst.IsAvailable() && viewImage3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

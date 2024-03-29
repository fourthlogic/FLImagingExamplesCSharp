﻿using System;
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

namespace StereoDisparity
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
			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((eResult = fliSrcImage.Load("../../ExampleImages/StereoDisparity/Left.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 로드 // Load image
				if((eResult = fliSrcImage2.Load("../../ExampleImages/StereoDisparity/Right.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);
				fliSrcImage2.SelectPage(0);

				// 이미지 뷰 생성 // Create an image view
				if((eResult = viewImageSrc.Create(100, 0, 548, 448)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((eResult = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰 2 생성 // Create the image view 2
				if((eResult = viewImageSrc2.Create(548, 0, 996, 448)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 2에 이미지를 디스플레이 // Display the image to the image view 2
				if((eResult = viewImageSrc2.SetImagePtr(ref fliSrcImage2)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((eResult = viewImageDst.Create(100, 448, 548, 896)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((eResult = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 3D 이미지 뷰 생성
				if((eResult = viewImage3DDst.Create(548, 448, 996, 896)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((eResult = viewImageSrc.SynchronizePointOfView(ref viewImageSrc2)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view. \n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((eResult = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view. \n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((eResult = viewImageSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((eResult = viewImageSrc2.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit\n");
					break;
				}

                // 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
                if ((eResult = viewImageSrc.SynchronizeWindow(ref viewImageSrc2)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view. \n");
                    break;
                }

                // 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
                if ((eResult = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view. \n");
                    break;
                }

                // 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
                if ((eResult = viewImageSrc.SynchronizeWindow(ref viewImage3DDst)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view. \n");
                    break;
                }

				viewImageSrc.SetFixThumbnailView(true);
				viewImageSrc2.SetFixThumbnailView(true);

				// StereoDisparity 객체 생성 // Create StereoDisparity object
				CStereoDisparity disparity = new CStereoDisparity();

				// Source 이미지 설정 // Set the source image
				disparity.SetSourceImage(ref fliSrcImage);
				// Source 이미지 설정 // Set the source image
				disparity.SetSourceImage2(ref fliSrcImage2);
				// Destination 이미지 설정 // Set the destination image
				disparity.SetDestinationImage(ref fliDstImage);
				// 결과 Texture 이미지 설정 // Set the result texture image
				disparity.SetResultTextureImage(ref fliTxtImage);
				// 최소 허용 Disparity 값 설정 // Set the minimum allowed disparity value
				disparity.SetMinimumDisparity(0);
				// Disparity 범위 설정 // Set the range of disparity
				disparity.SetNumberOfDisparities(15);
				// Matched Block 크기 설정 // Set the matched block size
				disparity.SetMatchBlockSize(7);
				// 좌우 간 최대 허용 차이 값 설정 // Set maximum allowed difference value between left and right
				disparity.SetMaximumDifference(10);
				// 고유비 값 설정 // Set the uniqueness ratio value
				disparity.SetUniquenessRatio(1);
				// P1 값 설정 // Set P1 Value
				disparity.SetP1(1000);
				// P2 값 설정 // Set P2 Value
				disparity.SetP2(4000);
				// Median Morphology 커널 사이즈 설정 // Set the median morphology kernel size
				disparity.SetMedianKernelSize(3);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = disparity.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute algorithm.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((eResult = viewImageDst.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit.\n");
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

				if((eResult = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerSrc2.DrawTextCanvas(flp, ("Source Image 2"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if((eResult = viewImage3DDst.SetHeightMap(fliDstImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 3D 이미지 뷰에 Texture 이미지를 디스플레이 // Display the Texture image on the 3D image view
				if((eResult = viewImage3DDst.SetTexture(fliTxtImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

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

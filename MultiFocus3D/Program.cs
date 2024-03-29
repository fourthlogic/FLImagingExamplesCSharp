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

namespace MultiFocus
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
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((eResult = fliSrcImage.Load("../../ExampleImages/MultiFocus3D/")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// 이미지 뷰 생성 // Create image view
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

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((eResult = viewImageDst.Create(548, 0, 996, 448)).IsFail())
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

				// Destination 3D 이미지 뷰 생성 // Create the destination 3D image view
				if((eResult = view3DDst.Create(400, 200, 1300, 800)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
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

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
				if((eResult = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view. \n");
					break;
				}

				viewImageSrc.SetFixThumbnailView(true);

				// MultiFocus 객체 생성 // Create MultiFocus object
				CMultiFocus3D multiFocus3d = new CMultiFocus3D();

				// Source 이미지 설정 // Set the source image
				multiFocus3d.SetSourceImage(ref fliSrcImage);
				// Destination 이미지 설정 // Set the destination image
				multiFocus3d.SetDestinationImage(ref fliDstImage);
				// 결과 Texture 이미지 설정 // Set the result texture image
				multiFocus3d.SetResultTextureImage(ref fliTxtImage);
				// Pixel Accuracy 설정 // Set the pixel accuracy
				multiFocus3d.SetPixelAccuracy(0.1);
				// Depth Pitch 설정 // Set the depth pitch
				multiFocus3d.SetDepthPitch(0.2);
				// Filter 설정 // Set filter
				multiFocus3d.SetFilter(CMultiFocus3D.EFilter.Guided);
				// Set the sigma range value
				multiFocus3d.SetSigmaSpatial(5);
				// Set the sigma spatial value
				multiFocus3d.SetSigmaRange(5);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = multiFocus3d.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute MultiFocus.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((eResult = viewImageDst.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);
				CGUIView3DLayer layer3D = view3DDst.GetLayer(0);

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

				if((eResult = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
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
				if((eResult = view3DDst.SetHeightMap(fliDstImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 3D 이미지 뷰에 Texture 이미지를 디스플레이 // Display the texture image on the 3D image view
				if((eResult = view3DDst.SetTexture(fliTxtImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				if((eResult = layer3D.DrawTextCanvas(flp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				view3DDst.SetFillType(EFillType3D.WireFrame);

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

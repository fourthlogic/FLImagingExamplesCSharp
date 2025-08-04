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
	class DepthImageReconstructionMAPBased3D
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();
			CFLImage fliTxtImage = new CFLImage();
			CFL3DObject floDstObject = new CFL3DObjectHeightMap();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/DepthImageReconstructionMAPBased3D/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 500)).IsFail() ||
					(res = view3DDst.Create(400, 200, 1300, 800)).IsFail())
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

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				viewImageSrc.SetFixThumbnailView(true);


				// 알고리즘 객체 생성 // Create algorithm object
				CDepthImageReconstructionMAPBased3D algObject = new CDepthImageReconstructionMAPBased3D();

				if((res = algObject.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				if((res = algObject.SetDestinationHeightMapImage(ref fliDstImage)).IsFail())
					break;
				if((res = algObject.SetDestinationTextureImage(ref fliTxtImage)).IsFail())
					break;

				if((res = algObject.SetFMBiasPageIndex(3)).IsFail())
					break;
				if((res = algObject.SetFMBiasValue(0.02)).IsFail())
					break;
				if((res = algObject.SetFocusMeasureMethod(CDepthImageReconstructionMAPBased3D.EFocusMeasureMethod.DoG)).IsFail())
					break;
				if((res = algObject.SetSigma1(0.4)).IsFail())
					break;
				if((res = algObject.SetSigma2(0.8)).IsFail())
					break;

				if((res = algObject.SetLocalRegularizationFactor(0.02)).IsFail())
					break;
				if((res = algObject.SetGlobalRegularizationFactor(0.00000000001)).IsFail())
					break;
				if((res = algObject.SetCGMTolerance(0.00001)).IsFail())
					break;
				if((res = algObject.SetCGMMaxIterations(100)).IsFail())
					break;

				if((res = algObject.SetDirection(CDepthImageReconstructionMAPBased3D.EDirection.BottomToTop)).IsFail())
					break;
				if((res = algObject.SetPixelAccuracy(1.0)).IsFail())
					break;
				if((res = algObject.SetDepthPitch(2.0)).IsFail())
					break;

				if((res = algObject.Enable3DObjectGeneration(true)).IsFail())
					break;
				if((res = algObject.SetDestinationObject(ref floDstObject)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}


				// ((CFL3DObjectHeightMap)floDstObject).SetTextureImage(fliTxtImage);
				// ((CFL3DObjectHeightMap)floDstObject).ActivateVertexColorTexture(true);

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if(view3DDst.PushObject(floDstObject).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
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
				layer3D.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flp = new CFLPoint<double>();
				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layer3D.DrawTextCanvas(flp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				viewImageSrc.SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);

				// Zoom Fit
				viewImageSrc.ZoomFit();
				viewImageDst.ZoomFit();
				view3DDst.ZoomFit();

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

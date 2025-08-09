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
	class Moire3D
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
			CFLImage[] fliLrnImage = new CFLImage[2];
			fliLrnImage[0] = new CFLImage();
			fliLrnImage[1] = new CFLImage();
			CFLImage[] fliSrcImage = new CFLImage[2];
			fliSrcImage[0] = new CFLImage();
			fliSrcImage[1] = new CFLImage();
			CFLImage fliDstImage = new CFLImage();
			CFLImage fliTxtImage = new CFLImage();
			CFL3DObject floDstObject = new CFL3DObjectHeightMap();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImageLrn = new CGUIViewImage[2];
			viewImageLrn[0] = new CGUIViewImage();
			viewImageLrn[1] = new CGUIViewImage();
			CGUIViewImage[] viewImageSrc = new CGUIViewImage[2];
			viewImageSrc[0] = new CGUIViewImage();
			viewImageSrc[1] = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			// Source 이미지를 저장할 Array 선언 // Declare an Array to store the source image
			List<CFLImage> vctLrnImages = new List<CFLImage>();

			// Source 이미지 입력 // source images add
			vctLrnImages.Add(fliLrnImage[0]);
			vctLrnImages.Add(fliLrnImage[1]);

			// Source 이미지를 저장할 Array 선언 // Declare an Array to store the source image
			List<CFLImage> vctSrcImages = new List<CFLImage>();

			// Source 이미지 입력 // source images add
			vctSrcImages.Add(fliSrcImage[0]);
			vctSrcImages.Add(fliSrcImage[1]);

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res;

			do
			{
				// Learn 이미지 로드 // Load the reference plane image for calibration
				if((res = vctLrnImages[0].Load("../../ExampleImages/Moire3D/Learn0/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 로드 // Load the source image
				if((res = vctSrcImages[0].Load("../../ExampleImages/Moire3D/Object0/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Learn 이미지 로드 // Load the reference plane image for calibration
				if((res = vctLrnImages[1].Load("../../ExampleImages/Moire3D/Learn1/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 로드 // Load the source image
				if((res = vctSrcImages[1].Load("../../ExampleImages/Moire3D/Object1/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Texture 이미지 로드 // Load the texture image
				if((res = fliTxtImage.Load("../../ExampleImages/Moire3D/text.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create the image view
				if((res = viewImageLrn[0].Create(100, 0, 600, 400)).IsFail() ||
					(res = viewImageLrn[1].Create(600, 0, 1100, 400)).IsFail() ||
					(res = viewImageSrc[0].Create(100, 400, 600, 800)).IsFail() ||
					(res = viewImageSrc[1].Create(600, 400, 1100, 800)).IsFail() ||
					(res = viewImageDst.Create(200, 200, 700, 600)).IsFail() ||
					(res = view3DDst.Create(400, 100, 1150, 550)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc[0].SynchronizePointOfView(ref viewImageSrc[1])).IsFail() ||
					(res = viewImageSrc[0].SynchronizePointOfView(ref viewImageLrn[0])).IsFail() ||
					(res = viewImageSrc[0].SynchronizePointOfView(ref viewImageLrn[1])).IsFail() ||
					(res = viewImageSrc[0].SynchronizePointOfView(ref viewImageDst)).IsFail())
				{

					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰의 페이지 인덱스를 동기화 한다 // Synchronize the page index of the two image views
				if((res = viewImageSrc[0].SynchronizePageIndex(ref viewImageSrc[1])).IsFail() ||
					(res = viewImageSrc[0].SynchronizePageIndex(ref viewImageLrn[0])).IsFail() ||
					(res = viewImageSrc[0].SynchronizePageIndex(ref viewImageLrn[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSrc[0].SynchronizeWindow(ref viewImageSrc[1])).IsFail() ||
					(res = viewImageSrc[0].SynchronizeWindow(ref viewImageLrn[0])).IsFail() ||
					(res = viewImageSrc[0].SynchronizeWindow(ref viewImageLrn[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageLrn[0].SetImagePtr(ref fliLrnImage[0])).IsFail() ||
					(res = viewImageLrn[1].SetImagePtr(ref fliLrnImage[1])).IsFail() ||
					(res = viewImageSrc[0].SetImagePtr(ref fliSrcImage[0])).IsFail() ||
					(res = viewImageSrc[1].SetImagePtr(ref fliSrcImage[1])).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// 알고리즘 객체 생성 // Create algorithm object
				CMoire3D moire3D = new CMoire3D();

				if((res = moire3D.SetWorkingDistance(330)).IsFail())
					break;
				if((res = moire3D.SetFieldOfView(400)).IsFail())
					break;
				CMultiVar<double> mvF64AngleOfProjector = new CMultiVar<double>(73, 105);
				if((res = moire3D.SetAngleOfProjector(mvF64AngleOfProjector)).IsFail())
					break;
				CMultiVar<double> mvF64SmallBinRange = new CMultiVar<double>(1, 1);
				if((res = moire3D.SetBinInterval(mvF64SmallBinRange)).IsFail())
					break;
				if((res = moire3D.SetPatternType(CMoire3D.EPatternType.SquareWave)).IsFail())
					break;
				if((res = moire3D.EnableNoiseReduction(true)).IsFail())
					break;

				if((res = moire3D.SetLearnImage(ref vctLrnImages)).IsFail())
					break;

				// 알고리즘 Calibrate // Calibrate the algorithm
				if((res = moire3D.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate the algorithm.");
					break;
				}

				if((res = moire3D.SetSourceImage(ref vctSrcImages)).IsFail())
					break;
				if((res = moire3D.SetDestinationHeightMapImage(ref fliDstImage)).IsFail())
					break;
				if((res = moire3D.SetDestinationObject(ref floDstObject)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = moire3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}


				((CFL3DObjectHeightMap)floDstObject).SetTextureImage(fliTxtImage);
				((CFL3DObjectHeightMap)floDstObject).ActivateVertexColorTexture(true);

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if(view3DDst.PushObject(floDstObject).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLrn0 = viewImageLrn[0].GetLayer(0);
				CGUIViewImageLayer layerLrn1 = viewImageLrn[1].GetLayer(0);
				CGUIViewImageLayer layerSrc0 = viewImageSrc[0].GetLayer(0);
				CGUIViewImageLayer layerSrc1 = viewImageSrc[1].GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);
				CGUIView3DLayer layer3D = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLrn0.Clear();
				layerLrn1.Clear();
				layerSrc0.Clear();
				layerSrc1.Clear();
				layerDst.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flp = new CFLPoint<double>();
				if((res = layerLrn0.DrawTextCanvas(flp, "Learn Image[0]", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerLrn1.DrawTextCanvas(flp, "Learn Image[1]", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerSrc0.DrawTextCanvas(flp, "Source Image[0]", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerSrc1.DrawTextCanvas(flp, "Source Image[1]", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layer3D.DrawTextCanvas(flp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				viewImageLrn[0].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageLrn[1].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageSrc[0].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageSrc[1].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);

				// Zoom Fit
				viewImageLrn[0].ZoomFit();
				viewImageLrn[1].ZoomFit();
				viewImageSrc[0].ZoomFit();
				viewImageSrc[1].ZoomFit();
				viewImageDst.ZoomFit();
				view3DDst.ZoomFit();

				// 이미지 뷰를 갱신 // Update image view
				viewImageLrn[0].Invalidate(true);
				viewImageLrn[1].Invalidate(true);
				viewImageSrc[0].Invalidate(true);
				viewImageSrc[1].Invalidate(true);
				viewImageDst.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageLrn[0].IsAvailable() && viewImageLrn[0].IsAvailable() && viewImageSrc[0].IsAvailable() && viewImageSrc[1].IsAvailable() && viewImageDst.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

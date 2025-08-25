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
	class StationaryConeBeamTranslationCT
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
			CFLImage fliDstSinoImage = new CFLImage();
			CFL3DObject floDestination = new CFL3DObject();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIViewImage viewImageDstSino = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/StationaryConeBeamTranslationCT/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 500)).IsFail() ||
					(res = viewImageDstSino.Create(100, 500, 600, 1000)).IsFail() ||
					(res = view3DDst.Create(600, 500, 1100, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail() ||
					(res = viewImageDstSino.SetImagePtr(ref fliDstSinoImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				viewImageSrc.SetFixThumbnailView(true);


				// 알고리즘 객체 생성 // Create algorithm object
				CStationaryConeBeamTranslationCT stationaryConeBeamTranslationCT = new CStationaryConeBeamTranslationCT();

				if((res = stationaryConeBeamTranslationCT.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetDestinationImage(ref fliDstImage)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetDestinationSinogramImage(ref fliDstSinoImage)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetDestinationObject(ref floDestination)).IsFail())
					break;

				if((res = stationaryConeBeamTranslationCT.SetDestinationSinogramIndex(15)).IsFail())
					break;

				if((res = stationaryConeBeamTranslationCT.SetDetectorCellSizeUnit(0.16708)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetObjectTranslateDirection(CStationaryConeBeamTranslationCT.EObjectTranslateDirection.RightToLeft)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetSourceObjectDistanceUnit(13.60)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetSourceDetectorDistanceUnit(60.00)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetObjectTranslationDistanceUnit(24.00)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetPrincipalDeltaXPixel(0.00)).IsFail())
					break;

				if((res = stationaryConeBeamTranslationCT.SetNormalizedAirThreshold(0.60)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetSinogramKeepRatio(0.30)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetInterpolationCoefficient(6)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetMergeCoefficient(21)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.EnableFrequencyRampFilter(true)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetFrequencyWindow(CStationaryConeBeamTranslationCT.EFrequencyWindow.Gaussian)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetSigma(0.30)).IsFail())
					break;

				if((res = stationaryConeBeamTranslationCT.SetReconstructionPlaneCount(140)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.EnableNegativeClip(true)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.EnableCircularMask(true)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.EnableSigmoid(true)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetSigmoidB(1.00)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetSigmoidM(0.00)).IsFail())
					break;
				if((res = stationaryConeBeamTranslationCT.SetIntensityThreshold(190)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = stationaryConeBeamTranslationCT.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}


				// 3D 이미지 뷰에 Destination Object 를 디스플레이
				if((res = view3DDst.PushObject(floDestination)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);
				CGUIViewImageLayer layerDstSino = viewImageDstSino.GetLayer(0);
				CGUIView3DLayer layer3D = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();
				layerDstSino.Clear();
				layer3D.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flp = new CFLPoint<double>();
				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDstSino.DrawTextCanvas(flp, ("Destination Sinogram Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layer3D.DrawTextCanvas(flp, "Destination Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				viewImageSrc.SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageDst.SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);

				// Zoom Fit
				viewImageSrc.ZoomFit();
				viewImageDst.ZoomFit();
				viewImageDstSino.ZoomFit();
				view3DDst.ZoomFit();

				// 3D 뷰 카메라 수정 // Modify 3D view camera
				CGUIView3DCamera cameraNew = new CGUIView3DCamera();
				cameraNew.Assign(view3DDst.GetCamera());
				CFLPoint3<float> flpPositionOld = cameraNew.GetPosition();
				flpPositionOld.y = flpPositionOld.z;
				cameraNew.SetPosition(flpPositionOld, false);
				view3DDst.SetCamera(cameraNew);

				// 이미지 뷰 갱신 // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);
				viewImageDstSino.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable() && viewImageDstSino.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

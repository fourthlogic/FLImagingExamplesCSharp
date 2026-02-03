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
	class StationaryConeBeamGeneralizedCT
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
			CFL3DObject floDestination = new CFL3DObject();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/StationaryConeBeamGeneralizedCT/p360 240x145.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 500)).IsFail() ||
					(res = view3DDst.Create(600, 500, 1100, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
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
				CStationaryConeBeamGeneralizedCT stationaryConeBeamGeneralizedCT = new CStationaryConeBeamGeneralizedCT();

				if((res = stationaryConeBeamGeneralizedCT.LoadCSV("../../ExampleImages/StationaryConeBeamGeneralizedCT/geometry.csv")).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetDestinationImage(ref fliDstImage)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetDestinationObject(ref floDestination)).IsFail())
					break;

				if((res = stationaryConeBeamGeneralizedCT.SetAngleUnit(EAngleUnit.Degree)).IsFail())
					break;

				TPoint3<double> tpObjectVoxelSize = new TPoint3<double>();
				tpObjectVoxelSize.x = 0.02;
				tpObjectVoxelSize.y = 0.02;
				tpObjectVoxelSize.z = 0.02;
				if((res = stationaryConeBeamGeneralizedCT.SetObjectVoxelSize(tpObjectVoxelSize)).IsFail())
					break;
				TPoint3<Int32> tpObjectVoxelCount = new TPoint3<Int32>();
				tpObjectVoxelCount.x = 150;
				tpObjectVoxelCount.y = 150;
				tpObjectVoxelCount.z = 150;
				if((res = stationaryConeBeamGeneralizedCT.SetObjectVoxelCount(tpObjectVoxelCount)).IsFail())
					break;
				TPoint3<Int32> tpObjectVoxelSubdivisionCount = new TPoint3<Int32>();
				tpObjectVoxelSubdivisionCount.x = 1;
				tpObjectVoxelSubdivisionCount.y = 1;
				tpObjectVoxelSubdivisionCount.z = 1;
				if((res = stationaryConeBeamGeneralizedCT.SetObjectVoxelSubdivisionCount(tpObjectVoxelSubdivisionCount)).IsFail())
					break;
				TPoint3<Int32> tpObjectVoxelOffset = new TPoint3<Int32>();
				tpObjectVoxelOffset.x = 0;
				tpObjectVoxelOffset.y = 0;
				tpObjectVoxelOffset.z = 0;
				if((res = stationaryConeBeamGeneralizedCT.SetObjectVoxelOffset(tpObjectVoxelOffset)).IsFail())
					break;

				if((res = stationaryConeBeamGeneralizedCT.EnableFrequencyRampFilter(true)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetFrequencyWindow(CStationaryConeBeamGeneralizedCT.EFrequencyWindow.Gaussian)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetSigma(0.50)).IsFail())
					break;

				if((res = stationaryConeBeamGeneralizedCT.SetOutputFormat(CStationaryConeBeamGeneralizedCT.EOutputFormat.U8)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetSigmoidB(1000.00)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetSigmoidM(0.00)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetIntensityThreshold(200)).IsFail())
					break;
				if((res = stationaryConeBeamGeneralizedCT.SetSlicingPlane(CStationaryConeBeamGeneralizedCT.ESlicingPlane.Coronal)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = stationaryConeBeamGeneralizedCT.Execute()).IsFail())
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
				CGUIView3DLayer layer3D = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();
				layer3D.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flp = new CFLPoint<double>();
				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
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
				view3DDst.ZoomFit();

				// 이미지 뷰 갱신 // Update image view
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

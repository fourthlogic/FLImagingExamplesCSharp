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
using CResult = FLImagingCLR.CResult;

namespace WignerVilleDistribution
{
	class WignerVilleDistribution
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
			CFLImage fliISrcImage = new CFLImage();
			CFLImage fliIDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/WignerVilleDistribution/chirp.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageSrc.SetImagePtr(ref fliISrcImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliIDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}


				// 알고리즘 객체 생성 // Create algorithm object
				CWignerVilleDistribution wvd = new CWignerVilleDistribution();

				if((res = wvd.SetSourceImage(ref fliISrcImage)).IsFail())
					break;
				if((res = wvd.SetDestinationImage(ref fliIDstImage)).IsFail())
					break;
				if((res = wvd.SetScale(0.00004)).IsFail())
					break;
				if((res = wvd.SetSelfCorrelationHalfSize(511)).IsFail())
					break;
				if((res = wvd.SetSelfCorrelationWindow(CWignerVilleDistribution.ESelfCorrelationWindow.Gaussian)).IsFail())
					break;
				if((res = wvd.SetSigma(0.3)).IsFail())
					break;
				if((res = wvd.SetOutputMode(CWignerVilleDistribution.EOutputMode.L2Norm)).IsFail())
					break;
				if((res = wvd.SetOutputDirection(CWignerVilleDistribution.EOutputDirection.Horizontal)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = (wvd.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
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
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);
				if((res = layerSrc.DrawTextImage(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextImage(flpTemp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				viewImageSrc.ZoomFit();
				viewImageDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

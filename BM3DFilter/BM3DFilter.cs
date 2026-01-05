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

namespace FLImagingExamplesCSharp
{
	class BM3DFilter
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
			CFLImage fliDst1Image = new CFLImage();
			CFLImage fliDst2Image = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst1 = new CGUIViewImage();
			CGUIViewImage viewImageDst2 = new CGUIViewImage();

			do
			{
				// 알고리즘 동작 결과 // Algorithm execution result
				CResult res = new CResult();

				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/NoiseImage/NoiseImage1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination1 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination1 image as same as source image
				if((res = fliDst1Image.Assign(fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// Destination2 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination2 image as same as source image
				if((res = fliDst2Image.Assign(fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 550, 480)).IsFail())
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

				// Destination1 이미지 뷰 생성 // Create the destination1 image view
				if((res = viewImageDst1.Create(550, 0, 1000, 480)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination1 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination1 image view
				if((res = viewImageDst1.SetImagePtr(ref fliDst1Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination2 이미지 뷰 생성 // Create the destination2 image view
				if((res = viewImageDst2.Create(1000, 0, 1450, 480)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination2 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination2 image view
				if((res = viewImageDst2.SetImagePtr(ref fliDst2Image)).IsFail())				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst1)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageDst1.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageDst2.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// BM3DFilter 객체 생성 // Create BM3DFilter object
				CBM3DFilter bm3dFilter = new CBM3DFilter();

				// 처리할 이미지 설정 // Set the image to process
				bm3dFilter.SetSourceImage(ref fliSrcImage);

				// Destination 이미지 설정 // Set the destination image
				bm3dFilter.SetDestinationImage(ref fliDst1Image);

				// Sigma (노이즈의 표준편차) 설정 // Set the sigma (standard deviation of the noise)
				bm3dFilter.SetSigma(0.2);

				// Processing Mode 설정 // Set the processing mode
				bm3dFilter.SetProcessingMode(CBM3DFilter.EProcessingMode.BasicEstimate);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = bm3dFilter.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute BM3D Filter.");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				bm3dFilter.SetDestinationImage(ref fliDst2Image);

				// Processing Mode 설정 // Set the processing mode
				bm3dFilter.SetProcessingMode(CBM3DFilter.EProcessingMode.FinalEstimate);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = bm3dFilter.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute BM3D Filter.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst1 = viewImageDst1.GetLayer(0);
				CGUIViewImageLayer layerDst2 = viewImageDst2.GetLayer(0);

				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerDst1.DrawTextCanvas(flp, ("Destination1 Image (Basic Estimate)"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerDst2.DrawTextCanvas(flp, ("Destination2 Image (Final Estimate)"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst1.Invalidate(true);
				viewImageDst2.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && viewImageDst1.IsAvailable() && viewImageDst2.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

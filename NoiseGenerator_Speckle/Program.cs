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

namespace NoiseGenerator
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
			CFLImage fliImage = new CFLImage();
			CFLImage fliDestinationImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[2];
			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/NoiseGenerator/Plate.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				if((res = fliDestinationImage.Load("../../ExampleImages/NoiseGenerator/Plate.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create imageview
				if((res = viewImage[0].Create(400, 0, 912, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				if((res = viewImage[1].Create(912, 0, 1424, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.
				if((res = viewImage[0].SynchronizePointOfView(viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the imageview
				if((res = viewImage[0].SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				if((res = viewImage[1].SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.");
					break;
				}

				// Noise Generator 객체 생성 // Create Noise Generator object
				CNoiseGenerator noiseGenerator = new CNoiseGenerator();

				// ROI 범위 설정 // Set ROI range
				CFLRect<int> flrROI = new CFLRect<int>(61, 63, 583, 376);

				// 처리할 이미지 설정 // Set the image to process
				noiseGenerator.SetSourceImage(ref fliImage);
				noiseGenerator.SetDestinationImage(ref fliDestinationImage);
				// 처리할 ROI 설정 // Set the ROI to be processed
				noiseGenerator.SetSourceROI(flrROI);
				noiseGenerator.SetDestinationROI(flrROI);

				// 생성할 노이즈 설정 // Set the noise to generate
				noiseGenerator.SetNoiseType(CNoiseGenerator.ENoiseType.SpeckleNoise);
				// 간섭 잡음의 비율 설정 // Set the ratio of speckle noise
				noiseGenerator.SetSpeckleNoise(0.05);

				// 알고리즘 수행 // Execute the algorithm
				if((res = noiseGenerator.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute noise generator.");
					break;
				}

				CGUIViewImageLayer layer = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layerDst = viewImage[1].GetLayer(0);

				layer.Clear();
				layerDst.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the ROI area is
				if((res = layer.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerDst.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPosition00 = new CFLPoint<double>(0, 0);

				if((res = layer.DrawTextCanvas(flpPosition00, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerDst.DrawTextCanvas(flpPosition00, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
    }
}

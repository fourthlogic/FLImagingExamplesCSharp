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

namespace Crop
{
	class Crop
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
			CFLImage fliICropImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[2];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/Crop/bacteria.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliICropImage.Assign(fliISrcImage);

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(300, 0, 300 + 520, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(300 + 520, 0, 300 + 520 * 2, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[0].SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[1].SetImagePtr(ref fliICropImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Crop  객체 생성 // Create Crop object
				CCrop Crop = new CCrop();

				// Crop ROI 지정 // Create ROI range
				CFLRect<double> flrCropROI = new CFLRect<double>(161, 181, 293, 302);

				// Source 이미지 설정 // Set source image 
				Crop.SetSourceImage(ref fliICropImage);

				// Crop 영역 지정 // set source ROI 
				Crop.SetSourceROI(flrCropROI);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (Crop.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Crop.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// Text 출력 // Display Text 
				if((res = layer1.DrawTextImage(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(flpTemp, "Crop Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// Crop 영역 표기 // Crop Area draw
				if((res = layer1.DrawFigureImage(flrCropROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				for(int i = 1; i >= 0; --i)
				{
					viewImage[i].ZoomFit();
					viewImage[i].Invalidate(true);
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

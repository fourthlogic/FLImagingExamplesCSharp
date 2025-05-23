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

namespace ImageThreshold
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
			CFLImage fliISrcImage = new CFLImage();
			CFLImage fliIOprImage = new CFLImage();
			CFLImage fliIDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[3];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();
			viewImage[2] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/ImageThreshold/SunSrc.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliIOprImage.Load("../../ExampleImages/ImageThreshold/SunOpr.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

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

				if((res = viewImage[2].Create(300 + 520 * 2, 0, 300 + 520 * 3, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the image views. 
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the image view windows
				if((res = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[2])).IsFail())
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
				if((res = viewImage[1].SetImagePtr(ref fliIOprImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[2].SetImagePtr(ref fliIDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// ImageThreshold  객체 생성 // Create Image Threshold object
				CImageThreshold ImageThreshold = new CImageThreshold();

				// Source 이미지 설정 // Set source image 
				ImageThreshold.SetSourceImage(ref fliISrcImage);

				// Operand 이미지 설정 // Set operand image 
				ImageThreshold.SetOperandImage(ref fliIOprImage);

				// Destination 이미지 설정 // Set destination image
				ImageThreshold.SetDestinationImage(ref fliIDstImage);

				// 임계값 오프셋 설정 // set threshold offset 
				ImageThreshold.SetThresholdOffset(new CMultiVar<double>(5));

				// 알고리즘 수행 // Execute the algorithm
				if((res = (ImageThreshold.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute ImageThreshold.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer layer3 = viewImage[2].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layer1.DrawTextImage(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(flpTemp, "Operand Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer3.DrawTextImage(flpTemp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");


				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);
				viewImage[2].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

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

namespace FormatConverter
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
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliConvertedImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageConverted = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliSourceImage.Load("../../ExampleImages/FormatConverter/city_3.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSource.Create(112, 0, 912, 534)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageConverted.Create(913, 0, 1713, 534)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImageConverted.SetImagePtr(ref fliConvertedImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewImageSource.SynchronizePointOfView(viewImageConverted)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSource.SynchronizeWindow(viewImageConverted)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerConverted = viewImageConverted.GetLayer(1);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layerSource.Clear();
				layerConverted.Clear();

				CFLPoint<double> resultRegion = new CFLPoint<double>(0, 0);

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layerSource.DrawTextCanvas(resultRegion, "Source Image(3Ch U8)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerConverted.DrawTextCanvas(resultRegion, "Converted Image(1Ch U8)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// FormatConverter 객체 생성 // Create FormatConverter object
				CFormatConverter formatConverter = new CFormatConverter();

				// Source 이미지 설정 // Set source image 
				if((res = formatConverter.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				if((res = formatConverter.SetDestinationImage(ref fliConvertedImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source ROI.");
					break;
				}

				// 변환할 픽셀 포멧 입력(C3_U8 -> C1_U8) // Set the pixel format to convert(C3_U8 -> C1_U8)
				if((res = (formatConverter.SetPixelFormat(EPixelFormat.C1_U8))).IsFail())
				{
					ErrorPrint(res, "Failed to set PixelFormat.");
					break;
				}

				// 알고리즘 수행 // Execute the algorithm
				if((res = (formatConverter.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				// 이미지 뷰의 zoom fit // image view zoom fit
				if((res = viewImageConverted.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImageSource.Invalidate(true);
				viewImageConverted.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageConverted.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

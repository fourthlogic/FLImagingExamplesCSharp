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

namespace PixelCounter
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
			CFLImage fliIDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[2];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/PixelCounter/Semiconductor.flif")).IsFail())
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

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[0].SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Pixel Counter 객체 생성 // Create Pixel Counter object
				CPixelCounter PixelCounter = new CPixelCounter();

				// Source 이미지 설정 // Set source image 
				PixelCounter.SetSourceImage(ref fliISrcImage);

				// Source ROI 이미지 설정 // Set Source ROI
				CFLQuad<double> flfSourceROI = new CFLQuad<double>(170.550171, 102.400000, 380.243003, 135.950853, 341.100341, 312.092833, 124.417747, 265.960410);
				PixelCounter.SetSourceROI(flfSourceROI);

				// threshold 모드 설정(Dual) // Set Threshold Mode(Dual)
				PixelCounter.SetThresholdMode(EThresholdMode.Dual_And);

				// 임계값 설정 (다채널 경우 CMultiVar 사용) // Set threshold value(Use CMultiVarD for multi-channel)
				PixelCounter.SetThreshold(120, EThresholdIndex.First);
				PixelCounter.SetThreshold(230, EThresholdIndex.Second);

				// 논리 조건 설정 // Set condition value
				PixelCounter.SetLogicalCondition((long)ELogicalCondition.Greater, EThresholdIndex.First);
				PixelCounter.SetLogicalCondition((long)ELogicalCondition.Less, EThresholdIndex.Second);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (PixelCounter.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute PixelCounter.");
					break;
				}

				Int64 i64TotalPixel = PixelCounter.GetResultTotalPixelCount();
				Int64 i64ValidPixel = PixelCounter.GetResultValidPixelCount();
				Int64 i64InvalidPixel = PixelCounter.GetResultInvalidPixelCount();

				// 전체 픽셀, 유효한 픽셀, 유효하지 않은 픽셀 갯수 출력 // display Total, Valid, Invalid Pixel Count
				{
					Console.WriteLine(String.Format("Total Pixel Count : {0}", i64TotalPixel));
					Console.WriteLine(String.Format("Valid Pixel Count : {0}", i64ValidPixel));
					Console.WriteLine(String.Format("Invalid Pixel Count : {0}", i64InvalidPixel));
				}

				// Text 출력 // Display Text 
				string flsDrawText;
				flsDrawText = String.Format("Source Image\n120 < threshold < 230\nTotal Pixel Count: {0}\nValid Pixel Count: {0}\nInvalid Pixel Count: {0}", i64TotalPixel, i64ValidPixel, i64InvalidPixel);

				// 레이어는 따로 해제하지 않아도 View가 해제 될 때 같이 해제된다. // The layer is released together when View is released without releasing it separately.
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// Text 출력 // Display Text 
				if((res = layer1.DrawTextImage(flpTemp, flsDrawText, EColor.RED)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// Source ROI 출력 // Display Source ROI 
				if((res = layer1.DrawFigureImage(flfSourceROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw Source ROI .\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage[0].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

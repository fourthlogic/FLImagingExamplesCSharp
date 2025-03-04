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

namespace ActiveContour
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
				if((res = fliISrcImage.Load("../../ExampleImages/ActiveContour/Grid Of Cross.flif")).IsFail())
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
				if((res = viewImage[1].SetImagePtr(ref fliIDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Active Contour 객체 생성 // Create Active Contour object
				CActiveContour ac = new CActiveContour();

				// Source 이미지 설정 // Set source image 
                ac.SetSourceImage(ref fliISrcImage);

				// Source ROI 설정 // Set Source ROI
				CFLFigure flfSourceROI = CFigureUtilities.ConvertFigureStringToObject("RG[D(129.22800000000007, 126.67680000000001), D(731.22800000000007, 120.67680000000001), D(733.22800000000007, 262.67680000000001), D(253.22800000000007, 246.67680000000001), D(265.22800000000007, 600.67679999999996), D(603.22800000000007, 594.67679999999996), D(607.22800000000007, 400.67680000000001), D(403.22800000000007, 396.67680000000001), D(409.22800000000007, 448.67680000000001), D(565.22800000000007, 450.67680000000001), D(549.22800000000007, 556.67679999999996), D(289.22800000000007, 558.67679999999996), D(291.22800000000007, 292.67680000000001), D(721.22800000000007, 294.67680000000001), D(721.22800000000007, 720.67679999999996), D(119.22800000000007, 718.67679999999996), D(113.22800000000007, 142.67680000000001)]");
				ac.SetSourceROI(flfSourceROI);

				// Destination 이미지 설정 // Set destination image
				ac .SetDestinationImage(ref fliIDstImage);

				// Point Count 설정 // Set Point Count
				ac.SetPointCount(3000);

				// Max Length 설정 // Set Max Length
				ac.SetMaxLength(3);

				// Low Threshold 설정 // Set Low Threshold
				ac.SetLowThreshold(20);

				// High Threshold 설정 // Set High Threshold
				ac.SetHighThreshold(50);

				// Fit Margin 설정 // Set Fit Margin
				ac.SetFitMargin(3);

				// 알고리즘 수행 // Execute the algorithm
                if ((res = (ac.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Active Contour.");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);

				for(Int32 i32Iteration = 0; i32Iteration < 20; ++i32Iteration)
				{
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Fit();
					ac.Spacing();
					ac.Spacing();
					ac.Spacing();
					ac.Spacing();
					ac.Spacing();

                    /* PushBack Figure */
                    {
						CFLRegion flfContourFigure = new CFLRegion();
						ac.GetContourFigure(out flfContourFigure);

						viewImage[0].ClearFigureObject();
                        viewImage[0].PushBackFigureObject(flfContourFigure);
                        viewImage[0].Invalidate(true);

						Thread.Sleep(50);
                    }
				}

				viewImage[0].PushBackFigureObject(ac.GetSourceROI());

				// 레이어는 따로 해제하지 않아도 View가 해제 될 때 같이 해제된다. // The layer is released together when View is released without releasing it separately.
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layer1.DrawTextImage(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(flpTemp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");


				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

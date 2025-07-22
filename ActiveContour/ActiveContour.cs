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
	class ActiveContour
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/ActiveContour/Grid Of Cross.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image views
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
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

				// 이미지 뷰에 이미지를 디스플레이 // Display the images in the image views
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}


				// 알고리즘 객체 생성 // Create algorithm object
				CActiveContour algObject = new CActiveContour();

				if((res = algObject.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				CFLFigure flfSourceROI = CFigureUtilities.ConvertFigureStringToObject("RG[D(129.22800000000007, 126.67680000000001), D(731.22800000000007, 120.67680000000001), D(733.22800000000007, 262.67680000000001), D(253.22800000000007, 246.67680000000001), D(265.22800000000007, 600.67679999999996), D(603.22800000000007, 594.67679999999996), D(607.22800000000007, 400.67680000000001), D(403.22800000000007, 396.67680000000001), D(409.22800000000007, 448.67680000000001), D(565.22800000000007, 450.67680000000001), D(549.22800000000007, 556.67679999999996), D(289.22800000000007, 558.67679999999996), D(291.22800000000007, 292.67680000000001), D(721.22800000000007, 294.67680000000001), D(721.22800000000007, 720.67679999999996), D(119.22800000000007, 718.67679999999996), D(113.22800000000007, 142.67680000000001)]");
				if((res = algObject.SetSourceROI(flfSourceROI)).IsFail())
					break;
				if((res = algObject .SetDestinationImage(ref fliDstImage)).IsFail())
					break;
				if((res = algObject.SetPointCount(3000)).IsFail())
					break;
				if((res = algObject.SetMaxLength(3)).IsFail())
					break;
				if((res = algObject.SetLowThreshold(20)).IsFail())
					break;
				if((res = algObject.SetHighThreshold(50)).IsFail())
					break;
				if((res = algObject.SetFitMargin(3)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}


				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				for(Int32 i32Iteration = 0; i32Iteration < 20; ++i32Iteration)
				{
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Fit();
					algObject.Spacing();
					algObject.Spacing();
					algObject.Spacing();
					algObject.Spacing();
					algObject.Spacing();

                    /* PushBack Figure */
                    {
						viewImageSrc.ClearFigureObject();
                        viewImageSrc.PushBackFigureObject(algObject.GetContourFigure());
						viewImageSrc.Invalidate(true);

						Thread.Sleep(50);
                    }
				}

				viewImageSrc.PushBackFigureObject(algObject.GetSourceROI());

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);
				if((res = layerSrc.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				// Zoom Fit
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

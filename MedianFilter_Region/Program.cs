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


namespace MedianFilter
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare the image object
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			do
			{
				// 이미지 로드 // Load image
                if (fliSrcImage.Load("../../ExampleImages/MedianFilter/Chip_Noise.flif").IsFail())
				{
					Console.WriteLine("Failed to load the image file.\n");
					break;
				}

				// Destination이미지를 Src 이미지와 동일한 이미지로 생성
				if(fliDstImage.Assign(fliSrcImage).IsFail())
				{
					Console.WriteLine("Failed to assign the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if(viewImageSrc.Create(400, 0, 912, 612).IsFail())
				{
					Console.WriteLine("Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if(viewImageSrc.SetImagePtr(fliSrcImage).IsFail())
				{
					Console.WriteLine("Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if(viewImageDst.Create(912, 0, 1424, 612).IsFail())
				{
					Console.WriteLine("Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if(viewImageDst.SetImagePtr(fliDstImage).IsFail())
				{
					Console.WriteLine("Failed to set image object on the image view.\n");
					break;
				}

				// Source이미지 뷰와 Dst 이미지 뷰의 초점을 맞춤
				if(viewImageSrc.SynchronizePointOfView(viewImageDst).IsFail())
				{
					Console.WriteLine("Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if(viewImageSrc.ZoomFit().IsFail())
				{
					Console.WriteLine("Failed to zoom fit\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if(viewImageDst.ZoomFit().IsFail())
				{
					Console.WriteLine("Failed to zoom fit\n");
					break;
				}

				CResult ipResult;

				// Blob 객체 생m성
                CMedianFilter filterMedian = new CMedianFilter();

				// 처리할 이미지 설정 // Set the image to process
				filterMedian.SetSourceImage(ref fliSrcImage);

				// ROI 범위 설정
				CFLRect<Int32> flrROI = new CFLRect<int>(100, 190, 360, 420);

				// Source ROI 설정 // Set the Source ROI
				filterMedian.SetSourceROI(flrROI);

				// Destination 이미지 설정 // Set the destination image
				filterMedian.SetDestinationImage(ref fliDstImage);

				// Destination ROI 설정 // Set Destination ROI
				filterMedian.SetDestinationROI(flrROI);

				// 처리할 filter의 Kernel 의 L(0, 0, 5, 5, 45.000000) // Set Morphology Kernel to L(0, 0, 5, 5, 45.000000)
				CFLRect<int> flrRegion = new CFLRect<int>(0, 0, 5, 5, 45.000000);
				filterMedian.SetKernel(flrRegion);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if ((ipResult = filterMedian.Execute()).IsFail())
				{
					Console.WriteLine("Failed to execute median filter.");
					Console.WriteLine(ipResult.GetString());
					Console.WriteLine("\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				if(layerSrc.DrawFigureImage(flrROI, EColor.BLUE).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if(layerDst.DrawFigureImage(flrROI, EColor.BLUE).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				CFLPoint<double> flp = new CFLPoint<double>();

				if(layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20).IsFail())
				{
					Console.WriteLine("Failed to draw text\n");
					break;
				}

				if(layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20).IsFail())
				{
					Console.WriteLine("Failed to draw text\n");
					break;
				}

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

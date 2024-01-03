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

namespace Morphology_Median_Separate
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			do
			{
				// 이미지 로드
				if(fliSrcImage.Load("../../FLImagingExamples/Images/Morphology/Chip_Noise.bmp").IsFail())
				{
					Console.WriteLine("Failed to load the image file.\n");
					break;
				}

				// Dst이미지를 Src 이미지와 동일한 이미지로 생성
				if(fliDstImage.Assign(fliSrcImage).IsFail())
				{
					Console.WriteLine("Failed to assign the image file.\n");
					break;
				}

				// 이미지 뷰 생성
				if(viewImageSrc.Create(400, 0, 912, 612).IsFail())
				{
					Console.WriteLine("Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이
				if(viewImageSrc.SetImagePtr(ref fliSrcImage).IsFail())
				{
					Console.WriteLine("Failed to set image object on the image view.\n");
					break;
				}

				// Dst 이미지 뷰 생성
				if(viewImageDst.Create(912, 0, 1424, 612).IsFail())
				{
					Console.WriteLine("Failed to create the image view.\n");
					break;
				}

				// Dst 이미지 뷰에 이미지를 디스플레이
				if(viewImageDst.SetImagePtr(ref fliDstImage).IsFail())
				{
					Console.WriteLine("Failed to set image object on the image view.\n");
					break;
				}

				// Src이미지 뷰와 Dst 이미지 뷰의 초점을 맞춤
				if(viewImageSrc.SynchronizePointOfView(ref viewImageDst).IsFail())
				{
					Console.WriteLine("Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정
				if(viewImageSrc.ZoomFit().IsFail())
				{
					Console.WriteLine("Failed to zoom fit\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정
				if(viewImageDst.ZoomFit().IsFail())
				{
					Console.WriteLine("Failed to zoom fit\n");
					break;
				}

				CImageProcessingResult ipResult;

				// Blob 객체 생성
				CMorphologyMedianSeparate morphologyMedianSeparate = new CMorphologyMedianSeparate();

				// 처리할 이미지 설정
				morphologyMedianSeparate.SetSourceImage(ref fliSrcImage);

				// ROI 범위 설정
				CFLRect<Int32> flrROI = new CFLRect<int>(100, 190, 360, 420);

				// Src ROI 설정
				morphologyMedianSeparate.SetSourceROI(flrROI);

				// Dst 이미지 설정
				morphologyMedianSeparate.SetDestinationImage(ref fliDstImage);

				// Dst ROI 설정
				morphologyMedianSeparate.SetDestinationROI(flrROI);

                // 처리할 MorphologyMedianSeparate의 Kernel Size 설정 (KernelSize = 11 일 경우, Kernel Size : 11x11, 홀수만 설정가능)
				morphologyMedianSeparate.SetKernelSize(11);

				// 앞서 설정된 파라미터 대로 알고리즘 수행
				if((ipResult = morphologyMedianSeparate.Execute()).IsFail())
				{
					Console.WriteLine("Failed to execute morphology MedianSeparate.");
					Console.WriteLine(ipResult.GetString());
					Console.WriteLine("\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제
				layerSrc.Clear();
				layerDst.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다
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

				// 이미지 뷰를 갱신 합니다.
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

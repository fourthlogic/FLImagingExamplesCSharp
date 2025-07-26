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

namespace FourierTransform
{
	class FourierTransform
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
			CFLImage fliFTImage = new CFLImage();
			CFLImage fliIFFTImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[3];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();
			viewImage[2] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/FourierTransform/TempleNoise.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(300, 0, 300 + 512, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(300 + 512, 0, 300 + 512 * 2, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[2].Create(300 + 512 * 2, 0, 300 + 512 * 3, 384)).IsFail())
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

				if((res = viewImage[1].SynchronizePointOfView(ref viewImage[2])).IsFail())
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

				if((res = viewImage[1].SynchronizeWindow(ref viewImage[2])).IsFail())
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
				if((res = viewImage[1].SetImagePtr(ref fliFTImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[2].SetImagePtr(ref fliIFFTImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Fourier Transform 객체 생성 // Create Fourier Transform object
				CFourierTransform FourierTransform = new CFourierTransform();

				// Source 이미지 설정 // Set source image 
				FourierTransform.SetSourceImage(ref fliISrcImage);

				// Destination 이미지 설정 // Set destination image
				FourierTransform.SetDestinationImage(ref fliFTImage);

				// 결과 이미지 포멧 설정 (FFT image, 32/64 bit Floating Point 설정 가능) // Set Result image format(FFT image, 32/64 bit Floating Point) 
				FourierTransform.SetResultType(EFloatingPointAccuracy.Bit32);

				// 푸리에 변환 결과 이미지를 쉬프트해서 받도록 설정 // Set to receive a shifted image of the Fourier transform result
				FourierTransform.SetShiftSpectrum(EFourierTransformShiftSpectrum.Shift);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (FourierTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute FourierTransform.");
					break;
				}

				// 이미지의 노이즈를 감소하기(Mask 사용) // Reduce the noise in the image (using Mask)
				// Mask 객체 생성 // Create Mask object
				CMask Mask = new CMask();

				// 변환 이미지를 설정(FFT) // Set source image(FFT image)
				Mask.SetSourceImage(ref fliFTImage);

				// CFLFigureArray 객체를 생성 // Create CFLFigureArray object
				CFLFigureArray flfArray = new CFLFigureArray();

				// 미리 그려둔 Mask region Figure Array 불러오기 // Load Pre-drawn Mask Region Figure Array
				if((res = flfArray.Load("../../ExampleImages/FourierTransform/FFTRegion.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.");
					break;
				}

				// 지정한 ROI를 입력 // Set mask ROI
				Mask.SetSourceROI(flfArray);

				// 마스크 값을 입력 // set mask value
				Mask.SetMask(0.0);

				// 알고리즘 수행(mask) // Execute the algorithm(mask)		
				if((res = (Mask.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Mask.");
					break;
				}

				// Source 이미지 설정(FFT image) // Set source image (FFT image)
				FourierTransform.SetSourceImage(ref fliFTImage);

				// Destination 이미지 설정(IFFT image) // Set destination image(IFFT image)
				FourierTransform.SetDestinationImage(ref fliIFFTImage);

				// 알고리즘 수행(IFFT) // Execute the algorithm(IFFT)
				if((res = (FourierTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Fourier Transform.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer layer3 = viewImage[2].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// Text 출력 // Display Text 
				if((res = layer1.DrawTextImage(flpTemp, "Spatial Domain", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(flpTemp, "Frequency Domain", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer3.DrawTextImage(flpTemp, "Inverse FFT Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				for(int i = 0; i < 3; ++i)
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

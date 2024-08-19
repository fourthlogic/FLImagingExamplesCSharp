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

namespace FilterGeneratorFD
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
			CFLImage fliIFFTImage = new CFLImage();
			CFLImage fliIFilterImage = new CFLImage();
			CFLImage fliIMultiplyImage = new CFLImage();
			CFLImage fliIDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[5];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();
			viewImage[2] = new CGUIViewImage();
			viewImage[3] = new CGUIViewImage();
			viewImage[4] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/FilterGeneratorFD/Sea1Ch.flif")).IsFail())
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

				if((res = viewImage[3].Create(300, 0 + 430, 300 + 520, 430 * 2)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[4].Create(300 + 520, 0 + 430, 300 + 520 * 2, 430 * 2)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewImage[0].SynchronizePointOfView(viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(viewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(viewImage[4])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(viewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(viewImage[4])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[0].SetImagePtr(fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[1].SetImagePtr(fliIFFTImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[2].SetImagePtr(fliIFilterImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[3].SetImagePtr(fliIMultiplyImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[4].SetImagePtr(fliIDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Fourier Transform 객체 생성 // Create Fourier Transform object
				CFourierTransform FourierTransform = new CFourierTransform();

				// Source 이미지 설정 // Set source image 
				FourierTransform.SetSourceImage(ref fliISrcImage);

				// Destination 이미지 설정 // Set destination image
				FourierTransform.SetDestinationImage(ref fliIFFTImage);

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

				// FilterGeneratorGaussFD  객체 생성 // Create FilterGeneratorGaussFD object
				CFilterGeneratorGaussFD filterGenerator = new CFilterGeneratorGaussFD();

				// Source 이미지 설정 // Set source image 
				filterGenerator.SetSourceImage(ref fliISrcImage);

				// Destination 이미지 설정 // Set destination image
				filterGenerator.SetDestinationImage(ref fliIFilterImage);

				// 정밀도 설정 (32/64 bit Floating Point 설정 가능) // Set Accuracy(32/64 bit Floating Point) 
				filterGenerator.SetAccuracy(EFloatingPointAccuracy.Bit32);

				// 필터 타입 설정 // set Filter type
				filterGenerator.SetType(CFilterGeneratorGaussFD.EFilterBaseFDType.FFT_Shift);

				// Sigma1 설정 // Set Sigma1
				filterGenerator.SetSigma1(2);

				// Sigma2 설정 // Set Sigma2
				filterGenerator.SetSigma2(1);

				// Phi 설정 // Set Phi
				filterGenerator.SetPhi(0.785398f);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (filterGenerator.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute FilterGeneratorGaussFD.");
					break;
				}

				// Operation Multiply 객체 생성 // Create Operation Multiply object
				COperationMultiply multiply = new COperationMultiply();

				// Source 이미지 설정 // Set the source image
				multiply.SetSourceImage(ref fliIFFTImage);

				// Operand 이미지 설정 // Set the operand image
				multiply.SetOperandImage(ref fliIFilterImage);

				// Destination 이미지 설정 // Set the destination image
				multiply.SetDestinationImage(ref fliIMultiplyImage);

				// 연산 방식 설정 // Set operation source
				multiply.SetOperationSource(EOperationSource.Image);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (multiply.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation multiply.");
					break;
				}

				// Source 이미지 설정(FFT image) // Set source image (FFT image)
				FourierTransform.SetSourceImage(ref fliIMultiplyImage);

				// Destination 이미지 설정(IFFT image) // Set destination image(IFFT image)
				FourierTransform.SetDestinationImage(ref fliIDstImage);

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
				CGUIViewImageLayer layer4 = viewImage[3].GetLayer(0);
				CGUIViewImageLayer layer5 = viewImage[4].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layer1.DrawTextImage(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(flpTemp, "FFT Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer3.DrawTextImage(flpTemp, "Destination Image(Filter)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer4.DrawTextImage(flpTemp, "Filtering FFT Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer5.DrawTextImage(flpTemp, "Filtering Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");


				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);
				viewImage[2].Invalidate(true);
				viewImage[3].Invalidate(true);
				viewImage[4].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

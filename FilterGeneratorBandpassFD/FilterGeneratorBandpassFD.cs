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

namespace FLImagingExamplesCSharp
{
	class FilterGeneratorBandpassFD
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliFFTImage = new CFLImage();
			CFLImage fliIdealFilter = new CFLImage();
			CFLImage fliButterworthFilter = new CFLImage();
			CFLImage fliGaussianFilter = new CFLImage();
			CFLImage fliIdealDst = new CFLImage();
			CFLImage fliButterworthDst = new CFLImage();
			CFLImage fliGaussianDst = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[8];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();
			viewImage[2] = new CGUIViewImage();
			viewImage[3] = new CGUIViewImage();
			viewImage[4] = new CGUIViewImage();
			viewImage[5] = new CGUIViewImage();
			viewImage[6] = new CGUIViewImage();
			viewImage[7] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/FilterGeneratorFD/Sea1Ch.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(300, 0, 300 + 400, 410)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(300 + 400, 0, 300 + 400 * 2, 410)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[2].Create(300 + 400 * 2, 0, 300 + 400 * 3, 410)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[3].Create(300 + 400 * 3, 0, 300 + 400 * 4, 410)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[4].Create(300, 410, 300 + 400, 820)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[5].Create(300 + 400, 410, 300 + 400 * 2, 820)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[6].Create(300 + 400 * 2, 410, 300 + 400 * 3, 820)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[7].Create(300 + 400 * 3, 410, 300 + 400 * 4, 820)).IsFail())
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

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[4])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[5])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[6])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[7])).IsFail())
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

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[4])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[5])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[6])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				if((res = viewImage[0].SynchronizeWindow(ref viewImage[7])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[0].SetImagePtr(ref fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[1].SetImagePtr(ref fliIdealFilter)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[2].SetImagePtr(ref fliButterworthFilter)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[3].SetImagePtr(ref fliGaussianFilter)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[4].SetImagePtr(ref fliFFTImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[5].SetImagePtr(ref fliIdealDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[6].SetImagePtr(ref fliButterworthDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage[7].SetImagePtr(ref fliGaussianDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Fourier Transform 객체 생성 // Create Fourier Transform object
				CFourierTransform fourierTransform = new CFourierTransform();

				// Source 이미지 설정 // Set source image 
				fourierTransform.SetSourceImage(ref fliSrcImage);

				// Destination 이미지 설정 // Set destination image
				fourierTransform.SetDestinationImage(ref fliFFTImage);

				// 결과 이미지 포멧 설정 (FFT image, 32/64 bit Floating Point 설정 가능) // Set Result image format(FFT image, 32/64 bit Floating Point) 
				fourierTransform.SetResultType(EFloatingPointAccuracy.Bit32);

				// 푸리에 변환 결과 이미지를 쉬프트해서 받도록 설정 // Set to receive a shifted image of the Fourier transform result
				fourierTransform.SetShiftSpectrum(EFourierTransformShiftSpectrum.Shift);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (fourierTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute FourierTransform.");
					break;
				}

				// FilterGeneratorBandpassFD  객체 생성 // Create FilterGeneratorBandpassFD object
				CFilterGeneratorBandpassFD filterGeneratorBandpassFD = new CFilterGeneratorBandpassFD();

				// Source 이미지 설정 // Set source image 
				filterGeneratorBandpassFD.SetSourceImage(ref fliFFTImage);

				// Destination 이미지 설정 // Set destination image
				filterGeneratorBandpassFD.SetDestinationImage(ref fliIdealFilter);

				// 정밀도 설정 (32/64 bit Floating Point 설정 가능) // Set Accuracy(32/64 bit Floating Point) 
				filterGeneratorBandpassFD.SetAccuracy(EFloatingPointAccuracy.Bit32);

				// MinFrequency 설정 // Set MinFrequency
				filterGeneratorBandpassFD.SetMinFrequency(0.1);

				// MaxFrequency 설정 // Set MaxFrequency
				filterGeneratorBandpassFD.SetMaxFrequency(0.6);

				// Filter Shape 설정 // Set Filter Shape
				filterGeneratorBandpassFD.SetFilterShape(CFilterGeneratorBandpassFD.EFilterShape.EFilterShape_Ideal);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (filterGeneratorBandpassFD.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute FilterGeneratorBandpassFD.");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				filterGeneratorBandpassFD.SetDestinationImage(ref fliButterworthFilter);

				// Filter Shape 설정 // Set Filter Shape
				filterGeneratorBandpassFD.SetFilterShape(CFilterGeneratorBandpassFD.EFilterShape.EFilterShape_Butterworth);

				// Distance 설정 // Set Distance
				filterGeneratorBandpassFD.SetDistance(256);

				// Degree 설정 // Set Degree
				filterGeneratorBandpassFD.SetDegree(2);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (filterGeneratorBandpassFD.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute FilterGeneratorBandpassFD.");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				filterGeneratorBandpassFD.SetDestinationImage(ref fliGaussianFilter);

				// Filter Shape 설정 // Set Filter Shape
				filterGeneratorBandpassFD.SetFilterShape(CFilterGeneratorBandpassFD.EFilterShape.EFilterShape_Gaussian);

				// Sigma1 설정 // Set Sigma1
				filterGeneratorBandpassFD.SetSigma1(1);

				// Sigma2 설정 // Set Sigma2
				filterGeneratorBandpassFD.SetSigma2(1);

				// Phi 설정 // Set Phi
				filterGeneratorBandpassFD.SetPhi(0);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (filterGeneratorBandpassFD.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute FilterGeneratorBandpassFD.");
					break;
				}

				// Operation Multiply 객체 생성 // Create Operation Multiply object
				COperationMultiply operationMultiply = new COperationMultiply();

				// Source 이미지 설정 // Set the source image
				operationMultiply.SetSourceImage(ref fliFFTImage);

				// Operand 이미지 설정 // Set the operand image
				operationMultiply.SetOperandImage(ref fliIdealFilter);

				// Destination 이미지 설정 // Set the destination image
				operationMultiply.SetDestinationImage(ref fliIdealDst);

				// 연산 방식 설정 // Set operation source
				operationMultiply.SetOperationSource(EOperationSource.Image);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (operationMultiply.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation multiply.");
					break;
				}

				// Operand 이미지 설정 // Set the operand image
				operationMultiply.SetOperandImage(ref fliButterworthFilter);

				// Destination 이미지 설정 // Set the destination image
				operationMultiply.SetDestinationImage(ref fliButterworthDst);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (operationMultiply.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation multiply.");
					break;
				}

				// Operand 이미지 설정 // Set the operand image
				operationMultiply.SetOperandImage(ref fliGaussianFilter);

				// Destination 이미지 설정 // Set the destination image
				operationMultiply.SetDestinationImage(ref fliGaussianDst);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (operationMultiply.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation multiply.");
					break;
				}

				// Source 이미지 설정(FFT image) // Set source image (FFT image)
				fourierTransform.SetSourceImage(ref fliIdealDst);

				// Destination 이미지 설정(IFFT image) // Set destination image(IFFT image)
				fourierTransform.SetDestinationImage(ref fliIdealDst);

				// 알고리즘 수행(IFFT) // Execute the algorithm(IFFT)
				if((res = (fourierTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Fourier Transform.");
					break;
				}

				// Source 이미지 설정(FFT image) // Set source image (FFT image)
				fourierTransform.SetSourceImage(ref fliButterworthDst);

				// Destination 이미지 설정(IFFT image) // Set destination image(IFFT image)
				fourierTransform.SetDestinationImage(ref fliButterworthDst);

				// 알고리즘 수행(IFFT) // Execute the algorithm(IFFT)
				if((res = (fourierTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Fourier Transform.");
					break;
				}

				// Source 이미지 설정(FFT image) // Set source image (FFT image)
				fourierTransform.SetSourceImage(ref fliGaussianDst);

				// Destination 이미지 설정(IFFT image) // Set destination image(IFFT image)
				fourierTransform.SetDestinationImage(ref fliGaussianDst);

				// 알고리즘 수행(IFFT) // Execute the algorithm(IFFT)
				if((res = (fourierTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Fourier Transform.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[4].GetLayer(0);
				CGUIViewImageLayer layer3 = viewImage[1].GetLayer(0);
				CGUIViewImageLayer layer4 = viewImage[2].GetLayer(0);
				CGUIViewImageLayer layer5 = viewImage[3].GetLayer(0);
				CGUIViewImageLayer layer6 = viewImage[5].GetLayer(0);
				CGUIViewImageLayer layer7 = viewImage[6].GetLayer(0);
				CGUIViewImageLayer layer8 = viewImage[7].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// View 정보를 디스플레이 합니다. // Display View information.
				if((res = layer1.DrawTextCanvas(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextCanvas(flpTemp, "FFT Image", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer3.DrawTextCanvas(flpTemp, "Ideal\nMin = 0.1, Max = 0.6", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer4.DrawTextCanvas(flpTemp, "Butterworth\nDistance = 256, Degree = 2", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer5.DrawTextCanvas(flpTemp, "Gaussian\nSigma1 = Sigma2 = 1, Phi = 0", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer6.DrawTextCanvas(flpTemp, "Ideal Filtering Image", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer7.DrawTextCanvas(flpTemp, "Butterworth Filtering Image", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer8.DrawTextCanvas(flpTemp, "Gaussian Filtering Image", EColor.YELLOW, EColor.BLACK, 22)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");


				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);
				viewImage[2].Invalidate(true);
				viewImage[3].Invalidate(true);
				viewImage[4].Invalidate(true);
				viewImage[5].Invalidate(true);
				viewImage[6].Invalidate(true);
				viewImage[7].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

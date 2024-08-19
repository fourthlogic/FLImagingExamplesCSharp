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

namespace Integral
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
				if((res = fliISrcImage.Load("../../ExampleImages/Integral/Lake.flif")).IsFail())
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
				if((res = viewImage[0].SynchronizePointOfView(viewImage[1])).IsFail())
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

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[0].SetImagePtr(fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage[1].SetImagePtr(fliIDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				CMultiVar<double> mvCoefficients = new CMultiVar<double>(1.7, 2.1, 0);

				// Integral  객체 생성 // Create Integral  object
				CIntegral Integral = new CIntegral();

				// Source 이미지 설정 // Set source image 
				Integral.SetSourceImage(ref fliISrcImage);

				// Destination 이미지 설정 // Set destination image
				Integral.SetDestinationImage(ref fliIDstImage);

				// 적분합 자료형 타입을 설정합니다. // Set integral data type.
				Integral.SetDataType(CIntegral.EDataType.Uint32);

				// Integral 누적합 연산 모드 설정 // Set integration operation method.
				// ECalculationMode_SquareAndSum : ax^2 + bx 다항식 방식의 합 // ECalculationMode_SquareAndSum : ax^2 + bx Polynomial Sum
				Integral.SetCalculationMode(CIntegral.ECalculationMode.SquareAndSum);

				// ax^2 + bx + c 계수 설정(a = 1.7, b = 2.1, c = 0) // ax^2 + bx + c Setting the coefficient (a = 1.7, b = 2.1, c = 0)
				Integral.SetCoefficients(mvCoefficients);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (Integral.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Integral.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);
				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// Text 출력 // Display Text 
				if((res = layer1.DrawTextImage(flpTemp, "Source Image", EColor.RED)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = layer2.DrawTextImage(flpTemp, "Destination Image", EColor.RED)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				for(int i = 0; i < 2; ++i)
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

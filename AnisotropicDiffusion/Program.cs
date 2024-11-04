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

namespace AnisotropicDiffusion
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

		enum EType
		{
			Source = 0,
			Destination1,
			Destination2,
			Destination3,
			ETypeCount,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrFliImage[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrViewImage[i] = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = arrFliImage[(int)EType.Source].Load("../../ExampleImages/NoiseImage/NoiseImage1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				bool bError = false;

				for(int i = (int)EType.Destination1; i < (int)EType.ETypeCount; ++i)
				{
					// Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
					if((res = (arrFliImage[i].Assign(arrFliImage[(int)EType.Source]))).IsFail())
					{
						ErrorPrint(res, "Failed to assign the image file.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					int x = i % 2;
					int y = i / 2;

					// 이미지 뷰 생성 // Create image view
					if((res = (arrViewImage[i].Create(x * 400 + 400, y * 400, x * 400 + 400 + 400, y * 400 + 400))).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						bError = true;
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
					if((res = (arrViewImage[i].SetImagePtr(ref arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}

					if(i == (int)EType.Source)
						continue;

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if((res = (arrViewImage[(int)EType.Source].SynchronizePointOfView(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
					if((res = (arrViewImage[(int)EType.Source].SynchronizeWindow(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// AnisotropicDiffusion 객체 생성 // Create AnisotropicDiffusion object
				CAnisotropicDiffusion AnisotropicDiffusion = new CAnisotropicDiffusion();

				// Source 이미지 설정 // Set the source image
				AnisotropicDiffusion.SetSourceImage(ref arrFliImage[(int)EType.Source]);

				// Destination1 이미지 설정 // Set the destination image
				AnisotropicDiffusion.SetDestinationImage(ref arrFliImage[(int)EType.Destination1]);

				// Diffusion coefficient 모드 설정(Parabolic) // Set diffusion coefficient mode(Parabolic)
				AnisotropicDiffusion.SetDiffusionMode(CAnisotropicDiffusion.EDiffusionCoefficientMode.Parabolic);

				// Contrast = 3 설정 // Set the Contrast value(3.0)
				AnisotropicDiffusion.SetContrast(3.0);

				// Theta = 2 설정 // Set the Theta value(2.0)
				AnisotropicDiffusion.SetTheta(2.0);

				// Iteration = 10 설정 // Set the Iteration value(10)
				AnisotropicDiffusion.SetIteration(10);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = AnisotropicDiffusion.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute AnisotropicDiffusion. \n");
					break;
				}

				// Destination2 이미지 설정 // Set the destination image
				AnisotropicDiffusion.SetDestinationImage(ref arrFliImage[(int)EType.Destination2]);

				// Diffusion coefficient 모드 설정(PeronaMalik) // Set diffusion coefficient mode(PeronaMalik)
				AnisotropicDiffusion.SetDiffusionMode(CAnisotropicDiffusion.EDiffusionCoefficientMode.PeronaMalik);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = AnisotropicDiffusion.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute AnisotropicDiffusion. \n");
					break;
				}

				// Destination3 이미지 설정 // Set the destination image
				AnisotropicDiffusion.SetDestinationImage(ref arrFliImage[(int)EType.Destination3]);

				// Diffusion coefficient 모드 설정(Weickert) // Set diffusion coefficient mode(Weickert)
				AnisotropicDiffusion.SetDiffusionMode(CAnisotropicDiffusion.EDiffusionCoefficientMode.Weickert);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = AnisotropicDiffusion.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute AnisotropicDiffusion. \n");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.ETypeCount];

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayer[i].Clear();
				}

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = (arrLayer[(int)EType.Source].DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination1].DrawTextCanvas(flpPoint, "Destination1 Image (Parabolic Mode)", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination2].DrawTextCanvas(flpPoint, "Destination2 Image (PeronaMalik Mode)", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination3].DrawTextCanvas(flpPoint, "Destination3 Image (Weickert Mode)", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < (int)EType.ETypeCount; ++i)
					arrViewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bAvailable = true;
				while(bAvailable)
				{
					for(int i = 0; i < (int)EType.ETypeCount; ++i)
					{
						bAvailable = arrViewImage[i].IsAvailable();

						if(!bAvailable)
							break;
					}

					Thread.Sleep(1);
				}
			}
			while(false);
		}
	}
}

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

namespace Convolution_UserDefinedKernel
{
	class Program
	{
		enum EType
		{
			Src = 0,
			Dst1,
			Dst2,
			Dst3,
			ETypeCount,
		};


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
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			bool bError = false;

			do
			{
				CResult res;

				// Source 이미지 로드 // Load the source image
				if((res = arrFliImage[(int)EType.Src].Load("../../ExampleImages/Filter/Sun.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					// Destination 이미지를 Src 이미지와 동일한 이미지로 생성
					if(i != (int)EType.Src)
					{
						if((res = arrFliImage[i].Assign(arrFliImage[(int)EType.Src])).IsFail())
						{
							ErrorPrint(res, "Failed to assign the image file.\n");
							bError = true;
							break;
						}
					}

					int i32X = i % 2;
					int i32Y = i / 2;

					// 이미지 뷰 생성 // Create image view
					if((res = arrViewImage[i].Create(i32X * 400 + 400, i32Y * 400, i32X * 400 + 400 + 400, i32Y * 400 + 400)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						bError = true;
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
					if((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if(i != (int)EType.Src)
					{
						if((res = arrViewImage[(int)EType.Src].SynchronizePointOfView(ref arrViewImage[i])).IsFail())
						{
							ErrorPrint(res, "Failed to synchronize view\n");
							bError = true;
							break;
						}

						// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
						if((res = arrViewImage[(int)EType.Src].SynchronizeWindow(ref arrViewImage[i])).IsFail())
						{
							ErrorPrint(res, "Failed to synchronize window.\n");
							bError = true;
							break;
						}
					}
				}

				if(bError)
					break;

				// Convolution UserDefinedKernel 객체 생성 // Create Convolution UserDefinedKernel object
				CConvolutionFilter convolution = new CConvolutionFilter();

				// Source 이미지 설정 // Set the source image
				convolution.SetSourceImage(ref arrFliImage[(int)EType.Src]);

				// 커널을 설정하기 위해 FLArray 생성
				List<List<float>> flarrKernel = new List<List<float>>();
				List<float> flarrKernelElement = new List<float>();

				// 커널 생성 (Gaussian blur)
				// 1.0f, 2.0f, 1.0f
				// 2.0f, 4.0f, 2.0f
				// 1.0f, 2.0f, 1.0f
				flarrKernel.Clear();

				flarrKernelElement.Clear();
				flarrKernelElement.Add(1.0f);
				flarrKernelElement.Add(2.0f);
				flarrKernelElement.Add(1.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				flarrKernelElement.Clear();
				flarrKernelElement.Add(2.0f);
				flarrKernelElement.Add(4.0f);
				flarrKernelElement.Add(2.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				flarrKernelElement.Clear();
				flarrKernelElement.Add(1.0f);
				flarrKernelElement.Add(2.0f);
				flarrKernelElement.Add(1.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				// 커널 설정
				convolution.SetKernel(flarrKernel);

				// Destination 이미지 설정 // Set the destination image
				convolution.SetDestinationImage(ref arrFliImage[(int)EType.Dst1]);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = convolution.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute convolution.");
					break;
				}

				// 커널 생성 (Sharpen)
				// 0.0f, -1.0f, 0.0f
				// -1.0f, 5.0f, -1.0f
				// 0.0f, -1.0f, 0.0f
				flarrKernel.Clear();

				flarrKernelElement.Clear();
				flarrKernelElement.Add(0.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(0.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				flarrKernelElement.Clear();
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(5.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				flarrKernelElement.Clear();
				flarrKernelElement.Add(0.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(0.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				// 커널 설정
				convolution.SetKernel(flarrKernel);

				// Destination 이미지 설정 // Set the destination image
				convolution.SetDestinationImage(ref arrFliImage[(int)EType.Dst2]);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = convolution.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute convolution.");
					break;
				}

				// 커널 생성 (Edge detection)
				// -1.0f, -1.0f, -1.0f
				// -1.0f, 8.0f, -1.0f
				// -1.0f, -1.0f, -1.0f
				flarrKernel.Clear();

				flarrKernelElement.Clear();
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				flarrKernelElement.Clear();
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(8.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));

				flarrKernelElement.Clear();
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernelElement.Add(-1.0f);
				flarrKernel.Add(new List<float>(flarrKernelElement));


				// 커널 설정
				convolution.SetKernel(flarrKernel);

				// Destination 이미지 설정 // Set the destination image
				convolution.SetDestinationImage(ref arrFliImage[(int)EType.Dst3]);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = convolution.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute convolution.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.ETypeCount];

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					arrLayer[i] = arrViewImage[i].GetLayer(0);
				}

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((res = arrLayer[(int)EType.Src].DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Dst1].DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image1", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Dst2].DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image2", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Dst3].DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image3", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}


				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					arrViewImage[i].Invalidate(true);
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bRun = true;
				while(bRun)
				{
					for(int i = 0; i < (int)EType.ETypeCount; ++i)
					{
						bRun &= arrViewImage[i].IsAvailable();
					}

					Thread.Sleep(1);
				}

			}
			while(false);
		}
	}
}
﻿using System;
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
	class Gradient_Prewitt_Sobel_Filter
	{
		enum EDst
		{
			Gradient = 0,
			Prewitt,
			Sobel,
			EDstCount,
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage[] arrFliDstImage = new CFLImage[(int)EDst.EDstCount];

			for(long i = 0; i < (long)EDst.EDstCount; ++i)
				arrFliDstImage[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage[] arrViewImageDst = new CGUIViewImage[(int)EDst.EDstCount];

			for(long i = 0; i < (long)EDst.EDstCount; ++i)
				arrViewImageDst[i] = new CGUIViewImage();

			bool bError = false;

			do
			{
				CResult res;
				// Source 이미지 로드 // Load the source image
				if((res = fliSrcImage.Load("../../ExampleImages/EdgeDetection/Alphabat.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create the source image view
				if((res = viewImageSrc.Create(400, 0, 800, 400)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				for(int i = 0; i < (long)EDst.EDstCount; ++i)
				{
					// Destination 이미지를 Src 이미지와 동일한 이미지로 생성
					if((res = arrFliDstImage[i].Assign(fliSrcImage)).IsFail())
					{
						ErrorPrint(res, "Failed to assign the image file.\n");
						bError = true;
						break;
					}

					int i32X = (i + 1) % 2;
					int i32Y = (i + 1) / 2;

					// Destination 이미지 뷰 생성 // Create the destination image view
					if((res = arrViewImageDst[i].Create(i32X * 400 + 400, i32Y * 400, i32X * 400 + 400 + 400, i32Y * 400 + 400)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						bError = true;
						break;
					}

					// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
					if((res = arrViewImageDst[i].SetImagePtr(ref arrFliDstImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if((res = viewImageSrc.SynchronizePointOfView(ref arrViewImageDst[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
					if((res = viewImageSrc.SynchronizeWindow(ref arrViewImageDst[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// ROI 설정을 위한 FLRectL 생성
				CFLRect<double> flrROI = new CFLRect<double>(200, 200, 500, 500);

				// Convolution Gradient 객체 생성 // Create Convolution Gradient object
				CGradientFilter convolutionGradient = new CGradientFilter();

				// Source 이미지 설정 // Set the source image
				convolutionGradient.SetSourceImage(ref fliSrcImage);
				// Source ROI 설정 // Set the Source ROI
				convolutionGradient.SetSourceROI(flrROI);
				// Destination 이미지 설정 // Set the destination image
				convolutionGradient.SetDestinationImage(ref arrFliDstImage[(int)EDst.Gradient]);
				// Destination ROI 설정 // Set Destination ROI
				convolutionGradient.SetDestinationROI(flrROI);
				// Convolution Gradient 커널 연산 방법 설정
				convolutionGradient.SetKernelMethod(CGradientFilter.EKernel.Gradient);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = convolutionGradient.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute convolution gradient.");
					break;
				}


				// Convolution Prewitt 객체 생성 // Create Convolution Prewitt object
				CPrewittFilter convolutionPrewitt = new CPrewittFilter();

				// Source 이미지 설정 // Set the source image
				convolutionPrewitt.SetSourceImage(ref fliSrcImage);
				// Source ROI 설정 // Set the Source ROI
				convolutionPrewitt.SetSourceROI(flrROI);
				// Destination 이미지 설정 // Set the destination image
				convolutionPrewitt.SetDestinationImage(ref arrFliDstImage[(int)EDst.Prewitt]);
				// Destination ROI 설정 // Set Destination ROI
				convolutionPrewitt.SetDestinationROI(flrROI);
				// Convolution Prewitt 커널 연산 방법 설정
				convolutionPrewitt.SetKernelMethod(CPrewittFilter.EKernel.Prewitt);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = convolutionPrewitt.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute convolution prewitt.");
					break;
				}


				// Convolution Sobel 객체 생성 // Create Convolution Sobel object
				CSobelFilter convolutionSobel = new CSobelFilter();

				// Source 이미지 설정 // Set the source image
				convolutionSobel.SetSourceImage(ref fliSrcImage);
				// Source ROI 설정 // Set the Source ROI
				convolutionSobel.SetSourceROI(flrROI);
				// Destination 이미지 설정 // Set the destination image
				convolutionSobel.SetDestinationImage(ref arrFliDstImage[(int)EDst.Sobel]);
				// Destination ROI 설정 // Set Destination ROI
				convolutionSobel.SetDestinationROI(flrROI);
				// Convolution Sobel 커널 연산 방법 설정
				convolutionSobel.SetKernelMethod(CSobelFilter.EKernel.Sobel);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = convolutionSobel.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute convolution sobel.");
					break;
				}


				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);

				CGUIViewImageLayer[] arrLayerDst = new CGUIViewImageLayer[(int)EDst.EDstCount];

				for(int i = 0; i < (int)EDst.EDstCount; ++i)
				{
					arrLayerDst[i] = arrViewImageDst[i].GetLayer(0);
				}

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				arrLayerDst[(int)EDst.Gradient].Clear();
				arrLayerDst[(int)EDst.Prewitt].Clear();
				arrLayerDst[(int)EDst.Sobel].Clear();


				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure 객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능 // FLimaging's Figure objects can be displayed as a function regardless of the shape
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
				if((res = layerSrc.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");

				if((res = arrLayerDst[(int)EDst.Gradient].DrawFigureImage(flrROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");

				if((res = arrLayerDst[(int)EDst.Prewitt].DrawFigureImage(flrROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");

				if((res = arrLayerDst[(int)EDst.Sobel].DrawFigureImage(flrROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");


				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((res = layerSrc.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = arrLayerDst[(int)EDst.Gradient].DrawTextCanvas(new CFLPoint<double>(0, 0), "Gradient Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = arrLayerDst[(int)EDst.Prewitt].DrawTextCanvas(new CFLPoint<double>(0, 0), "Prewitt Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = arrLayerDst[(int)EDst.Sobel].DrawTextCanvas(new CFLPoint<double>(0, 0), "Sobel Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");


				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				arrViewImageDst[(int)EDst.Gradient].Invalidate(true);
				arrViewImageDst[(int)EDst.Prewitt].Invalidate(true);
				arrViewImageDst[(int)EDst.Sobel].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && arrViewImageDst[(int)EDst.Gradient].IsAvailable()
					&& arrViewImageDst[(int)EDst.Prewitt].IsAvailable() && arrViewImageDst[(int)EDst.Sobel].IsAvailable())
					Thread.Sleep(1);

			}
			while(false);
		}
	}
}
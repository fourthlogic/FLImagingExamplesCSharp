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

namespace FLImagingExamplesCSharp
{
	class GaussFilter
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
			Destination4,
			ETypeCount,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrFliImage[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrViewImage[i] = new CGUIViewImage();

			do
			{
				// 이미지 로드 // Load image
                if ((res = arrFliImage[(int)EType.Source].Load("../../ExampleImages/NoiseImage/NoiseImage1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				bool bError = false;

				for(int i = (int)EType.Destination1; i < (int)EType.ETypeCount; ++i)
				{
					// Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
					if((res = arrFliImage[i].Assign(arrFliImage[(int)EType.Source])).IsFail())
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
					int x = i % 3;
					int y = i / 3;

					// 이미지 뷰 생성 // Create image view
					if((res = arrViewImage[i].Create(x * 400 + 400, y * 400, x * 400 + 400 + 400, y * 400 + 400)).IsFail())
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

					if(i == (int)EType.Source)
						continue;

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if((res = arrViewImage[(int)EType.Source].SynchronizePointOfView(ref arrViewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
					if((res = arrViewImage[(int)EType.Source].SynchronizeWindow(ref arrViewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// ROI 설정을 위한 CFLRect 객체 생성 // Create a CFLRect object for setting ROI
				CFLRect<int> flrROI = new CFLRect<int>(200, 200, 500, 500);

				// Gauss Filter 객체 생성 // Create Gauss Filter object
				CGaussFilter gaussFilter = new CGaussFilter();
				// Source 이미지 설정 // Set the source image
				gaussFilter.SetSourceImage(ref arrFliImage[(int)EType.Source]);
				// Source ROI 설정 // Set the Source ROI
				gaussFilter.SetSourceROI(flrROI);

				// Destination1 이미지 설정 // Set the destination1 image
				gaussFilter.SetDestinationImage(ref arrFliImage[(int)EType.Destination1]);
				// Destination1 ROI 설정 // Set the destination1 ROI
				gaussFilter.SetDestinationROI(flrROI);

				// Sigma 값 설정 // Set the sigma value
				gaussFilter.SetSigma(0.5);
				// kernel 크기 설정 // Set Kernel Size
				gaussFilter.SetKernel(3);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = gaussFilter.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				// Destination2 이미지 설정 // Set the destination2 image
				gaussFilter.SetDestinationImage(ref arrFliImage[(int)EType.Destination2]);
				// Destination2 ROI 설정 // Set the destination2 ROI
				gaussFilter.SetDestinationROI(flrROI);

				// Sigma 값 설정 // Set the sigma value
				gaussFilter.SetSigma(1);
				// kernel 크기 설정 // Set Kernel Size
				gaussFilter.SetKernel(7);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = gaussFilter.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				// Destination3 이미지 설정 // Set the destination3 image
				gaussFilter.SetDestinationImage(ref arrFliImage[(int)EType.Destination3]);
				// Destination3 ROI 설정 // Set the destination3 ROI
				gaussFilter.SetDestinationROI(flrROI);

				// Sigma 값 설정 // Set the sigma value
				gaussFilter.SetSigma(3);
				// kernel 크기 설정 // Set Kernel Size
				gaussFilter.SetKernel(19);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = gaussFilter.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				// Destination4 이미지 설정 // Set the destination4 image
				gaussFilter.SetDestinationImage(ref arrFliImage[(int)EType.Destination4]);
				// Destination4 ROI 설정 // Set the destination4 ROI
				gaussFilter.SetDestinationROI(flrROI);

				// Sigma 값 설정 // Set the sigma value
				gaussFilter.SetSigma(5);
				// kernel 크기 설정 // Set Kernel Size
				gaussFilter.SetKernel(31);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = gaussFilter.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
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

					// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
					// FLImaging의 Figure 객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능 // FLimaging's Figure objects can be displayed as a function regardless of the shape
					// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
					// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
					// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
					if((res = arrLayer[i].DrawFigureImage(flrROI, EColor.LIME)).IsFail())
						ErrorPrint(res, "Failed to draw figure.\n");
				}

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpZero = new CFLPoint<double>(0, 0);

				if((res = arrLayer[(int)EType.Source].DrawTextCanvas(flpZero, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination1].DrawTextCanvas(flpZero, "Destination1 Image (Sigma 0.5)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination2].DrawTextCanvas(flpZero, "Destination2 Image (Sigma 1)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination3].DrawTextCanvas(flpZero, "Destination3 Image (Sigma 3)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination4].DrawTextCanvas(flpZero, "Destination4 Image (Sigma 5)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
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
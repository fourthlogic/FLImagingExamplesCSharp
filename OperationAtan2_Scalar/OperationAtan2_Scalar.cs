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
	class OperationAtan2_Scalar
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
			CFLImage[] arrFliImage = new CFLImage[3];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[3];

			for(int i = 0; i < 3; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = (arrFliImage[0].Load("../../ExampleImages/OperationAtan2/Flower.flif"))).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = (arrViewImage[0].Create(100, 0, 612, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = (arrViewImage[1].Create(612, 0, 1124, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = (arrViewImage[2].Create(1124, 0, 1636, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				bool bError = false;

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				for(int i = 0; i < 3; ++i)
				{
					if((res = (arrViewImage[i].SetImagePtr(ref arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = (arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = (arrViewImage[0].SynchronizePointOfView(ref arrViewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = (arrViewImage[0].SynchronizeWindow(ref arrViewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = (arrViewImage[0].SynchronizeWindow(ref arrViewImage[2]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				CMultiVar<double> mvScalr = new CMultiVar<double>(1, 1, 1);
				CMultiVar<double> mvScalr2 = new CMultiVar<double>(65535, 65535, 65535);

				// Operation atan2 객체 생성 // Create Atan2 object
				COperationAtan2 atan2 = new COperationAtan2();
				// Source 이미지 설정 // Set source image
				atan2.SetSourceImage(ref arrFliImage[0]);
				// Destination 이미지 설정 // Set destination image 
				atan2.SetDestinationImage(ref arrFliImage[1]);
				// 연산 방식 설정 // Set operation source
				atan2.SetOperationSource(EOperationSource.Scalar);
				// Scalar 값 설정 // Set Scalar value
				atan2.SetScalarValue(mvScalr);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (atan2.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation atan2.");
					break;
				}

				// Destination 이미지 설정 // Set destination image 
				atan2.SetDestinationImage(ref arrFliImage[2]);
				// 연산 방식 설정 // Set operation source
				atan2.SetOperationSource(EOperationSource.Scalar);
				// Scalar 값 설정 // Set Scalar value
				atan2.SetScalarValue(mvScalr2);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (atan2.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation atan2.");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[3];
				arrLayer[0] = new CGUIViewImageLayer();
				arrLayer[1] = new CGUIViewImageLayer();
				arrLayer[2] = new CGUIViewImageLayer();

				for(int i = 0; i < 3; ++i)
				{
					// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
					// 따로 해제할 필요 없음 // No need to release separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
					arrLayer[i].Clear();
				}

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				TPoint<double> tpPosition = new TPoint<double>(0, 0);

				if((res = (arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 17))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[1].DrawTextCanvas(tpPosition, "Destination1 Image(Atan2 1, 1, 1)", EColor.YELLOW, EColor.BLACK, 17))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[2].DrawTextCanvas(tpPosition, "Destination2 Image (Atan2 65535, 65535, 65535)", EColor.YELLOW, EColor.BLACK, 17))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				arrViewImage[0].Invalidate(true);
				arrViewImage[1].Invalidate(true);
				arrViewImage[2].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImage[0].IsAvailable()
					  && arrViewImage[1].IsAvailable()
					  && arrViewImage[2].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

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
    class OperationBitwiseNor_Image
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
	        CFLImage fliSourceImage = new CFLImage();
            CFLImage fliOperandImage = new CFLImage();
            CFLImage fliDestinationImage = new CFLImage();

	        // 이미지 뷰 선언 // Declare the image view
	        CGUIViewImage viewImageSource = new CGUIViewImage();
            CGUIViewImage viewImageOperand = new CGUIViewImage();
            CGUIViewImage viewImageDestination = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
                // Source 이미지 로드 // Load the source image
                if ((res = fliSourceImage.Load("../../ExampleImages/OperationBitwiseNor/circle.flif")).IsFail())
				{
                    ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

                // Operand 이미지 로드 // Loads the operand image
                if ((res = fliOperandImage.Load("../../ExampleImages/OperationBitwiseNor/square.flif")).IsFail())
				{
                    ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

                // Destination 이미지 생성
                if ((res = fliDestinationImage.Create(250, 250)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image file. \n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res = viewImageSource.Create(100, 0, 600, 500)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Operand 이미지 뷰 생성 // Create operand image view
                if ((res = viewImageOperand.Create(600, 0, 1100, 500)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if ((res = viewImageDestination.Create(1100, 0, 1600, 500)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Source 이미지 뷰와 Operand 이미지 뷰의 시점을 동기화 한다
                if ((res = viewImageSource.SynchronizePointOfView(ref viewImageOperand)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageSource.SynchronizePointOfView(ref viewImageDestination)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if ((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // Operand 이미지 뷰에 이미지를 디스플레이
                if ((res = viewImageOperand.SetImagePtr(ref fliOperandImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // Destiantion 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if ((res = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSource.SynchronizeWindow(ref viewImageOperand)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSource.SynchronizeWindow(ref viewImageDestination)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // OperationBitwiseNor 객체 생성 // Create OperationBitwiseNor object
                COperationBitwiseNor bitnor = new COperationBitwiseNor();

                // Source 이미지 설정 // Set the source image
                bitnor.SetSourceImage(ref fliSourceImage);

                // Operand 이미지 설정 // Set the operand image
        		bitnor.SetOperandImage(ref fliOperandImage);

		        // Destination 이미지 설정 // Set the destination image
		        bitnor.SetDestinationImage(ref fliDestinationImage);

                // Image Operation 소스로 설정 // Set Operation Source to image
                bitnor.SetOperationSource(EOperationSource.Image);

                // 공백 색상 칠하기 모드 해제
                // 결과 이미지가 이미 존재할 경우 연산되지 않은 영역을 공백 색상으로 칠하지 않고 원본 그대로 둔다.
                bitnor.EnableFillBlankColorMode(false);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = bitnor.Execute()).IsFail())
				{
                    ErrorPrint(res, "Failed to execute Operation BitwiseNor.");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                CGUIViewImageLayer layerOperand = viewImageOperand.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerOperand.Clear();
                layerDestination.Clear();

                // 이미지 뷰 정보 표시 // Display image view information
                CFLPoint<double> flpPoint = new CFLPoint<double> (0, 0);

                if ((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text. \n");
		        	break;
		        }

                if ((res = layerOperand.DrawTextCanvas(flpPoint, "Operand Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text. \n");
		        	break;
		        }

                if ((res = layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text. \n");
		        	break;
		        }

                // 이미지 뷰를 갱신 // Update image view
                viewImageSource.Invalidate(true);
                viewImageOperand.Invalidate(true);
                viewImageDestination.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSource.IsAvailable() && viewImageOperand.IsAvailable() && viewImageDestination.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

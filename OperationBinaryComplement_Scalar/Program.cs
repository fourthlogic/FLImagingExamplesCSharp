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

namespace OperationBinaryComplement
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
            CFLImage fliSourceImage = new CFLImage();
            CFLImage fliDestinationImage = new CFLImage();

	        // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageSrc = new CGUIViewImage();
            CGUIViewImage viewImageDst = new CGUIViewImage();

			CResult eResult = new CResult();

			do
	        {
                // Source 이미지 로드 // Load the source image
                if ((eResult = fliSourceImage.Load("../../ExampleImages/OperationBinaryComplement/gradation.flif")).IsFail())
		        {
                    ErrorPrint(eResult, "Failed to load the image file. \n");
		        	break;
		        }

                // Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
                if ((eResult = fliDestinationImage.Assign(fliSourceImage)).IsFail())
		        {
                    ErrorPrint(eResult, "Failed to load the image file. \n");
                    break;
		        }

                // Source 이미지 뷰 생성 // Create source image view
                if ((eResult = viewImageSrc.Create(100, 0, 612, 512)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view. \n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create the destination image view
                if ((eResult = viewImageDst.Create(612, 0, 1124, 512)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((eResult = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view. \n");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if ((eResult = viewImageSrc.SetImagePtr(ref fliSourceImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view. \n");
                    break;
                }

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if ((eResult = viewImageDst.SetImagePtr(ref fliDestinationImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((eResult = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize window. \n");
                    break;
                }

                // OperationBitwiseAnd 객체 생성 // Create OperationBitwiseAnd object
                COperationBinaryComplement bc = new COperationBinaryComplement();

                // Source 이미지 설정 // Set the source image
                bc.SetSourceImage(ref fliDestinationImage);

                // ROI 범위 설정 // Set the ROI value
                CFLCircle<double> flcSourceROI = new CFLCircle<double>(128, 128, 80, 0, 0, 360, EArcClosingMethod.EachOther);

                // Source 이미지의 ROI 지정 // Set the Source ROI
                bc.SetSourceROI(flcSourceROI);

                // Scalar Operation 소스로 설정 // Set Operation Source to scalar
                bc.SetOperationSource(EOperationSource.Scalar);

                // 스칼라 값 지정 // Set the Scalar value
                bc.SetScalarValue(1);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((eResult = bc.Execute()).IsFail())
                {
                    ErrorPrint(eResult, "Failed to execute Operation Binary Complement. \n");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSrc.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDst.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerDestination.Clear();

                if ((eResult = layerSource.DrawFigureImage(flcSourceROI, EColor.LIME)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw figure. \n");
                    break;
                }

                if ((eResult = layerDestination.DrawFigureImage(flcSourceROI, EColor.LIME)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw figure. \n");
                    break;
                }

                // 이미지 뷰 정보 표시 // Display image view information
                CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

		        if ((eResult = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
		        {
                    ErrorPrint(eResult, "Failed to draw text. \n");
                    break;
		        }

		        if ((eResult = layerDestination.DrawTextCanvas(flpPoint, "Source & Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
		        {
                    ErrorPrint(eResult, "Failed to draw text. \n");
		        	break;
		        }

		        // 이미지 뷰를 갱신 // Update image view
		        viewImageSrc.Invalidate(true);
		        viewImageDst.Invalidate(true);

		        // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
		        while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}
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

namespace OperationBitwiseXnor
{
    class OperationBitwiseXnor_Scalar
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
            CFLImage fliDestinationImage1 = new CFLImage();
            CFLImage fliDestinationImage2 = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageSrc = new CGUIViewImage();
            CGUIViewImage viewImageDst1 = new CGUIViewImage();
            CGUIViewImage viewImageDst2 = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
                // Source 이미지 로드 // Load the source image
		        if((res = fliSourceImage.Load("../../ExampleImages/OperationBitwiseXnor/Cat.flif")).IsFail())
				{
                    ErrorPrint(res, "Failed to load the image file. \n");
		        	break;
		        }

                // Destination1 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination1 image as same as source image
                if ((res = fliDestinationImage1.Assign(fliSourceImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

                // Destination2 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination2 image as same as source image
                if ((res = fliDestinationImage2.Assign(fliSourceImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res = viewImageSrc.Create(100, 0, 612, 512)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Destination1 이미지 뷰 생성 // Create destination1 image view
                if ((res = viewImageDst1.Create(612, 0, 1124, 512)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Destination2 이미지 뷰 생성 // Create destination2 image view
                if ((res = viewImageDst2.Create(1124, 0, 1636, 512)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if ((res = viewImageSrc.SetImagePtr(ref fliSourceImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // Destination1 이미지 뷰에 이미지를 디스플레이  // Display the image in the destination1 image view
                if ((res = viewImageDst1.SetImagePtr(ref fliDestinationImage1)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // Destination2 이미지 뷰에 이미지를 디스플레이  // Display the image in the destination2 image view
                if ((res = viewImageDst2.SetImagePtr(ref fliDestinationImage2)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst1)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst2)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImageDst1)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImageDst2)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // OperationBitwiseXnor 객체 생성 // Create OperationBitwiseXnor object
                COperationBitwiseXnor bitwiseXnor = new COperationBitwiseXnor();

				// Source 이미지 설정 // Set the source image
				bitwiseXnor.SetSourceImage(ref fliSourceImage);

				// Destination 이미지 설정 // Set the destination image
				bitwiseXnor.SetDestinationImage(ref fliDestinationImage1);

                // Image Operation 소스로 설정 // Set Operation Source to image
                bitwiseXnor.SetOperationSource(EOperationSource.Scalar);

                // BitwiseXnor 값 설정 // Set BitwiseXnor value
                CMultiVar<double> mvScalar = new CMultiVar<double>(64, 64, 64);
                bitwiseXnor.SetScalarValue(mvScalar);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = bitwiseXnor.Execute()).IsFail())
				{
                    ErrorPrint(res, "Failed to execute Operation BitwiseXnor. \n");
                    break;
                }

                // Destination 이미지 설정 // Set the destination image
                bitwiseXnor.SetDestinationImage(ref fliDestinationImage2);

                // Image Operation 소스로 설정 // Set Operation Source to image
                bitwiseXnor.SetOperationSource(EOperationSource.Scalar);

                // BitwiseXnor 값 설정 // Set BitwiseXnor value
                mvScalar = new CMultiVar<double>(128, 128, 128);
                bitwiseXnor.SetScalarValue(mvScalar);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = bitwiseXnor.Execute()).IsFail())
				{
                    ErrorPrint(res, "Failed to execute Operation BitwiseXnor. \n");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSrc.GetLayer(0);
                CGUIViewImageLayer layerDestination1 = viewImageDst1.GetLayer(0);
                CGUIViewImageLayer layerDestination2 = viewImageDst2.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerDestination1.Clear();
                layerDestination2.Clear();

                // 이미지 뷰 정보 표시 // Display image view information
                CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

                if ((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text. \n");
                    break;
                }

                if ((res = layerDestination1.DrawTextCanvas(flpPoint, "Destination1 Image(BitwiseXnor 64)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text. \n");
                    break;
                }

                if ((res = layerDestination2.DrawTextCanvas(flpPoint, "Destination2 Image(BitwiseXnor 128)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text. \n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                viewImageSrc.Invalidate(true);
                viewImageDst1.Invalidate(true);
                viewImageDst2.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSrc.IsAvailable() && viewImageDst1.IsAvailable() && viewImageDst2.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

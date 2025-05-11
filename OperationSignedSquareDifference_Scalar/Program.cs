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

namespace OperationSignedSquareDifference
{
    class Program
    {
		public static void ErrorPrint(CResult res, string str)
		{
			if( str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", res.GetResultCode(), res.GetString());
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

	        do
			{
				// 알고리즘 동작 결과 // Algorithm execution result
				CResult res = new CResult();

				// Source 이미지 로드 // Load the source image
				if((res = fliSourceImage.Load("../../ExampleImages/OperationSignedSquareDifference/Gradation.flif")).IsFail())
		        {
                    ErrorPrint(res,"Failed to load the image file. \n");
		        	break;
		        }

                // Source 이미지 뷰 생성 // Create source image view
                if((res = viewImageSrc.Create(100, 0, 612, 512)).IsFail())
                {
                    ErrorPrint(res,"Failed to create the image view. \n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if((res = viewImageDst.Create(612, 0, 1124, 512)).IsFail())
                {
                    ErrorPrint(res,"Failed to create the image view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
	        	if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
	        	{
                    ErrorPrint(res,"Failed to synchronize view. \n");
	        		break;
	        	}

	        	// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
	        	if((res = viewImageSrc.SetImagePtr(ref fliSourceImage)).IsFail())
	        	{
                    ErrorPrint(res,"Failed to set image object on the image view. \n");
	        		break;
	        	}

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
	        	if((res = viewImageDst.SetImagePtr(ref fliDestinationImage)).IsFail())
	        	{
                    ErrorPrint(res,"Failed to set image object on the image view.\n");
	        		break;
	        	}

	        	// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
	        	if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
	        	{
                    ErrorPrint(res,"Failed to synchronize window. \n");
	        		break;
	        	}

	        	// Operation SignedSquareDifference 객체 생성 // Create Operation SignedSquareDifference object
	        	COperationSignedSquareDifference signedSquareDifference = new COperationSignedSquareDifference();

	        	// Source 이미지 설정 // Set the source image
	        	signedSquareDifference.SetSourceImage(ref fliSourceImage);

	        	// Destination 이미지 설정 // Set the destination image
	        	signedSquareDifference.SetDestinationImage(ref fliDestinationImage);

				// Operand Overflow Method 설정 // Set the overflow method
				signedSquareDifference.SetOverflowMethod(EOverflowMethod.Wrapping);

				// Scalar Operation 모드로 설정 // Set operation mode to scalar
				signedSquareDifference.SetOperationSource(EOperationSource.Scalar);

				// 스칼라 색상 지정 // Set scalar operand color
				CMultiVar<double> mvScalar = new CMultiVar<double> (0);
	        	signedSquareDifference.SetScalarValue(mvScalar);

				// 항상 공백 영역을 지정한 색으로 채우도록 설정 // Set the blank area to always be filled with a specified color
				signedSquareDifference.EnableFillBlankColorMode(true);

	        	// 공백 색상 지정 // Set blank color
                CMultiVar<double> mvBlank = new CMultiVar<double> (100);
	        	signedSquareDifference.SetBlankColor(mvBlank);

	        	// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
	        	if((res = signedSquareDifference.Execute()).IsFail())
	        	{
                    ErrorPrint(res,"Failed to execute Operation SignedSquareDifference. \n");
	        		break;
	        	}

	        	// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
	        	// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSrc.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDst.GetLayer(0);

	        	// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
	        	layerSource.Clear();
	        	layerDestination.Clear();
	        	
	        	// 이미지 뷰 정보 표시 // Display image view information
                CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

	        	if((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
	        	{
                    ErrorPrint(res,"Failed to draw text. \n");
                    break;
	        	}

	        	if((res = layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
	        	{
                    ErrorPrint(res,"Failed to draw text. \n");
	        		break;
	        	}

	        	// 이미지 뷰를 갱신 // Update image view
	        	viewImageSrc.Invalidate(true);
	        	viewImageDst.Invalidate(true);

	        	// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
	        		Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

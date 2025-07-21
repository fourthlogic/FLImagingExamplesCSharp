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

namespace PagePooling
{
    class PagePooling_MaxGaussian
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
            CFLImage fliIndexImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage(); ;
			CGUIViewImage viewImageDestination = new CGUIViewImage(); ;
            CGUIViewImage viewImageIndex = new CGUIViewImage();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
                // Source 이미지 로드 // Load the source image
                if ((res = fliSourceImage.Load("../../ExampleImages/PagePooling/Multiple File_Max.flif")).IsFail())
                {
					ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

				// Source 이미지 뷰 생성 // Create source image view
				if ((res = viewImageSource.Create(200, 0, 712, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if ((res = viewImageDestination.Create(712, 0, 1224, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Index 이미지 뷰 생성 // Create index image view
				if ((res = viewImageIndex.Create(1224, 0, 1736, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if ((res = viewImageSource.SynchronizePointOfView(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageDestination.SynchronizePointOfView(ref viewImageIndex)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSource.SynchronizeWindow(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageDestination.SynchronizeWindow(ref viewImageIndex)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if ((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
                {
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if ((res = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
                {
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                // Index 이미지 뷰에 이미지를 디스플레이 // Display the image in the Index image view
                if((res = viewImageIndex.SetImagePtr(ref fliIndexImage)).IsFail())
                {
                    ErrorPrint(res, "Failed to set image object on the image view. \n");
                    break;
                }

                
                // Page Pooling 객체 생성 // Create Page Pooling object
                CPagePooling pagePooling = new CPagePooling();

                // Source 이미지 설정 // Set the source image
                pagePooling.SetSourceImage(ref fliSourceImage);

                // Destination 이미지 설정 // Set the destination image
                pagePooling.SetDestinationImage(ref fliDestinationImage);

				// Index 이미지 // Index image
				//	- 각 픽셀별로 추출한 결과 값이 위치한 페이지 인덱스 값을 대응되는 좌표의 픽셀로 출력합니다. // For each pixel, output the page index value where the resulting value is located to the pixel of the corresponding coordinates.
				//	- Sampling 메소드가 Min Gaussian, Max Gaussian 모드인 경우 출력되는 인덱스 이미지는 각각 Min, Max 모드로 동작할 때 출력되는 인덱스 이미지와 동일합니다. // If the sampling method is in Min Gaussian and Max Gaussian modes, the output index image is the same as the output index image when operating in Min and Max modes, respectively.
				//	- Sampling 메소드가 Mean 모드인 경우는 인덱스 이미지 출력을 지원하지 않습니다. // Index image output is not supported when the Sampling method is in Mean mode.
				//	- 추출할 결과 값이 여러 페이지에 동일하게 존재할 경우, 가장 앞의 인덱스를 출력합니다. // Outputs the leading index if the resulting values to be extracted are equally present on multiple pages.
				//	- SetIndexImage 는 SetSourceImage 나 SetDestinationImage 에서 설정한 이미지와 동일하면 동작하지 않습니다. // - SetIndexImage will not work if it is the same as the image set in SetSourceImage or SetDestinationImage.
				//	- Index Image 를 지정하지 않을 경우 인덱스 이미지를 출력하지 않는 모드로 동작합니다. // If SetIndexImage is not specified, it operates in a mode that does not output the index image.
				//	- 인덱스 이미지 추출은 최대 65535 장 까지만 지원됩니다. // Index image extraction is supported up to 65535 pages
				//	- Source ROI 영역 밖에 해당하는 인덱스는 무효 값으로 8bit 인덱스 이미지에서는 255, 16bit 인덱스 이미지에서는 65535 가 입력됩니다. // Indexes outside the Source ROI area are invalid values, with 255 for an 8-bit index image and 65535 for a 16-bit index image.

				// Index 이미지 설정 // Set the index image
				pagePooling.SetIndexImage(ref fliIndexImage);

				// Sampling 메소드 설정 // Set the sampling method
				//	- Max : 입력된 이미지 가운데 최대 값을 출력합니다. // Max : Outputs the maximum value of the entered image.
				//	- MaxGaussian : 입력된 이미지 가운데 가장 앞 쪽 인덱스에 위치한 최대 값을 기준으로 가우시안 값을 출력합니다. // MaxGaussian : Outputs the Gaussian value based on the maximum value located in the leading index of the entered image.
				//	- Min : 입력된 이미지 가운데 최소 값을 출력합니다. // Min : Outputs the minimum value of the entered image.
				//	- MinGaussian : 입력된 이미지 가운데 가장 앞 쪽 인덱스에 위치한 최소 값을 기준으로 가우시안 값을 출력합니다. // MinGaussian : Outputs the Gaussian value based on the minimum value located in the leading index of the entered image.
				//	- Mean : 입력된 이미지들의 평균 값을 출력합니다. (최대 16843009 장 까지 지원됩니다.) // Mean: Outputs the average value of the entered images. (Up to 16843009 pages are supported.)
				pagePooling.SetSamplingMethod(CPagePooling.ESamplingMethod.MaxGaussian);

                if((res = pagePooling.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute Page Pooling.\n");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);
                CGUIViewImageLayer layerIndex = viewImageIndex.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerDestination.Clear();
                layerIndex.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if ((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
					ErrorPrint(res, "Failed to draw text. \n");
                    break;
                }

                if ((res = layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
					ErrorPrint(res, "Failed to draw text. \n");
                    break;
                }

                if ((res = layerIndex.DrawTextCanvas(flpPoint, "Index Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text. \n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                viewImageSource.Invalidate(true);
                viewImageDestination.Invalidate(true);
                viewImageIndex.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSource.IsAvailable() && viewImageDestination.IsAvailable() && viewImageIndex.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

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
            CFLImage fliIndexImage = new CFLImage();
            CFLImage fliDestinationImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
            CGUIViewImage viewImageIndex = new CGUIViewImage();
            CGUIViewImage viewImageDestination = new CGUIViewImage();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
                // Source 이미지 로드 // Load the source image
                if ((res = fliSourceImage.Load("../../ExampleImages/PagePixelPicker/MultiFile_Normal.flif")).IsFail())
                {
					ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

                // Index 이미지 로드 // Load the index image
                if ((res = fliIndexImage.Load("../../ExampleImages/PagePixelPicker/IndexMap.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file. \n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res = viewImageSource.Create(400, 0, 900, 500)).IsFail())
                {
					ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Index 이미지 뷰 생성 // Create index image view
                if ((res = viewImageIndex.Create(900, 0, 1400, 500)).IsFail())
                {
					ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if ((res = viewImageDestination.Create(1400, 0, 1900, 500)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageSource.SynchronizePointOfView(viewImageDestination)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageDestination.SynchronizePointOfView(viewImageIndex)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSource.SynchronizeWindow(viewImageDestination)).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageDestination.SynchronizeWindow(viewImageIndex)).IsFail())
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

                // Index 이미지 뷰에 이미지를 디스플레이 // Display the image in the Index image view
                if ((res = viewImageIndex.SetImagePtr(ref fliIndexImage)).IsFail())
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

                // Page Pixel Picker 객체 생성 // Create Page Pixel Picker object
                CPagePixelPicker pagePixelPicker = new CPagePixelPicker();

                // Source 이미지 설정 // Set the source image
                // 모든 Source 이미지는 동일한 사이즈와 포맷을 가져야합니다.
                pagePixelPicker.SetSourceImage(ref fliSourceImage);

                // Index 이미지 설정 // Set the index image
                // 8bit 와 16 bit 를 지원하며 반드시 입력되어야 합니다.
                // Index 이미지는 Source 이미지나 Destination 이미지와 다르게 설정해야 합니다. 
                // Index 이미지의 각 픽셀 값은 입력된 Source 이미지의 페이지 인덱스를 의미합니다.
                pagePixelPicker.SetIndexImage(ref fliIndexImage);

                // Destination 이미지 설정 // Set the destination image
                pagePixelPicker.SetDestinationImage(ref fliDestinationImage);

                // Source ROI, Source Pivot, Destination ROI, Destination Pivot 기능을 지원합니다.
                // ROI 및 Pivot 은 Image Operation Scalar 연산과 동일한 동작을 수행합니다.

                // Blank Color 설정
                // 기본적으로 Image Operation 의 Blank Color 와 동일한 동작을 합니다. 
                // Index Image 픽셀 값에 해당되는 페이지 인덱스가 존재하지 않을 경우 Blank color 로 채워집니다.
                pagePixelPicker.SetBlankColor(200);

                //EnableFillBlankColorMode 기능을 지원하며, Image Operation Scalar 연산과 동일한 동작을 수행합니다.
                //pagePixelPicker.EnableFillBlankColorMode(true);

                // Sampling 메소드 설정
                //	- Normal : Index 이미지 각 픽셀과 좌표에 대응되는 Source 이미지 픽셀 색상 값을 출력합니다.
                //	- Gaussian : Index 이미지 각 픽셀과 좌표에 대응되는 Source 이미지와 전 후 페이지 색상 값으로 가우시안 값을 계산하여 출력합니다.
                pagePixelPicker.SetSamplingMethod(CPagePixelPicker.ESamplingMethod.Normal);

                // 앞서 입력된 파라미터 대로 Page Pixel Picker 수행
                if((res = pagePixelPicker.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute Page Pixel Picker.\n");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                CGUIViewImageLayer layerIndex = viewImageIndex.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerIndex.Clear();
                layerDestination.Clear();

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

                if ((res = layerIndex.DrawTextCanvas(flpPoint, "Index Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
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
                viewImageIndex.Invalidate(true);
                viewImageDestination.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSource.IsAvailable() && viewImageDestination.IsAvailable() && viewImageIndex.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

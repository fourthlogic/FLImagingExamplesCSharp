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

namespace FLImagingExamplesCSharp
{
    class HistogramsOfOrientedGradients_PredictionImage
	{
        public static void ErrorPrint(CResult cResult, string str)
		{
            if (str.Length > 1)
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

            // 이미지 객체 선언 // Declare image object
            CFLImage fliSrcImage = new CFLImage();
            CFLImage fliDstImage = new CFLImage();

            // 이미지 뷰 선언 // Declare image view
            CGUIViewImage viewImageSrc = new CGUIViewImage();
            CGUIViewImage viewImageDst = new CGUIViewImage();

            // 알고리즘 동작 결과 // Algorithm execution result
            CResult res = new CResult();

            do
			{
                // Source 이미지 로드 // Load the source image
                if ((res = fliSrcImage.Load("../../ExampleImages/OperationCompare/candle.flif")).IsFail())
				{
                    ErrorPrint(res, "Failed to load the image file.");
                    break;
                }

                // Destination이미지를 Src 이미지와 동일한 이미지로 생성 // Create the destination Image same as source image
                if ((res = fliDstImage.Assign(fliSrcImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to assign the image file.\n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create the source image view
                if ((res = viewImageSrc.Create(400, 0, 912, 484)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if ((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view.");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create the destination image view
                if ((res = viewImageDst.Create(912, 0, 1424, 484)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if ((res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view.\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // HOG 객체 생성 // Create HOG object
                CHistogramsOfOrientedGradients histogramsOfOrientedGradients = new CHistogramsOfOrientedGradients();

                // ROI 범위 생성 // Create ROI area
                CFLRect<int> flrROI = new CFLRect<int>(200, 10, 300, 200);

                // Source 이미지 설정 // Set the source image
                if ((res = histogramsOfOrientedGradients.SetSourceImage(ref fliSrcImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set Source Image.");
                    break;
                }

                // 연산할 ROI 설정 // Set ROI to Calculate
                if ((res = histogramsOfOrientedGradients.SetSourceROI(flrROI)).IsFail())
				{
                    ErrorPrint(res, "Failed to set Source ROI.");
                    break;
                }

                // Destination 이미지 설정 // Set the destination image
                if ((res = histogramsOfOrientedGradients.SetSourceImage(ref fliDstImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set Source Image.");
                    break;
                }

                // Cell Size 설정 // Set Cell Size
                histogramsOfOrientedGradients.SetCellSize(4);

                // Block Size 설정 // Set Block Size
                histogramsOfOrientedGradients.SetBlockSize(3);

                // 비주얼 출력 타입 예측 이미지로 설정 // Set Prediction Image to visual result type
                histogramsOfOrientedGradients.SetVisualResultType(CHistogramsOfOrientedGradients.EVisualResultType.PredictionImage);

                // 알고리즘 수행 // Execute the algorithm
                if ((res = histogramsOfOrientedGradients.Execute()).IsFail())
				{
                    ErrorPrint(res, "Failed to execute Histograms Of Oriented Gradients.");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
                CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSrc.Clear();
                layerDst.Clear();

                // ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
                if ((res = layerSrc.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
                    break;
                }

                if ((res = layerDst.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
                    break;
                }

                // View 정보를 디스플레이 한다. // Display view information
                // 아래 함수 DrawTextCanvas 는 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
                // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
                // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                // Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
                //                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
                TPoint<double> tpPosition = new TPoint<double>(0, 0);
                if ((res = layerSrc.DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                if ((res = layerDst.DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                // 이미지 뷰를 갱신 합니다. // Update image view
                viewImageSrc.Invalidate(true);
                viewImageDst.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
                    CThreadUtilities.Sleep(1);
            }
            while (false);
        }
    }
}

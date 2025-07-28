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
	class HighDynamicRange
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult result = new CResult();

			do
			{
				// 이미지 로드 // Load image
                if (fliSrcImage.Load("../../ExampleImages/HighDynamicRange/").IsFail())
				{
					ErrorPrint(result, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// 이미지 뷰 생성 // Create image view
				if(viewImageSrc.Create(400, 0, 1012, 550).IsFail())
				{
					ErrorPrint(result, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if(viewImageSrc.SetImagePtr(ref fliSrcImage).IsFail())
				{
					ErrorPrint(result, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if(viewImageDst.Create(1012, 0, 1624, 550).IsFail())
				{
					ErrorPrint(result, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if(viewImageDst.SetImagePtr(ref fliDstImage).IsFail())
				{
					ErrorPrint(result, "Failed to set image object on the image view.\n");
					break;
				}

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if (viewImageSrc.SynchronizePointOfView(ref viewImageDst).IsFail())
				{
                    ErrorPrint(result, "Failed to synchronize view. \n");
                    break;
                }

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if(viewImageSrc.ZoomFit().IsFail())
				{
					ErrorPrint(result, "Failed to zoom fit\n");
					break;
				}

                viewImageSrc.SetFixThumbnailView(true);

				CResult ipResult;

				// HighDynamicRange 객체 생성 // Create HighDynamicRange object
				CHighDynamicRange highDynamicRange = new CHighDynamicRange();

				// 처리할 이미지 설정 // Set the source image
				highDynamicRange.SetSourceImage(ref fliSrcImage);
				// Destination 이미지 설정 // Set the destination image
				highDynamicRange.SetDestinationImage(ref fliDstImage);
				// Rate Value 설정 // Set the rate value
				highDynamicRange.SetRateValue(0.8);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((result = ipResult = highDynamicRange.Execute()).IsFail())
				{
					ErrorPrint(result, "Failed to execute HighDynamicRange.");
					Console.WriteLine(ipResult.GetString());
					Console.WriteLine("\n");
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if(viewImageDst.ZoomFit().IsFail())
				{
					ErrorPrint(result, "Failed to zoom fit\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				TPoint<double> flp = new TPoint<double>(0, 0);

				if(layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20).IsFail())
				{
					ErrorPrint(result, "Failed to draw text\n");
					break;
				}

				if(layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20).IsFail())
				{
					ErrorPrint(result, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

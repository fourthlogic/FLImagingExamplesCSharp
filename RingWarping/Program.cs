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

namespace RingWarping
{
    class Program
    {
		public static void ErrorPrint(CResult res, string str)
		{
			if (str.Length > 1)
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
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageDestination = new CGUIViewImage();

			do
	        {
				// 알고리즘 동작 결과 // Algorithm execution result
				CResult res = new CResult();

				// Source 이미지 로드 // Load the source image
				if (fliSourceImage.Load("../../ExampleImages/RingWarping/CircleColor.flif").IsFail())
                {
					ErrorPrint(res,"Failed to load the image file. \n");
					break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if (viewImageSource.Create(400, 0, 400 + 512, 384).IsFail())
                {
					ErrorPrint(res,"Failed to create the image view. \n");
					break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if (viewImageDestination.Create(400 + 512, 0, 400 + 512 * 2, 384).IsFail())
                {
					ErrorPrint(res,"Failed to create the image view. \n");
					break;
                }

				

				// Ring Warping 객체 생성 // Create Ring Warping object
				CRingWarping ringWarping = new CRingWarping();

                // Source 이미지 설정 // Set the source image
                ringWarping.SetSourceImage(ref fliSourceImage);

                // Source 이미지 관심 영역 파라미터 설정
				CFLDoughnut<double> fldSourceROI = new CFLDoughnut<double> (257.071130, 257.071130, 216.368201, 118.521494, 0.000000, -17.159659, 213.494067, EArcClosingMethod.Center);
				// Source 이미지 관심 영역 설정
				ringWarping.SetSourceROI(fldSourceROI);

                // Destination 이미지 설정 // Set the destination image
                ringWarping.SetDestinationImage(ref fliDestinationImage);

                // 보간법 설정 (Bicubic / Bilinear / NearestNeighbor)
                ringWarping.SetInterpolationMethod(EInterpolationMethod.Bilinear);

				// 공백 영역 색상 값 설정 // Set 공백 영역 색상 value
				CMultiVar<double> mvBlankColor = new CMultiVar<double>(10, 160, 20);

                // 공백 영역 색상 지정 // Set the blank color
                ringWarping.SetBlankColor(mvBlankColor);

                // 항상 공백 영역을 지정한 색으로 채우도록 설정
                ringWarping.EnableFillBlankColorMode(true);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = ringWarping.Execute()).IsFail())
                {
					ErrorPrint(res,"Failed to execute Ringwarping. \n");

					break;
				}

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if (viewImageSource.SetImagePtr(ref fliSourceImage).IsFail())
                {
					ErrorPrint(res,"Failed to set image object on the image view. \n");
                    break;
                }

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if (viewImageDestination.SetImagePtr(ref fliDestinationImage).IsFail())
                {
					ErrorPrint(res,"Failed to set image object on the image view.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if (viewImageSource.SynchronizeWindow(ref viewImageDestination).IsFail())
                {
					ErrorPrint(res,"Failed to synchronize window. \n");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerDestination.Clear();

				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				// Source ROI 영역이 어디인지 알기 위해 디스플레이 한다
				if (layerSource.DrawFigureImage(fldSourceROI, EColor.LIME, 3).IsFail())
				{
					ErrorPrint(res,"Failed to draw figure. \n");
					break;
				}

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

                if (layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30).IsFail())
                {
					ErrorPrint(res,"Failed to draw text. \n");
                    break;
                }

                if (layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30).IsFail())
                {
					ErrorPrint(res,"Failed to draw text. \n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                viewImageSource.Invalidate(true);
                viewImageDestination.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSource.IsAvailable() && viewImageDestination.IsAvailable())
                   Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

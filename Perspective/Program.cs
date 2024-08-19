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

using CResult = FLImagingCLR.CResult;

namespace Perspective
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
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageDestination = new CGUIViewImage();

			CResult res;

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = fliSourceImage.Load("../../ExampleImages/Perspective/calendar.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// Destination 이미지 로드 // Load the destination image
				if((res = fliDestinationImage.Load("../../ExampleImages/Perspective/space.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if((res = viewImageSource.Create(400, 0, 912, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if((res = viewImageDestination.Create(912, 0, 1424, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((res = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSource.SynchronizeWindow(viewImageDestination)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// Perspective 객체 생성 // Create Perspective object
				CPerspective perspective = new CPerspective();

				// Source 이미지 설정 // Set the source image
				perspective.SetSourceImage(ref fliSourceImage);

				// Source 이미지의 투영 영역 범위 설정 // Set the range of the projection area of ​​the Source image
				CFLQuad<double> flqSourceProjection = new CFLQuad<double>(290.87, 65.73, 531.69, 192.5, 169.68, 406.66, 34.59, 170.22);

				// Source 이미지의 투영 영역 지정 // Set the projection area of ​​the Source image
				perspective.SetSourceProjection(flqSourceProjection);

				// Destination 이미지 설정 // Set the destination image
				perspective.SetDestinationImage(ref fliDestinationImage);

				// Destination 이미지의 출력 대상 영역 범위 설정 // Set the output destination area range of Destination image
				CFLCircle<double> flcDestinationROI = new CFLCircle<double>(243, 261, 188, 0, 0, 360, EArcClosingMethod.EachOther);

				// Destination 이미지의 출력 대상 영역 지정 // Destination Specify the output target area of ​​the image
				perspective.SetDestinationROI(flcDestinationROI);

				// Destination 이미지의 투영 영역 범위 설정 // Set the range of the projection area of ​​the destination image
				CFLRect<double> flrDestinationProjection = new CFLRect<double>(192, 208, 332, 346);

				// Destination 이미지의 투영 영역 지정 // Set the projection area of ​​the destination image
				perspective.SetDestinationProjection(flrDestinationProjection);

				// 보간법 설정 (Bicubic / Bilinear / NearestNeighbor) // Set interpolation method (Bicubic / Bilinear / NearestNeighbor)
				perspective.SetInterpolationMethod(EInterpolationMethod.Bicubic);

				// 공백 영역 색상 값 설정 // Set blank area color value
				CMultiVar<double> mvBlankColor = new CMultiVar<double>(10, 160, 20);

				// 공백 영역 색상 지정 // Set the blank color
				perspective.SetBlankColor(mvBlankColor);

				// 항상 공백 영역을 지정한 색으로 채우도록 설정 // Always set blank areas to be filled with the specified color
				perspective.EnableFillBlankColorMode(true);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (perspective.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute perspective. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSource.Clear();
				layerDestination.Clear();

				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능 // FLImaging's figure objects can be displayed with a single function, regardless of the shape of the figure
				// Source Projection 영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the Source Projection area is
				if((res = layerSource.DrawFigureImage(flqSourceProjection, EColor.LIME, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure. \n");
					break;
				}

				// Destination Projection 영역이 어디인지 알기 위해 디스플레이한다. // Display to know where the Destination Projection area is.
				if((res = layerDestination.DrawFigureImage(flrDestinationProjection, EColor.LIME, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure.\n");
					break;
				}

				// Destination ROI 영역이 어디인지 알기 위해 디스플레이한다. // Display to know where the Destination ROI area is.
				if((res = layerDestination.DrawFigureImage(flcDestinationROI, EColor.RED, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure. \n");
					break;
				}

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewImageSource.Invalidate(true);
				viewImageDestination.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageDestination.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

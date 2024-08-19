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

namespace Homography
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
				if((res = fliSourceImage.Load("../../ExampleImages/Homography/calendar.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// Destination 이미지 로드 // Load the destination image
				if((res = fliDestinationImage.Load("../../ExampleImages/Homography/space.flif")).IsFail())
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

				// Homography 객체 생성 // Create Homography object
				CHomography homography = new CHomography();

				// Source 이미지 설정 // Set the source image
				homography.SetSourceImage(ref fliSourceImage);

				// Source 이미지의 투영 영역 범위 설정 // Set the range of the projection area of ​​the Source image
				CFLPointArray flpaSourceProjection = new CFLPointArray();

				flpaSourceProjection.PushBack(new CFLPoint<double>(564.137931, 316.551724));
				flpaSourceProjection.PushBack(new CFLPoint<double>(363.448276, 438.620690));
				flpaSourceProjection.PushBack(new CFLPoint<double>(220.689655, 283.448276));
				flpaSourceProjection.PushBack(new CFLPoint<double>(363.448276, 192.413793));
				flpaSourceProjection.PushBack(new CFLPoint<double>(121.379310, 163.448276));
				flpaSourceProjection.PushBack(new CFLPoint<double>(504.137931, 122.068966));
				flpaSourceProjection.PushBack(new CFLPoint<double>(80.000000, 120.000000));
				flpaSourceProjection.PushBack(new CFLPoint<double>(268.275862, 113.793103));
				flpaSourceProjection.PushBack(new CFLPoint<double>(32.413793, 380.689655));
				flpaSourceProjection.PushBack(new CFLPoint<double>(53.103448, 74.482759));
				flpaSourceProjection.PushBack(new CFLPoint<double>(214.482759, 68.275862));
				flpaSourceProjection.PushBack(new CFLPoint<double>(373.793103, 64.137931));
				flpaSourceProjection.PushBack(new CFLPoint<double>(160.689655, 28.965517));

				// Source 이미지의 투영 영역 지정 // Set the projection area of ​​the Source image
				homography.SetSourceProjection(flpaSourceProjection);

				// Destination 이미지 설정 // Set the destination image
				homography.SetDestinationImage(ref fliDestinationImage);

				// Destination 이미지의 투영 영역 범위 설정 // Set the range of the projection area of ​​the destination image
				CFLPointArray flpaDestinationProjection = new CFLPointArray();

				flpaDestinationProjection.PushBack(new CFLPoint<double>(529.223526, 181.280286));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(528.781754, 301.190422));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(403.991849, 315.695190));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(399.088041, 186.290795));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(262.810194, 316.159206));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(401.317045, 82.478995));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(122.572625, 317.202957));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(268.777064, 182.517166));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(408.998534, 438.577031));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(7.275742, 322.207494));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(131.900147, 180.456139));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(263.629186, 75.928433));
				flpaDestinationProjection.PushBack(new CFLPoint<double>(1.266525, 182.189349));

				// Destination 이미지의 투영 영역 지정 // Set the projection area of ​​the destination image
				res = homography.SetDestinationProjection(flpaDestinationProjection);

				// 보간법 설정 (Bicubic / Bilinear / NearestNeighbor) // Set interpolation method (Bicubic / Bilinear / NearestNeighbor)
				homography.SetInterpolationMethod(EInterpolationMethod.Bicubic);

				CFLFigure flpaTest = homography.GetDestinationProjection();

				// 공백 영역 색상 값 설정 // Set blank area color value
				CMultiVar<double> mvBlankColor = new CMultiVar<double>(10, 160, 20);

				// 공백 영역 색상 지정 // Set the blank color
				homography.SetBlankColor(mvBlankColor);

				// 항상 공백 영역을 지정한 색으로 채우도록 설정 // Always set blank areas to be filled with the specified color
				homography.EnableFillBlankColorMode(true);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (homography.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute homography. \n");
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
				if((res = layerSource.DrawFigureImage(flpaSourceProjection, EColor.LIME, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure. \n");
					break;
				}

				// Destination Projection 영역이 어디인지 알기 위해 디스플레이한다. // Display to know where the Destination Projection area is.
				if((res = layerDestination.DrawFigureImage(flpaDestinationProjection, EColor.LIME, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure.\n");
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

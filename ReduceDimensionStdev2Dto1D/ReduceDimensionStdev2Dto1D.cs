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

namespace ReduceDimensionStdev2Dto1D
{
	class ReduceDimensionStdev2Dto1D
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
			CFLImage fliDestinationImageX = new CFLImage();
			CFLImage fliDestinationImageY = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageDestinationX = new CGUIViewImage();
			CGUIViewImage viewImageDestinationY = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliSourceImage.Load("../../ExampleImages/ReduceDimensionStdev2Dto1D/Source.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSource.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageDestinationX.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageDestinationY.Create(1124, 0, 1636, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationX)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationY)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageDestinationX.SetImagePtr(ref fliDestinationImageX)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageDestinationY.SetImagePtr(ref fliDestinationImageY)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// Reduce Dimension Stdev 2D to 1D 객체 생성 // Create Reduce Dimension Stdev 2D to 1D object
				CReduceDimensionStdev2Dto1D reduceDimensionStdev2Dto1D = new CReduceDimensionStdev2Dto1D();

				// Source 이미지 설정 // Set source image 
				reduceDimensionStdev2Dto1D.SetSourceImage(ref fliSourceImage);

				// Destination 이미지 설정 // Set destination image
				reduceDimensionStdev2Dto1D.SetDestinationImage(ref fliDestinationImageX);

				// 축소 차원 설정 // Set reduction dimension
				reduceDimensionStdev2Dto1D.SetReductionDimension(CReduceDimensionStdev2Dto1D.EReductionDimension.X);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (reduceDimensionStdev2Dto1D.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Reduce Dimension Stdev 2D to 1D. \n");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				reduceDimensionStdev2Dto1D.SetDestinationImage(ref fliDestinationImageY);

				// 축소 차원 설정 // Set reduction dimension
				reduceDimensionStdev2Dto1D.SetReductionDimension(CReduceDimensionStdev2Dto1D.EReductionDimension.Y);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (reduceDimensionStdev2Dto1D.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Reduce Dimension Stdev 2D to 1D. \n");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerDestinationX = viewImageDestinationX.GetLayer(0);
				CGUIViewImageLayer layerDestinationY = viewImageDestinationY.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layerSource.Clear();
				layerDestinationX.Clear();
				layerDestinationY.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = (layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationX.DrawTextCanvas(flpPoint, "Destination Image(X Dimension)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationY.DrawTextCanvas(flpPoint, "Destination Image(Y Dimension)", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImageSource.Invalidate(true);
				viewImageDestinationX.Invalidate(true);
				viewImageDestinationY.Invalidate(true);

				// 이미지와 이미지 뷰의 Sync 를 동일하게 맞춥니다.
				if((res = (viewImageDestinationX.ZoomFit())).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit. \n");
					break;
				}

				if((res = (viewImageDestinationY.ZoomFit())).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit. \n");
					break;
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageDestinationX.IsAvailable() && viewImageDestinationX.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

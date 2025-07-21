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

namespace OperationSigmoid
{
	class OperationSigmoid
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
			CFLImage fliDestinationImage0 = new CFLImage();
			CFLImage fliDestinationImage1 = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageDestination0 = new CGUIViewImage();
			CGUIViewImage viewImageDestination1 = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliSourceImage.Load("../../ExampleImages/OperationSigmoid/Coord1D.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSource.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDestination0.Create(600, 0, 1100, 500)).IsFail() ||
					(res = viewImageDestination1.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. .
				if((res = viewImageSource.SynchronizePointOfView(ref viewImageDestination0)).IsFail() ||
					(res = viewImageSource.SynchronizePointOfView(ref viewImageDestination1)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestination0)).IsFail() ||
					(res = viewImageSource.SynchronizeWindow(ref viewImageDestination1)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail() ||
					(res = viewImageDestination0.SetImagePtr(ref fliDestinationImage0)).IsFail() ||
					(res = viewImageDestination1.SetImagePtr(ref fliDestinationImage1)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				COperationSigmoid algObject = new COperationSigmoid();

				algObject.SetSourceImage(ref fliSourceImage);
				algObject.SetDestinationImage(ref fliDestinationImage0);
				algObject.SetOperationMode(COperationSigmoid.EOperationMode.Forward);
				algObject.EnableGeneralizedMode(true);
				algObject.SetB(1.0);
				algObject.SetM(0.0);
				algObject.SetK(1.0);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if ((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation Sigmoid.");
					break;
				}

				algObject.SetDestinationImage(ref fliDestinationImage1);
				algObject.SetOperationMode(COperationSigmoid.EOperationMode.Backward);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation Sigmoid.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerDestination0 = viewImageDestination0.GetLayer(0);
				CGUIViewImageLayer layerDestination1 = viewImageDestination1.GetLayer(0);

				// 기존에 Layer 에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSource.Clear();
				layerDestination0.Clear();
				layerDestination1.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if ((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDestination0.DrawTextCanvas(flpPoint, "Destination Forward Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDestination1.DrawTextCanvas(flpPoint, "Destination Backward Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				// 이미지 뷰의 값 표현 방식 설정 // Set how values are expressed in image view
				viewImageSource.SetPixelNumberMode(EPixelNumberMode.Decimal);
				viewImageDestination0.SetPixelNumberMode(EPixelNumberMode.Decimal);
				viewImageDestination1.SetPixelNumberMode(EPixelNumberMode.Decimal);

				// floating 이미지의 색상 표현 범위 설정 // Set the color expression range of floating images
				viewImageSource.SetFloatingImageValueRange(-1.0, 1.0);
				viewImageDestination0.SetFloatingImageValueRange(-1.0, 1.0);
				viewImageDestination1.SetFloatingImageValueRange(-1.0, 1.0);

				// 이미지 뷰를 갱신 // Update image view
				viewImageSource.Invalidate(true);
				viewImageDestination0.Invalidate(true);
				viewImageDestination1.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageDestination0.IsAvailable() && viewImageDestination1.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

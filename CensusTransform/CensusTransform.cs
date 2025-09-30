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

namespace FLImagingExamplesCSharp
{
	class CensusTransform
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

			// 이미지 객체 선언 // Declare image object
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliDestinationImage = new CFLImage();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewSourceImage = new CGUIViewImage();
			CGUIViewImage viewDestinationImage = new CGUIViewImage();

			// 수행 결과 객체 선언 // Declare execution result object
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// Source 이미지 로드 // Load Source image
				if((res = fliSourceImage.Load("../../ExampleImages/CensusTransform/Src.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create Source image view
				if((res = viewSourceImage.Create(100, 0, 600, 545)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create Destination image view
				if((res = viewDestinationImage.Create(600, 0, 1100, 545)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 // Synchronize viewpoints of two image views
				if((res = viewSourceImage.SynchronizePointOfView(ref viewDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize point of view between image views.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display image in Source image view
				if((res = viewSourceImage.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display image in Destination image view
				if((res = viewDestinationImage.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref viewDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// Census Transform 객체 생성 // Create Census Transform object
				CCensusTransform censusTransform = new CCensusTransform();

				// Source 이미지 설정 // Set Source image
				if((res = censusTransform.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source image.\n");
					break;
				}

				// Destination 이미지 설정 // Set Destination image
				if((res = censusTransform.SetDestinationImage(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination image.\n");
					break;
				}

				// Kernel의 크기 설정 // Set kernel size
				if((res = censusTransform.SetKernel(9, 7)).IsFail())
				{
					ErrorPrint(res, "Failed to set kernel size.\n");
					break;
				}

				// Kernel의 픽셀 간의 거리를 설정 // Set distance between each pixel in kernel
				if((res = censusTransform.SetSparseLength(1)).IsFail())
				{
					ErrorPrint(res, "Failed to set sparse length.\n");
					break;
				}

				// 미러 모드 설정 // Set mirror mode
				if((res = censusTransform.EnableMirrorMode(true)).IsFail())
				{
					ErrorPrint(res, "Failed to set mirror mode flag.\n");
					break;
				}

				// 데이터 값의 정렬 방향 설정 // Set data alignment direction
				if((res = censusTransform.EnableMSBShiftMode(true)).IsFail())
				{
					ErrorPrint(res, "Failed to set MSB shift mode flag.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = censusTransform.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Census Transform.\n");
					break;
				}

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerSource = viewSourceImage.GetLayer(0);
				CGUIViewImageLayer layerDestination = viewDestinationImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear figures drawn on existing layer
				layerSource.Clear();
				layerDestination.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerSource.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerDestination.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewSourceImage.Invalidate(true);
				viewDestinationImage.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewSourceImage.IsAvailable() && viewDestinationImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

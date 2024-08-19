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

namespace OperationPopcount
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
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			CResult res;

			do
			{
				var arrU8 = new byte[32];
				for(int i = 0; i < 16; ++i)
				{
					UInt16 value = (UInt16)~(0xffff << i);
					arrU8[2 * i] = (byte)(value & 255);
					arrU8[2 * i + 1] = (byte)(value >> 8);
				}

				// 버퍼로부터 Source 이미지 생성 // Create the source image from the buffer
				if((res = fliSourceImage.Create(4, 4, arrU8, EPixelFormat.C1_U16)).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// 이미지 뷰 생성 // Create image views
				if ((res = viewImageSrc.Create(100, 0, 600, 545)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 545)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화한다 // Synchronize the viewpoints of the two image views
				if ((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the images in the image views
				if ((res = viewImageSrc.SetImagePtr(ref fliSourceImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				COperationPopcount algObject = new COperationPopcount();

				algObject.SetSourceImage(ref fliSourceImage);
				algObject.SetDestinationImage(ref fliDestinationImage);

				CMultiVar<UInt64> mvBlankColor = new CMultiVar<UInt64>(0);

				algObject.SetBlankColorU64(mvBlankColor);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if ((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation popcount.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSource = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDestination = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSource.Clear();
				layerDestination.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if ((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				// Source 이미지 뷰의 Pixel 값을 16진법으로 설정 // Show Pixel Values on Source Image View to Hexadecimal
				viewImageSrc.SetPixelNumberMode(EPixelNumberMode.Hexadecimal);

				// 이미지 뷰를 갱신 // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

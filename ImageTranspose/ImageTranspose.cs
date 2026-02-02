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

namespace FLImagingExamplesCSharp
{
	class ImageTranspose
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
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliDestinationImageXY = new CFLImage();
			CFLImage fliDestinationImageXZ = new CFLImage();
			CFLImage fliDestinationImageYX = new CFLImage();
			CFLImage fliDestinationImageYZ = new CFLImage();
			CFLImage fliDestinationImageZX = new CFLImage();
			CFLImage fliDestinationImageZY = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageDestinationXY = new CGUIViewImage();
			CGUIViewImage viewImageDestinationXZ = new CGUIViewImage();
			CGUIViewImage viewImageDestinationYX = new CGUIViewImage();
			CGUIViewImage viewImageDestinationYZ = new CGUIViewImage();
			CGUIViewImage viewImageDestinationZX = new CGUIViewImage();
			CGUIViewImage viewImageDestinationZY = new CGUIViewImage();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = fliSourceImage.Load("../../ExampleImages/ImageTranspose/Gradation.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if((res = viewImageSource.Create(100, 0, 700, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if((res = viewImageDestinationXY.Create(700, 0, 1000, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageDestinationXZ.Create(1000, 0, 1300, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageDestinationYX.Create(1300, 0, 1600, 300)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageDestinationYZ.Create(700, 300, 1000, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageDestinationZX.Create(1000, 300, 1300, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageDestinationZY.Create(1300, 300, 1600, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Source와 Desitination 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the source and destination image view windows
				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationXY)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationXZ)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationYX)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationYZ)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationZX)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageSource.SynchronizeWindow(ref viewImageDestinationZY)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the Destination image view
				if((res = viewImageDestinationXY.SetImagePtr(ref fliDestinationImageXY)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageDestinationXZ.SetImagePtr(ref fliDestinationImageXZ)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageDestinationYX.SetImagePtr(ref fliDestinationImageYX)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageDestinationYZ.SetImagePtr(ref fliDestinationImageYZ)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageDestinationZX.SetImagePtr(ref fliDestinationImageZX)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageDestinationZY.SetImagePtr(ref fliDestinationImageZY)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// Image Transpose 객체 생성 // Create Image Transpose object
				CImageTranspose imageTranspose = new CImageTranspose();

				// Source 이미지 설정 // Set the source image
				// 모든 Source 이미지는 동일한 사이즈와 포맷을 가져야합니다.
				imageTranspose.SetSourceImage(ref fliSourceImage);

				// Destination 이미지 설정 // Set the destination image
				imageTranspose.SetDestinationImage(ref fliDestinationImageXY);

				// Transpose 후 사용자에게 보일 평면을 XY 평면으로 설정 // Set the plane to the XY plane to be visible to the user after Transpose
				imageTranspose.SetResultPlane(CImageTranspose.EPlane.XY);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = imageTranspose.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Transpose.\n");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				imageTranspose.SetDestinationImage(ref fliDestinationImageXZ);

				// Transpose 후 사용자에게 보일 평면을 XZ 평면으로 설정 // Set the plane to the XZ plane to be visible to the user after Transpose
				imageTranspose.SetResultPlane(CImageTranspose.EPlane.XZ);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = imageTranspose.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Transpose.\n");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				imageTranspose.SetDestinationImage(ref fliDestinationImageYX);

				// Transpose 후 사용자에게 보일 평면을 YX 평면으로 설정 // Set the plane to the YX plane to be visible to the user after Transpose
				imageTranspose.SetResultPlane(CImageTranspose.EPlane.YX);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = imageTranspose.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Transpose.\n");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				imageTranspose.SetDestinationImage(ref fliDestinationImageYZ);

				// Transpose 후 사용자에게 보일 평면을 YZ 평면으로 설정 // Set the plane to the YZ plane to be visible to the user after Transpose
				imageTranspose.SetResultPlane(CImageTranspose.EPlane.YZ);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = imageTranspose.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Transpose.\n");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				imageTranspose.SetDestinationImage(ref fliDestinationImageZX);

				// Transpose 후 사용자에게 보일 평면을 ZX 평면으로 설정 // Set the plane to the ZX plane to be visible to the user after Transpose
				imageTranspose.SetResultPlane(CImageTranspose.EPlane.ZX);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = imageTranspose.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Transpose.\n");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				imageTranspose.SetDestinationImage(ref fliDestinationImageZY);

				// Transpose 후 사용자에게 보일 평면을 ZY 평면으로 설정 // Set the plane to the ZY plane to be visible to the user after Transpose
				imageTranspose.SetResultPlane(CImageTranspose.EPlane.ZY);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = imageTranspose.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Image Transpose.\n");
					break;
				}

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다.	// With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				viewImageDestinationXY.ZoomFit();
				viewImageDestinationXZ.ZoomFit();
				viewImageDestinationYX.ZoomFit();
				viewImageDestinationYZ.ZoomFit();
				viewImageDestinationZX.ZoomFit();
				viewImageDestinationZY.ZoomFit();

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerDestinationXY = viewImageDestinationXY.GetLayer(0);
				CGUIViewImageLayer layerDestinationXZ = viewImageDestinationXZ.GetLayer(0);
				CGUIViewImageLayer layerDestinationYX = viewImageDestinationYX.GetLayer(0);
				CGUIViewImageLayer layerDestinationYZ = viewImageDestinationYZ.GetLayer(0);
				CGUIViewImageLayer layerDestinationZX = viewImageDestinationZX.GetLayer(0);
				CGUIViewImageLayer layerDestinationZY = viewImageDestinationZY.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSource.Clear();
				layerDestinationXY.Clear();
				layerDestinationXZ.Clear();
				layerDestinationYX.Clear();
				layerDestinationYZ.Clear();
				layerDestinationZX.Clear();
				layerDestinationZY.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationXY.DrawTextCanvas(flpPoint, "XY Plane Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationXZ.DrawTextCanvas(flpPoint, "XZ Plane Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationYX.DrawTextCanvas(flpPoint, "YX Plane Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationYZ.DrawTextCanvas(flpPoint, "YZ Plane Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationZX.DrawTextCanvas(flpPoint, "ZX Plane Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				if((res = layerDestinationZY.DrawTextCanvas(flpPoint, "ZY Plane Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewImageSource.Invalidate(true);
				viewImageDestinationXY.Invalidate(true);
				viewImageDestinationXZ.Invalidate(true);
				viewImageDestinationYX.Invalidate(true);
				viewImageDestinationYZ.Invalidate(true);
				viewImageDestinationZX.Invalidate(true);
				viewImageDestinationZY.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageDestinationXY.IsAvailable() && viewImageDestinationXZ.IsAvailable() && viewImageDestinationYX.IsAvailable() && viewImageDestinationYZ.IsAvailable() && viewImageDestinationZX.IsAvailable() && viewImageDestinationZY.IsAvailable() )
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

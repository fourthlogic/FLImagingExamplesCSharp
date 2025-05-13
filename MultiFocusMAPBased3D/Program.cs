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
using FLImagingCLR.ThreeDim;

namespace MultiFocus
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();
			CFLImage fliTxtImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrcImage.Load("../../ExampleImages/MultiFocusMAPBased3D/")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliSrcImage.SelectPage(0);

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 548, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((res = viewImageDst.Create(548, 0, 996, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 3D 이미지 뷰 생성 // Create the destination 3D image view
				if((res = view3DDst.Create(400, 200, 1300, 800)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two view windows.
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				viewImageSrc.SetFixThumbnailView(true);

				// Multi Focus MAP Based 객체 생성 // Create Multi Focus MAP Based object
				CMultiFocusMAPBased3D algMultiFocusMAPBased3D = new CMultiFocusMAPBased3D();

				CFL3DObject fl3DOHM = new CFL3DObjectHeightMap();

				// Source 이미지 설정 // Set the source image
				if((res = algMultiFocusMAPBased3D.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				// 결과 destination height map 이미지 설정 // Set the destination height map image
				if((res = algMultiFocusMAPBased3D.SetDestinationHeightMapImage(ref fliDstImage)).IsFail())
					break;
				// 결과 destination texture 이미지 설정 // Set the destination texture image
				if((res = algMultiFocusMAPBased3D.SetDestinationTextureImage(ref fliTxtImage)).IsFail())
					break;

				// Focus measure bias page index 설정 // Set the focus measure bias page index
				if((res = algMultiFocusMAPBased3D.SetFMBiasPageIndex(3)).IsFail())
					break;
				// Focus measure bias value 설정 // Set the Focus measure bias value
				if((res = algMultiFocusMAPBased3D.SetFMBiasValue(0.02)).IsFail())
					break;
				// Focus measure method 설정 // Set focus measure method
				if((res = algMultiFocusMAPBased3D.SetFocusMeasureMethod(CMultiFocusMAPBased3D.EFocusMeasureMethod.DoG)).IsFail())
					break;
				// Sigma1 설정 // Set the sigma1
				if((res = algMultiFocusMAPBased3D.SetSigma1(0.4)).IsFail())
					break;
				// Sigma2 설정 // Set the sigma2
				if((res = algMultiFocusMAPBased3D.SetSigma2(0.8)).IsFail())
					break;

				// Local regularization factor 설정 // Set the local regularization factor
				if((res = algMultiFocusMAPBased3D.SetLocalRegularizationFactor(0.02)).IsFail())
					break;
				// Global regularization factor 설정 // Set the global regularization factor
				if((res = algMultiFocusMAPBased3D.SetGlobalRegularizationFactor(0.00000000001)).IsFail())
					break;
				// Conjugate Gradient Method 의 tolerance 설정 // Set the tolerance for Conjugate Gradient Method
				if((res = algMultiFocusMAPBased3D.SetCGMTolerance(0.00001)).IsFail())
					break;
				// Conjugate Gradient Method 의 max iterations 설정 // Set the max iterations for Conjugate Gradient Method
				if((res = algMultiFocusMAPBased3D.SetCGMMaxIterations(100)).IsFail())
					break;

				// Page Direction 설정 // Set the page direction
				if((res = algMultiFocusMAPBased3D.SetDirection(CMultiFocusMAPBased3D.EDirection.BottomToTop)).IsFail())
					break;
				// Pixel Accuracy 설정 // Set the pixel accuracy
				if((res = algMultiFocusMAPBased3D.SetPixelAccuracy(1.0)).IsFail())
					break;
				// Depth Pitch 설정 // Set the depth pitch
				if((res = algMultiFocusMAPBased3D.SetDepthPitch(2.0)).IsFail())
					break;

				// Destination 3D object 생성 활성화 // Enable the Destination 3D object generation
				if((res = algMultiFocusMAPBased3D.Enable3DObjectGeneration(true)).IsFail())
					break;
				// Destination 3D object 설정 // Set the Destination 3D object 
				if((res = algMultiFocusMAPBased3D.SetDestinationObject(ref fl3DOHM)).IsFail())
					break;


				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = algMultiFocusMAPBased3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute MultiFocus.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageDst.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);
				CGUIView3DLayer layer3D = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				CFL3DObjectHeightMap fl3DOHMResult = new CFL3DObjectHeightMap(fliDstImage, fliTxtImage);

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if(view3DDst.PushObject(fl3DOHMResult).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				view3DDst.ZoomFit();

				if((res = layer3D.DrawTextCanvas(flp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

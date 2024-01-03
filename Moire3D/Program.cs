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

namespace FPP
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
			CFLImage[] fliLrnImage = new CFLImage[2];
			// 이미지 객체 선언 // Declare the image object
			CFLImage[] fliSrcImage = new CFLImage[2];
			CFLImage fliImageDst = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImageLrn = new CGUIViewImage[2];
			CGUIViewImage[] viewImageSrc = new CGUIViewImage[2];
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DDst = new CGUIView3D();

			fliLrnImage[0] = new CFLImage();
			fliLrnImage[1] = new CFLImage();
			fliSrcImage[0] = new CFLImage();
			fliSrcImage[1] = new CFLImage();

			viewImageLrn[0] = new CGUIViewImage();
			viewImageLrn[1] = new CGUIViewImage();
			viewImageSrc[0] = new CGUIViewImage();
			viewImageSrc[1] = new CGUIViewImage();

			CFLImage fliTxtImage = new CFLImage();

			// Source 이미지를 저장할 Array 선언 // Declare an Array to store the source image
			List<CFLImage> vctLrnImages = new List<CFLImage>();

			// Source 이미지 입력 // source images add
			vctLrnImages.Add(fliLrnImage[0]);
			vctLrnImages.Add(fliLrnImage[1]);

			// Source 이미지를 저장할 Array 선언 // Declare an Array to store the source image
			List<CFLImage> vctSrcImages = new List<CFLImage>();

			// Source 이미지 입력 // source images add
			vctSrcImages.Add(fliSrcImage[0]);
			vctSrcImages.Add(fliSrcImage[1]);

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// Learn 이미지 로드 // Load the reference plane image for calibration
				if((eResult = vctLrnImages[0].Load("../../ExampleImages/Moire3D/Learn0/")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				vctLrnImages[0].SelectPage(0);

				// Source 이미지 로드 // Load the source image
				if((eResult = vctSrcImages[0].Load("../../ExampleImages/Moire3D/Object0/")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				vctSrcImages[0].SelectPage(0);

				// Learn 이미지 로드 // Load the reference plane image for calibration
				if((eResult = vctLrnImages[1].Load("../../ExampleImages/Moire3D/Learn1/")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				vctLrnImages[1].SelectPage(0);

				// Source 이미지 로드 // Load the source image
				if((eResult = vctSrcImages[1].Load("../../ExampleImages/Moire3D/Object1/")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				vctSrcImages[1].SelectPage(0);

				// Learn 이미지 뷰 생성 // Create the learn image view
				if((eResult = viewImageLrn[0].Create(100, 0, 548, 348)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Learn 이미지 뷰 생성 // Create the destination image view
				if((eResult = viewImageLrn[1].Create(548, 0, 996, 348)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create the Source image view
				if((eResult = viewImageSrc[0].Create(100, 348, 548, 696)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create the destination image view
				if((eResult = viewImageSrc[1].Create(548, 348, 996, 696)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Dst 이미지 뷰 생성 // Create the destination image view
				if((eResult = viewImageDst.Create(996, 348, 1444, 696)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Learn 이미지 뷰에 이미지를 디스플레이 // Display the image in the Learn image view
				for(int i32I = 0; i32I < 2; ++i32I)
				{
					if((eResult = viewImageLrn[i32I].SetImagePtr(ref fliLrnImage[i32I])).IsFail())
					{
						ErrorPrint(eResult, "Failed to set image object on the image view.\n");
						break;
					}
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the Source image view
				for(int i32I = 0; i32I < 2; ++i32I)
				{
					if((eResult = viewImageSrc[i32I].SetImagePtr(ref fliSrcImage[i32I])).IsFail())
					{
						ErrorPrint(eResult, "Failed to set image object on the image view.\n");
						break;
					}
				}

				// Dst 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((eResult = viewImageDst.SetImagePtr(ref fliImageDst)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two view windows
				if((eResult = viewImageLrn[1].SynchronizeWindow(ref viewImageLrn[0])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two view windows
				for(int i32I = 0; i32I < 2; ++i32I)
				{
					if((eResult = viewImageSrc[i32I].SynchronizeWindow(ref viewImageLrn[0])).IsFail())
					{
						ErrorPrint(eResult, "Failed to synchronize window.\n");
						break;
					}
				}

				// 두 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two view windows
				if((eResult = viewImageDst.SynchronizeWindow(ref viewImageLrn[0])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views. 
				if((eResult = viewImageLrn[1].SynchronizePointOfView(ref viewImageLrn[0])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views. 
				for(int i32I = 0; i32I < 2; ++i32I)
				{
					if((eResult = viewImageSrc[i32I].SynchronizePointOfView(ref viewImageLrn[0])).IsFail())
					{
						ErrorPrint(eResult, "Failed to synchronize view.\n");
						break;
					}
				}

				// 두 이미지 뷰의 페이지를 동기화 한다. // Synchronize the page of the two image views. 
				if((eResult = viewImageDst.SynchronizePointOfView(ref viewImageLrn[0])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰의 페이지를 동기화 한다. // Synchronize the page of the two image views. 
				if((eResult = viewImageLrn[1].SynchronizePageIndex(ref viewImageLrn[0])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view.\n");
					break;
				}

				// 두 이미지 뷰의 페이지를 동기화 한다. // Synchronize the page of the two image views. 
				for(int i32I = 0; i32I < 2; ++i32I)
				{
					if((eResult = viewImageSrc[i32I].SynchronizePageIndex(ref viewImageLrn[0])).IsFail())
					{
						ErrorPrint(eResult, "Failed to synchronize view.\n");
						break;
					}
				}

				// Destination 3D 이미지 뷰 생성 // Create the destination 3D image view
				if((eResult = view3DDst.Create(400, 200, 1300, 800)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				CMultiVar<double> mvF64AngleOfProjector = new CMultiVar<double>(73, 105);
				CMultiVar<double> mvF64SmallBinRange = new CMultiVar<double>(1, 1);

				// FPP 객체 생성 // Create FPP object
				CMoire3D Moire3D = new CMoire3D();

				// Learn 이미지 설정 // Set the learn image
				Moire3D.SetLearnImage(ref vctLrnImages);
				// Source 이미지 설정 // Set the source image
				Moire3D.SetSourceImage(ref vctSrcImages);
				// Destination 이미지 설정 // Set the destination image
				Moire3D.SetDestinationImage(ref fliImageDst);
				// 카메라의 working distance 설정 // Set working distance of the camera
				Moire3D.SetWorkingDistance(330);
				// 카메라의 field of view 설정 // Set field of view of the camera
				Moire3D.SetFieldOfView(400);
				// 프로젝터 각도 설정 // Set angle of projector
				Moire3D.SetAngleOfProjector(ref mvF64AngleOfProjector);
				// Phase Unwrap 히스토그램 bin 범위 설정 // Set histogram bin range for phase unwrapping
				Moire3D.SetBinInterval(ref mvF64SmallBinRange);
				// 패턴 타입 설정 // Set Pattern Type
				Moire3D.SetPatternType(CMoire3D.EPatternType.SquareWave);
				// Noise 감쇠 모드 활성화 // Enable noise reduction mode
				Moire3D.EnableNoiseReduction(true);

				// 앞서 설정된 파라미터 대로 Calibration 수행 // Calibrate algorithm according to previously set parameters
				if((eResult = Moire3D.Calibrate()).IsFail())
				{
					ErrorPrint(eResult, "Failed to calibrate.");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = Moire3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute algorithm.");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((eResult = viewImageDst.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLrn0 = viewImageLrn[0].GetLayer(0);
				CGUIViewImageLayer layerLrn1 = viewImageLrn[1].GetLayer(0);
				CGUIViewImageLayer layerSrc0 = viewImageSrc[0].GetLayer(0);
				CGUIViewImageLayer layerSrc1 = viewImageSrc[1].GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);
				CGUIView3DLayer layer3D = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLrn0.Clear();
				layerLrn1.Clear();
				layerSrc0.Clear();
				layerSrc1.Clear();
				layerDst.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flp = new CFLPoint<double>();

				if((eResult = layerLrn0.DrawTextCanvas(flp, "Learn Image[0]", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerLrn1.DrawTextCanvas(flp, "Learn Image[1]", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerSrc0.DrawTextCanvas(flp, "Source Image[0]", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerSrc1.DrawTextCanvas(flp, "Source Image[1]", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				// 3D 이미지 뷰에 Height Map (Destination Image) 이미지를 디스플레이 // Display the Height Map (Destination Image) on the 3D image view
				if((eResult = view3DDst.SetHeightMap(fliImageDst)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// Texture 이미지 로드 // Load the texture image
				if((eResult = fliTxtImage.Load("../../ExampleImages/Moire3D/text.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 3D 이미지 뷰에 Texture 이미지를 디스플레이 // Display the texture image on the 3D image view
				if((eResult = view3DDst.SetTexture(fliTxtImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				if((eResult = layer3D.DrawTextCanvas(flp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				viewImageLrn[0].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageLrn[1].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageSrc[0].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);
				viewImageSrc[1].SetLayerAutoClearMode(ELayerAutoClearMode.PageChanged, false);

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageLrn[0].Invalidate(true);
				viewImageLrn[1].Invalidate(true);
				viewImageSrc[0].Invalidate(true);
				viewImageSrc[1].Invalidate(true);
				viewImageDst.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(viewImageLrn[0].IsAvailable() && viewImageLrn[0].IsAvailable() && viewImageSrc[0].IsAvailable() && viewImageSrc[1].IsAvailable() && viewImageDst.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

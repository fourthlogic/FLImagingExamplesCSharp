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

namespace FLImagingExamplesCSharp
{
	class StereoDisparity3D
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
			CFLImage fliSource2Image = new CFLImage();
			CFLImage fliDestinationImage = new CFLImage();
			CFLImage fliTextureImage = new CFLImage();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewSourceImage = new CGUIViewImage();
			CGUIViewImage viewSource2Image = new CGUIViewImage();
			CGUIViewImage viewDestinationImage = new CGUIViewImage();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DDst = new CGUIView3D();

			do
			{
				// 수행 결과 객체 선언 // Declare execution result object
				CResult res = new CResult(EResult.UnknownError);

				// Source 이미지 로드 // Load Source image
				if((res = fliSourceImage.Load("../../ExampleImages/StereoDisparity3D/Left.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 2 이미지 로드 // Load Source 2 image
				if((res = fliSource2Image.Load("../../ExampleImages/StereoDisparity3D/Right.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create Source image view
				if((res = viewSourceImage.Create(100, 0, 548, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display image in Source image view
				if((res = viewSourceImage.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Source 2 이미지 뷰 생성 // Create Source 2 image view
				if((res = viewSource2Image.Create(548, 0, 996, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 2 이미지 뷰에 이미지를 디스플레이 // Display image in Source 2 image view
				if((res = viewSource2Image.SetImagePtr(ref fliSource2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create Destination image view
				if((res = viewDestinationImage.Create(100, 448, 548, 896)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display image in Destination image view
				if((res = viewDestinationImage.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 3D 뷰 생성 // Create Destination 3D view
				if((res = view3DDst.Create(548, 448, 996, 896)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref viewSource2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref viewDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewSourceImage.SynchronizeWindow(ref view3DDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}


				// Stereo Disparity 3D 객체 생성 // Create Stereo Disparity 3D object
				CStereoDisparity3D stereoDisparity3D = new CStereoDisparity3D();

				// 출력에 사용되는 3D Height Map 객채 생성 // Create 3D height map used as output
				CFL3DObject fl3DOHM = new CFL3DObjectHeightMap();

				// Source 이미지 설정 // Set Source image
				if((res = stereoDisparity3D.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source image.\n");
					break;
				}

				// Source 2 이미지 설정 // Set Source 2 image
				if((res = stereoDisparity3D.SetSourceImage2(ref fliSource2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source 2 image.\n");
					break;
				}

				// Destination Height Map 이미지 설정 // Set Destination Height Map image
				if((res = stereoDisparity3D.SetDestinationHeightMapImage(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination Height Map image.\n");
					break;
				}

				// Destination Texture 이미지 설정 // Set Destination Texture image
				if((res = stereoDisparity3D.SetDestinationTextureImage(ref fliTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination Texture image.\n");
					break;
				}

				// Destination 3D Object 설정 // Set Destination 3D Object 
				if((res = stereoDisparity3D.SetDestinationObject(ref fl3DOHM)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination 3D Object.\n");
					break;
				}

				// 최소 허용 Disparity 값 설정 // Set minimum allowed disparity value
				if((res = stereoDisparity3D.SetMinimumDisparity(-20)).IsFail())
				{
					ErrorPrint(res, "Failed to set the minimum allowed disparity value.\n");
					break;
				}

				// 최대 허용 Disparity 값 설정 // Set maximum allowed disparity value
				if((res = stereoDisparity3D.SetMaximumDisparity(0)).IsFail())
				{
					ErrorPrint(res, "Failed to set the maximum allowed disparity value.\n");
					break;
				}

				// Matched Block 크기 설정 // Set matched block size
				if((res = stereoDisparity3D.SetMatchBlockSize(3)).IsFail())
				{
					ErrorPrint(res, "Failed to set the matched block size.\n");
					break;
				}

				// 좌우 간 최대 허용 차이 값 설정 // Set maximum allowed difference value between left and right
				if((res = stereoDisparity3D.SetMaximumDifference(30)).IsFail())
				{
					ErrorPrint(res, "Failed to set the maximum allowed difference.\n");
					break;
				}

				// 고유비 값 설정 // Set uniqueness ratio value
				if((res = stereoDisparity3D.SetUniquenessRatio(0)).IsFail())
				{
					ErrorPrint(res, "Failed to set the uniqueness ratio value.\n");
					break;
				}

				// P1 값 설정 // Set P1 Value
				if((res = stereoDisparity3D.SetP1(200)).IsFail())
				{
					ErrorPrint(res, "Failed to set the P1 Value.\n");
					break;
				}

				// P2 값 설정 // Set P2 Value
				if((res = stereoDisparity3D.SetP2(800)).IsFail())
				{
					ErrorPrint(res, "Failed to set the P2 Value.\n");
					break;
				}

				// Median Morphology 커널 사이즈 설정 // Set median morphology kernel size
				if((res = stereoDisparity3D.SetFilterSize(5)).IsFail())
				{
					ErrorPrint(res, "Failed to set the median kernel size.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = stereoDisparity3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Stereo Disparity 3D.\n");
					break;
				}

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerSrc = viewSourceImage.GetLayer(0);
				CGUIViewImageLayer layerSrc2 = viewSource2Image.GetLayer(0);
				CGUIViewImageLayer layerDst = viewDestinationImage.GetLayer(0);

				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an 3D view and does not need to be released
				CGUIView3DLayer layer3DDestination = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerSrc2.Clear();
				layerDst.Clear();
				layer3DDestination.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerSrc.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerSrc2.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image 2", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerDst.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 3D 뷰 정보 표시 // Display 3D view information
				if((res = layer3DDestination.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination 3D Height Map", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 3D Height Map에 Texture 적용 // Apply texture to 3D height map
				if((res = ((CFL3DObjectHeightMap)fl3DOHM).SetTextureImage(fliTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to apply texture to height map.\n");
					break;
				}

				res = ((CFL3DObjectHeightMap)fl3DOHM).ActivateVertexColorTexture(true);

				// 결과 3D 객체 출력 // Print resulting 3D Object
				if((res = view3DDst.PushObject(fl3DOHM)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D Object.\n");
					break;
				}

				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewDestinationImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 새로 생성한 3D Object를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created 3D object
				if((res = view3DDst.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit 3D view.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewSourceImage.Invalidate(true);
				viewSource2Image.Invalidate(true);
				viewDestinationImage.Invalidate(true);

				// 3D 뷰를 갱신 // Update 3D view
				view3DDst.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewSourceImage.IsAvailable() && viewSource2Image.IsAvailable() && viewDestinationImage.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

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
	class PointCloudToXYZImageConverter3D
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
			CFLImage fliDstXYZVImage = new CFLImage();
			CFLImage fliDstTextureImage = new CFLImage();

			// 3D 객체 선언 // Declare 3D object
			CFL3DObject floSource = new CFL3DObject();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewDstXYZVImage = new CGUIViewImage();
			CGUIViewImage viewDstTextureImage = new CGUIViewImage();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DSrc = new CGUIView3D();

			do
			{
				// 수행 결과 객체 선언 // Declare execution result object
				CResult res = new CResult(EResult.UnknownError);

				// Source Point Cloud 로드 // Load Source Point Cloud
				if((res = floSource.Load("../../ExampleImages/PointCloudToXYZImageConverter3D/3DSrc.ply")).IsFail())
				{
					ErrorPrint(res, "Failed to load the point cloud.\n");
					break;
				}

				// Source 3D 뷰 생성 // Create Source 3D view
				if((res = view3DSrc.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// Destination XYZV 이미지 뷰 생성 // Create Destination XYZV image view
				if((res = viewDstXYZVImage.Create(100, 512, 612, 1024)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination Texture 이미지 뷰 생성 // Create Destination Texture image view
				if((res = viewDstTextureImage.Create(612, 512, 1124, 1024)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination XYZV 이미지 뷰에 이미지를 디스플레이 // Display image in Destination XYZV image view
				if((res = viewDstXYZVImage.SetImagePtr(ref fliDstXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination Texture 이미지 뷰에 이미지를 디스플레이 // Display image in Destination Texture image view
				if((res = viewDstTextureImage.SetImagePtr(ref fliDstTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 // Synchronize viewpoints of two image views
				if((res = viewDstXYZVImage.SynchronizePointOfView(ref viewDstTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize point of view between image views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = view3DSrc.SynchronizeWindow(ref viewDstXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = view3DSrc.SynchronizeWindow(ref viewDstTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// Point Cloud To XYZ Image Converter 3D 객체 생성 // Create Point Cloud To XYZ Image Converter 3D object
				CPointCloudToXYZImageConverter3D pointCloudToXYZImageConverter3D = new CPointCloudToXYZImageConverter3D();

				// Source Point Cloud 설정 // Set Source Point Cloud.
				if((res = pointCloudToXYZImageConverter3D.SetSourceObject(ref floSource)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source 3D object.\n");
					break;
				}

				// Destination XYZV 이미지 설정 // Set Destination XYZV image
				if((res = pointCloudToXYZImageConverter3D.SetDestinationImage(ref fliDstXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination XYZV image.\n");
					break;
				}

				// Destination Texture 이미지 설정 // Set Destination Texture image.
				if((res = pointCloudToXYZImageConverter3D.SetDestinationTextureImage(ref fliDstTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination Texture image.\n");
					break;
				}

				// Destination 이미지 크기 설정 // Set size of destination image
				if((res = pointCloudToXYZImageConverter3D.SetImageSize(140, 200)).IsFail())
				{
					ErrorPrint(res, "Failed to set destination image size.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = pointCloudToXYZImageConverter3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Point Cloud To XYZ Image Converter 3D.\n");
					break;
				}


				// 입력 3D 객체 출력 // Print source 3D Object
				if((res = view3DSrc.PushObject(floSource)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D Object.\n");
					break;
				}

				// 3D View 카메라 설정 // Set 3D view camera
				CFL3DCamera fl3DCam = new CFL3DCamera();

				fl3DCam.SetDirection(new CFLPoint3<float>(0, 0, -1));
				fl3DCam.SetDirectionUp(new CFLPoint3<float>(0, 1, 0));
				fl3DCam.SetPosition(new CFLPoint3<float>(10, -20, 750));

				view3DSrc.SetCamera(fl3DCam);

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerViewXYZV = viewDstXYZVImage.GetLayer(0);
				CGUIViewImageLayer layerViewTexture = viewDstTextureImage.GetLayer(0);

				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an 3D view and does not need to be released
				CGUIView3DLayer layerView3D = view3DSrc.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear figures drawn on existing layer
				layerView3D.Clear();
				layerViewXYZV.Clear();
				layerViewTexture.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerViewXYZV.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination XYZV Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerViewTexture.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Texture Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 3D 뷰 정보 표시 // Display 3D view information
				if((res = layerView3D.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Point Cloud", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}


				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewDstXYZVImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewDstTextureImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewDstXYZVImage.Invalidate(true);
				viewDstTextureImage.Invalidate(true);

				// 3D 뷰를 갱신 // Update 3D view
				view3DSrc.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewDstXYZVImage.IsAvailable() && viewDstTextureImage.IsAvailable() && view3DSrc.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

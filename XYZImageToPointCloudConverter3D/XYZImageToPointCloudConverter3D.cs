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
	class XYZImageToPointCloudConverter3D
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
			CFLImage fliSrcXYZVImage = new CFLImage();
			CFLImage fliSrcTextureImage = new CFLImage();

			// 3D 객체 선언 // Declare 3D object
			CFL3DObject floDestination = new CFL3DObject();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewSrcXYZVImage = new CGUIViewImage();
			CGUIViewImage viewSrcTextureImage = new CGUIViewImage();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DDst = new CGUIView3D();

			do
			{
				// 수행 결과 객체 선언 // Declare execution result object
				CResult res = new CResult(EResult.UnknownError);

				// Source XYZV 이미지 로드 // Load Source XYZV image
				if((res = fliSrcXYZVImage.Load("../../ExampleImages/XYZImageToPointCloudConverter3D/XYZV.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source Texture 이미지 로드 // Load Source Texture image
				if((res = fliSrcTextureImage.Load("../../ExampleImages/XYZImageToPointCloudConverter3D/Texture.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source XYZV 이미지 뷰 생성 // Create Source XYZV image view
				if((res = viewSrcXYZVImage.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source Texture 이미지 뷰 생성 // Create Source Texture image view
				if((res = viewSrcTextureImage.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 3D 뷰 생성 // Create Destination 3D view
				if((res = view3DDst.Create(100, 512, 612, 1024)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// Source XYZV 이미지 뷰에 이미지를 디스플레이 // Display image in Source XYZV image view
				if((res = viewSrcXYZVImage.SetImagePtr(ref fliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Source Texture 이미지 뷰에 이미지를 디스플레이 // Display image in Source Texture image view
				if((res = viewSrcTextureImage.SetImagePtr(ref fliSrcTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 // Synchronize viewpoints of two image views
				if((res = viewSrcXYZVImage.SynchronizePointOfView(ref viewSrcTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize point of view between image views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = view3DDst.SynchronizeWindow(ref viewSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = view3DDst.SynchronizeWindow(ref viewSrcTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// XYZ Image To Point Cloud Converter 3D 객체 생성 // Create XYZ Image To Point Cloud Converter 3D object
				CXYZImageToPointCloudConverter3D xyzImageToPointCloudConverter3D = new CXYZImageToPointCloudConverter3D();

				// Source XYZV 이미지 설정 // Set Source XYZV image
				if((res = xyzImageToPointCloudConverter3D.SetSourceImage(ref fliSrcXYZVImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source XYZV image.\n");
					break;
				}

				// Source Texture 이미지 설정 // Set Source Texture image
				if((res = xyzImageToPointCloudConverter3D.SetTextureImage(ref fliSrcTextureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Texture image.\n");
					break;
				}

				// Destination Point Cloud 설정 // Set Destination Point Cloud
				if((res = xyzImageToPointCloudConverter3D.SetDestinationObject(ref floDestination)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination 3D object.\n");
					break;
				}

				// Coordinate Adjustment Scale 설정 // Set coordinate adjustment scale
				if((res = xyzImageToPointCloudConverter3D.SetCoordinateAdjustmentScale(1, -1, -1)).IsFail())
				{
					ErrorPrint(res, "Failed to set coordinate adjustment scale.\n");
					break;
				}

				// Coordinate Adjustment Offset 설정 // Set coordinate adjustment offset
				if((res = xyzImageToPointCloudConverter3D.SetCoordinateAdjustmentOffset(-41, -5, 900)).IsFail())
				{
					ErrorPrint(res, "Failed to set coordinate adjustment offset.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = xyzImageToPointCloudConverter3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute XYZ Image To Point Cloud Converter 3D.\n");
					break;
				}


				// 결과 3D 객체 출력 // Print resulting 3D Object
				if((res = view3DDst.PushObject(floDestination)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D Object.\n");
					break;
				}

				// 3D View 카메라 설정 // Set 3D view camera
				CFL3DCamera fl3DCam = new CFL3DCamera();

				fl3DCam.SetDirection(new CFLPoint3<float>(0, 0, -1));
				fl3DCam.SetDirectionUp(new CFLPoint3<float>(0, 1, 0));
				fl3DCam.SetPosition(new CFLPoint3<float>(10, -20, 750));

				view3DDst.SetCamera(fl3DCam);

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerViewXYZV = viewSrcXYZVImage.GetLayer(0);
				CGUIViewImageLayer layerViewTexture = viewSrcTextureImage.GetLayer(0);

				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an 3D view and does not need to be released
				CGUIView3DLayer layerView3D = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear figures drawn on existing layer
				layerViewXYZV.Clear();
				layerViewTexture.Clear();
				layerView3D.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerViewXYZV.DrawTextCanvas(new CFLPoint<double>(0, 0), "XYZV Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerViewTexture.DrawTextCanvas(new CFLPoint<double>(0, 0), "Texture Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerView3D.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Point Cloud", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewSrcXYZVImage.Invalidate(true);
				viewSrcTextureImage.Invalidate(true);

				// 3D 뷰를 갱신 // Update 3D view
				view3DDst.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewSrcXYZVImage.IsAvailable() && viewSrcTextureImage.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

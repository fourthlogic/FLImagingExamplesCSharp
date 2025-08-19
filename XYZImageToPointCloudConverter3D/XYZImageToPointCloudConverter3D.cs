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

			CFLImage fliSource = new CFLImage(), fliTexture = new CFLImage();
			CFL3DObject floDestination = new CFL3DObject();
			CGUIView3D view3D = new CGUIView3D();
			CGUIViewImage viewDepthImage = new CGUIViewImage();
			CGUIViewImage viewTextureImage = new CGUIViewImage();
			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load the image
				if((eResult = fliSource.Load("../../ExampleImages/XYZImageToPointCloudConverter3D/XYZV.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image.\n");
					break;
				}

				if((eResult = fliTexture.Load("../../ExampleImages/XYZImageToPointCloudConverter3D/Texture.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image.\n");
					break;
				}

				// XYZImageToPointCloudConverter3D 객체 생성 // Create XYZImageToPointCloudConverter3D object
				CXYZImageToPointCloudConverter3D xyzImageToPointCloudConverter3D = new CXYZImageToPointCloudConverter3D();

				// Scale 설정 // Set the scale
				xyzImageToPointCloudConverter3D.SetCoordinateAdjustmentScale(1, -1, -1);

				// Offset 설정 // Set the offset
				xyzImageToPointCloudConverter3D.SetCoordinateAdjustmentOffset(-41, -5, 900);

				// Source 이미지 설정 // Set the source image.
				xyzImageToPointCloudConverter3D.SetSourceImage(ref fliSource);

				// Texture 이미지 설정 // Set the texture image.
				xyzImageToPointCloudConverter3D.SetTextureImage(ref fliTexture);

				// Destination 3D Object 설정 // Set the destination 3D object
				xyzImageToPointCloudConverter3D.SetDestinationObject(ref floDestination);

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewDepthImage.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Source image view.\n");
					break;
				}

				if((eResult = viewTextureImage.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Texture image view.\n");
					break;
				}

				// 결과 3D 뷰 생성 // Create result 3D view
				if((eResult = view3D.Create(100, 512, 612, 1024)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Result 3D view.\n");
					break;
				}

				// 이미지 포인터 설정 // Set image pointer
				viewDepthImage.SetImagePtr(ref fliSource);
				viewTextureImage.SetImagePtr(ref fliTexture);

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
				CGUIViewImageLayer layerViewDepth = viewDepthImage.GetLayer(0);
				CGUIViewImageLayer layerViewTexture = viewTextureImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerViewDepth.Clear();
				layerViewTexture.Clear();

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas 는 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((eResult = layerViewDepth.DrawTextCanvas(new CFLPoint<double>(0, 0), "Depth Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerViewTexture.DrawTextCanvas(new CFLPoint<double>(0, 0), "Texture Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = xyzImageToPointCloudConverter3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute XYZ Image To Point Cloud Converter 3D.");
					break;
				}

				view3D.PushObject(floDestination);
				view3D.UpdateObject(-1);
				view3D.UpdateScreen();
				view3D.ZoomFit();

				viewDepthImage.ZoomFit();
				viewTextureImage.ZoomFit();

				viewDepthImage.Invalidate();
				viewTextureImage.Invalidate();

				// 이미지 뷰가 종료될 때 까지 기다림
				while(viewDepthImage.IsAvailable() && viewTextureImage.IsAvailable() && view3D.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

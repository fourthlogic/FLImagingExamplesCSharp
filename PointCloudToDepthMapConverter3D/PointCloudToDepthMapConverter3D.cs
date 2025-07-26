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

namespace PointCloudToDepthMapConverter3D
{
	class PointCloudToDepthMapConverter3D
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

			CFLImage fliDestination = new CFLImage(), fliDestinationTexture = new CFLImage();
			CFL3DObject floSource = new CFL3DObject();
			CGUIView3D view3D = new CGUIView3D();
			CGUIViewImage viewDepthImage = new CGUIViewImage();
			CGUIViewImage viewDestinationTextureImage = new CGUIViewImage();
			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load the image
				if((eResult = floSource.Load("../../ExampleImages/PointCloudToDepthMapConverter3D/Example.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image.\n");
					break;
				}

				// PointCloudToDepthMapConverter3D 객체 생성 // Create PointCloudToDepthMapConverter3D object
				CPointCloudToDepthMapConverter3D PointCloudToDepthMapConverter3D = new CPointCloudToDepthMapConverter3D();

				// 이미지 크기 설정 // Set the image size.
				PointCloudToDepthMapConverter3D.SetImageSize(2064, 1544);

				// Destination 이미지 설정 // Set the Destination image.
				PointCloudToDepthMapConverter3D.SetDestinationImage(ref fliDestination);

				// Destination Texture 이미지 설정 // Set the texture image.
				PointCloudToDepthMapConverter3D.SetDestinationImageTexture(ref fliDestinationTexture);

				// Camera Matrix 설정 // Set the camera matrix
				CFLPoint<float> flpFocalLength = new CFLPoint<float>();
				CFLPoint<float> flpPrincipalPoint = new CFLPoint<float>();

				flpFocalLength.x = 2328.800049f;
				flpFocalLength.y = 2330.899902f;
				flpPrincipalPoint.x = 988.599976f;
				flpPrincipalPoint.y = 750.299988f;

				PointCloudToDepthMapConverter3D.SetIntrinsicParameter(flpFocalLength, flpPrincipalPoint);

				//왜곡 계수 설정 // Set the distortion coefficient
				List<double> flaDistortionCoefficient = new List<double>();

				flaDistortionCoefficient.Add(-0.2333453150000);
				flaDistortionCoefficient.Add(0.1352355330000);
				flaDistortionCoefficient.Add(0.0005843197580);
				flaDistortionCoefficient.Add(-0.0005675755000);
				flaDistortionCoefficient.Add(-0.0246060137000);

				PointCloudToDepthMapConverter3D.SetDistortionCoefficient(flaDistortionCoefficient);

				// Z축 방향 설정 // Set z-axis direction.
				PointCloudToDepthMapConverter3D.SetDirectionType(EDirectionType.Increment);

				// Source 3D Object 설정 // Set the source 3D object
				PointCloudToDepthMapConverter3D.SetSourceObject(ref floSource);

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewDepthImage.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Destination image view.\n");
					break;
				}

				if((eResult = viewDestinationTextureImage.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Destination Texture image view.\n");
					break;
				}

				// 결과 3D 뷰 생성 // Create result 3D view
				if((eResult = view3D.Create(100, 512, 612, 1024)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Result 3D view.\n");
					break;
				}

				// 이미지 포인터 설정 // Set image pointer
				viewDepthImage.SetImagePtr(ref fliDestination);
				viewDestinationTextureImage.SetImagePtr(ref fliDestinationTexture);

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
				CGUIViewImageLayer layerViewDepth = viewDepthImage.GetLayer(0);
				CGUIViewImageLayer layerViewDestinationTexture = viewDestinationTextureImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerViewDepth.Clear();
				layerViewDestinationTexture.Clear();

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = PointCloudToDepthMapConverter3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Point Cloud To Depth Map Converter 3D.");
					break;
				}

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas 는 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((eResult = layerViewDepth.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layerViewDestinationTexture.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Texture Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				view3D.PushObject(floSource);
				view3D.UpdateObject(-1);
				view3D.UpdateScreen();
				view3D.ZoomFit();

				viewDepthImage.ZoomFit();
				viewDestinationTextureImage.ZoomFit();

				viewDepthImage.Invalidate();
				viewDestinationTextureImage.Invalidate();

				// 이미지 뷰가 종료될 때 까지 기다림
				while(viewDepthImage.IsAvailable() && viewDestinationTextureImage.IsAvailable() && view3D.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

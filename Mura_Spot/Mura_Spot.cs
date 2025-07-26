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

namespace Mura
{
	class Mura_Spot
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
			CFLImage fliImageSrc = new CFLImage();
			CFLImage fliImageDst = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			do
			{
				CResult res;

				// 이미지 로드 // Load image
				if((res = fliImageSrc.Load("../../ExampleImages/Mura/Spot.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 548, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliImageSrc)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImageSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageDst.Create(548, 0, 996, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageDst.SetImagePtr(ref fliImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					Console.WriteLine("Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					Console.WriteLine("Failed to synchronize view\n");
					break;
				}

				// Mura 객체 생성 // Create Mura object
				CMura sMura = new CMura();

				// 처리할 이미지 설정 // Set the image to process
				sMura.SetSourceImage(ref fliImageSrc);
				// Auto Threshold 모드 설정 // Set auto threshold mode
				sMura.EnableAutoThresholdMode(true);
				// Kernel Size Rate 설정 // Set kernel size rate
				sMura.SetKernelSizeRate(0.25);
				// Mura Color Type 설정 // Set mura color type
				sMura.SetMuraColorType(CMura.EMuraColorType.All);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sMura.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Mura.");
					break;
				}

				sMura.GetResultMuraImage(ref fliImageDst);

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageDst.SetImagePtr(ref fliImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Mura 결과를 얻어오기 위해 FigureArray 선언
				CFLFigureArray flfaContours = new CFLFigureArray();

				// Mura 결과들 중 Contour를 얻어옴
				if((res = sMura.GetResultContours(ref flfaContours)).IsFail())
				{
					ErrorPrint(res, "Failed to get boundary rects from the Mura object.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// flfaContours 는 Figure들의 배열이기 때문에 Layer에 넣기만 해도 모두 드로윙이 가능하다.
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 여기서 0.25이므로 옅은 반투명 상태라고 볼 수 있다.
				// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
				if((res = layer.DrawFigureImage(flfaContours, EColor.RED, 1, EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
					break;
				}

				// Rect 정보값을 각각 확인하는 코드
				for(Int64 i = 0; i < flfaContours.GetCount(); ++i)
				{
					CFLRegion pFlrg = (CFLRegion)flfaContours.GetAt(i);

					string str = string.Format("{0}", i);

					layer.DrawTextImage(pFlrg.GetCenter(), str, EColor.CYAN);
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

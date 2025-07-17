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

namespace AlignmentTiling
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
			int i32SrcImageCount = 4;

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();
			CFLImage[] fliSrcImages = new CFLImage[i32SrcImageCount];

			for(int i = 0; i < i32SrcImageCount; ++i)
				fliSrcImages[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImageSrc = new CGUIViewImage[i32SrcImageCount];
			CGUIViewImage viewImageDst = new CGUIViewImage();

			for(int i = 0; i < i32SrcImageCount; ++i)
				arrViewImageSrc[i] = new CGUIViewImage();

			CResult res;

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = (fliSrcImage.Load("../../ExampleImages/AlignmentTiling/AlignmentTilingExampleImage.flif"))).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create the source image view
				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					if(i < 2)
					{
						if((res = (arrViewImageSrc[i].Create(100 + 300 * i, 0, 400 + 300 * i, 300))).IsFail())
						{
							ErrorPrint(res, "Failed to create the image view.\n");
							break;
						}
					}
					else
					{
						if((res = (arrViewImageSrc[i].Create(100 + 300 * (i - 2), 300, 400 + 300 * (i - 2), 600))).IsFail())
						{
							ErrorPrint(res, "Failed to create the image view.\n");
							break;
						}
					}

					arrViewImageSrc[i].SetFixThumbnailView(true);
				}

				// Source 이미지 뷰에 페이지에 각각 존재하는 이미지를 각각에 뷰에 디스플레이 하기 위해 얕은 복사로 각각의 이미지 객체에 할당.
				// Assign each image object to each image object by shallow copying to display each image on the page in the source image view to each view.
				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					// 원본이미지가 가진 4개의 페이지들을 각 페이지 마다 디스플레이 하기 위해 원본을 바탕으로 얕은 복사(Shallow copy)를 수행
					// Perform a shallow copy based on the original to display the 4 pages of the original image for each page
					fliSrcImages[i].Assign(fliSrcImage, false);
					// 얕은 복사된 이미지 객체에 대해 표시할 페이지를 선택 (이미지 데이터는 공유하지만, 페이지 등의 설정은 개별적으로 동작)
					// Select a page to display for shallow copied image objects (Image data is shared, but settings such as pages work individually)
					fliSrcImages[i].SelectPage(i);

					// 얕은 복사된 해당 페이지를 선택한 이미지를 뷰에 디스플레이
					// Display the selected image of the shallow-copied page in the view
					if((res = (arrViewImageSrc[i].SetImagePtr(ref fliSrcImages[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}
				}


				// Destination 이미지 뷰 생성 // Create the destination image view
				if((res = (viewImageDst.Create(912, 0, 1424, 612))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((res = (viewImageDst.SetImagePtr(ref fliDstImage))).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰 윈도우의 위치를 맞춤 // Align the position of the image view window
				for(int i = 1; i < i32SrcImageCount; ++i)
				{
					if((res = (arrViewImageSrc[0].SynchronizeWindow(ref arrViewImageSrc[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						break;
					}
				}

				if((res = (arrViewImageSrc[0].SynchronizeWindow(ref viewImageDst))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}


				// AlignmentTiling 객체 생성 // Create AlignmentTiling object
				CAlignmentTiling alignmentTiling = new CAlignmentTiling();

				// Source 이미지 설정 // Set the source image
				alignmentTiling.SetSourceImage(ref fliSrcImage);
				// Destination 이미지 설정 // Set the destination image
				alignmentTiling.SetDestinationImage(ref fliDstImage);
				// 공백 영역 색상 지정 // Set the blank color
				CMultiVar<double> mvBlankColor = new CMultiVar<double>(200);
				alignmentTiling.SetBlankColor(mvBlankColor);
				// Interpolation 옵션 설정 // Set the Interpolation option
				alignmentTiling.SetInterpolationMethod(EInterpolationMethod.Bilinear);
				// 병합할 이미지의 배열 설정 (Row, Column) // Set the array of images to be merged (Row, Column)
				alignmentTiling.SetImageArrangement(2, 2);
				// 결과 이미지의 크기 지정유무 옵션 설정 // Set whether or not to specify the size of the resulting image
				alignmentTiling.SetResizeMethod(EResizeMethod.Resize);
				// Resize일 경우, 결과 이미지의 크기 설정 // In case of Resize, set the size of the resulting image
				alignmentTiling.SetResultImageSize(1500, 1500);
				// 보정에 사용할 Accuracy 설정 // Set the Accuracy to be used for calibration
				alignmentTiling.SetAccuracy(EFloatingPointAccuracy.Bit32);
				// 결과 이미지의 중심이 될 위치를 설정 // Set the location to be the center of the resulting image
				CFLPoint<double> flpPivotPoint = new CFLPoint<double>(192, 778);
				alignmentTiling.SetSourcePivot(flpPivotPoint);
				// Pivot이 지정된 Page Index를 설정 // Set the Page Index with Pivot specified
				alignmentTiling.SetPivotPageIndex(1);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (alignmentTiling.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute alignment tiling.");
					break;
				}

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다.
				// With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				if((res = (viewImageDst.ZoomFit())).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit of the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] arrLayerSrc = new CGUIViewImageLayer[i32SrcImageCount];
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayerSrc[i] = arrViewImageSrc[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayerSrc[i].Clear();
				}

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerDst.Clear();

				CFLQuad<double> flqResult = new CFLQuad<double>();

				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					// 결과 이미지 기준으로 각각의 인덱스에 해당하는 병합된 결과 영역을 받아온다.
					// Receive the merged result area corresponding to each index based on the result image.
					alignmentTiling.GetPartialResultRegion(i, ref flqResult);

					// 병합된 영역이 어디인지 알기 위해 디스플레이 한다
					// FLImaging의 Figure 객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능 // FLimaging's Figure objects can be displayed as a function regardless of the shape
					// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
					// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
					// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
					if((res = (layerDst.DrawFigureImage(flqResult, EColor.LIME))).IsFail())
						ErrorPrint(res, "Failed to draw figure.\n");
				}

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpZero = new CFLPoint<double>(0, 0);

				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					string str = string.Format("Source Image {0}", i);

					if((res = (arrLayerSrc[i].DrawTextCanvas(flpZero, str, EColor.YELLOW, EColor.BLACK, 20))).IsFail())
					{
						ErrorPrint(res, "Failed to draw text.\n");
						break;
					}
				}

				if((res = (layerDst.DrawTextCanvas(flpZero, "Destination Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신한다. // Update the image view.
				for(int i = 0; i < i32SrcImageCount; ++i)
					arrViewImageSrc[i].RedrawWindow();

				viewImageDst.Invalidate(false);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImageSrc[0].IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
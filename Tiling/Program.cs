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
using Microsoft.SqlServer.Server;

namespace Tiling
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
			CFLImage[] arrFliImage = new CFLImage[2];
			CFLImage[] arrViewImages = new CFLImage[i32SrcImageCount];

			for(int i = 0; i < i32SrcImageCount; ++i)
				arrViewImages[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[5];

			for(int i = 0; i < 2; ++i)
			{
				arrFliImage[i] = new CFLImage();
			}

			for(int i = 0; i < 5; ++i)
			{
				arrViewImage[i] = new CGUIViewImage();
			}

			CResult eResult;

			do
			{
				// Source 이미지 로드 // Load the source image
				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					string strFileName = "../../ExampleImages/Tiling/TilingSourceImage" + i.ToString() + ".flif";

					if((eResult = (arrViewImages[i].Load(strFileName))).IsFail())
					{
						ErrorPrint(eResult, "Failed to load the image file.\n");
						break;
					}
				}

				// 여러 장의 이미지를 하나의 FLImage로 생성 // Create multiple images into one FLImage
				arrFliImage[0] = arrViewImages[0];

				for(int i = 1; i < i32SrcImageCount; ++i)
				{
					arrFliImage[0].PushBackPage(arrViewImages[i]);
				}

				if((eResult = (arrFliImage[1].Load("../../ExampleImages/Tiling/TilingDestinationImage.flif"))).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}
				// Source 이미지 뷰 생성 // Create the source image view
				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					if(i < 2)
					{
						if((eResult = (arrViewImage[i].Create(100 + 300 * i, 0, 400 + 300 * i, 300))).IsFail())
						{
							ErrorPrint(eResult, "Failed to create the image view.\n");
							break;
						}
					}
					else
					{
						if((eResult = (arrViewImage[i].Create(100 + 300 * (i - 2), 300, 400 + 300 * (i - 2), 600))).IsFail())
						{
							ErrorPrint(eResult, "Failed to create the image view.\n");
							break;
						}
					}

					arrViewImage[i].SetFixThumbnailView(true);
				}

				// Source 이미지 뷰에 페이지에 각각 존재하는 이미지를 각각에 뷰에 디스플레이 하기 위해 얕은 복사로 각각의 이미지 객체에 할당.
				// Assign each image object to each image object by shallow copying to display each image on the page in the source image view to each view.
				for(int i = 0; i < i32SrcImageCount; ++i)
				{					
					// 이미지를 뷰에 디스플레이
					// Display the image
					if((eResult = (arrViewImage[i].SetImagePtr(ref arrViewImages[i]))).IsFail())
					{
						ErrorPrint(eResult, "Failed to set image object on the image view.\n");
						break;
					}
				}

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((eResult = (arrViewImage[i32SrcImageCount].Create(912, 0, 1424, 612))).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((eResult = (arrViewImage[i32SrcImageCount].SetImagePtr(ref arrFliImage[1]))).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 이미지 뷰 윈도우의 위치를 맞춤 // Align the position of the image view window
				for(int i = 1; i < 5; ++i)
				{
					if((eResult = (arrViewImage[0].SynchronizeWindow(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(eResult, "Failed to synchronize window.\n");
						break;
					}
				}

				// Source 이미지에 ROI 추가 // Add ROI to source image
				CFLRect<double> flRect = new CFLRect<double>();
				flRect.Set(30, 68, 200, 235);

				flRect.SetName("0");
				arrFliImage[0].SelectPage(0);
				arrFliImage[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));
				arrViewImages[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));

				flRect.Set(260, 135, 415, 440);
				flRect.SetName("1");
				arrFliImage[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));
				arrViewImages[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));

				flRect.Set(280, 250, 480, 480);
				flRect.SetName("0");
				arrFliImage[0].SelectPage(1);
				arrFliImage[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));
				arrViewImages[1].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));

				flRect.Set(110, 150, 350, 440);
				flRect.SetName("0");
				arrFliImage[0].SelectPage(2);
				arrFliImage[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));
				arrViewImages[2].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));

				flRect.Set(220, 230, 470, 450);
				flRect.SetName("0");
				arrFliImage[0].SelectPage(3);
				arrFliImage[0].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));
				arrViewImages[3].PushBackFigure(CFigureUtils.ConvertFigureObjectToString(flRect));

				// Destination 이미지에 ROI 추가 // Add ROI to destination image
				arrFliImage[1].PushBackFigure("D(79.292035, 67.964602, 292.247788, 267.327434, INFO[NAME(0_0)])");
				arrFliImage[1].PushBackFigure("D(296.778761, 271.858407, 459.893805, 444.035398, INFO[NAME(0_1)])");
				arrFliImage[1].PushBackFigure("D(88.353982, 738.548673, 337.557522, 956.035398, INFO[NAME(1_0)])");
				arrFliImage[1].PushBackFigure("D(482.548673, 457.628319, 659.256637, 675.115044, INFO[NAME(2_0)])");
				arrFliImage[1].PushBackFigure("D(659.256638, 222.017700, 835.964602, 439.504425, INFO[NAME(3_0)])");

				// Tiling 객체 생성 // Create Tiling object

				// Tiling 객체 생성 // Create Tiling object
				CTiling tiling = new CTiling();

				// Source 이미지 설정 // Set the source image
				tiling.SetSourceImage(ref arrFliImage[0]);
				// Destination 이미지 설정 // Set the destination image
				tiling.SetDestinationImage(ref arrFliImage[1]);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = (tiling.Execute())).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute tiling.");
					break;
				}

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다.
				// With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				if((eResult = (arrViewImage[i32SrcImageCount].ZoomFit())).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit of the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] arrLayerSrc = new CGUIViewImageLayer[5];
				CGUIViewImageLayer layerDst = arrViewImage[i32SrcImageCount].GetLayer(0);

				for(int i = 0; i < i32SrcImageCount; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayerSrc[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayerSrc[i].Clear();
				}

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerDst.Clear();


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

					if((eResult = (arrLayerSrc[i].DrawTextCanvas(flpZero, str, EColor.YELLOW, EColor.BLACK, 20))).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw text.\n");
						break;
					}
				}

				if((eResult = (layerDst.DrawTextCanvas(flpZero, "Destination Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신한다. // Update the image view.
				for(int i = 0; i < 5; ++i)
					arrViewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImage[0].IsAvailable() && arrViewImage[i32SrcImageCount].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
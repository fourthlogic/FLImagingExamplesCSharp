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

namespace ImageFigureAttribute
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
			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = { new CGUIViewImage(), new CGUIViewImage() };

			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = { new CFLImage(), new CFLImage() };

			CResult res;

			do
			{
				// View 1 생성 // Create View 1
				if((res = (arrViewImage[0].Create(200, 0, 700, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// View 2 생성 // Create View 2
				if((res = (arrViewImage[1].Create(700, 0, 1200, 500))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 각 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each image view.
				if((res = (arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = (arrViewImage[0].SynchronizeWindow(ref arrViewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Source 이미지 로드 // Load the source image
				if((res = arrFliImage[0].Load("../../ExampleImages/Figure/ImageWithFigure2.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				CFLFigureArray flfaSource;

				// Source Image 에 있는 Figure 추출 // Extract figure from source image
				if((res = CFigureUtilities.ConvertImageFiguresToFigureArray(arrFliImage[0], out flfaSource)).IsFail())
				{
					ErrorPrint(res, "Failed to convert image figures to figure array.\n");
					break;
				}

				// Source Image 에 있는 Figure Clear // Figure Clear in Source Image
				arrFliImage[0].ClearFigures();

				// arrFliImage[1] 이미지를 arrFliImage[0] 이미지와 동일한 이미지로 생성
				// Create the arrFliImage[1] image as the same image as the arrFliImage[0] image.
				if((res = arrFliImage[1].Assign(arrFliImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				for(int i = 0; i < 2; ++i)
				{
					if((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// Source Image 에 Source FigureArray 를 추가 // Add Source FigureArray to Source Image
				if((res = CFigureUtilities.ConvertFigureArrayToImageFigures(flfaSource, arrFliImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to convert figure array to image figures.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] layer = { arrViewImage[0].GetLayer(0), arrViewImage[1].GetLayer(0) };

				// 화면상 좌표(고정 좌표)에 View 의 이름을 표시
				// Indicates view name on screen coordinates (fixed coordinates)
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 0), "Source View", EColor.YELLOW, EColor.BLACK, 30);
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 0), "Change Attribute", EColor.YELLOW, EColor.BLACK, 30);

				CFLFigureArray flfaAttribute = new CFLFigureArray(flfaSource);

				// ImageFigureAttribute 클래스 선언 // ImageFigureAttribute class declaration
				CImageFigureAttribute ifa = new CImageFigureAttribute();

				for(long i = 0; i < flfaAttribute.GetCount(); ++i)
				{
					// Source figure 설정 // Source figure settings
					ifa.SetSourceFigure(flfaAttribute.GetAt(i));

					// Figure 이름 추출 // Get figure name
					string strFigureName = flfaAttribute.GetAt(i).GetName();

					// Figure 이름별 속성 지정 // Specifying properties by figure name
					if(strFigureName.CompareTo("Rubber") == 0)
					{
						// 채우기 색상 설정 // Set fill color
						ifa.SetFillColor((uint)EColor.LIME);
						// 이름 설정 // Set name
						ifa.SetName("Block");
					}
					else if(strFigureName.CompareTo("SN") == 0)
					{
						// 채우기 색상 설정 // Set fill color
						ifa.SetFillColor((uint)EColor.KHAKI);
						// 채우기 색상 투명도 설정 // Set fill color alpha ratio
						ifa.SetFillColorAlphaRatio(0.15f);
						// 이름 설정 // Set name
						ifa.SetName("S/N");
					}
					else if(strFigureName.CompareTo("Flux") == 0)
					{
						// 채우기 색상 설정 // Set fill color
						ifa.SetFillColor((uint)EColor.BLUE);
						// 선 색상 설정 // Set line color
						ifa.SetLineColor((uint)EColor.WHITE);
						// 선 두께 설정 // Set line width
						ifa.SetLineWidth(3);
						// 이름 설정 // Set name
						ifa.SetName("");
					}

					// 알고리즘 동작 // Execute algorithm
					if((res = ifa.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute.\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// arrFliImage[1] Image 에 flfaAttribute 를 추가 // Add flfaAttribute to arrFliImage[1] Image
				if((res = CFigureUtilities.ConvertFigureArrayToImageFigures(flfaAttribute, arrFliImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to convert figure array to image figures.\n");
					break;
				}

				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 2; ++i)
					arrViewImage[i].Invalidate(true);

				// 이미지 뷰가 둘중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the two image views are turned off
				while(arrViewImage[0].IsAvailable() && arrViewImage[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

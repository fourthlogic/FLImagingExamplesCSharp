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


namespace InpaintingTexture
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
            CFLImage[] arrFliImage = new CFLImage[4];

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage[] arrViewImage = new CGUIViewImage[4];

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			for(int i = 0; i < 4; ++i)
            {
                arrFliImage[i] = new CFLImage();
                arrViewImage[i] = new CGUIViewImage();
            }

            do
            {
                // Source 이미지 로드 // Load the source image
                if ((res = arrFliImage[0].Load("../../ExampleImages/InpaintingTexture/Seville.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
				}

				// Source 이미지 2 로드 // Load the source image
				if((res = arrFliImage[2].Load("../../ExampleImages/InpaintingTexture/Wood.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
				if ((res = arrFliImage[1].Assign(arrFliImage[0])).IsFail())
                {
                    ErrorPrint(res, "Failed to assign the image file.\n");
                    break;
				}

				// Destination 이미지 2를 Source 이미지 2와 동일한 이미지로 생성 // Create destination image as same as source image
				if((res = arrFliImage[3].Assign(arrFliImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if ((res = arrViewImage[0].Create(400, 0, 800, 400)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

				// Source 이미지 뷰 2 생성 // Create source image view 2
				if((res = arrViewImage[2].Create(400, 400, 800, 800)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if ((res = arrViewImage[1].Create(800, 0, 1200, 400)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
				}

				// Destination 이미지 2 뷰 생성 // Create destination image view 2
				if((res = arrViewImage[3].Create(800, 400, 1200, 800)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				bool bError = false;

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                for (int i = 0; i < 4; ++i)
                {
                    if ((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
                    {
                        ErrorPrint(res, "Failed to set image object on the image view.\n");
                        bError = true;
                        break;
                    }
                }

                if (bError)
                    break;

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[0].SynchronizePointOfView(arrViewImage[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(arrViewImage[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = arrViewImage[2].SynchronizePointOfView(arrViewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = arrViewImage[2].SynchronizeWindow(arrViewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = arrViewImage[2].SynchronizeWindow(arrViewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// InpaintingTexture 객체 생성 // Create InpaintingTexture object
				CInpaintingTexture InpaintingTexture = new CInpaintingTexture();
                // Source 이미지 설정 // Set the source image
                InpaintingTexture.SetSourceImage(ref arrFliImage[0]);
                // Destination 이미지 설정 // Set the destination image
                InpaintingTexture.SetDestinationImage(ref arrFliImage[1]);
				// Patching을 진행할 Mask의 크기 설정 // Set the size of the mask for patching
				InpaintingTexture.SetMaskSize(13);
				// Patching의 Source가 되는 Mask를 찾기 위한 검색 영역의 크기 설정 // Set the size of the search area to find the mask that is the source of the patching
				InpaintingTexture.SetSearchSize(100);
				// Search step size 설정 // Set the search step size
				InpaintingTexture.SetSearchStepSize(1);
				// 매치를 위한 Gradient Value 곱 계수 설정 // Set a coefficient multiplied by gradient value for match
				InpaintingTexture.SetAnisotropy(1);

				CFLFigureArray flfaPaintingRegion = new CFLFigureArray();

                // 미리 그려둔 Painting region Figure Array 불러오기 // Load Pre-drawn Painting Region Figure Array
				if((res = flfaPaintingRegion.Load("../../ExampleImages/InpaintingTexture/PaintingRegion.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.");
					break;
				}

				// Inpainting을 위한 Painting region 설정 // Set painting region for Inpainting
				InpaintingTexture.SetPaintingRegion(flfaPaintingRegion);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = InpaintingTexture.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute algorithm.\n");
					break;
                }

				// Source 이미지 설정 // Set the source image
				InpaintingTexture.SetSourceImage(ref arrFliImage[2]);
				// Destination 이미지 설정 // Set the destination image
				InpaintingTexture.SetDestinationImage(ref arrFliImage[3]);
				// Patching을 진행할 Mask의 크기 설정 // Set the size of the mask for patching
				InpaintingTexture.SetMaskSize(15);
				// Patching의 Source가 되는 Mask를 찾기 위한 검색 영역의 크기 설정 // Set the size of the search area to find the mask that is the source of the patching
				InpaintingTexture.SetSearchSize(-1);
				// Search step size 설정 // Set the search step size
				InpaintingTexture.SetSearchStepSize(1);
				// 매치를 위한 Gradient Value 곱 계수 설정 // Set a coefficient multiplied by gradient value for match
				InpaintingTexture.SetAnisotropy(0);

				CFLFigureArray flfaPaintingRegion2 = new CFLFigureArray();

				// 미리 그려둔 Painting region 2 Figure Array 불러오기 // Load Pre-drawn Painting Region 2 Figure Array
				if((res = flfaPaintingRegion2.Load("../../ExampleImages/InpaintingTexture/PaintingRegion2.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load the figure file.");
					break;
				}

				// Inpainting을 위한 Painting region 2 설정 // Set painting region 2 for Inpainting
				InpaintingTexture.SetPaintingRegion(flfaPaintingRegion2);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = InpaintingTexture.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute algorithm.\n");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[4];

                for (int i = 0; i < 4; ++i)
                {
                    // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                    // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                    arrLayer[i] = arrViewImage[i].GetLayer(0);

                    // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                    arrLayer[i].Clear();
                }

                // 이미지 뷰 정보 표시 // Display image view information
                // 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
                // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
                // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                // Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
                //                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
                TPoint<double> tpPosition = new TPoint<double>(0, 0);

                if ((res = arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[1].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
				}

				if((res = arrLayer[2].DrawTextCanvas(tpPosition, "Source Image 2", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[3].DrawTextCanvas(tpPosition, "Destination Image 2", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// Painting region을 source image에 디스플레이 // Display painting region on the source image
				if(arrFliImage[0].PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(flfaPaintingRegion)) == -1)
				{
					ErrorPrint(res, "Failed to push figure on image\n");
					break;
				}

				// Painting region 2을 source image 2에 디스플레이 // Display painting region on the source image
				if(arrFliImage[2].PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(flfaPaintingRegion2)) == -1)
				{
					ErrorPrint(res, "Failed to push figure on image\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				arrViewImage[0].Invalidate(true);
                arrViewImage[1].Invalidate(true);
				arrViewImage[2].Invalidate(true);
				arrViewImage[3].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while (arrViewImage[0].IsAvailable()
                      && arrViewImage[1].IsAvailable() && arrViewImage[2].IsAvailable()
					  && arrViewImage[3].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

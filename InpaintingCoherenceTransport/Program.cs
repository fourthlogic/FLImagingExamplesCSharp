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


namespace InpaintingCoherenceTransport
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
            CFLImage[] arrFliImage = new CFLImage[2];

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage[] arrViewImage = new CGUIViewImage[2];

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			for(int i = 0; i < 2; ++i)
            {
                arrFliImage[i] = new CFLImage();
                arrViewImage[i] = new CGUIViewImage();
            }

            do
            {
                // Source 이미지 로드 // Load the source image
                if ((eResult = arrFliImage[0].Load("../../ExampleImages/InpaintingCoherenceTransport/owl.flif")).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load the image file.\n");
                    break;
				}

				// Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
				if ((eResult = arrFliImage[1].Assign(arrFliImage[0])).IsFail())
                {
                    ErrorPrint(eResult, "Failed to assign the image file.\n");
                    break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if ((eResult = arrViewImage[0].Create(400, 0, 800, 400)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.\n");
                    break;
                }

				// Destination 이미지 뷰 생성 // Create destination image view
				if ((eResult = arrViewImage[1].Create(800, 0, 1200, 400)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.\n");
                    break;
				}

				bool bError = false;

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                for (int i = 0; i < 2; ++i)
                {
                    if ((eResult = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
                    {
                        ErrorPrint(eResult, "Failed to set image object on the image view.\n");
                        bError = true;
                        break;
                    }
                }

                if (bError)
                    break;

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((eResult = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1])).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((eResult = arrViewImage[0].SynchronizeWindow(ref arrViewImage[1])).IsFail())
                {
                    ErrorPrint(eResult, "Failed to synchronize window.\n");
                    break;
                }

				// InpaintingCoherenceTransport 객체 생성 // Create InpaintingCoherenceTransport object
				CInpaintingCoherenceTransport InpaintingCoherenceTransport = new CInpaintingCoherenceTransport();
                // Source 이미지 설정 // Set the source image
                InpaintingCoherenceTransport.SetSourceImage(ref arrFliImage[0]);
                // Destination 이미지 설정 // Set the destination image
                InpaintingCoherenceTransport.SetDestinationImage(ref arrFliImage[1]);
				// Inpainting에 사용될 픽셀 영역 설정 // Setting the pixel area to be used for Inpainting
				InpaintingCoherenceTransport.SetEpsilon(5);
				// 선명도 % // Sharpness parameter(%)
				InpaintingCoherenceTransport.SetKappa(25);
				// 미분 평활화(가우사안) // Smoothing for derivative operator.(Gaussian)
				InpaintingCoherenceTransport.SetSigma(1.400000);
				// 확산 계수(가우사안) // Smoothing for diffusion coefficients.(Gaussian)
				InpaintingCoherenceTransport.SetRho(5.000000);
                // Channel 가중치 // Channel Weight
                CMultiVar<double> mvChannelWeight = new CMultiVar<double>(0.114, 0.587, 0.299);
				InpaintingCoherenceTransport.SetWeightOfChannels(mvChannelWeight);

				CFLRegion flrInpaintingRegion = new CFLRegion();

                // 미리 그려둔 Painting region Figure Array 불러오기 // Load Pre-drawn Painting Region Figure Array
				if((eResult = flrInpaintingRegion.Load("../../ExampleImages/InpaintingCoherenceTransport/InpaintingRegionOwl.fig")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the figure file.");
					break;
				}

				// Inpainting을 위한 Painting region 설정 // Set painting region for Inpainting
				InpaintingCoherenceTransport.SetPaintingRegion(flrInpaintingRegion);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = InpaintingCoherenceTransport.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute algorithm.\n");
					break;
                }			

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[4];

                for (int i = 0; i < 2; ++i)
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

                if ((eResult = arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                if ((eResult = arrLayer[1].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
				}

				// Painting region을 source image에 디스플레이 // Display painting region on the source image
				if(arrFliImage[0].PushBackFigure(CROIUtilities.ConvertFigureObjectToString(flrInpaintingRegion)) == -1)
				{
					ErrorPrint(eResult, "Failed to push figure on image\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				arrViewImage[0].Invalidate(true);
                arrViewImage[1].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while (arrViewImage[0].IsAvailable() && arrViewImage[1].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

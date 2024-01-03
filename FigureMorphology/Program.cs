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

namespace OperationBitwiseAnd
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // 이미지 객체 선언 // Declare the image object
            CFLImage[] arrFliImage = new CFLImage[5];

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage[] arrViewImage = new CGUIViewImage[5];

            for (int i = 0; i < 5; ++i)
            {
                arrViewImage[i] = new CGUIViewImage();
            }

            do
            {
               
                // Source 이미지 뷰 생성 // Create source image view
                if ((arrViewImage[0].Create(100, 0, 356, 256)).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                // Destination1 이미지 뷰 생성 // Create destination1 image view
                if ((arrViewImage[1].Create(100, 256, 356, 512)).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                // Destination2 이미지 뷰 생성 // Create destination2 image view
                if ((arrViewImage[2].Create(356, 256, 612, 512)).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

				// Destination3 이미지 뷰 생성 // Create the destination3 image view
				if ((arrViewImage[3].Create(100, 512, 356, 768)).IsFail())
				{
					Console.WriteLine("Failed to create the image view.\n");
					break;
				}

				// Destination4 이미지 뷰 생성 // Create the destination4 image view
				if ((arrViewImage[4].Create(356, 512, 612, 768)).IsFail())
				{
					Console.WriteLine("Failed to create the image view.\n");
					break;
				}

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((arrViewImage[0].SynchronizePointOfView(ref arrViewImage[2])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((arrViewImage[0].SynchronizePointOfView(ref arrViewImage[3])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((arrViewImage[0].SynchronizeWindow(ref arrViewImage[1])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((arrViewImage[0].SynchronizeWindow(ref arrViewImage[2])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize window.\n");
                    break;
                }

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if ((arrViewImage[0].SynchronizeWindow(ref arrViewImage[3])).IsFail())
				{
					Console.WriteLine("Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if ((arrViewImage[0].SynchronizeWindow(ref arrViewImage[4])).IsFail())
				{
					Console.WriteLine("Failed to synchronize window.\n");
					break;
				}

				// 알고리즘 동작 결과 // Algorithm execution result
				CResult eResult = new CResult();

				// Utility FigureMorphology 객체 생성 // Create Utility FigureMorphology object
				CFigureMorphology figureMorphology = new CFigureMorphology();

				// FigureMorphology Source ROI Setting
				CFLFigureArray flfSourceFigure = new CFLFigureArray();
				flfSourceFigure.PushBack(new CFLRect<double>(125, 100, 225, 150, 0.000000));
				flfSourceFigure.PushBack(new CFLRect<double>(15, 100, 115, 150, 0.000000));
				figureMorphology.SetSourceFigure(flfSourceFigure);
				// FigureMorphology FigureMorphologyMethod Setting
				figureMorphology.SetFigureMorphologyMethod(CFigureMorphology.EFigureMorphologyMethod.Erode);
				figureMorphology.SetKernel(5, 5);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((eResult = figureMorphology.Execute()).IsFail())
                {
                    Console.WriteLine("Failed to execute flip.");
                    Console.WriteLine(eResult.GetString());
                    break;
                }

				CFLFigureArray flfResultFigureErode = new CFLFigureArray();
				figureMorphology.GetResultFigure(out flfResultFigureErode);

				// FigureMorphology FigureMorphologyMethod Setting
				figureMorphology.SetFigureMorphologyMethod(CFigureMorphology.EFigureMorphologyMethod.Dilate);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if ((eResult = figureMorphology.Execute()).IsFail())
                {
                    Console.WriteLine("Failed to execute flip.");
                    Console.WriteLine(eResult.GetString());
                    break;
                }

				CFLFigureArray flfResultFigureDilate = new CFLFigureArray();
				figureMorphology.GetResultFigure(out flfResultFigureDilate);

				// FigureMorphology FigureMorphologyMethod Setting
				figureMorphology.SetFigureMorphologyMethod(CFigureMorphology.EFigureMorphologyMethod.Open);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if ((eResult = figureMorphology.Execute()).IsFail())
				{
					Console.WriteLine("Failed to execute flip.");
					Console.WriteLine(eResult.GetString());
					break;
				}

				CFLFigureArray flfResultFigureOpen = new CFLFigureArray();
				figureMorphology.GetResultFigure(out flfResultFigureOpen);

				// FigureMorphology FigureMorphologyMethod Setting
				figureMorphology.SetFigureMorphologyMethod(CFigureMorphology.EFigureMorphologyMethod.Close);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if ((eResult = figureMorphology.Execute()).IsFail())
				{
					Console.WriteLine("Failed to execute flip.");
					Console.WriteLine(eResult.GetString());
					break;
				}

				CFLFigureArray flfResultFigureClose = new CFLFigureArray();
				figureMorphology.GetResultFigure(out flfResultFigureClose);

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[5];
                arrLayer[0] = new CGUIViewImageLayer();
                arrLayer[1] = new CGUIViewImageLayer();
                arrLayer[2] = new CGUIViewImageLayer();
                arrLayer[3] = new CGUIViewImageLayer();
				arrLayer[4] = new CGUIViewImageLayer();

				for (int i = 0; i < 5; ++i)
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

                if ((arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    Console.WriteLine("Failed to draw text\n");
                    break;
                }

                if ((arrLayer[1].DrawTextCanvas(tpPosition, "Erode Method", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    Console.WriteLine("Failed to draw text\n");
                    break;
                }

                if ((arrLayer[2].DrawTextCanvas(tpPosition, "Dilate Method", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    Console.WriteLine("Failed to draw text\n");
                    break;
                }

				if ((arrLayer[3].DrawTextCanvas(tpPosition, "Open Method", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					Console.WriteLine("Failed to draw text\n");
					break;
				}

				if ((arrLayer[4].DrawTextCanvas(tpPosition, "Close Method", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					Console.WriteLine("Failed to draw text\n");
					break;
				}

				// ROI영역을 디스플레이 한다 // Display ROI
				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				if (arrLayer[0].DrawFigureImage(flfSourceFigure, EColor.RED).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[1].DrawFigureImage(flfSourceFigure, EColor.RED).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[2].DrawFigureImage(flfSourceFigure, EColor.RED).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[3].DrawFigureImage(flfSourceFigure, EColor.RED).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[4].DrawFigureImage(flfSourceFigure, EColor.RED).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[1].DrawFigureImage(flfResultFigureErode, EColor.GREEN).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[2].DrawFigureImage(flfResultFigureDilate, EColor.GREEN).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[3].DrawFigureImage(flfResultFigureOpen, EColor.GREEN).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				if (arrLayer[4].DrawFigureImage(flfResultFigureClose, EColor.GREEN).IsFail())
				{
					Console.WriteLine("Failed to draw figures objects on the image view.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				arrViewImage[0].Invalidate(true);
                arrViewImage[1].Invalidate(true);
                arrViewImage[2].Invalidate(true);
                arrViewImage[3].Invalidate(true);
				arrViewImage[4].Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (arrViewImage[0].IsAvailable()
                      && arrViewImage[1].IsAvailable()
                      && arrViewImage[2].IsAvailable()
                      && arrViewImage[3].IsAvailable()
					  && arrViewImage[4].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

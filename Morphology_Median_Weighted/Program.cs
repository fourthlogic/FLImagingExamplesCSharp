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

namespace Morphology_Median_Weighted
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // 이미지 객체 선언
            CFLImage[] arrFliImage = new CFLImage[3];

            // 이미지 뷰 선언
            CGUIViewImage[] arrViewImage = new CGUIViewImage[3];

            for (int i = 0; i < 3; ++i)
            {
                arrFliImage[i] = new CFLImage();
                arrViewImage[i] = new CGUIViewImage();
            }

            do
            {
                // Source 이미지 로드
                if ((arrFliImage[0].Load("../../FLImagingExamples/Images/Morphology/Chip_Noise.bmp")).IsFail())
                {
                    Console.WriteLine("Failed to load the image file.\n");
                    break;
                }

                // Destination1 이미지를 Source 이미지와 동일한 이미지로 생성
                if ((arrFliImage[1].Assign(arrFliImage[0])).IsFail())
                {
                    Console.WriteLine("Failed to assign the image file.\n");
                    break;
                }

                // Destination2 이미지를 Source 이미지와 동일한 이미지로 생성
                if ((arrFliImage[2].Assign(arrFliImage[0])).IsFail())
                {
                    Console.WriteLine("Failed to assign the image file.\n");
                    break;
                }

                // Source 이미지 뷰 생성
                if ((arrViewImage[0].Create(100, 0, 612, 512)).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                // Destination1 이미지 뷰 생성
                if ((arrViewImage[1].Create(612, 0, 1124, 512)).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                // Destination2 이미지 뷰 생성
                if ((arrViewImage[2].Create(1124, 0, 1636, 512)).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                bool bError = false;

                // 이미지 뷰에 이미지를 디스플레이
                for (int i = 0; i < 3; ++i)
                {
                    if ((arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
                    {
                        Console.WriteLine("Failed to set image object on the image view.\n");
                        bError = true;
                        break;
                    }
                }

                if (bError)
                    break;

                // 두 이미지 뷰의 시점을 동기화 한다
                if ((arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다
                if ((arrViewImage[0].SynchronizePointOfView(ref arrViewImage[2])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤
                if ((arrViewImage[0].SynchronizeWindow(ref arrViewImage[1])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤
                if ((arrViewImage[0].SynchronizeWindow(ref arrViewImage[2])).IsFail())
                {
                    Console.WriteLine("Failed to synchronize window.\n");
                    break;
                }

                // 알고리즘 동작 결과
                CImageProcessingResult eResult = new CImageProcessingResult();

                // Utility CMorphologyMedianWeighted 객체 생성
                CMorphologyMedianWeighted medianWeighted = new CMorphologyMedianWeighted();
                // Source 이미지 설정
                medianWeighted.SetSourceImage(ref arrFliImage[0]);
                // ROI 범위 설정
                CFLRect<Int32> flrROI = new CFLRect<int>(100, 190, 360, 420);
                // Src ROI 설정
                medianWeighted.SetSourceROI(flrROI);
                // Destination 이미지 설정
                medianWeighted.SetDestinationImage(ref arrFliImage[1]);
                // Dst ROI 설정
                medianWeighted.SetDestinationROI(flrROI);

                // 처리할 MorphologyMedianWeight의 Kernel Size 설정 (KernelSize = 11 일 경우, Kernel Size : 11x11, 홀수만 설정가능)
                medianWeighted.SetKernelSize(11);

                // Image CMorphologyMedianWeighted 가중 방식을 Gauss로 설정
                medianWeighted.SetWeightedMethod(CMorphologyMedianWeighted.EWeightedMethod.Gauss);

                // 앞서 설정된 파라미터 대로 알고리즘 수행
                if ((eResult = medianWeighted.Execute()).IsFail())
                {
                    Console.WriteLine("Failed to execute weighted median.");
                    Console.WriteLine(eResult.GetString());
                    break;
                }

                // Destination 이미지 설정
                medianWeighted.SetDestinationImage(ref arrFliImage[2]);
                // Image MedianWeighted 가중 방식을 Inner로 설정
                medianWeighted.SetWeightedMethod(CMorphologyMedianWeighted.EWeightedMethod.Inner);

                // 앞서 설정된 파라미터 대로 알고리즘 수행
                if ((eResult = medianWeighted.Execute()).IsFail())
                {
                    Console.WriteLine("Failed to execute weighted median.");
                    Console.WriteLine(eResult.GetString());
                    break;
                }

                CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[3];
                arrLayer[0] = new CGUIViewImageLayer();
                arrLayer[1] = new CGUIViewImageLayer();
                arrLayer[2] = new CGUIViewImageLayer();

                for (int i = 0; i < 3; ++i)
                {
                    // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴
                    // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음
                    arrLayer[i] = arrViewImage[i].GetLayer(0);

                    // 기존에 Layer에 그려진 도형들을 삭제
                    arrLayer[i].Clear();

                    // ROI영역이 어디인지 알기 위해 디스플레이 한다
                    // FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
                    if (arrLayer[i].DrawFigureImage(flrROI, EColor.BLUE).IsFail())
                    {
                        Console.WriteLine("Failed to draw figures objects on the image view.\n");
                        break;
                    }
                }

                // 이미지 뷰 정보 표시
                // 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.
                // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다.
                // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                TPoint<double> tpPosition = new TPoint<double>(0, 0);

                if ((arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    Console.WriteLine("Failed to draw text\n");
                    break;
                }

                if ((arrLayer[1].DrawTextCanvas(tpPosition, "MedianWeighted1 Gauss", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    Console.WriteLine("Failed to draw text\n");
                    break;
                }

                if ((arrLayer[2].DrawTextCanvas(tpPosition, "MedianWeighted2 Inner", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    Console.WriteLine("Failed to draw text\n");
                    break;
                }

                // 이미지 뷰를 갱신
                arrViewImage[0].Invalidate(true);
                arrViewImage[1].Invalidate(true);
                arrViewImage[2].Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림
                while (arrViewImage[0].IsAvailable()
                      && arrViewImage[1].IsAvailable()
                      && arrViewImage[2].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

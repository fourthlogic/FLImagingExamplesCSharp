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

namespace Normalization
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // 이미지 객체 선언
            CFLImage fliISrcImage = new CFLImage();
            CFLImage fliIDstImage = new CFLImage();

            // 이미지 뷰 선언
            CGUIViewImage[] viewImage = new CGUIViewImage[2];

            viewImage[0] = new CGUIViewImage();
            viewImage[1] = new CGUIViewImage();

            do
            {
                // 이미지 로드
                if (fliISrcImage.Load("../Images/Histogram/Flower.bmp").IsFail())
                {
                    Console.WriteLine("Failed to load the image file.\n");
                    break;
                }

                // 이미지 뷰 생성
                if (viewImage[0].Create(300, 0, 300 + 520, 430).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                if (viewImage[1].Create(300 + 520, 0, 300 + 520 * 2, 430).IsFail())
                {
                    Console.WriteLine("Failed to create the image view.\n");
                    break;
                }

                // 뷰의 시점을 동기화 한다
                if (viewImage[0].SynchronizePointOfView(ref viewImage[1]).IsFail())
                {
                    Console.WriteLine("Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤
                if (viewImage[0].SynchronizeWindow(ref viewImage[1]).IsFail())
                {
                    Console.WriteLine("Failed to synchronize window\n");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이
                if (viewImage[0].SetImagePtr(ref fliISrcImage).IsFail())
                {
                    Console.WriteLine("Failed to set image object on the image view.\n");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이
                if (viewImage[1].SetImagePtr(ref fliIDstImage).IsFail())
                {
                    Console.WriteLine("Failed to set image object on the image view.\n");
                    break;
                }

                // 알고리즘 동작 결과
                CImageProcessingResult eResult = new CImageProcessingResult();

                // Normalization  객체 생성
                CNormalization Normalization = new CNormalization();

                // 처리할 Src 이미지 설정
                Normalization.SetSourceImage(ref fliISrcImage);

                // 출력될 Dst 이미지 설정
                Normalization.SetDestinationImage(ref fliIDstImage);

                // Normalization Min Max 정규화식 적용
                Normalization.SetNormalizationMethod(CNormalization.ENormalizationMethod.MinMax);

                // 앞서 설정된 파라미터 대로 Normalization 수행
                if ((eResult = Normalization.Execute()).IsFail())
                {
                    Console.WriteLine("Failed to execute Normalization.");
                    Console.WriteLine(eResult.GetString());
                    Console.WriteLine("\n");
                }

                // 레이어는 View의 소유물 이므로 따로 해제하지 않아도 View가 해제 될 때 같이 해제된다.
                CGUIViewImageLayer layer1 = viewImage[0].GetLayer(0);
                CGUIViewImageLayer layer2 = viewImage[1].GetLayer(0);
                CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

                // Text 출력 
                if (layer1.DrawTextImage(flpTemp, "Source Image", EColor.LIGHTRED).IsFail())
                    Console.WriteLine("Failed to draw Text\n");

                if (layer2.DrawTextImage(flpTemp, "Destination Image", EColor.LIGHTRED).IsFail())
                    Console.WriteLine("Failed to draw Text\n");


                // 이미지 뷰를 갱신 합니다.
                viewImage[0].Invalidate(true);
                viewImage[1].Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림
                while (viewImage[0].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);


        }
    }
}

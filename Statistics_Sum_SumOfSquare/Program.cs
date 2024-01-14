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

namespace Statistics
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
            CFLImage fliImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImage = new CGUIViewImage();

			CResult eResult = new CResult();

			do
            {
                // 이미지 로드 // Load image
                if ((eResult = fliImage.Load("../../ExampleImages/Statistics/StatisticsSource.flif")).IsFail())
                {
                    ErrorPrint(eResult, "Failed to load the image file.");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if ((eResult = viewImage.Create(400, 0, 1150, 500)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to create the image view.");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                if ((eResult = viewImage.SetImagePtr(ref fliImage)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to set image object on the image view.");
                    break;
                }

                // Statistics 객체 생성 // Create Statistics object
                CImageStatistics statistics = new CImageStatistics();

                // ROI 범위 설정 // Set the ROI value
                CFLRect<Int32> flrROI = new CFLRect<Int32>(264, 189, 432, 364);

                // Source 이미지 설정 // Set the Source Image
                statistics.SetSourceImage(ref fliImage);
                // Source ROI 설정 // Set the Source ROI
                statistics.SetSourceROI(flrROI);

                // 결과값을 받아올 CMultiVarD 컨테이너 생성 // Create the CMultiVarD object to push the result
                CMultiVar<double> mvSum = new CMultiVar<double>();
                CMultiVar<double> mvSumOfSquares = new CMultiVar<double>();

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 합을 구하는 함수 // Function that calculate the sum of the pixel value of the image(or the region of ROI)
				if((eResult = statistics.GetSum(out mvSum)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 제곱합을 구하는 함수
                if ((eResult = statistics.GetSumOfSquares(out mvSumOfSquares)).IsFail()) // Function that calculate the sum of squares of the pixel value of the image(or the region of ROI)
                {
                    ErrorPrint(eResult, "Failed to process.");
                    break;
                }

                // trimming 옵션 설정(Lower:0.2, Upper:0.4) // Set the trimming value(Lower:0.2, Upper:0.4)
                statistics.SetTrimming(0.2, CImageStatistics.ETrimmingLocation.Lower);
                statistics.SetTrimming(0.4, CImageStatistics.ETrimmingLocation.Upper);

                // trimming 된 결과값을 받아올 CMultiVarD 컨테이너 생성 // Create the CMultiVarD object to push the trimmed result
                CMultiVar<double> mvTrimmingSum = new CMultiVar<double>();
                CMultiVar<double> mvTrimmingSumOfSquares = new CMultiVar<double>();

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 합을 구하는 함수 // Function that calculate the sum of the pixel value of the image(or the region of ROI)
                if((eResult = statistics.GetSum(out mvTrimmingSum)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 제곱합을 구하는 함수 // Function that calculate the sum of squares of the pixel value of the image(or the region of ROI)
                if((eResult = statistics.GetSumOfSquares(out mvTrimmingSumOfSquares)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to process.");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layer = viewImage.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layer.Clear();

                // ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
                if ((eResult = layer.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
                    ErrorPrint(eResult, "Failed to draw figure");

                string strTrimming = String.Format("Trimming Lower : {0}, Upper : {1}", statistics.GetTrimming(CImageStatistics.ETrimmingLocation.Lower), statistics.GetTrimming(CImageStatistics.ETrimmingLocation.Upper));

                string strSumValue, strSumOfSquaresValue;
                strSumValue = String.Format("Sum Of Region : {0}", mvSum.GetAt(0));
                strSumOfSquaresValue = String.Format("Sum of Squares of Region : {0}", mvSumOfSquares.GetAt(0));

                string strTrimmingSumValue, strTrimmingSumOfSquaresValue;
                strTrimmingSumValue = String.Format("Sum Of Region : {0}", mvTrimmingSum.GetAt(0));
                strTrimmingSumOfSquaresValue = String.Format("Sum of Squares of Region : {0}", mvTrimmingSumOfSquares.GetAt(0));

				Console.WriteLine(strSumValue);
				Console.WriteLine(strSumOfSquaresValue);
				Console.WriteLine(strTrimming);
				Console.WriteLine(strTrimmingSumValue);
				Console.WriteLine(strTrimmingSumOfSquaresValue);

                CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

                // 이미지 뷰 정보 표시 // Display image view information
                if((eResult = layer.DrawTextCanvas(flpPoint, strSumValue, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((eResult = layer.DrawTextCanvas(flpPoint, strSumOfSquaresValue, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((eResult = layer.DrawTextCanvas(flpPoint, strTrimming, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((eResult = layer.DrawTextCanvas(flpPoint, strTrimmingSumValue, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((eResult = layer.DrawTextCanvas(flpPoint, strTrimmingSumOfSquaresValue, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult, "Failed to draw text.\n");
                    break;
                }

                if ((eResult = layer.DrawFigureImage(flrROI, EColor.GREEN, 1, EColor.LIGHTGREEN, EGUIViewImagePenStyle.Solid, 0.5F, 0.5F)).IsFail())
                    ErrorPrint(eResult, "Failed to draw figure.\n");

                if ((eResult = layer.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
                    ErrorPrint(eResult, "Failed to draw figure.\n");

                // 이미지 뷰를 갱신 합니다. // Update image view
                viewImage.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImage.IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

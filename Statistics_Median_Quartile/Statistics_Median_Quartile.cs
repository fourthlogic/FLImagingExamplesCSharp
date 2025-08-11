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

namespace FLImagingExamplesCSharp
{
    class Statistics_Median_Quartile
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
            CFLImage fliImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImage = new CGUIViewImage();

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/Statistics/StatisticsSource.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 1150, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if ((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view.");
                    break;
                }

                // Statistics 객체 생성 // Create Statistics object
                CImageStatistics imageStatistics = new CImageStatistics();

				// ROI 범위 설정 // Set the ROI value
				CFLRect<Int32> flrROI = new CFLRect<Int32>(264, 189, 432, 364);

				// Source 이미지 설정 // Set the Source Image
				imageStatistics.SetSourceImage(ref fliImage);
                // Source ROI 설정 // Set the Source ROI
                imageStatistics.SetSourceROI(flrROI);

                // 결과값을 받아올 CMultiVar<double> 컨테이너 생성 // Create the CMultiVar<double> object to push the result
                CMultiVar<double> mvMedian = new CMultiVar<double>();
                CMultiVar<double> mvUpperQuartile = new CMultiVar<double>();
                CMultiVar<double> mvLowerQuartile = new CMultiVar<double>();

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 중간값을 구하는 함수 // Function that calculate the median of the pixel value of the image(or the region of ROI)
				if((res = imageStatistics.GetMedian(ref mvMedian)).IsFail())
				{
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 75사분위수를 구하는 함수 // Function that calculate the upper quartile of the pixel value of the image(or the region of ROI)
                if((res = imageStatistics.GetUpperQuartile(ref mvUpperQuartile)).IsFail())
				{
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 25사분위수를 구하는 함수 // Function that calculate the lower quartile of the pixel value of the image(or the region of ROI)
                if((res = imageStatistics.GetLowerQuartile(ref mvLowerQuartile)).IsFail())
				{
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // trimming 옵션 설정(Lower:0.2, Upper:0.4) // Set the trimming value(Lower:0.2, Upper:0.4)
                imageStatistics.SetTrimming(0.2, CImageStatistics.ETrimmingLocation.Lower);
                imageStatistics.SetTrimming(0.4, CImageStatistics.ETrimmingLocation.Upper);

                // trimming 된 결과값을 받아올 CMultiVar<double> 컨테이너 생성 // Create the CMultiVar<double> object to push the trimmed result
                CMultiVar<double> mvTrimmingMedian = new CMultiVar<double>();
                CMultiVar<double> mvTrimmingUpperQuartile = new CMultiVar<double>();
                CMultiVar<double> mvTrimmingLowerQuartile = new CMultiVar<double>();

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 중간값을 구하는 함수 // Function that calculate the median of the pixel value of the image(or the region of ROI)
                if((res = imageStatistics.GetMedian(ref mvTrimmingMedian)).IsFail())
				{
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 75사분위수를 구하는 함수 // Function that calculate the upper quartile of the pixel value of the image(or the region of ROI)
                if((res = imageStatistics.GetUpperQuartile(ref mvTrimmingUpperQuartile)).IsFail())
				{
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 25사분위수를 구하는 함수 // Function that calculate the lower quartile of the pixel value of the image(or the region of ROI)
                if((res = imageStatistics.GetLowerQuartile(ref mvTrimmingLowerQuartile)).IsFail())
				{
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layer = viewImage.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layer.Clear();

                // ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
                if ((res = layer.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
                    ErrorPrint(res, "Failed to draw figure");

                string strTrimming = String.Format("Trimming Lower : {0}, Upper : {1}", imageStatistics.GetTrimming(CImageStatistics.ETrimmingLocation.Lower), imageStatistics.GetTrimming(CImageStatistics.ETrimmingLocation.Upper));

                string strMedianValue, strLowerQuartileValue, strUpperQuartileValue;
                strMedianValue = String.Format("Median Of Region : {0}", mvMedian.GetAt(0));
                strLowerQuartileValue = String.Format("1st Quartile Of Region : {0}", mvLowerQuartile.GetAt(0));
                strUpperQuartileValue = String.Format("3rd Quartile Of Region : {0}", mvUpperQuartile.GetAt(0));

                string strTrimmingMedianValue, strTrimmingLowerQuartileValue, strTrimmingUpperQuartileValue;
                strTrimmingMedianValue = String.Format("Median Of Trimmed Region : {0}", mvTrimmingMedian.GetAt(0));
                strTrimmingLowerQuartileValue = String.Format("1st Quartile Of Trimmed Region : {0}", mvTrimmingLowerQuartile.GetAt(0));
                strTrimmingUpperQuartileValue = String.Format("3rd Quartile Of Trimmed Region : {0}", mvTrimmingUpperQuartile.GetAt(0));

                Console.WriteLine(strMedianValue);
                Console.WriteLine(strLowerQuartileValue);
                Console.WriteLine(strUpperQuartileValue);
                Console.WriteLine(strTrimming);
                Console.WriteLine(strTrimmingMedianValue);
                Console.WriteLine(strTrimmingLowerQuartileValue);
                Console.WriteLine(strTrimmingUpperQuartileValue);

                CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strMedianValue, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 20);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strLowerQuartileValue, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 20);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strUpperQuartileValue, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 20);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strTrimming, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 20);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strTrimmingMedianValue, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 20);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strTrimmingLowerQuartileValue, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 20);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strTrimmingUpperQuartileValue, EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = layer.DrawFigureImage(flrROI, EColor.GREEN, 1, EColor.LIGHTGREEN, EGUIViewImagePenStyle.Solid, 0.5F, 0.5F)).IsFail())
                    ErrorPrint(res, "Failed to draw figure.\n");

                if ((res = layer.DrawFigureImage(flrROI, EColor.LIME)).IsFail())
                    ErrorPrint(res, "Failed to draw figure.\n");

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

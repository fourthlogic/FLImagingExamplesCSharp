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

			CResult res = new CResult();

			do
            {
                // 이미지 로드 // Load image
                if ((res = fliImage.Load("../../ExampleImages/Statistics/MultiChannelSource.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if ((res = viewImage.Create(400, 0, 1150, 700)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                if ((res = viewImage.SetImagePtr(fliImage)).IsFail())
                {
                    ErrorPrint(res, "Failed to set image object on the image view.");
                    break;
                }

                // Statistics 객체 생성 // Create Statistics object
                CImageStatistics statistics = new CImageStatistics();

                // Source 이미지 설정 // Set the Source Image
                statistics.SetSourceImage(ref fliImage);
                // 상관관계를 구할 채널을 설정
                statistics.SetCorrelatedChannel(0, 1);

                // 결과값을 받아올 double 변수 생성 // Create the variable to save the result
                double f64Covariance = new double();
                double f64CorrelationCoeff = new double();

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 공분산을 구하는 함수 // Function that calculate the covariance of the pixel value of the image(or the region of ROI)
				if((res = statistics.GetCovariance(out f64Covariance)).IsFail())
                {
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 상관계수를 구하는 함수 // Function that calculate the correlation coefficient of the pixel value of the image(or the region of ROI)
                if((res = statistics.GetCorrelationCoefficient(out f64CorrelationCoeff)).IsFail())
                {
                    ErrorPrint(res, "Failed to process.");
                    break;
                }

				// 상관관계를 구할 채널을 설정 // Set the Correlation channel
				statistics.SetCorrelatedChannel(0, 2);

                // 결과값을 받아올 double 변수 생성 // Create the variable to save the result
                double f64Covariance2 = new double();
				double f64CorrelationCoeff2 = new double();

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 공분산을 구하는 함수 // Function that calculate the covariance of the pixel value of the image(or the region of ROI)
                if((res = statistics.GetCovariance(out f64Covariance2)).IsFail())
				{
					ErrorPrint(res, "Failed to process.");
					break;
				}

                // 이미지 전체(혹은 ROI 영역) 픽셀값의 상관계수를 구하는 함수 // Function that calculate the correlation coefficient of the pixel value of the image(or the region of ROI)
                if((res = statistics.GetCorrelationCoefficient(out f64CorrelationCoeff2)).IsFail())
				{
					ErrorPrint(res, "Failed to process.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layer.Clear();

                string strCorrChannel, strCovarianceValue, strCorrelationCoeffValue;
                string strCorrChannel2, strCovarianceValue2, strCorrelationCoeffValue2;

                strCorrChannel = String.Format("Correlation Channel: Channel 0 - Channel 1");
                strCovarianceValue = String.Format("Covariance : {0}", f64Covariance);
                strCorrelationCoeffValue = String.Format("Correlation Coefficient : {0}", f64CorrelationCoeff);

				strCorrChannel2 = String.Format("Correlation Channel: Channel 0 - Channel 2");
				strCovarianceValue2 = String.Format("Covariance : {0}", f64Covariance2);
				strCorrelationCoeffValue2 = String.Format("Correlation Coefficient : {0}", f64CorrelationCoeff2);

				Console.WriteLine(strCorrChannel);
                Console.WriteLine(strCovarianceValue);
                Console.WriteLine(strCorrelationCoeffValue);
				Console.WriteLine(strCorrChannel2);
				Console.WriteLine(strCovarianceValue2);
                Console.WriteLine(strCorrelationCoeffValue2);

				CFLPoint<double> flpPoint = new CFLPoint<double>(0,0);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strCorrChannel, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strCovarianceValue, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strCorrelationCoeffValue, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strCorrChannel2, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strCovarianceValue2, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				flpPoint.Offset(0, 30);

                // 이미지 뷰 정보 표시 // Display image view information
                if((res = layer.DrawTextCanvas(flpPoint, strCorrelationCoeffValue2, EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

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

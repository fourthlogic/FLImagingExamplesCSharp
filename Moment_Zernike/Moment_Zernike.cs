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
using System.Runtime.InteropServices;

namespace Moment
{
    class Moment_Zernike
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
				if((res = fliImage.Load("../../ExampleImages/Moment/airEdge.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.");
                    break;
                }

				// 이미지 뷰 생성 // Create imageview
				if((res = viewImage.Create(400, 0, 1424, 768)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.");
                    break;
                }

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the imageview
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
                {
                    ErrorPrint(res, "Failed to set image object on the image view.");
                    break;
                }

				// Moment 객체 생성 // Create Moment object
				CMoment moment = new CMoment();

				// ROI 범위 설정 // Set ROI range
				CFLRect<double> flrROI = new CFLRect<double>(15, 150, 420, 280);

				// 처리할 이미지 설정 // Set the image to process
				moment.SetSourceImage(ref fliImage);
				// 처리할 ROI 설정 // Set the ROI to be processed
				moment.SetSourceROI(flrROI);
				// 처리할 이미지의 이진화이미지로 판단 유무 설정 // Set whether to judge the image to be processed as a binarized image
				moment.EnableBinaryImage(true);

				// 계산 대상에 저니키 모멘트 N, M 파라미터를 추가합니다. // Add Journey Moment N, M parameters to the calculation target.
				moment.AddZernike(1, -1);
                moment.AddZernike(1, 1);
                moment.AddZernike(3, -3);
                moment.AddZernike(3, -1);
                moment.AddZernike(3, 1);
                moment.AddZernike(3, 3);

				// 알고리즘 수행 // Execute the algorithm
				if((res = moment.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute moment.");
                    break;
                }

				// 모멘트 결과들을 가져옵니다. // Get the moment results.
				CMoment.SZernike zernike = new CMoment.SZernike();
                long i64ZernikeCount = (long)moment.GetZernikeCount();

                for (int i = 0; i < i64ZernikeCount; ++i)
                {
                    moment.GetZernike(ref zernike, i);
					Console.WriteLine("Zernike N = {0}, M = {1}, RealValue : {2}, Imaginary : {3}", zernike.i32N, zernike.i32M, zernike.f64ZernikeReal, zernike.f64ZernikeImag);
                }

                CGUIViewImageLayer layer = viewImage.GetLayer(0);

                layer.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the ROI area is
				if((res = layer.DrawFigureImage(flrROI, EColor.BLUE)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImage.IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

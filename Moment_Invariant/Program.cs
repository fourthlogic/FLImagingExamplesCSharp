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

namespace Moment
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
				// 이진화이미지로 판단할 경우 0이 아닌 모든 화소값은 1로 처리함 // When judging as a binarized image, all non-zero pixel values ??are treated as 1.
				moment.EnableBinaryImage(true);

                bool bCalcGeometricMoment = true;
                bool bCalcCentroidMoment = true;
                bool bCalcCentralMoment = true;
                bool bCalcNormalizedCentralMoment = true;
                bool bCalcHuMoment = true;

				// 계산 대상에 기하학적 모멘트를 포함합니다. // Include the geometric moment in the computed object.
				moment.EnableGeometricMoment(bCalcGeometricMoment);
				// 계산 대상에 도심 모멘트를 포함합니다. // Include the centroid moment in the calculation target.
				moment.EnableCentroidMoment(bCalcCentroidMoment);
				// 계산 대상에 중심 모멘트를 포함합니다. // Include the central moment in the calculation target.
				moment.EnableCentralMoment(bCalcCentralMoment);
				// 계산 대상에 정규화된 중심 모멘트를 포함합니다. // Include the normalized central moment in the computed target.
				moment.EnableNormalizedCentralMoment(bCalcNormalizedCentralMoment);
				// 계산 대상에 휴(불변) 모멘트를 포함합니다. // Include the idle (invariant) moment in the calculation target.
				moment.EnableHuMoment(bCalcHuMoment);

				// 알고리즘 수행 // Execute the algorithm
				if((res = moment.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute moment.");
                    break;
                }

				// 모멘트 결과들을 가져옵니다. // Get the moment results.
				CMoment.SMoment results;
                moment.GetMoment(ref results);

				// 모멘트 결과를 Console창에 출력 // Output the moment result to the console window
				if(bCalcGeometricMoment)
                {
                    Console.WriteLine("< Geometric Moment > ");
                    Console.WriteLine(" Moment 00 : {0}", results.pSGeometricMoments.f64GeometricM00);
                    Console.WriteLine(" Moment 10 : {0}", results.pSGeometricMoments.f64GeometricM10);
                    Console.WriteLine(" Moment 01 : {0}", results.pSGeometricMoments.f64GeometricM01);
                    Console.WriteLine(" Moment 20 : {0}", results.pSGeometricMoments.f64GeometricM20);
                    Console.WriteLine(" Moment 11 : {0}", results.pSGeometricMoments.f64GeometricM11);
                    Console.WriteLine(" Moment 02 : {0}", results.pSGeometricMoments.f64GeometricM02);
                    Console.WriteLine(" Moment 30 : {0}", results.pSGeometricMoments.f64GeometricM30);
                    Console.WriteLine(" Moment 21 : {0}", results.pSGeometricMoments.f64GeometricM21);
                    Console.WriteLine(" Moment 12 : {0}", results.pSGeometricMoments.f64GeometricM12);
                    Console.WriteLine(" Moment 03 : {0}", results.pSGeometricMoments.f64GeometricM03);
                    Console.WriteLine("");
                }

                if (bCalcCentroidMoment)
                {
                    Console.WriteLine("< Centroid Moment > ");
                    Console.WriteLine(" Moment Centroid X : {0}", results.pSCentroidMoment.f64CentroidX);
                    Console.WriteLine(" Moment Centroid Y : {0}", results.pSCentroidMoment.f64CentroidY);
                    Console.WriteLine("");
                }

                if (bCalcCentralMoment)
                {
                    Console.WriteLine("< Central Moment > ");
                    Console.WriteLine(" Moment 00 : {0}", results.pSCentralMoments.f64CentralM00);
                    Console.WriteLine(" Moment 10 : {0}", results.pSCentralMoments.f64CentralM10);
                    Console.WriteLine(" Moment 01 : {0}", results.pSCentralMoments.f64CentralM01);
                    Console.WriteLine(" Moment 20 : {0}", results.pSCentralMoments.f64CentralM20);
                    Console.WriteLine(" Moment 11 : {0}", results.pSCentralMoments.f64CentralM11);
                    Console.WriteLine(" Moment 02 : {0}", results.pSCentralMoments.f64CentralM02);
                    Console.WriteLine(" Moment 30 : {0}", results.pSCentralMoments.f64CentralM30);
                    Console.WriteLine(" Moment 21 : {0}", results.pSCentralMoments.f64CentralM21);
                    Console.WriteLine(" Moment 12 : {0}", results.pSCentralMoments.f64CentralM12);
                    Console.WriteLine(" Moment 03 : {0}", results.pSCentralMoments.f64CentralM03);
                    Console.WriteLine("");
                }

                if (bCalcNormalizedCentralMoment)
                {
                    Console.WriteLine("< Normalized Central Moment > ");
                    Console.WriteLine(" Moment 00 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM00);
                    Console.WriteLine(" Moment 10 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM10);
                    Console.WriteLine(" Moment 01 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM01);
                    Console.WriteLine(" Moment 20 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM20);
                    Console.WriteLine(" Moment 11 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM11);
                    Console.WriteLine(" Moment 02 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM02);
                    Console.WriteLine(" Moment 30 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM30);
                    Console.WriteLine(" Moment 21 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM21);
                    Console.WriteLine(" Moment 12 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM12);
                    Console.WriteLine(" Moment 03 : {0}", results.pSNormalizedCentralMoments.f64NormalizedCentralM03);
                    Console.WriteLine("");
                }

                if (bCalcHuMoment)
                {
                    Console.WriteLine("< Hu Moment > ");
                    Console.WriteLine(" Hu 0 : {0}", results.pSHuMoments.f64Hu0);
                    Console.WriteLine(" Hu 1 : {0}", results.pSHuMoments.f64Hu1);
                    Console.WriteLine(" Hu 2 : {0}", results.pSHuMoments.f64Hu2);
                    Console.WriteLine(" Hu 3 : {0}", results.pSHuMoments.f64Hu3);
                    Console.WriteLine(" Hu 4 : {0}", results.pSHuMoments.f64Hu4);
                    Console.WriteLine(" Hu 5 : {0}", results.pSHuMoments.f64Hu5);
                    Console.WriteLine(" Hu 6 : {0}", results.pSHuMoments.f64Hu6);
                    Console.WriteLine("");
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

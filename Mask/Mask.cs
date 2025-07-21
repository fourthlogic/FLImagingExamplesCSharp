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

using CResult = FLImagingCLR.CResult;

namespace Mask
{
	class Mask
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

			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = (fliImage.Load("../../ExampleImages/Mask/Moon.flif"))).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = (viewImage.Create(400, 0, 912, 612))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = (viewImage.SetImagePtr(ref fliImage))).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				// Mask 객체 생성 // Create Mask object
				CMask mask = new CMask();

				// ROI 범위 설정 // Set ROI range
				CFLCircle<double> flcROI = new CFLCircle<double>(280, 169, 25);

				// Source 이미지 설정 // Set the source image
				mask.SetSourceImage(ref fliImage);
				// Source ROI 설정 // Set the Source ROI
				mask.SetSourceROI(flcROI);
				// Mask 색상 지정 // Set mask color
				CMultiVar<double> mvMaskValue = new CMultiVar<double>(255, 255, 255);
				mask.SetMask(mvMaskValue);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (mask.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute mask.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				if((res = (layer.DrawFigureImage(flcROI, EColor.LIME))).IsFail())
					ErrorPrint(res, "Failed to draw figure");

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPosition00 = new CFLPoint<double>(0, 0);

				if((res = (layer.DrawTextCanvas(flpPosition00, "Source Image", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
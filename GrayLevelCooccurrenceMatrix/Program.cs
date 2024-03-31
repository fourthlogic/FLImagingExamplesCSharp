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
using CResult = FLImagingCLR.CResult;

namespace GrayLevelCooccurrenceMatrix
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
				if((res = fliImage.Load("../../ExampleImages/GrayLevelCooccurrenceMatrix/Texture.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 912, 612)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				// GrayLevelCooccurrenceMatrix 객체 생성 // Create GrayLevelCooccurrenceMatrix object
				CGrayLevelCooccurrenceMatrix flaGLCM = new CGrayLevelCooccurrenceMatrix();

				// ROI 범위 설정 // Set the ROI value
				CFLRect<double> flfSourceROI = new CFLRect<double>(143.508137, 70.054249, 295.117540, 213.562386, 0.000000);

				// Source 이미지 설정 // Set the Source Image
				flaGLCM.SetSourceImage(ref fliImage);
				// Source ROI 설정 // Set the Source ROI
				flaGLCM.SetSourceROI(flfSourceROI);

				// grayLevel 설정(2^8 = 256) // Set gray level (2^8 = 256)
				flaGLCM.SetGrayLevel(8);

				// Matrix Direction 0도 설정 // Set Matrix Direction 0 Degree
				flaGLCM.SetDirection(CGrayLevelCooccurrenceMatrix.EDirection.Degree0);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (flaGLCM.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Gray Level Cooccurrence Matrix.");
					break;
				}

				// 결과값을 받아올 List 컨테이너 생성 // Create the List object to push the result
				List<List<double>> listEnergy = new List<List<double>>();
				List<List<double>> listCorrelation = new List<List<double>>();
				List<List<double>> listHomogeneity = new List<List<double>>();
				List<List<double>> listContrast = new List<List<double>>();

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 Energy를 구하는 함수 // Function that calculate Energy of the image(or the region of ROI)
				if((res = flaGLCM.GetResultEnergy(out listEnergy)).IsFail())
				{
					ErrorPrint(res, "No Result");
					break;
				}

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 Correlation를 구하는 함수 // Function that calculate Correlation of the image(or the region of ROI)
				if((res = flaGLCM.GetResultCorrelation(out listCorrelation)).IsFail())
				{
					ErrorPrint(res, "No Result");
					break;
				}

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 Homogeneity를 구하는 함수 // Function that calculate Homogeneity of the image(or the region of ROI)
				if((res = flaGLCM.GetResultHomogeneity(out listHomogeneity)).IsFail())
				{
					ErrorPrint(res, "No Result");
					break;
				}

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 Contrast를 구하는 함수 // Function that calculate Contrast of the image(or the region of ROI)
				if((res = flaGLCM.GetResultContrast(out listContrast)).IsFail())
				{
					ErrorPrint(res, "No Result");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				if((res = layer.DrawFigureImage(flfSourceROI, EColor.LIME)).IsFail())
					ErrorPrint(res, "Failed to draw figure");

				string strText = "";

				for(int i32PageIdx = 0; i32PageIdx < listEnergy.Count; ++i32PageIdx)
				{
					//strText += String.Format("Page.No {0} ", i32PageIdx);

					for(int i32Ch = 0; i32Ch < listEnergy[i32PageIdx].Count(); i32Ch++)
					{
						//strText += String.Format("\nChannel {0} ", i32Ch);
						strText += String.Format("Energy {0:F9} ", listEnergy[i32PageIdx][i32Ch]);
						strText += String.Format("\nCorrelation {0:F9} ", listCorrelation[i32PageIdx][i32Ch]);
						strText += String.Format("\nHomogeneity {0:F9} ", listHomogeneity[i32PageIdx][i32Ch]);
						strText += String.Format("\nContrast {0:F9} ", listContrast[i32PageIdx][i32Ch]);
					}

					//strText += String.Format("\n\n");
				}

				Console.WriteLine(strText);
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layer.DrawTextCanvas(flpPoint, strText, EColor.YELLOW, EColor.BLACK, 25)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
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

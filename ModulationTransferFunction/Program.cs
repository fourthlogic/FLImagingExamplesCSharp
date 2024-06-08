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

namespace ModulationTransferFunction
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
				if((res = fliImage.Load("../../ExampleImages/ModulationTransferFunction/ISO12233Crop.flif")).IsFail())
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

				// ModulationTransferFunction 객체 생성 // Create ModulationTransferFunction object
				CModulationTransferFunction flaGLCM = new CModulationTransferFunction();

				// ROI 범위 설정 // Set the ROI value
				CFLRect<double> flfSourceROI = new CFLRect<double>(349.0, 43.0, 396.0, 85.0);

				// Source 이미지 설정 // Set the Source Image
				flaGLCM.SetSourceImage(ref fliImage);
				// Source ROI 설정 // Set the Source ROI
				flaGLCM.SetSourceROI(flfSourceROI);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (flaGLCM.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Modulation Transfer Function.");
					break;
				}

				// 결과값을 받아올 List 컨테이너 생성 // Create the List object to push the result
				List<double> listMTF = new List<double>();

				// 이미지 전체(혹은 ROI 영역) 픽셀값의 MTF를 구하는 함수 // Function that calculate MTF of the image(or the region of ROI)
				if((res = flaGLCM.GetResult(out listMTF)).IsFail())
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

				for(int i32PageIdx = 0; i32PageIdx < listMTF.Count; ++i32PageIdx)
				{
					//strText += String.Format("Page.No {0} ", i32PageIdx);

					strText += String.Format("MTF {0:F9} ", listMTF[i32PageIdx]);
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

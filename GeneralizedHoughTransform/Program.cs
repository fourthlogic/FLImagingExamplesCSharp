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

namespace GeneralizedHoughTransform
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
			CFLImage fliISrcImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();

			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/HoughTransform/PatternExample.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(300, 0, 300 + 600, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImage.SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// GeneralizedHoughTransform 객체 생성 // Create GeneralizedHoughTransform object
				CGeneralizedHoughTransform GeneralizedHoughTransform = new CGeneralizedHoughTransform();

				// Source 이미지 설정 // Set the source image
				GeneralizedHoughTransform.SetSourceImage(ref fliISrcImage);

				CFLCircle<long> flfPatternROI = new CFLCircle<long>(575, 755, 71, 0, 0, 360, EArcClosingMethod.EachOther);
				GeneralizedHoughTransform.SetPatternROI(flfPatternROI);

				// Threshold 값 설정 // Set Threshold value
				GeneralizedHoughTransform.SetPixelThreshold(128);

				// 신뢰도 설정 // set confidence
				GeneralizedHoughTransform.SetConfidence(0.5);

				// 탐색할 각도 단위 설정 (degree) // Set the angle unit to search (degree)
				GeneralizedHoughTransform.SetAngleTolerance(90);

				// 탐색할 크기 설정 (percent) // Set the scale tolerance to search (percent)
				GeneralizedHoughTransform.SetScaleTolerance(10);

				// 최대 검출 수 설정 // Set the maximum number of detections
				GeneralizedHoughTransform.SetMaxObjectCount(100);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (GeneralizedHoughTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute GeneralizedHoughTransform.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// Result 갯수 체크 // Check the number of results
				if(GeneralizedHoughTransform.GetDetectedObjectCount() > 0)
				{
					CFLFigureArray flfaDetectedObjects = new CFLFigureArray();
					GeneralizedHoughTransform.GetDetectedObjects(ref flfaDetectedObjects);

					// 이미지 뷰에 검출된 객체 출력 // Output the detected object to the image view
					if((res = (layer.DrawFigureImage(flfaDetectedObjects, EColor.BRIGHTCYAN, 2))).IsFail())
					{
						ErrorPrint(res, "Failed to draw Figure");
						break;
					}
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

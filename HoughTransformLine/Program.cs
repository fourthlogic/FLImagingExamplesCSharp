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

namespace HoughTransformLine
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

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliISrcImage.Load("../../ExampleImages/HoughTransform/Sudoku.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(300, 0, 300 + 520, 430)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewImage.SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// HoughTransform  객체 생성 // Create HoughTransform object
				CHoughTransform HoughTransform = new CHoughTransform();

				// Source 이미지 설정 // Set source image 
				HoughTransform.SetSourceImage(ref fliISrcImage);

				// HoughTransform Line 변환 선택 // Set houghTransform line transform
				HoughTransform.SetHoughShape(CHoughTransform.EHoughShape.Line);

				// 연산 방식 설정 // Set calculation method
				HoughTransform.SetExecuteMode(CHoughTransform.EExecuteMode.Image);

				// Threshold 값 설정 // set threshold value
				HoughTransform.SetPixelThreshold(10);

				// 조건 타입 설정 Less (Threshold 값 이하의 픽셀) // set logical condition(pixels below the threshold value)
				HoughTransform.SetLogicalCondition(ELogicalCondition.Less);

				// 최소 픽셀 카운터 수 (픽셀 카운터 기준보다 낮을 경우 필터링) // Minimum number of pixel counters (Filter if lower than pixel counter criteria)
				HoughTransform.SetMinPixelCount(200);

				// 인접 거리 필터링 설정 (거리 +-5, 각도 +-5 기준 가장 카운팅이 많이된 픽셀 값을 Line으로 선정) 
				// Neighbor Distance Filtering Settings(Based on distance +-5, angle +-5, the most counted pixel value is selected as a line)
				HoughTransform.SetNearbyLineFilter(5);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (HoughTransform.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute HoughTransform.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layer.Clear();

				// Result 갯수 가져오기 // get result count
				long i64ResultCount = HoughTransform.GetResultLinesCount();

				// Line 객체 선언
				CFLLine<double> fllLine = new CFLLine<double>();

				for(long i = 0; i < i64ResultCount; i++) // 출력
				{
					// line 결과 가져오기 // get result line
					HoughTransform.GetResultLine(i, ref fllLine);

					// line 출력 // display line
					if((res = (layer.DrawFigureImage(fllLine, EColor.LIME, 1))).IsFail())
					{
						ErrorPrint(res, "Failed to draw Figure");
						break;
					}
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);


		}
	}
}

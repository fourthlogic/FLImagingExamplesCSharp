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

namespace HoughTransformCircle
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

			CResult eResult;

			do
			{
				// 이미지 로드 // Load image
				if((eResult = fliISrcImage.Load("../../ExampleImages/HoughTransform/coins.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImage.Create(300, 0, 300 + 600, 600)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((eResult = viewImage.SetImagePtr(ref fliISrcImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// HoughTransform 객체 생성 // Create HoughTransform  object
				CHoughTransform HoughTransform = new CHoughTransform();

				// Source 이미지 설정 // Set the source image
				HoughTransform.SetSourceImage(ref fliISrcImage);

				// HoughTransform Circle 변환 선택 // Select HoughTransform Circle transform
				HoughTransform.SetHoughShape(CHoughTransform.EHoughShape.Circle);

				// 이미지로 임계값으로 동작하는 모드 적용 // Apply the mode that operates as a threshold to the image
				HoughTransform.SetExecuteMode(CHoughTransform.EExecuteMode.Image);

				// Threshold 값 설정 // Set Threshold value
				HoughTransform.SetPixelThreshold(200);

				// 조건 타입 설정 Less (Threshold 값 이하의 픽셀) // Set the condition type Less (pixels below the Threshold value)
				HoughTransform.SetLogicalCondition(ELogicalCondition.Less);

				// 검출할 최소 반지름 설정 // Set minimum radius to detect
				HoughTransform.SetMinRadius(35);

				// 검출할 최대 반지름 설정 // Set the maximum radius to detect
				HoughTransform.SetMaxRadius(40);

				// 탐색할 반지름 단위 설정 (1로 설정할 시 35(Min), 36, 37 ... 40(Max) 으로 탐색)
				// Set the radius unit to search (when set to 1, search with 35 (Min), 36, 37 ... 40 (Max))
				HoughTransform.SetPixelResolution(1);

				// 탐색할 각도 단위 설정 (degree) // Set the angle unit to search (degree)
				HoughTransform.SetAngleResolution(5);

				// 신뢰도 설정 (%) // set confidence (%)
				HoughTransform.SetConfidence(70);

				// 최대 검출 수 설정 // Set the maximum number of detections
				HoughTransform.SetMaxCount(100);

				// Canny Edge 적용유무 설정 (true 로 설정할 경우, SetPixelThreshold와 SetLogicalCondition로 설정한 값은 활용하지 않음)
				// Setting whether to apply Canny Edge (if set to true, the values ​​set by SetPixelThreshold and SetLogicalCondition are not used)
				HoughTransform.EnableCannyEdgeAppliance(false);

				// 인접하게 검출된 원을 필터링하는 옵션 // Option to filter adjacent detected circles
				HoughTransform.EnableAdjacentFilterAppliance(true);


				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = (HoughTransform.Execute())).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute HoughTransform.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// Result 갯수 가져오기 // Get the number of results
				long i64ResultCount = HoughTransform.GetResultCirclesCount();

				Console.WriteLine("Result Circle Count : {0}", i64ResultCount);

				CFLCircle<double> flcResult;

				for(long i = 0; i < i64ResultCount; i++)
				{
					HoughTransform.GetResultCircle(i, out flcResult);

					// 이미지 뷰에 검출된 원 객체 출력 // Output the detected original object to the image view
					if((eResult = (layer.DrawFigureImage(flcResult, EColor.LIGHTGREEN, 1))).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw Figure");
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

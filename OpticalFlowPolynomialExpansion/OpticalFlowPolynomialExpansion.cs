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
	class OpticalFlowPolynomialExpansion
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
			CFLImage[] arrFliImage = new CFLImage[2];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[2];

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			for(int i = 0; i < 2; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = arrFliImage[0].Load("../../ExampleImages/OpticalFlowPolynomialExpansion/Highway.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
				if((res = arrFliImage[1].Assign(arrFliImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// OpticalFlowPolynomialExpansion 객체 생성 // Create OpticalFlowPolynomialExpansion object
				COpticalFlowPolynomialExpansion opticalFlowPolynomialExpansion = new COpticalFlowPolynomialExpansion();
				// Source 이미지 설정 // Set the source image
				opticalFlowPolynomialExpansion.SetSourceImage(ref arrFliImage[0]);
				// Destination 이미지 설정 // Set the destination image
				opticalFlowPolynomialExpansion.SetDestinationImage(ref arrFliImage[1]);
				// Pyramid Level 설정 // Set Pyramid Level
				opticalFlowPolynomialExpansion.SetPyramidLevel(2);
				// Iteration 설정 // Set Iteration
				opticalFlowPolynomialExpansion.SetIteration(3);
				// Window Size 설정 // Set Window Size
				opticalFlowPolynomialExpansion.SetWindowSize(15);
				// Binning Size 설정 // Set Binning Size
				opticalFlowPolynomialExpansion.SetBinningSize(8);
				// Minimum Vector Size 설정 // Set  Minimum Vector Size
				opticalFlowPolynomialExpansion.SetMinimumVectorSize(5.000000);

				Console.WriteLine("Processing....");

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = opticalFlowPolynomialExpansion.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute OpticalFlow Polynomial Expansion.");
					Console.WriteLine(res.GetString());
					break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if((res = arrViewImage[0].Create(400, 0, 1012, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if((res = arrViewImage[1].Create(1012, 0, 1624, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				bool bError = false;

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				for(int i = 0; i < 2; ++i)
				{
					if((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// 두 이미지 뷰의 페이지를 동기화 한다
				if((res = arrViewImage[0].SynchronizePageIndex(ref arrViewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[3];

				for(int i = 0; i < 2; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayer[i].Clear();
				}

				// 이미지 뷰 정보 표시 // Display image view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				TPoint<double> tpPosition = new TPoint<double>(0, 30);

				if((res = arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[1].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				arrViewImage[0].Invalidate(true);
				arrViewImage[1].Invalidate(true);

				// 이미지 페이지 변경으로 인한 Auto Clear Mode 비활성화
				arrViewImage[0].SetLayerAutoClearMode(0, ELayerAutoClearMode.PageChanged, false);
				arrViewImage[1].SetLayerAutoClearMode(0, ELayerAutoClearMode.PageChanged, false);
				arrViewImage[0].SetLayerAutoClearMode(1, ELayerAutoClearMode.PageChanged, false);

				// 첫번째 이미지 페이지 선택
				arrViewImage[0].MoveToPage(0);
				arrViewImage[1].MoveToPage(0);

				arrViewImage[0].GetLayer(1).SetLayerDrawingMethod(ELayerDrawingMethod.Manual);

				int i32PageIndex = 0;
				CPerformanceCounter performanceCounter = new CPerformanceCounter();
				CFLFigureArray flfaResultArrow = new CFLFigureArray();

				opticalFlowPolynomialExpansion.GetResultMotionVectorsArrowShapeAllScenes(ref flfaResultArrow);
				performanceCounter.Start();

				// 이미지 뷰에 Optical Flow 출력
				while(arrViewImage[0].IsAvailable() && arrViewImage[1].IsAvailable())
				{
					if(arrFliImage[0].GetPageCount() - 1 == arrFliImage[0].GetSelectedPageIndex())
					{
						arrViewImage[0].MoveToPage(0);
						arrViewImage[1].MoveToPage(0);
						i32PageIndex = 0;
						continue;
					}

					arrViewImage[0].MoveToPage(i32PageIndex);
					arrViewImage[1].MoveToPage(i32PageIndex);
					arrViewImage[0].GetLayer(1).Clear();
					arrViewImage[1].GetLayer(1).DrawFigureImage(flfaResultArrow.GetAt(i32PageIndex), EColor.BLACK, 3);
					arrViewImage[1].GetLayer(1).DrawFigureImage(flfaResultArrow.GetAt(i32PageIndex), EColor.YELLOW, 1);
					arrViewImage[0].GetLayer(1).Update();
					arrViewImage[0].RedrawWindow();

					if(!arrViewImage[0].IsAvailable() || !arrViewImage[1].IsAvailable())
						break;

					while(performanceCounter.GetElapsedTimeFromStartInMilliSecond() <= 40.0)
						CThreadUtilities.Sleep(1);

					performanceCounter.Start();
					i32PageIndex++;
				}
			}
			while(false);
		}
	}
}
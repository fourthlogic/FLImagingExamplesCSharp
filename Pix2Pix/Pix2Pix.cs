using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;
using FLImagingCLR.ThreeDim;
using FLImagingCLR.AI;
using System.Net.NetworkInformation;

namespace FLImagingExamplesCSharp
{
	class Pix2Pix
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

			// 라이브러리가 완전히 로드 될 때까지 기다림 // Wait for the library to fully load
			Thread.Sleep(1000);

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliLearnImageInput = new CFLImage();
			CFLImage fliValidationImageInput = new CFLImage();
			CFLImage fliLearnImageTarget = new CFLImage();
			CFLImage fliValidationImageTarget = new CFLImage();
			CFLImage fliResultImage = new CFLImage();
	
			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearnInput = new CGUIViewImage();
			CGUIViewImage viewImageLearnTarget = new CGUIViewImage();
			CGUIViewImage viewImageValidationInput = new CGUIViewImage();
			CGUIViewImage viewImageValidationTarget = new CGUIViewImage();
			CGUIViewImage viewImagesResult = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImageInput.Load("../../ExampleImages/Pix2Pix/Gray_Learn.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}
				
				// 이미지 로드 // Load image
				if((res = fliLearnImageTarget.Load("../../ExampleImages/Pix2Pix/Color_Learn.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliValidationImageInput.Load("../../ExampleImages/Pix2Pix/Gray_Validation.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliValidationImageTarget.Load("../../ExampleImages/Pix2Pix/Color_Validation.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearnInput.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearnTarget.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageValidationInput.Create(100, 500, 600, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageValidationTarget.Create(600, 500, 1100, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImagesResult.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((res = viewGraph.Create(1100, 500, 1600, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				viewGraph.SetDarkMode();

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearnInput.SetImagePtr(ref fliLearnImageInput)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearnTarget.SetImagePtr(ref fliLearnImageTarget)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageValidationInput.SetImagePtr(ref fliValidationImageInput)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageValidationTarget.SetImagePtr(ref fliValidationImageTarget)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImagesResult.SetImagePtr(ref fliResultImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 다섯 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the four image view windows
				if((res = viewImageLearnInput.SynchronizeWindow(ref viewImageValidationInput)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 다섯 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the four image view windows
				if((res = viewImageLearnInput.SynchronizeWindow(ref viewImageLearnTarget)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnInput.SynchronizeWindow(ref viewImageValidationInput)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnInput.SynchronizeWindow(ref viewImageValidationTarget)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnInput.SynchronizeWindow(ref viewImagesResult)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnInput.SynchronizeWindow(ref viewGraph)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnInput.SynchronizeWindow(ref viewGraph)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnInput.SynchronizePageIndex(ref viewImageLearnTarget)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageValidationInput.SynchronizePageIndex(ref viewImageValidationTarget)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearnInput = viewImageLearnInput.GetLayer(0);
				CGUIViewImageLayer layerLearnTarget = viewImageLearnTarget.GetLayer(0);
				CGUIViewImageLayer layerValidationInput = viewImageValidationInput.GetLayer(0);
				CGUIViewImageLayer layerValidationTarget = viewImageValidationTarget.GetLayer(0);
				CGUIViewImageLayer layerResult = viewImagesResult.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearnInput.Clear();
				layerLearnTarget.Clear();
				layerValidationInput.Clear();
				layerValidationTarget.Clear();
				layerResult.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = layerLearnInput.DrawTextCanvas(flpPoint, "LEARN INPUT", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerLearnTarget.DrawTextCanvas(flpPoint, "LEARN TARGET", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerValidationInput.DrawTextCanvas(flpPoint, "VALIDATION INPUT", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerValidationTarget.DrawTextCanvas(flpPoint, "VALIDATION TARGET", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResult.DrawTextCanvas(flpPoint, "INFERENCE RESULT", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearnInput.RedrawWindow();
				viewImageLearnTarget.RedrawWindow();
				viewImageValidationInput.RedrawWindow();
				viewImageValidationTarget.RedrawWindow();
				viewImagesResult.RedrawWindow();

				// Pix2Pix 객체 생성 // Create Pix2Pix object
				CPix2PixDL pix2pixDL = new CPix2PixDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				pix2pixDL.SetLearningImage(ref fliLearnImageInput);
				pix2pixDL.SetLearningTargetImage(ref fliLearnImageTarget);

				// 검증할 이미지 설정 // Set the image to validation
				pix2pixDL.SetLearningValidationImage(ref fliValidationImageInput);
				pix2pixDL.SetLearningValidationTargetImage(ref fliValidationImageTarget);

				// 분류할 이미지 설정 // Set the image to classify
				pix2pixDL.SetInferenceImage(ref fliValidationImageInput);
				pix2pixDL.SetInferenceResultImage(ref fliResultImage);

				// 학습할 Pix2Pix 모델 설정 // Set up the Pix2Pix model to learn
				pix2pixDL.SetModel(CPix2PixDL.EModel.FLGenNet_Pix2Pix);
				// 학습할 Pix2Pix 모델 Version 설정 // Set up the Pix2Pix model version to learn
				pix2pixDL.SetModelVersion(CPix2PixDL.EModelVersion.FLGenNet_Pix2Pix_V1_128);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				pix2pixDL.SetLearningEpoch(500);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				pix2pixDL.SetInterpolationMethod(EInterpolationMethod.Bilinear);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(.0001f);

				// 설정한 Optimizer를 Pix2Pix에 적용 // Apply Optimizer that we set up to Pix2Pix
				pix2pixDL.SetLearningOptimizerSpec(optSpec);

				// 학습을 종료할 조건식 설정. accuracy값이 0.95 이상인 경우 학습 종료한다.
				// Set Conditional Expression to End Learning. If the accuracy value is 0.95 or more, end learning.
				pix2pixDL.SetLearningStopCondition("accuracy >= 0.95");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				CAutoSaveSpec autoSaveSpec = new CAutoSaveSpec();

				// 자동 저장 활성화 // Enable Auto-Save
				// 저장 때문에 발생하는 속도 저하를 막기 위해 예제에서는 코드 사용법만 표시하고 옵션은 끔 // To prevent performance degradation caused by saving, the examples only demonstrate how to use the code, with the saving option disabled.
				autoSaveSpec.EnableAutoSave(false);
				// 저장할 모델 경로 설정 // Set Model path to save
				autoSaveSpec.SetAutoSavePath("model.flpp");
				// 자동 저장 조건식 설정. 현재 cost값이 최소이고 metric값이 최대 값인 경우 저장 활성화
				// Set auto-save conditional expressions. Enable save if the metric value is maximum
				autoSaveSpec.SetAutoSaveCondition("metric > max('metric')");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				pix2pixDL.SetLearningAutoSaveSpec(autoSaveSpec);

				// Pix2Pix learn function을 진행하는 스레드 생성 // Create the Pix2Pix Learn function thread
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if((res = pix2pixDL.Learn()).IsFail())
						ErrorPrint(res, "Failed to execute Learn.\n");

					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!pix2pixDL.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = pix2pixDL.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevPSNRCount = 0;
				int i32PrevSSIMCount = 0;
				int i32PrevValidationCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 미니 배치 반복 횟수 받기 // Get the last maximum number of iterations of the last mini batch 
					int i32MiniBatchCount = pix2pixDL.GetActualMiniBatchCount();
					// 마지막 미니 배치 반복 횟수 받기 // Get the last number of mini batch iterations
					int i32Iteration = pix2pixDL.GetLearningResultCurrentIteration();
					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = pix2pixDL.GetLastEpoch();

					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Iteration == i32MiniBatchCount && i32Epoch > 0)
					{
						// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
						float f32CurrCost = pix2pixDL.GetLearningResultLastCost();
						// 마지막 PSNR 결과 받기 // Get the last PSNR result
						float f32PSNRPa = pix2pixDL.GetLearningResultLastPSNR();
						// 마지막 SSIM 결과 받기 // Get the last SSIM result
						float f32SSIMPa = pix2pixDL.GetLearningResultLastSSIM();
						// 마지막 검증 결과 받기 // Get the last validation result
						float f32ValidationPa = pix2pixDL.GetLearningResultLastAccuracy();

						// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
						Console.WriteLine("Cost : {0:F6} PSNR : {1:F6} SSIM : {2:F6} Accuracy : {3:F6}  Epoch {4} / {5}", f32CurrCost, f32PSNRPa, f32SSIMPa, f32ValidationPa, i32Epoch, i32MaxEpoch);

						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> flaCostHistory = new List<float>();
						List<float> flaPSNRHistory = new List<float>();
						List<float> flaSSIMHistory = new List<float>();
						List<float> flaValidationHistory = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						pix2pixDL.GetLearningResultAllHistory(ref flaCostHistory, ref flaValidationHistory, ref flaPSNRHistory, ref flaSSIMHistory,  ref vctValidationEpoch);

						// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
						if((flaCostHistory.Count() != 0 && i32PrevCostCount != flaCostHistory.Count()) || (flaPSNRHistory.Count() != 0 && i32PrevPSNRCount != flaPSNRHistory.Count()) || (flaSSIMHistory.Count() != 0 && i32PrevSSIMCount != flaSSIMHistory.Count()) || (flaValidationHistory.Count() != 0 && i32PrevValidationCount != flaValidationHistory.Count()))
						{
							int i32Step = pix2pixDL.GetLearningValidationStep();
							List<float> flaX = new List<float>();

							for(long i = 0; i < flaValidationHistory.Count() - 1; ++i)
								flaX.Add((float)(i * i32Step));

							flaX.Add((float)(flaCostHistory.Count() - 1));

							// 이전 그래프의 데이터를 삭제 // Clear previous graph data
							viewGraph.LockUpdate();
							viewGraph.Clear();

							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(flaCostHistory, EChartType.Line, EColor.RED, "Cost");
							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(flaX, flaPSNRHistory, EChartType.Line, EColor.BLUE, "PSNR");
							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(flaX, flaSSIMHistory, EChartType.Line, EColor.GREEN, "SSIM");
							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(flaX, flaValidationHistory, EChartType.Line, EColor.CYAN, "Accuracy");
							viewGraph.UnlockUpdate();

							viewGraph.UpdateWindow();
							viewGraph.Invalidate();
							viewGraph.RedrawWindow();
						}

						if(bEscape)
							pix2pixDL.Stop();

						i32PrevEpoch = i32Epoch;
						i32PrevCostCount = flaCostHistory.Count();
						i32PrevValidationCount = flaValidationHistory.Count();
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!pix2pixDL.IsRunning())
						break;
				}

				// Result Label Image에 피겨를 포함하지 않는 Execute
				// 분류할 이미지 설정 // Set the image to classify
				pix2pixDL.SetInferenceImage(ref fliValidationImageInput);
				// 추론 결과 이미지 설정 // Set the inference result Image
				pix2pixDL.SetInferenceResultImage(ref fliResultImage);

				// 알고리즘 수행 // Execute the algorithm
				if((res = pix2pixDL.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// 결과 이미지를 이미지 뷰에 맞게 조정합니다. // Fit the result image to the image view.
				viewImagesResult.ZoomFit();

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearnInput.RedrawWindow();
				viewImageLearnTarget.RedrawWindow();
				viewImageValidationInput.RedrawWindow();
				viewImageValidationTarget.RedrawWindow();
				viewImagesResult.RedrawWindow();

				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearnInput.IsAvailable() && viewImageValidationInput.IsAvailable() && viewImageLearnTarget.IsAvailable() && viewImageValidationTarget.IsAvailable() && viewImagesResult.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

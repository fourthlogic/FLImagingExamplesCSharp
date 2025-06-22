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

namespace SuperResolution
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
			// 라이브러리가 완전히 로드 될 때까지 기다림 // Wait for the library to fully load
			Thread.Sleep(1000);

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliLearnImageLowResolution = new CFLImage();
			CFLImage fliValidationImageLowResolution = new CFLImage();
			CFLImage fliLearnImageHighResolution = new CFLImage();
			CFLImage fliValidationImageHighResolution = new CFLImage();
			CFLImage fliResultLabelFigureImage = new CFLImage();
	
			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearnLowResolution = new CGUIViewImage();
			CGUIViewImage viewImageLearnHighResolution = new CGUIViewImage();
			CGUIViewImage viewImageValidationLowResolution = new CGUIViewImage();
			CGUIViewImage viewImageValidationHighResolution = new CGUIViewImage();
			CGUIViewImage viewImagesLabelFigure = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImageLowResolution.Load("../../ExampleImages/SuperResolution/SuperResolutionTrainDataLowResolution.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}
				
				// 이미지 로드 // Load image
				if((res = fliLearnImageHighResolution.Load("../../ExampleImages/SuperResolution/SuperResolutionTrainDataHighResolution.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliValidationImageLowResolution.Load("../../ExampleImages/SuperResolution/SuperResolutionValidationDataLowResolution.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliValidationImageHighResolution.Load("../../ExampleImages/SuperResolution/SuperResolutionValidationDataHighResolution.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearnLowResolution.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearnHighResolution.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageValidationLowResolution.Create(100, 500, 600, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageValidationHighResolution.Create(600, 500, 1100, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImagesLabelFigure.Create(1100, 0, 1600, 500)).IsFail())
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
				if((res = viewImageLearnLowResolution.SetImagePtr(ref fliLearnImageLowResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearnHighResolution.SetImagePtr(ref fliLearnImageHighResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageValidationLowResolution.SetImagePtr(ref fliValidationImageLowResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageValidationHighResolution.SetImagePtr(ref fliValidationImageHighResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImagesLabelFigure.SetImagePtr(ref fliResultLabelFigureImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 다섯 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the four image view windows
				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewImageValidationLowResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 다섯 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the four image view windows
				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewImageLearnHighResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewImageValidationLowResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewImageValidationHighResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewImagesLabelFigure)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewGraph)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnLowResolution.SynchronizeWindow(ref viewGraph)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearnLowResolution.SynchronizePageIndex(ref viewImageLearnHighResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageValidationLowResolution.SynchronizePageIndex(ref viewImageValidationHighResolution)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageValidationHighResolution.SynchronizePageIndex(ref viewImagesLabelFigure)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageValidationHighResolution.SynchronizePointOfView(ref viewImagesLabelFigure)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearnLowResolution = viewImageLearnLowResolution.GetLayer(0);
				CGUIViewImageLayer layerLearnHighResolution = viewImageLearnHighResolution.GetLayer(0);
				CGUIViewImageLayer layerValidationLowResolution = viewImageValidationLowResolution.GetLayer(0);
				CGUIViewImageLayer layerValidationHighResolution = viewImageValidationHighResolution.GetLayer(0);
				CGUIViewImageLayer layerResultLabelFigure = viewImagesLabelFigure.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearnLowResolution.Clear();
				layerLearnHighResolution.Clear();
				layerValidationLowResolution.Clear();
				layerValidationHighResolution.Clear();
				layerResultLabelFigure.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = layerLearnLowResolution.DrawTextCanvas(flpPoint, "LEARN LOW RESOLUTION", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerLearnHighResolution.DrawTextCanvas(flpPoint, "LEARN HIGH RESOLUTION", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerValidationLowResolution.DrawTextCanvas(flpPoint, "VALIDATION LOW RESOLUTION", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerValidationHighResolution.DrawTextCanvas(flpPoint, "VALIDATION HIGH RESOLUTION", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResultLabelFigure.DrawTextCanvas(flpPoint, "INFERENCE RESULT", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearnLowResolution.RedrawWindow();
				viewImageLearnHighResolution.RedrawWindow();
				viewImageValidationLowResolution.RedrawWindow();
				viewImageValidationHighResolution.RedrawWindow();
				viewImagesLabelFigure.RedrawWindow();

				// SuperResolution 객체 생성 // Create SuperResolution object
				CSuperResolutionDL superResolution = new CSuperResolutionDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				superResolution.SetLearningLowResolutionImage(ref fliLearnImageLowResolution);
				superResolution.SetLearningHighResolutionImage(ref fliLearnImageHighResolution);

				// 검증할 이미지 설정 // Set the image to validation
				superResolution.SetLearningLowResolutionValidationImage(ref fliValidationImageLowResolution);
				superResolution.SetLearningHighResolutionValidationImage(ref fliValidationImageHighResolution);

				// 분류할 이미지 설정 // Set the image to classify
				superResolution.SetInferenceImage(ref fliValidationImageLowResolution);
				superResolution.SetInferenceResultImage(ref fliResultLabelFigureImage);

				// 학습할 SuperResolution 모델 설정 // Set up the SuperResolution model to learn
				superResolution.SetModel(CSuperResolutionDL.EModel.SRCNN);
				// 학습할 SuperResolution 모델 Version 설정 // Set up the SuperResolution model version to learn
				superResolution.SetModelVersion(CSuperResolutionDL.EModelVersion.SRCNN_V1_128);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				superResolution.SetLearningEpoch(500);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				superResolution.SetInterpolationMethod(EInterpolationMethod.Bilinear);
				// 이미지 배율 설정 // Set Scale Ratio
				superResolution.SetScaleRatio(2);
				// 모델의 최적의 상태를 추적 후 마지막에 최적의 상태로 적용할 지 여부 설정 // Set whether to track the optimal state of the model and apply it as the optimal state at the end.
				superResolution.EnableOptimalLearningStatePreservation(true);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(.001f);

				// 설정한 Optimizer를 SuperResolution에 적용 // Apply Optimizer that we set up to SuperResolution
				superResolution.SetLearningOptimizerSpec(optSpec);

				// AugmentationSpec 설정 // Set the AugmentationSpec
				CAugmentationSpec augSpec = new CAugmentationSpec();

				augSpec.EnableAugmentation(true);
				augSpec.SetCommonActivationRate(0.5);
				augSpec.SetCommonInterpolationMethod(EInterpolationMethod.Bilinear);
				augSpec.EnableTranslation(true);
				augSpec.SetTranslationParam(0.0, 0.1, 0.0, 0.1, 1.0);
				augSpec.EnableHorizontalFlip(true);
				augSpec.EnableVerticalFlip(true);

				superResolution.SetLearningAugmentationSpec(augSpec);

				// 학습을 종료할 조건식 설정. accuracy값이 0.9 이상인 경우 학습 종료한다.
				// Set Conditional Expression to End Learning. If the accuracy value is 0.9 or more, end learning.
				superResolution.SetLearningStopCondition("accuracy >= 0.9");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				CAutoSaveSpec autoSaveSpec = new CAutoSaveSpec();

				// 자동 저장 활성화 // Enable Auto-Save
				// 저장 때문에 발생하는 속도 저하를 막기 위해 예제에서는 코드 사용법만 표시하고 옵션은 끔 // To prevent performance degradation caused by saving, the examples only demonstrate how to use the code, with the saving option disabled.
				autoSaveSpec.EnableAutoSave(false);
				// 저장할 모델 경로 설정 // Set Model path to save
				autoSaveSpec.SetAutoSavePath("model.flsr");
				// 자동 저장 조건식 설정. 현재 cost값이 최소이고 accuracy값이 최대 값인 경우 저장 활성화
				// Set auto-save conditional expressions. Enable save if the current cost value is minimum and the accumulation value is maximum
				autoSaveSpec.SetAutoSaveCondition("cost < min('cost') & accuracy > max('accuracy')");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				superResolution.SetLearningAutoSaveSpec(autoSaveSpec);

				// SuperResolution learn function을 진행하는 스레드 생성 // Create the SuperResolution Learn function thread
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if((res = superResolution.Learn()).IsFail())
						ErrorPrint(res, "Failed to execute Learn.\n");

					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!superResolution.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = superResolution.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevPSNRCount = 0;
				int i32PrevSSIMCount = 0;
				int i32PrevValidationCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 미니 배치 반복 횟수 받기 // Get the last maximum number of iterations of the last mini batch 
					int i32MiniBatchCount = superResolution.GetActualMiniBatchCount();
					// 마지막 미니 배치 반복 횟수 받기 // Get the last number of mini batch iterations
					int i32Iteration = superResolution.GetLearningResultCurrentIteration();
					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = superResolution.GetLastEpoch();

					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Iteration == i32MiniBatchCount && i32Epoch > 0)
					{
						// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
						float f32CurrCost = superResolution.GetLearningResultLastCost();
						// 마지막 PSNR 결과 받기 // Get the last PSNR result
						float f32PSNRPa = superResolution.GetLearningResultLastPSNR();
						// 마지막 SSIM 결과 받기 // Get the last SSIM result
						float f32SSIMPa = superResolution.GetLearningResultLastSSIM();
						// 마지막 검증 결과 받기 // Get the last validation result
						float f32ValidationPa = superResolution.GetLearningResultLastAccuracy();

						// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
						Console.WriteLine("Cost : {0:F6} PSNR : {0:F6} SSIM : {0:F6} Accuracy : {1:F6}  Epoch {2} / {3}", f32CurrCost, f32PSNRPa, f32SSIMPa, f32ValidationPa, i32Epoch, i32MaxEpoch);

						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> flaCostHistory = new List<float>();
						List<float> flaPSNRHistory = new List<float>();
						List<float> flaSSIMHistory = new List<float>();
						List<float> flaValidationHistory = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						superResolution.GetLearningResultAllHistory(out flaCostHistory, out flaValidationHistory, out flaPSNRHistory, out flaSSIMHistory,  out vctValidationEpoch);

						// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
						if((flaCostHistory.Count() != 0 && i32PrevCostCount != flaCostHistory.Count()) || (flaPSNRHistory.Count() != 0 && i32PrevPSNRCount != flaPSNRHistory.Count()) || (flaSSIMHistory.Count() != 0 && i32PrevSSIMCount != flaSSIMHistory.Count()) || (flaValidationHistory.Count() != 0 && i32PrevValidationCount != flaValidationHistory.Count()))
						{
							int i32Step = superResolution.GetLearningValidationStep();
							List<float> flaX = new List<float>();

							for(long i = 0; i < flaValidationHistory.Count() - 1; ++i)
								flaX.Add((float)(i * i32Step));

							flaX.Add((float)(flaCostHistory.Count() - 1));

							// 이전 그래프의 데이터를 삭제 // Clear previous grpah data
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

						// 검증 결과가 1.0일 경우 학습을 중단하고 분류 진행 
						// If the validation result is 1.0, stop learning and classify images 
						if(f32ValidationPa == 1.0f || bEscape)
							superResolution.Stop();

						i32PrevEpoch = i32Epoch;
						i32PrevCostCount = flaCostHistory.Count();
						i32PrevValidationCount = flaValidationHistory.Count();
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!superResolution.IsRunning())
						break;
				}

				// Result Label Image에 피겨를 포함하지 않는 Execute
				// 분류할 이미지 설정 // Set the image to classify
				superResolution.SetInferenceImage(ref fliValidationImageLowResolution);
				// 추론 결과 이미지 설정 // Set the inference result Image
				superResolution.SetInferenceResultImage(ref fliResultLabelFigureImage);

				// 알고리즘 수행 // Execute the algorithm
				if((res = superResolution.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// 결과 이미지를 이미지 뷰에 맞게 조정합니다. // Fit the result image to the image view.
				viewImagesLabelFigure.ZoomFit();

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearnLowResolution.RedrawWindow();
				viewImageLearnHighResolution.RedrawWindow();
				viewImageValidationLowResolution.RedrawWindow();
				viewImageValidationHighResolution.RedrawWindow();
				viewImagesLabelFigure.RedrawWindow();

				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearnLowResolution.IsAvailable() && viewImageValidationLowResolution.IsAvailable() && viewImageLearnHighResolution.IsAvailable() && viewImageValidationHighResolution.IsAvailable() && viewImagesLabelFigure.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

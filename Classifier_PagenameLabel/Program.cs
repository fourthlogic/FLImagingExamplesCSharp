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

namespace Classifier
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
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliSourceImage = new CFLImage();

			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageSource = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();

			CResult res = new CResult();

			bool bTerminated = false;

			do
			{
				// 라이브러리가 완전히 로드 될 때까지 기다림 // Wait for the library to fully load
				Thread.Sleep(1000);

				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/Classifier/mnist1000.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliSourceImage.Load("../../ExampleImages/Classifier/mnist100.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearn.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageSource.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}
				// Graph 뷰 생성 // Create graph view
				if((res = viewGraph.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, " Failed to create the graph view. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageSource)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerSource.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((res = layerLearn.DrawTextCanvas(flpPoint, "LEARN", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerSource.DrawTextCanvas(flpPoint, "INFERENCE", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// Classifier 객체 생성 // Create Classifier object
				CClassifierDL classifier = new CClassifierDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				classifier.SetLearningImage(ref fliLearnImage);
				// 검증할 이미지 설정 // Set the image to validate
				classifier.SetLearningValidationImage(ref fliSourceImage);
				// 분류할 이미지 설정 // Set the image to classify
				classifier.SetInferenceImage(ref fliSourceImage);
				classifier.SetInferenceResultImage(ref fliSourceImage);

				// 학습할 Classifier 모델 설정 // Set up the Classifier model to learn
				classifier.SetModel(CClassifierDL.EModel.FL_CF_C);
				// 학습할 Classifier 모델 설정 // Set up the Classifier model to learn
				classifier.SetModelVersion(CClassifierDL.EModelVersion.FL_CF_C_V1_32);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				classifier.SetLearningEpoch(150);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				classifier.SetInterpolationMethod(EInterpolationMethod.Bilinear);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(.001f);
				// 설정한 Optimizer를 Classifier에 적용 // Apply Optimizer that we set up to Classifier
				classifier.SetLearningOptimizerSpec(optSpec);
				// 모델의 최적의 상태를 추적 후 마지막에 최적의 상태로 적용할 지 여부 설정 // Set whether to track the optimal state of the model and apply it as the optimal state at the end.
				classifier.EnableOptimalLearningStatePreservation(true);

				// 학습을 종료할 조건식 설정. f1score값이 0.999 이상인 경우 학습 종료한다. metric와 동일한 값입니다.
				// Set Conditional Expression to End Learning. If the f1score value is 0.999 or higher, end the learning. Same value as metric.
				classifier.SetLearningStopCondition("f1score >= 0.999");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				CAutoSaveSpec autoSaveSpec = new CAutoSaveSpec();

				// 자동 저장 활성화 // Enable Auto-Save
				// 저장 때문에 발생하는 속도 저하를 막기 위해 예제에서는 코드 사용법만 표시하고 옵션은 끔 // To prevent performance degradation caused by saving, the examples only demonstrate how to use the code, with the saving option disabled.
				autoSaveSpec.EnableAutoSave(false);
				// 저장할 모델 경로 설정 // Set Model path to save
				autoSaveSpec.SetAutoSavePath("model.flcf");
				// 자동 저장 조건식 설정. 현재 f1score값이 최대 값인 경우 저장 활성화
				// Set auto-save conditional expressions. Enable save if the current f1score value is the maximum value
				autoSaveSpec.SetAutoSaveCondition("f1score > max('f1score')");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				classifier.SetLearningAutoSaveSpec(autoSaveSpec);

				// Classifier learn function을 진행하는 스레드 생성 // Create the Classifier Learn function thread
				CResult eLearnResult = new CResult();

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					eLearnResult = classifier.Learn();
					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!classifier.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = classifier.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevValidationCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 미니 배치 최대 반복 횟수 받기 // Get the last maximum number of iterations of the last mini batch 
					int i32MaxIteration = classifier.GetActualMiniBatchCount();
					// 마지막 미니 배치 반복 횟수 받기 // Get the last number of mini batch iterations
					int i32Iteration = classifier.GetLearningResultCurrentIteration();
					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = classifier.GetLastEpoch();

					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Iteration == i32MaxIteration && i32Epoch > 0)
					{
						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> vctCosts = new List<float>();
						List<float> vctValidations = new List<float>();
						List<float> vctF1Score = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						classifier.GetLearningResultAllHistory(ref vctCosts, ref vctValidations, ref vctF1Score, ref vctValidationEpoch);

						if(vctCosts.Count != 0)
						{
							// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
							float f32CurrCost = vctCosts.Last();
							// 마지막 검증 결과 받기 // Get the last validation result
							float f32Validation = vctValidations.Count != 0 ? vctValidations.Last() : 0;
							// 마지막 F1점수 결과 받기 // Get the last F1 Score result
							float f32F1Score = vctF1Score.Count != 0 ? vctF1Score.Last() : 0;

							// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
							Console.WriteLine("Cost : {0:F6} Validation : {1:F6} F1 Score : {2:F6} Epoch {3} / {4}", f32CurrCost, f32Validation, f32F1Score, i32Epoch, i32MaxEpoch);

							// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
							if((vctCosts.Count() != 0 && i32PrevCostCount != vctCosts.Count()) || (vctValidations.Count() != 0 && i32PrevValidationCount != vctValidations.Count()))
							{
								viewGraph.LockUpdate();

								// 이전 그래프의 데이터를 삭제 // Clear previous grpah data
								viewGraph.Clear();
								// Graph View 데이터 입력 // Input Graph View Data
								viewGraph.Plot(vctCosts, EChartType.Line, EColor.RED, "Cost");

								int i32Step = classifier.GetLearningValidationStep();
								List<float> flaX = new List<float>();

								for(long i = 0; i < vctValidations.Count() - 1; ++i)
									flaX.Add((float)(i * i32Step));

								flaX.Add((float)(vctCosts.Count() - 1));
								// Graph View 데이터 입력 // Input Graph View Data
								viewGraph.Plot(flaX, vctValidations, EChartType.Line, EColor.BLUE, "Validation");

								viewGraph.UnlockUpdate();
								viewGraph.Invalidate();
							}

							// 검증 결과가 1.0일 경우 학습을 중단하고 분류 진행 
							// If the validation result is 1.0, stop learning and classify images 
							if(f32Validation == 1.0f || bEscape)
								classifier.Stop();

							i32PrevEpoch = i32Epoch;
							i32PrevCostCount = vctCosts.Count();
							i32PrevValidationCount = vctValidations.Count();
						}
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!classifier.IsRunning())
						break;
				}

				if(eLearnResult.IsFail())
				{
					ErrorPrint(eLearnResult, "Failed to learn");
					break;
				}

				// 추론 결과 정보에 대한 설정 // Set for the inference result information
				classifier.SetInferenceResultItemSettings(CClassifierDL.EInferenceResultItemSettings.ClassNum_ClassName_ConfidenceScore);

				// 알고리즘 수행 // Execute the algorithm
				if((res = classifier.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}
				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.Invalidate(true);
				viewImageSource.Invalidate(true);
				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewImageSource.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

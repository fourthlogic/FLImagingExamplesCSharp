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

namespace ObjectDetection
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
			CFLImage fliValidationImage = new CFLImage();
			CFLImage fliResultImage = new CFLImage();
	
			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageValidation = new CGUIViewImage();
			CGUIViewImage viewImagesLabel = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;

			CResult res = new CResult();

			do
			{
				// 라이브러리가 완전히 로드 될 때까지 기다림 // Wait for the library to fully load
				Thread.Sleep(1000);

				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/ObjectDetection/Train.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliValidationImage.Load("../../ExampleImages/ObjectDetection/Validation.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearn.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageValidation.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImagesLabel.Create(100, 500, 600, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((res = viewGraph.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, " Failed to create the graph view. \n");
					break;
				}

				viewGraph.SetDarkMode();

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageValidation.SetImagePtr(ref fliValidationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImagesLabel.SetImagePtr(ref fliResultImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 다섯 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the four image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageValidation)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearn.SynchronizeWindow(ref viewImagesLabel)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerValidation = viewImageValidation.GetLayer(0);
				CGUIViewImageLayer layerResultLabel = viewImagesLabel.GetLayer(0);
		
				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerValidation.Clear();
				layerResultLabel.Clear();

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
				
				if((res = layerValidation.DrawTextCanvas(flpPoint, "VALIDATION", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResultLabel.DrawTextCanvas(flpPoint, "RESULT LABEL", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImagesLabel.RedrawWindow();
		
				// ObjectDetection 객체 생성 // Create ObjectDetection object
				CObjectDetectionDL objectDetection = new CObjectDetectionDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				objectDetection.SetLearningImage(ref fliLearnImage);
				// 검증할 이미지 설정 // Set the image to validate
				objectDetection.SetLearningValidationImage(ref fliValidationImage);
				// 분류할 이미지 설정 // Set the image to classify
				objectDetection.SetInferenceImage(ref fliValidationImage);
				objectDetection.SetInferenceResultImage(ref fliResultImage);

				// 학습할 ObjectDetection 모델 설정 // Set up the ObjectDetection model to learn
				objectDetection.SetModel(CObjectDetectionDL.EModel.YOLO);
				// 학습할 ObjectDetection 모델 Version 설정 // Set up the ObjectDetection model version to learn
				objectDetection.SetModelVersion(CObjectDetectionDL.EModelVersion.YOLO_V4_256);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				objectDetection.SetLearningEpoch(1024);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				objectDetection.SetInterpolationMethod(EInterpolationMethod.Bilinear);
				// 검증을 진행 할 최소 평균 Cost값 설정 // Set the minimum average cost value at which verification will be triggered
				objectDetection.SetLearningRequiredCostForValidation(5);
				// 학습 중단 Metrics 값 설정 // Set Learning Stop Metrics Values
				String strStopConditon = "mAP >= 0.9";
				objectDetection.SetLearningStopCondition(strStopConditon);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(5e-4f);
				optSpec.SetWeightDecay(0);

				// 설정한 Optimizer를 ObjectDetection에 적용 // Apply Optimizer that we set up to ObjectDetection
				objectDetection.SetLearningOptimizerSpec(optSpec);

				// AugmentationSpec 설정 // Set the AugmentationSpec
				CAugmentationSpec augSpec = new CAugmentationSpec();

				augSpec.SetCommonActivationRatio(0.8);
				augSpec.SetCommonInterpolationMethod(EInterpolationMethod.Bilinear);
				augSpec.EnableRotation(true);
				augSpec.SetRotationParam(30.0, true, false);
				augSpec.EnableScaleCrop(true);
				augSpec.SetScaleCropParam(.8, 1.0, true, 1.0, 1.0, 1.0, 1.0, true);
				augSpec.EnableHorizontalFlip(true);
				augSpec.EnableVerticalFlip(true);

				objectDetection.SetLearningAugmentationSpec(augSpec);

				// ObjectDetection learn function을 진행하는 스레드 생성 // Create the ObjectDetection Learn function thread
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if((res = objectDetection.Learn()).IsFail())
						ErrorPrint(res, "Failed to execute Learn.\n");
					
					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!objectDetection.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = objectDetection.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevValidationCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = objectDetection.GetLastEpoch();
			
					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Epoch > 0)
					{
						// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
						float f32CurrCost = objectDetection.GetLearningResultLastCost();
						// 마지막 평균 학습 결과 비용 받기 // Get the last cost of the learning result
						float f32AvgCost = objectDetection.GetLearningResultLastAverageCost();
						// 마지막 검증 결과 받기 // Get the last validation result
						float f32Validation = objectDetection.GetLearningResultLastMeanAP();
			
						// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
						if(f32AvgCost < objectDetection.GetLearningRequiredCostForValidation())
							Console.WriteLine("Cost : {0:F6} Avg Cost : {1:F6} mAP : {2:F6} Epoch {3} / {4}", f32CurrCost, f32AvgCost, f32Validation, i32Epoch, i32MaxEpoch);
						else
							Console.WriteLine("Cost : {0:F6} Avg Cost : {1:F6} Epoch {2} / {3}", f32CurrCost, f32AvgCost, i32Epoch, i32MaxEpoch);

						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> vctCosts = new List<float>();
						List<float> vctAvgCosts = new List<float>();
						List<float> vctmAP = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						objectDetection.GetLearningResultAllHistory(out vctCosts, out vctAvgCosts, out vctmAP, out vctValidationEpoch);

						// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
						if((vctCosts.Count() != 0 && i32PrevCostCount != vctCosts.Count()) || (vctmAP.Count() != 0 && i32PrevValidationCount != vctmAP.Count()))
						{
							int i32Step = objectDetection.GetLearningValidationStep();
							List<float> flaX = new List<float>();

							for(long i = 0; i < vctmAP.Count() - 1; ++i)
								flaX.Add((float)(i * i32Step));

							flaX.Add((float)(vctCosts.Count() - 1));

							// 이전 그래프의 데이터를 삭제 // Clear previous graph data
							viewGraph.LockUpdate();
							viewGraph.Clear();

							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(vctCosts, EChartType.Line, EColor.RED, "Cost");
							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(vctAvgCosts, EChartType.Line, EColor.CYAN, "Avg Cost");
							viewGraph.Plot(flaX, vctmAP, EChartType.Line, EColor.PINK, "mAP");
							viewGraph.UnlockUpdate();

							viewGraph.UpdateWindow();
							viewGraph.Invalidate();
							viewGraph.RedrawWindow();
						}

						// 검증 결과가 1.0일 경우 학습을 중단하고 분류 진행 
						// If the validation result is 1.0, stop learning and classify images 
						if(f32Validation == 1.0f || bEscape)
							objectDetection.Stop();

						i32PrevEpoch = i32Epoch;
						i32PrevCostCount = vctCosts.Count();
						i32PrevValidationCount = vctmAP.Count();
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!objectDetection.IsRunning())
						break;
				}

				// Result Label Image에 피겨를 포함하지 않는 Execute
				// 분류할 이미지 설정 // Set the image to classify
				objectDetection.SetInferenceImage(ref fliValidationImage);
				// 추론 결과 이미지 설정 // Set the inference result Image
				objectDetection.SetInferenceResultImage(ref fliResultImage);
				// 추론 결과 옵션 설정 // Set the inference result options;
				// Result 결과의 옵션 설정 // Set the option of results
				objectDetection.SetInferenceResultItemSettings(CObjectDetectionDL.EInferenceResultItemSettings.ClassNum_ClassName_Objectness);

				// 알고리즘 수행 // Execute the algorithm
				if((res = objectDetection.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImagesLabel.RedrawWindow();
			
				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewImageValidation.IsAvailable() && viewImagesLabel.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

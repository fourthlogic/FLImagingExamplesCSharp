﻿using System;
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

namespace SemanticSegmentation
{
	class SemanticSegmentation
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
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliValidationImage = new CFLImage();
			CFLImage fliResultLabelImage = new CFLImage();
			CFLImage fliResultLabelFigureImage = new CFLImage();
	
			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageValidation = new CGUIViewImage();
			CGUIViewImage viewImagesLabel = new CGUIViewImage();
			CGUIViewImage viewImagesLabelFigure = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/SemanticSegmentation/Train.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliValidationImage.Load("../../ExampleImages/SemanticSegmentation/Validation.flif")).IsFail())
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

				if((res = viewImagesLabelFigure.Create(600, 500, 1100, 1000)).IsFail())
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

				viewImagesLabel.EnablePixelSegmentationMode(true);

				if((res = viewImagesLabel.SetImagePtr(ref fliResultLabelImage)).IsFail())
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

				if((res = viewImageLearn.SynchronizeWindow(ref viewImagesLabelFigure)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerValidation = viewImageValidation.GetLayer(0);
				CGUIViewImageLayer layerResultLabel = viewImagesLabel.GetLayer(0);
				CGUIViewImageLayer layerResultLabelFigure = viewImagesLabelFigure.GetLayer(0);
	
				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerValidation.Clear();
				layerResultLabel.Clear();
				layerResultLabelFigure.Clear();
	
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

				if((res = layerResultLabel.DrawTextCanvas(flpPoint, "RESULT", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResultLabelFigure.DrawTextCanvas(flpPoint, "RESULT FIGURE", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 결과 이미지를 이미지 뷰에 맞게 조정합니다. // Fit the result image to the image view.
				viewImagesLabel.ZoomFit();
				viewImagesLabelFigure.ZoomFit();                
				
				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImagesLabel.RedrawWindow();
				viewImagesLabelFigure.RedrawWindow();
		
				// SemanticSegmentation 객체 생성 // Create SemanticSegmentation object
				CSemanticSegmentationDL semanticSegmentation = new CSemanticSegmentationDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				semanticSegmentation.SetLearningImage(ref fliLearnImage);
				// 검증할 이미지 설정 // Set the image to validate
				semanticSegmentation.SetLearningValidationImage(ref fliValidationImage);
				// 분류할 이미지 설정 // Set the image to classify
				semanticSegmentation.SetInferenceImage(ref fliValidationImage);
				semanticSegmentation.SetInferenceResultImage(ref fliResultLabelImage);

				// 학습할 SemanticSegmentation 모델 설정 // Set up the SemanticSegmentation model to learn
				semanticSegmentation.SetModel(CSemanticSegmentationDL.EModel.FLSegNet);
				// 학습할 SemanticSegmentation 모델 Version 설정 // Set up the SemanticSegmentation model version to learn
				semanticSegmentation.SetModelVersion(CSemanticSegmentationDL.EModelVersion.FLSegNet_V1_512_B3);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				semanticSegmentation.SetLearningEpoch(120);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				semanticSegmentation.SetInterpolationMethod(EInterpolationMethod.Bilinear);
				// 모델의 최적의 상태를 추적 후 마지막에 최적의 상태로 적용할 지 여부 설정 // Set whether to track the optimal state of the model and apply it as the optimal state at the end.
				semanticSegmentation.EnableOptimalLearningStatePreservation(true);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(.001f);

				// 설정한 Optimizer를 SemanticSegmentation에 적용 // Apply Optimizer that we set up to SemanticSegmentation
				semanticSegmentation.SetLearningOptimizerSpec(optSpec);

				// AugmentationSpec 설정 // Set the AugmentationSpec
				CAugmentationSpec augSpec = new CAugmentationSpec();

				augSpec.EnableAugmentation(true);
				augSpec.SetCommonActivationRate(0.5);
				augSpec.SetCommonInterpolationMethod(EInterpolationMethod.Bilinear);
				augSpec.EnableRotation(true);
				augSpec.SetRotationParam(-180.0, 180.0, false, true, 1.0);
				augSpec.EnableHorizontalFlip(true);
				augSpec.EnableVerticalFlip(true);

				semanticSegmentation.SetLearningAugmentationSpec(augSpec);

				// 학습을 종료할 조건식 설정. miou.ze값이 0.85 이상인 경우 학습 종료한다. metric.ze와 동일한 값입니다.
				// Set Conditional Expression to End Learning. If the miou.ze value is 0.85 or higher, end the learning. Same value as metric.ze.
				semanticSegmentation.SetLearningStopCondition("mIoU.ze >= 0.85");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				CAutoSaveSpec autoSaveSpec = new CAutoSaveSpec();

				// 자동 저장 활성화 // Enable Auto-Save
				// 저장 때문에 발생하는 속도 저하를 막기 위해 예제에서는 코드 사용법만 표시하고 옵션은 끔 // To prevent performance degradation caused by saving, the examples only demonstrate how to use the code, with the saving option disabled.
				autoSaveSpec.EnableAutoSave(false);
				// 저장할 모델 경로 설정 // Set Model path to save
				autoSaveSpec.SetAutoSavePath("model.flss");
				// 자동 저장 조건식 설정. 현재 miou.ze값이 최대 값인 경우 저장 활성화
				// Set auto-save conditional expressions. Enable save if the current miou.ze value is the maximum value
				autoSaveSpec.SetAutoSaveCondition("miou.ze > max('miou.ze')");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				semanticSegmentation.SetLearningAutoSaveSpec(autoSaveSpec);

				// SemanticSegmentation learn function을 진행하는 스레드 생성 // Create the SemanticSegmentation Learn function thread
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if((res = semanticSegmentation.Learn()).IsFail())
						ErrorPrint(res, "Failed to execute Learn.\n");
					
					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!semanticSegmentation.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = semanticSegmentation.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevValidationCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 미니 배치 반복 횟수 받기 // Get the last maximum number of iterations of the last mini batch 
					int i32MiniBatchCount = semanticSegmentation.GetActualMiniBatchCount();
					// 마지막 미니 배치 반복 횟수 받기 // Get the last number of mini batch iterations
					int i32Iteration = semanticSegmentation.GetLearningResultCurrentIteration();
					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = semanticSegmentation.GetLastEpoch();
			
					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Iteration == i32MiniBatchCount && i32Epoch > 0)
					{
						// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
						float f32CurrCost = semanticSegmentation.GetLearningResultLastCost();
						// 마지막 검증 결과 받기 // Get the last validation result
						float f32ValidationPa = semanticSegmentation.GetLearningResultLastAccuracy();
						float f32ValidationPaMeanIoU = semanticSegmentation.GetLearningResultLastMeanIoU();

						// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
						Console.WriteLine("Cost : {0:F6} Pixel Accuracy : {1:F6} mIoU : {2:F6} Epoch {3} / {4}", f32CurrCost, f32ValidationPa, f32ValidationPaMeanIoU, i32Epoch, i32MaxEpoch);

						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> vctCosts = new List<float>();
						List<float> vctValidations = new List<float>();
						List<float> vctMeanIoU = new List<float>();
						List<float> vctValidationsZE = new List<float>();
						List<float> vctMeanIoUZE = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						semanticSegmentation.GetLearningResultAllHistory(ref vctCosts, ref vctValidations, ref vctMeanIoU, ref vctValidationsZE, ref vctMeanIoUZE, ref vctValidationEpoch);

						// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
						if((vctCosts.Count() != 0 && i32PrevCostCount != vctCosts.Count()) || (vctValidations.Count() != 0 && i32PrevValidationCount != vctValidations.Count()))
						{
							int i32Step = semanticSegmentation.GetLearningValidationStep();
							List<float> flaX = new List<float>();

							for(long i = 0; i < vctValidations.Count() - 1; ++i)
								flaX.Add((float)(i * i32Step));

							flaX.Add((float)(vctCosts.Count() - 1));

							// 이전 그래프의 데이터를 삭제 // Clear previous grpah data
							viewGraph.LockUpdate();
							viewGraph.Clear();

							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(vctCosts, EChartType.Line, EColor.RED, "Cost");
							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(flaX, vctValidations, EChartType.Line, EColor.CYAN, "Validation");
							viewGraph.Plot(flaX, vctMeanIoU, EChartType.Line, EColor.PINK, "mIoU");
							viewGraph.UnlockUpdate();

							viewGraph.UpdateWindow();
							viewGraph.Invalidate();
							viewGraph.RedrawWindow();
						}

						// 검증 결과가 1.0일 경우 학습을 중단하고 분류 진행 
						// If the validation result is 1.0, stop learning and classify images 
						if(f32ValidationPa == 1.0f || bEscape)
							semanticSegmentation.Stop();

						i32PrevEpoch = i32Epoch;
						i32PrevCostCount = vctCosts.Count();
						i32PrevValidationCount = vctValidations.Count();
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!semanticSegmentation.IsRunning())
						break;
				}

				// Result Label Image에 피겨를 포함하지 않는 Execute
				// 분류할 이미지 설정 // Set the image to classify
				semanticSegmentation.SetInferenceImage(ref fliValidationImage);
				// 추론 결과 이미지 설정 // Set the inference result Image
				semanticSegmentation.SetInferenceResultImage(ref fliResultLabelImage);
				// 추론 결과 옵션 설정 // Set the inference result options;
				// Result 결과를 Label Image로 받을지 여부 설정 // Set whether to receive the result as a Label Image
				semanticSegmentation.EnableInferenceResultLabelImage(true);
				// Result 결과에 Region Figure를 포함 여부 설정 // Set whether to include region figure in result
				semanticSegmentation.EnableInferenceResultIncludingRegionFigures(false);

				// 알고리즘 수행 // Execute the algorithm
				if((res = semanticSegmentation.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// Result Label Image에 피겨를 포함한 Execute
				// 추론 결과 이미지 설정 // Set the inference result Image
				semanticSegmentation.SetInferenceResultImage(ref fliResultLabelFigureImage);
				// 추론 결과 옵션 설정 // Set the inference result options;
				// Result 결과를 Label Image로 받을지 여부 설정 // Set whether to receive the result as a Label Image
				semanticSegmentation.EnableInferenceResultLabelImage(false);
				// Result 결과에 Region Figure를 포함 여부 설정 // Set whether to include region figure in result
				semanticSegmentation.EnableInferenceResultIncludingRegionFigures(true);
				// Result item settings enum 설정 // Set the result item settings
				semanticSegmentation.SetInferenceResultItemSettings(CSemanticSegmentationDL.EInferenceResultItemSettings.ClassNum_ClassName_ConfidenceScore_RegionType_Contour);

				// 알고리즘 수행 // Execute the algorithm
				if((res = semanticSegmentation.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				int i32LearningClassCount = semanticSegmentation.GetLearningResultClassCount();
				// ResultContours 인덱스와 매칭 되는 라벨 번호배열을 가져오기 // ResultContours Get an array of label numbers matching the index.
				for(int classNum = 1; classNum < i32LearningClassCount; ++classNum)
				{
					List<string> flaNames = new List<string>();

					semanticSegmentation.GetLearningResultClassNames(classNum,ref flaNames);
					viewImagesLabel.SetSegmentationLabelText(0, (double)classNum, flaNames[0]);
				}

				// ResultLabl 뷰에 Floating Value Range를 설정
				viewImagesLabel.SetFloatingImageValueRange(0, (float)semanticSegmentation.GetLearningResultClassCount());

				// 결과 이미지를 이미지 뷰에 맞게 조정합니다. // Fit the result image to the image view.
				viewImagesLabel.ZoomFit();
				viewImagesLabelFigure.ZoomFit();

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImagesLabel.RedrawWindow();
				viewImagesLabelFigure.RedrawWindow();
			
				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewImageValidation.IsAvailable() && viewImagesLabel.IsAvailable() && viewImagesLabelFigure.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

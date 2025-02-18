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

namespace InstanceSegmentation
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
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliValidationImage = new CFLImage();
			CFLImage fliResultBoxContourImage = new CFLImage();
			CFLImage fliResultContourImage = new CFLImage();
	
			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageValidation = new CGUIViewImage();
			CGUIViewImage viewImagesBoxContour = new CGUIViewImage();
			CGUIViewImage viewImagesContour = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/InstanceSegmentation/Bolt.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliValidationImage.Load("../../ExampleImages/InstanceSegmentation/Bolt.flif")).IsFail())
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

				if((res = viewImagesBoxContour.Create(100, 500, 600, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImagesContour.Create(600, 500, 1100, 1000)).IsFail())
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

				if((res = viewImagesBoxContour.SetImagePtr(ref fliResultBoxContourImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImagesContour.SetImagePtr(ref fliResultContourImage)).IsFail())
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

				if((res = viewImageLearn.SynchronizeWindow(ref viewImagesBoxContour)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				if((res = viewImageLearn.SynchronizeWindow(ref viewImagesContour)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerValidation = viewImageValidation.GetLayer(0);
				CGUIViewImageLayer layerResultLabel = viewImagesBoxContour.GetLayer(0);
				CGUIViewImageLayer layerResultLabelFigure = viewImagesContour.GetLayer(0);
	
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

				if((res = layerResultLabel.DrawTextCanvas(flpPoint, "RESULT BOX CONTOUR", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResultLabelFigure.DrawTextCanvas(flpPoint, "RESULT CONTOUR", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImagesBoxContour.RedrawWindow();
				viewImagesContour.RedrawWindow();
		
				// InstanceSegmentation 객체 생성 // Create InstanceSegmentation object
				CInstanceSegmentationDL instanceSegmentation = new CInstanceSegmentationDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				instanceSegmentation.SetLearningImage(ref fliLearnImage);
				// 검증할 이미지 설정 // Set the image to validate
				instanceSegmentation.SetLearningValidationImage(ref fliValidationImage);
				// 분류할 이미지 설정 // Set the image to classify
				instanceSegmentation.SetInferenceImage(ref fliValidationImage);
				instanceSegmentation.SetInferenceResultImage(ref fliResultBoxContourImage);

				// 학습할 InstanceSegmentation 모델 설정 // Set up the InstanceSegmentation model to learn
				instanceSegmentation.SetModel(CInstanceSegmentationDL.EModel.R_FLSegNet);
				// 학습할 InstanceSegmentation 모델 Version 설정 // Set up the InstanceSegmentation model version to learn
				instanceSegmentation.SetModelVersion(CInstanceSegmentationDL.EModelVersion.R_FLSegNet_V1_256);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				instanceSegmentation.SetLearningEpoch(500);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				instanceSegmentation.SetInterpolationMethod(EInterpolationMethod.Bilinear);
				// 학습 중단 조건 설정 // Set the condtiion of stopping learning
				instanceSegmentation.SetLearningStopCondition("cost <= 0 | validation >= 0.8");
	
				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(.0001f);

				// 설정한 Optimizer를 InstanceSegmentation에 적용 // Apply Optimizer that we set up to InstanceSegmentation
				instanceSegmentation.SetLearningOptimizerSpec(optSpec);
				instanceSegmentation.EnableOptimalLearningStatePreservation(false);

				// AugmentationSpec 설정 // Set the AugmentationSpec
				CAugmentationSpec augSpec = new CAugmentationSpec();

				augSpec.EnableAugmentation(true);
				augSpec.SetCommonActivationRatio(0.700000);
				augSpec.SetCommonIoUThreshold(0.800000);
				augSpec.SetCommonInterpolationMethod(EInterpolationMethod.Bilinear);
				augSpec.EnableHorizontalFlip(true);
				augSpec.EnableVerticalFlip(true);
				augSpec.EnableScale(true);
				augSpec.SetScaleParam(1.000000, 1.000000, 1.000000, 1.000000, true);

				instanceSegmentation.SetLearningAugmentationSpec(augSpec);

				// 학습을 종료할 조건식 설정. map값이 0.9 이상인 경우 학습 종료한다. metric와 동일한 값입니다.
				// Set Conditional Expression to End Learning. If the map value is 0.9 or higher, end the learning. Same value as metric.
				instanceSegmentation.SetLearningStopCondition("map >= 0.9");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				CAutoSaveSpec autoSaveSpec = new CAutoSaveSpec();

				// 자동 저장 활성화 // Enable Auto-Save
				autoSaveSpec.EnableAutoSave(true);
				// 저장할 모델 경로 설정 // Set Model path to save
				autoSaveSpec.SetAutoSavePath("model.flis");
				// 자동 저장 조건식 설정. 현재 map값이 최대 값인 경우 저장 활성화
				// Set auto-save conditional expressions. Enable save if the current map value is the maximum value
				autoSaveSpec.SetAutoSaveCondition("map > max('map')");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				instanceSegmentation.SetLearningAutoSaveSpec(autoSaveSpec);

				// InstanceSegmentation learn function을 진행하는 스레드 생성 // Create the InstanceSegmentation Learn function thread
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if((res = instanceSegmentation.Learn()).IsFail())
						ErrorPrint(res, "Failed to execute Learn.\n");
					
					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!instanceSegmentation.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = instanceSegmentation.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevValidationCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 미니 배치 반복 횟수 받기 // Get the last maximum number of iterations of the last mini batch 
					int i32MiniBatchCount = instanceSegmentation.GetActualMiniBatchCount();
					// 마지막 미니 배치 반복 횟수 받기 // Get the last number of mini batch iterations
					int i32Iteration = instanceSegmentation.GetLearningResultCurrentIteration();
					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = instanceSegmentation.GetLastEpoch();
			
					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Iteration == i32MiniBatchCount && i32Epoch > 0)
					{
						// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
						float f32CurrCost = instanceSegmentation.GetLearningResultLastCost();
						// 마지막 검증 결과 받기 // Get the last validation result
						float f32MeanAP = instanceSegmentation.GetLearningResultLastMeanAP();
		
						// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
						Console.WriteLine("Cost : {0:F6} mAP : {1:F6} Epoch {2} / {3}", f32CurrCost, f32MeanAP, i32Epoch, i32MaxEpoch);

						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> vctCosts = new List<float>();
						List<float> vctMeanAP = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						instanceSegmentation.GetLearningResultAllHistory(out vctCosts, out vctMeanAP, out vctValidationEpoch);

						// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
						if((vctCosts.Count() != 0 && i32PrevCostCount != vctCosts.Count()) || (vctMeanAP.Count() != 0 && i32PrevValidationCount != vctMeanAP.Count()))
						{
							int i32Step = instanceSegmentation.GetLearningValidationStep();
							List<float> flaX = new List<float>();

							for(long i = 0; i < vctMeanAP.Count() - 1; ++i)
								flaX.Add((float)(i * i32Step));

							flaX.Add((float)(vctCosts.Count() - 1));

							// 이전 그래프의 데이터를 삭제 // Clear previous grpah data
							viewGraph.LockUpdate();
							viewGraph.Clear();

							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(vctCosts, EChartType.Line, EColor.RED, "Cost");
							// Graph View 데이터 입력 // Input Graph View Data
							viewGraph.Plot(flaX, vctMeanAP, EChartType.Line, EColor.CYAN, "mAP");
							viewGraph.UnlockUpdate();

							viewGraph.UpdateWindow();
							viewGraph.Invalidate();
							viewGraph.RedrawWindow();
						}

						// 검증 결과가 1.0일 경우 학습을 중단하고 분류 진행 
						// If the validation result is 1.0, stop learning and classify images 
						if(f32MeanAP == 1.0f || bEscape)
							instanceSegmentation.Stop();

						i32PrevEpoch = i32Epoch;
						i32PrevCostCount = vctCosts.Count();
						i32PrevValidationCount = vctMeanAP.Count();
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!instanceSegmentation.IsRunning())
						break;
				}

				// Result Image에 Box & Contour 모두 출력하는 Execute // Execute to print both Box& Contour in Result Image
				// 분류할 이미지 설정 // Set the image to classify
				instanceSegmentation.SetInferenceImage(ref fliValidationImage);
				// 추론 결과 이미지 설정 // Set the inference result Image
				instanceSegmentation.SetInferenceResultImage(ref fliResultBoxContourImage);
				// 추론 결과 옵션 설정 // Set the inference result options
				// Figure 옵션 설정 // Set the option of figures
				CInstanceSegmentationDL.EInferenceResultItemSettings eFigureOption = (CInstanceSegmentationDL.EInferenceResultItemSettings)(CInstanceSegmentationDL.EInferenceResultItemSettings.ClassNum | CInstanceSegmentationDL.EInferenceResultItemSettings.ClassName | CInstanceSegmentationDL.EInferenceResultItemSettings.Objectness | CInstanceSegmentationDL.EInferenceResultItemSettings.BoundaryRect | CInstanceSegmentationDL.EInferenceResultItemSettings.Contour);
				instanceSegmentation.SetInferenceResultItemSettings(eFigureOption);
				// Objectness Threshold 설정 // Set the obectness threshold
				instanceSegmentation.SetInferenceResultObjectnessThreshold(0.5f);
				// Mask Threshold 설정 // Set The mask threshold
				instanceSegmentation.SetInferenceResultMaskThreshold(0.5f);

				// 알고리즘 수행 // Execute the algorithm
				if((res = instanceSegmentation.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// Result Image에 Contour 만 출력하는 Execute // Execute to print both Box& Contour in Result Image
				// 분류할 이미지 설정 // Set the image to classify
				instanceSegmentation.SetInferenceImage(ref fliValidationImage);
				// 추론 결과 이미지 설정 // Set the inference result Image
				instanceSegmentation.SetInferenceResultImage(ref fliResultContourImage);
				// 추론 결과 옵션 설정 // Set the inference result options
				// Figure 옵션 설정 // Set the option of figures
				eFigureOption = (CInstanceSegmentationDL.EInferenceResultItemSettings)(CInstanceSegmentationDL.EInferenceResultItemSettings.Contour);
				instanceSegmentation.SetInferenceResultItemSettings(eFigureOption);
				// Objectness Threshold 설정 // Set the obectness threshold
				instanceSegmentation.SetInferenceResultObjectnessThreshold(0.5f);
				// Mask Threshold 설정 // Set The mask threshold
				instanceSegmentation.SetInferenceResultMaskThreshold(0.5f);

				// 알고리즘 수행 // Execute the algorithm
				if((res = instanceSegmentation.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImagesBoxContour.RedrawWindow();
				viewImagesContour.RedrawWindow();
			
				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewImageValidation.IsAvailable() && viewImagesBoxContour.IsAvailable() && viewImagesContour.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

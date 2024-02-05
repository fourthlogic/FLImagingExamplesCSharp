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

namespace SemanticSegmentation
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
			CFLImage fliResultLabelImage = new CFLImage();
			CFLImage fliResultLabelFigureImage = new CFLImage();

			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageValidation = new CGUIViewImage();
			CGUIViewImage viewImageResultLabel = new CGUIViewImage();
			CGUIViewImage viewImageResultLabelFigure = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;

			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((eResult = fliLearnImage.Load("../../ExampleImages/SemanticSegmentation/Train.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file. \n");
					break;
				}

				if((eResult = fliValidationImage.Load("../../ExampleImages/SemanticSegmentation/Validation.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImageLearn.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view. \n");
					break;
				}

				if((eResult = viewImageValidation.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				if((eResult = viewImageResultLabel.Create(100, 500, 600, 1000)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				if((eResult = viewImageResultLabelFigure.Create(600, 500, 1100, 1000)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((eResult = viewGraph.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(eResult, " Failed to create the graph view. \n");
					break;
				}

				viewGraph.SetDarkMode();

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((eResult = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				if((eResult = viewImageValidation.SetImagePtr(ref fliValidationImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				viewImageResultLabel.EnablePixelSegmentationMode(true);

				if((eResult = viewImageResultLabel.SetImagePtr(ref fliResultLabelImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				fliResultLabelFigureImage.Assign(fliValidationImage);
				fliResultLabelFigureImage.ClearFigures();

				if((eResult = viewImageResultLabelFigure.SetImagePtr(ref fliResultLabelFigureImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				// 다섯 개의 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the four image view windows
				if((eResult = viewImageLearn.SynchronizeWindow(ref viewImageValidation)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				if((eResult = viewImageLearn.SynchronizeWindow(ref viewImageResultLabel)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				if((eResult = viewImageLearn.SynchronizeWindow(ref viewImageResultLabelFigure)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerValidation = viewImageValidation.GetLayer(0);
				CGUIViewImageLayer layerResultLabel = viewImageResultLabel.GetLayer(0);
				CGUIViewImageLayer layerResultLabelFigure = viewImageResultLabelFigure.GetLayer(0);

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

				if((eResult = layerLearn.DrawTextCanvas(flpPoint, "LEARN", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}
				
				if((eResult = layerValidation.DrawTextCanvas(flpPoint, "VALIDATION", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}

				if((eResult = layerResultLabel.DrawTextCanvas(flpPoint, "RESULT LABEL", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}

				if((eResult = layerResultLabelFigure.DrawTextCanvas(flpPoint, "RESULT LABEL", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImageResultLabel.RedrawWindow();
				viewImageResultLabelFigure.RedrawWindow();

				// SemanticSegmentation 객체 생성 // Create SemanticSegmentation object
				CSemanticSegmentationDL semanticSegmentation = new CSemanticSegmentationDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				semanticSegmentation.SetLearningImage(ref fliLearnImage);
				// 검증할 이미지 설정 // Set the image to validate
				semanticSegmentation.SetLearningValidationImage(ref fliValidationImage);
				// 분류할 이미지 설정 // Set the image to classify
				semanticSegmentation.SetSourceImage(ref fliValidationImage);

				// 학습할 SemanticSegmentation 모델 설정 // Set up the SemanticSegmentation model to learn
				semanticSegmentation.SetModel(CSemanticSegmentationDL.EModel.FL_SS_GP);
				// 학습할 SemanticSegmentation 모델 Version 설정 // Set up the SemanticSegmentation model version to learn
				semanticSegmentation.SetModelVersion(CSemanticSegmentationDL.EModelVersion.FL_SS_GP_V1_512);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				semanticSegmentation.SetLearningEpoch(120);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				semanticSegmentation.SetInterpoloationMethod(EInterpolationMethod.Bilinear);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(.0001f);
				// 설정한 Optimizer를 SemanticSegmentation에 적용 // Apply Optimizer that we set up to SemanticSegmentation
				semanticSegmentation.SetLearningOptimizerSpec(optSpec);

				// AugmentationSpec 설정 // Set the AugmentationSpec
				CAugmentationSpec augSpec = new CAugmentationSpec();

				augSpec.SetCommonIOUThreshold(0.5);
				augSpec.SetCommonInterpolationMethod(EInterpolationMethod.Bilinear);
				augSpec.EnableRotation(true);
				augSpec.SetRotationParam(180.0, false);
				augSpec.EnableFlip(true);
				augSpec.EnableGaussianNoise(true);
				semanticSegmentation.SetLearningAugmentationSpec(augSpec);

				// SemanticSegmentation learn function을 진행하는 스레드 생성 // Create the SemanticSegmentation Learn function thread
				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if((eResult = semanticSegmentation.Learn()).IsFail())
						ErrorPrint(eResult, "Failed to execute Learn.\n");
					
					bTerminated = true;
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
						float f32CurrCost;
						semanticSegmentation.GetLearningResultLastCost(out f32CurrCost);
						// 마지막 검증 결과 받기 // Get the last validation result
						float f32ValidationPa = 0;
						float f32ValidationPaMeanIoU = 0;
						semanticSegmentation.GetLearningResultLastPixelAccuracy(out f32ValidationPa);//SemanticSegmentation.GetLastValidationPixelAccuracy(out f32ValidationPa);
						semanticSegmentation.GetLearningResultLastMeanIoU(out f32ValidationPaMeanIoU);//SemanticSegmentation.GetLastValidationMeanIoU(out f32ValidationPaMeanIoU);
																							// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
						Console.WriteLine("Cost : {0:F6} Pixel Accuracy : {1:F6} mIoU : {2:F6} Epoch {3} / {4}", f32CurrCost, f32ValidationPa, f32ValidationPaMeanIoU, i32Epoch, i32MaxEpoch);

						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> vctCosts = new List<float>();
						List<float> vctValidations = new List<float>();
						List<float> vctMeanIoU = new List<float>();
				
						semanticSegmentation.GetLearningResultAllHistory(out vctCosts, out vctValidations, out vctMeanIoU);

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
						if(f32ValidationPa == 1.0f)
							semanticSegmentation.Stop();

						i32PrevEpoch = i32Epoch;
						i32PrevCostCount = vctCosts.Count();
						i32PrevValidationCount = vctValidations.Count();
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!semanticSegmentation.IsRunning())
						break;
				}

				// 알고리즘 수행 // Execute the algorithm
				if((eResult = semanticSegmentation.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute");
					break;
				}

				// ResultLabelImage 받아오기 // Get The ResultLabelImage
				semanticSegmentation.GetInferenceResultImage(out fliResultLabelImage);

				// ResultImage를 ResultFigureImage에 복사 // Copy ResultImage to resultFigureImage
				fliResultLabelFigureImage.Assign(fliResultLabelImage, false);
				// SegmentationRegionExtractor를 이용하여 라벨 이미지를 피겨로 추출 // Extract label image into figure using SegmentationRegionExtractor
				// SegmentationRegionExtractor 객체 생성 // Create the SegmentationRegionExtractor object
				CSegmentationRegionExtractor semanticRE = new CSegmentationRegionExtractor();

				// Blob 파라미터 셋팅 // Set the blob's parameters
				Tuple<int, int> falRange = new Tuple<int, int>((int)1, (int)semanticSegmentation.GetLearningResultClassCount());

				semanticRE.SetResultType(CBlob.EBlobResultType.Contour);
				semanticRE.AddRangesToInclude(falRange);
				semanticRE.SetContourResultType(CBlob.EContourResultType.Perforated);
				semanticRE.SetSourceImage(ref fliResultLabelFigureImage);

				// 결과를 추출하여 이미지에 붙여넣기 // Extract results and paste them into image
				if((eResult = semanticRE.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to process\n");
					break;
				}

				CFLFigureArray flfaResultContours = new CFLFigureArray();

				if((semanticRE.GetResultContours(out flfaResultContours)).IsFail())
				{
					ErrorPrint(eResult, "Failed to process\n");
					break;
				}

				List<Int64> flaLabelList;
				// ResultContours 인덱스와 매칭 되는 라벨 번호배열을 가져오기 // ResultContours Get an array of label numbers matching the index.
				semanticRE.GetResultSegmentationList(out flaLabelList);

				Int64 i64ResultCount = flaLabelList.Count();

				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					List<string> flaNames = new List<string>();
					Int64 i64RealClassNum = flaLabelList[(int)i];
					CFLFigureArray flfaResultContoursCur = (CFLFigureArray)flfaResultContours.GetAt(i);
					semanticSegmentation.GetLearningResultClassNames(i64RealClassNum, out flaNames);

					string flsLabel = string.Format("{0}({1})", i64RealClassNum, flaNames[0]);

					flfaResultContoursCur.SetName(flsLabel);
					fliResultLabelFigureImage.PushBackFigure(CROIUtilities.ConvertFigureObjectToString(flfaResultContoursCur));
					//ResultLabel 이미지의 세그먼테이션 라벨 텍스트 설정 // Set segmentation label text for tthe result label image
					viewImageResultLabel.SetSegmentationLabelText(0, (double)i64RealClassNum, flsLabel);
				}

				// ResultLabl 뷰에 Floating Value Range를 설정
				viewImageResultLabel.SetFloatingImageValueRange(0, (float)semanticSegmentation.GetLearningResultClassCount());

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.RedrawWindow();
				viewImageValidation.RedrawWindow();
				viewImageResultLabel.RedrawWindow();
				viewImageResultLabelFigure.RedrawWindow();
				
				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.RedrawWindow();

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

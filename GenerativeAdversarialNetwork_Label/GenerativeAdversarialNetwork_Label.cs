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

namespace GenerativeAdversarialNetwork
{
	class GenerativeAdversarialNetwork_Label
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
			CFLImage fliValidateIamge = new CFLImage();
			CFLImage fliResultImageOK = new CFLImage();
			CFLImage fliResultImageDamage = new CFLImage();

			/// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageValidate = new CGUIViewImage();
			CGUIViewImage viewImageResultOK = new CGUIViewImage();
			CGUIViewImage viewImageResultDamage = new CGUIViewImage();

			// 그래프 뷰 선언 // Declare the graph view
			CGUIViewGraph viewGraph = new CGUIViewGraph();
			bool bTerminated = false;
			CResult res = new CResult();

			do
			{
				// 라이브러리가 완전히 로드 될 때까지 기다림 // Wait for the library to fully load
				Thread.Sleep(1000);

				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/GenerativeAdversarialNetwork/CircleLabel_Learn.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				if((res = fliValidateIamge.Load("../../ExampleImages/GenerativeAdversarialNetwork/CircleLabel_Validation.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file. \n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearn.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageValidate.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageResultOK.Create(100, 512, 612, 1024)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				if((res = viewImageResultDamage.Create(612, 512, 1124, 1024)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// Graph 뷰 생성 // Create graph view
				if((res = viewGraph.Create(1124, 512, 1636, 1024)).IsFail())
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

				if((res = viewImageValidate.SetImagePtr(ref fliValidateIamge)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageResultOK.SetImagePtr(ref fliResultImageOK)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				if((res = viewImageResultDamage.SetImagePtr(ref fliResultImageDamage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerValidate = viewImageValidate.GetLayer(0);
				CGUIViewImageLayer layerResultOK = viewImageResultOK.GetLayer(0);
				CGUIViewImageLayer layerResultDamage = viewImageResultDamage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerLearn.Clear();
				layerValidate.Clear();
				layerResultOK.Clear();
				layerResultDamage.Clear();

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

				if((res = layerValidate.DrawTextCanvas(flpPoint, "VALIDATE", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResultOK.DrawTextCanvas(flpPoint, "OK", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				if((res = layerResultDamage.DrawTextCanvas(flpPoint, "Damage", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// Generative Adversarial Network 객체 생성 // Create Generative Adversarial Network object
				CGenerativeAdversarialNetworkDL gan = new CGenerativeAdversarialNetworkDL();

				// OptimizerSpec 객체 생성 // Create OptimizerSpec object
				COptimizerSpecAdamGradientDescent optSpec = new COptimizerSpecAdamGradientDescent();

				// 학습할 이미지 설정 // Set the image to learn
				gan.SetLearningImage(ref fliLearnImage);
				// 검증할 이미지 설정 // Set the image to validate
				gan.SetLearningValidationImage(ref fliValidateIamge);
				
				// 학습할 Generative Adversarial Network 모델 설정 // Set up the Generative Adversarial Network model to learn
				gan.SetModel(CGenerativeAdversarialNetworkDL.EModel.FLGenNet_Label);
				// 학습할 Generative Adversarial Network 모델 설정 // Set up the Generative Adversarial Network model to learn
				gan.SetModelVersion(CGenerativeAdversarialNetworkDL.EModelVersion.FLGenNet_Label_V1_64);
				// 학습 epoch 값을 설정 // Set the learn epoch value 
				gan.SetLearningEpoch(500);
				// 학습 이미지 Interpolation 방식 설정 // Set Interpolation method of learn image
				gan.SetInterpolationMethod(EInterpolationMethod.Bilinear);
				// 모델의 최적의 상태를 추적 후 마지막에 최적의 상태로 적용할 지 여부 설정 // Set whether to track the optimal state of the model and apply it as the optimal state at the end.
				gan.EnableOptimalLearningStatePreservation(true);

				// Optimizer의 학습률 설정 // Set learning rate of Optimizer
				optSpec.SetLearningRate(1e-4f);
				// Optimizer의 Weight Decay 설정 // Set weight decay of Optimizer
				optSpec.SetWeightDecay(.0f);
				// Optimizer의 Beta1 설정 // Set Beta1 of Optimizer
				optSpec.SetBeta1(.5f);

				// Gradient Clipping 옵션 적용 // Set the gradient clipping option
				gan.EnableLearningGradientClipping(true);
				gan.SetLearningGradientClippingThreshold(1.0f);
				// 설정한 Optimizer를 GAN에 적용 // Apply the Optimizer that we set up to GAN
				gan.SetLearningOptimizerSpec(optSpec);

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				CAutoSaveSpec autoSaveSpec = new CAutoSaveSpec();

				// 자동 저장 활성화 // Enable Auto-Save
				// 저장 때문에 발생하는 속도 저하를 막기 위해 예제에서는 코드 사용법만 표시하고 옵션은 끔 // To prevent performance degradation caused by saving, the examples only demonstrate how to use the code, with the saving option disabled.
				autoSaveSpec.EnableAutoSave(false);
				// 저장할 모델 경로 설정 // Set Model path to save
				autoSaveSpec.SetAutoSavePath("model.flgan");
				// 자동 저장 조건식 설정. 현재 metric값이 최대 값인 경우 저장 활성화
				// Set auto-save conditional expressions. Enable save if the current metric value is the maximum value
				autoSaveSpec.SetAutoSaveCondition("metric > max('metric')");

				// 자동 저장 옵션 설정 // Set Auto-Save Options
				gan.SetLearningAutoSaveSpec(autoSaveSpec);

				// GenerativeAdversarialNetwork learn function을 진행하는 스레드 생성 // Create the GenerativeAdversarialNetwork Learn function thread
				CResult eLearnResult = new CResult();

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					eLearnResult = gan.Learn();
					bTerminated = true;
				}, null);

				bool bEscape = false;

				ThreadPool.QueueUserWorkItem((arg) =>
				{
					if(Console.ReadKey().Key == ConsoleKey.Escape)
						bEscape = true;
				}, null);

				while(!gan.IsRunning() && !bTerminated)
					Thread.Sleep(1);

				int i32MaxEpoch = gan.GetLearningEpoch();
				int i32PrevEpoch = 0;
				int i32PrevCostCount = 0;
				int i32PrevSSIMCount = 0;

				while(true)
				{
					Thread.Sleep(1);

					// 마지막 미니 배치 최대 반복 횟수 받기 // Get the last maximum number of iterations of the last mini batch 
					int i32MaxIteration = gan.GetActualMiniBatchCount();
					// 마지막 미니 배치 반복 횟수 받기 // Get the last number of mini batch iterations
					int i32Iteration = gan.GetLearningResultCurrentIteration();
					// 마지막 학습 횟수 받기 // Get the last epoch learning
					int i32Epoch = gan.GetLastEpoch();

					// 미니 배치 반복이 완료되면 cost와 validation 값을 디스플레이 
					// Display cost and validation value if iterations of the mini batch is completed 
					if(i32Epoch != i32PrevEpoch && i32Iteration == i32MaxIteration && i32Epoch > 0)
					{
						// 학습 결과 비용과 검증 결과 기록을 받아 그래프 뷰에 출력  
						// Get the history of cost and validation and print it at graph view
						List<float> vctCosts = new List<float>();
						List<float> vctSSIM = new List<float>();
						List<float> vctPDV = new List<float>();
						List<int> vctValidationEpoch = new List<int>();

						gan.GetLearningResultAllHistory(ref vctCosts, ref vctSSIM, ref vctPDV, ref vctValidationEpoch);

						if(vctCosts.Count != 0)
						{
							// 마지막 학습 결과 비용 받기 // Get the last cost of the learning result
							float f32CurrCost = vctCosts.Last();
							// 마지막 SSIM 결과 받기 // Get the last SSIM result
							float f32SSIM = vctSSIM.Count != 0 ? vctSSIM.Last() : 0;
							// 마지막 PDV 결과 받기 // Get the last PDV result
							float f32PDV = vctPDV.Count != 0 ? vctPDV.Last() : 0;

							// 해당 epoch의 비용과 검증 결과 값 출력 // Print cost and validation value for the relevant epoch
							Console.WriteLine("Cost : {0:F6} SSIM : {1:F6} PDV : {2:F6} Epoch {3} / {4}", f32CurrCost, f32SSIM, f32PDV, i32Epoch, i32MaxEpoch);

							// 비용 기록이나 검증 결과 기록이 있다면 출력 // Print results if cost or validation history exists
							if((vctCosts.Count() != 0 && i32PrevCostCount != vctCosts.Count()) || (vctSSIM.Count() != 0 && i32PrevSSIMCount != vctSSIM.Count()))
							{
								viewGraph.LockUpdate();

								// 이전 그래프의 데이터를 삭제 // Clear previous grpah data
								viewGraph.Clear();
								// Graph View 데이터 입력 // Input Graph View Data
								viewGraph.Plot(vctCosts, EChartType.Line, EColor.RED, "Cost");

								int i32Step = gan.GetLearningValidationStep();
								List<float> flaX = new List<float>();

								for(long i = 0; i < vctSSIM.Count() - 1; ++i)
									flaX.Add((float)(i * i32Step));

								flaX.Add((float)(vctCosts.Count() - 1));
								// Graph View 데이터 입력 // Input Graph View Data
								viewGraph.Plot(flaX, vctSSIM, EChartType.Line, EColor.GREEN, "SSIM");

								flaX.Clear();

								for(long i = 0; i < vctPDV.Count() - 1; ++i)
									flaX.Add((float)(i * i32Step));

								flaX.Add((float)(vctCosts.Count() - 1));
								// Graph View 데이터 입력 // Input Graph View Data
								viewGraph.Plot(flaX, vctPDV, EChartType.Line, EColor.BLUE, "PDV");

								viewGraph.UnlockUpdate();
								viewGraph.Invalidate();
							}

							if(bEscape)
								gan.Stop();

							i32PrevEpoch = i32Epoch;
							i32PrevCostCount = vctCosts.Count();
							i32PrevSSIMCount = vctSSIM.Count();
						}
					}

					// epoch만큼 학습이 완료되면 종료 // End when learning progresses as much as epoch
					if(!gan.IsRunning())
						break;
				}

				if(eLearnResult.IsFail())
				{
					ErrorPrint(eLearnResult, "Failed to learn");
					break;
				}

				// 결과 이미지 개수 설정 // Set Result Image Count
				gan.SetInferenceResultCount(10);

				// 생성할 이미지 설정 // Set the image to create
				gan.SetInferenceResultImage(ref fliResultImageOK);

				List<float> liClassWeight = new List<float>();

				liClassWeight.Add(1.0f);
				liClassWeight.Add(0.0f);

				// 클래스별 가중치 설정 // Set Class Weight
				gan.SetInferenceClassWeight(liClassWeight);

				// 알고리즘 수행 // Execute the algorithm
				if((res = gan.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}

				// 생성할 이미지 설정 // Set the image to create
				gan.SetInferenceResultImage(ref fliResultImageDamage);

				liClassWeight.Clear();
				liClassWeight.Add(0.0f);
				liClassWeight.Add(1.0f);

				// 클래스별 가중치 설정 // Set Class Weight
				gan.SetInferenceClassWeight(liClassWeight);

				// 알고리즘 수행 // Execute the algorithm
				if((res = gan.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute");
					break;
				}

				// 결과 이미지를 이미지 뷰에 맞게 조정합니다. // Fit the result image to the image view.
				viewImageResultOK.ZoomFit();
				viewImageResultDamage.ZoomFit();

				// 이미지 뷰를 갱신 // Update the image view.
				viewImageLearn.Invalidate(true);
				viewImageValidate.Invalidate(true);
				viewImageResultOK.Invalidate(true);
				viewImageResultDamage.Invalidate(true);
				// 그래프 뷰를 갱신 // Update the Graph view.
				viewGraph.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewImageResultOK.IsAvailable() && viewImageResultDamage.IsAvailable() && viewImageValidate.IsAvailable() && viewGraph.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

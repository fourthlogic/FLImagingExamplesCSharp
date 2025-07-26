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
	class ConditionalReplacer_Image
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
			CFLImage[] arrFliImage = new CFLImage[4];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[4];

			for(int i = 0; i < 4; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			CResult res = new CResult();

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = arrFliImage[0].Load("../../ExampleImages/ConditionalReplacer/CatSource.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Operand 이미지 로드 // Loads the operand image
				if((res = arrFliImage[1].Load("../../ExampleImages/ConditionalReplacer/CatOperand.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination1 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination1 image as same as source image
				if((res = arrFliImage[2].Assign(arrFliImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// Destination2 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination2 image as same as source image
				if((res = arrFliImage[3].Assign(arrFliImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if((res = arrViewImage[0].Create(100, 0, 548, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Operand 이미지 뷰 생성 // Create operand image view
				if((res = arrViewImage[1].Create(548, 0, 996, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination1 이미지 뷰 생성 // Create destination1 image view
				if((res = arrViewImage[2].Create(996, 0, 1444, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination2 이미지 뷰 생성 // Create destination2 image view
				if((res = arrViewImage[3].Create(1444, 0, 1892, 448)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				bool bError = false;

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				for(int i = 0; i < 4; ++i)
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

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[3])).IsFail())
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

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[2])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Conditional Replacer 객체 생성 // Create Conditional Replacer object
				CConditionalReplacer conditionalReplacer = new CConditionalReplacer();
				// Source 이미지 설정 // Set the source image
				conditionalReplacer.SetSourceImage(ref arrFliImage[0]);
				// Operand 이미지 설정 // Set the operand image
				conditionalReplacer.SetOperandImage(ref arrFliImage[1]);
				// Destination 이미지 설정 // Set the destination image
				conditionalReplacer.SetDestinationImage(ref arrFliImage[2]);

				// Operation Source 설정 // Set the Operation Source
				conditionalReplacer.SetOperationSource(EOperationSource.Image);

				// Threshold Mode 설정 // Set the threshold mode
				conditionalReplacer.SetThresholdMode(EThresholdMode.Single);

				// 논리조건 설정 // Set the logical condition
				conditionalReplacer.SetLogicalCondition(ELogicalCondition.GreaterEqual);

				// 채널 논리조건 설정 // Set the logical condition of channels
				conditionalReplacer.SetLogicalConditionOfChannels(ELogicalConditionOfChannels.And);

				// 조건이 참일 경우 Replacement 값 설정하기 위한 MultiVar 객체 생성 // Create the MultiVar object that sets the Replacement value if the condition is true
				CMultiVar<double> mvReplacement = new CMultiVar<double>(0.0);

				// Replacement 값 설정 // Set Replacement value
				conditionalReplacer.SetReplacementValue(mvReplacement);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = conditionalReplacer.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute conditional replacer.");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				conditionalReplacer.SetDestinationImage(ref arrFliImage[3]);

				// 채널 논리조건 설정 // Set the logical condition of channels
				conditionalReplacer.SetLogicalConditionOfChannels(ELogicalConditionOfChannels.Or);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = conditionalReplacer.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute conditional replacer.");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[4];

				for(int i = 0; i < 4; ++i)
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
				TPoint<double> tpPosition = new TPoint<double>(0, 0);

				if((res = arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[1].DrawTextCanvas(tpPosition, "Operand Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[2].DrawTextCanvas(tpPosition, "Destination1 Image\nThreshold Mode: Single(Greater Equal)\nChannel Condition: And", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[3].DrawTextCanvas(tpPosition, "Destination2 Image\nThreshold Mode: Single(Greater Equal)\nChannel Condition: Or", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				arrViewImage[0].Invalidate(true);
				arrViewImage[1].Invalidate(true);
				arrViewImage[2].Invalidate(true);
				arrViewImage[3].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImage[0].IsAvailable()
					  && arrViewImage[1].IsAvailable()
					  && arrViewImage[2].IsAvailable()
					  && arrViewImage[3].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

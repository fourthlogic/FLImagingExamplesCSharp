﻿using System;
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

using CResult = FLImagingCLR.CResult;

namespace HoleFilling
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

		enum EType
		{
			Source = 0,
			Destination,
			ETypeCount,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrFliImage[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrViewImage[i] = new CGUIViewImage();

			CResult res;

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = arrFliImage[(int)EType.Source].Load("../../ExampleImages/HoleFilling/TodoList.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination1 이미지를 16-bit 이미지로 로드 // Load the 16-bit destination image
				if((res = arrFliImage[(int)EType.Destination].Assign(arrFliImage[(int)EType.Source])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				bool bError = false;

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					// 이미지 뷰 생성 // Create image view
					if((res = (arrViewImage[i].Create(i * 512 + 100, 0, i * 512 + 100 + 512, 512))).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						bError = true;
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
					if((res = (arrViewImage[i].SetImagePtr(ref arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}

					if(i == (int)EType.Source)
						continue;

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if((res = (arrViewImage[(int)EType.Source].SynchronizePointOfView(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
					if((res = (arrViewImage[(int)EType.Source].SynchronizeWindow(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// 알고리즘 객체 생성 // Create Algorithm object
				CHoleFilling alg = new CHoleFilling();

				// Source 이미지 설정 // Set the source image
				if((res = alg.SetSourceImage(ref arrFliImage[(int)EType.Source])).IsFail())
					break;
				// Destination 이미지 설정 // Set the destination image
				if((res = alg.SetDestinationImage(ref arrFliImage[(int)EType.Destination])).IsFail())
					break;
				// 처리할 Hole Area 넓이 범위 설정 // Set hole area range to process
				if((res = alg.SetMinimumHoleArea(10)).IsFail())
					break;
				// 처리할 Hole Area 넓이 범위 설정 // Set hole area range to process
				if((res = alg.SetMaximumHoleArea(99999999999)).IsFail())
					break;
				// 이미지 경계와 맞닿은 hole 의 처리 여부 설정 // Set whether to process holes that touch the image boundary
				if((res = alg.EnableIgnoreBoundaryHole(true)).IsFail())
					break;
				// Threshold 를 통과한 픽셀이 hole 인지 object 인지 설정 // Set whether the pixel that passed the threshold is a hole or an object
				if((res = alg.SetThresholdPassTarget(CHoleFilling.EThresholdPassTarget.Object)).IsFail())
					break;
				// Threshold 수와 결합 방식을 의미하는 Threshold 모드 설정 // Threshold mode setting, which refers to the number of threshold and combination method
				if((res = alg.SetThresholdMode(EThresholdMode.Dual_And)).IsFail())
					break;
				// 각 Threshold 내에서 채널 별 논리 결과 간의 결합 방식을 의미하는 Logical Condition Of Channels 설정 // Set the Logical Condition Of Channels, which refers to the combination method between logical results for each channel within each Threshold
				if((res = alg.SetLogicalConditionOfChannels(ELogicalConditionOfChannels.And)).IsFail())
					break;
				// Hole 영역을 채우는 방식을 설정 // Set the method of filling the hole area
				if((res = alg.SetFillingMethod(CHoleFilling.EFillingMethod.HarmonicInterpolation)).IsFail())
					break;
				// Harmonic Interpolation 의 Precision 값 설정 // Set precision value for Harmonic Interpolation
				if((res = alg.SetPrecision(0.1)).IsFail())
					break;
				// Harmonic Interpolation 의 Max Iteration 값 설정 // Set max iteration value for Harmonic Interpolation
				if((res = alg.SetMaxIteration(100)).IsFail())
					break;
				// 첫 번째 Threshold 의 채널 별 논리 연산자와 값 설정 // Set the logical operator and value for each channel of the first Threshold
				CMultiVar<UInt64> mvThresholdCondition1 = new CMultiVar<UInt64>((UInt64)ELogicalCondition.GreaterEqual, (UInt64)ELogicalCondition.GreaterEqual, (UInt64)ELogicalCondition.GreaterEqual);
				if((res = alg.SetThresholdCondition(EThresholdIndex.First, mvThresholdCondition1)).IsFail())
					break;
				CMultiVar<UInt64> mvThresholdValue1U64 = new CMultiVar<UInt64>(175, 230, 240);
				if((res = alg.SetThresholdValue(EThresholdIndex.First, mvThresholdValue1U64)).IsFail())
					break;
				// 두 번째 Threshold 의 채널 별 논리 연산자와 값 설정 // Set the logical operator and value for each channel of the second Threshold
				CMultiVar<UInt64> mvThresholdCondition2 = new CMultiVar<UInt64>((UInt64)ELogicalCondition.Less, (UInt64)ELogicalCondition.Less, (UInt64)ELogicalCondition.Less);
				if((res = alg.SetThresholdCondition(EThresholdIndex.Second, mvThresholdCondition2)).IsFail())
					break;
				CMultiVar<UInt64> mvThresholdValue2U64 = new CMultiVar<UInt64>(200, 240, 255);
				if((res = alg.SetThresholdValue(EThresholdIndex.Second, mvThresholdValue2U64)).IsFail())
					break;

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (alg.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute HoleFilling.");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.ETypeCount];

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayer[i].Clear();
				}

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpZero = new CFLPoint<double>(0, 0);

				if((res = (arrLayer[(int)EType.Source].DrawTextCanvas(flpZero, "Source Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination].DrawTextCanvas(flpZero, "Destination Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				CFLFigure flfHoleContour = alg.GetSelectedPageFigureObject();

				if((res = (arrLayer[(int)EType.Source].DrawFigureImage(flfHoleContour, EColor.CYAN))).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < (int)EType.ETypeCount; ++i)
					arrViewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bAvailable = true;
				while(bAvailable)
				{
					for(int i = 0; i < (int)EType.ETypeCount; ++i)
					{
						bAvailable = arrViewImage[i].IsAvailable();

						if(!bAvailable)
							break;
					}

					Thread.Sleep(1);
				}
			}
			while(false);
		}
	}
}
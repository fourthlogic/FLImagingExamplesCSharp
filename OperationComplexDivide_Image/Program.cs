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

namespace OperationComplexDivide
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
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliOperandImage = new CFLImage();
			CFLImage fliDestinationImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageOperand = new CGUIViewImage();
			CGUIViewImage viewImageDestination = new CGUIViewImage();

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult eResult;

			do
			{
				// Source 이미지 로드 // Load the source image
				if((eResult = fliSourceImage.Load("\"../../ExampleImages/OperationComplexDivide/ExampleSource.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file. \n");
					break;
				}

				// Operand 이미지 로드 // Loads the operand image
				if((eResult = fliOperandImage.Load("../../ExampleImages/OperationComplexDivide/ExampleOperand.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file. \n");
					break;
				}

				// Destination 이미지를 Source 이미지와 동일하게 설정 // Assign the Source image to Destination image
				if((eResult = fliDestinationImage.Assign(fliSourceImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to assign the image file. \n");
					break;
				}

				// Source 이미지 뷰 생성 // Create source image view
				if((eResult = viewImageSource.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view. \n");
					break;
				}

				// Operand 이미지 뷰 생성 // Create operand image view
				if((eResult = viewImageOperand.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view. \n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if((eResult = viewImageDestination.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view. \n");
					break;
				}

				// Source 이미지 뷰와 Operand 이미지 뷰의 시점을 동기화한다 // Synchronize the viewpoints of the source view and the operand view
				if((eResult = viewImageSource.SynchronizePointOfView(ref viewImageOperand)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view. \n");
					break;
				}

				// Source 이미지 뷰와 Destination 이미지 뷰의 시점을 동기화한다 // Synchronize the viewpoints of the source view and the destination view
				if((eResult = viewImageSource.SynchronizePointOfView(ref viewImageDestination)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view. \n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((eResult = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				// Operand 이미지 뷰에 이미지를 디스플레이
				if((eResult = viewImageOperand.SetImagePtr(ref fliOperandImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((eResult = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view. \n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화한다 // Synchronize the positions of the two image view windows
				if((eResult = viewImageSource.SynchronizeWindow(ref viewImageOperand)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화한다 // Synchronize the positions of the two image view windows
				if((eResult = viewImageSource.SynchronizeWindow(ref viewImageDestination)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window. \n");
					break;
				}

				// Operation Divide 객체 생성 // Create Operation Divide object
				COperationComplexDivide cd = new COperationComplexDivide();

				// Source 이미지 설정 // Set the source image
				cd.SetSourceImage(ref fliSourceImage);

				// Operand 이미지 설정 // Set the operand image
				cd.SetOperandImage(ref fliOperandImage);

				// Destination 이미지 설정 // Set the destination image
				cd.SetDestinationImage(ref fliDestinationImage);

				// 오버플로 처리 방법 설정 // Set the overflow handling method
				cd.SetOverflowMethod(EOverflowMethod.Wrapping);

				// 연산 방식 이미지로 설정 // Set operation source to image
				cd.SetOperationSource(EOperationSource.Image);

				// 공백 색상 칠하기 모드 해제 // Set the Fill blank color mode false
				// 결과 이미지가 이미 존재할 경우 연산되지 않은 영역을 공백 색상으로 칠하지 않고 원본 그대로 둔다. // If the destination image already exists, the uncomputed area is left intact without being painted in a blank color.
				cd.EnableFillBlankColorMode(false);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = cd.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute operation complex divide. \n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerOperand = viewImageOperand.GetLayer(0);
				CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSource.Clear();
				layerOperand.Clear();
				layerDestination.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

				if((eResult = layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text. \n");
					break;
				}

				if((eResult = layerOperand.DrawTextCanvas(flpPoint, "Operand Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text. \n");
					break;
				}

				if((eResult = layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text. \n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewImageSource.Invalidate(true);
				viewImageOperand.Invalidate(true);
				viewImageDestination.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSource.IsAvailable() && viewImageOperand.IsAvailable() && viewImageDestination.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
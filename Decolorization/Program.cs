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

namespace Decolorization
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
			CFLImage fliConvertedImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSource = new CGUIViewImage();
			CGUIViewImage viewImageConverted = new CGUIViewImage();

			do
			{
				CResult eResult;
				// 이미지 로드 // Load image
				if((eResult = fliSourceImage.Load("../../ExampleImages/Decolorization/Pattern.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImageSource.Create(112, 0, 912, 534)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				if((eResult = viewImageConverted.Create(913, 0, 1713, 534)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((eResult = viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				if((eResult = viewImageConverted.SetImagePtr(ref fliConvertedImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((eResult = viewImageSource.SynchronizePointOfView(ref viewImageConverted)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((eResult = viewImageSource.SynchronizeWindow(ref viewImageConverted)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
				CGUIViewImageLayer layerConverted = viewImageConverted.GetLayer(1);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layerSource.Clear();
				layerConverted.Clear();

				CFLPoint<double> resultRegion = new CFLPoint<double>(0, 0);

				// View 정보를 디스플레이 합니다. // Display View information.
				if((eResult = layerSource.DrawTextCanvas(resultRegion, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text");
					break;
				}

				if((eResult = layerConverted.DrawTextCanvas(resultRegion, "Converted Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text");
					break;
				}

				// Decolorization 객체 생성 // Create Decolorization object
				CDecolorization Decolorization = new CDecolorization();

				// Source 이미지 설정 // Set source image 
				if((eResult = Decolorization.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set Source Image.");
					break;
				}

				// Destination 이미지 설정 // Set destination image
				if((eResult = Decolorization.SetDestinationImage(ref fliConvertedImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set Destination Image.");
					break;
				}

				// 연산 속도 설정 // Set the Operation Speed
				if((eResult = (Decolorization.SetOperationSpeed(EOperationSpeed.Normal))).IsFail())
				{
					ErrorPrint(eResult, "Failed to set Operation Speed.");
					break;
				}

				// 알고리즘 수행 // Execute the algorithm
				if((eResult = (Decolorization.Execute())).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute.");
					break;
				}

				// 이미지 뷰의 zoom fit // image view zoom fit
				if((eResult = viewImageConverted.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImageSource.Invalidate(true);
				viewImageConverted.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageConverted.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
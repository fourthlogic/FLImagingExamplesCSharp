﻿using FLImagingCLR;
using FLImagingCLR.AdvancedFunctions;
using FLImagingCLR.Base;
using FLImagingCLR.Devices;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.ThreeDim;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace OpticalCharacterRecognition
{
	internal class Program
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
			do
			{
				CFLImage fliImage = new CFLImage();
				CFLImage fliRecognizeImage = new CFLImage();
				// Declaration of the image view.
				CGUIViewImage viewImage = new CGUIViewImage();
				CGUIViewImage viewImageRecognize = new CGUIViewImage();

				CResult eResult;

				// 이미지 로드 // Load image
				if((eResult = fliImage.Load("../../ExampleImages/OpticalCharacterRecognition/OCR_Learn.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 로드 // Load image
				if((eResult = fliRecognizeImage.Load("../../ExampleImages/OpticalCharacterRecognition/OCR_Recognition.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImage.Create(200, 0, 712, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImageRecognize.Create(712, 0, 1224, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((eResult = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// Converted 이미지 뷰에 이미지를 디스플레이
				if((eResult = viewImageRecognize.SetImagePtr(ref fliRecognizeImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((eResult = viewImage.SynchronizeWindow(ref viewImageRecognize)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}


				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((eResult = viewImage.SynchronizePointOfView(ref viewImageRecognize)).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view\n");
					break;
				}

				CGUIViewImageLayer layer = new CGUIViewImageLayer();
				CGUIViewImageLayer layerRecognize = new CGUIViewImageLayer();

				layer = viewImage.GetLayer(0);
				layerRecognize = viewImageRecognize.GetLayer(1);


				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();
				layerRecognize.Clear();

				if((eResult = layer.DrawTextCanvas(new CFLPoint<double>(0, 0), "Learn", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text");
					break;
				}

				if((eResult = layerRecognize.DrawTextCanvas(new CFLPoint<double>(0, 0), "Recognition", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text");
					break;
				}

				COCR ocr = new COCR();

				// 문자를 학습할 이미지 설정
				if((eResult = ocr.SetLearnImage(ref fliImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set Source Image.");
					break;
				}

				// 이미지에서 학습할 문자의 색상 및 배경 색상을 설정
				/*if(IsFail(ocr.SetCharacterColor(COCR::ECharacterColorType_BlackOnWhite)))
				{
					printf("Failed to set character color type.");
					break;
				}*/

				// 학습할 이미지에 저장되어있는 Figure 학습
				if((eResult = ocr.Learn()).IsFail())
				{
					ErrorPrint(eResult, "Failed to train.");
					break;
				}

				// 문자를 인식할 이미지 설정
				if((eResult = ocr.SetSourceImage(ref fliRecognizeImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set Source Image.");
					break;
				}

				// 인식할 각도의 범위를 설정
				if((eResult = ocr.SetAngleTolerance(0)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set noise filter size.");
					break;
				}

				// 인식할 이미지에서 문자의 Threshold 값을 입력 받지 않고 자동으로 찾는 모드를 설정
				if((eResult = ocr.EnableAutoScale(true)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set threshold auto.");
					break;
				}

				// 인식할 최소 점수를 설정
				if((eResult = ocr.SetMinimumScore(0.7)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set noise filter size.");
					break;
				}

				// 인식할 이미지에서 문자를 찾는 기능을 수행
				if((eResult = ocr.Execute()).IsFail())
				{
					ErrorPrint(eResult, eResult.GetString());
					break;
				}

				// 찾은 문자의 개수를 받아오는 함수
				Int64 i64ResultCount = ocr.GetResultCount();

				// 찾은 문자의 정보를 받아올 컨테이너
				COCR.COCRRecognitionCharacterInfo resultChar;

				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					ocr.GetResultRecognizedCharactersInfo(i, out resultChar);

					string flsResultString = "";
					string flsResultName = resultChar.flfaCharacter.GetName();

					int i32Score = (int)(resultChar.f64Score * 100.0);

					flsResultString = "[" + flsResultName + "]" + string.Format("Score : {0}%\n", i32Score);
					Console.WriteLine(flsResultString);
					CFLRect<double> flrBoundary = new CFLRect<double>();

					resultChar.flfaCharacter.GetBoundaryRect(out flrBoundary);

					if((eResult = layerRecognize.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(eResult, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((eResult = layerRecognize.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(eResult, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}
				}


				viewImage.Invalidate();
				viewImageRecognize.Invalidate();

				// The image view is waiting until close.
				while(viewImage.IsAvailable() && viewImageRecognize.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

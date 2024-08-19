using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.AdvancedFunctions;
using FLImagingCLR.Base;
using FLImagingCLR.Devices;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.ThreeDim;

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
				CFLImage fliRecognizeImage = new CFLImage();
				CFLImage fliRecognizeImageUnicode = new CFLImage();
				// Declaration of the image view.
				CGUIViewImage viewImageRecognize = new CGUIViewImage();
				CGUIViewImage viewImageRecognizeUnicode = new CGUIViewImage();

				CResult res;

				// 이미지 로드 // Load image
				if((res = fliRecognizeImage.Load("../../ExampleImages/OpticalCharacterRecognition/OCR_Recognition.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 로드 // Load image
				if((res = fliRecognizeImageUnicode.Load("../../ExampleImages/OpticalCharacterRecognition/OCR_Recognition_Unicode2.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageRecognize.Create(200, 0, 712, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageRecognizeUnicode.Create(712, 0, 1224, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImageRecognize.SetImagePtr(fliRecognizeImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Converted 이미지 뷰에 이미지를 디스플레이
				if((res = viewImageRecognizeUnicode.SetImagePtr(fliRecognizeImageUnicode)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = viewImageRecognize.SynchronizeWindow(viewImageRecognizeUnicode)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageRecognize.SynchronizePointOfView(viewImageRecognizeUnicode)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				CGUIViewImageLayer layerRecognize = new CGUIViewImageLayer();
				CGUIViewImageLayer layerRecognizeUnicode = new CGUIViewImageLayer();

				layerRecognize = viewImageRecognize.GetLayer(0);
				layerRecognizeUnicode = viewImageRecognizeUnicode.GetLayer(1);


				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layerRecognize
				layerRecognize.Clear();
				layerRecognizeUnicode.Clear();

				if((res = layerRecognize.DrawTextCanvas(new CFLPoint<double>(0, 0), "Recognition1", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				if((res = layerRecognizeUnicode.DrawTextCanvas(new CFLPoint<double>(0, 0), "Recognition2", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				COCR ocr = new COCR();

				// 학습 정보 파일을 로드
				if((res = ocr.Load("../../ExampleImages/OpticalCharacterRecognition/OCR_FourthLogic.flocr")).IsFail())
				{
					ErrorPrint(res, "Failed to load learnt file.");
					break;
				}

				// 문자를 인식할 이미지 설정
				if((res = ocr.SetSourceImage(ref fliRecognizeImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 인식할 이미지에서 문자를 찾는 기능을 수행
				if((res = ocr.Execute()).IsFail())
				{
					ErrorPrint(res, res.GetString());
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
					double f64Scale = resultChar.f64ScaleWidth * resultChar.f64ScaleHeight;
					CFLRect<double> flrBoundary = new CFLRect<double>();

					flsResultString = "[" + flsResultName + "]" + string.Format("Score: {0}%\nScale: {1}\nRotation: {2}", i32Score, f64Scale.ToString("n2"), resultChar.f64Rotation);
					Console.WriteLine(flsResultString);
					resultChar.flfaCharacter.GetBoundaryRect(out flrBoundary);

					if((res = layerRecognize.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layerRecognize.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}
				}

				// 문자를 인식할 이미지 설정
				if((res = ocr.SetSourceImage(ref fliRecognizeImageUnicode)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 인식할 이미지에서 문자를 찾는 기능을 수행
				if((res = ocr.Execute()).IsFail())
				{
					ErrorPrint(res, res.GetString());
					break;
				}

				// 찾은 문자의 개수를 받아오는 함수
				i64ResultCount = ocr.GetResultCount();

				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					ocr.GetResultRecognizedCharactersInfo(i, out resultChar);

					string flsResultString = "";
					string flsResultName = resultChar.flfaCharacter.GetName();
					int i32Score = (int)(resultChar.f64Score * 100.0);
					double f64Scale = resultChar.f64ScaleWidth * resultChar.f64ScaleHeight;
					CFLRect<double> flrBoundary = new CFLRect<double>();

					flsResultString = "[" + flsResultName + "]" + string.Format("Score: {0}%\nScale: {1}\nRotation: {2}", i32Score, f64Scale.ToString("n2"), resultChar.f64Rotation);
					Console.WriteLine(flsResultString);
					resultChar.flfaCharacter.GetBoundaryRect(out flrBoundary);

					if((res = layerRecognizeUnicode.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layerRecognizeUnicode.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}
				}

				viewImageRecognize.Invalidate();
				viewImageRecognizeUnicode.Invalidate();

				// The image view is waiting until close.
				while(viewImageRecognize.IsAvailable() && viewImageRecognizeUnicode.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

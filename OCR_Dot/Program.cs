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

namespace OCR
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
				// Declaration of the image view.
				CGUIViewImage viewImage = new CGUIViewImage();

				CResult res;

				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/OCR/Dot_Print_Number.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(200, 0, 712, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				CGUIViewImageLayer layer = new CGUIViewImageLayer();

				layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				if((res = layer.DrawTextCanvas(new CFLPoint<double>(0, 0), "Learn & Recognition", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				COCR ocr = new COCR();

				// 문자를 학습할 이미지 설정
				if((res = ocr.SetLearnImage(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 학습할 이미지에 저장되어있는 Figure 학습
				if((res = ocr.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to train.");
					break;
				}

				CFLFigureArray flfaLearned = new CFLFigureArray();

				// 학습한 문자의 모양를 받아오는 함수
				ocr.GetLearnedCharacter(out flfaLearned);

				Int64 i64LearnedCount = flfaLearned.GetCount();

				for(Int64 i = 0; i < i64LearnedCount; ++i)
				{
					CFLFigure flfLearned = new CFLFigureArray(flfaLearned.GetAt(i));
					string flsResultString = flfLearned.GetName();
					CFLRect<double> flrBoundary = new CFLRect<double>();

					flrBoundary = flfLearned.GetBoundaryRect();

					if((res = layer.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layer.DrawFigureImage(flfLearned, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}
				}

				// 문자를 인식할 이미지 설정
				if((res = ocr.SetSourceImage(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 인식할 문자의 각도 범위를 설정
				if((res = ocr.SetRecognizingAngleTolerance(6.0)).IsFail())
				{
					ErrorPrint(res, "Failed to set angle tolerance.");
					break;
				}

				// 오토 스케일 여부를 설정
				if((res = ocr.EnableRecognizingAutoScale(false)).IsFail())
				{
					ErrorPrint(res, "Failed to set auto scale.");
					break;
				}

				// 인식할 문자의 최소 직사각 너비를 설정
				if((res = ocr.SetRecognizingMinimumRectangleWidth(20.0)).IsFail())
				{
					ErrorPrint(res, "Failed to set minimum rectangle width.");
					break;
				}

				// 인식할 문자의 최소 직사각 높이를 설정
				if((res = ocr.SetRecognizingMinimumRectangleHeight(40.0)).IsFail())
				{
					ErrorPrint(res, "Failed to set minimum rectangle height.");
					break;
				}

				// 인식할 문자의 최대 직사각 너비를 설정
				if((res = ocr.SetRecognizingMaximumRectangleWidth(40.0)).IsFail())
				{
					ErrorPrint(res, "Failed to set maximum rectangle width.");
					break;
				}

				// 인식할 문자의 최대 직사각 높이를 설정
				if((res = ocr.SetRecognizingMaximumRectangleHeight(60.0)).IsFail())
				{
					ErrorPrint(res, "Failed to set maximum rectangle height.");
					break;
				}

				// 인식할 최소 점수를 설정
				if((res = ocr.SetRecognizingMinimumScore(0.5)).IsFail())
				{
					ErrorPrint(res, "Failed to set minimum score.");
					break;
				}

				// 인식할 최대 개수를 설정
				if((res = ocr.SetRecognizingMaximumCharacterCount(100)).IsFail())
				{
					ErrorPrint(res, "Failed to set maximum character count.");
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

					flsResultString = "[" + flsResultName + "]" + string.Format("Score: {0}%\nScale: {1}\nRotation: {2}", i32Score, f64Scale.ToString("n2"), resultChar.f64Rotation);
					Console.WriteLine(flsResultString);
					CFLRect<double> flrBoundary = new CFLRect<double>();

					resultChar.flfaCharacter.GetBoundaryRect(out flrBoundary);

					if((res = layer.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layer.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}
				}

				viewImage.Invalidate();

				// The image view is waiting until close.
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

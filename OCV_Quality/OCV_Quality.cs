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

namespace OCV
{
	internal class OCV_Quality
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
				CFLImage fliImage1 = new CFLImage();
				CFLImage fliImage2 = new CFLImage();
				CFLImage fliImage3 = new CFLImage();
				
				CGUIViewImage viewImage1 = new CGUIViewImage();
				CGUIViewImage viewImage2 = new CGUIViewImage();
				CGUIViewImage viewImage3 = new CGUIViewImage();

				CResult res;

				// 이미지 로드 // Load image
				if((res = fliImage1.Load("../../ExampleImages/OCV/A.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliImage2.Load("../../ExampleImages/OCV/A_Demaged1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliImage3.Load("../../ExampleImages/OCV/A_Demaged2.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage1.Create(200, 0, 712, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage2.Create(712, 0, 1224, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage3.Create(1224, 0, 1736, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImage1.SetImagePtr(ref fliImage1)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage2.SetImagePtr(ref fliImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImage3.SetImagePtr(ref fliImage3)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = viewImage1.SynchronizeWindow(ref viewImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				if((res = viewImage1.SynchronizeWindow(ref viewImage3)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImage1.SynchronizePointOfView(ref viewImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				if((res = viewImage1.SynchronizePointOfView(ref viewImage3)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				CGUIViewImageLayer layer1 = new CGUIViewImageLayer();
				CGUIViewImageLayer layer2 = new CGUIViewImageLayer();
				CGUIViewImageLayer layer3 = new CGUIViewImageLayer();

				layer1 = viewImage1.GetLayer(0);
				layer2 = viewImage2.GetLayer(1);
				layer3 = viewImage3.GetLayer(2);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer1.Clear();
				layer2.Clear();
				layer3.Clear();

				COCV ocv = new COCV();

				// OCR Font 파일을 로드
				if((res = ocv.LoadFontData("../../ExampleImages/OCV/A.flocr")).IsFail())
				{
					ErrorPrint(res, "Failed to load Font file.");
					break;
				}

				// 문자를 검증할 이미지 설정
				if((res = ocv.SetSourceImage(ref fliImage1)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 이미지에서 문자를 검증하는 기능을 수행
				if((res = ocv.Execute()).IsFail())
				{
					ErrorPrint(res, res.GetString());
					break;
				}

				if((res = layer1.DrawTextCanvas(new CFLPoint<double>(0, 0), ocv.GetResultVerificationState() == COCV.EVerificationState.OK ? "Verify" : "Fail", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 찾은 문자의 개수를 받아오는 함수
				Int64 i64ResultCount = ocv.GetResultCount();

				// 찾은 문자의 정보를 받아올 컨테이너
				COCV.COCVVerificationCharacterInfo resultChar = new COCV.COCVVerificationCharacterInfo();

				Console.WriteLine("Image 1\n");

				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					ocv.GetResultVerificationCharactersInfo(i, ref resultChar);

					string flsResultString = "";
					string flsResultString2 = "";
					string flsResultName = resultChar.flfaCharacter.GetName();
					int i32Quality = (int)(resultChar.f64Quality * 100.0);
					CFLRect<double> flrBoundary = resultChar.flrBoundary;
					CFLLine<double> fllBlankSpaceWidth = resultChar.fllBlankSpaceWidthLine;

					flsResultString = "[" + flsResultName + "]" + string.Format("Quality: {0}%\nScale: {1}\nAngle: {2}\nLighting: {3}\nContrast: {4}", i32Quality, (resultChar.f64ScaleWidth * resultChar.f64ScaleHeight).ToString("n2"), resultChar.f64Rotation.ToString("n2"), resultChar.f64Lighting.ToString("n2"), resultChar.f64Contrast.ToString("n2"));
					flsResultString2 = string.Format("Space Width: {0}", resultChar.f64BlankSpaceWidth.ToString("n2"));
					Console.WriteLine(flsResultString);
					Console.WriteLine(flsResultString2);
					Console.WriteLine();

					if((res = layer1.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layer1.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					if((res = layer1.DrawFigureImage(flrBoundary, resultChar.bVerified ? EColor.GREEN : EColor.RED, 3, resultChar.bVerified ? EColor.GREEN : EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, 0.0f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					layer1.DrawFigureImage(resultChar.flfaIntrusion, EColor.YELLOW, 1, EColor.YELLOW, EGUIViewImagePenStyle.Solid, 1.0f, 0.3f);
					layer1.DrawFigureImage(resultChar.flfaExtrusion, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1.0f, 0.3f);

					if((res = layer1.DrawFigureImage(fllBlankSpaceWidth, EColor.BLACK, 3, EColor.BLACK, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					if((res = layer1.DrawTextImage(new CFLPointArray(fllBlankSpaceWidth).GetAt(0), flsResultString2, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}
				}

				// 문자를 검증할 이미지 설정
				if((res = ocv.SetSourceImage(ref fliImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 이미지에서 문자를 검증하는 기능을 수행
				if((res = ocv.Execute()).IsFail())
				{
					ErrorPrint(res, res.GetString());
					break;
				}

				if((res = layer2.DrawTextCanvas(new CFLPoint<double>(0, 0), ocv.GetResultVerificationState() == COCV.EVerificationState.OK ? "Verify" : "Fail", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 찾은 문자의 개수를 받아오는 함수
				i64ResultCount = ocv.GetResultCount();
				Console.WriteLine("Image 2\n");

				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					ocv.GetResultVerificationCharactersInfo(i, ref resultChar);

					string flsResultString = "";
					string flsResultString2 = "";
					string flsResultName = resultChar.flfaCharacter.GetName();
					int i32Quality = (int)(resultChar.f64Quality * 100.0);
					CFLRect<double> flrBoundary = resultChar.flrBoundary;
					CFLLine<double> fllBlankSpaceWidth = resultChar.fllBlankSpaceWidthLine;

					flsResultString = "[" + flsResultName + "]" + string.Format("Quality: {0}%\nScale: {1}\nAngle: {2}\nLighting: {3}\nContrast: {4}", i32Quality, (resultChar.f64ScaleWidth * resultChar.f64ScaleHeight).ToString("n2"), resultChar.f64Rotation.ToString("n2"), resultChar.f64Lighting.ToString("n2"), resultChar.f64Contrast.ToString("n2"));
					flsResultString2 = string.Format("Space Width: {0}", resultChar.f64BlankSpaceWidth.ToString("n2"));
					Console.WriteLine(flsResultString);
					Console.WriteLine(flsResultString2);
					Console.WriteLine();

					if((res = layer2.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layer2.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					if((res = layer2.DrawFigureImage(flrBoundary, resultChar.bVerified ? EColor.GREEN : EColor.RED, 3, resultChar.bVerified ? EColor.GREEN : EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, 0.0f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					layer2.DrawFigureImage(resultChar.flfaIntrusion, EColor.YELLOW, 1, EColor.YELLOW, EGUIViewImagePenStyle.Solid, 1.0f, 0.3f);
					layer2.DrawFigureImage(resultChar.flfaExtrusion, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1.0f, 0.3f);

					if((res = layer2.DrawFigureImage(fllBlankSpaceWidth, EColor.BLACK, 3, EColor.BLACK, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					if((res = layer2.DrawTextImage(new CFLPointArray(fllBlankSpaceWidth).GetAt(0), flsResultString2, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}
				}

				// 문자를 검증할 이미지 설정
				if((res = ocv.SetSourceImage(ref fliImage3)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 이미지에서 문자를 검증하는 기능을 수행
				if((res = ocv.Execute()).IsFail())
				{
					ErrorPrint(res, res.GetString());
					break;
				}

				if((res = layer3.DrawTextCanvas(new CFLPoint<double>(0, 0), ocv.GetResultVerificationState() == COCV.EVerificationState.OK ? "Verify" : "Fail", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text");
					break;
				}

				// 찾은 문자의 개수를 받아오는 함수
				i64ResultCount = ocv.GetResultCount();
				Console.WriteLine("Image 3\n");

				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					ocv.GetResultVerificationCharactersInfo(i, ref resultChar);

					string flsResultString = "";
					string flsResultString2 = "";
					string flsResultName = resultChar.flfaCharacter.GetName();
					int i32Quality = (int)(resultChar.f64Quality * 100.0);
					CFLRect<double> flrBoundary = resultChar.flrBoundary;
					CFLLine<double> fllBlankSpaceWidth = resultChar.fllBlankSpaceWidthLine;

					flsResultString = "[" + flsResultName + "]" + string.Format("Quality: {0}%\nScale: {1}\nAngle: {2}\nLighting: {3}\nContrast: {4}", i32Quality, (resultChar.f64ScaleWidth * resultChar.f64ScaleHeight).ToString("n2"), resultChar.f64Rotation.ToString("n2"), resultChar.f64Lighting.ToString("n2"), resultChar.f64Contrast.ToString("n2"));
					flsResultString2 = string.Format("Space Width: {0}", resultChar.f64BlankSpaceWidth.ToString("n2"));
					Console.WriteLine(flsResultString);
					Console.WriteLine(flsResultString2);
					Console.WriteLine();

					if((res = layer3.DrawTextImage(new CFLPoint<double>(flrBoundary.left, flrBoundary.top), flsResultString, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}

					if((res = layer3.DrawFigureImage(resultChar.flfaCharacter, EColor.LIME, 1, EColor.LIME, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					if((res = layer3.DrawFigureImage(flrBoundary, resultChar.bVerified ? EColor.GREEN : EColor.RED, 3, resultChar.bVerified ? EColor.GREEN : EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, 0.0f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					layer3.DrawFigureImage(resultChar.flfaIntrusion, EColor.YELLOW, 1, EColor.YELLOW, EGUIViewImagePenStyle.Solid, 1.0f, 0.3f);
					layer3.DrawFigureImage(resultChar.flfaExtrusion, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1.0f, 0.3f);

					if((res = layer3.DrawFigureImage(fllBlankSpaceWidth, EColor.BLACK, 3, EColor.BLACK, EGUIViewImagePenStyle.Solid, 1.0f, 0.35f)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}", i));
						break;
					}

					if((res = layer3.DrawTextImage(new CFLPointArray(fllBlankSpaceWidth).GetAt(0), flsResultString2, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, string.Format("Failed to draw recognized character : {0}\n", i));
						break;
					}
				}

				viewImage1.Invalidate();
				viewImage2.Invalidate();
				viewImage3.Invalidate();

				// The image view is waiting until close.
				while(viewImage1.IsAvailable() && viewImage2.IsAvailable() && viewImage3.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

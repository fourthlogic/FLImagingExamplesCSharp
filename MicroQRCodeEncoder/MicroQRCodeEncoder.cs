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

namespace FLImagingExamplesCSharp
{
	class MicroQRCodeEncoder
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
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();

			do
			{
				CResult res;

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 1424, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// MicroQR Code Encoder 객체 생성 // Create MicroQR Code Encoder object
				CMicroQRCodeEncoder qrCodeEncoder = new CMicroQRCodeEncoder();
				CMicroQRCodeSpec codeSpec = new CMicroQRCodeSpec();

				// 처리할 이미지 설정 // Set the image to process
				qrCodeEncoder.SetSourceImage(ref fliImage);

				// Encoding Message 설정
				Console.WriteLine("Please input encoding message.: ");
				string strInput = Console.ReadLine();

				qrCodeEncoder.SetEncodingMessage(strInput);

				// 인코딩 이미지 포멧
				codeSpec.SetImageFormat(EPixelFormat.C1_U8);

				// Encoding 스펙 설정
				// 설정하지 않을 시에는 기본 값으로 동작한다
				qrCodeEncoder.SetMicroQRCodeEncodingSpec(codeSpec);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = qrCodeEncoder.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute MicroQR Code Encoder.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// MicroQRCode 객체 생성 // Create MicroQRCode object
				CMicroQRCodeDecoder qrCodeDecoder = new CMicroQRCodeDecoder();

				// 처리할 이미지 설정 // Set the image to process
				qrCodeDecoder.SetSourceImage(ref fliImage);

				// Decode 데이터 영역 색상 설정
				qrCodeDecoder.SetColorMode(EDataCodeColor.BlackOnWhite);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = qrCodeDecoder.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute MicroQRCode decoder.");
					break;
				}

				// MicroQR Code Decoder 결과 개수를 얻는다.
				Int64 i64Results = qrCodeDecoder.GetResultCount();

				// 디코딩 결과값을 각각 확인하는 코드
				for(Int64 i = 0; i < i64Results; ++i)
				{
					// MicroQR Code Decoder 결과를 얻어오기 위해 FLQuadD 선언
					CFLQuad<double> flqRegion = new CFLQuad<double>();

					// MicroQR Code Decoder 결과들 중 Data Region 을 얻어옴
					if((res = qrCodeDecoder.GetResultDataRegion(i, ref flqRegion)).IsFail())
					{
						ErrorPrint(res, "Failed to get data region from the MicroQR Code decoder object.");
						continue;
					}

					// MicroQR Code 의 영역을 디스플레이 한다.
					if((res = layer.DrawFigureImage(flqRegion, EColor.LIME, 2)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure object on the image view.\n");
						continue;
					}

					// MicroQR Code Decoder 결과를 얻어오기 위해 FigureArray 선언
					CFLFigureArray flfaGridRegion = new CFLFigureArray();

					// MicroQR Code Decoder 결과들 중 Grid Region 을 얻어옴
					if((res = qrCodeDecoder.GetResultGridRegion(i, ref flfaGridRegion)).IsFail())
					{
						ErrorPrint(res, "Failed to get grid region from the MicroQR Code decoder object.");
						continue;
					}

					// MicroQR Code 의 Grid Region 을 디스플레이 한다.
					if((res = layer.DrawFigureImage(flfaGridRegion, EColor.LIME, 2)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
						continue;
					}

					// MicroQR Code Decoder 결과를 얻어오기 위해 FLStringW 선언
					StringBuilder strDecoded = new StringBuilder();

					// MicroQR Code Decoder 결과들 중 Decoded String 을 얻어옴
					if((res = qrCodeDecoder.GetResultDecodedString(i, ref strDecoded)).IsFail())
					{
						ErrorPrint(res, "Failed to get decoded string from the MicroQR Code decoder object.");
						continue;
					}

					CMicroQRCodeSpec codeSpecResult = new CMicroQRCodeSpec();
					qrCodeDecoder.GetResultMicroQRCodeSpec(i, ref codeSpecResult);

					EMicroQRCodeErrorCorrectionLevel eECLevel = codeSpecResult.GetMicroQRCodeErrorCorrectionLevel();
					EMicroQRCodeSymbolType eSymbol = codeSpec.GetSymbolType();

					string strAdditionalData = "";

					switch(eECLevel)
					{
					case EMicroQRCodeErrorCorrectionLevel.None:
						strAdditionalData = "[None";
						break;
					case EMicroQRCodeErrorCorrectionLevel.Low:
						strAdditionalData = "[Low";
						break;
					case EMicroQRCodeErrorCorrectionLevel.Medium:
						strAdditionalData = "[Medium";
						break;
					case EMicroQRCodeErrorCorrectionLevel.Quartile:
						strAdditionalData = "[Quartile";
						break;
					default:
						break;
					}

					switch(eSymbol)
					{
					case EMicroQRCodeSymbolType.MicroQRVersion1:
						strAdditionalData += "-1]";
						break;
					case EMicroQRCodeSymbolType.MicroQRVersion2:
						strAdditionalData += "-2]";
						break;
					case EMicroQRCodeSymbolType.MicroQRVersion3:
						strAdditionalData += "-3]";
						break;
					case EMicroQRCodeSymbolType.MicroQRVersion4:
						strAdditionalData += "-4]";
						break;
					default:
						break;
					}

					Console.Write("No. {0} Code : {1} {2}\n", i, strAdditionalData, strDecoded);

					// String 을 디스플레이 하기 위한 기준 좌표 FLPointL 선언
					CFLPoint<double> flplPos = new CFLPoint<double>(flqRegion.flpPoints[3]);

					// Decoded String 을 디스플레이 한다.
					// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
					// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
					// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
					//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
					// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
					//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
					if((res = layer.DrawTextImage(flqRegion.flpPoints[0], strAdditionalData, EColor.YELLOW, EColor.BLACK, 12, false, flqRegion.flpPoints[3].GetAngle(flqRegion.flpPoints[2]), EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, "Failed to draw string object on the image view.");
						continue;
					}

					if((res = layer.DrawTextImage(flqRegion.flpPoints[3], strDecoded.ToString(), EColor.CYAN, EColor.BLACK, 12, false, flqRegion.flpPoints[3].GetAngle(flqRegion.flpPoints[2]))).IsFail())
					{
						ErrorPrint(res, "Failed to draw string object on the image view.");
						continue;
					}
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

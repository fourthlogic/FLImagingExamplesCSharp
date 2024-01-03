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

using CResult = FLImagingCLR.CResult;

namespace QRCode
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
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();

			do
			{
				CResult eResult;

				// 이미지 로드 // Load image
				if((eResult = fliImage.Load("../../ExampleImages/QRCode/Plate.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImage.Create(400, 0, 1424, 768)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((eResult = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((eResult = viewImage.ZoomFit()).IsFail())
				{
					ErrorPrint(eResult, "Failed to zoom fit\n");
					break;
				}

				// QR Code 객체 생성 // Create QR Code object
				CQRCodeDecoder qrCodeDecoder = new CQRCodeDecoder();

				// 처리할 이미지 설정 // Set the image to process
				qrCodeDecoder.SetSourceImage(ref fliImage);

				// ROI 범위 설정
				CFLRect<Int32> flrROI = new CFLRect<Int32>(210, 60, 400, 250);

				// 처리할 ROI 설정
				qrCodeDecoder.SetSourceROI(flrROI);

				// Decode 데이터 영역 색상 설정
				// EQRCodeColors.Auto 로 설정 시 자동으로 Decode 된다.
				qrCodeDecoder.SetColorMode(EDataCodeColor.WhiteOnBlack);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = qrCodeDecoder.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute QR Code decoder.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				if((eResult = layer.DrawFigureImage(flrROI, EColor.BLUE)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw figures objects on the image view.\n");
					break;
				}

				// QR Code Decoder 결과 개수를 얻는다.
				Int64 i64Results = qrCodeDecoder.GetResultCount();

				// 디코딩 결과값을 각각 확인하는 코드
				for(Int64 i = 0; i < i64Results; ++i)
				{
					// QR Code Decoder 결과를 얻어오기 위해 FLQuadD 선언
					CFLQuad<double> flqRegion;

					// QR Code Decoder 결과들 중 Data Region 을 얻어옴
					if((eResult = qrCodeDecoder.GetResultDataRegion(i, out flqRegion)).IsFail())
					{
						ErrorPrint(eResult, "Failed to get data region from the QR Code decoder object.");
						continue;
					}

					// QR Code 의 영역을 디스플레이 한다.
					if((eResult = layer.DrawFigureImage(flqRegion, EColor.LIME, 2)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw figure object on the image view.\n");
						continue;
					}

					// QR Code Decoder 결과를 얻어오기 위해 FigureArray 선언
					CFLFigureArray flfaGridRegion;

					// QR Code Decoder 결과들 중 Grid Region 을 얻어옴
					if((eResult = qrCodeDecoder.GetResultGridRegion(i, out flfaGridRegion)).IsFail())
					{
						ErrorPrint(eResult, "Failed to get grid region from the QR Code decoder object.");
						continue;
					}

					// QR Code 의 Grid Region 을 디스플레이 한다.
					if((eResult = layer.DrawFigureImage(flfaGridRegion, EColor.LIME, 2)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw figure objects on the image view.\n");
						continue;
					}

					// QR Code Decoder 결과를 얻어오기 위해 FLStringW 선언
					string strDecoded = "";

					// QR Code Decoder 결과들 중 Decoded String 을 얻어옴
					if((eResult = qrCodeDecoder.GetResultDecodedString(i, out strDecoded)).IsFail())
					{
						ErrorPrint(eResult, "Failed to get decoded string from the QR Code decoder object.");
						continue;
					}

					CQRCodeSpec codeSpec = new CQRCodeSpec();
					qrCodeDecoder.GetResultQRCodeSpec(i, out codeSpec);

					EQRCodeErrorCorrectionLevel eECLevel = codeSpec.GetQRCodeErrorCorrectionLevel();
					EQRCodeSymbolVersion1 eSymbol1 = EQRCodeSymbolVersion1.None;
					EQRCodeSymbolVersion2 eSymbol2 = EQRCodeSymbolVersion2.None;
					codeSpec.GetQRCodeSymbolVersion(out eSymbol1, out eSymbol2);

					string strAdditionalData = "";

					switch(eECLevel)
					{
					case EQRCodeErrorCorrectionLevel.Low:
						strAdditionalData = "[Low";
						break;
					case EQRCodeErrorCorrectionLevel.Medium:
						strAdditionalData = "[Medium";
						break;
					case EQRCodeErrorCorrectionLevel.Quartile:
						strAdditionalData = "[Quartile";
						break;
					case EQRCodeErrorCorrectionLevel.High:
						strAdditionalData = "[High";
						break;
					default:
						break;
					}

					if(eSymbol1 != EQRCodeSymbolVersion1.None)
					{
						int i32SymbolValue = (int)eSymbol1;
						int i32Symbol = 0;

						for(int j = 0; j < 20; ++j)
						{
							if(((i32SymbolValue >> j) & 1) == 1)
							{
								i32Symbol = j + 1;
								break;
							}
						}
												
						strAdditionalData += String.Format("-{0}]", i32Symbol);
					}

					if(eSymbol2 != EQRCodeSymbolVersion2.None)
					{
						int i32SymbolValue = (int)eSymbol1;
						int i32Symbol = 0;

						for(int j = 0; j < 20; ++j)
						{
							if(((i32SymbolValue >> j) & 1) == 1)
							{
								i32Symbol = j + 21;
								break;
							}
						}

						strAdditionalData += String.Format("-{0}]", i32Symbol);
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
					if((eResult = layer.DrawTextImage(flqRegion.flpPoints[0], strAdditionalData, EColor.YELLOW, EColor.BLACK, 15, false, flqRegion.flpPoints[0].GetAngle(flqRegion.flpPoints[1]), EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw string object on the image view.");
						continue;
					}

					if((eResult = layer.DrawTextImage(flqRegion.flpPoints[3], strDecoded, EColor.CYAN, EColor.BLACK, 20, false, flqRegion.flpPoints[3].GetAngle(flqRegion.flpPoints[2]))).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw string object on the image view.");
						continue;
					}
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

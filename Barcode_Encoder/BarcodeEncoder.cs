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
	class BarcodeEncoder
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

				// 입력 받을 문자열 선언
				Console.WriteLine("Please input encoding message.: ");
				string strInput = Console.ReadLine();

				// Barcode Encoder 객체 생성 // Create Barcode Encoder object
				CBarcodeEncoder barcode = new CBarcodeEncoder();
				CBarcodeSpec codeSpec = new CBarcodeSpec();

				// 처리할 이미지 설정 // Set the image to process
				barcode.SetSourceImage(ref fliImage);
				// Encoding Barcode 타입 설정
				codeSpec.SetSymbolType(EBarcodeSymbolType.EAN128);
				// Encoding Message 설정
				barcode.SetEncodingMessage(strInput);

				// 설정된 Barcode 스펙
				barcode.SetBarcodeEncodingSpec(codeSpec);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = barcode.Execute()).IsFail())
					ErrorPrint(res, "Failed to execute Barcode encoder.");

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

				// Barcode Decoder 객체 생성 // Create Barcode Decoder object
				CBarcodeDecoder sBarcodeDecoder = new CBarcodeDecoder();

				// 처리할 이미지 설정 // Set the image to process
				sBarcodeDecoder.SetSourceImage(ref fliImage);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sBarcodeDecoder.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute barcode decoder.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// 검출된 총 바코드 개수
				Int64 i64Results = sBarcodeDecoder.GetResultCount();

				// 바코드 정보 출력
				for(Int32 i = 0; i < i64Results; i++)
				{
					// Barcode Decoder 결과를 얻어오기 위해 FLQuadD 선언
					CFLQuad<double> flqRegion = new CFLQuad<double>();

					// Barcode Decoder 결과들 중 Data Region 을 얻어옴
					if((res = sBarcodeDecoder.GetResultDataRegion(i, ref flqRegion)).IsFail())
					{
						ErrorPrint(res, "Failed to get data region from the barcode decoder object.");
						continue;
					}

					// Barcode 의 영역을 디스플레이 한다.
					// FLImaging의 Figure 객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능 // FLimaging's Figure objects can be displayed as a function regardless of the shape
					// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
					// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
					// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
					if((res = layer.DrawFigureImage(flqRegion, EColor.LIME, 2, 0, EGUIViewImagePenStyle.Solid, 1, 0)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure object on the image view.\n");
						continue;
					}

					StringBuilder strDecodedMsg = new StringBuilder();

					// Barcode Decoder 결과들 중 Decoded String 을 얻어옴
					if((res = sBarcodeDecoder.GetResultDecodedString(i, ref strDecodedMsg)).IsFail())
					{
						ErrorPrint(res, "Failed to get decoded string from the barcode decoder object.");
						continue;
					}

					Console.Write("No. {0} Code : {1}\n", i, strDecodedMsg.ToString());

					// String 을 디스플레이 하기 위한 기준 좌표 FLPointL 선언
					CFLPoint<Int32> flplPos = new CFLPoint<Int32>();
					flqRegion.GetCenter(ref flplPos);

					// Decoded String 을 디스플레이 한다.
					// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
					// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
					// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
					//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
					// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
					//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
					if(layer.DrawTextImage(flqRegion.flpPoints[3], strDecodedMsg.ToString(), EColor.CYAN, EColor.BLACK, 20).IsFail())
					{
						ErrorPrint(res, "Failed to draw string object on the image view.\n");
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

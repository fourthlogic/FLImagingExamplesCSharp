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

namespace FLImagingExamplesCSharp
{
	class ModelBasedDataMatrixDecoder
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}
		public enum EViewList
		{
			Learn,
			ModelBaseFind,
			NormalFind,

			Count
		};

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// Declaration of the image view 
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EViewList.Count];

			CFLImage[] arrFliImage = new CFLImage[(int)EViewList.Count];
			CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EViewList.Count];

			for(int i = 0; i < (int)EViewList.Count; ++i)
			{
				arrViewImage[i] = new CGUIViewImage();
				arrFliImage[i] = new CFLImage();
				arrLayer[i] = new CGUIViewImageLayer();
			}

			string[] arrWcsViewText = new string[(int)EViewList.Count];

			arrWcsViewText[(int)EViewList.Learn] = "Learn View";
			arrWcsViewText[(int)EViewList.ModelBaseFind] = "Model Based Decoder Result View";
			arrWcsViewText[(int)EViewList.NormalFind] = "Decoder Result View";

			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Loads image
				if((res = arrFliImage[(int)EViewList.Learn].Load("../../ExampleImages/DataMatrix/Learn.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = arrFliImage[(int)EViewList.ModelBaseFind].Load("../../ExampleImages/DataMatrix/Find.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = arrFliImage[(int)EViewList.NormalFind].Load("../../ExampleImages/DataMatrix/Find.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Create the image view 
				const int i32ViewSize = 450;
				const int i32Start = 100;

				for(int i = 0; i < (int)EViewList.Count; ++i)
				{
					int i32X = i32ViewSize * i;
					int i32Y = i32Start;

					if((res = arrViewImage[i].Create(i32X + i32Start, i32Y, i32X + i32ViewSize + i32Start, i32Y + i32ViewSize)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display the image in the imageview
					if((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}

					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					if((res = arrLayer[i].DrawTextCanvas(new CFLPoint<double>(0, 0), arrWcsViewText[i], EColor.YELLOW, EColor.BLUE, 20)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}
				}

				// 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the all image views. 
				if((res = arrViewImage[(int)EViewList.ModelBaseFind].SynchronizePointOfView(ref arrViewImage[(int)EViewList.NormalFind])).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Model Based Data Matrix Decoder 객체 생성 // Create Model Based Data Matrix Decoder object
				CModelBasedDataMatrixDecoder modelBasedDataMatrixDecoder = new CModelBasedDataMatrixDecoder();

				// 학습 이미지 설정 // Sets the learn image.
				modelBasedDataMatrixDecoder.SetLearnImage(ref arrFliImage[(int)EViewList.Learn]);
				// 코드 색상 설정 // Sets the code color.
				modelBasedDataMatrixDecoder.SetColorMode(EDataCodeColor.WhiteOnBlack);

				// 학습 동작 // Learn
				if((res = modelBasedDataMatrixDecoder.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to learn data matrix decoder.");
					break;
				}

				// 동작 이미지 설정 // Set source image
				modelBasedDataMatrixDecoder.SetSourceImage(ref arrFliImage[(int)EViewList.ModelBaseFind]);
				// 디코딩 결과 개수 설정 // Sets the number of decoding results.
				modelBasedDataMatrixDecoder.SetDetectingCount(EDataCodeDecoderDetectingCount.All);
				modelBasedDataMatrixDecoder.SetMaximumDetectingCount(3);

				// 학습 이미지 기준 탐색 각도 설정 // Sets the search angle relative to the learn data.
				modelBasedDataMatrixDecoder.SetAngleTolerance(30);

				// 동작 // Execute
				if((res = modelBasedDataMatrixDecoder.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute data matrix decoder.");
					break;
				}


				// Learn 동작 결과를 얻어온다 // Gets the result of the learn result.
				CModelBasedDataMatrixDecoder.CDataMatrixLearnInformation datamatrixLearnInfo = new CModelBasedDataMatrixDecoder.CDataMatrixLearnInformation();
				modelBasedDataMatrixDecoder.GetLearnResult(datamatrixLearnInfo);
				{
					Console.Write("\n[Model Based Learn Result]\n");

					CFLQuad<double> flqLearnedCodeRegion = datamatrixLearnInfo.decodedDataMatrixInformation.pFlqRegion;
					string flsLearnedCode = datamatrixLearnInfo.decodedDataMatrixInformation.pStrCode;

					// Learn 동작 결과 영역 및 코드 출력 // Outputs the regions and codes from the learn operation results.
					arrLayer[(int)EViewList.Learn].DrawFigureImage(flqLearnedCodeRegion, EColor.LIME, 2);
					arrLayer[(int)EViewList.Learn].DrawTextImage(flqLearnedCodeRegion.flpPoints[3], flsLearnedCode, EColor.CYAN, EColor.BLACK, 20, false, flqLearnedCodeRegion.flpPoints[3].GetAngle(flqLearnedCodeRegion.flpPoints[2]));

					Console.Write("Code Size : {0} x {1}\n", datamatrixLearnInfo.i32Rows, datamatrixLearnInfo.i32Cols);

					string flsCodeColor = "";

					if(datamatrixLearnInfo.eColor == EDataCodeColor.BlackOnWhite)
						flsCodeColor = "Black On White";
					else if(datamatrixLearnInfo.eColor == EDataCodeColor.WhiteOnBlack)
						flsCodeColor = "White On Black";

					Console.Write("Code Color : {0}\n", flsCodeColor);

					string flsFlip = datamatrixLearnInfo.bFlip ? "Yes" : "No";

					Console.Write("Flip : {0}\n", flsFlip);
					Console.Write("Code : {0}\n", flsLearnedCode);
				}

				// Data Matrix Decoder 결과 개수를 얻는다. // Gets the number of results from the Data Matrix decoder.
				long i64Results = modelBasedDataMatrixDecoder.GetResultCount();

				Console.Write("\n[Model Based Decoded Result]\n");
				for(long i = 0; i < i64Results; ++i)
				{
					CFLQuad<double> flqDecodedCodeRegion = new CFLQuad<double>();
					StringBuilder flsDecodedCode = new StringBuilder();

					// Data Matrix Decoder 결과들 중 Data Region 을 얻어옴 // Gets the Data Region from the results of the Data Matrix decoder.
					if((res = modelBasedDataMatrixDecoder.GetResultDataRegion(i, ref flqDecodedCodeRegion)).IsFail())
					{
						ErrorPrint(res, "Failed to get data region from the data matrix decoder object.");
						continue;
					}

					// Data Matrix Decoder 결과들 중 Decoded String 을 얻어옴 // Gets the decoded string from the results of the Data Matrix decoder.
					if((res = modelBasedDataMatrixDecoder.GetResultDecodedString(i, ref flsDecodedCode)).IsFail())
					{
						ErrorPrint(res, "Failed to get decoded string from the data matrix decoder object.");
						continue;
					}

					Console.Write("No. {0} Code : {1}\n", i, flsDecodedCode);

					arrLayer[(int)EViewList.ModelBaseFind].DrawFigureImage(flqDecodedCodeRegion, EColor.LIME, 2);
					arrLayer[(int)EViewList.ModelBaseFind].DrawTextImage(flqDecodedCodeRegion.flpPoints[3], flsDecodedCode.ToString(), EColor.CYAN, EColor.BLACK, 16, false, flqDecodedCodeRegion.flpPoints[3].GetAngle(flqDecodedCodeRegion.flpPoints[2]));
				}

				// 일반 Data Matrix Decoder 결과와 비교하기 위한 동작 // Operation for comparing with standard Data Matrix decoder results.

				// Data Matrix Decoder 객체 생성 // Create Data Matrix Decoder object
				CDataMatrixDecoder datamatrixDecoder = new CDataMatrixDecoder();

				// 동작 이미지 설정 // Set source image
				datamatrixDecoder.SetSourceImage(ref arrFliImage[(int)EViewList.NormalFind]);
				// 코드 색상 설정 // Sets the code color.
				datamatrixDecoder.SetColorMode(EDataCodeColor.WhiteOnBlack);
				// 디코딩 결과 개수 설정 // Sets the number of decoding results.
				datamatrixDecoder.SetDetectingCount(EDataCodeDecoderDetectingCount.All);
				datamatrixDecoder.SetMaximumDetectingCount(3);

				// 동작 // Execute
				if((res = datamatrixDecoder.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute data matrix decoder.");
					break;
				}

				// Data Matrix Decoder 결과 개수를 얻는다. // Gets the number of results from the Data Matrix decoder.
				i64Results = datamatrixDecoder.GetResultCount();

				Console.Write("\n[Normal Decoded Result]\n");
				for(long i = 0; i < i64Results; ++i)
				{
					CFLQuad<double> flqDecodedCodeRegion = new CFLQuad<double>();
					StringBuilder flsDecodedCode = new StringBuilder();

					// Data Matrix Decoder 결과들 중 Data Region 을 얻어옴 // Gets the Data Region from the results of the Data Matrix decoder.
					if((res = datamatrixDecoder.GetResultDataRegion(i, ref flqDecodedCodeRegion)).IsFail())
					{
						ErrorPrint(res, "Failed to get data region from the data matrix decoder object.");
						continue;
					}

					// Data Matrix Decoder 결과들 중 Decoded String 을 얻어옴 // Gets the decoded string from the results of the Data Matrix decoder.
					if((res = datamatrixDecoder.GetResultDecodedString(i, ref flsDecodedCode)).IsFail())
					{
						ErrorPrint(res, "Failed to get decoded string from the data matrix decoder object.");
						continue;
					}

					Console.Write("No. {0} Code : {1}\n", i, flsDecodedCode);

					arrLayer[(int)EViewList.NormalFind].DrawFigureImage(flqDecodedCodeRegion, EColor.LIME, 2);
					arrLayer[(int)EViewList.NormalFind].DrawTextImage(flqDecodedCodeRegion.flpPoints[3], flsDecodedCode.ToString(), EColor.CYAN, EColor.BLACK, 16, false, flqDecodedCodeRegion.flpPoints[3].GetAngle(flqDecodedCodeRegion.flpPoints[2]));
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < (int)EViewList.Count; ++i)
					arrViewImage[i].Invalidate();

				// The image view is waiting until close.
				while(arrViewImage[(int)EViewList.Learn].IsAvailable() && arrViewImage[(int)EViewList.ModelBaseFind].IsAvailable() && arrViewImage[(int)EViewList.NormalFind].IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

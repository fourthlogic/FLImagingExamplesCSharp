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
	class OrientedFASTandRotatedBRIEF
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

			CResult res;

			do
			{
				// 이미지 로드 // Load image
                if ((res = fliImage.Load("../../ExampleImages/OrientedFASTandRotatedBRIEF/Chip.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 1168, 540)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// 객체 생성
				COrientedFASTandRotatedBRIEF orientedFASTandRotatedBRIEF = new COrientedFASTandRotatedBRIEF();

				// ROI 범위 설정
				CFLRect<int> flrROI = new CFLRect<int>(100, 50, 450, 450);

				// 처리할 이미지 설정 // Set the image to process
                if ((res = orientedFASTandRotatedBRIEF.SetSourceImage(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source Image.");
					break;
				}

				// 처리할 ROI 설정
                if ((res = (orientedFASTandRotatedBRIEF.SetSourceROI(flrROI))).IsFail())
				{
					ErrorPrint(res, "Failed to set Source ROI.");
					break;
				}

				// 특징점을 추출할 NLevels 수를 설정
				if((res = (orientedFASTandRotatedBRIEF.SetNLevels(4))).IsFail())
				{
					ErrorPrint(res, "Failed to set NLevels.");
					break;
				}

				// 특징점을 추출할 Nfeature 수를 설정
				if((res = (orientedFASTandRotatedBRIEF.SetNfeature(500))).IsFail())
				{
					ErrorPrint(res, "Failed to set Nfeature.");
					break;
				}

				// 특징점을 추출할 ScoreType 설정
				if((res = (orientedFASTandRotatedBRIEF.SetScoreType(COrientedFASTandRotatedBRIEF.EScoreType.FastScore))).IsFail())
				{
					ErrorPrint(res, "Failed to set Score Type.");
					break;
				}

				// 추출할 특징점의 FAST Threshold 설정
				if((res = (orientedFASTandRotatedBRIEF.SetFASTThreshold(10))).IsFail())
				{
					ErrorPrint(res, "Failed to set FAST Threshold.");
					break;
                }

				// ORB의 파라미터 Scale Factor 설정
				if((res = (orientedFASTandRotatedBRIEF.SetScaleFactor(1.2f))).IsFail())
				{
					ErrorPrint(res, "Failed to set Scale Factor.");
					break;
                }

				// ORB 실행 함수
                if ((res = (orientedFASTandRotatedBRIEF.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				// 실행 결과를 받아오기 위한 컨테이너
				CFLPointArray flfaResultPoints = new CFLPointArray();

				// 검출된 점을 가져오는 함수
                if ((res = (orientedFASTandRotatedBRIEF.GetResultPoints(ref flfaResultPoints))).IsFail())
				{
					ErrorPrint(res, "Failed to get result.");
					break;
				}

				// 검출된 점을 출력
				layer.DrawFigureImage(flfaResultPoints, EColor.RED, 1);

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				if((res = (layer.DrawFigureImage(flrROI, EColor.BLUE))).IsFail())
				{
					ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
					break;
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
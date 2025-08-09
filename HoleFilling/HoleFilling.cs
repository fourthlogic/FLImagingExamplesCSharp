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
	class HoleFilling
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			CResult res;

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = fliSrcImage.Load("../../ExampleImages/HoleFilling/TodoList.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image views
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view. \n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the images in the image views
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}


				// 알고리즘 객체 생성 // Create algorithm object
				CHoleFilling holeFilling = new CHoleFilling();

				if((res = holeFilling.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				if((res = holeFilling.SetDestinationImage(ref fliDstImage)).IsFail())
					break;
				if((res = holeFilling.SetMinimumHoleArea(10)).IsFail())
					break;
				if((res = holeFilling.SetMaximumHoleArea(99999999999)).IsFail())
					break;
				if((res = holeFilling.EnableIgnoreBoundaryHole(true)).IsFail())
					break;
				if((res = holeFilling.SetThresholdPassTarget(CHoleFilling.EThresholdPassTarget.Object)).IsFail())
					break;
				if((res = holeFilling.SetThresholdMode(EThresholdMode.Dual_And)).IsFail())
					break;
				if((res = holeFilling.SetLogicalConditionOfChannels(ELogicalConditionOfChannels.And)).IsFail())
					break;
				if((res = holeFilling.SetFillingMethod(CHoleFilling.EFillingMethod.HarmonicInterpolation)).IsFail())
					break;
				if((res = holeFilling.SetPrecision(0.1)).IsFail())
					break;
				if((res = holeFilling.SetMaxIteration(100)).IsFail())
					break;
				CMultiVar<UInt64> mvThresholdCondition1 = new CMultiVar<UInt64>((UInt64)ELogicalCondition.GreaterEqual, (UInt64)ELogicalCondition.GreaterEqual, (UInt64)ELogicalCondition.GreaterEqual);
				if((res = holeFilling.SetThresholdCondition(EThresholdIndex.First, mvThresholdCondition1)).IsFail())
					break;
				CMultiVar<UInt64> mvThresholdValue1U64 = new CMultiVar<UInt64>(175, 230, 240);
				if((res = holeFilling.SetThresholdValue(EThresholdIndex.First, mvThresholdValue1U64)).IsFail())
					break;
				CMultiVar<UInt64> mvThresholdCondition2 = new CMultiVar<UInt64>((UInt64)ELogicalCondition.Less, (UInt64)ELogicalCondition.Less, (UInt64)ELogicalCondition.Less);
				if((res = holeFilling.SetThresholdCondition(EThresholdIndex.Second, mvThresholdCondition2)).IsFail())
					break;
				CMultiVar<UInt64> mvThresholdValue2U64 = new CMultiVar<UInt64>(200, 240, 255);
				if((res = holeFilling.SetThresholdValue(EThresholdIndex.Second, mvThresholdValue2U64)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = holeFilling.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}


				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);
				if((res = layerSrc.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text. \n");
					break;
				}

				CFLFigure flfHoleContour = holeFilling.GetSelectedPageFigureObject();
				if((res = (layerSrc.DrawFigureImage(flfHoleContour, EColor.CYAN))).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure.\n");
					break;
				}

				// Zoom Fit
				viewImageSrc.ZoomFit();
				viewImageDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
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
	class ImageGridSplitter
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		public enum EType
		{
			Src = 0,
			Dst,
			Count,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.Count];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.Count];

			for(int i = 0; i < (int)EType.Count; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = arrFliImage[(int)EType.Src].Load("../../ExampleImages/Crop/bacteria.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				for(int i = 0; i < (int)EType.Count; ++i)
				{
					int i32X = i % 2;
					int i32Y = i / 2;

					// 이미지 뷰 생성 // Create image view
					if((res = arrViewImage[i].Create(i32X * 400 + 400, i32Y * 400, i32X * 400 + 400 + 400, i32Y * 400 + 400)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
					if((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}
				}

				// Image Grid Splitter  객체 생성 // Create Image Grid Splitter object
				CImageGridSplitter imageGridSplitter = new CImageGridSplitter();

				// Source 이미지 설정 // Set source image 
				imageGridSplitter.SetSourceImage(ref arrFliImage[(int)EType.Src]);

				// 이미지 분할 방향을 설정 // Set image split direction
				imageGridSplitter.SetSplitDirection(CImageGridSplitter.ESplitDirection.LeftTopToRight);

				// 이미지 분할 크기 설정 // Set iamge split size;
				imageGridSplitter.SetSplitSize(128, 128);

				// Destination 이미지 설정 // Set destination image 
				imageGridSplitter.SetDestinationImage(ref arrFliImage[(int)EType.Dst]);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (imageGridSplitter.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute ImageGridSplitter.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CFLPoint<double> flpTmp = new CFLPoint<double>(0, 0);
				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.Count];

				for(int i = 0; i < (int)EType.Count; ++i)
					arrLayer[i] = arrViewImage[i].GetLayer(0);

				// Text 출력 // Display Text 
				if((res = arrLayer[(int)EType.Src].DrawTextImage(flpTmp, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				if((res = arrLayer[(int)EType.Dst].DrawTextImage(flpTmp, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// Image 분할 영역 계산 // Calculate image split regions.
				CFLRect<double> flrSource = new CFLRect<double>(arrFliImage[(int)EType.Src]);
				CFLFigureArray flgaGrid = new CFLFigureArray();

				flrSource.Split(4, 4, ref flgaGrid);

				// Image 분할 영역을 원본 이미지 레이어에 디스플레이 // Display grid of split Image to image layer.
				if((res = arrLayer[(int)EType.Src].DrawFigureImage(flgaGrid, EColor.RED)).IsFail())
					ErrorPrint(res, "Failed to draw figure\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				for(int i = (int)EType.Count - 1; i >= 0; --i)
				{
					arrViewImage[i].ZoomFit();
					arrViewImage[i].Invalidate(true);
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bRun = true;
				while(bRun)
				{
					for(int i = 0; i < (int)EType.Count; ++i)					
						bRun &= arrViewImage[i].IsAvailable();					

					Thread.Sleep(1);
				}
			}
			while(false);


		}
	}
}

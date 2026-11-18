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
	class FloodFill
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}
		enum EType
		{
			Source = 0,
			Destination1,
			Destination2,
			COUNT,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.COUNT];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] viewImage = new CGUIViewImage[(int)EType.COUNT];

			for(int i = 0; i < (int)EType.COUNT; ++i)
			{
				arrFliImage[i] = new CFLImage();
				viewImage[i] = new CGUIViewImage();
			}

			do
			{
				CResult res;

				// 이미지 로드 // Load image
				if((res = arrFliImage[(int)EType.Source].Load("../../ExampleImages/FloodFill/Ball.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				for(int i = 0; i < (int)EType.COUNT; ++i)
				{
					if((res = viewImage[i].Create(300 + 520 * i, 0, 300 + 520 * (i + 1), 430)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// 이미지 뷰들의 시점을 동기화 한다 // Synchronize the viewpoints of the image views.
				for(int i = (int)EType.Destination1; i < (int)EType.COUNT; ++i)
				{
					if((res = viewImage[(int)EType.Source].SynchronizePointOfView(ref viewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// 이미지 뷰 윈도우들의 위치를 동기화 한다 // Synchronize the positions of the image view windows
				for(int i = (int)EType.Destination1; i < (int)EType.COUNT; ++i)
				{
					if((res = viewImage[(int)EType.Source].SynchronizeWindow(ref viewImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				for(int i = 0; i < (int)EType.COUNT; ++i)
				{
					if((res = viewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// Flood Fill 객체 생성 // Create Flood Fill object
				CFloodFill floodFill = new CFloodFill();

				// Source 이미지 설정 // Set source image
				floodFill.SetSourceImage(ref arrFliImage[(int)EType.Source]);

				// 시작 지점 설정 // Set seed point
				CFLPoint<double> flpSeedPoint = new CFLPoint<double>(9, 9);
				floodFill.SetSeedPoint(flpSeedPoint);

				// 픽셀 값 차이의 상한 설정 // Set upper bound of pixel value differences
				floodFill.SetUpperDifference(new CMultiVar<double>(10));

				// 픽셀 값 차이의 하한 설정 // Set lower bound of pixel value differences
				floodFill.SetLowerDifference(new CMultiVar<double>(10));

				// 연결 영역 출력값 설정 // Set output value of connected region
				floodFill.SetRangeValue(new CMultiVar<double>(255));

				// 연결 영역이 아닌 영역의 출력값 설정 // Set output value of out of connected region
				floodFill.SetOutOfRangeValue(new CMultiVar<double>(0));

				// Floating Range 결과 이미지 설정 // Set destination image for Floating Range
				floodFill.SetDestinationImage(ref arrFliImage[(int)EType.Destination1]);

				// 기준 픽셀 지정 방식을 Floating Range로 설정 // Set reference pixel specification method to Floating Range
				floodFill.SetRangeMode(CFloodFill.ERangeMode.FloatingRange);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (floodFill.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Flood Fill (Floating Range).");
					break;
				}

				// Fixed Range 결과 이미지 설정 // Set destination image for Fixed Range
				floodFill.SetDestinationImage(ref arrFliImage[(int)EType.Destination2]);

				// 기준 픽셀 지정 방식을 Fixed Range로 설정 // Set reference pixel specification method to Fixed Range
				floodFill.SetRangeMode(CFloodFill.ERangeMode.FixedRange);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (floodFill.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Flood Fill (Fixed Range).");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.COUNT];

				for(int i = 0; i < (int)EType.COUNT; ++i)
					arrLayer[i] = viewImage[i].GetLayer(0);

				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// 시작 지점을 디스플레이 합니다. // Display the seed point.
				if((res = arrLayer[(int)EType.Source].DrawFigureImage(flpSeedPoint, EColor.RED, 3)).IsFail())
					ErrorPrint(res, "Failed to draw figure.\n");

				// View 정보를 디스플레이 합니다. // Display View information.
				string[] arrViewText = new string[(int)EType.COUNT];
				arrViewText[(int)EType.Source] = "Source Image";
				arrViewText[(int)EType.Destination1] = "Floating Range";
				arrViewText[(int)EType.Destination2] = "Fixed Range";

				for(int i = 0; i < (int)EType.COUNT; ++i)
				{
					if((res = arrLayer[i].DrawTextImage(flpTemp, arrViewText[i], EColor.YELLOW, EColor.BLACK, 20)).IsFail())
						ErrorPrint(res, "Failed to draw text.\n");
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				for(int i = 0; i < (int)EType.COUNT; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bAvailable = true;

				while(bAvailable)
				{
					for(int i = 0; i < (int)EType.COUNT; ++i)
						bAvailable &= viewImage[i].IsAvailable();

					Thread.Sleep(1);
				}

				for(int i = 0; i < (int)EType.COUNT; ++i)
					viewImage[i].Destroy();
			}
			while(false);
		}
	}
}

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
	class ImageConcatenatorGrid
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
				if((res = arrFliImage[(int)EType.Src].Load("../../ExampleImages/ImageConcatenatorGrid/mnist0.flif")).IsFail())
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

				// ImageConcatenatorGrid  객체 생성 // Create ImageConcatenatorGrid object
				CImageConcatenatorGrid imageConcatenatorGrid = new CImageConcatenatorGrid();

				// Source 이미지 설정 // Set source image 
				imageConcatenatorGrid.SetSourceImage(ref arrFliImage[(int)EType.Src]);

				// Destination 이미지 설정 // Set destination image 
				imageConcatenatorGrid.SetDestinationImage(ref arrFliImage[(int)EType.Dst]);

				// Page scope 설정 // Set page scope
				imageConcatenatorGrid.SetPageScope(CImageConcatenatorGrid.EPageScope.Source_AllPage_Operand_None);

				// Tile Size 설정 // Set Tile Size
				imageConcatenatorGrid.SetTileSize(3, 3);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (imageConcatenatorGrid.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute ImageConcatenatorGrid.");
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

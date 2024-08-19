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

namespace Demoire
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
			CFLImage[] arrFliImage = new CFLImage[2];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[2];

			for(int i = 0; i < 2; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = (arrFliImage[0].Load("../../ExampleImages/Demoire/Demoire.flif"))).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지를 Copy // image copy
				if((res = (arrFliImage[1].Assign(arrFliImage[0]))).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = (arrViewImage[0].Create(100, 0, 612, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = (arrViewImage[1].Create(612, 0, 1124, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				bool bError = false;

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				for(int i = 0; i < 2; ++i)
				{
					if((res = (arrViewImage[i].SetImagePtr(arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = (arrViewImage[0].SynchronizePointOfView(arrViewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = (arrViewImage[0].SynchronizeWindow(arrViewImage[1]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// Demoire 객체 생성 // Create Demoire object
				CDemoire Demoire = new CDemoire();
				// Source 이미지 설정 // Set source image
				Demoire.SetSourceImage(ref arrFliImage[0]);
				// Destination 이미지 설정 // Set destination image 
				Demoire.SetDestinationImage(ref arrFliImage[1]);

				// 알고리즘 수행 // Execute the algorithm
                if ((res = (Demoire.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute operation add.");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[2];
				arrLayer[0] = new CGUIViewImageLayer();
				arrLayer[1] = new CGUIViewImageLayer();

				for(int i = 0; i < 2; ++i)
				{
					// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
					// 따로 해제할 필요 없음 // No need to release separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
					arrLayer[i].Clear();
				}

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				TPoint<double> tpPosition = new TPoint<double>(0, 0);

				if((res = (arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[1].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				arrViewImage[0].Invalidate(true);
				arrViewImage[1].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImage[0].IsAvailable()
					  && arrViewImage[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

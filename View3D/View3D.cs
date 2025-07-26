using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using CResult = FLImagingCLR.CResult;

namespace View3D
{
	class View3D
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
			Model = 0,
			Texture,
			Count,
		};

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

			for(int i = 0; i < 2; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			// 3D 뷰 선언 // Declare the 3D view
			CGUIView3D view3D = new CGUIView3D();

			do
			{
				CResult res;

				// Model 이미지 로드 // Load model image
				if((res = (arrFliImage[(int)EType.Model].Load("../../ExampleImages/View3D/mountain.flif"))).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Texture 이미지 로드 // Load texture image
				if((res = (arrFliImage[(int)EType.Texture].Load("../../ExampleImages/View3D/mountain_texture.flif"))).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Model 이미지 뷰 생성 // Create model image view
				if((res = (arrViewImage[(int)EType.Model].Create(100, 0, 612, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Texture 이미지 뷰 생성 // Create texture image view
				if((res = (arrViewImage[(int)EType.Texture].Create(612, 0, 1124, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 3D 뷰 생성 // Create 3D view
				if((res = (view3D.Create(1124, 0, 1636, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				bool bError = false;

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				for(int i = 0; i < (int)EType.Count; ++i)
				{
					if((res = (arrViewImage[i].SetImagePtr(ref arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = (arrViewImage[(int)EType.Model].SynchronizePointOfView(ref arrViewImage[(int)EType.Texture]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the two image view windows.
				if((res = (arrViewImage[(int)EType.Model].SynchronizeWindow(ref arrViewImage[(int)EType.Texture]))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 3D 뷰와 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of the image view and the 3D view window
				if((res = (arrViewImage[(int)EType.Model].SynchronizeWindow(ref view3D))).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				CFL3DObjectHeightMap fl3DOHM = new CFL3DObjectHeightMap(arrFliImage[(int)EType.Model], arrFliImage[(int)EType.Texture]);

				// 3D 뷰에 높이 맵과 텍스쳐를 로드하여 디스플레이
				if(view3D.PushObject(fl3DOHM).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the 3D view.\n");
					break;
				}

				view3D.ZoomFit();

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[2];

				for(int i = 0; i < 2; ++i)
				{
					// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
					// 따로 해제할 필요 없음 // No need to release
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the shapes drawn on the layer
					arrLayer[i].Clear();
				}

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the shapes drawn on the layer
				view3D.GetLayer(0).Clear();

				// View 정보를 디스플레이 합니다. // Display the view information.
				// 아래 함수 DrawTextCanvas() 는 Screen 좌표를 기준으로 하는 문자열을 Drawing 한다. // The function DrawTextCanvas() draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpPosition = new CFLPoint<double>(0, 0);

				if((res = (arrLayer[(int)EType.Model].DrawTextCanvas(flpPosition, "Model Image", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Texture].DrawTextCanvas(flpPosition, "Texture Image", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (view3D.GetLayer(0).DrawTextCanvas(flpPosition, "3D View", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				arrViewImage[(int)EType.Model].Invalidate(true);
				arrViewImage[(int)EType.Texture].Invalidate(true);
				view3D.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImage[(int)EType.Model].IsAvailable()
					  && arrViewImage[(int)EType.Texture].IsAvailable()
					  && view3D.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

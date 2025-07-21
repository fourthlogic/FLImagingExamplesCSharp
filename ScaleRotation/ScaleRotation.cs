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

namespace ScaleRotation
{
	class ScaleRotation
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
			Destination3,
			ETypeCount,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrFliImage[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrViewImage[i] = new CGUIViewImage();

			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = arrFliImage[(int)EType.Source].Load("../../ExampleImages/Affine/Sea.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				bool bError = false;

				for(int i = (int)EType.Destination1; i < (int)EType.ETypeCount; ++i)
				{
					// Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
					if((res = (arrFliImage[i].Assign(arrFliImage[(int)EType.Source]))).IsFail())
					{
						ErrorPrint(res, "Failed to assign the image file.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					int x = i % 2;
					int y = i / 2;

					// 이미지 뷰 생성 // Create image view
					if((res = (arrViewImage[i].Create(x * 400 + 400, y * 400, x * 400 + 400 + 400, y * 400 + 400))).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						bError = true;
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
					if((res = (arrViewImage[i].SetImagePtr(ref arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}

					if(i == (int)EType.Source)
						continue;

					// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
					if((res = (arrViewImage[(int)EType.Source].SynchronizeWindow(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// ScaleRotation 객체 생성 // Create ScaleRotation object
				CScaleRotation scaleRotation = new CScaleRotation();
				// Source 이미지 설정 // Set the source image
				scaleRotation.SetSourceImage(ref arrFliImage[(int)EType.Source]);
				// Destination 이미지 설정 // Set the destination image
				scaleRotation.SetDestinationImage(ref arrFliImage[(int)EType.Destination1]);
				// Scale 비율 설정 // set scale ratio
				scaleRotation.SetScale(1.5, 1.5);
				// 회전 각도 설정 // set rotation angle
				scaleRotation.SetAngle(30.0);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (scaleRotation.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute scaleRotation.");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				scaleRotation.SetDestinationImage(ref arrFliImage[(int)EType.Destination2]);
				// Scale 비율 설정 // set scale ratio
				scaleRotation.SetScale(0.7, 1.5);
				// 회전 각도 설정 // set rotation angle
				scaleRotation.SetAngle(30.0);
				// Image Resize 설정 // Set Image Resize
				scaleRotation.SetResizeMethod(EResizeMethod.Resize);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (scaleRotation.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute scaleRotation.");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				scaleRotation.SetDestinationImage(ref arrFliImage[(int)EType.Destination3]);
				// Scale 비율 설정 // set scale ratio
				scaleRotation.SetScale(1.5, 0.7);
				// 회전 각도 설정 // set rotation angle
				scaleRotation.SetAngle(240.0);
				// Image Resize 설정 // Set Image Resize
				scaleRotation.SetResizeMethod(EResizeMethod.Resize);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (scaleRotation.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute scaleRotation.");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.ETypeCount];

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayer[i].Clear();
				}

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpZero = new CFLPoint<double>(0, 0);

				if((res = (arrLayer[(int)EType.Source].DrawTextCanvas(flpZero, "Source Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination1].DrawTextCanvas(flpZero, "Destination1 Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination2].DrawTextCanvas(flpZero, "Destination2 Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = (arrLayer[(int)EType.Destination3].DrawTextCanvas(flpZero, "Destination3 Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					arrViewImage[i].ZoomFit();
					arrViewImage[i].Invalidate(true);
				}


				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bAvailable = true;
				while(bAvailable)
				{
					for(int i = 0; i < (int)EType.ETypeCount; ++i)
					{
						bAvailable = arrViewImage[i].IsAvailable();

						if(!bAvailable)
							break;
					}

					Thread.Sleep(1);
				}
			}
			while(false);
		}
	}
}
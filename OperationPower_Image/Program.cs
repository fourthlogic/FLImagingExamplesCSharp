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


namespace OperationBitwiseAnd
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
            CFLImage[] arrFliImage0 = new CFLImage[3];
            CFLImage[] arrFliImage1 = new CFLImage[3];

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage[] arrViewImage0 = new CGUIViewImage[3];
            CGUIViewImage[] arrViewImage1 = new CGUIViewImage[3];

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			for(int i = 0; i < 3; ++i)
            {
                arrFliImage0[i] = new CFLImage();
                arrViewImage0[i] = new CGUIViewImage();
                arrFliImage1[i] = new CFLImage();
                arrViewImage1[i] = new CGUIViewImage();
            }

            do
            {
                // Source 이미지 로드 // Load the source image
                if ((res = arrFliImage0[0].Load("../../ExampleImages/OperationPower/Sea3Ch.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Operand 이미지 로드 // Loads the operand image
                if ((res = arrFliImage0[1].Load("../../ExampleImages/OperationPower/Gradation.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Source 이미지 로드 // Load the source image
                if ((res =  arrFliImage1[0].Load("../../ExampleImages/OperationPower/Sea3ChF32.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Operand 이미지 로드 // Loads the operand image
                if ((res =  arrFliImage1[1].Load("../../ExampleImages/OperationPower/Gradation.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
                if ((res =  arrFliImage0[2].Assign(arrFliImage0[0])).IsFail())
                {
                    ErrorPrint(res, "Failed to assign the image file.\n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res =  arrViewImage0[0].Create(100, 0, 548, 448)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Operand 이미지 뷰 생성 // Create operand image view
                if ((res =  arrViewImage0[1].Create(548, 0, 996, 448)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if ((res =  arrViewImage0[2].Create(996, 0, 1444, 448)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }


                // Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
                if ((res =  arrFliImage1[2].Assign(arrFliImage1[0])).IsFail())
                {
                    ErrorPrint(res, "Failed to assign the image file.\n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res =  arrViewImage1[0].Create(100, 448, 548, 896)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Operand 이미지 뷰 생성 // Create operand image view
                if ((res =  arrViewImage1[1].Create(548, 448, 996, 896)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if ((res =  arrViewImage1[2].Create(996, 448, 1444, 896)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }


                bool bError = false;

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                for (int i = 0; i < 3; ++i)
                {
                    if ((res =  arrViewImage0[i].SetImagePtr(ref arrFliImage0[i])).IsFail())
                    {
                        ErrorPrint(res, "Failed to set image object on the image view.\n");
                        bError = true;
                        break;
                    }
                }

                for (int i = 0; i < 3; ++i)
                {
                    if ((res =  arrViewImage1[i].SetImagePtr(ref arrFliImage1[i])).IsFail())
                    {
                        ErrorPrint(res, "Failed to set image object on the image view.\n");
                        bError = true;
                        break;
                    }
                }

                if (bError)
                    break;

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res =  arrViewImage0[0].SynchronizePointOfView(arrViewImage0[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res =  arrViewImage0[0].SynchronizePointOfView(arrViewImage0[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res =  arrViewImage0[0].SynchronizeWindow(arrViewImage0[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res =  arrViewImage0[0].SynchronizeWindow(arrViewImage0[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res =  arrViewImage0[0].SynchronizePointOfView(arrViewImage1[0])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res =  arrViewImage0[0].SynchronizePointOfView(arrViewImage1[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res =  arrViewImage0[0].SynchronizePointOfView(arrViewImage1[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res =  arrViewImage0[0].SynchronizeWindow(arrViewImage1[0])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res =  arrViewImage0[0].SynchronizeWindow(arrViewImage1[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res =  arrViewImage0[0].SynchronizeWindow(arrViewImage1[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // Operation Power 객체 생성 // Create Operation Power object
                COperationPower power = new COperationPower();

                // Source 이미지 설정 // Set the source image
                power.SetSourceImage(ref arrFliImage0[0]);
                // Operand 이미지 설정 // Set the operand image
                power.SetOperandImage(ref arrFliImage0[1]);
                // Destination 이미지 설정 // Set the destination image
                power.SetDestinationImage(ref arrFliImage0[2]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Image);
             
                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation power.");
                    Console.WriteLine(res.GetString());
                    break;
                }

                power.SetSourceImage(ref arrFliImage1[0]);
                // Operand 이미지 설정 // Set the operand image
                power.SetOperandImage(ref arrFliImage1[1]);
                // Destination 이미지 설정 // Set the destination image
                power.SetDestinationImage(ref arrFliImage1[2]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Image);
				// Overflow Method Wrapping 옵션으로 설정 // Set Overflow Method to Wrapping option
				power.SetOverflowMethod(EOverflowMethod.Wrapping);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation power.");
                    Console.WriteLine(res.GetString());
                    break;
                }

                CGUIViewImageLayer[] arrLayer0 = new CGUIViewImageLayer[3];
                CGUIViewImageLayer[] arrLayer1 = new CGUIViewImageLayer[3];

                for (int i = 0; i < 3; ++i)
                {
                    // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                    // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                    arrLayer0[i] = arrViewImage0[i].GetLayer(0);

                    // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                    arrLayer0[i].Clear();
                }

                for (int i = 0; i < 3; ++i)
                {
                    // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                    // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                    arrLayer1[i] = arrViewImage1[i].GetLayer(0);

                    // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                    arrLayer1[i].Clear();
                }

                // 이미지 뷰 정보 표시 // Display image view information
                // 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
                // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
                // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                // Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
                //                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
                TPoint<double> tpPosition = new TPoint<double>(5, 5);

                if ((res =  arrLayer0[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res =  arrLayer0[1].DrawTextCanvas(tpPosition, "Operand Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res =  arrLayer0[2].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }


                if ((res =  arrLayer1[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res =  arrLayer1[1].DrawTextCanvas(tpPosition, "Operand Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res =  arrLayer1[2].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                arrViewImage0[0].Invalidate(true);
                arrViewImage0[1].Invalidate(true);
                arrViewImage0[2].Invalidate(true);
                arrViewImage1[0].Invalidate(true);
                arrViewImage1[1].Invalidate(true);
                arrViewImage1[2].Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (arrViewImage0[0].IsAvailable()
                      && arrViewImage0[1].IsAvailable()
                      && arrViewImage0[2].IsAvailable()
                      && arrViewImage1[0].IsAvailable()
                      && arrViewImage1[1].IsAvailable()
                      && arrViewImage1[2].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

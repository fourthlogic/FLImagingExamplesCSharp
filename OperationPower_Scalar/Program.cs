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
            CFLImage[] arrFliImage = new CFLImage[6];

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage[] arrViewImage = new CGUIViewImage[6];

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			for(int i = 0; i < 6; ++i)
            {
                arrFliImage[i] = new CFLImage();
                arrViewImage[i] = new CGUIViewImage();
            }

            do
            {
                // Source 이미지 로드 // Load the source image
                if ((res = arrFliImage[0].Load("../../ExampleImages/OperationPower/Space3Ch.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Destination1 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination1 image as same as source image
                if ((res = arrFliImage[1].Assign(arrFliImage[0])).IsFail())
                {
                    ErrorPrint(res, "Failed to assign the image file.\n");
                    break;
                }

				// Destination2 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination2 image as same as source image
				if((res = arrFliImage[2].Assign(arrFliImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				// Destination3 이미지 로드. // Load the destination3 image
				if ((res = arrFliImage[3].Load("../../ExampleImages/OperationPower/Dst16Depth.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Destination4 이미지 로드. // Load the destination4 image
                if ((res = arrFliImage[4].Load("../../ExampleImages/OperationPower/Dst16Depth.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Destination5 이미지 로드. // Load the destination5 image
                if ((res = arrFliImage[5].Load("../../ExampleImages/OperationPower/Dst64Depth.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res = arrViewImage[0].Create(100, 0, 548, 448)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination1 이미지 뷰 생성 // Create destination1 image view
                if ((res = arrViewImage[1].Create(548, 0, 996, 448)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination2 이미지 뷰 생성 // Create destination2 image view
                if ((res = arrViewImage[2].Create(996, 0, 1444, 448)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination3 이미지 뷰 생성 // Create the destination3 image view
                if ((res = arrViewImage[3].Create(100, 448, 548, 896)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination4 이미지 뷰 생성 // Create the destination4 image view
                if ((res = arrViewImage[4].Create(548, 448, 996, 896)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // Destination5 이미지 뷰 생성 // Create the destination5 image view
                if ((res = arrViewImage[5].Create(996, 448, 1444, 896)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                bool bError = false;

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                for (int i = 0; i < 6; ++i)
                {
                    if ((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
                    {
                        ErrorPrint(res, "Failed to set image object on the image view.\n");
                        bError = true;
                        break;
                    }
                }

                if (bError)
                    break;

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[3])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[4])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[0].SynchronizePointOfView(ref arrViewImage[5])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[3])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[4])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[5])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                CMultiVar<double> mvScalr1 = new CMultiVar<double>(1.1, 1.2, 1.5);
                CMultiVar<double> mvScalr2 = new CMultiVar<double>(0.8, 0.8, 1.1);
                CMultiVar<double> mvScalr3 = new CMultiVar<double>(2.5, 2.5, 2.5);
                CMultiVar<double> mvScalr4 = new CMultiVar<double>(2.5, 2.5, 2.5);
                CMultiVar<double> mvScalr5 = new CMultiVar<double>(10, 10, 10);

                // Operation Power 객체 생성 // Create Operation Power object
                COperationPower power = new COperationPower();

                // Source 이미지 설정 // Set the source image
                power.SetSourceImage(ref arrFliImage[0]);
                // Destination 이미지 설정 // Set the destination image
                power.SetDestinationImage(ref arrFliImage[1]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Scalar);
				// Overflow Method Clamping 옵션으로 설정 // Set Overflow Method to Clamping option
				power.SetOverflowMethod(EOverflowMethod.Clamping);
                // Exponent 값 설정 // Set Exponent value
                power.SetScalarValue(mvScalr1);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation Power.\n");
                    break;
                }

				// Destination 이미지 설정 // Set the destination image
				power.SetDestinationImage(ref arrFliImage[2]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Scalar);
				// Overflow Method Clamping 옵션으로 설정 // Set Overflow Method to Clamping option
				power.SetOverflowMethod(EOverflowMethod.Clamping);
                // Exponent 값 설정 // Set Exponent value
                power.SetScalarValue(mvScalr2);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation Power.\n");
                    break;
                }

				// Destination 이미지 설정 // Set the destination image
				power.SetDestinationImage(ref arrFliImage[3]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Scalar);
				// Overflow Method Wrapping 옵션으로 설정 // Set Overflow Method to Wrapping option
				power.SetOverflowMethod(EOverflowMethod.Wrapping);
                // Exponent 값 설정 // Set Exponent value
                power.SetScalarValue(mvScalr3);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation Power.\n");
                    break;
                }

				// Destination 이미지 설정 // Set the destination image
				power.SetDestinationImage(ref arrFliImage[4]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Scalar);
				// Overflow Method Clamping 옵션으로 설정 // Set Overflow Method to Clamping option
				power.SetOverflowMethod(EOverflowMethod.Clamping);
                // Exponent 값 설정 // Set Exponent value
                power.SetScalarValue(mvScalr4);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation Power.");
                    Console.WriteLine(res.GetString());
                    break;
                }

				// Destination 이미지 설정 // Set the destination image
				power.SetDestinationImage(ref arrFliImage[5]);
                // Image Operation 모드로 설정 // Set operation mode to image
                power.SetOperationSource(EOperationSource.Scalar);
				// Overflow Method Wrapping 옵션으로 설정 // Set Overflow Method to Wrapping option
				power.SetOverflowMethod(EOverflowMethod.Wrapping);
                // Exponent 값 설정 // Set Exponent value
                power.SetScalarValue(mvScalr5);

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = power.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute operation Power.\n");
                    break;
                }

                CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[6];
                arrLayer[0] = new CGUIViewImageLayer();
                arrLayer[1] = new CGUIViewImageLayer();
                arrLayer[2] = new CGUIViewImageLayer();
                arrLayer[3] = new CGUIViewImageLayer();
                arrLayer[4] = new CGUIViewImageLayer();
                arrLayer[5] = new CGUIViewImageLayer();

                for (int i = 0; i < 6; ++i)
                {
                    // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                    // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                    arrLayer[i] = arrViewImage[i].GetLayer(0);

                    // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                    arrLayer[i].Clear();
                }

                // 이미지 뷰 정보 표시 // Display image view information
                // 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
                // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
                // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                // Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
                //                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
                TPoint<double> tpPosition0 = new TPoint<double>(5, 0);
                TPoint<double> tpPosition1 = new TPoint<double>(5, 22);

                if ((res = arrLayer[0].DrawTextCanvas(tpPosition0, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[1].DrawTextCanvas(tpPosition0, "Destination1 Image(Power 1.1, 1.2, 1.5)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[1].DrawTextCanvas(tpPosition1, "Unsigned Int / 8 / Saturation", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[2].DrawTextCanvas(tpPosition0, "Destination2 Image(Power 0.8, 0.8, 1.1)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[2].DrawTextCanvas(tpPosition1, "Unsigned Int / 8 / Saturation", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[3].DrawTextCanvas(tpPosition0, "Destination3 Image(Power 2.5, 2.5, 2.5)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[3].DrawTextCanvas(tpPosition1, "Unsigned Int / 16 / Overflow", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[4].DrawTextCanvas(tpPosition0, "Destination4 Image(Power 2.5, 2.5, 2.5)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[4].DrawTextCanvas(tpPosition1, "Unsigned Int / 16 / Saturation", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                if ((res = arrLayer[5].DrawTextCanvas(tpPosition0, "Destination5 Image(Power 10, 10, 10)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }
                if ((res = arrLayer[5].DrawTextCanvas(tpPosition1, "Unsigned Int / 64 / Overflow", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                arrViewImage[0].Invalidate(true);
                arrViewImage[1].Invalidate(true);
                arrViewImage[2].Invalidate(true);
                arrViewImage[3].Invalidate(true);
                arrViewImage[4].Invalidate(true);
                arrViewImage[5].Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (arrViewImage[0].IsAvailable()
                      && arrViewImage[1].IsAvailable()
                      && arrViewImage[2].IsAvailable()
                      && arrViewImage[3].IsAvailable()
                      && arrViewImage[4].IsAvailable()
                      && arrViewImage[5].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

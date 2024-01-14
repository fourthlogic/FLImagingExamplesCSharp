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

namespace OperationCompare
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
            CFLImage fliSourceImage = new CFLImage();
            CFLImage fliOperandImage = new CFLImage();
            CFLImage fliDestinationImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageSource = new CGUIViewImage();
            CGUIViewImage viewImageOperand = new CGUIViewImage();
            CGUIViewImage viewImageDestination = new CGUIViewImage();

	        do
	        {

				// 알고리즘 동작 결과 // Algorithm execution result
				CResult eResult = new CResult();


				// Source 이미지 로드 // Load the source image
				if((eResult =fliSourceImage.Load("../../ExampleImages/OperationCompare/siamesecat3ch.flif")).IsFail())
                {
                    ErrorPrint(eResult,"Failed to load the image file. \n");
                    break;
                }

                // Operand 이미지 로드 // Loads the operand image
                if((eResult =fliOperandImage.Load("../../ExampleImages/OperationCompare/space3ch.flif")).IsFail())
                {
                    ErrorPrint(eResult,"Failed to load the image file. \n");
                    break;
                }

                // Destination 이미지 로드 // Load the destination image
                if((eResult =fliDestinationImage.Load("../../ExampleImages/OperationCompare/sea3ch.flif")).IsFail())
                {
                    ErrorPrint(eResult,"Failed to load the image file. \n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if((eResult =viewImageSource.Create(100, 0, 612, 512)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to create the image view. \n");
                    break;
                }

                // Operand 이미지 뷰 생성 // Create operand image view
                if((eResult =viewImageOperand.Create(612, 0, 1124, 512)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to create the image view.\n");
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if((eResult =viewImageDestination.Create(1124, 0, 1636, 512)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to create the image view. \n");
                    break;
                }

                // Source 이미지 뷰와 Operand 이미지 뷰의 시점을 동기화 한다
                if((eResult =viewImageSource.SynchronizePointOfView(ref viewImageOperand)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to synchronize view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if((eResult =viewImageSource.SynchronizePointOfView(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to synchronize view. \n");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if((eResult =viewImageSource.SetImagePtr(ref fliSourceImage)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to set image object on the image view. \n");
                    break;
                }

                // Operand 이미지 뷰에 이미지를 디스플레이
                if((eResult =viewImageOperand.SetImagePtr(ref fliOperandImage)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to set image object on the image view. \n");
                    break;
                }

                // Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if((eResult =viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to set image object on the image view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if((eResult =viewImageSource.SynchronizeWindow(ref viewImageOperand)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to synchronize window. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if((eResult =viewImageSource.SynchronizeWindow(ref viewImageDestination)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to synchronize window. \n");
                    break;
                }

                // Operation Compare 객체 생성 // Create Operation Compare object
                COperationCompare compare = new COperationCompare();

                // Source 이미지 설정 // Set the source image
	        	compare.SetSourceImage(ref fliSourceImage);

                // Source 이미지의 ROI 범위 설정
                CFLCircle<double> flcSourceROI = new CFLCircle<double>(258, 258, 174, 0, 0, 360, EArcClosingMethod.EachOther);

                // Source 이미지의 ROI 지정
		        compare.SetSourceROI(flcSourceROI);

		        // Operand 이미지 설정 // Set the operand image
		        compare.SetOperandImage(ref fliOperandImage);

		        // Destination 이미지 설정 // Set the destination image
		        compare.SetDestinationImage(ref fliDestinationImage);

		        // Destination 이미지 Pivot 좌표 설정
                CFLPoint<double> flpDestinationPivot = new CFLPoint<double>(174, 169);

		        // Destination 이미지 Pivot 지정
		        compare.SetDestinationPivot(flpDestinationPivot);

		        // Image Operation 모드로 설정 // Set operation mode to image
		        compare.SetOperationSource(EOperationSource.Image);

		        // 공백 색상 칠하기 모드 해제
		        // 결과 이미지가 이미 존재할 경우 연산되지 않은 영역을 공백 색상으로 칠하지 않고 원본 그대로 둔다.
		        compare.EnableFillBlankColorMode(false);

		        // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
		        if((eResult = compare.Execute()).IsFail())
		        {
                    ErrorPrint(eResult,"Failed to execute Operation Compare. \n");
                    ErrorPrint(eResult,"\n");

                    break;
		        }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                CGUIViewImageLayer layerOperand = viewImageOperand.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSource.Clear();
                layerOperand.Clear();
                layerDestination.Clear();

                // FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
                // Source ROI 영역이 어디인지 알기 위해 디스플레이 한다
                if((eResult =layerSource.DrawFigureImage(flcSourceROI, EColor.LIME)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to draw figure. \n");
                    break;
                }

                // Destination Pivot 영역이 어디인지 알기 위해 디스플레이 한다
                CFLFigureArray flfaDestinationPivotCrossHair = flpDestinationPivot.MakeCrossHair(20, true);

                if((eResult =layerDestination.DrawFigureImage(flfaDestinationPivotCrossHair, EColor.BLACK, 3)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to draw figure. \n");
                    break;
                }

                if((eResult =layerDestination.DrawFigureImage(flfaDestinationPivotCrossHair, EColor.LIME)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to draw figure. \n");
                    break;
                }

                // 이미지 뷰 정보 표시 // Display image view information
                CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

                if((eResult =layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to draw text. \n");
                    break;
                }

                if((eResult =layerOperand.DrawTextCanvas(flpPoint, "Operand Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to draw text. \n");
                    break;
                }

                if((eResult =layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(eResult,"Failed to draw text. \n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                viewImageSource.Invalidate(true);
                viewImageOperand.Invalidate(true);
                viewImageDestination.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSource.IsAvailable() && viewImageOperand.IsAvailable() && viewImageDestination.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

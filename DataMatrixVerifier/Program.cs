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

namespace DataMatrix
{
    class Program
    {
        public static void ErrorPrint(CResult cResult, string str)
        {
            if (str.Length > 1)
                Console.WriteLine(str);

            Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
            Console.WriteLine("\n");
            Console.ReadKey();
        }
        [STAThread]
        static void Main(string[] args)
        {
            // 이미지 객체 선언 // Declare the image object
            CFLImage fliImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImage = new CGUIViewImage();

            do
            {
                CResult res;

                // 이미지 로드 // Load image
                if ((res = fliImage.Load("../../ExampleImages/DataMatrix/Module.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if ((res = viewImage.Create(400, 0, 1424, 768)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                if ((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
                {
                    ErrorPrint(res, "Failed to set image object on the image view.\n");
                    break;
                }

                // Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
                if ((res = viewImage.ZoomFit()).IsFail())
                {
                    ErrorPrint(res, "Failed to zoom fit\n");
                    break;
                }

                // DataMatrix 객체 생성 // Create DataMatrix object
                CDataMatrixVerifier dataMatrixVerifier = new CDataMatrixVerifier();

                // 처리할 이미지 설정 // Set the image to process
                dataMatrixVerifier.SetSourceImage(ref fliImage);

                // ROI 범위 설정
                CFLRect<Int32> flrROI = new CFLRect<Int32>(150, 25, 475, 300);

                // 처리할 ROI 설정
                dataMatrixVerifier.SetSourceROI(flrROI);

                // Decode 데이터 영역 색상 설정
                // EDataMatrixColor_Auto 로 설정 시 자동으로 Decode 된다.
                dataMatrixVerifier.SetColorMode(EDataCodeColor.BlackOnWhite);

				// ISO/IEC 15415 양식 인쇄 품질 평가를 활성화합니다. 기본값은 true이며 처리하지 않아도 됩니다.
				// Enables ISO/IEC 15415 Form Print Quality Assessment. The default is true and does not require processing.
				dataMatrixVerifier.EnablePrintQuality_ISOIEC_15415(true);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = dataMatrixVerifier.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute DataMatrix decoder.");
                    break;
                }

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                CGUIViewImageLayer layer = viewImage.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layer.Clear();

                // ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
                // FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
                if ((res = layer.DrawFigureImage(flrROI, EColor.BLUE)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
                    break;
                }

                // Data Matrix Verifier 결과 개수를 얻는다.
                Int64 i64Results = dataMatrixVerifier.GetResultCount();

                // 디코딩 결과값을 각각 확인하는 코드
                for (Int64 i = 0; i < i64Results; ++i)
                {
                    // Data Matrix Verifier 결과를 얻어오기 위해 FLQuadD 선언
                    CFLQuad<double> flqRegion;

                    // Data Matrix Verifier 결과들 중 Data Region 을 얻어옴
                    if ((res = dataMatrixVerifier.GetResultDataRegion(i, out flqRegion)).IsFail())
                    {
                        ErrorPrint(res, "Failed to get data region from the data matrix decoder object.");
                        continue;
                    }

                    // Data Matrix 의 영역을 디스플레이 한다.
                    if ((res = layer.DrawFigureImage(flqRegion, EColor.LIME, 2)).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw figure object on the image view.\n");
                        continue;
                    }

                    // Data Matrix Verifier 결과를 얻어오기 위해 FigureArray 선언
                    CFLFigureArray flfaGridRegion;

                    // Data Matrix Verifier 결과들 중 Grid Region 을 얻어옴
                    if ((res = dataMatrixVerifier.GetResultGridRegion(i, out flfaGridRegion)).IsFail())
                    {
                        ErrorPrint(res, "Failed to get grid region from the data matrix decoder object.");
                        continue;
                    }

                    // Data Matrix 의 Grid Region 을 디스플레이 한다.
                    if ((res = layer.DrawFigureImage(flfaGridRegion, EColor.LIME, 2)).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        continue;
                    }

                    // Data Matrix Verifier 결과를 얻어오기 위해 FLStringW 선언
                    string strDecoded = "";

                    // Data Matrix Verifier 결과들 중 Decoded String 을 얻어옴
                    if ((res = dataMatrixVerifier.GetResultDecodedString(i, out strDecoded)).IsFail())
                    {
                        ErrorPrint(res, "Failed to get decoded string from the data matrix decoder object.");
                        continue;
                    }

                    Console.Write("No. {0} Code : {1}\n", i, strDecoded);

                    // Decoded String 을 디스플레이 한다.
                    // 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
                    // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
                    // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                    //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                    // Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
                    //                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
                    if ((res = layer.DrawTextImage(flqRegion.flpPoints[3], strDecoded, EColor.CYAN, EColor.BLACK, 20, false, flqRegion.flpPoints[3].GetAngle(flqRegion.flpPoints[2]))).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw string object on the image view.");
                        continue;
                    }

					// Data Matrix Verifier 결과들 중 인쇄 품질을 얻어옴 // Get print quality among Data Matrix Verifier results
					CPrintQuality_ISOIEC_15415 printQuality = new CPrintQuality_ISOIEC_15415();

					if((res = dataMatrixVerifier.GetResultPrintQuality_ISOIEC_15415(i, out printQuality)).IsFail())
					{
						ErrorPrint(res, "Failed to get print quality from the data matrix decoder object.");
						continue;
					}

					// 등급 계산이 처리되었는 지 확인
					if(printQuality.IsGraded())
					{
						string strGrade = string.Format("[ISO/IEC 15415]\r\nDecoding Grade : {0:F1}\r\nAxialNonuniformity Grade : {1:F1}\r\nGridNonuniformity Grade : {2:F1}\r\nSymbolContrast Grade : {3:F1}\r\nUnusedErrorCorrection Grade : {4:F1}\r\nModulation Grade : {5:F1}\r\nFixedPatternDamage Grade : {6:F1}\r\nHorizontalPrintGrowth Grade : {7:F1}\r\nVerticalPrintGrowth Grade : {8:F1}\r\nOverallSymbol Grade : {9:F1}", printQuality.f64DecodingGrade, printQuality.f64AxialNonuniformityGrade, printQuality.f64GridNonuniformityGrade, printQuality.f64SymbolContrastGrade, printQuality.f64UnusedErrorCorrectionGrade, printQuality.f64ModulationGrade, printQuality.f64FixedPatternDamageGrade, printQuality.f64HorizontalPrintGrowthGrade, printQuality.f64VerticalPrintGrowthGrade, printQuality.f64OverallSymbolGrade);

						CFLRect<double> flrBoundary = flqRegion.GetBoundaryRect();
                        CFLPoint<double> flpPoint = new CFLPoint<double>(flrBoundary.left, flrBoundary.top);

						if((res = layer.DrawTextImage(flpPoint, strGrade, EColor.YELLOW, EColor.BLACK, 15, false, 0.0, EGUIViewImageTextAlignment.RIGHT_TOP)).IsFail())
						{
							ErrorPrint(res, "Failed to draw string object on the image view.\n");
							continue;
						}
					}
				}

                // 이미지 뷰를 갱신 합니다. // Update image view
                viewImage.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImage.IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
        }
    }
}

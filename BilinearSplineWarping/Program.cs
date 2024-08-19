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


namespace BilinearSplineWarping
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
			CFLImage[] arrFliImage = new CFLImage[4];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[4];

			for(int i = 0; i < 4; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			do
			{
				CResult res;
				// Source 이미지 로드 // Load the source image
				if ((res = arrFliImage[0].Load("../../ExampleImages/BilinearSplineWarping/chess.flif")).IsFail())
                {
                    ErrorPrint(res, "Failed to load the image file.\n");
                    break;
                }

                // Source 이미지 뷰 생성 // Create source image view
                if ((res = arrViewImage[0].Create(100, 0, 612, 512)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");;
                    break;
                }

                // Destination 이미지 뷰 생성 // Create destination image view
                if ((res = arrViewImage[1].Create(612, 0, 1124, 512)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");;
                    break;
                }

                // Source 이미지 뷰 2 생성 // Create source image view2
                if((res = arrViewImage[2].Create(100, 512, 612, 1024)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");;
                    break;
                }

                // Destination 이미지 뷰 2 생성 // Create destination image view2
                if((res = arrViewImage[3].Create(612, 512, 1124, 1024)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the image view.\n");;
                    break;
                }

                bool bError = false;

                // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                for (int i = 0; i < 4; ++i)
                {
                    if ((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
                    {
                        ErrorPrint(res, "Failed to set image object on the image view.\n");;
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

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[0].SynchronizeWindow(ref arrViewImage[1])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = arrViewImage[2].SynchronizePointOfView(ref arrViewImage[3])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize view\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
                if ((res = arrViewImage[3].SynchronizeWindow(ref arrViewImage[2])).IsFail())
                {
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }

                // BilinearSplineWarping 객체 생성 // Create BilinearSplineWarping object
                CBilinearSplineWarping BilinearSplineWarping = new CBilinearSplineWarping();
                // Source 이미지 설정 // Set the source image
                BilinearSplineWarping.SetSourceImage(ref arrFliImage[0]);
                // Destination 이미지 설정 // Set the destination image
                BilinearSplineWarping.SetDestinationImage(ref arrFliImage[1]);
                // Interpolation Method 설정 // Set the interpolation method
                BilinearSplineWarping.SetInterpolationMethod(EInterpolationMethod.Bilinear);

                // 그리드를 (5,5)로 초기화 // Initialize the grid to (5,5)
                CFLPoint<int> flpGridSize = new CFLPoint<int>(5, 5);

                CFLPoint<int> flpGridIndex = new CFLPoint<int>();

                CFLPointArray flpaSource = new CFLPointArray();
                CFLPointArray flpaTarget = new CFLPointArray();

                double f64ScaleX = arrFliImage[0].GetWidth() / 4.0;
                double f64ScaleY = arrFliImage[0].GetHeight() / 4.0;

                for (int y = 0; y < flpGridSize.y; ++y)
                {
                    flpGridIndex.y = y;

                    for (int x = 0; x < flpGridSize.x; ++x)
                    {
                        flpGridIndex.x = x;

                        // Grid Index와 같은 좌표로 Source 좌표를 설정 // Set Source coordinates to the same coordinates as Grid Index
                        CFLPoint<double> flpSource = new CFLPoint<double>(flpGridIndex.x * f64ScaleX, flpGridIndex.y * f64ScaleY);

                        double f64RandomX = CRandomGenerator.Double(-0.2, 0.2);
                        double f64RandomY = CRandomGenerator.Double(-0.2, 0.2);

                        // 외곽의 좌표는 안쪽으로 변형 되도록 설정 // Set the outer coordinates to be Warpinged inward
                        if(y == 0)
                            f64RandomY = f64RandomY < 0 ? -f64RandomY : f64RandomY;

                        if (x == 0)
                            f64RandomX = f64RandomX < 0 ? -f64RandomX : f64RandomX;

                        if (y == flpGridSize.y - 1)
                            f64RandomY = f64RandomY > 0 ? -f64RandomY : f64RandomY;

                        if (x == flpGridSize.x - 1)
                            f64RandomX = f64RandomX > 0 ? -f64RandomX : f64RandomX;

                        // Grid Index와 같은 좌표에서 미세한 랜덤 값을 부여해서 좌표를 왜곡 // Distort coordinates by giving fine random values at the same coordinates as Grid Index
                        CFLPoint<double> flpDistortion = new CFLPoint<double>((flpGridIndex.x + f64RandomX) * f64ScaleX, (flpGridIndex.y + f64RandomY) * f64ScaleY);

                        flpaSource.PushBack(flpSource);
                        flpaTarget.PushBack(flpDistortion);
                    }
                }

                // 위에서 설정한 좌표들을 바탕으로 BicubicSplineWarping 클래스에 Point 배열 설정 // Set the Point array in the BicubicSplineWarping class based on the coordinates set above
                BilinearSplineWarping.SetCalibrationPointArray(flpaSource, flpaTarget);

                CGUIViewImageLayer layer = arrViewImage[0].GetLayer(0);

                // BicubicSplineWarping 클래스에 설정된 Vertex 정보를 화면에 Display // Display the vertex information set in the BicubicSplineWarping class on the screen
                for(int k = 0; k < flpaSource.GetCount(); ++k)
                {
                    CFLPoint<double> flpSource = new CFLPoint<double>();
                    CFLPoint<double> flpTarget = new CFLPoint<double>();

                    flpSource = flpaSource.GetAt(k);
                    flpTarget = flpaTarget.GetAt(k);

                    CFLLine<double> fllLine = new CFLLine<double>(flpSource, flpTarget);
                    CFLFigureArray flfaArrow = new CFLFigureArray();

                    flfaArrow = fllLine.MakeArrowWithRatio(0.25, true, 20);

					// Target Vertex를 각 View Layer에 Drawing // Drawing the target vertex on each view layer
					if((res = layer.DrawFigureImage(flpTarget, EColor.BLUE, 1)).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        break;
                    }

                    // Source Vertex를 각 View Layer에 Drawing // Drawing the source vertex on each view layer
                    if((res = layer.DrawFigureImage(flpSource, EColor.RED, 1)).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        break;
                    }

                    // Source Vertex를 각 View Layer에 Drawing // Drawing the source vertex on each view layer
                    if((res = layer.DrawFigureImage(flfaArrow, EColor.YELLOW, 1)).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        break;
                    }
                }

                // 앞서 설정된 이미지, Calibration Point Array로 Calibrate 수행 // Calibrate with previously set image, Calibration Point Array
                if((res = BilinearSplineWarping.Calibrate()).IsFail())
                {
                    ErrorPrint(res, "Failed to calibrate BilinearSplineWarping.");
                    
                    break;
                }

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = BilinearSplineWarping.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute BilinearSplineWarping.");
                    
                    break;
                }

				// 두번째 Source 이미지 설정 // Set the second source image
				arrFliImage[2].Assign(arrFliImage[1]);

                // Source 이미지 설정 // Set the source image
                BilinearSplineWarping.SetSourceImage(ref arrFliImage[2]);
                // Destination 이미지 설정 // Set the destination image
                BilinearSplineWarping.SetDestinationImage(ref arrFliImage[3]);
                // Interpolation Method 설정 // Set the interpolation method
                BilinearSplineWarping.SetInterpolationMethod(EInterpolationMethod.Bilinear);
                // Calibration Src, Target Points 바꿔서 셋팅 // Set Calibration Src, Target Points by changing
                BilinearSplineWarping.SetCalibrationPointArray(flpaTarget, flpaSource);

                // 앞서 설정된 이미지, Calibration Point Array로 Calibrate 수행 // Calibrate with previously set image, Calibration Point Array
                if((res = BilinearSplineWarping.Calibrate()).IsFail())
                {
                    ErrorPrint(res, "Failed to calibrate BilinearSplineWarping.");
                    
                    break;
                }

                // 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = BilinearSplineWarping.Execute()).IsFail())
                {
                    ErrorPrint(res, "Failed to execute BilinearSplineWarping.");
                    
                    break;
                }

                layer = arrViewImage[2].GetLayer(0);

				// BicubicSplineWarping 클래스에 설정된 Vertex 정보를 화면에 Display // Display the vertex information set in the BicubicSplineWarping class on the screen.
                                for (int k = 0; k < flpaSource.GetCount(); ++k)
                {
                    CFLPoint<double> flpSource = new CFLPoint<double>();
                    CFLPoint<double> flpTarget = new CFLPoint<double>();

                    flpSource = flpaTarget.GetAt(k);
                    flpTarget = flpaSource.GetAt(k);

                    CFLLine<double> fllLine = new CFLLine<double>(flpSource, flpTarget);
                    CFLFigureArray flfaArrow = new CFLFigureArray();

                    flfaArrow = fllLine.MakeArrowWithRatio(0.25, true, 20);

                    // Target Vertex를 각 View Layer에 Drawing // Drawing the target vertex on each view layer
                    if((res = layer.DrawFigureImage(flpTarget, EColor.BLUE, 1)).IsFail())
                    {
						ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        break;
                    }

					// Source Vertex를 각 View Layer에 Drawing // Drawing the source vertex on each view layer
					if((res = layer.DrawFigureImage(flpSource, EColor.RED, 1)).IsFail())
                    {
						ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        break;
                    }

                    // Source Vertex를 각 View Layer에 Drawing // Drawing the source vertex on each view layer
                    if((res = layer.DrawFigureImage(flfaArrow, EColor.YELLOW, 1)).IsFail())
                    {
                        ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
                        break;
                    }
                }

                CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[4];

                for (int i = 0; i < 4; ++i)
                {
                    // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                    // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
                    arrLayer[i] = arrViewImage[i].GetLayer(1);

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
                TPoint<double> tpPosition = new TPoint<double>(0, 0);

                if ((res = arrLayer[0].DrawTextCanvas(tpPosition, "Source Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                if ((res = arrLayer[1].DrawTextCanvas(tpPosition, "Destination Image", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                if ((res = arrLayer[2].DrawTextCanvas(tpPosition, "Source Image 2", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                if ((res = arrLayer[3].DrawTextCanvas(tpPosition, "Destination Image 2", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                // 이미지 뷰를 갱신 // Update image view
                for (int i = 0; i < 4; i++)
                    arrViewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(arrViewImage[0].IsAvailable()
					  && arrViewImage[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

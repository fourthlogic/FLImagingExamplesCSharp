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

namespace LineSegmentDetector
{
	class LineSegmentDetector
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
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

            // 이미지 객체 선언 // Declare the image object
            CFLImage fliSrc = new CFLImage();
            CFLImage fliDst = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageSrc = new CGUIViewImage();
            CGUIViewImage viewImageDst = new CGUIViewImage();

            CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliSrc.Load("../../ExampleImages/Matching/DrawingImage.flif")).IsFail() ||
                    (res = fliDst.Assign(fliSrc)).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view

                if ((res = viewImageSrc.Create(0, 0, 600, 600)).IsFail() ||
                   (res = viewImageDst.Create(600, 0, 1200, 600)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                // 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
                if ((res = viewImageSrc.SetImagePtr(ref fliSrc)).IsFail() ||
                   (res =  viewImageDst.SetImagePtr(ref fliDst)).IsFail())
				{
                    ErrorPrint(res, "Failed to set image object on the image view.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if ((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize point of view.\n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if ((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
                    ErrorPrint(res, "Failed to synchronize window.\n");
                    break;
                }


				// lsd 객체 생성 // Create lsd object
                var lsd = new CLineSegmentDetector();
                lsd.SetSourceImage(ref fliSrc);
                

				// 알고리즘 수행 // Execute the algorithm
				if((res = lsd.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute lsd.");
					break;
                }
                
                CFLFigureArray flfaResults = new CFLFigureArray();
                List<double> arrScores = new List<double>();
                double f64ScoreThreshold = lsd.GetScoreThreshold();

                lsd.GetResultLineSegments(ref flfaResults);
                lsd.GetResultScores(ref arrScores);

                // 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
                // 따로 해제할 필요 없음 // No need to release separately
                var layer = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layer.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic

                layer.DrawFigureImage(flfaResults, EColor.RED);

				// 이미지 뷰를 갱신 합니다. // Update the image view.
                viewImageSrc.Invalidate(true);
                viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
                    CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

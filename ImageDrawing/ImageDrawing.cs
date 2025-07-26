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
    class ImageDrawing
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
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare image object
	        CFLImage fliImage = new CFLImage();

	        // 이미지 드로잉 객체 선언 // Declare image drawing object
	        CFLImageDrawing fliImageDrawing = new CFLImageDrawing();

	        // 이미지 뷰 선언 // Declare image view
	        CGUIViewImage viewImageSrc = new CGUIViewImage();
	        CGUIViewImage viewImageDst = new CGUIViewImage();

	        do
			{
                // 수행 결과 객체 선언 // Declare the execution result object
			    CResult res;

		        // 이미지 로드 // Load image
		        if((res = fliImage.Load("../../ExampleImages/Blob/AlignBall.flif")).IsFail())
				{
			        ErrorPrint(res, "Failed to load the image file.\n");
			        break;
		        }

		        // Drawing 이미지를 Src 이미지와 동일한 이미지로 생성
		        if((res = fliImageDrawing.Assign(fliImage)).IsFail())
				{
			        ErrorPrint(res, "Failed to assign the image file.\n");
			        break;
		        }

		        // 이미지 뷰 생성 // Create image view
		        if((res = viewImageSrc.Create(400, 0, 800, 400)).IsFail())
				{
			        ErrorPrint(res, "Failed to create the image view.\n");
			        break;
		        }

		        // 이미지 뷰 생성 // Create image view
		        if((res = viewImageDst.Create(800, 0, 1200, 400)).IsFail())
				{
			        ErrorPrint(res, "Failed to create the image view.\n");
			        break;
		        }

		        // 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.
		        if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
			        ErrorPrint(res, "Failed to synchronize view\n");
			        break;
		        }

		        // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
		        if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
			        ErrorPrint(res, "Failed to synchronize window.\n");
			        break;
		        }

		        // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
		        if((res = viewImageSrc.SetImagePtr(ref fliImage)).IsFail())
				{
			        ErrorPrint(res, "Failed to set image object on the image view.\n");
			        break;
		        }

                // 레이어는 따로 해제하지 않아도 View가 해제 될 때 같이 해제된다. // The layer is released together when View is released without releasing it separately.
                CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);

                // 이미지에 출력하기 위해 이미지 드로잉 객채에서 레이어를 얻어옴 // Gets layers from image drawing object for output to image
                CGUIViewImageLayer layerDst = fliImageDrawing.GetLayer();

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layerSrc.Clear();
                layerDst.Clear();

                // 이미지에 정보 표시 // Display image information
		        layerSrc.DrawTextImage(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 30);
		        layerDst.DrawTextImage(new CFLPoint<double>(0, 0), "Destination Image", EColor.YELLOW, EColor.BLACK, 30);

		        CFLPoint<double> flpDraw = new CFLPoint<double>(10.0, 10.0);

		        layerSrc.DrawFigureImage(flpDraw, EColor.RED, 3);
		        layerDst.DrawFigureImage(flpDraw, EColor.RED, 3);

		        CFLLine<double> fllDraw = new CFLLine<double>(15.0, 15.0, 80.0, 30.0);

		        layerSrc.DrawFigureImage(fllDraw, EColor.ORANGE, 3);
		        layerDst.DrawFigureImage(fllDraw, EColor.ORANGE, 3);

		        CFLRect<double> flrDraw = new CFLRect<double>(80.0, 80.0, 150.0, 150.0);

		        layerSrc.DrawFigureImage(flrDraw, EColor.YELLOW, 3);
		        layerDst.DrawFigureImage(flrDraw, EColor.YELLOW, 3);

		        CFLQuad<double> flqDraw = new CFLQuad<double>(170.0, 170.0, 200.0, 180.0, 220.0, 210.0, 180.0, 230.0);

		        layerSrc.DrawFigureImage(flqDraw, EColor.GREEN, 3);
		        layerDst.DrawFigureImage(flqDraw, EColor.GREEN, 3);

		        CFLCircle<double> flcDraw = new CFLCircle<double>(250.0, 250.0, 50.0);
  
  		        layerSrc.DrawFigureImage(flcDraw, EColor.BLUE, 3);
  		        layerDst.DrawFigureImage(flcDraw, EColor.BLUE, 3);

                CFLEllipse<double> fleDraw = new CFLEllipse<double>(350.0, 350.0, 50.0, 80.0, 25.0);

		        layerSrc.DrawFigureImage(fleDraw, EColor.VIOLET, 3);
		        layerDst.DrawFigureImage(fleDraw, EColor.VIOLET, 3);


		        // 이미지에 그립니다. // Draw in the image.
		        if((res = fliImageDrawing.Draw()).IsFail())
				{
			        ErrorPrint(res, "Failed to draw.\n");
			        break;
		        }

		        // 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
                CFLImage fliTemp = (CFLImage)fliImageDrawing;
                if ((res = viewImageDst.SetImagePtr(ref fliTemp)).IsFail())
				{
			        ErrorPrint(res, "Failed to set image object on the image view.\n");
			        break;
		        }

		        // 이미지 뷰를 갱신 합니다. // Update image view
		        viewImageSrc.Invalidate();
		        viewImageDst.Invalidate();

		        // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
		        while(viewImageSrc.IsAvailable() || viewImageDst.IsAvailable())
			        CThreadUtilities.Sleep(1);
	        }
	        while(false);
		}
    }
}

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

namespace OperationLogicalNot
{
    class OperationLogicalNot
	{
        [STAThread]
        static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

            // 이미지 객체 선언 // Declare the image object
            CFLImage fliSourceImage = new CFLImage();
            CFLImage fliDestinationImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImageSource = new CGUIViewImage();
            CGUIViewImage viewImageDestination = new CGUIViewImage();

	        do
			{
                // 이미지 로드 // Load image
                if (fliSourceImage.Load("../../ExampleImages/OperationLogicalNot/Gambling.flif").IsFail())
				{
                    Console.WriteLine("Failed to load the image file. \n");
                    break;
                }

                // Destination 이미지를 Source 이미지와 동일한 이미지로 생성 // Create destination image as same as source image
                if (fliDestinationImage.Assign(fliSourceImage).IsFail())
				{
                    Console.WriteLine("Failed to assign the image file. \n");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if (viewImageSource.Create(100, 0, 612, 512).IsFail())
				{
                    Console.WriteLine("Failed to create the image view. \n");
                    break;
                }

                // 이미지 뷰 생성 // Create image view
                if (viewImageDestination.Create(612, 0, 1124, 512).IsFail())
				{
                    Console.WriteLine("Failed to create the image view. \n");
                    break;
                }

                // 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
                if (viewImageSource.SynchronizePointOfView(ref viewImageDestination).IsFail())
				{
                    Console.WriteLine("Failed to synchronize view. \n");
                    break;
                }

                // Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
                if (viewImageSource.SetImagePtr(ref fliSourceImage).IsFail())
				{
                    Console.WriteLine("Failed to set image object on the image view. \n");
                    break;
                }

                // Destiantion 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
                if (viewImageDestination.SetImagePtr(ref fliDestinationImage).IsFail())
				{
                    Console.WriteLine("Failed to set image object on the image view. \n");
                    break;
                }

                // 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
                if (viewImageSource.SynchronizeWindow(ref viewImageDestination).IsFail())
				{
                    Console.WriteLine("Failed to synchronize window. \n");
                    break;
                }

				// Operation Logical Not 객체 생성 // Create Logical Not object
				COperationLogicalNot LogicalNot = new COperationLogicalNot();

				// Source 이미지 설정 // Set source image
				LogicalNot.SetSourceImage(ref fliSourceImage);

				// Destination 이미지 설정 // Set destination image 
				LogicalNot.SetDestinationImage(ref fliDestinationImage);

				// Operation Logical Not 수행
				if(LogicalNot.Execute().IsFail())
				{
                    Console.WriteLine("Failed to execute OperationLogicalNot. \n");
                    break;
                }

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layerSource = viewImageSource.GetLayer(0);
                CGUIViewImageLayer layerDestination = viewImageDestination.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
                layerSource.Clear();
                layerDestination.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);

                if (layerSource.DrawTextCanvas(flpPoint, "Source Image", EColor.YELLOW, EColor.BLACK, 30).IsFail())
				{
                    Console.WriteLine("Failed to draw text. \n");
                    break;
                }

                if (layerDestination.DrawTextCanvas(flpPoint, "Destination Image", EColor.YELLOW, EColor.BLACK, 30).IsFail())
				{
                    Console.WriteLine("Failed to draw text. \n");
                    break;
                }

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImageSource.Invalidate(true);
                viewImageDestination.Invalidate(true);

                // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImageSource.IsAvailable() && viewImageDestination.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);
        }
    }
}

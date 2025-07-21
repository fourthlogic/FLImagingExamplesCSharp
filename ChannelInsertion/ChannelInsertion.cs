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

namespace ChannelInsertion
{
	class ChannelInsertion
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
			CFLImage[] fliInsertionImage = new CFLImage[2];
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewSrcImage = new CGUIViewImage();
			CGUIViewImage[] viewInsertionImage = new CGUIViewImage[2];
			CGUIViewImage viewDstImage = new CGUIViewImage();

			fliInsertionImage[0] = new CFLImage();
			fliInsertionImage[1] = new CFLImage();

			viewInsertionImage[0] = new CGUIViewImage();
			viewInsertionImage[1] = new CGUIViewImage();

			do
			{
				CResult res;
				// 이미지 로드 // Load image
				if((res = fliSourceImage.Load("../../ExampleImages/ChannelInsertion/Valley1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliInsertionImage[0].Load("../../ExampleImages/ChannelInsertion/Valley2.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliInsertionImage[1].Load("../../ExampleImages/ChannelInsertion/Valley3.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewSrcImage.Create(100, 0, 100 + 440, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewInsertionImage[0].Create(100 + 440, 0, 100 + 440 * 2, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewInsertionImage[1].Create(100 + 440 * 2, 0, 100 + 440 * 3, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewDstImage.Create(100 + 440 * 3, 0, 100 + 440 * 4, 340)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewSrcImage.SynchronizePointOfView(ref viewInsertionImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewInsertionImage[0].SynchronizePointOfView(ref viewInsertionImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. 
				if((res = viewInsertionImage[1].SynchronizePointOfView(ref viewDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewSrcImage.SynchronizeWindow(ref viewInsertionImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}
				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewInsertionImage[0].SynchronizeWindow(ref viewInsertionImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}
				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewInsertionImage[1].SynchronizeWindow(ref viewDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				if((res = viewSrcImage.SetImagePtr(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewInsertionImage[0].SetImagePtr(ref fliInsertionImage[0])).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewInsertionImage[1].SetImagePtr(ref fliInsertionImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewDstImage.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Channel Insertion 객체 생성 // Create Channel Insertion object
				CChannelInsertion channelInsertion = new CChannelInsertion();

				// 삽입 이미지를 저장할 Array 선언 // Declare an Array to store the insertion image
				List<CFLImage> vctInsertionImages = new List<CFLImage>();

				// 추출할 채널을 저장할 Array 선언 // Declare an Array to extract the channels
				List<Int64> vctInsertionChannels = new List<Int64>();

				// 삽입할 색인을 저장할 Array 선언 // Declare an Array to insert the indices
				List<Int64> vctInsertionIndices = new List<Int64>();

				// 삽입 이미지 입력 // insertion images add
				vctInsertionImages.Add(fliInsertionImage[0]);
				vctInsertionImages.Add(fliInsertionImage[1]);

				// 이미지별 추출할 채널을 입력 // channels add
				vctInsertionChannels.Add((Int64)EChannelSelection.Channel_0);
				vctInsertionChannels.Add((Int64)EChannelSelection.Channel_0);

				// 이미지별 삽입할 색인을 입력 // indices add
				vctInsertionIndices.Add(0);
				vctInsertionIndices.Add(1);

				// 소스 이미지 설정 // Set source image
				channelInsertion.SetSourceImage(ref fliSourceImage);

				// 결합할 이미지 및 채널입력 // Set images, channels
				channelInsertion.SetInsertionImage(ref vctInsertionImages, vctInsertionChannels, vctInsertionIndices);

				// 결합 결과를 저장할 이미지 설정 // Set destination image
				channelInsertion.SetDestinationImage(ref fliDstImage);

				// 알고리즘 수행 // Execute the algorithm
				if((res = (channelInsertion.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute channelInsertion.");
					break;
				}

				// 출력을 위한 이미지 레이어를 얻어옵니다. //  Gets the image layer for output.
				// 따로 해제할 필요 없음 // No need to release separately
				CGUIViewImageLayer layer1 = viewSrcImage.GetLayer(0);
				CGUIViewImageLayer layer2 = viewInsertionImage[0].GetLayer(0);
				CGUIViewImageLayer layer3 = viewInsertionImage[1].GetLayer(0);
				CGUIViewImageLayer layer4 = viewDstImage.GetLayer(0);

				CFLPoint<double> flpTemp = new CFLPoint<double>(0, 0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Delete the shapes drawn on the existing layer
				layer1.Clear();
				layer2.Clear();
				layer3.Clear();
				layer4.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
                if ((res = layer1.DrawTextImage(flpTemp, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

                if ((res = layer2.DrawTextImage(flpTemp, "Insertion Image1", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

                if ((res = layer3.DrawTextImage(flpTemp, "Insertion Image 2", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

                if ((res = layer4.DrawTextImage(flpTemp, "Insertion Image 1 +\nSource Image +\nInsertion Image 2", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					ErrorPrint(res, "Failed to draw text.\n");

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewSrcImage.Invalidate(true);
				viewInsertionImage[0].Invalidate(true);
				viewInsertionImage[1].Invalidate(true);
				viewDstImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewDstImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

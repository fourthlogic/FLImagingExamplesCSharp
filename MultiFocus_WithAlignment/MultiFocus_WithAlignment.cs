﻿using System;
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

namespace FLImagingExamplesCSharp
{
	class MultiFocus_WithAlignment
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}
		public class CMessageReceiver : CFLBase
		{
			// CMessageReceiver 생성자 // CMessageReceiver constructor
			public CMessageReceiver(ref CGUIViewImage viewImage)
			{
				m_viewImage = viewImage;

				// 메세지를 전달 받기 위해 CBroadcastManager 에 구독 등록 //Subscribe to CBroadcast Manager to receive messages
				CBroadcastManager.Subscribe(this);
			}

			// CMessageReceiver 소멸자 // CMessageReceiver Destructor
			~CMessageReceiver()
			{
				// 객체가 소멸할때 메세지 수신을 중단하기 위해 구독을 해제한다. // Unsubscribe to stop receiving messages when the object disappears.
				CBroadcastManager.Unsubscribe(this);
			}

			// 메세지가 들어오면 호출되는 함수 OnReceiveBroadcast 오버라이드하여 구현 // Implemented by overriding the function OnReceive Broadcast that is invoked when a message is received
			public override void OnReceiveBroadcast(CBroadcastMessage message)
			{
				do
				{
					// message 가 null 인지 확인 // Verify message is null
					if(message == null)
						break;

					// GetCaller() 가 등록한 이미지뷰인지 확인 // Verify that GetCaller() is a registered image view
					if(message.GetCaller() != m_viewImage)
						break;

					// 메세지의 채널을 확인 // Check the channel of the message
					switch(message.GetChannel())
					{
					case (uint)EGUIBroadcast.ViewImage_PostPageChange:
						{
							// 메세지를 호출한 객체를 CGUIViewImage 로 캐스팅 // Casting the object that called the message as CGUIViewImage
							CGUIViewImage viewImage = message.GetCaller() as CGUIViewImage;

							// viewImage 가 null 인지 확인 // Verify viewImage is null
							if(viewImage == null)
								break;

							CFLImage fliSrc = viewImage.GetImage();

							if(fliSrc == null)
								break;

							int i64CurPage = fliSrc.GetSelectedPageIndex();

							if(i64CurPage == 0)
							{
								// 이미지뷰의 3번 레이어 가져오기 // Get layer 3rd of image view
								CGUIViewImageLayer wrapImageLayer = viewImage.GetLayer(3);
								wrapImageLayer.DrawFigureImage(m_fliFirstPageAlignment, EColor.LIME, 1);

								TPoint<double> tpPoint = new TPoint<double>(m_fliFirstPageAlignment.flpPoints[0].x, m_fliFirstPageAlignment.flpPoints[0].y);

								wrapImageLayer.DrawTextImage(tpPoint, "First Page Alignment", EColor.CYAN);
							}
							else if(i64CurPage == fliSrc.GetPageCount() - 1)
							{
								// 이미지뷰의 4번 레이어 가져오기 // Get layer 4th of image view
								CGUIViewImageLayer wrapImageLayer = viewImage.GetLayer(4);
								wrapImageLayer.DrawFigureImage(m_fliLastPageAlignment, EColor.LIME, 1);

								TPoint<double> tpPoint = new TPoint<double>(m_fliLastPageAlignment.flpPoints[0].x, m_fliLastPageAlignment.flpPoints[0].y);

								wrapImageLayer.DrawTextImage(tpPoint, "Last Page Alignment", EColor.CYAN);
							}

							// 이미지뷰를 갱신 // Update the image view.
							viewImage.Invalidate();
						}
						break;
					}
				}
				while(false);
			}

			public CFLQuad<double> m_fliFirstPageAlignment = new CFLQuad<double>();
			public CFLQuad<double> m_fliLastPageAlignment = new CFLQuad<double>();
			CGUIViewImage m_viewImage;
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();

			// Message Reciever 객체 생성 // Create Message Reciever object
			CMessageReceiver msgReceiver = new CMessageReceiver(ref viewImageSrc);

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
                if ((res = fliSrcImage.Load("../../ExampleImages/MultiFocus/SourceAlignment.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				CFLQuad<double> flqFirstPageAlignment = new CFLQuad<double>();
				CFLQuad<double> flqLastPageAlignment = new CFLQuad<double>();

				if((res = flqFirstPageAlignment.Load("../../ExampleImages/MultiFocus/FirstPageAlignment.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load Source Projection Figure.");
					break;
				}

				if((res = flqLastPageAlignment.Load("../../ExampleImages/MultiFocus/LastPageAlignment.fig")).IsFail())
				{
					ErrorPrint(res, "Failed to load Destination Projection Figure.");
					break;
				}

				// 메시지 리시버에 Figure Pointer 설정 // Set Figure Point to message receiver
				msgReceiver.m_fliFirstPageAlignment = flqFirstPageAlignment;
				msgReceiver.m_fliLastPageAlignment = flqLastPageAlignment;

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(400, 0, 800, 400)).IsFail() ||
					(res = viewImageDst.Create(800, 0, 1200, 400)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageSrc.SynchronizePointOfView(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageSrc.SynchronizeWindow(ref viewImageDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail() ||
					(res = viewImageDst.SetImagePtr(ref fliDstImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				viewImageSrc.SetFixThumbnailView(true);


				// 알고리즘 객체 생성 // Create algorithm object
				CMultiFocus algObject = new CMultiFocus();

				// Source 이미지 설정 // Set the source image
				if((res = algObject.SetSourceImage(ref fliSrcImage)).IsFail())
					break;
				// Destination 이미지 설정 // Set the destination image
				if((res = algObject.SetDestinationImage(ref fliDstImage)).IsFail())
					break;
				// Kernel Size 설정 // Set the kernel size
				if((res = algObject.SetKernel(23)).IsFail())
					break;
				// 첫번째 페이지 Alignment 설정 // Set first page alignment
				if((res = algObject.SetFirstPageAlignment(flqFirstPageAlignment)).IsFail())
					break;
				// 마지막 페이지 Alignment 설정 // Set last page alignment
				if((res = algObject.SetLastPageAlignment(flqLastPageAlignment)).IsFail())
					break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.\n");
					break;
				}


				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst = viewImageDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst.Clear();

				layerSrc.SetAutoClearMode(ELayerAutoClearMode.PageChanged, false);

				// View 정보를 디스플레이 합니다. // Display View information.
				CFLPoint<double> flp = new CFLPoint<double>();
				if((res = layerSrc.DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layerDst.DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				viewImageSrc.ZoomFit();
				viewImageDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageSrc.IsAvailable() && viewImageDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

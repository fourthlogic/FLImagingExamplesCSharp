using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Numerics;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;
using FLImagingCLR.Devices;

namespace FLImagingExamplesCSharp
{
	// 카메라에서 이미지 취득 이벤트를 받기 위해 CDeviceEventImageBase 를 상속 받아서 구현
	// Inherit from CDeviceEventImageBase to receive image acquisition events from the camera.
	public class CDeviceEventImageEx : CDeviceEventImageBase
	{
        // CDeviceEventImageEx 생성자 // Constructor of CDeviceEventImageEx
        public CDeviceEventImageEx()
		{
            // 이미지를 받을 객체 생성 // Create an object to receive images.
            m_fliImage = new CFLImage();
        }

        // 취득한 이미지를 표시할 이미지 뷰를 설정하는 함수 // Sets the image view to display acquired images.
        public void SetViewImage(CGUIViewImage viewImage)
		{
            m_viewImage = viewImage;

            // 이미지 뷰에 이미지 포인터 설정 // Set the image pointer for the image view.
            m_viewImage.SetImagePtr(ref m_fliImage);
        }

        // 카메라에서 이미지 취득 시 호출 되는 함수 // Callback function called when an image is acquired from the camera.
        public override void OnAcquisition(CDeviceImageBase pDeviceImage)
		{
            // 이미지 뷰의 유효성을 확인한다. // Check the validity of the image view.
            if (m_viewImage.IsAvailable())
			{
				// 카메라에서 취득 한 이미지를 얻어온다. // Get the image acquired from the camera.
				m_fliImage.Lock();
				pDeviceImage.GetAcquiredImage(ref m_fliImage);
				m_fliImage.Unlock();

				// 이미지 뷰를 재갱신 한다. // Invalidate the image view.
				m_viewImage.Invalidate();
            }
        }

        CGUIViewImage m_viewImage;
        CFLImage m_fliImage;
    }

    class DeviceCameraEGrabber
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

            CResult res = new CResult(EResult.UnknownError);

	        // 이미지 뷰 선언 // Declare image view
	        CGUIViewImage viewImage = new CGUIViewImage();

			// EGrabber 카메라 선언 // Declare EGrabber camera

			CDeviceCameraEGrabber camEGrabber = new CDeviceCameraEGrabber();

	        do
			{
                String strInput = "";

		        int i32BoardIndex = 0;
                int i32ModuleIndex = 0;

				// 보드의 인덱스를 입력합니다. // Enter the board index.
				Console.Write("Enter board index: ");
                strInput = Console.ReadLine();

                if(Int32.TryParse(strInput, out i32BoardIndex) == false)
                    i32BoardIndex = 0;

                // 모듈의 인덱스를 입력합니다. // Enter the module index.
                Console.Write("Enter module index: ");
                strInput = Console.ReadLine();

                if (Int32.TryParse(strInput, out i32ModuleIndex) == false)
                    i32ModuleIndex = 0;

		        Console.Write("\n");

		        // 이벤트를 받을 객체 선언 // Declare an event object.
		        CDeviceEventImageEx eventImage = new CDeviceEventImageEx();

		        // 카메라에 이벤트 객체 설정 // Set the event object for the camera.
		        camEGrabber.RegisterDeviceEvent(eventImage);

		        // 카메라에 장치 설정 // Configure the camera device.
		        camEGrabber.SetBoardIndex(i32BoardIndex);
                camEGrabber.SetModuleIndex(i32ModuleIndex);

		        // 카메라를 초기화 합니다. // Initialize the camera.
		        if((res = camEGrabber.Initialize()).IsFail())
				{
			        ErrorPrint(res, "Failed to initialize the camera.\n");
			        break;
		        }

		        // 이미지 뷰 생성 // Create image view
		        if((res = viewImage.Create(0, 0, 1000, 1000)).IsFail())
				{
			        ErrorPrint(res, "Failed to create the image view.\n");
			        break;
		        }

		        eventImage.SetViewImage(viewImage);

		        // 카메라를 Live 합니다. // Start live acquisition.
		        if((res = camEGrabber.Live()).IsFail())
				{
			        ErrorPrint(res, "Failed to live the camera.\n");
			        break;
		        }

		        // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImage.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);

	        // 카메라의 초기화를 해제합니다. // Terminate the camera.
	        camEGrabber.Terminate();
        }
    }
}

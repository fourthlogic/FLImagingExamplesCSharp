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

namespace DeviceCameraHIK
{
    // 카메라에서 이미지 취득 이벤트를 받기 위해 CDeviceEventImageBase 를 상속 받아서 구현
    public class CDeviceEventImageEx : CDeviceEventImageBase
    {
        // CDeviceEventImageEx 생성자
        public CDeviceEventImageEx()
        {
            // 이미지를 받을 객체 생성 // Create 이미지를 받을 object
            m_fliImage = new CFLImage();
        }

        // 취득한 이미지를 표시할 이미지 뷰를 설정하는 함수
        public void SetViewImage(CGUIViewImage viewImage)
        {
            m_viewImage = viewImage;

            // 이미지 뷰에 이미지 포인터 설정
            m_viewImage.SetImagePtr(ref m_fliImage);
        }

        // 카메라에서 이미지 취득 시 호출 되는 함수
        public override void OnAcquisition(CDeviceImageBase pDeviceImage)
        {
            // 이미지 뷰의 유효성을 확인한다.
            if (m_viewImage.IsAvailable())
            {
				// 카메라에서 취득 한 이미지를 얻어온다.
				m_fliImage.Lock();
				pDeviceImage.GetAcquiredImage(ref m_fliImage);
				m_fliImage.Unlock();

				// 이미지 뷰를 재갱신 한다.
				m_viewImage.Invalidate();
            }
        }

        CGUIViewImage m_viewImage;
        CFLImage m_fliImage;
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CResult drResult = new CResult(EResult.UnknownError);

            // 이미지 뷰 선언 // Declare the image view
	        CGUIViewImage viewImage = new CGUIViewImage();

	        // HIK 카메라 선언
	        CDeviceCameraHIK camHik = new CDeviceCameraHIK();

	        do
	        {
		        String strInput = "";

		        CDeviceGenICamBase.EDeviceType eDeviceType = CDeviceGenICamBase.EDeviceType.GigE;
		        bool bAutoDetect = false;
		        int i32SelectDevice = -1;
		        CDeviceGenICamBase.EConnectionMethod eConnectionMethod = CDeviceGenICamBase.EConnectionMethod.SerialNumber;
		        String strConnection = "";

		        // 장치 타입을 선택합니다.
		        while(true)
		        {
			        Console.Write("1. GigE\n");
			        Console.Write("2. IEEE 1394\n");
			        Console.Write("3. USB\n");
			        Console.Write("4. Camera Link\n");
			        Console.Write("Select Device Type: ");

                    strInput = Console.ReadLine();

                    int i32Select = 0;

                    if(Int32.TryParse(strInput, out i32Select) == true)
                    {
			            bool bSelected = true;

			            switch(i32Select)
			            {
			            case 1:
				            eDeviceType = CDeviceGenICamBase.EDeviceType.GigE;
				            break;

			            case 2:
				            eDeviceType = CDeviceGenICamBase.EDeviceType.IEEE1394;
				            break;

			            case 3:
				            eDeviceType = CDeviceGenICamBase.EDeviceType.USB;
				            break;

			            case 4:
				            eDeviceType = CDeviceGenICamBase.EDeviceType.CameraLink;
				            break;

			            default:
				            bSelected = false;
				            break;
			            }

			            if(bSelected)
				            break;
                    }

			        Console.Write("Incorrect input. Please select again.\n\n");
		        }

		        Console.Write("\n");

		        // 장치 찾기 방법을 선택합니다.
		        while(true)
		        {
			        Console.Write("1. Auto Detect\n");
			        Console.Write("2. Manual\n");
			        Console.Write("Select Detection Method: ");

                    strInput = Console.ReadLine();

                    int i32Select = 0;

                    if(Int32.TryParse(strInput, out i32Select) == true)
                    {
                        bool bSelected = true;

			            switch(i32Select)
			            {
			            case 1:
				            bAutoDetect = true;
				            break;

			            case 2:
				            bAutoDetect = false;
				            break;

			            default:
				            bSelected = false;
				            break;
			            }

			            if(bSelected)
				            break;
                    }

			        Console.Write("Incorrect input. Please select again.\n\n");
		        }

		        Console.Write("\n");

		        if(bAutoDetect)
		        {
                    List<String> listSerialNumbers = null;

			        // 연결되어 있는 카메라의 시리얼 번호를 얻는다.
			        switch(eDeviceType)
			        {
			        case CDeviceGenICamBase.EDeviceType.GigE:
				        drResult = camHik.GetAutoDetectGigECameraSerialNumbers(out listSerialNumbers);
				        break;

			        case CDeviceGenICamBase.EDeviceType.IEEE1394:
                        drResult = camHik.GetAutoDetectIEEE1394CameraSerialNumbers(out listSerialNumbers);
				        break;

			        case CDeviceGenICamBase.EDeviceType.USB:
                        drResult = camHik.GetAutoDetectUSBCameraSerialNumbers(out listSerialNumbers);
				        break;

			        case CDeviceGenICamBase.EDeviceType.CameraLink:
                        drResult = camHik.GetAutoDetectCameraLinkCameraSerialNumbers(out listSerialNumbers);
				        break;

			        default:
				        break;
			        }

			        if(drResult.IsFail() || listSerialNumbers == null || listSerialNumbers.Count == 0)
			        {
                        drResult = new CResult(EResult.FailedToRead);
				        Console.Write("Not Found Device.\n");
				        break;
			        }

			        // 연결 할 카메라를 선택합니다.
			        while(true)
			        {
				        for(int i = 0; i < listSerialNumbers.Count; ++i)
				        {
                            String strElement = String.Format("{0}. ", i + 1);
                            strElement += listSerialNumbers[i] + "\n";

					        Console.Write(strElement);
				        }

				        Console.Write("Select Device: ");

                        strInput = Console.ReadLine();

                        int i32Select = 0;

                        if(Int32.TryParse(strInput, out i32Select) == true)
                        {
                            --i32Select;

                            if(i32Select >= 0 && i32Select < listSerialNumbers.Count)
				            {
                                i32SelectDevice = i32Select;
				                break;
				            }
                        }
        
                        Console.Write("Incorrect input. Please select again.\n\n");
			        }
		        }
		        else
		        {
			        if(eDeviceType == CDeviceGenICamBase.EDeviceType.GigE)
			        {
				        // 연결 방법을 선택합니다.
				        while(true)
				        {
					        Console.Write("1. Serial Number\n");
					        Console.Write("2. IP Address\n");
					        Console.Write("Select Connection Method: ");

                            strInput = Console.ReadLine();

                            int i32Select = 0;
                            if(Int32.TryParse(strInput, out i32Select) == true)
                            {
                                bool bSelected = true;

					            switch(i32Select)
					            {
					            case 1:
						            eConnectionMethod = CDeviceGenICamBase.EConnectionMethod.SerialNumber;
						            break;

					            case 2:
						            eConnectionMethod = CDeviceGenICamBase.EConnectionMethod.IPAddress;
						            break;

					            default:
						            bSelected = false;
						            break;
					            }

					            if(bSelected)
						            break;
                            }

					        Console.Write("Incorrect input. Please select again.\n\n");
				        }

				        Console.Write("\n");
			        }
			        else
				        eConnectionMethod = CDeviceGenICamBase.EConnectionMethod.SerialNumber;

			        // 시리얼 번호 혹은 IP 주소를 입력 받는다.
			        if(eConnectionMethod == CDeviceGenICamBase.EConnectionMethod.SerialNumber)
				        Console.Write("Input Serial Number: ");
			        else
				        Console.Write("Input IP Address: ");

                    strConnection = Console.ReadLine();
		        }

		        // 이벤트를 받을 객체 선언
		        CDeviceEventImageEx eventImage = new CDeviceEventImageEx();

		        // 카메라에 이벤트 객체 설정
		        camHik.RegisterDeviceEvent(eventImage);

		        // 카메라에 장치 타입 설정
		        camHik.SetDeviceType(eDeviceType);

		        // 인덱스에 해당하는 카메라로 연결을 설정한다.
		        if(bAutoDetect)
		        {
			        switch(eDeviceType)
			        {
			        case CDeviceGenICamBase.EDeviceType.GigE:
				        drResult = camHik.AutoDetectGigECamera(i32SelectDevice);
				        break;

			        case CDeviceGenICamBase.EDeviceType.IEEE1394:
                        drResult = camHik.AutoDetectIEEE1394Camera(i32SelectDevice);
				        break;

			        case CDeviceGenICamBase.EDeviceType.USB:
                        drResult = camHik.AutoDetectUSBCamera(i32SelectDevice);
				        break;

			        case CDeviceGenICamBase.EDeviceType.CameraLink:
                        drResult = camHik.AutoDetectCameraLinkCamera(i32SelectDevice);
				        break;

			        default:
				        break;
			        }
		        }
		        else
		        {
			        // 장치 연결 방법을 설정합니다.
			        camHik.SetConnectionMethod(eConnectionMethod);

			        if(eConnectionMethod == CDeviceGenICamBase.EConnectionMethod.SerialNumber)
				        // 시리얼 번호를 설정합니다.
				        camHik.SetSerialNumber(strConnection);
			        else
				        // IP 주소를 설정합니다.
                        camHik.SetIPAddress(strConnection);
		        }

		        // 카메라를 초기화 합니다.
                drResult = camHik.Initialize();
                if (drResult.IsFail())
		        {
			        Console.Write("Failed to initialize the camera.\n");
			        break;
		        }

                // 이미지 뷰 생성 // Create image view
                if (viewImage.Create(0, 0, 1000, 1000).IsFail())
                {
                    drResult = new CResult(EResult.FailedToCreateObject);
                    Console.Write("Failed to create the image view.\n");
                    break;
                }

                eventImage.SetViewImage(viewImage);

		        // 카메라를 Live 합니다.
                drResult = camHik.Live();
                if (drResult.IsFail())
		        {
			        Console.Write("Failed to live the camera.\n");
			        break;
		        }

		        // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
		        while(viewImage.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);

	        // 카메라의 초기화를 해제합니다.
	        camHik.Terminate();
			camHik.ClearDeviceEvents();

			if (drResult.IsFail())
                Console.ReadLine();
        }
    }
}

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

namespace DeviceCameraMechEye
{
    // 카메라에서 이미지 취득 이벤트를 받기 위해 CDeviceEventImageBase 를 상속 받아서 구현
    public class CDeviceEventImageEx : CDeviceEventImageBase
    {
        // CDeviceEvent3DEx 생성자
        public CDeviceEventImageEx()
        {
        }

        // 취득한 이미지를 표시할 이미지 뷰를 설정하는 함수
        public void SetView3D(CGUIView3D view3D)
        {
            m_view3D = view3D;
        }

        // 카메라에서 이미지 취득 시 호출 되는 함수
        public override void OnAcquisition(CDeviceImageBase pDeviceImage)
        {
            do
            {
	            if(m_view3D == null)
		            break;

	            // 3D 뷰의 유효성을 확인한다.
	            if(!m_view3D.IsAvailable())
		            break;

                CDeviceCameraMechEye camera = pDeviceImage as CDeviceCameraMechEye;

	            if(camera == null)
		            break;

	            // 데이터 객체 선언
	            CFL3DObject floData = new CFL3DObject();

	            // 카메라에서 취득 한 데이터를 얻어온다.
	            camera.GetAcquired3DData(ref floData);

                if(floData == null)
                    break;

	            // 3D 뷰의 업데이트를 막습니다.
	            m_view3D.LockUpdate();

	            // 3D 뷰의 유효성을 확인한다.
	            if(!m_view3D.IsAvailable())
		            break;

	            // 3D 뷰의 객체 개수를 얻어옵니다.
	            int i32ObjectCount = m_view3D.GetObjectCount();

	            // 3D 뷰의 객체들을 모두 클리어합니다.
	            m_view3D.ClearObjects();

	            // 3D 뷰의 유효성을 확인한다.
	            if(!m_view3D.IsAvailable())
		            break;

	            // 3D 뷰에 객체를 추가합니다.
	            m_view3D.PushObject(floData);

	            // 3D 뷰의 유효성을 확인한다.
	            if(!m_view3D.IsAvailable())
		            break;

	            // 3D 뷰의 업데이트 막은 것을 해제합니다.
	            m_view3D.UnlockUpdate();

	            // 3D 뷰의 스케일을 조정합니다.
	            if(i32ObjectCount == 0)
		            m_view3D.ZoomFit();
            }
            while(false);
        }

        CGUIView3D m_view3D;
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CResult drResult = new CResult(EResult.UnknownError);

            // 이미지 뷰 선언 // Declare the 3D view
	        CGUIView3D view3D = new CGUIView3D();

	        // MechEye 카메라 선언
	        CDeviceCameraMechEye camMechEye = new CDeviceCameraMechEye();

	        do
	        {
		        String strInput = "";
		        bool bAutoDetect = false;
		        int i32SelectDevice = -1;
		        String strConnection = "";

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
                    List<String> listSerialNumbers = new List<String>();

                    // 연결되어 있는 카메라의 시리얼 번호를 얻는다.
                    drResult = camMechEye.GetAutoDetectCameraSerialNumbers(ref listSerialNumbers);

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
			        // 시리얼 번호를 입력 받는다.
                    Console.Write("Input Serial Number: ");

                    strConnection = Console.ReadLine();
		        }

		        // 이벤트를 받을 객체 선언
                CDeviceEventImageEx eventImage = new CDeviceEventImageEx();

		        // 카메라에 이벤트 객체 설정
                camMechEye.RegisterDeviceEvent(eventImage);

		        // 인덱스에 해당하는 카메라로 연결을 설정한다.
		        if(bAutoDetect)
                    // 인덱스에 해당하는 카메라로 연결을 설정합니다.
                    drResult = camMechEye.AutoDetectCamera(i32SelectDevice);
		        else
                    // 시리얼 번호를 설정합니다.
                    camMechEye.SetSerialNumber(strConnection);

		        // 카메라를 초기화 합니다.
                drResult = camMechEye.Initialize();

                if (drResult.IsFail())
		        {
			        Console.Write("Failed to initialize the camera.\n");
			        break;
		        }

                // 이미지 뷰 생성 // Create 3D view
                if (view3D.Create(0, 0, 1000, 1000).IsFail())
                {
                    drResult = new CResult(EResult.FailedToCreateObject);
                    Console.Write("Failed to create the 3D view.\n");
                    break;
                }

                eventImage.SetView3D(view3D);

		        // 카메라를 Live 합니다.
                drResult = camMechEye.Live();

                if (drResult.IsFail())
		        {
			        Console.Write("Failed to live the camera.\n");
			        break;
		        }

		        // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the 3D view to close
		        while(view3D.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);

	        // 카메라의 초기화를 해제합니다.
	        camMechEye.Terminate();
			camMechEye.ClearDeviceEvents();

            if (drResult.IsFail())
                Console.ReadLine();
        }
    }
}

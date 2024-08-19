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

namespace DeviceCameraMatrox
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
        }

        // 카메라에서 이미지 취득 시 호출 되는 함수
        public override void OnAcquisition(CDeviceImageBase pDeviceImage)
        {
            // 이미지 뷰의 유효성을 확인한다.
            if (m_viewImage.IsAvailable())
            {
                // 카메라에서 취득 한 이미지를 얻어온다.
                pDeviceImage.GetAcquiredImage(ref m_fliImage);

                if (m_viewImage.GetImage() != m_fliImage)
                {
                    // 이미지 뷰에 이미지 포인터 설정
                    m_viewImage.SetImagePtr(ref m_fliImage);
                }

                // 이미지 뷰를 재갱신 한다.
                m_viewImage.Invalidate();
            }
        }

        CGUIViewImage m_viewImage;
        CFLImage m_fliImage;
    }

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
            CResult res = new CResult(EResult.UnknownError);

	        // 이미지 뷰 선언 // Declare image view
	        CGUIViewImage viewImage = new CGUIViewImage();

	        // Matrox 카메라 선언
	        CDeviceCameraMatrox camMatrox = new CDeviceCameraMatrox();

	        do
	        {
                String strInput = "";

                String strCamfilePath = "";
		        CDeviceCameraMatrox.EDeviceType eDeviceType = CDeviceCameraMatrox.EDeviceType.Unknown;
		        int i32DeviceIndex = 0;
		        int i32ModuleIndex = 0;

                // Cam file 의 전체 경로를 입력합니다.
                Console.Write("Enter camfile full path (e.g. C:/Camfile/AnyCamfile.cam): ");
                strCamfilePath = Console.ReadLine();

                // 장치 타입을 입력합니다.
                Console.Write("Device type\n");
                Console.Write("1.Clarity UHD\t\t2.Concord POE\t\t3.GenTL\n");
                Console.Write("4.GevIQ\t\t\t5.GigE\t\t\t6.Host\n");
                Console.Write("7.Indio\t\t\t8.Iris GTX\t\t9.Morphis\n");
                Console.Write("10.Radient eV-CXP\t11.Radient eV-CL\t12.Rapixo Pro CL\n");
                Console.Write("13.Rapixo CXP\t\t14.Solios\t\t15.USB3\n");
                Console.Write("Enter device type: ");
                strInput = Console.ReadLine();

                int i32DeviceType = 0;

                if (Int32.TryParse(strInput, out i32DeviceType) == true)
                    eDeviceType = (CDeviceCameraMatrox.EDeviceType)i32DeviceType;

                // 장치의 인덱스를 입력합니다.
                Console.Write("Enter device index: ");
                strInput = Console.ReadLine();

                if(Int32.TryParse(strInput, out i32DeviceIndex) == false)
                    i32DeviceIndex = 0;

                // 모듈의 인덱스를 입력합니다.
                Console.Write("Enter module index: ");
                strInput = Console.ReadLine();

                if (Int32.TryParse(strInput, out i32ModuleIndex) == false)
                    i32ModuleIndex = 0;

		        Console.Write("\n");

		        // 이벤트를 받을 객체 선언
		        CDeviceEventImageEx eventImage = new CDeviceEventImageEx();

		        // 카메라에 이벤트 객체 설정
		        camMatrox.RegisterDeviceEvent(eventImage);

		        // 카메라에 장치 설정
                camMatrox.SetCamFilePath(strCamfilePath);
                camMatrox.SetDeviceType(eDeviceType);
                camMatrox.SetDeviceIndex(i32DeviceIndex);
                camMatrox.SetModuleIndex(i32ModuleIndex);

		        // 카메라를 초기화 합니다.
		        if((res = camMatrox.Initialize()).IsFail())
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

		        // 카메라를 Live 합니다.
		        if((res = camMatrox.Live()).IsFail())
		        {
			        ErrorPrint(res, "Failed to live the camera.\n");
			        break;
		        }

		        // 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
                while (viewImage.IsAvailable())
                    Thread.Sleep(1);
	        }
	        while(false);

	        // 카메라의 초기화를 해제합니다.
	        camMatrox.Terminate();
        }
    }
}

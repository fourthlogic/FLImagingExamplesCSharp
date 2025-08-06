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
	// 프로파일 데이터 취득 이벤트를 받기 위해 CDeviceEventProfileBase 를 상속 받아서 구현
	// Inherit and implement CDeviceEventProfileBase to receive profile data acquisition events
	public class CDeviceEventProfileEx : CDeviceEventProfileBase
	{
		// CDeviceEventProfileEx 생성자 // CDeviceEventProfileEx Constructor
		public CDeviceEventProfileEx()
		{
			// 높이 데이터 프로파일 이미지를 받을 객체 생성
			// Create an object to receive the height data profile image
			m_fliHeight = new CFLImage();
			// 휘도 데이터 프로파일 이미지를 받을 객체 생성
			// Create an object to receive the luminance data profile image
			m_fliLuminance = new CFLImage();
		}

		// 취득한 높이 이미지를 표시할 이미지 뷰를 설정하는 함수
		// Function to set the image view to display the acquired height image
		public void SetViewHeightImage(CGUIViewImage viewHeightImage)
		{
			m_viewHeightImage = viewHeightImage;

			// 이미지 뷰에 이미지 포인터 설정
			m_viewHeightImage.SetImagePtr(ref m_fliHeight);
		}

		// 취득한 휘도 이미지를 표시할 이미지 뷰를 설정하는 함수
		// Function to set the image view to display the acquired luminance image
		public void SetViewLuminanceImage(CGUIViewImage viewHeightImage)
		{
			m_viewLuminanceImage = viewHeightImage;

			// 이미지 뷰에 이미지 포인터 설정
			m_viewLuminanceImage.SetImagePtr(ref m_fliLuminance);
		}

		// 카메라에서 이미지 취득 시 호출 되는 함수
		public override void OnAcquisition(CDeviceProfileBase pDeviceImage)
		{
			// 이미지 뷰의 유효성을 확인한다. // Check the validity of the image view.
			if(m_viewHeightImage != null && m_viewHeightImage.IsAvailable())
			{
				// 센서에서 취득 한 프로파일 이미지를 얻어온다. // Retrieves the profile image acquired from the sensor.
				m_fliHeight.Lock();
				pDeviceImage.GetAcquiredHeightProfile(ref m_fliHeight);
				m_fliHeight.Unlock();

				// 이미지 뷰를 재갱신 한다. // Invalidate the image view.
				m_viewHeightImage.Invalidate();
			}

			// 이미지 뷰의 유효성을 확인한다. // Check the validity of the image view.
			if(m_viewLuminanceImage != null && m_viewLuminanceImage.IsAvailable())
			{
				// 센서에서 취득 한 프로파일 이미지를 얻어온다. // Retrieves the profile image acquired from the sensor.
				m_fliLuminance.Lock();
				pDeviceImage.GetAcquiredLuminanceProfile(ref m_fliLuminance);
				m_fliLuminance.Unlock();

				// 이미지 뷰를 재갱신 한다. // Invalidate the image view.
				m_viewLuminanceImage.Invalidate();
			}
		}

		CGUIViewImage m_viewHeightImage;
		CGUIViewImage m_viewLuminanceImage;
		CFLImage m_fliHeight;
		CFLImage m_fliLuminance;
	}

	class DeviceCameraArena
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

			CResult res = new CResult(EResult.UnknownError);

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewHeightImage = new CGUIViewImage();
			CGUIViewImage viewLuminanceImage = new CGUIViewImage();

			// Laser Profile Sensor 선언 // Laser Profile Sensor Declaration
			CDeviceLaserProfileSensorKeyence devLaserProfile = new CDeviceLaserProfileSensorKeyence();

			// 이벤트를 받을 객체 선언 // Declare an object to receive events
			CDeviceEventProfileEx eventProfile = new CDeviceEventProfileEx();

			// 센서에 이벤트 객체 설정 // Set event object on sensor
			devLaserProfile.RegisterDeviceEvent(eventProfile);

			do
			{
				String strConnection = "";

				// IP 주소를 입력 받습니다. // Enter the IP address.
				while(true)
				{
					Console.Write("Input IP Address: ");

					strConnection = Console.ReadLine();

					if((res = devLaserProfile.SetConnectionIPAddress(strConnection)).IsOK())
						break;

					ErrorPrint(res, "Failed to set IP Address.\n");
				}

				// 포트 번호를 입력 받습니다. // Enter the port number.
				while(true)
				{
					Console.Write("Input Port Number: ");

					strConnection = Console.ReadLine();

					ushort u16Port = ushort.Parse(strConnection);

					if((res = devLaserProfile.SetPortNumber(u16Port)).IsOK())
						break;

					ErrorPrint(res, "Failed to set port number.\n");
				}

				// 고속 통신용 포트 번호를 입력 받습니다. // Enter the port number for high-speed communication.
				while(true)
				{
					Console.Write("Input High-speed Port Number: ");

					strConnection = Console.ReadLine();

					ushort u16Port = ushort.Parse(strConnection);

					if((res = devLaserProfile.SetHighSpeedPort(u16Port)).IsOK())
						break;

					ErrorPrint(res, "Failed to set high-speed port number.\n");
				}

				// Profile 수를 입력 받습니다. // Enter the profile number.
				while(true)
				{
					Console.Write("Input Profile Count: ");

					strConnection = Console.ReadLine();

					int i32ProfileCount = int.Parse(strConnection);

					if((res = devLaserProfile.SetProfileCount(i32ProfileCount)).IsOK())
						break;

					ErrorPrint(res, "Failed to set profile count.\n");
				}

				// 프로파일 센서를 초기화 합니다. // Initialize the profile sensor.
				if((res = devLaserProfile.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize device.\n");
					break;
				}

				// 높이 이미지 뷰 생성 // Create height image view
				if((res = viewHeightImage.Create(400, 0, 812, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 휘도 이미지 뷰 생성 // Create luminance image view
				if((res = viewLuminanceImage.Create(812, 0, 1224, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 각 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each image view.
				if((res = viewHeightImage.SynchronizePointOfView(ref viewLuminanceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}
				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((res = viewHeightImage.SynchronizeWindow(ref viewLuminanceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 이벤트 객체에 View를 설정합니다. // Set a View on the event object.
				eventProfile.SetViewHeightImage(viewHeightImage);
				eventProfile.SetViewLuminanceImage(viewLuminanceImage);

				// 활성 프로그램 No. 를 전환합니다. // Change active program number.
				devLaserProfile.SetProgramNumber(0);
				// 설정 함수의 적용 범위를 설정합니다. // Set the setting depth of the setting function.
				devLaserProfile.SetSettingDepth(CDeviceLaserProfileSensorKeyence.ESettingDepth.Running);

				// 파라미터 설정 함수 - Keyence 사의 LJ X Navigator 설치 후 C:\Program Files\KEYENCE\LJ-X Navigator\lib\Manual 경로의 매뉴얼 11.3 참고
				// Parameter setting function - Refer to section 11.3 of the manual located at C:\Program Files\KEYENCE\LJ-X Navigator\lib\Manual after installing Keyence's LJ-X Navigator.

				// Trigger 모드 설정(0 : 연속 트리거, 1 : 외부 트리거, 2 : 인코더 트리거)
				// Trigger mode setting (0: continuous trigger, 1: external trigger, 2: encoder trigger)
				{
					long i64DataSize = 4;
					byte[] arrData = new byte[i64DataSize];

					arrData[0] = 0;

					devLaserProfile.SetTriggerSetting(CDeviceLaserProfileSensorKeyence.ETriggerSettingItem.TriggerMode, arrData);
				}

				// 배치 측정 설정(0 : 배치 OFF, 1 : 배치 ON)
				// Batch measurement settings (0: batch OFF, 1: batch ON)
				{
					long i64DataSize = 4;
					byte[] arrData = new byte[i64DataSize];

					arrData[0] = 0;

					devLaserProfile.SetTriggerSetting(CDeviceLaserProfileSensorKeyence.ETriggerSettingItem.BatchMeasurement, arrData);
				}

				// 휘도 출력 설정(0 : 높이 데이터만, 1 : 높이 + 휘도 데이터)
				// Luminance output settings (0: height data only, 1: height + luminance data)
				{
					long i64DataSize = 4;
					byte[] arrData = new byte[i64DataSize];

					arrData[0] = 1;

					devLaserProfile.SetCommonSetting(CDeviceLaserProfileSensorKeyence.ECommonSettingItem.LuminanceOutput, arrData);
				}

				// 프로파일 센서를 Start 합니다. // Start the profile sensor.
				if((res = devLaserProfile.Start()).IsFail())
				{
					ErrorPrint(res, "Failed to start the profile sensor.\n");
					break;
				}

				CThreadUtilities.Sleep(100);
				viewHeightImage.ZoomFit();

				// 화면에 출력하기 위해 Image View 에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerHeight = new CGUIViewImageLayer();
				CGUIViewImageLayer layerLuminance = new CGUIViewImageLayer();

				// 각각의 image View 에서 0번 레이어 가져오기 // Get Layer 0 from each image view 
				layerHeight = viewHeightImage.GetLayer(0);
				layerLuminance = viewLuminanceImage.GetLayer(0);

				// 각 레이어 캔버스에 텍스트 그리기 // Draw text to each Layer Canvas
				layerHeight.DrawTextCanvas(new CFLPoint<Int32>(0, 0), "Height", EColor.YELLOW, EColor.BLACK, 30);
				layerLuminance.DrawTextCanvas(new CFLPoint<Int32>(0, 0), "Luminance", EColor.YELLOW, EColor.BLACK, 30);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewHeightImage.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);

			// 프로파일 센서의 초기화를 해제합니다. // Uninitialize the profile sensor.
			devLaserProfile.Terminate();
			devLaserProfile.ClearDeviceEvents();

			if(res.IsFail())
				Console.ReadLine();
		}
	}
}

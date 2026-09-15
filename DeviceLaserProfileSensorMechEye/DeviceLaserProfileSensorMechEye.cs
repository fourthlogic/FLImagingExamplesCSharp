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
	// 프로파일 센서에서 이미지 취득 이벤트를 받기 위해 CDeviceEventProfileBase 를 상속 받아서 구현
	// Inherit from CDeviceEventProfileBase to receive image acquisition events from the profile sensor.
	public class CDeviceEventProfileEx : CDeviceEventProfileBase
	{
		// CDeviceEventProfileEx 생성자 // Constructor of CDeviceEventProfileEx
		public CDeviceEventProfileEx()
		{
		}

		// 취득한 이미지를 표시할 이미지 뷰를 설정하는 함수 // Sets the image view to display acquired images.
		public void SetView3D(CGUIView3D view3D)
		{
			m_view3D = view3D;
		}

		// 프로파일 센서에서 이미지 취득 시 호출 되는 함수 // Callback function called when an image is acquired from the profile sensor.
		public override void OnAcquisition(CDeviceProfileBase pDeviceProfile)
		{
			do
			{
				if(m_view3D == null)
					break;

				// 3D 뷰의 유효성을 확인한다. // Check the validity of the 3D view.
				if(!m_view3D.IsAvailable())
					break;

				CDeviceLaserProfileSensorMechEyeBase sensor = pDeviceProfile as CDeviceLaserProfileSensorMechEyeBase;

				if(sensor == null)
					break;

				// 데이터 객체 선언 // Declare a 3D data object.
				CFL3DObject floData = new CFL3DObject();

				// 프로파일 센서에서 취득 한 데이터를 얻어온다. // Get the 3D data acquired from the profile sensor.
				sensor.GetAcquired3DData(ref floData);

				if(floData == null)
					break;

				// 3D 뷰의 업데이트를 막습니다. // Lock updates to the 3D view.
				m_view3D.LockUpdate();

				// 3D 뷰의 유효성을 확인한다. // Check the validity of the 3D view.
				if(!m_view3D.IsAvailable())
					break;

				// 3D 뷰의 객체 개수를 얻어옵니다. // Get the number of objects in the 3D view.
				int i32ObjectCount = m_view3D.GetObjectCount();

				// 3D 뷰의 객체들을 모두 클리어합니다. // Clear all objects in the 3D view.
				m_view3D.ClearObjects();

				// 3D 뷰의 유효성을 확인한다. // Check the validity of the 3D view.
				if(!m_view3D.IsAvailable())
					break;

				// 3D 뷰에 객체를 추가합니다. // Push objects to the 3D view.
				m_view3D.PushObject(floData);

				// 3D 뷰의 유효성을 확인한다. // Check the validity of the 3D view.
				if(!m_view3D.IsAvailable())
					break;

				// 3D 뷰의 업데이트 막은 것을 해제합니다. // Unlock 3D view updates.
				m_view3D.UnlockUpdate();

				// 3D 뷰의 스케일을 조정합니다. // Adjust the scale of the 3D view.
				if(i32ObjectCount == 0)
					m_view3D.ZoomFit();
			}
			while(false);
		}

		CGUIView3D m_view3D;
	}

	class DeviceLaserProfileSensorMechEye
	{
		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			CResult drResult = new CResult(EResult.UnknownError);

			// 이미지 뷰 선언 // Declare the 3D view
			CGUIView3D view3D = new CGUIView3D();

			// MechEye 프로파일 센서 선언 // Declare MechEye profile sensor
			CDeviceLaserProfileSensorMechEye_2_6_0 sensor = new CDeviceLaserProfileSensorMechEye_2_6_0();

			do
			{
				String strInput = "";
				bool bAutoDetect = false;
				int i32SelectDevice = -1;
				String strConnection = "";

				// 장치 찾기 방법을 선택합니다. // Select the detection method
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

					// 연결되어 있는 프로파일 센서의 시리얼 번호를 얻는다. // Get the serial number of the connected profile sensor.
					drResult = sensor.GetAutoDetectSerialNumbers(ref listSerialNumbers);

					if(drResult.IsFail() || listSerialNumbers == null || listSerialNumbers.Count == 0)
					{
						drResult = new CResult(EResult.FailedToRead);
						Console.Write("Not Found Device.\n");
						break;
					}

					// 연결 할 프로파일 센서를 선택합니다. // Select the profile sensor to connect to.
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
					// 시리얼 번호를 입력 받습니다. // Enter the serial number.
					Console.Write("Input Serial Number: ");

					strConnection = Console.ReadLine();
				}

				// 이벤트를 받을 객체 선언 // Declare an event object.
				CDeviceEventProfileEx eventProfile = new CDeviceEventProfileEx();

				// 프로파일 센서에 이벤트 객체 설정 // Set the event object for the profile sensor.
				sensor.RegisterDeviceEvent(eventProfile);

				if(bAutoDetect)
					// 인덱스에 해당하는 프로파일 센서로 연결을 설정합니다. // Set the profile sensor specified by the index for connection.
					drResult = sensor.AutoDetect(i32SelectDevice);
				else
					// 시리얼 번호를 설정합니다. // Set the serial number.
					sensor.SetSerialNumber(strConnection);

				// 프로파일 센서를 초기화 합니다. // Initialize the profile sensor.
				drResult = sensor.Initialize();

				if(drResult.IsFail())
				{
					Console.Write("Failed to initialize the profile sensor.\n");
					break;
				}

				// 이미지 뷰 생성 // Create 3D view
				if(view3D.Create(0, 0, 1000, 1000).IsFail())
				{
					drResult = new CResult(EResult.FailedToCreateObject);
					Console.Write("Failed to create the 3D view.\n");
					break;
				}

				eventProfile.SetView3D(view3D);

				// 프로파일 센서를 Start 합니다. // Start acquisition.
				drResult = sensor.Start();

				if(drResult.IsFail())
				{
					Console.Write("Failed to start the profile sensor.\n");
					break;
				}

				// 프로파일 센서에 소프트웨어 트리거를 발생합니다. // Triggers the profile sensor using a software trigger.
				drResult = sensor.Trigger();

				if(drResult.IsFail())
				{
					Console.Write("Failed to trigger the profile sensor.\n");
					break;
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the 3D view to close
				while(view3D.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);

			// 프로파일 센서의 초기화를 해제합니다. // Terminate the profile sensor.
			sensor.Terminate();
			sensor.ClearDeviceEvents();

			if(drResult.IsFail())
				Console.ReadLine();
		}
	}
}

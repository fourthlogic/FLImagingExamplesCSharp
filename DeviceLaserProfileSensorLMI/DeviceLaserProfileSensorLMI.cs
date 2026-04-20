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
			// 이미지를 받을 객체 생성
			// Create an object to receive the image
			m_fliImage = new CFLImage();
		}

		// 취득한 이미지를 표시할 이미지 뷰를 설정하는 함수
		// Function to set the image view to display the acquired image
		public void SetViewImage(CGUIViewImage viewImage)
		{
			m_viewImage = viewImage;

			// 이미지 뷰에 이미지 참조 설정 // Set image reference to the image view
			m_viewImage.SetImagePtr(ref m_fliImage);
		}

		// 취득한 3D 데이터를 표시할 3D 뷰를 설정하는 함수
		// Function to set the 3D view to display the acquired 3D data
		public void SetView3D(CGUIView3D view3D)
		{
			m_view3D = view3D;
		}

		// 카메라에서 이미지 취득 시 호출 되는 함수 // Function called when acquiring an image from the camera
		public override void OnAcquisition(CDeviceProfileBase pDeviceImage)
		{
			do
			{
				CDeviceLaserProfileSensorLMI pDeviceLMI = pDeviceImage as CDeviceLaserProfileSensorLMI;

				if(pDeviceLMI == null)
					break;

				if(m_viewImage == null)
					break;

				if(m_view3D == null)
					break;

				// 스캔 모드를 얻어옵니다. // Get the scan mode.
				CDeviceLaserProfileSensorLMI.EScanMode eScanMode = CDeviceLaserProfileSensorLMI.EScanMode.Image;

				pDeviceLMI.GetScanMode(ref eScanMode);

				switch(eScanMode)
				{
				case CDeviceLaserProfileSensorLMI.EScanMode.Image:
					{
						// 이미지 뷰의 유효성을 확인합니다. // Validate the image view.
						if(!m_viewImage.IsAvailable())
							break;

						if(m_fliImage == null)
							break;

						// 기존 이미지 정보를 얻어옵니다. // Retrieve the existing image information.
						long i64Width = m_fliImage.GetWidth();
						long i64Height = m_fliImage.GetHeight();
						int i32SelectedPageIndex = m_fliImage.GetSelectedPageIndex();
						bool bZoomFit = false;

						m_fliImage.Lock();

						// 취득 한 이미지를 얻어온다. //Retrieve the acquired image.
						pDeviceLMI.GetAcquiredImage(ref m_fliImage);

						// 기존 선택된 페이지 인덱스로 선택합니다. // Select the page using the existing selected page index.
						m_fliImage.SelectPage(i32SelectedPageIndex);

						// 기존 이미지 정보와 비교합니다. // Compare with the existing image information.
						if(i64Width != m_fliImage.GetWidth() || i64Height != m_fliImage.GetHeight())
							bZoomFit = true;

						m_fliImage.Unlock();

						// 뷰를 Zoom fit 합니다. // Fit the view to the window.
						if(bZoomFit)
							m_viewImage.ZoomFit();

						// 이미지 뷰를 재갱신 한다. // Invalidate the image view.
						m_viewImage.Invalidate();
					}
					break;

				case CDeviceLaserProfileSensorLMI.EScanMode.Profile:
					{
						// 프로파일 데이터를 얻어올 리스트를 선언합니다. // Declare an list to store profile data.
						List<List<List<double>>> listProfile = new List<List<List<double>>>();

						// 취득한 프로파일 데이터를 얻어온다. // Retrieve the acquired profile data.
						pDeviceLMI.GetAcquiredProfile(ref listProfile);

						// 프로파일 데이터를 표시합니다. // Display the profile data.
						StringBuilder stringBuilderProfile = new StringBuilder();

						for(int i = 0; i < listProfile.Count; ++i)
						{
							if(i != 0)
								stringBuilderProfile.Append("\n");

							stringBuilderProfile.Append("[");

							for(int j = 0; j < listProfile[i].Count; ++j)
							{
								if(j != 0)
									stringBuilderProfile.Append("\n");

								stringBuilderProfile.Append("{");

								for(int k = 0; k < listProfile[i][j].Count; ++k)
								{
									if(k != 0)
										stringBuilderProfile.Append(", ");

									stringBuilderProfile.AppendFormat("({0},{1},{2})", listProfile[i][j][(int)CDeviceLaserProfileSensorLMI.EProfileDataElement.PositionX], listProfile[i][j][(int)CDeviceLaserProfileSensorLMI.EProfileDataElement.PositionZ], listProfile[i][j][(int)CDeviceLaserProfileSensorLMI.EProfileDataElement.Intensity]);
								}

								stringBuilderProfile.Append("}");
							}

							stringBuilderProfile.Append("]");
						}

						Console.Write(stringBuilderProfile.ToString());
					}
					break;

				case CDeviceLaserProfileSensorLMI.EScanMode.Surface:
					{
						// 3D 뷰의 유효성을 확인합니다. // Validate the 3D view.
						if(!m_view3D.IsAvailable())
							break;

						// 3D 데이터를 얻어올 객체를 선언합니다. // Declare an object to store 3D data.
						CFL3DObjectGroup flogData = new CFL3DObjectGroup();

						// 취득한 3D 데이터를 얻어온다. // Retrieve the acquired 3D data.
						pDeviceLMI.GetAcquired3DData(ref flogData);

						// 3D 뷰의 유효성을 확인합니다. // Validate the 3D view.
						if(!m_view3D.IsAvailable())
							break;

						// 3D 데이터를 3D 뷰에 표시합니다. // Display the 3D data in the 3D view.
						m_view3D.LockUpdate();

						int i32ObjectCount = m_view3D.GetObjectCount();

						m_view3D.ClearObjects();

						for(long i = 0; i < flogData.GetObjectCount(); ++i)
							m_view3D.PushObject(flogData.GetObjectByIndex(i));

						m_view3D.UnlockUpdate();

						if(i32ObjectCount == 0)
							m_view3D.ZoomFit();
					}
					break;
				}
			}
			while(false);
		}

		CGUIViewImage m_viewImage;
		CGUIView3D m_view3D;
		CFLImage m_fliImage;
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
			CGUIViewImage viewImage = new CGUIViewImage();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3D = new CGUIView3D();

			// Laser Profile Sensor 선언 // Laser Profile Sensor Declaration
			CDeviceLaserProfileSensorLMI devLaserProfile = new CDeviceLaserProfileSensorLMI();

			// 이벤트를 받을 객체 선언 // Declare an object to receive events
			CDeviceEventProfileEx eventProfile = new CDeviceEventProfileEx();

			// 센서에 이벤트 객체 설정 // Set event object on sensor
			devLaserProfile.RegisterDeviceEvent(eventProfile);

			do
			{
				String strInput = "";

				// IP 주소를 입력 받습니다. // Enter the IP address.
				while(true)
				{
					Console.Write("Input IP Address: ");

					strInput = Console.ReadLine();

					if((res = devLaserProfile.SetIPAddress(strInput)).IsOK())
						break;

					ErrorPrint(res, "Failed to set IP Address.\n");
				}

				// 포트 번호를 입력 받습니다. // Enter the port number.
				while(true)
				{
					Console.Write("Input Port Number: ");

					strInput = Console.ReadLine();

					ushort u16Port = ushort.Parse(strInput);

					if((res = devLaserProfile.SetPortNumber(u16Port)).IsOK())
						break;

					ErrorPrint(res, "Failed to set port number.\n");
				}

				// 프로파일 센서를 초기화 합니다. // Initialize the profile sensor.
				if((res = devLaserProfile.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize device.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 812, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 3D 뷰 생성 // Create 3D view
				if((res = view3D.Create(812, 0, 1224, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// 각 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each view window
				if((res = viewImage.SynchronizeWindow(ref view3D)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 이벤트 객체에 View를 설정합니다. // Set a View on the event object.
				eventProfile.SetViewImage(viewImage);
				eventProfile.SetView3D(view3D);

				// 서피스 컬러라이제이션 기능을 설정합니다. Set the surface colorization feature.
				devLaserProfile.EnableSurfaceColorization(true);
				devLaserProfile.SetSurfaceColorizationRange(-5, 5);

				// 프로파일 센서를 Start 합니다. // Start the profile sensor.
				if((res = devLaserProfile.Start()).IsFail())
				{
					ErrorPrint(res, "Failed to start the profile sensor.\n");
					break;
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
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

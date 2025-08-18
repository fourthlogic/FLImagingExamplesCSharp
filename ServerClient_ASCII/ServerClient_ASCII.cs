using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Devices;

// 서버 소켓 이벤트 핸들러 클래스
// Server socket event class
// 소켓 이벤트를 수신하기 위해 CDeviceEventSocketBase에서 상속받아 구현
// Inherit from CDeviceEventSocketBase to receive socket events
class CDeviceEventSocketServerASCIIEx : CDeviceEventSocketASCII
{
	// 생성자 // Constructor
	public CDeviceEventSocketServerASCIIEx(CDeviceSocketServerASCII pSocketServerASCII)
	{
		m_pSocketServerASCII = pSocketServerASCII;
	}

	// 수신 이벤트 함수 재정의 // Override receive event function
	public override void OnReceived(CDeviceSocketBase pDeviceSocketClientASCII, string pSocketPacket)
	{
		// 받은 데이터를 문자열로 출력 // Print received data
		if(pSocketPacket != null)
		{
			Console.WriteLine("[Server] Recv " + pSocketPacket);
		}

		// 재전송(echo) // Send string(echo)
		if(pDeviceSocketClientASCII != null && pSocketPacket != null)
		{
			CDeviceSocketClientASCII pClient = pDeviceSocketClientASCII as CDeviceSocketClientASCII;

			if(pClient != null)
			{
				// Obtain Client Manager Objects
				CDeviceSocketClientASCIIManager pDeviceSocketClientASCIIManager = m_pSocketServerASCII.GetSocketClientManager();

				if(pDeviceSocketClientASCIIManager != null)
				{
					Console.WriteLine("[Server] Send " + pSocketPacket);

					// 연결이 살아있고 연결 상태라면 데이터 전송 // Send if connection is alive
					if(pDeviceSocketClientASCIIManager.IsClientAlive(pClient))
						pClient.Send(pSocketPacket);

					Thread.Sleep(500);
				}
			}
		}
	}

	protected

		CDeviceSocketServerASCII m_pSocketServerASCII;

}

// 클라이언트 소켓 이벤트 클래스
// Client socket event class
class CDeviceEventSocketClientASCIIEx : CDeviceEventSocketASCII
{
	// 생성자 // Constructor
	public CDeviceEventSocketClientASCIIEx()
	{
		m_bConnect = false;
	}

	// 연결 이벤트 함수 재정의 // Override connection event functions
	public override void OnConnected(CDeviceSocketBase pDeviceSocketClientASCII)
	{
		m_bConnect = true;
	}

	// 연결 해제 이벤트 함수 재정의 // Override disconnection event functions
	public override void OnDisconnected(CDeviceSocketBase pDeviceSocketClientASCII)
	{
		m_bConnect = false;
	}

	// 수신 이벤트 함수 재정의 // Override receive event function
	public override void OnReceived(CDeviceSocketBase pDeviceSocketClientASCII, string pSocketPacket)
	{
		// 받은 데이터를 문자열로 출력 // Print received data
		if(pSocketPacket != null)
		{
			Console.WriteLine("[Client] Recv " + pSocketPacket);
		}

		// 재전송(echo) // Send string(echo)
		if(m_bConnect && pDeviceSocketClientASCII != null && pSocketPacket != null)
		{
			CDeviceSocketClientASCII pClient = pDeviceSocketClientASCII as CDeviceSocketClientASCII;

			if(pClient != null)
			{
				Console.WriteLine("[Client] Send " + pSocketPacket);
				pClient.Send(pSocketPacket);
				Thread.Sleep(500);
			}
		}
	}

	protected

		bool m_bConnect;
}

class Program
{
	public static void ErrorPrint(CResult cResult, string str)
	{
		if(str.Length > 1)
			Console.WriteLine(str);

		Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
		Console.WriteLine("\n");
		Console.ReadKey();
	}
	static void Main(string[] args)
	{
		// FLImaging 라이브러리 초기화 (필수 호출) 
		// Initialize the FLImaging library (must be called once)
		CLibraryUtilities.Initialize();

		CResult res;

		// 소켓 서버, 클라이언트 선언 // Declare socket server and client
		CDeviceSocketServerASCII deviceSocketServerASCII = new CDeviceSocketServerASCII();
		CDeviceSocketClientASCII deviceSocketClientASCII = new CDeviceSocketClientASCII();

		// 이벤트 객체 생성 및 등록 // Create and register event handlers
		CDeviceEventSocketServerASCIIEx deviceEventSocketServerASCII = new CDeviceEventSocketServerASCIIEx(deviceSocketServerASCII);
		CDeviceEventSocketClientASCIIEx deviceEventSocketClientASCII = new CDeviceEventSocketClientASCIIEx();

		deviceSocketServerASCII.RegisterDeviceEvent(deviceEventSocketServerASCII);
		deviceSocketClientASCII.RegisterDeviceEvent(deviceEventSocketClientASCII);

		do
		{
			// 소켓 모드 설정 (Passive)
			// Set socket mode (Passive)
			deviceSocketServerASCII.SetSocketMode(ESocketMode.NoProtocol_Passive);
			deviceSocketClientASCII.SetSocketMode(ESocketMode.NoProtocol_Passive);

			// IP 주소와 포트 설정 // Set IP and port
			string flsIPAddress = "127.0.0.1";
			UInt16 u16Port = 4444;

			deviceSocketServerASCII.SetConnectionIPAddress(flsIPAddress, u16Port);
			deviceSocketClientASCII.SetConnectionIPAddress(flsIPAddress, u16Port);

			// 소켓 서버 초기화 // Initialize socket server
			if((res = deviceSocketServerASCII.Initialize()).IsFail())
			{
				ErrorPrint(res, "Failed to initialize server.");
				break;
			}

			// 소켓 클라이언트 초기화 (서버에 연결) // Initialize client (connect to server)
			if((res = deviceSocketClientASCII.Initialize()).IsFail())
			{
				ErrorPrint(res, "Failed to initialize client.");
				break;
			}

			// 테스트용 데이터 생성 // Create test data
			string flsData = "Socket echo test. [Enter any key if you want to exit]";

			// 클라이언트에서 서버로 데이터 송신 // Send data from client to server
			if((res = deviceSocketClientASCII.Send(flsData)).IsFail())
			{
				ErrorPrint(res, "Failed to send.");
				break;
			}

			Console.ReadKey();

			// 소켓 해제 및 이벤트 해제 // Terminate sockets and clear events
			deviceSocketServerASCII.Terminate();
			deviceSocketClientASCII.Terminate();
		}
		while(false);
	}
}

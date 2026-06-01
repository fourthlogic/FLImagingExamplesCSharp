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

namespace DeviceRS232C
{
	class Program
	{
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			CResult res = new CResult(EResult.UnknownError);

			// RS232C 선언 // Declare the RS232C
			CDeviceRS232C rs232C = new CDeviceRS232C();

			do
			{
				string strInput = string.Empty;
				bool bExit = false;

				// 페시브 모드 설정 false // Set passive mode to false
				rs232C.SetPassive(false);

				{
					// 컴포트 번호를 입력합니다. // Enter the COM port number.
					int i32ComPortNumber = 0;

					Console.Write("Port Number: ");
					strInput = Console.ReadLine();

					int.TryParse(strInput, out i32ComPortNumber);

					Console.Clear();

					// 컴포트 번호 설정 // Set the COM port number.
					rs232C.SetComPortNumber(i32ComPortNumber);
				}

				{
					int i32MenuSelection = 0;
					int i32BaudRate = 0;
					bool bValidInput = false;

					// 보드레이트를 입력합니다. // Enter the Baud Rate.
					while(!bValidInput)
					{
						Console.WriteLine("=== Select Baud Rate ===");
						Console.WriteLine("1. 9600");
						Console.WriteLine("2. 19200");
						Console.WriteLine("3. 38400");
						Console.WriteLine("4. 57600");
						Console.WriteLine("5. 115200");
						Console.Write("Select Number: ");

						strInput = Console.ReadLine();

						int.TryParse(strInput, out i32MenuSelection);

						Console.Clear();

						// 선택한 번호에 따라 보드 레이트 매핑 // Map the Baud Rate according to the selected number
						switch(i32MenuSelection)
						{
						case 1:
							i32BaudRate = 9600;
							bValidInput = true;
							break;

						case 2:
							i32BaudRate = 19200;
							bValidInput = true;
							break;

						case 3:
							i32BaudRate = 38400;
							bValidInput = true;
							break;

						case 4:
							i32BaudRate = 57600;
							bValidInput = true;
							break;

						case 5:
							i32BaudRate = 115200;
							bValidInput = true;
							break;

						default:
							Console.WriteLine("[Error] Invalid selection. Please try again.");
							Console.WriteLine();
							bValidInput = false;
							break;
						}
					}

					// 보드 레이트 설정 // Set the Baud Rate.
					rs232C.SetBaudRate(i32BaudRate);
				}

				if((res = rs232C.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the light controller.\n");
					break;
				}

				while(true)
				{
					int i32SelectMode = 0;

					// 작업 모드를 선택합니다. // Select the operation mode.
					Console.WriteLine("1. Send");
					Console.WriteLine("2. Recv");
					Console.WriteLine("0. Exit");
					Console.WriteLine();
					Console.Write("Select Number: ");

					strInput = Console.ReadLine();

					Console.WriteLine();
					Console.Clear();

					int.TryParse(strInput, out i32SelectMode);

					if(i32SelectMode == 0)
					{
						bExit = true;
						break;
					}

					if(i32SelectMode == 1)
					{
						// 텍스트를 입력합니다. // Input text.
						Console.Write("Input Text: ");
						strInput = Console.ReadLine();

						Console.WriteLine();
						Console.Clear();

						string cleanedInput = strInput.TrimEnd('\r', '\n');
						byte[] arrData = System.Text.Encoding.ASCII.GetBytes(strInput);

						rs232C.Send(arrData);
					}
					else if(i32SelectMode == 2)
					{
						CDeviceSerialPacket packet = new CDeviceSerialPacket();

						// 데이터 수신 // Receive data
						rs232C.Recv(ref packet);

						// 수신 데이터 출력 // Print the received data
						if(packet.GetSize() > 0)
						{
							string strRecv = System.Text.Encoding.ASCII.GetString(
								packet.GetBuffer(),
								0,
								(int)packet.GetSize());

							Console.WriteLine($"Recv Text: {strRecv}");
						}
						else
						{
							Console.WriteLine("No Recv Data.");
						}
					}
				}

				if(bExit)
					break;
			}
			while(false);

			// RS232C 연결을 종료합니다. // Terminate the connection to the RS232C.
			if((res = rs232C.Terminate()).IsFail())
			{
				// [오류 수정] 기존 "Failed to terminate the motion." 주석 및 문자열을 통신 종료에 맞게 변경
				ErrorPrint(res, "Failed to terminate the communication.\n");
			}
		}

		// 경고 코드 출력 함수 // Error print function
		public static void ErrorPrint(CResult res, string strMessage)
		{
			if(!string.IsNullOrEmpty(strMessage))
				Console.WriteLine(strMessage);

			Console.WriteLine(
				$"Error code : {res.GetResultCode()}\n" +
				$"Error name : {res.GetString()}\n");
		}
	}
}
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
using FLImagingCLR.Devices;
using CResult = FLImagingCLR.CResult;
using System.Reflection;

namespace FLImagingExamplesCSharp
{
	class DeviceLightControllerLVS_PN_08
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

			CResult res = new CResult();

			// 조명 컨트롤러 LVS_PN_08 선언 // Declare the LVS_PN_08 Light Controller
			CDeviceLightControllerLVS_PN_08 lightControllerLVS_PN08 = new CDeviceLightControllerLVS_PN_08();

			bool bExit = false;

			do
			{
				Console.Clear();
				Console.Write("Port Number: ");

				if(int.TryParse(Console.ReadLine(), out var portNumber))
				{
					// 컴포트 번호 설정 // Set the COM port number.
					lightControllerLVS_PN08.SetComPortNumber(portNumber);
				}

				Console.Clear();
				Console.Write("BaudRate(Switch OFF = 9600, ON = 19200]: ");

				if(int.TryParse(Console.ReadLine(), out var baudRate))
				{
					// 보드레이트 설정 // Set the baud rate.
					lightControllerLVS_PN08.SetBaudRate(baudRate);
				}

				if(lightControllerLVS_PN08.Initialize().IsFail())
				{
					Console.WriteLine("Failed to initialize the light controller.");
					break;
				}

				while(true)
				{
					// 작업 모드를 선택합니다. // Select the operation mode.
					Console.WriteLine("1. Light On/Off");
					Console.WriteLine("2. Light Value");
					Console.WriteLine("0. Exit\n");
					Console.Write("Select Number: ");

					if(!int.TryParse(Console.ReadLine(), out var operationMode))
					{
						Console.Clear();
						Console.WriteLine("Invalid input. Try again.\n");
						continue;
					}

					Console.Clear();

					if(operationMode == 0)
					{
						bExit = true;
						break;
					}

					Console.Write("Select Channel: ");

					if(int.TryParse(Console.ReadLine(), out var channel))
					{
						if(operationMode == 1)
						{
							Console.WriteLine("\n1. On\n2. Off");
							Console.Write("Enter On/Off: ");

							if(int.TryParse(Console.ReadLine(), out var onOff))
							{
								// 채널별 On/Off 상태를 설정합니다. // Set the On/Off state for the channel.
								lightControllerLVS_PN08.SetChannelState(channel, onOff == 1);
							}
						}
						else if(operationMode == 2)
						{
							Console.Write("Input Light Value (0 ~ 255): ");

							if(int.TryParse(Console.ReadLine(), out var lightValue))
							{
								// 조명 값을 설정합니다. // Set the light value.
								lightControllerLVS_PN08.SetLightValue(channel, (byte)lightValue);
							}
						}
					}

					Console.Clear();

					// 입력된 파라미터를 적용합니다. // Apply the configured parameters.
					lightControllerLVS_PN08.Apply();
				}

				if(bExit)
					break;
			}
			while(false);

			// 조명 컨트롤러에 연결을 종료합니다. // Terminate the connection to the light controller.
			if(lightControllerLVS_PN08.Terminate().IsFail())
			{
				Console.WriteLine("Failed to terminate the motion.\n");
			}
		}
	}
}

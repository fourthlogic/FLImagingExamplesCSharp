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
using CResult = FLImagingCLR.CResult;

namespace DeviceCameraiRAYPLE
{
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

		[STAThread]
		static void Main(string[] args)
		{
			CResult res = new CResult();

			// 조명 컨트롤러 WordopPD5_6024 선언 // Declare the WordopPD5_6024 Light Controller
			CDeviceLightControllerWordopPD5_6024 lightController = new CDeviceLightControllerWordopPD5_6024();

			bool bExit = false;

			do
			{
				int i32connectionType = 0;

				do
				{
					// 조명 컨트롤러 연결 방식을 선택합니다. // Select the connection method for the light controller.
					Console.WriteLine("1. RS232C");
					Console.WriteLine("2. TCP Server");
					Console.WriteLine("3. TCP Client");
					Console.WriteLine("4. UDP");
					Console.WriteLine("0. Exit\n");
					Console.Write("Connection Type: ");

					if(!int.TryParse(Console.ReadLine(), out i32connectionType) || i32connectionType < 0 || i32connectionType > 4)
					{
						Console.Clear();
						Console.WriteLine("Incorrect input. Please select again.\n");
						continue;
					}

					Console.Clear();

					if(i32connectionType == 0)
						bExit = true;

					break;
				}
				while(true);

				if(bExit)
					break;

				CDeviceLightControllerWordopPD5_6024.EConnectionMethod connectionMethod = CDeviceLightControllerWordopPD5_6024.EConnectionMethod.RS232C;

				if(i32connectionType == 2)
					connectionMethod = CDeviceLightControllerWordopPD5_6024.EConnectionMethod.TCPServer;
				else if(i32connectionType == 3)
					connectionMethod = CDeviceLightControllerWordopPD5_6024.EConnectionMethod.TCPClient;
				else if(i32connectionType == 4)
					connectionMethod = CDeviceLightControllerWordopPD5_6024.EConnectionMethod.UDP;

				// 연결 방식을 설정합니다. // Set the connection method.
				lightController.SetConnectionMethod(connectionMethod);

				if(i32connectionType == 1) // RS232C
				{
					Console.Clear();
					Console.Write("Port Number: ");

					if(int.TryParse(Console.ReadLine(), out var portNumber))
					{
						// 컴포트 번호 설정 // Set the COM port number.
						lightController.SetConnectionComPortNumber(portNumber);
					}
				}
				else
				{
					Console.Clear();
					Console.Write("Input IP Address: ");

					var ipAddress = Console.ReadLine();

					Console.Write("Port Number: ");

					if(int.TryParse(Console.ReadLine(), out var port))
					{
						// IP 주소, Port 설정 // Set the IP address and port.
						lightController.SetConnectionIPAddress(ipAddress);
						lightController.SetConnectionPort((ushort)port);
					}
				}

				if(lightController.Initialize().IsFail())
				{
					Console.WriteLine("Failed to initialize the light controller.");
					break;
				}

				int i32channelCount = 0;

				do
				{
					// 채널 갯수를 선택합니다. // Select the number of channels.
					Console.WriteLine("\n1. Channel 4");
					Console.WriteLine("2. Channel 8");
					Console.WriteLine("0. Exit\n");
					Console.Write("Input Channel Count: ");

					if(!int.TryParse(Console.ReadLine(), out i32channelCount) || (i32channelCount != 0 && i32channelCount != 1 && i32channelCount != 2))
					{
						Console.Clear();
						Console.WriteLine("Incorrect input. Please select again.\n");
						continue;
					}

					Console.Clear();

					if(i32channelCount == 0)
						bExit = true;

					break;
				} 
				while(true);

				if(bExit)
					break;

				CDeviceLightControllerWordopPD5_6024.ELightChannel eLightChannel = i32channelCount == 1 ? CDeviceLightControllerWordopPD5_6024.ELightChannel.Port_8 : CDeviceLightControllerWordopPD5_6024.ELightChannel.Port_4;

				// 채널 갯수 설정 // Set the number of channels.
				lightController.SetLightChannel(eLightChannel);

				int i32communicationType = 0;

				do
				{
					// 통신 방식을 선택합니다. // Select the communication method.
					Console.WriteLine("1. ASCII Code");
					Console.WriteLine("2. Hexadecimal");
					Console.WriteLine("0. Exit\n");
					Console.Write("Input Communication Type: ");

					if(!int.TryParse(Console.ReadLine(), out i32communicationType) || (i32communicationType != 0 && i32communicationType != 1 && i32communicationType != 2))
					{
						Console.Clear();
						Console.WriteLine("Incorrect input. Please select again.\n");
						continue;
					}

					Console.Clear();

					if(i32communicationType == 0)
					{
						bExit = true;
					}

					break;
				} 
				while(true);

				if(bExit)
					break;

				CDeviceLightControllerWordopPD5_6024.ECommunicationType commType = i32communicationType == 1 ? CDeviceLightControllerWordopPD5_6024.ECommunicationType.ASCIICode : CDeviceLightControllerWordopPD5_6024.ECommunicationType.Hexadecimal;

				// 통신 방식을 설정합니다. // Set the communication type.
				lightController.SetCommunicationType(commType);

				while(true)
				{
					// 작업 모드를 선택합니다. // Select the operation mode.
					Console.WriteLine("1. Light On/Off");
					Console.WriteLine("2. Light Value");
					Console.WriteLine("3. Strobe Time");
					Console.WriteLine("4. Trigger Method");
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

					if(operationMode == 4)
					{
						int triggerMethod = 0;

						do
						{
							// 트리거 방식을 선택합니다. // Select the trigger method.
							Console.WriteLine("\n1. Low Level");
							Console.WriteLine("2. High Level");
							Console.WriteLine("3. Falling Edge");
							Console.WriteLine("4. Rising Edge");
							Console.WriteLine("0. Exit\n");
							Console.Write("Input Trigger Method: ");

							if(!int.TryParse(Console.ReadLine(), out triggerMethod) || (triggerMethod != 0 && triggerMethod != 1 && triggerMethod != 2 && triggerMethod != 3 && triggerMethod != 4))
							{
								Console.Clear();
								Console.WriteLine("Incorrect input. Please select again.\n");
								continue;
							}

							Console.Clear();

							if(triggerMethod == 0)
							{
								bExit = true;
							}

							break;

						} 
						while(true);

						if(bExit)
							break;

						CDeviceLightControllerWordopPD5_6024.ETriggerMethod eTriggerMethod = CDeviceLightControllerWordopPD5_6024.ETriggerMethod.LowLevel;

						if(triggerMethod == 2)
							eTriggerMethod = CDeviceLightControllerWordopPD5_6024.ETriggerMethod.HighLevel;
						else if(triggerMethod == 3)
							eTriggerMethod = CDeviceLightControllerWordopPD5_6024.ETriggerMethod.FallingEdge;
						else if(triggerMethod == 4)
							eTriggerMethod = CDeviceLightControllerWordopPD5_6024.ETriggerMethod.RisingEdge;

						// 트리거 방식을 설정합니다. // Set the trigger method.
						lightController.SetTriggerMethod(eTriggerMethod);
					}
					else
					{
						Console.Write("Select Channel: ");

						if(int.TryParse(Console.ReadLine(), out var channel))
						{
							if(operationMode == 1)
							{
								Console.WriteLine("\n0. On\n1. Off");
								Console.Write("Enter On/Off: ");

								if(int.TryParse(Console.ReadLine(), out var onOff))
								{
									// 채널별 On/Off 상태를 설정합니다. // Set the On/Off state for the channel.
									lightController.SetChannelState(channel, onOff == 0);
								}
							}
							else if(operationMode == 2)
							{
								Console.Write("Input Light Value (0 ~ 255): ");

								if(int.TryParse(Console.ReadLine(), out var lightValue))
								{
									// 조명 값을 설정합니다. // Set the light value.
									lightController.SetLightValue(channel, (byte)lightValue);
								}
							}
							else if(operationMode == 3)
							{
								Console.Write("Input Strobe Time (1 ~ 999 ms): ");

								if(int.TryParse(Console.ReadLine(), out var strobeTime))
								{
									// 스토로브 타임을 설정합니다. // Set the strobe time.
									lightController.SetStrobeTime(channel, (ushort)strobeTime);
								}
							}
						}

						Console.Clear();
					}

					// 입력된 파라미터를 적용합니다. // Apply the configured parameters.
					lightController.Apply();
				}

				if(bExit)
					break;
			}
			while(false);

			// 조명 컨트롤러에 연결을 종료합니다. // Terminate the connection to the light controller.
			if(lightController.Terminate().IsFail())
			{
				Console.WriteLine("Failed to terminate the motion.\n");
			}
		}
	}
}

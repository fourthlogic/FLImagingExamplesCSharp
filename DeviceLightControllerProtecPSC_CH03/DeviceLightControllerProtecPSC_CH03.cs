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

namespace DeviceLightController
{
	class DeviceLightControllerProtecPSC_CH03
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

			// 조명 컨트롤러 ProtecPSC_CH03 선언 // Declare the ProtecPSC_CH03 Light Controller
			CDeviceLightControllerProtecPSC_CH03 lightController = new CDeviceLightControllerProtecPSC_CH03();

			bool bExit = false;

			do
			{
				Console.Clear();
				Console.Write("Port Number: ");

				if(int.TryParse(Console.ReadLine(), out var portNumber))
				{
					// 컴포트 번호 설정 // Set the COM port number.
					lightController.SetComPortNumber(portNumber);
				}

				if(lightController.Initialize().IsFail())
				{
					Console.WriteLine("Failed to initialize the light controller.");
					break;
				}

				while(true)
				{
					// 작업 모드를 선택합니다. // Select the operation mode.
					Console.WriteLine("1. Live Mode");
					Console.WriteLine("2. Strobe Mode");
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

					Int32 i32triggerIndex = 0;

					if(operationMode == 1)
					{
						lightController.SetOperationMode(CDeviceLightControllerProtecPSC_CH03.EOperationMode.Live);

						// On/Off 상태를 설정합니다. // Set the On/Off state.
						Console.WriteLine("\n1. Live On\n2. Live Off");
						Console.Write("Select Number: ");

						if(int.TryParse(Console.ReadLine(), out var onOff))
						{
							if(onOff == 1)
							{
								lightController.EnableLiveTurnOn(true);

								Console.Write("Select Channel Index: ");

								if(int.TryParse(Console.ReadLine(), out var channel))
								{
									Console.Write("Input Light Value (0 ~ 255): ");

									if(int.TryParse(Console.ReadLine(), out var lightValue))
									{
										// 조명 값을 설정합니다. // Set the light value.
										lightController.SetLightValue(channel, (byte)lightValue);
									}
								}
							}
							else if(onOff == 2)
								lightController.EnableLiveTurnOn(false);
						}

						Console.Clear();
					}
					else if(operationMode == 2)
					{
						lightController.SetOperationMode(CDeviceLightControllerProtecPSC_CH03.EOperationMode.Strobe);

						Console.Write("Select Trigger Index: ");

						if(int.TryParse(Console.ReadLine(), out i32triggerIndex))
						{
							lightController.EnableLiveTurnOn(true);

							Console.Write("Select Channel Index: ");

							if(int.TryParse(Console.ReadLine(), out var channel))
							{
								Console.Write("Input Strobe Value (0 ~ 4000us): ");

								if(int.TryParse(Console.ReadLine(), out var strobeValue))
								{
									// 조명 값을 설정합니다. // Set the light value.
									lightController.SetStrobe(i32triggerIndex, channel, (UInt16)strobeValue);
								}
							}
						}

						Console.Clear();
					}

					// 입력된 파라미터를 적용합니다. // Apply the configured parameters.
					if(lightController.Apply(i32triggerIndex).IsFail())
					{
						Console.WriteLine("Failed to apply the light controller.");
						break;
					}

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

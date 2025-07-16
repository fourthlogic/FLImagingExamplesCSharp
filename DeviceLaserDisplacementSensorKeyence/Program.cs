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

namespace DeviceLaserDisplacementSensorKeyence
{
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
            CResult res;

			// Keyence 레이저 변위 센서 장치를 선언 // Declare keyence laser displacement sensor device
			CDeviceLaserDisplacementSensorKeyence devLaserDisplacement = new CDeviceLaserDisplacementSensorKeyence();

			do
			{
				String strInput = "";

				// 컴포트 번호를 입력합니다. // Enter the com port number.
				Console.Write("Enter com port number: ");
				strInput = Console.ReadLine();

				int i32ComPortNumber = 0;
				Int32.TryParse(strInput, ref i32ComPortNumber);

				devLaserDisplacement.SetComPortNumber(i32ComPortNumber);

				while(true)
				{
					// 보드 레이트를 선택합니다. // Select the baud rate.
					Console.Write("\n");
					Console.Write("1. 9600\n");
					Console.Write("2. 19200\n");
					Console.Write("3. 38400\n");
					Console.Write("4. 57600\n");
					Console.Write("5. 115200\n");
					Console.Write("Select baud rate: ");

					strInput = Console.ReadLine();

					int i32Select = 0;
					Int32.TryParse(strInput, ref i32Select);

					int i32BaudRate = -1;

					switch(i32Select)
					{
					case 1:
						i32BaudRate = 9600;
						break;
					case 2:
						i32BaudRate = 19200;
						break;
					case 3:
						i32BaudRate = 38400;
						break;
					case 4:
						i32BaudRate = 57600;
						break;
					case 5:
						i32BaudRate = 115200;
						break;
					}

					if(i32BaudRate != -1)
					{
						devLaserDisplacement.SetBaudRate(i32BaudRate);
						break;
					}

					Console.Write("Incorrect input. Please select again.\n\n");
				}

				while(true)
				{
					// 패리티를 선택합니다. // Select the parity.
					Console.Write("\n");
					Console.Write("1. None\n");
					Console.Write("2. Even\n");
					Console.Write("3. Odd\n");
					Console.Write("Select parity: ");

					strInput = Console.ReadLine();

					int i32Select = 0;
					Int32.TryParse(strInput, ref i32Select);

					int i32Parity = -1;

					switch(i32Select)
					{
					case 1:
						i32Parity = 0;
						break;
					case 2:
						i32Parity = 1;
						break;
					case 3:
						i32Parity = 2;
						break;
					}

					if(i32Parity != -1)
					{
						devLaserDisplacement.SetParity(i32Parity);
						break;
					}

					Console.Write("Incorrect input. Please select again.\n\n");
				}

				// 레이저 변위 센서 장치를 초기화 합니다. // Initialize the laser displacement sensor device.
				if((res = devLaserDisplacement.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the device.");
					break;
				}

				while(true)
				{
					// 출력 채널을 선택합니다. // Select the output channel.
					Console.Write("\n");
					Console.Write("1. Output channel 1\n");
					Console.Write("2. Output channel 2\n");
					Console.Write("Select output channel: ");

					strInput = Console.ReadLine();

					int i32Select = 0;
					Int32.TryParse(strInput, ref i32Select);

					// 측정값을 얻어옵니다. // Retrieve the measured value
					List<double> listMeasured = new List<double>();

					switch(i32Select)
					{
					case 1:
						res = devLaserDisplacement.GetMeasuredValue(CDeviceLaserDisplacementSensorKeyence.EOutputChannel.Channel1, ref listMeasured);
						break;

					case 2:
						res = devLaserDisplacement.GetMeasuredValue(CDeviceLaserDisplacementSensorKeyence.EOutputChannel.Channel2, ref listMeasured);
						break;

					default:
						Console.Write("Incorrect input. Please select again.\n\n");
						break;
					}

					if(listMeasured.Count == 0)
						continue;

					if(res.IsFail())
					{
						ErrorPrint(res, "Failed to get measured value.");
						continue;
					}

					Console.Write("Output channel {0} measured: {1}\n\n", i32Select, listMeasured[0]);
				}
            }
            while(false);

			// 레이저 변위 센서 장치의 초기화를 해제합니다. // Terminate the laser displacement sensor device.
			devLaserDisplacement.Terminate();

            if (res.IsFail())
                Console.ReadLine();
        }
    }
}

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

namespace DeviceTriggerAxl
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

			// Axl Trigger 장치를 선언 // Declare Axl Trigger device
			CDeviceTriggerAxl devTrigger = new CDeviceTriggerAxl();

			do
			{
				String strInput = "";

				// 장치의 모듈 인덱스를 입력합니다. // Enter the module index of the device.
				Console.Write("Enter Module index: ");
				strInput = Console.ReadLine();

				int i32ModuleIndex = 0;
				Int32.TryParse(strInput, out i32ModuleIndex);

				// 장치의 모듈 인덱스를 설정합니다. // Sets the module index for the device.
				if((res = devTrigger.SetModuleIndex(i32ModuleIndex)).IsFail())
				{
					ErrorPrint(res, "Failed to set module index.");
					break;
				}

				// Trigger 장치를 초기화 합니다. // Initialize the Trigger device.
				if((res = devTrigger.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the device.");
					break;
				}

				// 트리거 채널을 입력합니다. // Enter the trigger channel.
				int i32Channel = 0;

				while(true)
				{
					Console.Write("\n");
					Console.Write("Enter trigger channel(0 ~ {0}): ", devTrigger.GetTriggerChannelCount() - 1);
					strInput = Console.ReadLine();

					Int32.TryParse(strInput, out i32Channel);

					if(i32Channel < 0 || i32Channel >= devTrigger.GetTriggerChannelCount())
						Console.Write("Incorrect input. Please enter again.\n");
					else
						break;
				}


				// 엔코더 소스를 입력합니다. // Enter the encoder source.
				CDeviceTriggerAxl.EEncoderSource eEncoderSource = CDeviceTriggerAxl.EEncoderSource.ABPhase;

				while(true)
				{
					Console.Write("\n");
					Console.Write("Encoder Source\n");
					Console.Write("1. AB Phase\n");
					Console.Write("2. Z Phase\n");
					Console.Write("Select: ");
					strInput = Console.ReadLine();

					int i32Select = 0;
					Int32.TryParse(strInput, out i32Select);

					bool bSelected = true;

					switch(i32Select)
					{
					case 1:
						eEncoderSource = CDeviceTriggerAxl.EEncoderSource.ABPhase;
						break;

					case 2:
						eEncoderSource = CDeviceTriggerAxl.EEncoderSource.ZPhase;
						break;

					default:
						bSelected = false;
						break;
					}

					if(bSelected)
						break;

					Console.Write("Incorrect input. Please select again.\n");
				}

				// 엔코더 소스를 설정합니다. // Sets the encoder source.
				if((res = devTrigger.SetEncoderSource(i32Channel, eEncoderSource)).IsFail())
				{
					ErrorPrint(res, "Failed to set encoder source.");
					break;
				}


				// 엔코더 방식을 입력합니다. // Enter the encoder method.
				CDeviceTriggerAxl.EEncoderMethod eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.UpDownSqr1;

				while(true)
				{
					Console.Write("\n");
					Console.Write("Encoder Method\n");
					Console.Write("1. Up/Down Square 1\n");
					Console.Write("2. Up/Down Square 2\n");
					Console.Write("3. AB Phase Square 1\n");
					Console.Write("4. AB Phase Square 2\n");
					Console.Write("5. AB Phase Square 4\n");
					Console.Write("6. Pulse/Direction Square 1\n");
					Console.Write("7. Pulse/Direction Square 2\n");
					Console.Write("Select: ");
					strInput = Console.ReadLine();

					int i32Select = 0;
					Int32.TryParse(strInput, out i32Select);

					bool bSelected = true;

					switch(i32Select)
					{
					case 1:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.UpDownSqr1;
						break;

					case 2:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.UpDownSqr2;
						break;

					case 3:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.ABPhaseSqr1;
						break;

					case 4:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.ABPhaseSqr2;
						break;

					case 5:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.ABPhaseSqr4;
						break;

					case 6:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.PulseDirSqr1;
						break;

					case 7:
						eEncoderMethod = CDeviceTriggerAxl.EEncoderMethod.PulseDirSqr2;
						break;

					default:
						bSelected = false;
						break;
					}

					if(bSelected)
						break;

					Console.Write("Incorrect input. Please select again.\n");
				}

				// 엔코더 방식을 설정합니다. // Sets the encoder method.
				if((res = devTrigger.SetEncoderMethod(i32Channel, eEncoderMethod)).IsFail())
				{
					ErrorPrint(res, "Failed to set encoder method.");
					break;
				}

				// 트리거 모드를 설정합니다. // Sets the trigger mode.
				if((res = devTrigger.SetTriggerMode(i32Channel, CDeviceTriggerAxl.ETriggerMode.Position)).IsFail())
				{
					ErrorPrint(res, "Failed to set trigger mode.");
					break;
				}

				while(true)
				{
					// 트리거를 비활성화 합니다. // Disable the trigger.
					if((res = devTrigger.SetTriggerEnable(i32Channel, false)).IsFail())
					{
						ErrorPrint(res, "Failed to set trigger enable.");
						break;
					}

					// 엔코더 포지션을 0 으로 설정합니다. // Set the encoder position to 0.
					if((res = devTrigger.SetEncoderPosition(i32Channel, 0)).IsFail())
					{
						ErrorPrint(res, "Failed to set encoder position.");
						break;
					}

					// 포지션 값을 입력합니다. // Enter a position value.
					Console.Write("\n");
					Console.Write("Enter trigger position(10, 20, 30, ...): ");
					strInput = Console.ReadLine();

					// 포지션 값을 담기위해 List<double> 생성 // Create List<double> to hold position values
					List<double> listPosition = new List<double>();

					// 입력 받은 문자열을 ',' 으로 구분하여 double 값으로 변환합니다. // Separate the input string with ',' and convert it to a double value.
					String[] arrStrInput = strInput.Split(',');

					foreach(var input in arrStrInput)
					{
						if(input.Length == 0)
							break;

						if(input.Equals("\n"))
							break;

						double f64Input = 0;

						if(Double.TryParse(input, out f64Input) == false)
							continue;

						listPosition.Add(f64Input);
					}

					// 트리거 포지션을 설정합니다. // Sets the trigger position.
					if((res = devTrigger.SetTriggerPosition(i32Channel, listPosition)).IsFail())
					{
						ErrorPrint(res, "Failed to set trigger position.");
						break;
					}

					// 트리거를 활성화 합니다. // Enables the trigger.
					if((res = devTrigger.SetTriggerEnable(i32Channel, true)).IsFail())
					{
						ErrorPrint(res, "Failed to set trigger enable.");
						break;
					}

					Console.Write("\n");
					Console.Write("0. Reset the trigger position\n");
					Console.Write("Other. Exit\n");
					Console.Write("Enter: ");
					strInput = Console.ReadLine();

					if(strInput != "0")
						break;
				}
			}
			while(false);

			// Trigger 장치의 초기화를 해제합니다. // Terminate the Trigger device.
			devTrigger.Terminate();

			if(res.IsFail())
				Console.ReadLine();
		}
    }
}

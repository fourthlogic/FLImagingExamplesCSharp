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

namespace DeviceDIOAxl
{
    class DeviceDIOAxl
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

            // Axl DIO 장치를 선언 // Declare Axl DIO device
            CDeviceDIOAxl devDIO = new CDeviceDIOAxl();

            do
            {
                String strInput = "";

	            // DIO 장치를 초기화 합니다. // Initialize the DIO device.
	            if((res = devDIO.Initialize()).IsFail())
	            {
                    ErrorPrint(res, "Failed to initialize the device.");
			        break;
	            }

	            while(true)
	            {
		            // 사용할 기능을 선택합니다. // Select the features you want to use.
		            Console.Write("1. Read input\n");
		            Console.Write("2. Read output\n");
		            Console.Write("3. Write input\n");
		            Console.Write("4. Write output\n");
		            Console.Write("Select: ");
		            strInput = Console.ReadLine();

                    int i32Select = 0;
                    Int32.TryParse(strInput, out i32Select);

		            switch(i32Select)
		            {
		            case 1:
		            case 2:
			            {
				            // Bit 를 입력 받습니다. // Enter Bit.
				            Console.Write("Bit input: ");
				            strInput = Console.ReadLine();

				            int i32Bit = 0;
                            Int32.TryParse(strInput, out i32Bit);

				            bool bReadStatus = false;

				            // Bit 의 상태를 읽습니다. // Read Bit status.
				            if(i32Select == 1)
					            bReadStatus = devDIO.ReadInBit(i32Bit);
				            else 
					            bReadStatus = devDIO.ReadOutBit(i32Bit);

				            Console.Write("Read status: {0}\n\n", bReadStatus);
			            }
			            break;

		            case 3:
		            case 4:
			            {
				            // Bit 를 입력 받습니다. // Enter Bit.
				            Console.Write("Bit input: ");
				            strInput = Console.ReadLine();

				            int i32Bit = 0;
                            Int32.TryParse(strInput, out i32Bit);

				            // 상태를 입력 받습니다. // Enter status.
				            Console.Write("Status input: ");
				            strInput = Console.ReadLine();
				
				            bool bWriteStatus = false;

                            int i32Status = 0;
                            Int32.TryParse(strInput, out i32Status);

				            if(i32Status != 0)
					            bWriteStatus = true;

				            // Bit 에 상태를 기록합니다. // Write the status in Bit.
				            if(i32Select == 3)
					            res = devDIO.WriteInBit(i32Bit, bWriteStatus);
				            else
					            res = devDIO.WriteOutBit(i32Bit, bWriteStatus);

				            if(res.IsOK())
					            Console.Write("Succeeded to write.\n\n");
				            else
					            Console.Write("Failed to write.\n\n");
			            }
			            break;

		            default:
			            Console.Write("Incorrect input. Please select again.\n\n");
			            break;
		            }
	            }
            }
            while(false);

            // DIO 장치의 초기화를 해제합니다. // Terminate the DIO device.
            devDIO.Terminate();

            if (res.IsFail())
                Console.ReadLine();
        }
    }
}

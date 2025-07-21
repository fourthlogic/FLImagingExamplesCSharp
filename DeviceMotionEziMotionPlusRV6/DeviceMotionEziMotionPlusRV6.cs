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
	class DeviceMotionEziMotionPlusRV6
    {
		// 모션 타입 enum // motion type enum
		enum EMotion
		{
			SearchOriginPosition = 0,
			MoveAbsolutePosition,
			MoveIncrementalPosition,
		}

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
			CResult res;

			// Ezi Motion Plus R V6 선언 // Declare Ezi Motion Plus R V6
			CDeviceMotionEziMotionPlusRV6 motionEziMotionPluse = new CDeviceMotionEziMotionPlusRV6();

			do
			{
				int i32PortNumber = 0;
				int i32AxisNumber = 0;
				double f64Resolution = 0;
				double f64MoveSpeed = 0;
				double f64AccDecSpeed = 0;
				double f64Position = 0;
				EMotion eMotionType = EMotion.SearchOriginPosition;
				String strInput = "";

				// 장치가 연결된 포트 번호를 입력합니다. // Connected devices port number.
				Console.Write("Port Number : ");
				strInput = Console.ReadLine();
				Int32.TryParse(strInput, out i32PortNumber);

				// 포트 번호를 설정합니다. // Set Port Number.
				motionEziMotionPluse.SetPortNo((byte)i32PortNumber);

				// 연결할 축 갯수 // connected axis Count
				motionEziMotionPluse.SetAxisCount(1);

				// 장치 초기화 // devices initialize
				if((res = motionEziMotionPluse.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the motion.\n");
					break;
				}

				// 모션 축 객체 // motion axis object
				CDeviceMotionAxisEziMotionPlusRV6 pDMAxis = (CDeviceMotionAxisEziMotionPlusRV6)motionEziMotionPluse.GetMotionAxis(i32AxisNumber);

				// 서보 On // Servo On
				if((res = pDMAxis.SetServoOn(true)).IsFail())
				{
					ErrorPrint(res, "Failed to servo on.\n");
					break;
				}

				do
				{
					Thread.Sleep(100);
				}
				while(!pDMAxis.IsServoOn());

				// Axis resolution을 입력합니다. // Set axis resolution value.
				Console.Write("Enter axis resolution(mm/pulse) : ");
				strInput = Console.ReadLine();
				double.TryParse(strInput, out f64Resolution);
				pDMAxis.SetResolution(f64Resolution);

				while(true)
				{
					bool bExit = false;

					while(true)
					{
						Console.Write("\n");
						Console.Write("1. Search Origin Position\n");
						Console.Write("2. Move Absolute Position\n");
						Console.Write("3. Move Incremental Position\n");
						Console.Write("0. Exit\n");
						Console.Write("Select Motion Type: ");
						strInput = Console.ReadLine();
						Console.Write("\n");

						int i32Select = 0;
						int.TryParse(strInput, out i32Select);
						bool bSelected = true;

						switch(i32Select)
						{
						case 0:
							bExit = true;
							break;
						case 1:
							eMotionType = EMotion.SearchOriginPosition;
							break;
						case 2:
							eMotionType = EMotion.MoveAbsolutePosition;
							break;
						case 3:
							eMotionType = EMotion.MoveIncrementalPosition;
							break;

						default:
							bSelected = false;
							break;
						}

						if(bSelected)
							break;

						Console.Write("Incorrect input. Please select again.\n\n");
					}

					if(bExit)
						break;

					if(eMotionType == EMotion.SearchOriginPosition)
					{
						// 원점 복귀 운전을 진행합니다. // 
						if((res = pDMAxis.SearchOriginPosition()).IsFail())
						{
							ErrorPrint(res, "Failed to search origin position.\n");
							break;
						}

						// 모션이 정지 될때까지 대기 // Wait until motion stops/
						do
						{
							Thread.Sleep(100);
						}
						while(pDMAxis.IsSearchOrigin());

						if(!pDMAxis.IsSearchOriginEnd())
							Console.Write("Failed to search origin position.\n");
						else
							Console.Write("Successed to search origin position.\n");
					}
					else
					{
						// 이동 속도를 입력합니다. // Set axis speed.
						Console.Write("Enter Axis Speed(mm/s): ");
						strInput = Console.ReadLine();
						double.TryParse(strInput, out f64MoveSpeed);

						// 가감속을 입력합니다. // Set axis acceleation.
						Console.Write("Enter Axis Acceleation(mm/s2): ");
						strInput = Console.ReadLine();
						double.TryParse(strInput, out f64AccDecSpeed);

						// 이동거리나 절대위치를 입력합니다. // Set incremental or absolute position.
						Console.Write("Enter Axis Position(mm): ");
						strInput = Console.ReadLine();
						double.TryParse(strInput, out f64Position);

						if(eMotionType == EMotion.MoveAbsolutePosition)
						{
							// 절대 좌표로 이동 // move absolute position
							if((res = pDMAxis.MovePosition(f64Position, f64MoveSpeed, f64AccDecSpeed, f64AccDecSpeed, false)).IsFail())
							{
								ErrorPrint(res, "Failed to move absolute position.\n");
								break;
							}

							// 모션이 정지 될때까지 대기 // Wait until motion stops/
							do
							{
								Thread.Sleep(100);
							}
							while(pDMAxis.IsInMotion());

							if(!pDMAxis.IsMotionDone())
								Console.Write("Failed to move absolute position.\n");
							else
								Console.Write("Successed to move absolute position.\n");
						}
						else if(eMotionType == EMotion.MoveIncrementalPosition)
						{
							if((res = pDMAxis.MoveDistance(f64Position, f64MoveSpeed, f64AccDecSpeed, f64AccDecSpeed, false)).IsFail())
							{
								ErrorPrint(res, "Failed to move incremental position.\n");
								break;
							}

							// 모션이 정지 될때까지 대기 // Wait until motion stops/
							do
							{
								Thread.Sleep(100);
							}
							while(pDMAxis.IsInMotion());

							if(!pDMAxis.IsMotionDone())
								Console.Write("Failed to move incremental position.\n");
							else
								Console.Write("Successed to move incremental position.\n");
						}
					}
				}

			}
			while(false);

			// Motion 객체에 연결을 종료 합니다. // Terminate the connection to the motion object.
			if((res = motionEziMotionPluse.Terminate()).IsFail())
			{
				ErrorPrint(res, "Failed to terminate the motion.\n");
			}

			return;
		}
    }
}

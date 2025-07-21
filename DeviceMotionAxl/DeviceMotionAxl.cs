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

namespace DeviceMotionAxl
{
	class DeviceMotionAxl
    {
		// 모션 기능 enum // motion feature enum
		enum EMotionFeature
		{
			SearchOriginPosition = 0,
			MoveAbsolutePosition,
			MoveIncrementalPosition,
		};

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

			// Axl Motion 장치를 선언 // Declare Axl Motion device
			CDeviceMotionAxl devMotion = new CDeviceMotionAxl();

			do
			{
				String strInput = "";

				// 모션 파일의 전체 경로를 입력합니다. // Enter the full path of the motion file.
				Console.Write("Enter motion file full path (e.g. C:/Motion/Any.mot): ");
				strInput = Console.ReadLine();

				String strMotionPath = strInput;
				strMotionPath.Replace("\r", "");
				strMotionPath.Replace("\n", "");

				// 모션 파일의 경로를 설정합니다. // Sets the path to the motion file.
				if((res = devMotion.SetMotionFilePath(strMotionPath)).IsFail())
				{
					ErrorPrint(res, "Failed to set motion file path.");
					break;
				}

				// 연결할 축 개수를 설정합니다. // Sets the number of axes to connect to.
				if((res = devMotion.SetAxisCount(1)).IsFail())
				{
					ErrorPrint(res, "Failed to set axis count.");
					break;
				}

				// 모션 장치를 초기화 합니다. // Initialize the motion device.
				if((res = devMotion.Initialize()).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the device.");
					break;
				}

				// 모션 축 객체를 얻어옵니다. // Obtain motion axis objects.
				CDeviceMotionAxlAxis motionAxis = devMotion.GetMotionAxis(0) as CDeviceMotionAxlAxis;

				if(motionAxis == null)
				{
					Console.Write("Failed to get motion axis.\n");
					break;
				}

				// 서보를 켭니다. // Turn on the servo.
				if((res = motionAxis.SetServoOn(true)).IsFail())
				{
					ErrorPrint(res, "Failed to servo on.\n");
					break;
				}

				do
				{
					CThreadUtilities.Sleep(100);
				}
				while(!motionAxis.IsServoOn());

				while(true)
				{
					EMotionFeature eMotionFeature = EMotionFeature.MoveAbsolutePosition;
					bool bExit = false;

					while(true)
					{
						// 사용할 모션 기능을 선택합니다. // Select the motion feature you want to use.
						Console.Write("\n");
						Console.Write("1. Search Origin Position\n");
						Console.Write("2. Move Absolute Position\n");
						Console.Write("3. Move Incremental Position\n");
						Console.Write("0. Exit\n");
						Console.Write("Select: ");
						strInput = Console.ReadLine();

						int i32Select = 0;
						Int32.TryParse(strInput, out i32Select);

						bool bSelected = true;

						switch(i32Select)
						{
						case 0:
							bExit = true;
							break;
						case 1:
							eMotionFeature = EMotionFeature.SearchOriginPosition;
							break;
						case 2:
							eMotionFeature = EMotionFeature.MoveAbsolutePosition;
							break;
						case 3:
							eMotionFeature = EMotionFeature.MoveIncrementalPosition;
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

					if(eMotionFeature == EMotionFeature.SearchOriginPosition)
					{
						// 원점 복귀 동작을 진행합니다. // Proceed with the return-to-origin action.
						if((res = motionAxis.SearchOriginPosition()).IsFail())
						{
							ErrorPrint(res, "Failed to search origin position.\n");
							break;
						}

						// 원점 복귀 동작이 완료 될때까지 대기합니다. // Wait until the return to origin action is complete.
						do
						{
							CThreadUtilities.Sleep(100);
						}
						while(motionAxis.GetSearchOriginStatus() == CDeviceMotionAxlAxis.ESearchOriginStatus.Searching);

						if(motionAxis.GetSearchOriginStatus() == CDeviceMotionAxlAxis.ESearchOriginStatus.Error)
							Console.Write("Failed to search origin position.\n");
						else
							Console.Write("Successed to search origin position.\n");
					}
					else
					{
						double f64MoveVelocity = 0;
						double f64MoveAccelAndDecel = 0;
						double f64MovePosition = 0;

						// 이동 속도를 입력합니다. // Enter the velocity of movement.
						Console.Write("Enter Axis Velocity(mm/s): ");
						strInput = Console.ReadLine();

						Double.TryParse(strInput, out f64MoveVelocity);

						// 가감속을 입력합니다. // Enter acceleration and deceleration.
						Console.Write("Enter Axis Acceleration(mm/s^2): ");
						strInput = Console.ReadLine();

						Double.TryParse(strInput, out f64MoveAccelAndDecel);

						// 이동거리나 절대위치를 입력합니다. // Enter the distance or absolute position.
						Console.Write("Enter Axis Position(mm): ");
						strInput = Console.ReadLine();

						Double.TryParse(strInput, out f64MovePosition);

						if(eMotionFeature == EMotionFeature.MoveAbsolutePosition)
						{
							// 절대 좌표로 이동합니다. // Move to absolute coordinates.
							if((res = motionAxis.MovePosition(f64MovePosition, f64MoveVelocity, f64MoveAccelAndDecel, f64MoveAccelAndDecel, false)).IsFail())
							{
								ErrorPrint(res, "Failed to move position.\n");
								break;
							}

							// 모션이 정지 될때까지 대기합니다. // Wait until motion stops.
							do
							{
								CThreadUtilities.Sleep(100);
							}
							while(!motionAxis.IsMotionDone());
						}
						else if(eMotionFeature == EMotionFeature.MoveIncrementalPosition)
						{
							// 상대 좌표로 이동합니다. // Move to relative coordinates.
							if((res = motionAxis.MoveDistance(f64MovePosition, f64MoveVelocity, f64MoveAccelAndDecel, f64MoveAccelAndDecel, false)).IsFail())
							{
								ErrorPrint(res, "Failed to move distance.\n");
								break;
							}

							// 모션이 정지 될때까지 대기합니다. // Wait until motion stops.
							do
							{
								CThreadUtilities.Sleep(100);
							}
							while(!motionAxis.IsMotionDone());
						}
					}
				}
			}
			while(false);

			// Motion 장치의 초기화를 해제합니다. // Terminate the Motion device.
			devMotion.Terminate();

			if(res.IsFail())
				Console.ReadLine();
		}
    }
}

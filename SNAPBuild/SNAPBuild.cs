using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
    class SNAPBuild
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

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult res;

			do
			{
				// 스냅 빌드 객체 선언 // Declare SNAP Build
		        CSNAPBuild snapBuild = new CSNAPBuild();

		        // 스냅 파일 로드 // Load SNAP file
                if ((res = snapBuild.Load("C:/Users/Public/Documents/FLImaging/FLImagingExamplesSNAP/Advanced Functions/Object/Blob.flsf")).IsFail())
				{
			        ErrorPrint(res, "Failed to load the SNAP file.\n");
			        break;
		        }

		        // 스냅 실행 // Run SNAP
		        res = snapBuild.Run();

		        // 스냅이 종료될 때 까지 기다림 // Wait for the SNAP to close
		        while(snapBuild.IsAvailable())
			        Thread.Sleep(1);
			}
			while(false);
		}
	}
}

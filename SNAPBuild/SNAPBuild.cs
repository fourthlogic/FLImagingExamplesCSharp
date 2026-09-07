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
                if ((res = snapBuild.Load("..\\SNAPBuild\\Example.flsf")).IsFail())
				{
					ErrorPrint(res, "Failed to load the SNAP file.\n");
					break;
				}

				// 소스 이미지 노드를 찾습니다. // Finds the source image node.
				CSNAPObjectNode nodeSourceImage = snapBuild.FindNode("Image", "Source Image");

				if(nodeSourceImage == null)
				{
					res = new CResult(EResult.FailedToFind);
					ErrorPrint(res, "Failed to find the node.\n");
					break;
				}

				// 임계값 처리 결과 노드를 찾습니다. // Finds the threshold result node.
				CSNAPObjectNode nodeThresholdResult = snapBuild.FindNode("MultiVar<Double>", "Threshold Result");

				if(nodeThresholdResult == null)
				{
					res = new CResult(EResult.FailedToFind);
					ErrorPrint(res, "Failed to find the node.\n");
					break;
				}

				CFLImage fliSource = new CFLImage();

				// 소스 이미지를 로드합니다. // Loads the source image.
				if((res = fliSource.Load("..\\..\\ExampleImages\\Blob\\Ball.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image.\n");
					break;
				}

				// 노드에 소스 이미지를 설정합니다. // Sets the source image to the node.
				if((res = nodeSourceImage.SetParameter("Image", fliSource)).IsFail())
				{
					ErrorPrint(res, "Failed to set the parameter.\n");
					break;
				}

				// 스냅 실행 // Run SNAP
				res = snapBuild.Run();

				// 스냅 실행이 종료 될때까지 대기합니다. // Waits until the SNAP run is complete.
				snapBuild.WaitStop();

				List<double> listThresholdResult = new List<double>();

				// 노드에서 임계값 처리 결과를 얻어옵니다. // Gets the threshold result from the node.
				res = nodeThresholdResult.GetParameter("Multi Variable", listThresholdResult);

				// 스냅이 종료될 때 까지 기다림 // Wait for the SNAP to close
				while(snapBuild.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

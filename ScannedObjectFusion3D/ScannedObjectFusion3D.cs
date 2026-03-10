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
using FLImagingCLR.ThreeDim;

namespace FLImagingExamplesCSharp
{
	class ScannedObjectFusion3D
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

			// 이미지 뷰 선언 // Declare image view	
			int i32SourceCount = 6;
			CGUIView3D view3DDst = new CGUIView3D();
			CGUIView3D[] arrSourceView = new CGUIView3D[6];
			CFL3DObject[] arrSourceObjects = new CFL3DObject[6];
			CFL3DObject floDst = new CFL3DObject();

			CResult res = new CResult();

			do
			{
				// ScannedObjectFusion3D 객체 생성 // Create ScannedObjectFusion3D object
				CScannedObjectFusion3D scannedObjectFusion3D = new CScannedObjectFusion3D();

				// Source object 로드 // load the source object									
				for(int i = 0; i < i32SourceCount; ++i)
				{
					arrSourceView[i] = new CGUIView3D();
					arrSourceObjects[i] = new CFL3DObject();

					string strPath = string.Format("../../ExampleImages/ScannedObjectFusion3D/{0}.fl3do", i);

					if((res = arrSourceObjects[i].Load(strPath)).IsFail())
					{
						//ErrorPrint(res, L"Failed to load the object.\n");
						break;
					}

					scannedObjectFusion3D.AddSourceObject(ref arrSourceObjects[i]);
				}

				int i32WindowWidth = 300;
				int i32WindowHeight = 300;

				for(int i = 0; i < i32SourceCount / 3; ++i)
				{
					int i32Height = i32WindowHeight * i;

					for(int j = 0; j < i32SourceCount / 2; ++j)
					{
						int i32Width = i32WindowWidth * j;

						arrSourceView[i * 3 + j].Create(10 + i32Width, 10 + i32Height, 10 + i32Width + i32WindowWidth, 10 + i32Height + i32WindowHeight);
					}
				}

				if((res = view3DDst.Create(910, 10, 1510, 610)).IsFail())
					break;

				for(int i = 1; i < i32SourceCount; ++i)
					arrSourceView[0].SynchronizeWindow(ref arrSourceView[i]);

				CFLPoint<double> flpTopLeft = new CFLPoint<double>(0, 0);

				for(int i = 0; i < i32SourceCount; ++i)
				{
					arrSourceView[i].PushObject(arrSourceObjects[i]);
					arrSourceView[i].ZoomFit();

					CGUIView3DLayer layer3DSrc = arrSourceView[i].GetLayer(0);
					string strName = string.Format("Scene {0}", i);

					if(layer3DSrc.DrawTextCanvas(flpTopLeft, strName, EColor.YELLOW, EColor.BLACK, 20).IsFail())
						continue;
				}

				// 샘플링 거리 설정 // Set the sampling distance
				scannedObjectFusion3D.SetSamplingDistance(0.03f);
				// 기준 좌표계 설정 // Set the base coordinate system
				scannedObjectFusion3D.SetBaseCoordinateFrame(0);
				// 데이터 취득 타입 설정 // Set the data acquisition type
				scannedObjectFusion3D.SetAcquisitionType(CScannedObjectFusion3D.EAcquisitionType.Unordered);
				// Destination object 설정 // Set the destination object
				scannedObjectFusion3D.SetDestinationObject(ref floDst);

				if((res = scannedObjectFusion3D.Calibrate()).IsFail())
				{
					//ErrorPrint(res, "Failed to Calibrate\n");
					break;
				}

				view3DDst.PushObject(floDst);

				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				if((res = layer3DDst.DrawTextCanvas(flpTopLeft, "Calibration Result", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					//ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 뷰를 갱신 // Update view
				for(int i = 0; i < i32SourceCount; ++i)
					arrSourceView[i].Invalidate(true);

				view3DDst.ZoomFit();
				view3DDst.Invalidate(true);

				// 뷰가 종료될 때까지 기다림 // Wait for the view to close
				while(view3DDst.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

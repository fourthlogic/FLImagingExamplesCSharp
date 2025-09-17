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
using System.Diagnostics;
using System.Collections;

namespace FLImagingExamplesCSharp
{
	class PointCloudResampler3D
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

			// 이미지 뷰 선언 // Declare the image view
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DUpsample = new CGUIView3D();
			CGUIView3D view3DDownsample = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				if((res = view3DSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = view3DUpsample.Create(600, 0, 1100, 500)).IsFail() ||
					(res = view3DDownsample.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				view3DSrc.SetTopologyType(ETopologyType3D.PointCloud);
				view3DUpsample.SetTopologyType(ETopologyType3D.PointCloud);
				view3DDownsample.SetTopologyType(ETopologyType3D.PointCloud);

				view3DSrc.PushObject(new CFL3DObject());
				var viewObjectSrc = view3DSrc.GetView3DObject(0);
				var floSrc = viewObjectSrc.Get3DObject();

				view3DUpsample.PushObject(new CFL3DObject());
				var viewObjectUpsample = view3DUpsample.GetView3DObject(0);
				var floUpsample = viewObjectUpsample.Get3DObject();

				view3DDownsample.PushObject(new CFL3DObject());
				var viewObjectDownsample = view3DDownsample.GetView3DObject(0);
				var floDownsample = viewObjectDownsample.Get3DObject();

				if((res = floSrc.Load("../../ExampleImages/CoordinateFrameUnification3D/Office_mosaicked(Middle).ply")).IsFail())
				{
					ErrorPrint(res, "Failed to load source object.");
					break;
				}

				CPointCloudResampler3D pointCloudResampler3D = new CPointCloudResampler3D();

				// 파라미터 설정 // Set parameter
				pointCloudResampler3D.SetSourceObject(ref floSrc);
				pointCloudResampler3D.SetColoringMode(EColoringMode.Interpolate);
				pointCloudResampler3D.EnableNormalInterpolation(true);
				pointCloudResampler3D.SetSamplingMode(CPointCloudResampler3D.ESamplingMode.Ratio_Strict);
				pointCloudResampler3D.EnableFaceReconstruction(false);
				pointCloudResampler3D.EnableFaceRetainment(false);

				pointCloudResampler3D.SetDestinationObject(ref floUpsample);
				pointCloudResampler3D.SetSampleRatio(20);

				if((res = pointCloudResampler3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				pointCloudResampler3D.SetDestinationObject(ref floDownsample);
				pointCloudResampler3D.SetSampleRatio(0.15);


				if((res = pointCloudResampler3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}
				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DUpsample = view3DUpsample.GetLayer(0);
				CGUIView3DLayer layer3DDownsample = view3DDownsample.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DSrc.Clear();
				layer3DUpsample.Clear();
				layer3DDownsample.Clear();

				CFLPoint<double> flpTopLeft = new CFLPoint<double>(0, 0);

				if((res = layer3DSrc.DrawTextCanvas(flpTopLeft, "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layer3DUpsample.DrawTextCanvas(flpTopLeft, "Destination Object(Upsample)", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					(res = layer3DDownsample.DrawTextCanvas(flpTopLeft, "Destination Object(Downsample)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				viewObjectSrc.UpdateAll();
				view3DSrc.UpdateObject(0);

				viewObjectUpsample.UpdateAll();
				view3DUpsample.UpdateObject(0);

				viewObjectDownsample.UpdateAll();
				view3DDownsample.UpdateObject(0);

				view3DSrc.SynchronizePointOfView(ref view3DUpsample);
				view3DSrc.SynchronizeWindow(ref view3DUpsample);
				view3DSrc.SynchronizePointOfView(ref view3DDownsample);
				view3DSrc.SynchronizeWindow(ref view3DDownsample);

				CFL3DCamera cam = new CFL3DCamera();

				cam.SetProjectionType(E3DCameraProjectionType.Perspective);
				cam.SetDirection(new CFLPoint3<float>(0.327705, 0.066764, -0.942418));
				cam.SetDirectionUp(new CFLPoint3<float>(0.311277, 0.839746, -0.444896));
				cam.SetPosition(new CFLPoint3<float>(-1.825832, 0.425620, 1.548716));
				cam.SetAngleOfViewY(45);

				view3DUpsample.SetCamera(cam);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSrc.IsAvailable() && view3DUpsample.IsAvailable() || view3DDownsample.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

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

namespace PerspectiveMerge3D
{
	class Program
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
			CResult res = new CResult();
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DSrc2 = new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();
			CGUIViewImage viewTestDescription = new CGUIViewImage();

			do
			{
				// Source 3D 뷰 생성 // Create the Source 3D view
				if((res = view3DSrc.Create(100, 0, 600, 500)).IsFail() ||
				   (res = view3DSrc2.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// Destination 3D 뷰 생성 // Create the destination 3D view
				if((res = view3DDst.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				if((res = viewTestDescription.Create(100, 500, 600, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				CFL3DObject fl3DOSrc = new CFL3DObject();
				CFL3DObject fl3DOSrc2 = new CFL3DObject();
				res = fl3DOSrc.Load("../../ExampleImages/PerspectiveMerge3D/Left Cam.ply");
				res = fl3DOSrc2.Load("../../ExampleImages/PerspectiveMerge3D/Right Cam.ply");

				CPerspectiveMerge3D algObject = new CPerspectiveMerge3D();

				CFL3DObject fl3DODst = new CFL3DObject();

				algObject.SetEulerSequence(EEulerSequence.Extrinsic_ZXY);

				TPoint3<float> tpPosition = new TPoint3<float>(-0.152f, 0.0f, 0f);
				TPoint3<float> tpRotation = new TPoint3<float>(-90f, 8f, -29f);
				TPoint3<float> tpPosition2 = new TPoint3<float>(0.152f, 0.0f, 0f);
				TPoint3<float> tpRotation2 = new TPoint3<float>(-90f, 8f, 29f);

				// 카메라 1, 2의 Source 객체 설정 // Set the source object of camera 1, 2
				algObject.AddSourceObject(ref fl3DOSrc, tpPosition, tpRotation);
				algObject.AddSourceObject(ref fl3DOSrc2, tpPosition2, tpRotation2);
				// Destination 객체 설정 // Set the destination object
				algObject.SetDestinationObject(ref fl3DODst);

				// 앞서 설정된 파라미터대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = algObject.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				view3DSrc.PushObject(fl3DOSrc);
				view3DSrc2.PushObject(fl3DOSrc2);
				view3DDst.PushObject(algObject.GetDestinationObject());

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다. // With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				view3DSrc.ZoomFit();
				view3DSrc2.ZoomFit();
				view3DDst.ZoomFit();

				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DSrc2 = view3DSrc2.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layer3DSrc.DrawTextCanvas(flp, ("Left Camera"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DSrc2.DrawTextCanvas(flp, ("Right Camera"), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DDst.DrawTextCanvas(flp, ("Result"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				CFLImage fliTestDescription = new CFLImage();
				fliTestDescription.Load("../../ExampleImages/PerspectiveMerge3D/Test Environment.flif");
				viewTestDescription.SetImagePtr(ref fliTestDescription);
				CGUIViewImageLayer layerTestDescription = viewTestDescription.GetLayer(0);

				if((res = layerTestDescription.DrawTextCanvas(flp, ("Test Environment"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSrc.Invalidate(true);
				view3DSrc2.Invalidate(true);
				view3DDst.Invalidate(true);
				viewTestDescription.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSrc.IsAvailable() && view3DSrc2.IsAvailable() && view3DDst.IsAvailable() && viewTestDescription.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

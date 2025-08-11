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
	class PointCloudUpsamplerPoisson3D
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
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 뷰 선언 // Declare the image view
			CGUIView3D view3DSrc= new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

            do
			{
                if ((res = view3DSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = view3DDst.Create(600, 0, 1100, 500)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
				}

				view3DSrc.SetTopologyType(ETopologyType3D.PointCloud);
				view3DDst.SetTopologyType(ETopologyType3D.PointCloud);

				view3DSrc.PushObject(new CFL3DObject());
				var viewObjectSrc = view3DSrc.GetView3DObject(0);
				var floSrc = viewObjectSrc.Get3DObject();

				view3DDst.PushObject(new CFL3DObject());
				var viewObjectDst = view3DDst.GetView3DObject(0);
				var floDst = viewObjectDst.Get3DObject();

				if((res = floSrc.Load("../../ExampleImages/CoordinateFrameUnification3D/Office_mosaicked(Middle).ply")).IsFail())
				{
					ErrorPrint(res, "Failed to load source object.");
					break;
				}

				CPointCloudUpsamplerPoisson3D pointCloudUpsamplerPoisson3D = new CPointCloudUpsamplerPoisson3D();

				// 파라미터 설정 // Set parameter
				pointCloudUpsamplerPoisson3D.SetSourceObject(ref floSrc);
				pointCloudUpsamplerPoisson3D.SetDestinationObject(ref floDst);
				pointCloudUpsamplerPoisson3D.SetColoringMode(CPointCloudUpsampler3DBase.EColoringMode.Interpolate);
				pointCloudUpsamplerPoisson3D.EnableNormalInterpolation(true);
				pointCloudUpsamplerPoisson3D.EnableAutoDistance(true);
				pointCloudUpsamplerPoisson3D.EnableCopyVertex(true);
				pointCloudUpsamplerPoisson3D.EnableFaceReconstruction(false);
				pointCloudUpsamplerPoisson3D.EnableFaceRetainment(false);
				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DSrc.Clear();
				layer3DDst.Clear();


				CFLPoint<double> flpTopLeft = new CFLPoint<double>(0, 0);
				
				if((res = layer3DSrc.DrawTextCanvas(flpTopLeft, "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() || 
					(res = layer3DDst.DrawTextCanvas(flpTopLeft, "Destination Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = pointCloudUpsamplerPoisson3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

				viewObjectSrc.UpdateAll();
				view3DSrc.UpdateObject(0);

				viewObjectDst.UpdateAll();
				view3DDst.UpdateObject(0);

				view3DSrc.SynchronizePointOfView(ref view3DDst);
				view3DSrc.SynchronizeWindow(ref view3DDst);

				CFL3DCamera cam = new CFL3DCamera();

				cam.SetProjectionType(E3DCameraProjectionType.Perspective);
				cam.SetDirection(new CFLPoint3<float>(0.327705, 0.066764, -0.942418));
				cam.SetDirectionUp(new CFLPoint3<float>(0.311277, 0.839746, -0.444896));
				cam.SetPosition(new CFLPoint3<float>(-1.825832, 0.425620, 1.548716));
				cam.SetAngleOfViewY(45);

				view3DDst.SetCamera(cam);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DDst.IsAvailable())
					Thread.Sleep(1);
            }
            while (false);
        }
	}
}

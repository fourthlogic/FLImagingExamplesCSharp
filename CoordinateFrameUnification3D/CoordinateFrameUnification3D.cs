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
	class CoordinateFrameUnification3D
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
			CGUIView3D view3DSrc0 = new CGUIView3D();
			CGUIView3D view3DSrc1 = new CGUIView3D();
			CGUIView3D view3DWorld = new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();
			CFL3DObject floSource0 = new CFL3DObject();
			CFL3DObject floSource1 = new CFL3DObject();
			CFL3DObject floWorld = new CFL3DObject();
			CFL3DObject fl3DObjectDst = new CFL3DObject();

			CResult res = new CResult();

			do
			{
				// 데이터 로드 // Load data
				floSource0.Load("../../ExampleImages/CoordinateFrameUnification3D/Office_mosaicked(Left).ply");
				floWorld.Load("../../ExampleImages/CoordinateFrameUnification3D/Office_mosaicked(Middle).ply");
				floSource1.Load("../../ExampleImages/CoordinateFrameUnification3D/Office_mosaicked(Right).ply");

				CCoordinateFrameUnification3D cfu = new CCoordinateFrameUnification3D();

				// Scene 0와 World 좌표 간 점 대응을 추가
				// Add point correpondence between Scene 0 & World

				var flaWorld0 = new List<TPoint3<float>>(4);
				var flaScene0 = new List<TPoint3<float>>(4);


				flaWorld0.Add(new TPoint3<float>(0.316194f, 0.089235f, -0.955000f));
				flaScene0.Add(new TPoint3<float>(0.048920f, 0.131229f, -0.824725f));
				flaWorld0.Add(new TPoint3<float>(0.328092f, 0.086743f, -0.952000f));
				flaScene0.Add(new TPoint3<float>(0.062442f, 0.128631f, -0.826201f));
				flaWorld0.Add(new TPoint3<float>(0.465690f, 0.065212f, -0.920000f));
				flaScene0.Add(new TPoint3<float>(0.202130f, 0.117711f, -0.854954f));
				flaWorld0.Add(new TPoint3<float>(0.339934f, -0.020669f, -0.646000f));
				flaScene0.Add(new TPoint3<float>(0.189541f, -0.046209f, -0.589000f));

				cfu.AddSourceObject(ref floSource0, flaWorld0, flaScene0);

				// Scene 1과 World 좌표 간 점 대응을 추가
				// Add point correpondence between Scene 1 & World
				var flaWorld1 = new List<TPoint3<float>>(6);
				var flaScene1 = new List<TPoint3<float>>(6);

				flaWorld1.Add(new TPoint3<float>(-0.553926f, 0.204508f, -1.155000f));
				flaScene1.Add(new TPoint3<float>(0.202496f, 0.448916f, -0.853000f));
				flaWorld1.Add(new TPoint3<float>(-0.552240f, 0.189193f, -1.160931f));
				flaScene1.Add(new TPoint3<float>(0.208646f, 0.434687f, -0.859625f));
				flaWorld1.Add(new TPoint3<float>(-0.479978f, 0.192098f, -1.145000f));
				flaScene1.Add(new TPoint3<float>(0.251620f, 0.415887f, -0.796545f));
				flaWorld1.Add(new TPoint3<float>(-0.477483f, 0.173172f, -1.146783f));
				flaScene1.Add(new TPoint3<float>(0.258778f, 0.401190f, -0.810000f));
				flaWorld1.Add(new TPoint3<float>(-0.406276f, -0.267226f, -0.835000f));
				flaScene1.Add(new TPoint3<float>(0.138451f, -0.120545f, -0.677569f));
				flaWorld1.Add(new TPoint3<float>(-0.016503f, -0.275241f, -1.050700f));
				flaScene1.Add(new TPoint3<float>(0.568925f, -0.122618f, -0.588000f));

				cfu.AddSourceObject(ref floSource1, flaWorld1, flaScene1);

				CFL3DObject floDestination = new CFL3DObject();
				cfu.SetDestinationObject(ref floDestination);


				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = cfu.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// 3D 뷰 생성 // Create the 3D views
				if((res = view3DSrc0.Create(100, 250, 600, 750)).IsFail() ||
				   (res = view3DWorld.Create(600, 0, 1100, 500)).IsFail() ||
				   (res = view3DSrc1.Create(1100, 250, 1600, 750)).IsFail() ||
				   (res = view3DDst.Create(600, 500, 1100, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3d view.\n");
					break;
				}

				view3DDst.SynchronizeWindow(ref view3DSrc0);
				view3DDst.SynchronizeWindow(ref view3DWorld);
				view3DDst.SynchronizeWindow(ref view3DSrc1);

				view3DSrc0.PushObject(floSource0);
				view3DWorld.PushObject(floWorld);
				view3DSrc1.PushObject(floSource1);
				view3DDst.PushObject(floDestination);

				view3DDst.SynchronizePointOfView(ref view3DSrc0);
				view3DDst.SynchronizePointOfView(ref view3DWorld);
				view3DDst.SynchronizePointOfView(ref view3DSrc1);


				// 3D 뷰에 카메라 위치 직접 세팅 // Set 3d view camera pose manually
				CFL3DCamera cam = new CFL3DCamera();

				cam.SetProjectionType(E3DCameraProjectionType.Perspective);
				cam.SetDirection(new CFLPoint3<float>(0.337466, -0.125061, -0.932993));
				cam.SetDirectionUp(new CFLPoint3<float>(0.139977, 0.986837, -0.080987));
				cam.SetPosition(new CFLPoint3<float>(-0.70, 0.16, 1.0));
				cam.SetAngleOfViewY(45);

				view3DWorld.SetCamera(cam);

				CGUIView3DLayer layer3DSrc0 = view3DSrc0.GetLayer(0);
				CGUIView3DLayer layer3DSrc1 = view3DSrc1.GetLayer(0);
				CGUIView3DLayer layer3DWorld = view3DWorld.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				CFLPoint<double> flpTopLeft = new CFLPoint<double>(0, 0);

				if((res = layer3DSrc0.DrawTextCanvas(flpTopLeft, "Scene 0", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DWorld.DrawTextCanvas(flpTopLeft, "World(Reference)", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DSrc1.DrawTextCanvas(flpTopLeft, "Scene 1", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DDst.DrawTextCanvas(flpTopLeft, "Result", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 뷰를 갱신 // Update view
				view3DSrc0.Invalidate(true);
				view3DWorld.Invalidate(true);
				view3DSrc1.Invalidate(true);
				view3DDst.Invalidate(true);

				// 뷰가 종료될 때까지 기다림 // Wait for the view to close
				while(view3DSrc0.IsAvailable() && view3DSrc1.IsAvailable() && view3DWorld.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

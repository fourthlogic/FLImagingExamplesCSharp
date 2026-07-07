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
	class MeshSimplifierGridCluster3D
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

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D[] view3DDst = new CGUIView3D[4];
			for(int i = 0; i < 4; ++i)
				view3DDst[i] = new CGUIView3D();

			CGUIView3D view3DSource = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				if((res = view3DSource.Create(0, 0, 500, 500)).IsFail() ||
				   (res = view3DDst[0].Create(500, 0, 1000, 500)).IsFail() ||
				   (res = view3DDst[1].Create(500, 500, 1000, 1000)).IsFail() ||
				   (res = view3DDst[2].Create(1000, 0, 1500, 500)).IsFail() ||
				   (res = view3DDst[3].Create(1000, 500, 1500, 1000)).IsFail())
				{
					ErrorPrint(res, "Failed To Create Views");
					break;
				}

				for(int i = 0; i < 4; ++i)
				{
					view3DDst[i].SynchronizePointOfView(ref view3DSource);
					view3DDst[i].SynchronizeWindow(ref view3DSource);
				}

				// 소스 오브젝트 생성 // Make Source Object
				CFL3DObject floPointCloud = new CFL3DObject();
				int i32Count = (int)1e6;
				{
					CXorshiroRandomGenerator rng = new CXorshiroRandomGenerator();
					rng.Seed(42);

					List<TPoint3<float>> listVertices = new List<TPoint3<float>>();

					for(int i = 0; i < i32Count; ++i)
					{
						TPoint3<float> tp = rng.GenerateUniformRandomPointOnUnitSphereF32();
						listVertices.Add(new TPoint3<float>(tp.x * 2.0f, tp.y * 3.0f, tp.z * -3.5f));
					}

					floPointCloud.SetVertices(listVertices);
				}

				CFL3DObject floMesh = new CFL3DObject();
				CConvexHull3D convexHull3D = new CConvexHull3D();
				convexHull3D.SetSourceObject(ref floPointCloud);
				convexHull3D.SetDestinationObject(ref floMesh);
				convexHull3D.EnableNormalRecalculation(true);
				convexHull3D.EnablePreserveVertex(false);

				if((res = convexHull3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed To Execute Convex Hull.");
					break;
				}

				view3DSource.PushObject(floMesh);
				view3DSource.ZoomFit();

				// 소스 뷰에 메시지 출력 // Display message on the source view
				CGUIView3DLayer layer3DSource = view3DSource.GetLayer(0);
				layer3DSource.Clear();
				layer3DSource.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source View", EColor.YELLOW, EColor.BLACK, 20);

				// CMeshSimplifierGridCluster3D 실행 // Execute CMeshSimplifierGridCluster3D
				CMeshSimplifierGridCluster3D meshSimplifierGridCluster3D = new CMeshSimplifierGridCluster3D();

				int[] arrI32SampleSize = { 3333, 1000, 350, 100 };

				for(int i = 0; i < 4; ++i)
				{
					CFL3DObject floResult = new CFL3DObject();

					meshSimplifierGridCluster3D.SetSourceObject(ref floMesh);
					meshSimplifierGridCluster3D.SetDestinationObject(ref floResult);
					meshSimplifierGridCluster3D.SetSamplingSize(arrI32SampleSize[i]);
					meshSimplifierGridCluster3D.SetClusteringMethod(CMeshSimplifierGridCluster3D.EClusteringMethod.FastGrid);

					if((res = meshSimplifierGridCluster3D.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed To Execute Mesh Simplifier Grid Cluster.");
						break;
					}

					view3DDst[i].PushObject(floResult);

					// 목적지 뷰에 메시지 출력 // Display message on the destination view
					CGUIView3DLayer layer3DDst = view3DDst[i].GetLayer(0);
					layer3DDst.Clear();
					layer3DDst.DrawTextCanvas(new CFLPoint<double>(0, 0), String.Format("Destination(Count: {0})", arrI32SampleSize[i]), EColor.YELLOW, EColor.BLACK, 20);
				}

				while(view3DSource.IsAvailable() && view3DDst[0].IsAvailable() && view3DDst[1].IsAvailable() && view3DDst[2].IsAvailable() && view3DDst[3].IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

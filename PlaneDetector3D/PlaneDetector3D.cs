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
	class PlaneDetector3D
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

			// 3D 객체 선언 // Declare 3D object
			CFL3DObject floSrcObject = new CFL3DObject();
			CFL3DObject floDstObject = new CFL3DObject();

			// 이미지 뷰 선언 // Declare image view
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// Source Object 로드 // Load the Source object
				if((res = floSrcObject.Load("../../ExampleImages/PlaneDetector3D/Source.ply")).IsFail())
				{
					ErrorPrint(res, "Failed to load the object file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = view3DSrc.Create(0, 0, 500, 500)).IsFail() ||
				   (res = view3DDst.Create(500, 0, 1000, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = view3DSrc.SynchronizePointOfView(ref view3DDst)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// Source Object 3D 뷰 생성 // Create the source object 3D view
				if((res = view3DSrc.PushObject(floSrcObject)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D object.\n");
					break;
				}

				// PlaneDetector3D 객체 생성 // Create PlaneDetector3D object
				CPlaneDetector3D planeDetector3D = new CPlaneDetector3D();

				if((res = planeDetector3D.SetSourceObject(ref floSrcObject)).IsFail())
					break;
				if((res = planeDetector3D.SetDestinationObject(ref floDstObject)).IsFail())
					break;

				if((res = planeDetector3D.SetPlaneFittingTarget(CPlaneDetector3D.EPlaneFittingTarget.VertexNormal)).IsFail())
					break;
				if((res = planeDetector3D.SetMaximumParallelPlaneCount(1)).IsFail())
					break;
				if((res = planeDetector3D.SetNormalVectorResolution(new TPoint3<int>(19, 19, 19))).IsFail())
					break;
				if((res = planeDetector3D.SetInlierDistance(0.5)).IsFail())
					break;
				if((res = planeDetector3D.SetMinimumInlierCount(500)).IsFail())
					break;
				if((res = planeDetector3D.SetFitCosineSimilarity(0.995)).IsFail())
					break;
				if((res = planeDetector3D.SetExpandCosineSimilarity(0.99)).IsFail())
					break;
				if((res = planeDetector3D.SetMergeCosineSimilarity(0.99)).IsFail())
					break;

				if((res = planeDetector3D.SetOutliersFilteringMethod(CPlaneDetector3D.EOutliersFilteringMethod.LeastSquare)).IsFail())
					break;
				if((res = planeDetector3D.SetOutliersThresholdCount(1)).IsFail())
					break;
				if((res = planeDetector3D.SetOutliersThreshold(3.00)).IsFail())
					break;
				if((res = planeDetector3D.SetPlaneFittingMethod(CPlaneDetector3D.EPlaneFittingMethod.LeastSquare)).IsFail())
					break;

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = planeDetector3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Plane Detector 3D.");
					break;
				}

				// 3D 이미지 뷰에 3d object 를 디스플레이 // Display the 3d object on the 3D image view
				CGUIView3DObject gvoDst = new CGUIView3DObject();
				if((res = (gvoDst.Get3DObject()).Swap(ref floDstObject)).IsFail())
					break;
				if((res = gvoDst.SetTopologyType((ETopologyType3D)0x1f)).IsFail())
					break;
				if((res = gvoDst.SetPointSize(2)).IsFail())
					break;
				if((res = view3DDst.PushObject(gvoDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DSrc.Clear();
				layer3DDst.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				int i32PlaneCount = planeDetector3D.GetResultPlaneCount();
				CFLPoint<double> flp = new CFLPoint<double>(0, 0);
				String strDstLayerString;
				strDstLayerString = String.Format("Plane Count {0}", i32PlaneCount);
				if((res = layer3DSrc.DrawTextCanvas(flp, "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DDst.DrawTextCanvas(flp, "Destination Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DDst.DrawTextCanvas(new CFLPoint<double>(0, 30), strDstLayerString, EColor.YELLOW, EColor.BLACK, 12)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}
				bool bDrawTextIndex = true;
				bool bDrawTextCount = true;
				bool bDrawTextArea = false;
				// text
				if(bDrawTextIndex || bDrawTextCount || bDrawTextArea)
				{
					CPlaneDetector3D.EPlaneFittingTarget ePlaneFittingTarget = planeDetector3D.GetPlaneFittingTarget();
					bool bVertexNormal = ePlaneFittingTarget == CPlaneDetector3D.EPlaneFittingTarget.VertexNormal;
					List<TPoint3<float>> listResultPlaneCentroids = new List<TPoint3<float>>();
					List<int> listResultPlaneTargetCounts = new List<int>();
					List<float> listResultPlaneTargetAreas = new List<float>();
					if((res = planeDetector3D.GetResultPlaneCentroids(listResultPlaneCentroids)).IsFail())
						break;
					if((res = planeDetector3D.GetResultPlaneTargetCounts(listResultPlaneTargetCounts)).IsFail())
						break;
					if((res = planeDetector3D.GetResultPlaneAreas(listResultPlaneTargetAreas)).IsFail())
						break;

					for(int i32PlaneIndex = 0; i32PlaneIndex < i32PlaneCount; i32PlaneIndex++)
					{
						String strTotal = "";
						String str;
						TPoint3<double> tp3Centroid = new TPoint3<double>();
						tp3Centroid.x = listResultPlaneCentroids[i32PlaneIndex].x;
						tp3Centroid.y = listResultPlaneCentroids[i32PlaneIndex].y;
						tp3Centroid.z = listResultPlaneCentroids[i32PlaneIndex].z;
						int i32PlaneTargetCount = listResultPlaneTargetCounts[i32PlaneIndex];
						float f32PlaneArea = listResultPlaneTargetAreas[i32PlaneIndex];

						if(bDrawTextIndex)
						{
							str = String.Format("[{0}]\n", i32PlaneIndex);
							strTotal = strTotal + str;
						}
						if(bDrawTextCount)
						{
							str = String.Format("Count {0}\n", i32PlaneTargetCount);
							strTotal += str;
						}
						if(bDrawTextArea)
						{
							str = String.Format("Area {0:F3}\n", f32PlaneArea);
							strTotal += str;
						}
						layer3DDst.DrawText3D(tp3Centroid, strTotal, EColor.YELLOW, 0, 9);
					}
				}

				// Zoom Fit
				view3DSrc.ZoomFit();
				view3DDst.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSrc.Invalidate(true);
				view3DDst.Invalidate(true);

				//이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSrc.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);

			}
			while(false);
		}
	}
}

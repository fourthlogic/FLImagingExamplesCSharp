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
	class SphereDetector3D
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

			// 뷰 선언 // Declare view
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DResult = new CGUIView3D();

			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 법선과 색상이 포함된 원본 점군 로드 // Load the source point cloud with normals and colors
				if((res = floSrcObject.Load("../../ExampleImages/SphereDetector3D/Source.fl3do")).IsFail())
				{
					ErrorPrint(res, "Failed to load ../../ExampleImages/SphereDetector3D/Source.fl3do.\n");
					break;
				}

				// 뷰 생성 // Create view
				if((res = view3DSrc.Create(0, 0, 640, 640)).IsFail() ||
				   (res = view3DResult.Create(640, 0, 1280, 640)).IsFail())
				{
					ErrorPrint(res, "Failed to create the source and result 3D views.\n");
					break;
				}

				// 원본 장면을 두 뷰에 표시 // Display the source scene on both views
				if((res = view3DSrc.PushObject(floSrcObject)).IsFail() ||
				   (res = view3DResult.PushObject(floSrcObject)).IsFail())
				{
					ErrorPrint(res, "Failed to display the source point cloud on the 3D views.\n");
					break;
				}

				// 알고리즘 객체 생성 // Create algorithm object
				CSphereDetector3D sphereDetector3D = new CSphereDetector3D();

				// 구의 대략적인 크기와 개수를 설정 // Set the approximate sphere size and count
				// 작은 구의 관측 점도 포함하도록 최소 inlier 개수를 설정
				// Set the minimum inlier count to include the observed points of the small sphere
				if((res = sphereDetector3D.SetSourceObject(ref floSrcObject)).IsFail() ||
				   (res = sphereDetector3D.SetRadiusRange(0.55f, 1.65f)).IsFail() ||
				   (res = sphereDetector3D.SetMaxCount(4)).IsFail() ||
				   (res = sphereDetector3D.SetMinimumInlierCount(40)).IsFail() ||
				   (res = sphereDetector3D.SetDistanceThreshold(0.03f)).IsFail() ||
				   (res = sphereDetector3D.SetFitCosineSimilarity(0.98)).IsFail() ||
				   (res = sphereDetector3D.SetMinimumSurfaceCoverage(0.25f)).IsFail() ||
				   (res = sphereDetector3D.SetScoreItem(CSphereDetector3D.EScoreItem.SurfaceCoverage, CSphereDetector3D.ESortOrder.Descending)).IsFail() ||
				   (res = sphereDetector3D.SetIterationCount(5000)).IsFail() ||
				   (res = sphereDetector3D.SetRefinementIterationCount(10)).IsFail())
				{
					ErrorPrint(res, "Failed to set the Sphere Detector 3D source object or detection parameters.\n");
					break;
				}

				// 알고리즘 수행 // Execute the algorithm
				if((res = sphereDetector3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Sphere Detector 3D.\n");
					break;
				}

				// 결과 개수, inlier 개수 및 구면 Coverage 가져오기
				// Get the result count, inlier counts and surface coverages
				long i64SphereCount = sphereDetector3D.GetResultCount();
				List<long> listInlierCounts = new List<long>();
				List<float> listSurfaceCoverages = new List<float>();

				if((res = sphereDetector3D.GetResultInlierCounts(ref listInlierCounts)).IsFail() ||
				   (res = sphereDetector3D.GetResultSurfaceCoverages(ref listSurfaceCoverages)).IsFail())
				{
					ErrorPrint(res, "Failed to retrieve the result inlier counts or surface coverages.\n");
					break;
				}

				// 화면에 출력하기 위해 3D View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to a 3D view and does not need to be released separately
				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DResult = view3DResult.GetLayer(0);
				string strCount;
				strCount = String.Format("Sphere Count {0}", i64SphereCount);

				if((res = layer3DSrc.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DResult.DrawTextCanvas(new CFLPoint<double>(0, 0), "Sphere Detection Result", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DResult.DrawTextCanvas(new CFLPoint<double>(0, 30), strCount, EColor.YELLOW, EColor.BLACK, 12)).IsFail())
				{
					ErrorPrint(res, "Failed to draw the view titles or sphere count.\n");
					break;
				}

				Console.WriteLine("Sphere Count: {0}", i64SphereCount);

				for(long i = 0; i < i64SphereCount; ++i)
				{
					// 결과 구를 3D ROI로 표시 // Display the result sphere as a 3D ROI
					CFLSphere3<double> flsp = new CFLSphere3<double>();

					if((res = sphereDetector3D.GetResultSphere(ref flsp, i)).IsFail() ||
					   (res = view3DResult.PushBackROI(flsp)).IsFail())
					{
						ErrorPrint(res, "Failed to retrieve a result sphere or display it as a 3D ROI.\n");
						break;
					}

					CFLPoint3<double> flpCenter = flsp.GetCenter3D();
					TPoint3<double> tpCenter = new TPoint3<double>(flpCenter.x, flpCenter.y, flpCenter.z);
					string strText;
					// 구 위에는 번호를, 화면 상단에는 상세 결과를 표시
					// Display the index on the sphere and the detailed results at the top of the view
					strText = String.Format("[{0}]", i);

					if((res = layer3DResult.DrawText3D(tpCenter, strText, EColor.YELLOW, EColor.BLACK, 12)).IsFail())
					{
						ErrorPrint(res, "Failed to draw the result sphere index in 3D.\n");
						break;
					}

					strText = String.Format("[{0}]  Radius: {1:F3}  Inliers: {2}  Coverage: {3:F1}%",
						i, flsp.GetRadius(), listInlierCounts[(int)i], listSurfaceCoverages[(int)i] * 100.0f);

					if((res = layer3DResult.DrawTextCanvas(new CFLPoint<double>(0, 50 + 18 * i), strText, EColor.YELLOW, EColor.BLACK, 11)).IsFail())
					{
						ErrorPrint(res, "Failed to draw the result sphere radius, inlier count and coverage.\n");
						break;
					}

					Console.WriteLine("[{0}] Center: ({1:F3}, {2:F3}, {3:F3}), Radius: {4:F3}, Inliers: {5}, Coverage: {6:F1}%",
						i, tpCenter.x, tpCenter.y, tpCenter.z, flsp.GetRadius(), listInlierCounts[(int)i], listSurfaceCoverages[(int)i] * 100.0f);
				}

				if(res.IsFail())
					break;

				// 원본 장면을 관측 방향에서 바라보기 // View the source scene from the observation direction
				CGUIView3DCamera cameraSrc = view3DSrc.GetCamera();

				if((res = cameraSrc.SetPosition(new CFLPoint3<float>(13.0f, -20.0f, 39.0f))).IsFail() ||
				   (res = cameraSrc.SetTarget(new CFLPoint3<float>(0.0f, 0.0f, 0.0f))).IsFail() ||
				   (res = cameraSrc.SetDirectionUp(new CFLPoint3<float>(0.0f, 0.0f, 1.0f))).IsFail() ||
				   (res = view3DSrc.SetCamera(cameraSrc)).IsFail() ||
				   (res = view3DSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to set the source camera viewpoint or fit the source scene.\n");
					break;
				}

				// 결과 뷰는 원본 뷰와 동일한 시점으로 표시 // Display the result view from the same viewpoint as the source view
				if((res = view3DResult.SetCamera(view3DSrc.GetCamera())).IsFail() ||
				   (res = view3DSrc.SynchronizePointOfView(ref view3DResult)).IsFail())
				{
					ErrorPrint(res, "Failed to copy and synchronize the source and result camera viewpoints.\n");
					break;
				}

				// 뷰 갱신 // Update view
				view3DSrc.Invalidate(true);
				view3DResult.Invalidate(true);

				// 뷰가 종료될 때까지 기다림 // Wait for the views to close
				while(view3DSrc.IsAvailable() && view3DResult.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

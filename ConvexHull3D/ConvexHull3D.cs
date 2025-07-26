using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ThreeDim;
using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
    class ConvexHull3D
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

			CFL3DObject floSrc = new CFL3DObject();
			CFL3DObject floDst = new CFL3DObject();
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();

			do
			{
				// 알고리즘 동작 결과 // Algorithm execution result
				CResult res = new CResult(EResult.UnknownError);

				// 이미지 로드 // Load the image
				if((res = floSrc.Load("../../ExampleImages/ConvexHull3D/RandomPointsOnSphere.ply")).IsFail())
				{
					ErrorPrint(res, "Failed to load the 3D Object.\n");
					break;
				}

				// CConvexHull3D 객체 생성 // Create CConvexHull3D object
				CConvexHull3D convexhull3D = new CConvexHull3D();

				{
					// 3D 뷰 생성 // Create 3D view
					if((res = view3DSrc.Create(100, 0, 612, 512)).IsFail() ||
					   (res = view3DDst.Create(612, 0, 1124, 512)).IsFail())
					{
						ErrorPrint(res, "Failed to create 3D views.\n");
						break;
					}

					// 처리할 3D 객체 설정 // Set 3D object to process
					convexhull3D.SetSourceObject(ref floSrc);
					convexhull3D.SetDestinationObject(ref floDst);

					// 생성될 볼록껍질 객체의 컬러를 설정 // Set color of new 3D object
					convexhull3D.EnableVertexRecoloring(true);
					convexhull3D.SetTargetVertexColor(new TPoint3<byte>(255, 125, 0));

					// 3D 뷰에 표시할 3D 객체 추가 // Add 3D object to the 3D view
					view3DSrc.PushObject(floSrc);
					view3DSrc.SetPointSize(3);

					// 앞서 설정된 파라미터대로 알고리즘 수행 // Execute algorithm according to previously set parameters
					if((res = convexhull3D.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute Convex Hull 3D.");
						break;
					}

					view3DDst.PushObject(floDst);

					// 화면에 출력하기 위해 View에서 레이어 0번을 얻어옴 // Obtain layer number 0 from view for display

					CGUIView3DLayer layerViewSrc = view3DSrc.GetLayer(0);
					CGUIView3DLayer layerViewDst = view3DDst.GetLayer(0);

					// View 정보를 디스플레이 한다. // Display view information
					if((res = layerViewSrc.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Object(Point Cloud)", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
					   (res = layerViewDst.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Object(Convex Hull)",EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text.\n");
						break;
					}

					// 입력 뷰의 시점을 이동
					view3DSrc.ZoomFit();

					// 입력, 출력 뷰의 시점을 맞춤
					view3DSrc.SynchronizePointOfView(ref view3DDst);

					// 입력, 출력 뷰를 하나의 창으로 취급
					view3DSrc.SynchronizeWindow(ref view3DDst);

					view3DSrc.Invalidate();
					view3DDst.Invalidate();

					// 3D 뷰가 종료될 때까지 기다림
					while(view3DSrc.IsAvailable() && view3DDst.IsAvailable())
						CThreadUtilities.Sleep(1);
				}
			}
			while(false);
		}
	}
}

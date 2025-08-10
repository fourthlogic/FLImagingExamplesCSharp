using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
    class ProjectionUtilities3D
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

            // 이미지 뷰 선언 // Declare image view
            CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

            CResult res;

            do
			{
                // 이미지 뷰 생성 // Create image view
                if ((res = viewImage[0].Create(0, 0, 400, 440)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                if ((res = viewImage[1].Create(400, 0, 800, 440)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                if ((res = viewImage[2].Create(800, 0, 1200, 440)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

				// 윈도우의 위치 동기화 // / Synchronize the positions of windows
				viewImage[0].SynchronizeWindow(ref viewImage[1]);
                viewImage[0].SynchronizeWindow(ref viewImage[2]);

				// 3D Object 파일 로드 // Load 3D Object file
				CFL3DObject pObj3D = new CFL3DObject();
                pObj3D.Load("../../ExampleImages/ProjectionUtilities3D/Cylinder.step");

                CFLImage[] fliFinal = { new CFLImage(), new CFLImage(), new CFLImage() };
                CFLImage fliRes = new CFLImage();
                CFLFigureText<int> figureText = new CFLFigureText<int>();

                // CProjectionUtilities3D 객체 생성 // Create CProjectionUtilities3D object
                CProjectionUtilities3D projectionUtilities3D = new CProjectionUtilities3D();

                // CProjectionUtilities3D 객체에 3D Object 를 추가 // Add 3D Object to CProjectionUtilities3D object
                projectionUtilities3D.PushBack3DObject(pObj3D);

                // 결과 이미지 크기 설정 // Set result image size
                projectionUtilities3D.SetResultImageSize(400, 400);

                // 결과 이미지 배경 색상 설정 // Set background color of result image
                projectionUtilities3D.SetBackgroundColorOfResultImage(21, 21, 21);

                // 1-1. 특정 시점의 투영 이미지 얻기 // 1-1. Get projection image from specific viewpoint
                // 카메라 시점 설정 // Set camera viewpoint
                CFL3DCamera camSet1 = new CFL3DCamera();
                camSet1.SetProjectionType(E3DCameraProjectionType.Perspective);
                camSet1.SetPosition(new CFLPoint3<float>(-1.41, -317.67, 280.92));
                camSet1.SetDirection(new CFLPoint3<float>(0.01, 0.87, -0.50));
                camSet1.SetDirectionUp(new CFLPoint3<float>(-0.01, 0.50, 0.87));
                camSet1.SetAngleOfViewY(45);
                camSet1.SetTarget(new CFLPoint3<float>(2.13, -59.49, 132.75));
                camSet1.SetNearZ((float)271.84);
                camSet1.SetFarZ((float)459.30);

                // 카메라 설정 // Set camera
                projectionUtilities3D.SetCamera(camSet1);

                // 프로젝션 수행 // Perform projection
                res = projectionUtilities3D.Execute();

                // 결과 이미지 얻기 // Get result image
                res = projectionUtilities3D.GetResult(ref fliRes);

                // 결과 이미지에 정보 텍스트 추가 // Add information text to result image
                figureText.Set(new CFLPoint<int>(10, 10), "1. Projection(Camera Set 1)", (uint)EColor.YELLOW, (uint)EColor.BLACK, 20, false, 0.0, EFigureTextAlignment.LEFT_TOP, "", 1, 1, EFigureTextFontWeight.BOLD, false);
                fliRes.PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(figureText));

                // 최종 이미지에 투영 결과 이미지 복사 // Copy projection result image to final image
                fliFinal[0].Assign(fliRes);

                // 1-2. 특정 시점의 투영 이미지 얻기 // 1-2. Get projection image from another specific viewpoint
                // 카메라 시점 설정 // Set camera viewpoint
                CFL3DCamera camSet2 = new CFL3DCamera();
                camSet2.SetProjectionType(E3DCameraProjectionType.Perspective);
                camSet2.SetPosition(new CFLPoint3<float>(-80.38, 97.35, 341.92));
                camSet2.SetDirection(new CFLPoint3<float>(0.42, -0.27, -0.86));
                camSet2.SetDirectionUp(new CFLPoint3<float>(0.77, 0.61, 0.19));
                camSet2.SetAngleOfViewY(45);
                camSet2.SetTarget(new CFLPoint3<float>(-5.45, 49.05, 189.72));
                camSet2.SetNearZ((float)148.33);
                camSet2.SetFarZ((float)390.77);

                // 카메라 설정 // Set camera
                projectionUtilities3D.SetCamera(camSet2);

                // 프로젝션 수행 // Perform projection
                res = projectionUtilities3D.Execute();

                // 결과 이미지 얻기 // Get result image
                res = projectionUtilities3D.GetResult(ref fliRes);

                // 결과 이미지에 정보 텍스트 추가 // Add information text to result image
                figureText.Set(new CFLPoint<int>(10, 10), "1. Projection(Camera Set 2)", (uint)EColor.YELLOW, (uint)EColor.BLACK, 20, false, 0.0, EFigureTextAlignment.LEFT_TOP, "", 1, 1, EFigureTextFontWeight.BOLD, false);
                fliRes.PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(figureText));

                // 최종 이미지에 투영 결과 이미지 추가 // Add projection result image to final image
                fliFinal[0].PushBackPage(fliRes);
                // 결과 이미지를 이미지 뷰에 로드 // Load result image into image view
                viewImage[0].SetImagePtr(ref fliFinal[0]);
                viewImage[0].SetFixThumbnailView(true);
				viewImage[0].ShowImageMiniMap(false);
				viewImage[0].ShowPageIndex(false);


				// 2. 카메라 1과 카메라 2 사이의 시점에 대한 프로젝션 // 2. Projection for viewpoints between Camera 1 and Camera 2		

				projectionUtilities3D.SetTopologyType(ETopologyType3D.Wireframe);

				for(int i = 0; i <= 10; ++i)
				{
                    // 카메라 시점 설정 // Set camera viewpoint
                    CFL3DCamera camInterpolation = new CFL3DCamera();
                    float f32T = (float)i * 0.1f;
                    CFL3DCamera.Interpolate(camSet1, camSet2, f32T, ref camInterpolation);

                    // 카메라 설정 // Set camera
                    projectionUtilities3D.SetCamera(camInterpolation);

                    // 프로젝션 수행 // Perform projection
                    res = projectionUtilities3D.Execute();

                    // 결과 이미지 얻기 // Get result image
                    res = projectionUtilities3D.GetResult(ref fliRes);

                    // 결과 이미지에 정보 텍스트 추가 // Add information text to result image
                    String str = String.Format("2. Projection(Camera Interpolation T={1})", i, f32T);
                    figureText.Set(new CFLPoint<int>(10, 10), str, (uint)EColor.YELLOW, (uint)EColor.BLACK, 15, false, 0.0, EFigureTextAlignment.LEFT_TOP, "", 1, 1, EFigureTextFontWeight.SEMIBOLD, false);
                    fliRes.PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(figureText));

                    // 최종 이미지에 투영 결과 이미지 추가 // Add projection result image to final image
                    if (i == 0)
                        fliFinal[1].Assign(fliRes);
                    else
                        fliFinal[1].PushBackPage(fliRes);
                }

                // 결과 이미지를 이미지 뷰에 로드 // Load result image into image view
                viewImage[1].SetImagePtr(ref fliFinal[1]);
                viewImage[1].SetFixThumbnailView(true);
				viewImage[1].ShowImageMiniMap(false);
				viewImage[1].ShowPageIndex(false);


				// 3. Zoom Fit 시점의 이미지 얻기 // 3. Get image at Zoom Fit viewpoint

				projectionUtilities3D.SetTopologyType(ETopologyType3D.PointCloud);
				projectionUtilities3D.SetPointSize((float)5);

				projectionUtilities3D.ZoomFitCamera();

                // 프로젝션 수행 // Perform projection
                res = projectionUtilities3D.Execute();

                // 결과 이미지 얻기 // Get result image
                res = projectionUtilities3D.GetResult(ref fliFinal[2]);

                // 결과 이미지에 정보 텍스트 추가 // Add information text to result image
                figureText.Set(new CFLPoint<int>(10, 10), "3. Projection(ZoomFit)", (uint)EColor.YELLOW, (uint)EColor.BLACK, 20, false, 0.0, EFigureTextAlignment.LEFT_TOP, "", 1, 1, EFigureTextFontWeight.BOLD, false);
                fliFinal[2].PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(figureText));

                // 결과 이미지를 이미지 뷰에 로드 // Load result image into image view
                viewImage[2].SetImagePtr(ref fliFinal[2]);
                viewImage[2].SetFixThumbnailView(true);
				viewImage[2].ShowImageMiniMap(false);
				viewImage[2].ShowPageIndex(false);

				while (viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable())
                    CThreadUtilities.Sleep(1);
            }
            while (false);
        }
    }
}

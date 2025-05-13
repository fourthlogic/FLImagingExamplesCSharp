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
using FLImagingCLR.ImageProcessing;
using CResult = FLImagingCLR.CResult;

namespace Projection3D
{
    class Program
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
        {	// 3D 객체 선언 // Declare 3D object
            CFL3DObject floSrc = new CFL3DObject();
            CFLImage fliDst = new CFLImage();
            // 3D 뷰 선언 // Declare 3D view	

            CGUIView3D view3DSrc = new CGUIView3D();
            CGUIViewImage viewImgDst = new CGUIViewImage();

            do
            {
	            // 알고리즘 동작 결과 // Algorithm execution result
	            CResult res = new CResult(EResult.UnknownError);


	            // Source Object 로드 // Load the Source object
	            if((res = floSrc.Load("../../ExampleImages/Projection3D/icosahedron.ply")).IsFail())
	            {
		            ErrorPrint(res, "Failed to load the object file.\n");
		            break;
	            }

				// Source 3D 뷰 생성 // Create the Source 3D view
				if((res = view3DSrc.Create(612, 0, 1124, 512)).IsFail())
	            {
		            ErrorPrint(res, "Failed to create the Source 3D view.\n");
		            break;
	            }

				// Destination 이미지 뷰 생성 // Create the destination image view
				if((res = viewImgDst.Create(1124, 0, 1636, 512)).IsFail())
	            {
		            ErrorPrint(res, "Failed to create the Destination 3D view.\n");
		            break;
	            }

	            // Source 객체를 3D 뷰에 추가 // push source object to 3D view
	            if((res = view3DSrc.PushObject(floSrc)).IsFail())
	            {
		            ErrorPrint(res, "Failed to display the 3D object.\n");
		            break;
	            }

	            viewImgDst.SynchronizeWindow(ref view3DSrc);

	            // Projection3D 객체 생성 // Create Projection3D object
	            CProjection3D projection3D = new CProjection3D();

				// 알고리즘 대상 설정 // set algorithm target
	            projection3D.SetDestinationImage(ref fliDst);
	            projection3D.SetSourceObject(ref floSrc);

	            //3D View의 카메라 파라미터 값을 초기화하기 위하여 먼저 호출
	            view3DSrc.ZoomFit();

	            CGUIView3DCamera cam = view3DSrc.GetCamera();

	            TPoint3<float> ToTPoint3(CFLPoint3<float> pt)
	            {
		            return new TPoint3<float>(pt.x, pt.y, pt.z);
	            };

	            TPoint3<float> ptCamPos = ToTPoint3(cam.GetPosition());
	            TPoint3<float> ptCamDir = ToTPoint3(cam.GetDirection());
	            TPoint3<float> ptCamDirUp = ToTPoint3(cam.GetDirectionUp());
	            float f32AovX = cam.GetAngleOfViewX();
	            float f32AovY = cam.GetAngleOfViewY();
				float f32TargetDistance = cam.GetDistanceFromTarget();
				
				// 알고리즘 파라미터 설정 // set algorithm parameters
				projection3D.SetCameraPosition(ptCamPos);
	            projection3D.SetCameraDirection(ptCamDir);
	            projection3D.SetDirectionUp(ptCamDirUp);
				projection3D.SetAngleOfView(f32AovX, f32AovY, EAngleUnit.Degree);
	            projection3D.SetWorkingDistance(100);
				projection3D.SetImageSize(512, 512);

				// 앞서 설정된 파라미터대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = projection3D.Execute()).IsFail())
	            {
		            ErrorPrint(res, "Failed to execute Projection 3D.");
		            break;
	            }

				// 결과 이미지를 뷰에 연결 // Map the result image to the destination view
				if((res = viewImgDst.SetImagePtr(ref fliDst)).IsFail())
				{
					ErrorPrint(res, "Failed to set object on the 3d view.\n");
					break;
				}
				//Source View와 동일한 시점을 유지하기 위해, 이미지의 정중앙을 뷰의 중앙에 맞추고 배율을 1로 설정
				//Set view's center to the center of image and scale to 1, to match the viewpoint of the source view
				viewImgDst.SetViewCenterAndScale(new CFLPoint<double>(256, 256), 1.0);

				// 화면에 출력하기 위해 View에서 레이어 0번을 얻어옴 // Obtain layer number 0 from view for display

				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
	            CGUIViewImageLayer layer3DDst = viewImgDst.GetLayer(0);

	            // View 정보를 디스플레이한다. // Display view information
	            // 함수 DrawTextCanvas는 Screen 좌표를 기준으로 하는 문자열을 드로잉한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
	            CFLPoint<double> flpTopLeft = new CFLPoint<double>(0, 0);
	            if((res = layer3DSrc.DrawTextCanvas(flpTopLeft, "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
		            (res = layer3DDst.DrawTextCanvas(flpTopLeft, "Destination Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
	            {
		            ErrorPrint(res, "Failed to draw text.\n");
		            break;
	            }


				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSrc.Invalidate(true);
	            viewImgDst.Invalidate(true);

	            // 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림
	            while(view3DSrc.IsAvailable() && viewImgDst.IsAvailable())
		            CThreadUtilities.Sleep(1);
            }
            while(false);
        }
    }
}
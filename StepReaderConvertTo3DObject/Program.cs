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

namespace StepReaderConvertTo3DObject
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
            // 3D 뷰 선언 // Declare the 3D view
            CGUIView3D[] view3D = { new CGUIView3D(), new CGUIView3D() };

            CResult res;

            do
            {
                // 3D 뷰 생성
                // Create 3D views.
                if ((res = view3D[0].Create(100, 0, 612, 512)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the 3D view.\n");
                    break;
                }

                if ((res = view3D[1].Create(612, 0, 1124, 512)).IsFail())
                {
                    ErrorPrint(res, "Failed to create the 3D view.\n");
                    break;
                }

                view3D[0].SetTopologyType(ETopologyType3D.Wireframe);
                view3D[1].SetTopologyType(ETopologyType3D.Wireframe);

				// 우선 빈 CGUIView3DObject 객체를 뷰에 추가한 후 해당 객체의 인덱스를 i32ReturnIndex 에 얻어 오기
				// First, add an empty CGUIView3DObject object to the view, then retrieve the index of that object into i32ReturnIndex.
				int i32ReturnIndex = -1;
				if((res = view3D[0].PushObject(new CGUIView3DObject(), ref i32ReturnIndex)).IsFail())
				{
					ErrorPrint(res, "Failed to display 3D object.\n");
					break;
				}

				// 뷰에 추가된 CGUIView3DObject 객체를 i32ReturnIndex 를 이용해서 얻어 오기
				// Retrieve the CGUIView3DObject object added to the view using i32ReturnIndex.
				CGUIView3DObject objView3D = view3D[0].GetView3DObject(i32ReturnIndex);
				if(objView3D == null)
				{
					ErrorPrint(res, "Failed to display 3D object.\n");
					break;
				}

				// 뷰에 추가된 CGUIView3DObject 객체의 내부 CFL3DObject 포인터를 얻어 오기
				// Retrieve the internal CFL3DObject pointer of the CGUIView3DObject object added to the view.
				CFL3DObject fl3DObject = objView3D.Get3DObject();
				if(fl3DObject == null)
				{
					ErrorPrint(res, "Failed to display 3D object.\n");
					break;
				}

				//곡선의 접선에서 코드(Chord, 곡선의 두 점을 직선으로 연결한 선분)가 벗어날 수 있는 최대 거리를 나타냅니다. 
				// 이 값이 클수록 분할된 삼각형의 수가 적어지며, 
				// 반대로 값이 작을수록 더 많은 삼각형이 형성되어 곡선을 더 정밀하게 근사합니다. 
				// 기본값 0을 입력하면 step 모델에서 적절한 chordal deviation 값을 자동으로 계산합니다.
				// It represents the maximum distance that a chord (a straight line segment connecting two points on the curve) can deviate from the tangent of the curve. 
				// A larger value results in fewer triangles being formed, 
				// while a smaller value results in more triangles, providing a more precise approximation of the curve. 
				// The default value of 0 automatically calculates the appropriate chordal deviation value based on the imported step model.
				double f64ChordalDeviation = 0;

				// 방법 1. CFL3DObject 에서 Step 파일 로드
				// Method 1. Load the STEP file in CFL3DObject

				// 뷰에 추가된 CGUIView3DObject 객체의 내부 CFL3DObject 에 STEP 파일 로드
				// Load the STEP file into the internal CFL3DObject of the CGUIView3DObject object added to the view.

				if((res = fl3DObject.LoadSTEP("../../ExampleImages/StepReaderConvertTo3DObject/Cylinder.step", f64ChordalDeviation)).IsFail())
				{
					ErrorPrint(res, "Failed to load step file.\n");
					break;
				}

				// CFL3DObject 에 STEP 파일을 로드하였으므로 뷰의 CGUIView3DObject 객체를 업데이트
				// Since the STEP file has been loaded into the CFL3DObject, update the CGUIView3DObject object in the view.
				view3D[0].UpdateObject(i32ReturnIndex);
				view3D[0].ZoomFit();

                // 방법 2. CStepReader 에서 Step 파일 로드 후 GetResult3DObject() 로 CFL3DObject 에 할당
                // Method 2. Load the STEP file using CStepReader and then assign it to CFL3DObject using GetResult3DObject().                
                CStepReader sr = new CStepReader();
                f64ChordalDeviation = 0.00001;

				i32ReturnIndex = -1;
				if((res = view3D[1].PushObject(new CGUIView3DObject(), ref i32ReturnIndex)).IsFail())
				{
					ErrorPrint(res, "Failed to display 3D object.\n");
					break;
				}

				CGUIView3DObject objView3D2 = view3D[1].GetView3DObject(i32ReturnIndex);
				if(objView3D2 == null)
				{
					ErrorPrint(res, "Failed to display 3D object.\n");
					break;
				}

				CFL3DObject fl3DObject2 = objView3D2.Get3DObject();
				if(fl3DObject2 == null)
				{
					ErrorPrint(res, "Failed to display 3D object.\n");
					break;
				}

				if ((res = sr.Load("../../ExampleImages/StepReaderConvertTo3DObject/Cylinder.step")).IsFail())
                {
                    ErrorPrint(res, "Failed to load step file.\n");
                    break;
                }

                if ((res = sr.GetResult3DObject(ref fl3DObject2, f64ChordalDeviation)).IsFail())
                {
                    ErrorPrint(res, "Failed to get 3D object from the StepReader.\n");
                    break;
                }

				view3D[1].UpdateObject(i32ReturnIndex);
				view3D[1].ZoomFit();

				// 3D View 에서 카메라 객체 얻어 오기
				// Gets the camera object from the 3D View.
				CGUIView3DCamera camera = new CGUIView3DCamera();

                // 카메라가 바라보는 방향을 설정
                // Sets the direction that the camera is looking.
                camera.SetDirection(new CFLPoint3<float>(-0.2, 0.8, -0.6));

                // 카메라의 위쪽 방향 벡터를 설정
                // Sets the up direction vector of the camera.
                camera.SetDirectionUp(new CFLPoint3<float>(-0.2, 1.0, 0.1));

                // 카메라의 포지션을 설정
                // Sets the position of the camera.
                camera.SetPosition(new CFLPoint3<float>(56.2, -276.5, 324.0));

                // 카메라가 바라보는 물체의 좌표를 설정
                // Sets the coordinates of the object that the camera is looking at.
                camera.SetTarget(new CFLPoint3<float>(9.6, -34.5, 151.4));

                // 추가한 3D 객체를 잘 볼 수 있는 시점으로 이동
                // Moves the viewpoint to a good position to see the added 3D object.
                view3D[0].SetCamera(camera);
                view3D[1].SetCamera(camera);

                // 아래 함수 DrawTextCanvas()는 screen좌표를 기준으로 하는 string을 drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
                // 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
                // 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
                //                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
                // Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
                //                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
                CFLPoint<double> flp = new CFLPoint<double>(0, 0);
                if ((res = view3D[0].GetLayer(2).DrawTextCanvas(flp, "Chordal Deviation = 0.0", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                if ((res = view3D[1].GetLayer(2).DrawTextCanvas(flp, "Chordal Deviation = 0.00001", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
                {
                    ErrorPrint(res, "Failed to draw text\n");
                    break;
                }

                view3D[0].SetCanvasColor(EColor.WHITE);
                view3D[1].SetCanvasColor(EColor.WHITE);

                // 3D 뷰를 갱신 // Update 3D view
                view3D[0].UpdateScreen();
                view3D[1].UpdateScreen();

                // 3D 뷰가 종료될 때 까지 기다림 // Wait for the 3D view 
                while (view3D[0].IsAvailable() && view3D[1].IsAvailable())
                    Thread.Sleep(1);
            }
            while (false);
		}
	}
}

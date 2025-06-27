using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CResult = FLImagingCLR.CResult;

namespace Figure_FLTriangularSolid3
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
			// 3D 뷰 선언
			// Declaration of the 3D view 
			CGUIView3D[] view3D = new CGUIView3D[2];

			for(int i = 0; i < 2; ++i)
				view3D[i] = new CGUIView3D();

			// 3D 뷰 layer 선언
			// Declaration of the 3D view layer
			CGUIView3DLayer[] layer3D = new CGUIView3DLayer[2];

			for(int i = 0; i < 2; ++i)
				layer3D[i] = new CGUIView3DLayer();

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult res = new CResult();

			do
			{
				// 3D 뷰 생성 // Create the 3D view
				if((res = view3D[0].Create(400, 0, 812, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				if((res = view3D[1].Create(812, 0, 1224, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// 각 3D 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each 3D view.
				if((res = view3D[0].SynchronizePointOfView(ref view3D[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 3D 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each 3D view window
				if((res = view3D[0].SynchronizeWindow(ref view3D[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 각각의 3D View 에서 0번 레이어 가져오기 // Get Layer 0 from each 3D view 
				for(int i = 0; i < 2; ++i)
					layer3D[i] = view3D[i].GetLayer(0);

				// 각 레이어 캔버스에 텍스트 그리기 // Draw text to each Layer Canvas
				layer3D[0].DrawTextCanvas(new CFLPoint<double>(3, 0), "Figure A", EColor.YELLOW, EColor.BLACK, 20);
				layer3D[1].DrawTextCanvas(new CFLPoint<double>(3, 0), "Figure A", EColor.YELLOW, EColor.BLACK, 20);

				layer3D[0].DrawTextCanvas(new CFLPoint<double>(3, 30), "Base Plane", EColor.YELLOW, EColor.BLACK, 15);
				layer3D[1].DrawTextCanvas(new CFLPoint<double>(3, 30), "Length : +20", EColor.YELLOW, EColor.BLACK, 15);

				// Figure A 의 한쪽 면 생성 // Create one side of Figure A
				CFLPoint3<double> flpFigA0 = new CFLPoint3<double>(0, 0, 0);
				CFLPoint3<double> flpFigA1 = new CFLPoint3<double>(0, 10, 0);
				CFLPoint3<double> flpFigA2 = new CFLPoint3<double>(10, 0, 0);
				CFLTriangle3<double> fltBasePlaneFigA = new CFLTriangle3<double>(flpFigA0, flpFigA1, flpFigA2);
				CFLTriangularSolid3<double> fltsBasePlaneFigA = new CFLTriangularSolid3<double>(fltBasePlaneFigA, fltBasePlaneFigA);

				// 두 번째 평면은 첫 번째 평면의 법선 방향으로 `Length`만큼 떨어진 위치에 계산됩니다.
				// The second plane is calculated at a distance of `Length` in the normal direction of the first plane.
				double f64LengthA = 20;
				CFLTriangularSolid3<double> fltsSolidFigA = new CFLTriangularSolid3<double>(fltBasePlaneFigA, f64LengthA);


				// 3D 뷰에 3D figure 추가 // Add 3D figures to the 3D view
				view3D[0].PushBackROI(fltsBasePlaneFigA);
				view3D[1].PushBackROI(fltsSolidFigA);

				// 추가한 3D 객체가 화면 안에 들어오도록 Zoom Fit // Perform Zoom Fit to ensure added 3D objects are within the view
				view3D[1].ZoomFit();

				// 3D 뷰어의 시점(카메라) 변경 // Change the viewpoint (camera) of the 3D viewer
				CGUIView3DCamera cam1 = view3D[1].GetCamera();
				cam1.SetPosition(new CFLPoint3<float>(18.48, -32.13, 7.31));
				cam1.SetDirection(new CFLPoint3<float>(-0.38, 0.92, 0.06));
				cam1.SetDirectionUp(new CFLPoint3<float>(0.03, -0.05, 1.00));
				view3D[1].SetCamera(cam1);


				// Console 출력 // Console output
				Console.Write("<Figure A>\n");
				Console.Write("Base Plane : \n{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(fltBasePlaneFigA));
				Console.Write("Solid Figure : \n{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(fltsSolidFigA));

				// 3D 뷰들을 갱신 합니다. // Update the 3D views.
				for(int i = 0; i < 2; ++i)
				{
					view3D[i].UpdateScreen();
					view3D[i].Invalidate(true);
				}

				// 3D 뷰가 둘중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the two 3D views are turned off
				while(view3D[0].IsAvailable() && view3D[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

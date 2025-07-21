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

namespace Figure
{
	class Figure_FLQuadrilateralSolid3
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
			CGUIView3D[] view3D = new CGUIView3D[4];

			for(int i = 0; i < 4; ++i)
				view3D[i] = new CGUIView3D();

			// 3D 뷰 layer 선언
			// Declaration of the 3D view layer
			CGUIView3DLayer[] layer3D = new CGUIView3DLayer[4];

			for(int i = 0; i < 4; ++i)
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

				if((res = view3D[2].Create(400, 384, 812, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				if((res = view3D[3].Create(812, 384, 1224, 768)).IsFail())
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
				if((res = view3D[2].SynchronizePointOfView(ref view3D[3])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 각 3D 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each 3D view window
				for(int i = 1; i < 4; ++i)
				{
					if((res = view3D[0].SynchronizeWindow(ref view3D[i])).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						break;
					}
				}

				if(res.IsFail())
					break;

				// 각각의 3D View 에서 0번 레이어 가져오기 // Get Layer 0 from each 3D view 
				for(int i = 0; i < 4; ++i)
					layer3D[i] = view3D[i].GetLayer(0);

				// 각 레이어 캔버스에 텍스트 그리기 // Draw text to each Layer Canvas
				layer3D[0].DrawTextCanvas(new CFLPoint<double>(3, 0), "Figure A (Regular Quad)", EColor.YELLOW, EColor.BLACK, 20);
				layer3D[1].DrawTextCanvas(new CFLPoint<double>(3, 0), "Figure A (Regular Quad)", EColor.YELLOW, EColor.BLACK, 20);
				layer3D[2].DrawTextCanvas(new CFLPoint<double>(3, 0), "Figure B (Distorted Quad)", EColor.YELLOW, EColor.BLACK, 20);
				layer3D[3].DrawTextCanvas(new CFLPoint<double>(3, 0), "Figure B (Distorted Quad)", EColor.YELLOW, EColor.BLACK, 20);

				layer3D[0].DrawTextCanvas(new CFLPoint<double>(3, 30), "Base Plane", EColor.YELLOW, EColor.BLACK, 15);
				layer3D[1].DrawTextCanvas(new CFLPoint<double>(3, 30), "Length : +20", EColor.YELLOW, EColor.BLACK, 15);
				layer3D[2].DrawTextCanvas(new CFLPoint<double>(3, 30), "Base Plane", EColor.YELLOW, EColor.BLACK, 15);
				layer3D[3].DrawTextCanvas(new CFLPoint<double>(3, 30), "Length : -20", EColor.YELLOW, EColor.BLACK, 15);

				// Figure A 의 한쪽 면 생성 // Create one side of Figure A
				CFLPoint3<double> flpFigA0 = new CFLPoint3<double>(0, 0, 5);
				CFLPoint3<double> flpFigA1 = new CFLPoint3<double>(0, 10, 5);
				CFLPoint3<double> flpFigA2 = new CFLPoint3<double>(10, 10, 0);
				CFLPoint3<double> flpFigA3 = new CFLPoint3<double>(10, 0, 0);
				CFLQuad3<double> flqBasePlaneFigA = new CFLQuad3<double>(flpFigA0, flpFigA1, flpFigA2, flpFigA3);

				// 두 번째 평면의 각 꼭짓점은 첫 번째 평면의 각 꼭짓점에서 면의 법선 방향으로 `Length`만큼 떨어진 위치에 계산됩니다.
				// 각 꼭짓점마다 독립적으로 법선을 구하며, 이는 주어진 꼭짓점과 그 주변 두 꼭짓점으로 구성된 삼각형의 법선을 기반으로 합니다.
				// 
				// 법선 벡터 계산은 다음과 같은 절차로 이루어집니다:
				// 
				// 각 꼭짓점에 대해, 해당 꼭짓점과 인접한 두 꼭짓점을 이용해 삼각형을 구성합니다.
				// -삼각형의 두 변을 외적하여 면의 법선 벡터를 구하고 단위 벡터로 정규화합니다.
				// -정규화된 법선 벡터를 `Length`만큼 스케일링하여 원래 꼭짓점 위치에 더함으로써 두 번째 평면의 꼭짓점을 생성합니다.
				//
				// 예를 들어, 첫 번째 평면의 꼭짓점 0에 대해서는 다음과 같이 법선이 계산됩니다 :
				// -삼각형 : (0, 1, 3)
				// -법선 벡터 : (flpPoints[1] - flpPoints[0]) ^ (flpPoints[0] - flpPoints[3])
				//
				// 이러한 방식으로 네 개의 꼭짓점 각각에 대해 법선 벡터가 개별적으로 계산되어, 두 번째 평면이 생성됩니다.
				// 
				// Each vertex of the second plane is calculated at a position `Length` away from each vertex of the first plane,
				// in the direction of the face normal.
				// The normal is calculated independently for each vertex, based on the normal of the triangle formed by
				// the given vertex and its two surrounding vertices.
				//
				// The normal vector calculation proceeds as follows:
				//
				// For each vertex, a triangle is formed using that vertex and two adjacent vertices.
				// - The normal vector of the face is obtained by taking the cross product of the two sides of the triangle, and then normalized to a unit vector.
				// - The normalized normal vector is scaled by `Length` and added to the original vertex position to create the vertex for the second plane.
				//
				// For example, for vertex 0 of the first plane, the normal is calculated as follows:
				// - Triangle: (0, 1, 3)
				// - Normal vector: (flpPoints[1] - flpPoints[0]) ^ (flpPoints[0] - flpPoints[3])
				//
				// In this way, a normal vector is individually calculated for each of the four vertices, thereby creating the second plane.
				double f64LengthA = 20;
				CFLQuadrilateralSolid3<double> flqsSolidFigA = new CFLQuadrilateralSolid3<double>(flqBasePlaneFigA, f64LengthA);

				// Figure B 의 한쪽 면 생성 // Create one side of Figure B
				CFLPoint3<double> flpFigB0 = new CFLPoint3<double>(0, 0, -3);
				CFLPoint3<double> flpFigB1 = new CFLPoint3<double>(-1, 9, -4);
				CFLPoint3<double> flpFigB2 = new CFLPoint3<double>(10, 10, 1);
				CFLPoint3<double> flpFigB3 = new CFLPoint3<double>(11, 2, 0);
				CFLQuad3<double> flqBasePlaneFigB = new CFLQuad3<double>(flpFigB0, flpFigB1, flpFigB2, flpFigB3);

				// 위와 같은 로직으로 Figure B 의 반대쪽 면을 생성하며, length 가 음수인 경우 법선 벡터의 반대방향으로 반대쪽 면 생성
				// The opposite side of Figure B is created using the same logic as above, and if length is a negative number, the opposite side is created in the opposite direction of the normal vector.
				double f64LengthB = -20;
				CFLQuadrilateralSolid3<double> flqsSolidFigB = new CFLQuadrilateralSolid3<double>(flqBasePlaneFigB, f64LengthB);


				// 3D 뷰에 3D figure 추가 // Add 3D figures to the 3D view
				CGUIView3DObject view3DObj = new CGUIView3DObject();
				view3DObj.SetTopologyType(ETopologyType3D.Wireframe);

				CFL3DObject[] arr3DObj = new CFL3DObject[4];
				arr3DObj[0] = new CFL3DObject(flqBasePlaneFigA);
				arr3DObj[1] = new CFL3DObject(flqsSolidFigA);
				arr3DObj[2] = new CFL3DObject(flqBasePlaneFigB);
				arr3DObj[3] = new CFL3DObject(flqsSolidFigB);

				for(int i = 0; i < 4; ++i)
				{
					view3DObj.Set3DObject(arr3DObj[i]);
					view3D[i].PushObject(view3DObj);
				}

				// 추가한 3D 객체가 화면 안에 들어오도록 Zoom Fit // Perform Zoom Fit to ensure added 3D objects are within the view
				view3D[1].ZoomFit();
				view3D[3].ZoomFit();

				// 3D 뷰어의 시점(카메라) 변경 // Change the viewpoint (camera) of the 3D viewer
				CGUIView3DCamera cam1 = view3D[1].GetCamera();
				cam1.SetPosition(new CFLPoint3<float>(21.40, -30.30, 9.04));
				cam1.SetDirection(new CFLPoint3<float>(-0.38, 0.92, 0.06));
				cam1.SetDirectionUp(new CFLPoint3<float>(0.03, -0.05, 1.00));
				view3D[1].SetCamera(cam1);

				CGUIView3DCamera cam3 = view3D[3].GetCamera();
				cam3.SetPosition(new CFLPoint3<float>(28.82, -20.0, 14.0));
				cam3.SetDirection(new CFLPoint3<float>(-0.58, 0.58, -0.58));
				cam3.SetDirectionUp(new CFLPoint3<float>(-0.41, 0.41, 0.82));
				view3D[3].SetCamera(cam3);



				// Console 출력 // Console output
				Console.Write("<Figure A>\n");
				Console.Write("Base Plane : \n{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flqBasePlaneFigA));
				Console.Write("Solid Figure : \n{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flqsSolidFigA));

				Console.Write("<Figure B>\n");
				Console.Write("Base Plane : \n{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flqBasePlaneFigB));
				Console.Write("Solid Figure : \n{0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flqsSolidFigB));

				// 3D 뷰들을 갱신 합니다. // Update the 3D views.
				for(int i = 0; i < 4; ++i)
				{
					view3D[i].UpdateScreen();
					view3D[i].Invalidate(true);
				}

				// 3D 뷰가 넷중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the four 3D views are turned off
				while(view3D[0].IsAvailable() && view3D[1].IsAvailable() && view3D[2].IsAvailable() && view3D[3].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

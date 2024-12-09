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

namespace ROIUtilities3D
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

public enum EType
{
	Model = 0,
	Texture,
	Count,
};

[STAThread]
static void Main(string[] args)
{
	// 3D 뷰 선언 // Declaration of the 3D view 
	CGUIView3D view3DSrc = new CGUIView3D();
	CGUIView3D view3DInclude = new CGUIView3D();
	CGUIView3D view3DExclude = new CGUIView3D();
	CGUIView3D view3DAdd = new CGUIView3D();
	CGUIView3D view3DRemove = new CGUIView3D();
	CGUIView3D view3DXOR = new CGUIView3D();
	CGUIView3D[] arrView3D = new CGUIView3D[6];
	arrView3D[0] = view3DSrc;
	arrView3D[1] = view3DInclude;
	arrView3D[2] = view3DExclude;
	arrView3D[3] = view3DAdd;
	arrView3D[4] = view3DRemove;
	arrView3D[5] = view3DXOR;

	CResult res;

	do
	{
		// 3D 뷰 생성 // Create the 3D view
		view3DInclude.Create(0, 0, 300, 300); // L, T, R, B(Left, Top, Right, Bottom) 
		view3DSrc.Create(300, 0, 600, 300);
		view3DExclude.Create(600, 0, 900, 300);
		view3DAdd.Create(0, 300, 300, 600);
		view3DRemove.Create(300, 300, 600, 600);
		view3DXOR.Create(600, 300, 900, 600);

		// 3D 객체 선언 // Declare a 3D object
		CFL3DObject fl3DObjLeft = new CFL3DObject();
		CFL3DObject fl3DObjRight = new CFL3DObject();
		// 3D 객체 로드 // Load a 3D object
		res = fl3DObjLeft.Load("../../ExampleImages/ROIUtilities3D/Left Cam.ply");
		res = fl3DObjRight.Load("../../ExampleImages/ROIUtilities3D/Right Cam.ply");

		for(int i = 0; i < 6; ++i)
		{
			// 3D 뷰에 3D 객체 추가 // Add 3D objects to the 3D view
			arrView3D[i].PushObject(fl3DObjLeft);
			arrView3D[i].PushObject(fl3DObjRight);

			// 추가한 3D 객체가 화면 안에 들어오도록 Zoom Fit // Perform Zoom Fit to ensure added 3D objects are within the view
			arrView3D[i].ZoomFit();

			// 3D 뷰어의 시점(카메라) 변경 // Change the viewpoint (camera) of the 3D viewer
			CGUIView3DCamera cam = arrView3D[i].GetCamera();
			cam.SetPosition(new CFLPoint3<float>(0.71, 0.02, 10.94));
			cam.SetDirectionUp(new CFLPoint3<float>(1, 0, 0));
			arrView3D[i].SetCamera(cam);

			if(i > 0)
			{
				// 3D 뷰 시점 동기화 // Synchronize the viewpoint of the 3D view
				arrView3D[i].SynchronizePointOfView(ref arrView3D[i - 1]);
				// 윈도우 동기화 // Synchronize the window
				arrView3D[i].SynchronizeWindow(ref arrView3D[i - 1]);
			}
		}

		// 절두체 ROI 선언 // Declare the frustum ROI
		CFLFrustum<float> flfr = new CFLFrustum<float>();
		// 파일에서 절두체 ROI 로드 // Load the frustum ROI from a file
		flfr.Load("../../ExampleImages/ROIUtilities3D/frustumROI.fig");

		// 3D 뷰에 ROI 추가 // Add the ROI to the 3D view
		for(int i = 0; i < 6; ++i)
			arrView3D[i].PushBackROI(flfr);

		// CROIUtilities3D 객체 선언 // Declare the CROIUtilities3D object
		CROIUtilities3D roiUtil3D = new CROIUtilities3D();

		// CROIUtilities3D 객체에 3D Object 추가 // Add 3D objects to the CROIUtilities3D object
		roiUtil3D.PushBack3DObject(fl3DObjLeft);
		roiUtil3D.PushBack3DObject(fl3DObjRight);

		// CROIUtilities3D 객체에 절두체 ROI 추가 // Add the frustum ROI to the CROIUtilities3D object
		roiUtil3D.PushBackROI(flfr);

		// 선택 타입 설정 : ROI 안에 포함되는 정점만 선택 // Set the selection type to include only vertices inside the ROI
		roiUtil3D.SetSelectionType(CROIUtilities3D.EResultSelectionType.Include);

		// CROIUtilities3D 실행 // Execute the CROIUtilities3D object
		if((res = roiUtil3D.Execute()).IsFail())
			break;

		// CROIUtilities3D 에서 결과 얻어 오기 // Retrieve the results from CROIUtilities3D
		List<List<int>> arr2ResultROIIndexInclude;
		if((res = roiUtil3D.GetResult(out arr2ResultROIIndexInclude)).IsFail())
			break;

		// EResultSelectionType.Add 연산을 위해 CROIUtilities3D 객체 선언 및 roiUtil3D 를 복사 생성. Include 연산으로 얻은 결과값까지 복사됨
		// Declare and copy construct a CROIUtilities3D object for the EResultSelectionType.Add operation. 
		// The results from the Include operation are also copied.
		CROIUtilities3D roiUtil3DAdd = new CROIUtilities3D(roiUtil3D);
		// 복사한 객체에서 ROI를 모두 클리어 // Clear all ROIs from the copied object
		roiUtil3DAdd.ClearROI();

		if(arr2ResultROIIndexInclude.Count() > 0)
		{
			int i32ObjectIdx = 0;

			// 3D 뷰어에 추가된 3D 객체의 개수 // Number of 3D objects added to the 3D viewer
			int i32ObjectCount = view3DInclude.GetObjectCount();

			for(int i = 0; i < i32ObjectCount; ++i)
			{
				// 3D 뷰어에 추가된 i번째 3D 객체 // The i-th 3D object added to the 3D viewer
				CGUIView3DObject pObj = view3DInclude.GetView3DObject(i);

				// 해당 객체가 없거나, 해당 객체에 대해 선택이 비활성화 되어 있다면 continue // Skip if the object is null or selection is disabled
				if(pObj == null || !pObj.IsSelectionEnabled())
					continue;

				// i번째 3D 객체의 데이터(CFL3DObject) // Data of the i-th 3D object (CFL3DObject)
				CFL3DObject pObjData = (CFL3DObject)pObj.GetData();

				// 해당 객체가 없다면 continue // Skip if the object data is null
				if(pObjData == null)
					continue;

				// i번째 3D 객체에 대한 결과값 배열. 이 배열은 i번째 3D 객체에 대해, ROI 내부에 위치한 정점의 인덱스로 이루어짐 // Result array for the i-th 3D object. Contains indices of vertices within the ROI.
				List<int> flaCollisionIndex = arr2ResultROIIndexInclude[i32ObjectIdx];
				i32ObjectIdx++;

				if(flaCollisionIndex.Count() == 0)
					continue;

				int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

				// i번째 3D 객체에 대해, ROI 내부에 위치한 정점을 빨간색으로 표시 // Mark vertices within the ROI of the i-th 3D object in red
				for(int j = 0; j < i32CollisionIndexCount; ++j)
					pObjData.SetVertexColorAt(flaCollisionIndex[j], 255, 0, 0); // RED

				// 3D 뷰어에 추가된 i번째 3D 객체에 대해 렌더링 업데이트 // Update rendering for the i-th 3D object added to the 3D viewer
				pObj.UpdateAll();
				view3DInclude.UpdateObject(i);
			}

			// 3D 뷰어 업데이트 // Update the 3D viewer
			view3DInclude.UpdateScreen();

		}
		// 선택 타입 설정 : ROI 바깥의 정점만 선택 // Set selection type: Select only vertices outside the ROI
		roiUtil3D.SetSelectionType(CROIUtilities3D.EResultSelectionType.Exclude);

		// CROIUtilities3D 실행 // Execute CROIUtilities3D
		if((res = roiUtil3D.Execute()).IsFail())
			break;

		// CROIUtilities3D 에서 결과 얻어 오기 // Retrieve results from CROIUtilities3D
		List<List<int>> arr2ResultROIIndexExclude;
		if((res = roiUtil3D.GetResult(out arr2ResultROIIndexExclude)).IsFail())
			break;

		// EResultSelectionType.Remove 연산을 위해 CROIUtilities3D 객체 선언 및 roiUtil3D 를 복사 생성. Exclude 연산으로 얻은 결과값까지 복사됨 // Declare a CROIUtilities3D object for EResultSelectionType.Remove operation and copy roiUtil3D. Results from the Exclude operation are also copied.
		CROIUtilities3D roiUtil3DRemove = new CROIUtilities3D(roiUtil3D);
		// EResultSelectionType.XOR 연산을 위해 CROIUtilities3D 객체 선언 및 roiUtil3D 를 복사 생성. Exclude 연산으로 얻은 결과값까지 복사됨 // Declare a CROIUtilities3D object for EResultSelectionType.XOR operation and copy roiUtil3D. Results from the Exclude operation are also copied.
		CROIUtilities3D roiUtil3DXOR = new CROIUtilities3D(roiUtil3D);
		// 복사한 객체에서 ROI를 모두 클리어 // Clear all ROIs from the copied objects
		roiUtil3DRemove.ClearROI();
		roiUtil3DXOR.ClearROI();

		if(arr2ResultROIIndexExclude.Count() > 0)
		{
			int i32ObjectIdx = 0;

			// 3D 뷰어에 추가된 3D 객체의 개수 // Number of 3D objects added to the 3D viewer
			int i32ObjectCount = view3DExclude.GetObjectCount();

			for(int i = 0; i < i32ObjectCount; ++i)
			{
				// 3D 뷰어에 추가된 i번째 3D 객체 // The i-th 3D object added to the 3D viewer
				CGUIView3DObject pObj = view3DExclude.GetView3DObject(i);

				// 해당 객체가 없거나, 해당 객체에 대해 선택이 비활성화 되어 있다면 continue // Skip if the object is null or selection is disabled
				if(pObj == null || !pObj.IsSelectionEnabled())
					continue;

				// i번째 3D 객체의 데이터(CFL3DObject) // Data of the i-th 3D object (CFL3DObject)
				CFL3DObject pObjData = (CFL3DObject)pObj.GetData();

				// 해당 객체가 없다면 continue // Skip if the object data is null
				if(pObjData == null)
					continue;

				// i번째 3D 객체에 대한 결과값 배열. 이 배열은 i번째 3D 객체에 대해, ROI 외부에 위치한 정점의 인덱스로 이루어짐 // Result array for the i-th 3D object. Contains indices of vertices outside the ROI.
				List<int> flaCollisionIndex = arr2ResultROIIndexExclude[i32ObjectIdx];
				i32ObjectIdx++;

				if(flaCollisionIndex.Count() == 0)
					continue;

				int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

				// i번째 3D 객체에 대해, ROI 바깥에 위치한 정점을 파란색으로 표시 // Mark vertices outside the ROI of the i-th 3D object in blue
				for(int j = 0; j < i32CollisionIndexCount; ++j)
					pObjData.SetVertexColorAt(flaCollisionIndex[j], 0, 0, 255); // BLUE

				// 3D 뷰어에 추가된 i번째 3D 객체에 대해 렌더링 업데이트 // Update rendering for the i-th 3D object added to the 3D viewer
				pObj.UpdateAll();
				view3DExclude.UpdateObject(i);
			}
			// 3D 뷰어 업데이트 // Update the 3D viewer
			view3DExclude.UpdateScreen();
		}

		// 기존 선택 영역(위에서 Include로 선택한 영역)에 추가로 선택할 영역을 ROI로 설정 // Set an additional ROI to be selected in the existing selection area (previously selected with Include)
		CFLFrustum<float> flfrAdd = new CFLFrustum<float>();
		flfrAdd.Load("../../ExampleImages/ROIUtilities3D/frustumROI_Add.fig");

		// CROIUtilities3D 객체에 절두체 ROI 추가 // Add the frustum ROI to the CROIUtilities3D object
		roiUtil3DAdd.PushBackROI(flfrAdd);
		// 3D 뷰어에 절두체 ROI 추가 // Add the frustum ROI to the 3D viewer
		view3DAdd.PushBackROI(flfrAdd);

		// 선택 타입 설정 : 기존 결과에 ROI 안에 포함되는 정점을 추가 // Set selection type: Add vertices within the ROI to the existing results
		roiUtil3DAdd.SetSelectionType(CROIUtilities3D.EResultSelectionType.Add);

		// CROIUtilities3D 실행 // Execute CROIUtilities3D
		if((res = roiUtil3DAdd.Execute()).IsFail())
			break;

		// CROIUtilities3D 에서 결과 얻어 오기 // Retrieve results from CROIUtilities3D
		List<List<int>> arr2ResultROIIndexAdd;
		if((res = roiUtil3DAdd.GetResult(out arr2ResultROIIndexAdd)).IsFail())
			break;

		if(arr2ResultROIIndexAdd.Count() > 0)
		{
			int i32ObjectIdx = 0;
			// 3D 뷰어에 추가된 3D 객체의 개수 // Number of 3D objects added to the 3D viewer
			int i32ObjectCount = view3DAdd.GetObjectCount();

			for(int i = 0; i < i32ObjectCount; ++i)
			{
				// 3D 뷰어에 추가된 i번째 3D 객체 // The i-th 3D object added to the 3D viewer
				CGUIView3DObject pObj = view3DAdd.GetView3DObject(i);

				// 해당 객체가 없거나, 해당 객체에 대해 선택이 비활성화 되어 있다면 continue // Skip if the object is null or selection is disabled
				if(pObj == null || !pObj.IsSelectionEnabled())
					continue;

				// i번째 3D 객체의 데이터(CFL3DObject) // Data of the i-th 3D object (CFL3DObject)
				CFL3DObject pObjData = (CFL3DObject)pObj.GetData();

				// 해당 객체가 없다면 continue // Skip if the object data is null
				if(pObjData == null)
					continue;

				// i번째 3D 객체에 대한 결과값 배열. // Result array for the i-th 3D object
				List<int> flaCollisionIndex = arr2ResultROIIndexAdd[i32ObjectIdx];
				i32ObjectIdx++;

				if(flaCollisionIndex.Count() == 0)
					continue;

				int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

				// i번째 3D 객체에 대해, ROI 내부에 위치한 정점을 빨간색으로 표시 // Mark vertices within the ROI of the i-th 3D object in red
				for(int j = 0; j < i32CollisionIndexCount; ++j)
					pObjData.SetVertexColorAt(flaCollisionIndex[j], 255, 0, 0); // RED

				// 3D 뷰어에 추가된 i번째 3D 객체에 대해 렌더링 업데이트 // Update rendering for the i-th 3D object added to the 3D viewer
				pObj.UpdateAll();
				view3DAdd.UpdateObject(i);
			}

			// 3D 뷰어 업데이트 // Update the 3D viewer
			view3DAdd.UpdateScreen();
		}

		// 기존 선택 영역(위에서 Exclude로 선택한 영역)에서 제거할 영역을 ROI로 설정 // Set ROIs to remove areas from the existing selection (previously selected with Exclude)
		CFLFrustum<float> flfrRemove1 = new CFLFrustum<float>();
		CFLFrustum<float> flfrRemove2 = new CFLFrustum<float>();
		flfrRemove1.Load("../../ExampleImages/ROIUtilities3D/frustumROI_Remove1.fig");
		flfrRemove2.Load("../../ExampleImages/ROIUtilities3D/frustumROI_Remove2.fig");

		// CROIUtilities3D 객체에 절두체 ROI 추가 // Add the frustum ROIs to the CROIUtilities3D object
		roiUtil3DRemove.PushBackROI(flfrRemove1);
		roiUtil3DRemove.PushBackROI(flfrRemove2);
		// 3D 뷰에 ROI 추가 // Add the frustum ROIs to the 3D view
		view3DRemove.PushBackROI(flfrRemove1);
		view3DRemove.PushBackROI(flfrRemove2);

		// 선택 타입 설정 : 기존 결과에서 ROI 안의 정점을 제거 // Set selection type: Remove vertices within the ROI from the existing results
		roiUtil3DRemove.SetSelectionType(CROIUtilities3D.EResultSelectionType.Remove);

		// CROIUtilities3D 실행 // Execute CROIUtilities3D
		if((res = roiUtil3DRemove.Execute()).IsFail())
			break;

		// CROIUtilities3D 에서 결과 얻어 오기 // Retrieve results from CROIUtilities3D
		List<List<int>> arr2ResultROIIndexRemove;
		if((res = roiUtil3DRemove.GetResult(out arr2ResultROIIndexRemove)).IsFail())
			break;

		if(arr2ResultROIIndexRemove.Count() > 0)
		{
			int i32ObjectIdx = 0;

			// 3D 뷰어에 추가된 3D 객체의 개수 // Number of 3D objects added to the 3D viewer
			int i32ObjectCount = view3DRemove.GetObjectCount();

			for(int i = 0; i < i32ObjectCount; ++i)
			{
				// 3D 뷰어에 추가된 i번째 3D 객체 // The i-th 3D object added to the 3D viewer
				CGUIView3DObject pObj = view3DRemove.GetView3DObject(i);

				// 해당 객체가 없거나, 해당 객체에 대해 선택이 비활성화 되어 있다면 continue // Skip if the object is null or selection is disabled
				if(pObj == null || !pObj.IsSelectionEnabled())
					continue;

				// i번째 3D 객체의 데이터(CFL3DObject) // Data of the i-th 3D object (CFL3DObject)
				CFL3DObject pObjData = (CFL3DObject)pObj.GetData();

				// 해당 객체가 없다면 continue // Skip if the object data is null
				if(pObjData == null)
					continue;

				// i번째 3D 객체에 대한 결과값 배열. // Result array for the i-th 3D object
				List<int> flaCollisionIndex = arr2ResultROIIndexRemove[i32ObjectIdx];
				i32ObjectIdx++;

				if(flaCollisionIndex.Count() == 0)
					continue;

				int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

				// i번째 3D 객체에 대해, 기존 결과에서 ROI 안의 정점을 제거 후 선택된 정점을 파란색으로 표시 // Mark selected vertices after removing vertices within the ROI of the i-th 3D object in blue
				for(int j = 0; j < i32CollisionIndexCount; ++j)
					pObjData.SetVertexColorAt(flaCollisionIndex[j], 0, 0, 255); // BLUE

				// 3D 뷰어에 추가된 i번째 3D 객체에 대해 렌더링 업데이트 // Update rendering for the i-th 3D object added to the 3D viewer
				pObj.UpdateAll();
				view3DRemove.UpdateObject(i);
			}

			// 3D 뷰어 업데이트 // Update the 3D viewer
			view3DRemove.UpdateScreen();
		}

		// 기존 선택 영역(위에서 Exclude로 선택한 영역)에서 XOR 선택할 영역을 ROI로 설정 // Set an ROI to perform XOR operation on the existing selection (previously selected with Exclude)
		CFLFrustum<float> flfrXOR = new CFLFrustum<float>();
		flfrXOR.Load("../../ExampleImages/ROIUtilities3D/frustumROI_XOR.fig");

		// CROIUtilities3D 객체에 절두체 ROI 추가 // Add the frustum ROI to the CROIUtilities3D object
		roiUtil3DXOR.PushBackROI(flfrXOR);
		// 3D 뷰에 ROI 추가 // Add the frustum ROI to the 3D view
		view3DXOR.PushBackROI(flfrXOR);

		// 선택 타입 설정 : 기존 결과에서 ROI 안의 정점을 XOR 연산하여 선택 // Set selection type: Perform XOR operation with vertices inside the ROI on the existing results
		roiUtil3DXOR.SetSelectionType(CROIUtilities3D.EResultSelectionType.XOR);

		// CROIUtilities3D 실행 // Execute CROIUtilities3D
		if((res = roiUtil3DXOR.Execute()).IsFail())
			break;

		// CROIUtilities3D 에서 결과 얻어 오기 // Retrieve results from CROIUtilities3D
		List<List<int>> arr2ResultROIIndexXOR;
		if((res = roiUtil3DXOR.GetResult(out arr2ResultROIIndexXOR)).IsFail())
			break;

		if(arr2ResultROIIndexXOR.Count() > 0)
		{
			int i32ObjectIdx = 0;

			// 3D 뷰어에 추가된 3D 객체의 개수 // Number of 3D objects added to the 3D viewer
			int i32ObjectCount = view3DXOR.GetObjectCount();

			for(int i = 0; i < i32ObjectCount; ++i)
			{
				// 3D 뷰어에 추가된 i번째 3D 객체 // The i-th 3D object added to the 3D viewer
				CGUIView3DObject pObj = view3DXOR.GetView3DObject(i);

				// 해당 객체가 없거나, 해당 객체에 대해 선택이 비활성화 되어 있다면 continue // Skip if the object is null or selection is disabled
				if(pObj == null || !pObj.IsSelectionEnabled())
					continue;

				// i번째 3D 객체의 데이터(CFL3DObject) // Data of the i-th 3D object (CFL3DObject)
				CFL3DObject pObjData = (CFL3DObject)pObj.GetData();

				// 해당 객체가 없다면 continue // Skip if the object data is null
				if(pObjData == null)
					continue;

				// i번째 3D 객체에 대한 결과값 배열. // Result array for the i-th 3D object
				List<int> flaCollisionIndex = arr2ResultROIIndexXOR[i32ObjectIdx];
				i32ObjectIdx++;

				if(flaCollisionIndex.Count() == 0)
					continue;

				int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

				// i번째 3D 객체에 대해, 기존 결과에서 ROI 안의 정점을 XOR 연산한 결과 정점들을 파란색으로 표시 // Mark the vertices resulting from XOR operation within the ROI of the i-th 3D object in blue
				for(int j = 0; j < i32CollisionIndexCount; ++j)
					pObjData.SetVertexColorAt(flaCollisionIndex[j], 0, 0, 255); // BLUE

				// 3D 뷰어에 추가된 i번째 3D 객체에 대해 렌더링 업데이트 // Update rendering for the i-th 3D object added to the 3D viewer
				pObj.UpdateAll();
				view3DXOR.UpdateObject(i);
			}

			// 3D 뷰어 업데이트 // Update the 3D viewer
			view3DXOR.UpdateScreen();
		}


		// 아래 함수 DrawTextCanvas()는 screen좌표를 기준으로 하는 string을 drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
		// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
		// 파라미터 순서 : 레이어 . 기준 좌표 Figure 객체 . 문자열 . 폰트 색 . 면 색 . 폰트 크기 . 실제 크기 유무 . 각도 .
		//                 얼라인 . 폰트 이름 . 폰트 알파값(불투명도) . 면 알파값 (불투명도) . 폰트 두께 . 폰트 이텔릭
		// Parameter order: layer . reference coordinate Figure object . string . font color . Area color . font size . actual size . angle .
		//                  Align . Font Name . Font Alpha Value (Opaqueness) . Cotton Alpha Value (Opaqueness) . Font Thickness . Font Italic
				CFLPoint<double> flp = new CFLPoint<double>(0, 0);
		if((res = view3DSrc.GetLayer(0).DrawTextCanvas(flp, "Source", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
		{
			ErrorPrint(res, "Failed to draw text\n");
			break;
		}

		if((res = view3DInclude.GetLayer(0).DrawTextCanvas(flp, "Include", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
		{
			ErrorPrint(res, "Failed to draw text\n");
			break;
		}

		if((res = view3DExclude.GetLayer(0).DrawTextCanvas(flp, "Exclude", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
		{
			ErrorPrint(res, "Failed to draw text\n");
			break;
		}

		if((res = view3DAdd.GetLayer(0).DrawTextCanvas(flp, "Add(Include Result+Add)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
		{
			ErrorPrint(res, "Failed to draw text\n");
			break;
		}

		if((res = view3DRemove.GetLayer(0).DrawTextCanvas(flp, "Remove(Exclude Result-Remove)", EColor.YELLOW, EColor.BLACK, 18)).IsFail())
		{
			ErrorPrint(res, "Failed to draw text\n");
			break;
		}

		if((res = view3DXOR.GetLayer(0).DrawTextCanvas(flp, "XOR(Exclude Result^XOR)", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
		{
			ErrorPrint(res, "Failed to draw text\n");
			break;
		}

		// 3D 뷰를 갱신 // Update 3D view
		for(int i = 0; i < 6; ++i)
			arrView3D[i].Invalidate(true);

		// 3D 뷰가 종료될 때 까지 기다림 // Wait for the 3D view 
		while(arrView3D[0].IsAvailable() && arrView3D[1].IsAvailable() && arrView3D[2].IsAvailable() && arrView3D[3].IsAvailable() && arrView3D[4].IsAvailable() && arrView3D[5].IsAvailable())
			Thread.Sleep(1);
	}
	while(false);
}
}
}

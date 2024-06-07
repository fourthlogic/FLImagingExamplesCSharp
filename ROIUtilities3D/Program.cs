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
			// 3D 뷰 선언 // Declare the 3D view
			CGUIView3D view3D = new CGUIView3D();

			do
			{
				CResult res;

				// 3D 뷰 생성 // Create 3D view
				if((res = (view3D.Create(100, 0, 612, 512))).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// 3D 뷰에 PLY 파일 디스플레이
				if((res = view3D.Load("../../ExampleImages/ROIUtilities3D/Right Cam.ply")).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the 3D view.\n");
					break;
				}

				view3D.ZoomFit();

				int i32ObjectCount = view3D.GetObjectCount();

				if(i32ObjectCount == 0)
				{
					ErrorPrint(res, "The 3D view doesn't have any 3D objects.\n");
					break;
				}

				// 3D 뷰에 ROI 추가
				TRect<float> trROI = new TRect<float>(137, 96, 239, 369);
				view3D.PushBackROI(trROI);

				// Figure 객체를 로드하여 3D 뷰에 ROI 추가
				CFLFigure pFlf = CFigureUtilities.LoadFigure("../../ExampleImages/ROIUtilities3D/frustumROI.fig");

				if(pFlf != null)
					view3D.PushBackROI(pFlf);

				int i32ROICount = view3D.GetROICount();

				if(i32ROICount == 0)
				{
					ErrorPrint(res, "There is no ROI in the 3D view.\n");
					break;
				}

				CROIUtilities3D roiUtil3D = new CROIUtilities3D();

				for(int i = 0; i < i32ROICount; ++i)
				{
					CFLFrustum<float> flfr = new CFLFrustum<float>();
					view3D.GetROI(i, out flfr);

					roiUtil3D.PushBackROI(flfr);
				}

				for(int i = 0; i < i32ObjectCount; ++i)
				{
					CGUIView3DObject pObj = view3D.GetView3DObject(i);

					if(pObj == null || !pObj.IsSelectionEnabled())
						continue;

					CFL3DObject pObjData = pObj.GetData();

					if(pObjData == null)
						continue;

					roiUtil3D.PushBack3DObject(pObjData);
				}

				List<List<int>> flfaResultROIIndexInclude, flfaResultROIIndexExclude;

				roiUtil3D.SetSelectionType(CROIUtilities3D.EResultSelectionType.Include);

				if((res = roiUtil3D.Execute()).IsFail())
					break;

				if((res = roiUtil3D.GetResult(out flfaResultROIIndexInclude)).IsFail())
					break;

				CFL3DObject resultObject3D = new CFL3DObject();

				if((res = roiUtil3D.GetResult(out resultObject3D)).IsFail())
					break;

				roiUtil3D.SetSelectionType(CROIUtilities3D.EResultSelectionType.Exclude);

				if((res = roiUtil3D.Execute()).IsFail())
					break;

				if((res = roiUtil3D.GetResult(out flfaResultROIIndexExclude)).IsFail())
					break;

				if(flfaResultROIIndexInclude.Count() > 0)
				{
					int i32IndexRes = 0;

					for(int i = 0; i < i32ObjectCount; ++i)
					{
						CGUIView3DObject pObj = view3D.GetView3DObject(i);

						if(pObj == null || !pObj.IsSelectionEnabled())
							continue;

						CFL3DObject pObjData = pObj.GetData();

						if(pObjData == null)
							continue;

						List<int> flaCollisionIndex = flfaResultROIIndexInclude[i32IndexRes];
						i32IndexRes++;

						if(flaCollisionIndex.Count() == 0)
							continue;

						int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

						for(int j = 0; j < i32CollisionIndexCount; ++j)
							pObjData.SetVertexColorAt(flaCollisionIndex[j], 255, 0, 0);

						pObj.UpdateAll();
						view3D.UpdateObject(i);
					}
				}

				if(flfaResultROIIndexExclude.Count() > 0)
				{
					int i32IndexRes = 0;

					for(int i = 0; i < i32ObjectCount; ++i)
					{
						CGUIView3DObject pObj = view3D.GetView3DObject(i);

						if(pObj == null || !pObj.IsSelectionEnabled())
							continue;

						CFL3DObject pObjData = pObj.GetData();

						if(pObjData == null)
							continue;

						List<int> flaCollisionIndex = flfaResultROIIndexExclude[i32IndexRes];
						i32IndexRes++;

						if(flaCollisionIndex.Count() == 0)
							continue;

						int i32CollisionIndexCount = (int)flaCollisionIndex.Count();

						for(int j = 0; j < i32CollisionIndexCount; ++j)
							pObjData.SetVertexColorAt(flaCollisionIndex[j], 0, 0, 255); // BLUE

						pObj.UpdateAll();
						view3D.UpdateObject(i);
					}
				}

				view3D.UpdateScreen();

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the shapes drawn on the layer
				view3D.GetLayer(0).Clear();

				// View 정보를 디스플레이 합니다. // Display the view information.
				// 아래 함수 DrawTextCanvas() 는 Screen 좌표를 기준으로 하는 문자열을 Drawing 한다. // The function DrawTextCanvas() draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 . 기준 좌표 Figure 객체 . 문자열 . 폰트 색 . 면 색 . 폰트 크기 . 실제 크기 유무 . 각도 .
				//                 얼라인 . 폰트 이름 . 폰트 알파값(불투명도) . 면 알파값 (불투명도) . 폰트 두께 . 폰트 이텔릭
				// Parameter order: layer . reference coordinate Figure object . string . font color . Area color . font size . actual size . angle .
				//                  Align . Font Name . Font Alpha Value (Opaqueness) . Cotton Alpha Value (Opaqueness) . Font Thickness . Font Italic
				CFLPoint<double> flpPosition = new CFLPoint<double>(0, 0);

				if((res = (view3D.GetLayer(0).DrawTextCanvas(flpPosition, "3D View", EColor.YELLOW, EColor.BLACK, 30))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				view3D.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(view3D.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

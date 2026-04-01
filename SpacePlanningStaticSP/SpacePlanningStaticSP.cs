using System;
using System.Collections.Generic;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ThreeDim;

namespace FLImagingExamplesCSharp
{
	class SpacePlanningStaticSP
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

			// 3D 뷰 선언 // Declare 3D views
			CGUIView3D view3DResult = new CGUIView3D();

			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 알고리즘 객체 선언 // Declare algorithm object
				CSpacePlanningStaticSP alg = new CSpacePlanningStaticSP();

				// Bin spec 설정 // Set the bin spec
				CSpacePlanningBaseSP.SBinSpec<float> binSpec = new CSpacePlanningBaseSP.SBinSpec<float>(12f, 9f, 10f);

				if((res = alg.AddBinSpec(binSpec)).IsFail())
				{
					ErrorPrint(res, "Failed to add bin spec.\n");
					break;
				}

				// Item spec 설정 (회전 없음) // Set the item specs (no rotation)
				CSpacePlanningBaseSP.SItemSpec<float> itemSpec1 = new CSpacePlanningBaseSP.SItemSpec<float>(3f, 3f, 4f, 1f, CSpacePlanningBaseSP.ERotationType.NoRotation);
				CSpacePlanningBaseSP.SItemSpec<float> itemSpec2 = new CSpacePlanningBaseSP.SItemSpec<float>(4f, 3f, 3f, 1f, CSpacePlanningBaseSP.ERotationType.NoRotation);
				CSpacePlanningBaseSP.SItemSpec<float> itemSpec3 = new CSpacePlanningBaseSP.SItemSpec<float>(5f, 3f, 2f, 1f, CSpacePlanningBaseSP.ERotationType.NoRotation);

				if((res = alg.AddItemSpec(itemSpec1)).IsFail() ||
				   (res = alg.AddItemSpec(itemSpec2)).IsFail() ||
				   (res = alg.AddItemSpec(itemSpec3)).IsFail())
				{
					ErrorPrint(res, "Failed to add item spec.\n");
					break;
				}

				// Static list 파라미터 설정 // Set the static list parameters
				CSpacePlanningBaseSP.SStaticListParameters parameters = new CSpacePlanningBaseSP.SStaticListParameters(new List<int>() { 4, 3, 2 });

				if((res = alg.SetStaticListParameters(parameters)).IsFail())
				{
					ErrorPrint(res, "Failed to set static list parameters.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 학습 수행 // Perform learning according to previously set parameters
				if((res = alg.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to learn.\n");
					break;
				}

				// 배치 결과 3D 오브젝트 그룹 취득 // Get the placement result 3D object group
				// 구조: [0, ItemCount) = 배치된 아이템, [ItemCount, end) = 빈(bin) * 2개씩 (속 채움, 외곽선)
				// Structure: [0, ItemCount) = placed items, [ItemCount, end) = bins * 2 each (filled, wireframe)
				CFL3DObjectGroup flog = new CFL3DObjectGroup();

				if((res = alg.Get3DObject(ref flog)).IsFail())
				{
					ErrorPrint(res, "Failed to get 3D object.\n");
					break;
				}

				int i32BinCount = alg.GetBinSpecCount();
				int i32ItemCount = alg.GetItemSpecCount();

				if((res = view3DResult.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				view3DResult.SetRenderingTransparencyMode(ERenderingTransparencyMode.DepthPeelingOIT);
				view3DResult.SetRenderingResolutionScale(2);

				// 결과 뷰에 아이템 및 Bin 오브젝트 추가 // Push item and bin objects to the result view
				for(int i = 0; i < i32ItemCount; ++i)
				{
					int i32ObjIndex = -1;
					if((res = view3DResult.PushObject(flog.GetObjectByIndex(i), ref i32ObjIndex)).IsFail())
					{
						ErrorPrint(res, "Failed to push 3D object.\n");
						break;
					}

					CGUIView3DObject objView3D = view3DResult.GetView3DObject(i32ObjIndex);
					if(objView3D != null)
						objView3D.SetOpacity(0.6f);
				}

				if(res.IsFail())
					break;

				for(int i = 0; i < i32BinCount; ++i)
				{
					int i32ObjIndex = -1;

					if((res = view3DResult.PushObject(flog.GetObjectByIndex(i32ItemCount + 2 * i), ref i32ObjIndex)).IsFail())
					{
						ErrorPrint(res, "Failed to push 3D object.\n");
						break;
					}

					CGUIView3DObject objFilled = view3DResult.GetView3DObject(i32ObjIndex);
					if(objFilled != null)
						objFilled.SetOpacity(0.2f);

					if((res = view3DResult.PushObject(flog.GetObjectByIndex(i32ItemCount + 2 * i + 1), ref i32ObjIndex)).IsFail())
					{
						ErrorPrint(res, "Failed to push 3D object.\n");
						break;
					}

					CGUIView3DObject objWireframe = view3DResult.GetView3DObject(i32ObjIndex);
					if(objWireframe != null)
						objWireframe.SetOpacity(0.6f);
				}

				if(res.IsFail())
					break;

				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 from the 3D view for display
				// 이 객체는 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to the view and does not need to be released separately
				CGUIView3DLayer layer3DResult = view3DResult.GetLayer(0);

				CFLPoint<double> flpLeftTop = new CFLPoint<double>(0, 0);
				layer3DResult.DrawTextCanvas(flpLeftTop, "Result 3D View", EColor.YELLOW, EColor.BLACK, 20);

				// 결과 정보를 3D 뷰에 텍스트로 표시 // Draw result summary text on the 3D view
				float f32TotalVolume = 0f;
				float f32UsedVolume = 0f;
				if((res = alg.GetCurrentVolumeUsage(0, ref f32TotalVolume, ref f32UsedVolume)).IsFail())
				{
					ErrorPrint(res, "Failed to get volume usage.\n");
					break;
				}

				float f32VolumeUsage = f32TotalVolume > 0f ? 100f * f32UsedVolume / f32TotalVolume : 0f;
				string strResultInfo = string.Format("Optimal strategy index: {0}\nVolume Usage: {1:F1}%({2:F1}/{3:F1})", alg.GetOptimalStrategyIndex(), f32VolumeUsage, f32UsedVolume, f32TotalVolume);

				layer3DResult.DrawTextCanvas(new CFLPoint<double>(0, 25), strResultInfo, EColor.YELLOW, EColor.BLACK, 16);

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다.
				// With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				view3DResult.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DResult.Invalidate(true);

				// 3D 뷰가 종료될 때 까지 기다림 // Wait for the 3D view to close
				while(view3DResult.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

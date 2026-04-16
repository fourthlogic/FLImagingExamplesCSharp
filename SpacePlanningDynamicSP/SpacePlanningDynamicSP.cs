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
	class SpacePlanningDynamicSP
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		static CResult InitializeCoordinateConverter(CSpacePlanningBaseSP alg, ref CSpacePlanningCoordinateConverterSP converter)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				if((res = alg.GetCoordinateConverter(ref converter)).IsFail())
					break;

				int i32BinCount = alg.GetBinSpecCount();
				for(int i = 0; i < i32BinCount; ++i)
				{
					TPoint3<float> tpWorldPivot = new TPoint3<float>(16f * i, 0f, 0f);
					TPoint3<float> tpBinPivot = new TPoint3<float>(0f, 0f, 0f);
					TPoint3<float> tpDirectionZ = new TPoint3<float>(0.03f, 0f, 1f);
					TPoint3<float> tpUpY = new TPoint3<float>(0f, 1f, 0.3f);

					if((res = converter.SetBinTransform(i, tpWorldPivot, tpBinPivot, tpDirectionZ, tpUpY)).IsFail())
						break;
				}

				if(res.IsFail())
					break;

				int i32ItemCount = alg.GetItemSpecCount();
				for(int i = 0; i < i32ItemCount; ++i)
				{
					if((res = converter.SetItemPivotNormalized(i, new TPoint3<float>(0.5f, 0.5f, 0.5f))).IsFail())
						break;
				}

				if(res.IsFail())
					break;

				res = converter.Learn();
			}
			while(false);

			return res;
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DResult = new CGUIView3D();

			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 알고리즘 객체 선언 // Declare algorithm object
				CSpacePlanningDynamicSP alg = new CSpacePlanningDynamicSP();

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

				// Random sequence 파라미터 설정 // Set the random sequence parameters
				// itemChances: 각 아이템 타입의 상대적 출현 비율 // Relative appearance ratio of each item type
				// Lookahead: 다음 배치 결정 시 고려할 선택지 수 // Number of candidates to consider for next placement
				CSpacePlanningBaseSP.SRandomSequenceParameters parameters =
					CSpacePlanningBaseSP.SRandomSequenceParameters.CreateInfinite(new List<float>() { 4f, 3f, 2f }, 1);

				if((res = alg.SetRandomSequenceParameters(parameters)).IsFail())
				{
					ErrorPrint(res, "Failed to set random sequence parameters.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 학습 수행 // Perform learning according to previously set parameters
				if((res = alg.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to learn.\n");
					break;
				}

				// 학습된 전략 중 최적 전략 선택 // Select the optimal strategy among learned strategies
				int i32OptimalStrategyIndex = alg.GetOptimalStrategyIndex();

				if((res = alg.SelectStrategy(i32OptimalStrategyIndex)).IsFail())
				{
					ErrorPrint(res, "Failed to select strategy.\n");
					break;
				}

				CSpacePlanningCoordinateConverterSP converter = new CSpacePlanningCoordinateConverterSP();
				if((res = InitializeCoordinateConverter(alg, ref converter)).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the coordinate converter.\n");
					break;
				}

				CFL3DObjectGroup flogBins = new CFL3DObjectGroup();
				if((res = converter.MakeBinObjectGroup(ref flogBins)).IsFail())
				{
					ErrorPrint(res, "Failed to build world-space bin objects.\n");
					break;
				}

				Console.WriteLine("Optimal strategy index: {0}", i32OptimalStrategyIndex);

				// 3D 뷰 생성 // Create 3D view
				if((res = view3DResult.Create(600, 0, 1200, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				view3DResult.SetRenderingTransparencyMode(ERenderingTransparencyMode.DepthPeelingOIT);
				view3DResult.SetRenderingResolutionScale(2);

				int i32BinCount  = alg.GetBinSpecCount();
				int i32ItemCount = alg.GetItemSpecCount();

				// 타이틀은 layer 0에 한 번만 그림 // Draw title once on layer 0
				// 매 스텝마다 갱신되는 상태 텍스트는 layer 1을 Clear 후 재작성 // Per-step status goes on layer 1, cleared each step
				view3DResult.GetLayer(0).DrawTextCanvas(new CFLPoint<double>(0, 0), "Dynamic SP - Interactive Placement", EColor.YELLOW, EColor.BLACK, 20);

				// 인터랙티브 모드 실행 // Run in interactive mode
				if((res = alg.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.\n");
					break;
				}

				// 아이템 도착 시뮬레이션 (컨베이어 벨트 상황) // Simulate item arrival (conveyor belt scenario)
				// 아이템 타입을 무작위로 생성하여 빈이 꽉 찰 때까지 계속 배치
				// Randomly generate item types and keep placing until the bin is full (EResult.FullOfCapacity)
				Random rng = new Random();
				List<CSpacePlanningBaseSP.SPlacementInfo> placementResults = new List<CSpacePlanningBaseSP.SPlacementInfo>();

				int i32ArrivalIdx  = 0;
				int i32PlacedCount = 0;

				for(;;)
				{
					if(!view3DResult.IsAvailable())
						break;

					int i32ItemType = rng.Next(0, i32ItemCount);

					// 아이템을 대기열에 추가하고 권장 위치에 자동 배치
					// Push item to queue and automatically place it at the recommended position
					CSpacePlanningBaseSP.SPlacementInfo placement = null;

					res = alg.PushAndPlace(i32ItemType, true, ref placement);

					++i32ArrivalIdx;

					if(res.IsFail())
					{
						if(res == new CResult(EResult.FullOfCapacity))
						{
							// 빈이 꽉 참 — 정상 종료 // Bin is full — normal termination
							Console.WriteLine("Arrival {0}: bin is full. Stopping.", i32ArrivalIdx);
							break;
						}
						// 예상치 못한 오류 // Unexpected error
						ErrorPrint(res, "Failed to push and place.\n");
						break;
					}

					++i32PlacedCount;
					placementResults.Add(placement);

					TPoint3<float> tpWorldPosition = new TPoint3<float>();
					if((res = converter.Convert(placement, ref tpWorldPosition)).IsFail())
					{
						ErrorPrint(res, "Failed to convert placement coordinates.\n");
						break;
					}

					Console.WriteLine(
						"Arrival {0}: placed item type {1} at bin {2} (rotation={3}, bin pos=[{4:F1}, {5:F1}, {6:F1}], world center=[{7:F1}, {8:F1}, {9:F1}])",
						i32ArrivalIdx,
						placement.i32ItemIndex,
						placement.i32BinIndex,
						(int)placement.eRotation,
						placement.tpPosition.x, placement.tpPosition.y, placement.tpPosition.z,
						tpWorldPosition.x, tpWorldPosition.y, tpWorldPosition.z);

					CFL3DObjectGroup flogItems = new CFL3DObjectGroup();
					if((res = converter.MakeItemObjectGroup(placementResults, ref flogItems)).IsFail())
					{
						ErrorPrint(res, "Failed to build world-space item objects.\n");
						break;
					}

					view3DResult.Lock();

					// 매 스텝마다 world-space 오브젝트로 뷰를 재구성
					// Rebuild the view with world-space objects on each step
					view3DResult.ClearObjects();

					for(int i = 0; i < i32PlacedCount; ++i)
					{
						int i32ObjIndex = -1;
						view3DResult.PushObject(flogItems.GetObjectByIndex(i), ref i32ObjIndex);
						CGUIView3DObject obj = view3DResult.GetView3DObject(i32ObjIndex);
						if(obj != null)
							obj.SetOpacity(0.6f);
					}

					for(int i = 0; i < i32BinCount; ++i)
					{
						int i32ObjIndex = -1;

						view3DResult.PushObject(flogBins.GetObjectByIndex(i), ref i32ObjIndex);
						CGUIView3DObject objFilled = view3DResult.GetView3DObject(i32ObjIndex);
						if(objFilled != null)
							objFilled.SetOpacity(0.2f);
					}

					// 상태 텍스트는 layer 1에 매 스텝 Clear 후 재작성 // Clear layer 1 each step and redraw status text
					// 이 객체는 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to the view and does not need to be released separately
					CGUIView3DLayer layer3DStatus = view3DResult.GetLayer(1);
					layer3DStatus.Clear();

					float f32TotalVolume = 0f;
					float f32UsedVolume  = 0f;
					alg.GetCurrentVolumeUsage(0, ref f32TotalVolume, ref f32UsedVolume);
					float f32VolumeUsage = f32TotalVolume > 0f ? 100f * f32UsedVolume / f32TotalVolume : 0f;

					string strStatus = string.Format(
						"Arrival {0}  |  Placed: {1}  |  Volume Usage: {2:F1}% ({3:F1} / {4:F1})\nWorld-space rendering enabled",
						i32ArrivalIdx, i32PlacedCount, f32VolumeUsage, f32UsedVolume, f32TotalVolume);

					layer3DStatus.DrawTextCanvas(new CFLPoint<double>(0, 25), strStatus, EColor.YELLOW, EColor.BLACK, 16);

					// 첫 아이템 배치 시 카메라를 전체에 맞게 조정 // Fit camera to all objects on first placement
					if(i32PlacedCount == 1)
						view3DResult.ZoomFit();

					view3DResult.Unlock();

					// 이미지 뷰를 갱신 합니다. // Update image view
					view3DResult.Invalidate(true);

					// 배치 과정을 눈으로 확인할 수 있도록 잠시 대기 // Pause briefly so the placement process is visible
					CThreadUtilities.Sleep(600);
				}

				if(!view3DResult.IsAvailable())
					break;

				// 최종 결과 요약 출력 // Print final result summary
				{
					float f32TotalVolume = 0f;
					float f32UsedVolume  = 0f;
					alg.GetCurrentVolumeUsage(0, ref f32TotalVolume, ref f32UsedVolume);
					float f32VolumeUsage = f32TotalVolume > 0f ? 100f * f32UsedVolume / f32TotalVolume : 0f;

					CGUIView3DLayer layer3DStatus = view3DResult.GetLayer(1);
					layer3DStatus.Clear();

					string strFinalInfo = string.Format("Done  |  Arrivals: {0}  |  Placed: {1}  |  Volume Usage: {2:F1}% ({3:F1} / {4:F1})", i32ArrivalIdx, i32PlacedCount, f32VolumeUsage, f32UsedVolume, f32TotalVolume);

					layer3DStatus.DrawTextCanvas(new CFLPoint<double>(0, 25), strFinalInfo, EColor.YELLOW, EColor.BLACK, 16);

					view3DResult.Invalidate(true);
				}

				// 3D 뷰가 종료될 때 까지 기다림 // Wait for the 3D view to close
				while(view3DResult.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

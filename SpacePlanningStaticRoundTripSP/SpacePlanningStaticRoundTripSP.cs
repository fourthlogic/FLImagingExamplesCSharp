using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ThreeDim;
using FLImagingCLR.ThreeDim.SpacePlanning;

// SpacePlanningStaticSP 고급 예제: top item 만 한 개씩 반대편 상자로 옮긴 뒤 다시 되돌립니다.
// Advanced SpacePlanningStaticSP example: move only top-pickable items to the other bin and back.
// 일부 물품은 XZ 평면 회전, 일부 물품은 모든 축 회전을 허용합니다.
// Some items allow XZ-plane rotation, and one item allows full axis rotation.

namespace FLImagingExamplesCSharp
{
	class SpacePlanningStaticRoundTripSP
	{
		// 배치된 물품 하나. tpMin/tpMax 는 회전 적용 후 영역 // Placed item; tpMin/tpMax store rotated bounds
		class SItemInstance
		{
			public int i32ItemSpecIndex;
			public EAxisRotation eRotation;
			public TPoint3<float> tpMin;
			public TPoint3<float> tpMax;
		}

		static TPoint3<float> GetRotatedItemSize(SItemSpec<float> itemSpec, EAxisRotation eRotation)
		{
			switch(eRotation)
			{
			case EAxisRotation.XYZ:
				return new TPoint3<float>(itemSpec.width, itemSpec.height, itemSpec.depth);
			case EAxisRotation.ZYX:
				return new TPoint3<float>(itemSpec.depth, itemSpec.height, itemSpec.width);
			case EAxisRotation.XZY:
				return new TPoint3<float>(itemSpec.width, itemSpec.depth, itemSpec.height);
			case EAxisRotation.ZXY:
				return new TPoint3<float>(itemSpec.depth, itemSpec.width, itemSpec.height);
			case EAxisRotation.YXZ:
				return new TPoint3<float>(itemSpec.height, itemSpec.width, itemSpec.depth);
			case EAxisRotation.YZX:
				return new TPoint3<float>(itemSpec.height, itemSpec.depth, itemSpec.width);
			}

			return new TPoint3<float>(itemSpec.width, itemSpec.height, itemSpec.depth);
		}

		static SItemInstance MakeItemInstance(List<SItemSpec<float>> listItemSpecs, SPlacementInfo placement)
		{
			TPoint3<float> tpSize = GetRotatedItemSize(listItemSpecs[placement.i32ItemIndex], placement.eRotation);

			SItemInstance instance = new SItemInstance();
			instance.i32ItemSpecIndex = placement.i32ItemIndex;
			instance.eRotation = placement.eRotation;
			instance.tpMin = placement.tpPosition;
			instance.tpMax = new TPoint3<float>(
				placement.tpPosition.x + tpSize.x,
				placement.tpPosition.y + tpSize.y,
				placement.tpPosition.z + tpSize.z);

			return instance;
		}

		// lower 위에 upper 가 얹혀 있는지 판정 // Check whether upper rests on lower
		static bool IsBelow(SItemInstance lower, SItemInstance upper)
		{
			bool bXOverlap = (lower.tpMin.x < upper.tpMax.x) && (upper.tpMin.x < lower.tpMax.x);
			bool bZOverlap = (lower.tpMin.z < upper.tpMax.z) && (upper.tpMin.z < lower.tpMax.z);
			bool bUpperIsAbove = upper.tpMin.y >= lower.tpMax.y - 0.001f;

			return bXOverlap && bZOverlap && bUpperIsAbove;
		}

		// 한 상자의 배치 상태와 상단 물품 판정용 CountAbove // Bin placement state and CountAbove for top-item checks
		class SBinState
		{
			public List<SItemInstance> listItems = new List<SItemInstance>();
			public List<int> listCountAbove = new List<int>();

			public void Clear()
			{
				listItems.Clear();
				listCountAbove.Clear();
			}

			// 물품을 추가하고 아래 물품들의 CountAbove 갱신 // Add an item and update CountAbove of items below it
			public void AddInstance(SItemInstance instance)
			{
				for(int i = 0; i < listItems.Count; ++i)
				{
					if(IsBelow(listItems[i], instance))
						listCountAbove[i]++;
				}

				listItems.Add(instance);
				listCountAbove.Add(0);
			}

			// 지정 타입의 상단 물품을 제거하고 새로 드러난 상단 인덱스를 반환 // Remove one top item and return newly exposed tops
			public CResult RemovePickableOfType(int i32ItemSpecIndex, List<int> listNewlyPickable)
			{
				int i32Found = -1;
				for(int j = 0; j < listItems.Count; ++j)
				{
					if(listItems[j].i32ItemSpecIndex == i32ItemSpecIndex && listCountAbove[j] == 0)
					{
						i32Found = j;
						break;
					}
				}

				if(i32Found < 0)
					return new CResult(EResult.DoesNotExist);

				SItemInstance removed = listItems[i32Found];
				listItems.RemoveAt(i32Found);
				listCountAbove.RemoveAt(i32Found);

				for(int i = 0; i < listItems.Count; ++i)
				{
					if(IsBelow(listItems[i], removed))
					{
						int i32Old = listCountAbove[i];
						listCountAbove[i]--;

						if(i32Old == 1)
							listNewlyPickable.Add(i);
					}
				}

				return new CResult(EResult.OK);
			}
		}

		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		// 캐시가 기준 파일보다 최신인지 확인 // Check whether the cache is newer than the reference file
		static bool IsCacheUpToDate(string strCache, string strReference)
		{
			if(!File.Exists(strCache))
				return false;

			if(!File.Exists(strReference))
				return false;

			return File.GetLastWriteTimeUtc(strCache) > File.GetLastWriteTimeUtc(strReference);
		}

		// 동일 사양으로 알고리즘 구성 및 학습 // Configure and learn with identical specs
		static CResult ConfigureAndLearn(CSpacePlanningStaticSP alg, SBinSpec<float> binSpec,
		                                 List<SItemSpec<float>> listItemSpecs, List<int> listItemCounts)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 재학습 시 사양 중복 추가 방지 // Avoid duplicated specs on re-learn
				alg.Clear();

				if((res = alg.AddBinSpec(binSpec)).IsFail())
					break;

				bool bFailed = false;
				foreach(SItemSpec<float> itemSpec in listItemSpecs)
				{
					if((res = alg.AddItemSpec(itemSpec)).IsFail())
					{
						bFailed = true;
						break;
					}
				}

				if(bFailed)
					break;

				SStaticListParameters parameters = new SStaticListParameters(listItemCounts);
				if((res = alg.SetStaticListParameters(parameters)).IsFail())
					break;

				res = alg.Learn();
			}
			while(false);

			return res;
		}

		// 캐시가 유효하면 Load, 아니면 Learn 후 Save // Load a valid cache, otherwise learn and save
		static CResult LearnOrLoadModel(CSpacePlanningStaticSP alg, string strCache, string strSource,
		                                SBinSpec<float> binSpec, List<SItemSpec<float>> listItemSpecs, List<int> listItemCounts)
		{
			CResult res = new CResult(EResult.UnknownError);

			// 캐시가 소스보다 최신이면 학습 결과를 불러옴 // Load the learned result from a newer cache
			if(IsCacheUpToDate(strCache, strSource))
			{
				res = alg.Load(strCache);

				// PartialOK 는 파라미터만 로드된 상태이므로 재학습 필요 // PartialOK means parameters were loaded but learning is required
				if(res.IsOK() && alg.IsLearned())
				{
					Console.WriteLine("Loaded cached model: {0}", strCache);
					return res;
				}
			}

			// ConfigureAndLearn 이 Clear 하므로 부분 로드 상태여도 안전 // ConfigureAndLearn clears any partial-load state
			if((res = ConfigureAndLearn(alg, binSpec, listItemSpecs, listItemCounts)).IsFail())
				return res;

			// 저장 실패는 예제 진행에 치명적이지 않으므로 경고만 출력 // Save failure is non-fatal for this example
			CResult resSave = alg.Save(strCache);
			if(resSave.IsFail())
				Console.WriteLine("Warning: failed to cache model ({0}): {1}", strCache, resSave.GetString());
			else
				Console.WriteLine("Learned and cached model: {0}", strCache);

			return res;
		}

		// 학습된 알고리즘에서 world 좌표 변환기 구성 // Build a world-space coordinate converter from a learned algorithm
		static CResult SetupConverter(CSpacePlanningBaseSP alg, ref CSpacePlanningCoordinateConverterSP converter, float f32WorldXOffset)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				if((res = alg.GetCoordinateConverter(ref converter)).IsFail())
					break;

				TPoint3<float> tpWorldPivot = new TPoint3<float>(f32WorldXOffset, 0f, 0f);
				TPoint3<float> tpBinPivot = new TPoint3<float>(0f, 0f, 0f);
				TPoint3<float> tpDirectionZ = new TPoint3<float>(0f, 0f, 1f);
				TPoint3<float> tpUpY = new TPoint3<float>(0f, 1f, 0f);

				if((res = converter.SetBinTransform(0, tpWorldPivot, tpBinPivot, tpDirectionZ, tpUpY)).IsFail())
					break;

				int i32ItemCount = alg.GetItemSpecCount();
				bool bFailed = false;
				for(int i = 0; i < i32ItemCount; ++i)
				{
					if((res = converter.SetItemPivotNormalized(i, new TPoint3<float>(0.5f, 0.5f, 0.5f))).IsFail())
					{
						bFailed = true;
						break;
					}
				}

				if(bFailed)
					break;

				res = converter.Learn();
			}
			while(false);

			return res;
		}

		// 지정한 상자 상태를 뷰에 그림 // Draw a bin state into the view
		static CResult PushBinToView(CGUIView3D view3D, CSpacePlanningCoordinateConverterSP converter, SBinState bin)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				CFL3DObjectGroup flogBin = new CFL3DObjectGroup();
				if((res = converter.MakeBinObjectGroup(ref flogBin)).IsFail())
					break;

				int i32BinObjIndex = -1;
				view3D.PushObject(flogBin.GetObjectByIndex(0), ref i32BinObjIndex);
				CGUIView3DObject binObj = view3D.GetView3DObject(i32BinObjIndex);
				if(binObj != null)
					binObj.SetOpacity(0.15f);

				if(bin.listItems.Count == 0)
				{
					res = new CResult(EResult.OK);
					break;
				}

				List<SPlacementInfo> listPlacements = new List<SPlacementInfo>();
				for(int i = 0; i < bin.listItems.Count; ++i)
				{
					SPlacementInfo placement = new SPlacementInfo();
					placement.i32BinIndex = 0;
					placement.i32ItemIndex = bin.listItems[i].i32ItemSpecIndex;
					placement.eRotation = bin.listItems[i].eRotation;
					placement.tpPosition = bin.listItems[i].tpMin;
					listPlacements.Add(placement);
				}

				CFL3DObjectGroup flogItems = new CFL3DObjectGroup();
				if((res = converter.MakeItemObjectGroup(listPlacements, ref flogItems)).IsFail())
					break;

				for(int i = 0; i < bin.listItems.Count; ++i)
				{
					int i32ObjIndex = -1;
					view3D.PushObject(flogItems.GetObjectByIndex(i), ref i32ObjIndex);
					CGUIView3DObject obj = view3D.GetView3DObject(i32ObjIndex);
					if(obj != null)
						obj.SetOpacity(0.6f);
				}

				res = new CResult(EResult.OK);
			}
			while(false);

			return res;
		}

		// source 의 상단 물품을 dstAlg 추천 위치로 모두 이송 // Move all top-pickable source items to destination recommendations
		static CResult TransferAllItems(SBinState srcBin, CSpacePlanningStaticSP dstAlg, SBinState dstBin,
		                                List<SItemSpec<float>> listItemSpecs, string strLabel, Action fnOnStep)
		{
			CResult res = new CResult(EResult.UnknownError);

			if((res = dstAlg.ClearInteractiveStates()).IsFail())
				return res;

			if((res = dstAlg.Execute()).IsFail())
				return res;

			// 현재 상단 물품을 목적지 대기열에 추가 // Push currently top-pickable items into the destination queue
			for(int i = 0; i < srcBin.listItems.Count; ++i)
			{
				if(srcBin.listCountAbove[i] != 0)
					continue;

				if((res = dstAlg.PushItem(srcBin.listItems[i].i32ItemSpecIndex, 1)).IsFail())
					return res;
			}

			int i32Step = 0;
			while(srcBin.listItems.Count > 0)
			{
				// 목적지 추천 물품/위치 조회 // Get the next destination recommendation
				SPlacementInfo placement = null;
				if((res = dstAlg.GetRecommendedNextPlacement(ref placement)).IsFail())
					break;

				// 같은 타입의 상단 물품 제거 // Remove a top item of the recommended type
				List<int> listNewlyPickable = new List<int>();
				if((res = srcBin.RemovePickableOfType(placement.i32ItemIndex, listNewlyPickable)).IsFail())
					return res;

				bool bFailed = false;
				foreach(int i32Idx in listNewlyPickable)
				{
					if((res = dstAlg.PushItem(srcBin.listItems[i32Idx].i32ItemSpecIndex, 1)).IsFail())
					{
						bFailed = true;
						break;
					}
				}

				if(bFailed)
					break;

				// 목적지에 추천 위치 그대로 배치 // Place into the destination at the recommended position
				if((res = dstAlg.AddPlacement(placement)).IsFail())
					return res;

				dstBin.AddInstance(MakeItemInstance(listItemSpecs, placement));

				++i32Step;
				Console.WriteLine("[{0}] step {1,2}: picked item type {2} -> placed at bin {3} [{4:F1}, {5:F1}, {6:F1}]  (source left: {7})",
					strLabel, i32Step, placement.i32ItemIndex, placement.i32BinIndex,
					placement.tpPosition.x, placement.tpPosition.y, placement.tpPosition.z,
					srcBin.listItems.Count);

				if(fnOnStep != null)
					fnOnStep();
			}

			return res;
		}

		[STAThread]
		static void Main(string[] args)
		{
			// FLImaging(R) 라이브러리 사용 전 반드시 한 번 호출
			// Must be called once before using any FLImaging(R) features.
			CLibraryUtilities.Initialize();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DResult = new CGUIView3D();

			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 물품 사양과 개수 설정 // Set item specs and counts
				List<SItemSpec<float>> listItemSpecs = new List<SItemSpec<float>>();
				listItemSpecs.Add(new SItemSpec<float>(3f, 3f, 4f, 1f, ERotationAllowance.VerticalAxisOnly));
				listItemSpecs.Add(new SItemSpec<float>(2f, 4.3f, 5.9f, 1f, ERotationAllowance.VerticalAxisOnly));
				listItemSpecs.Add(new SItemSpec<float>(4f, 3f, 3.5f, 1f, ERotationAllowance.VerticalAxisOnly));
				listItemSpecs.Add(new SItemSpec<float>(5f, 3f, 2.5f, 1f, ERotationAllowance.FullRotation));

				List<int> listItemCounts = new List<int>() { 4, 3, 3, 3 };

				SBinSpec<float> binSpecA = new SBinSpec<float>(9f, 12f, 10f);
				SBinSpec<float> binSpecB = new SBinSpec<float>(10f, 11f, 9f);

				// A/B 상자를 서로 다른 사양으로 학습 // Learn bins A/B with different specs
				CSpacePlanningStaticSP algA = new CSpacePlanningStaticSP();
				CSpacePlanningStaticSP algB = new CSpacePlanningStaticSP();

				// 모델 캐시 파일 설정 // Set model cache files
				string strSource = Assembly.GetExecutingAssembly().Location;
				string strCacheA = string.Format("SpacePlanningStaticRoundTrip_A.{0}", algA.GetFileExtension());
				string strCacheB = string.Format("SpacePlanningStaticRoundTrip_B.{0}", algB.GetFileExtension());

				if((res = LearnOrLoadModel(algA, strCacheA, strSource, binSpecA, listItemSpecs, listItemCounts)).IsFail())
				{
					ErrorPrint(res, "Failed to prepare model for bin A.\n");
					break;
				}

				if((res = LearnOrLoadModel(algB, strCacheB, strSource, binSpecB, listItemSpecs, listItemCounts)).IsFail())
				{
					ErrorPrint(res, "Failed to prepare model for bin B.\n");
					break;
				}

				// A/B 를 world 좌표계에서 나란히 배치 // Place A/B side by side in world coordinates
				CSpacePlanningCoordinateConverterSP converterA = new CSpacePlanningCoordinateConverterSP();
				CSpacePlanningCoordinateConverterSP converterB = new CSpacePlanningCoordinateConverterSP();

				if((res = SetupConverter(algA, ref converterA, 0f)).IsFail() ||
				   (res = SetupConverter(algB, ref converterB, 18f)).IsFail())
				{
					ErrorPrint(res, "Failed to set up coordinate converters.\n");
					break;
				}

				// 학습 결과로 A 의 초기 적재 상태 구성 // Build initial bin A state from learned placements
				SBinState binA = new SBinState();
				SBinState binB = new SBinState();

				List<SPlacementInfo> listLearnedA = new List<SPlacementInfo>();
				if((res = algA.GetLearnedPlacements(ref listLearnedA)).IsFail())
				{
					ErrorPrint(res, "Failed to get learned placements for bin A.\n");
					break;
				}

				foreach(SPlacementInfo p in listLearnedA)
				{
					binA.AddInstance(MakeItemInstance(listItemSpecs, p));
				}

				// 3D 뷰 생성 // Create 3D view
				if((res = view3DResult.Create(600, 0, 1300, 650)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				view3DResult.SetRenderingTransparencyMode(ERenderingTransparencyMode.DepthPeelingOIT);
				view3DResult.SetRenderingResolutionScale(2);

				view3DResult.GetLayer(0).DrawTextCanvas(
					new CFLPoint<double>(0, 0), "Static SP Round Trip - pick top items only", EColor.YELLOW, EColor.BLACK, 20);

				bool bZoomFitted = false;

				// 매 스텝 두 상자 다시 그리기 // Redraw both bins on each step
				Action fnRender = delegate()
				{
					if(!view3DResult.IsAvailable())
						return;

					view3DResult.Lock();
					view3DResult.ClearObjects();

					PushBinToView(view3DResult, converterA, binA);
					PushBinToView(view3DResult, converterB, binB);

					if(!bZoomFitted)
					{
						view3DResult.ZoomFit();
						bZoomFitted = true;
					}

					view3DResult.Unlock();
					view3DResult.Invalidate(true);

					CThreadUtilities.Sleep(500);
				};

				// 초기 상태 표시 // Show the initial state
				fnRender();

				// A -> B 이송 // Transfer A -> B
				Console.WriteLine("Starting transfer A->B.");
				if((res = TransferAllItems(binA, algB, binB, listItemSpecs, "A->B", fnRender)).IsFail())
				{
					if(view3DResult.IsAvailable())
						ErrorPrint(res, "Transfer A->B failed.\n");
					break;
				}

				Console.WriteLine("A->B complete. Switching to B->A after 2 seconds.");
				CThreadUtilities.Sleep(2000);

				// B -> A 이송 (왕복) // Transfer B -> A (round trip)
				Console.WriteLine("Starting transfer B->A.");
				if((res = TransferAllItems(binB, algA, binA, listItemSpecs, "B->A", fnRender)).IsFail())
				{
					if(view3DResult.IsAvailable())
						ErrorPrint(res, "Transfer B->A failed.\n");
					break;
				}

				Console.WriteLine("Round trip complete.");

				// 3D 뷰가 종료될 때 까지 기다림 // Wait for the 3D view to close
				while(view3DResult.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

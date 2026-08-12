using System;
using System.Collections.Generic;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ThreeDim;
using FLImagingCLR.ThreeDim.SpacePlanning;

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

			// 3D 뷰 선언 // Declare 3D views
			CGUIView3D[] arrView3DResults = new CGUIView3D[3];
			for(int i = 0; i < arrView3DResults.Length; ++i)
				arrView3DResults[i] = new CGUIView3D();

			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 알고리즘 객체 선언 // Declare algorithm object
				CSpacePlanningStaticSP alg = new CSpacePlanningStaticSP();

				// Bin spec 설정 // Set the bin spec
				SBinSpec<float> binSpec = new SBinSpec<float>(8f, 6f, 8f);

				if((res = alg.AddBinSpec(binSpec)).IsFail())
				{
					ErrorPrint(res, "Failed to add bin spec.\n");
					break;
				}

				// Item spec 설정 (회전 없음) // Set the item specs (no rotation)
				SItemSpec<float> itemSpec1 = new SItemSpec<float>(3f, 3f, 4f, 1f, ERotationAllowance.NoRotation);
				SItemSpec<float> itemSpec2 = new SItemSpec<float>(4f, 3f, 3f, 1f, ERotationAllowance.NoRotation);
				SItemSpec<float> itemSpec3 = new SItemSpec<float>(5f, 3f, 2f, 1f, ERotationAllowance.NoRotation);

				if((res = alg.AddItemSpec(itemSpec1)).IsFail() ||
				   (res = alg.AddItemSpec(itemSpec2)).IsFail() ||
				   (res = alg.AddItemSpec(itemSpec3)).IsFail())
				{
					ErrorPrint(res, "Failed to add item spec.\n");
					break;
				}

				// Static list 파라미터 설정 // Set the static list parameters
				List<int> itemCounts = new List<int>() { 4, 3, 2 };
				SStaticListParameters parameters = new SStaticListParameters(itemCounts);

				if((res = alg.SetStaticListParameters(parameters)).IsFail())
				{
					ErrorPrint(res, "Failed to set static list parameters.\n");
					break;
				}

				// 같은 item 수량을 직접 순서, 순서 무관, seed shuffle의 세 방식으로 평가
				// Evaluate the same item counts as a direct sequence, order-free supply, and seeded shuffle
				var evaluationCaseA = new CScoreEvaluationCaseSequenceSP(
					new List<int>() { 0, 1, 2, 0, 1, 2, 0, 1, 0 },
					1);
				var evaluationCaseB = new CScoreEvaluationCaseOrderFreeSP(itemCounts);
				var evaluationCaseC = new CScoreEvaluationCaseShuffledSP(itemCounts, 2, 20260729UL);

				if((res = alg.AddScoreEvaluationCase(evaluationCaseA, "Alternating sequence")).IsFail() ||
				   (res = alg.AddScoreEvaluationCase(evaluationCaseB, "Order-free counts")).IsFail() ||
				   (res = alg.AddScoreEvaluationCase(evaluationCaseC, "Seeded shuffle")).IsFail() ||
				   (res = alg.EnableImmediateScoreEvaluation(false)).IsFail())
				{
					ErrorPrint(res, "Failed to configure score-evaluation cases.\n");
					break;
				}

				System.Console.Write("Learning...");

				// Learn은 전략을 준비하고, EvaluateScore mode의 Execute가 같은 case들을 평가
				// Learn prepares strategies; Execute in EvaluateScore mode evaluates the same cases
				if((res = alg.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to learn.\n");
					break;
				}

				if((res = alg.SetExecutionMode(EExecutionMode.EvaluateScore)).IsFail() ||
				   (res = alg.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to evaluate scores.\n");
					break;
				}

				if(!alg.HasValidScoreEvaluation())
				{
					ErrorPrint(new CResult(EResult.NoResult), "Score evaluation did not commit results.\n");
					break;
				}

				// 한 고정 strategy에 대해 case별 요약과 실제 배치 순서를 보관
				// Keep each case summary and actual placement order for one fixed strategy
				var evaluationStrategyId = new SSpacePlanningStrategyId(EStrategyGroup.Search, 0);
				int i32EvaluationCaseCount = alg.GetScoreEvaluationCaseCount();
				if(i32EvaluationCaseCount != 3)
				{
					ErrorPrint(new CResult(EResult.NoResult), "Expected three committed score-evaluation cases.\n");
					break;
				}

				SScoreEvaluationCaseInfo[] arrCaseInfos = new SScoreEvaluationCaseInfo[3];
				SScoreEvaluationResult[] arrEvaluationResults = new SScoreEvaluationResult[3];

				for(int i = 0; i < i32EvaluationCaseCount; ++i)
				{
					arrEvaluationResults[i] = new SScoreEvaluationResult();
					if((res = alg.GetScoreEvaluationCaseInfo(i, ref arrCaseInfos[i])).IsFail() ||
					   (res = alg.GetScoreEvaluationResult(
						   evaluationStrategyId,
						   arrCaseInfos[i].u64CaseId,
						   ref arrEvaluationResults[i])).IsFail())
					{
						ErrorPrint(res, "Failed to get score-evaluation results.\n");
						break;
					}

					Console.WriteLine(
						"Case {0} - {1}: placed {2}/{3}, utilization {4:F2}%",
						(char)('A' + i),
						arrCaseInfos[i].strName,
						arrEvaluationResults[i].i32PlacedItemCount,
						arrCaseInfos[i].i32TotalItemCount,
						arrEvaluationResults[i].f64VolumeUtilization * 100.0);
				}

				if(res.IsFail())
					break;

				CSpacePlanningCoordinateConverterSP converter = new CSpacePlanningCoordinateConverterSP();
				if((res = InitializeCoordinateConverter(alg, ref converter)).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the coordinate converter.\n");
					break;
				}

				CFL3DObjectGroup flogBins = new CFL3DObjectGroup();
				CFL3DObjectGroup[] arrFlogItems = new CFL3DObjectGroup[3];
				if((res = converter.MakeBinObjectGroup(ref flogBins)).IsFail())
				{
					ErrorPrint(res, "Failed to build world-space bin objects.\n");
					break;
				}

				for(int i = 0; i < i32EvaluationCaseCount; ++i)
				{
					arrFlogItems[i] = new CFL3DObjectGroup();
					if((res = converter.MakeItemObjectGroup(arrEvaluationResults[i].lstPlacements, ref arrFlogItems[i])).IsFail())
					{
						ErrorPrint(res, "Failed to build world-space item objects.\n");
						break;
					}

					Console.WriteLine("\nCase {0} placements:", (char)('A' + i));
					for(int j = 0; j < arrEvaluationResults[i].lstPlacements.Count; ++j)
					{
						TPoint3<float> tpWorldPosition = new TPoint3<float>();
						if((res = converter.Convert(arrEvaluationResults[i].lstPlacements[j], ref tpWorldPosition)).IsFail())
						{
							ErrorPrint(res, "Failed to convert placement coordinates.\n");
							break;
						}

						Console.WriteLine("  {0}: bin {1}, item {2} -> world center [{3:F1}, {4:F1}, {5:F1}]",
							j,
							arrEvaluationResults[i].lstPlacements[j].i32BinIndex,
							arrEvaluationResults[i].lstPlacements[j].i32ItemIndex,
							tpWorldPosition.x, tpWorldPosition.y, tpWorldPosition.z);
					}

					if(res.IsFail())
						break;
				}

				if(res.IsFail())
					break;

				int i32BinCount = alg.GetBinSpecCount();
				int i32ViewWidth = 600;
				int i32ViewHeight = 500;

				for(int i = 0; i < i32EvaluationCaseCount; ++i)
				{
					CGUIView3D view3DResult = arrView3DResults[i];
					SScoreEvaluationCaseInfo info = arrCaseInfos[i];
					SScoreEvaluationResult evaluationResult = arrEvaluationResults[i];
					int i32PlacedCount = evaluationResult.lstPlacements.Count;

					if((res = view3DResult.Create(i32ViewWidth * i, 0, i32ViewWidth * (i + 1), i32ViewHeight)).IsFail())
					{
						ErrorPrint(res, "Failed to create a 3D view.\n");
						break;
					}

					view3DResult.SetRenderingTransparencyMode(ERenderingTransparencyMode.DepthPeelingOIT);
					view3DResult.SetRenderingResolutionScale(2);

					// 결과 뷰에 해당 case의 world-space 아이템 및 bin 오브젝트 추가
					// Push this case's world-space item and bin objects to its result view
					for(int j = 0; j < i32PlacedCount; ++j)
					{
						int i32ObjIndex = -1;
						if((res = view3DResult.PushObject(arrFlogItems[i].GetObjectByIndex(j), ref i32ObjIndex)).IsFail())
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

					for(int j = 0; j < i32BinCount; ++j)
					{
						int i32ObjIndex = -1;
						if((res = view3DResult.PushObject(flogBins.GetObjectByIndex(j), ref i32ObjIndex)).IsFail())
						{
							ErrorPrint(res, "Failed to push 3D object.\n");
							break;
						}

						CGUIView3DObject objFilled = view3DResult.GetView3DObject(i32ObjIndex);
						if(objFilled != null)
							objFilled.SetOpacity(0.2f);
					}

					if(res.IsFail())
						break;

					// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 from the 3D view for display
					// 이 객체는 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to the view and does not need to be released separately
					CGUIView3DLayer layer3DResult = view3DResult.GetLayer(0);
					layer3DResult.DrawTextCanvas(
						new CFLPoint<double>(0, 0),
						string.Format("Case {0} - {1}", (char)('A' + i), info.strName),
						EColor.YELLOW,
						EColor.BLACK,
						20);

					// 결과 정보를 3D 뷰에 텍스트로 표시 // Draw result summary text on the 3D view
					string strResultInfo = string.Format(
						"Evaluation strategy: group={0}, id={1}\n"
						+ "Placed items: {2}/{3}\n"
						+ "Volume utilization: {4:F1}%\n"
						+ "Coordinate converter: world-space center pivot",
						evaluationStrategyId.eGroup,
						evaluationStrategyId.i32IDInStrategy,
						evaluationResult.i32PlacedItemCount,
						info.i32TotalItemCount,
						evaluationResult.f64VolumeUtilization * 100.0);

					layer3DResult.DrawTextCanvas(new CFLPoint<double>(0, 25), strResultInfo, EColor.YELLOW, EColor.BLACK, 16);

					// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다.
					// With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
					view3DResult.ZoomFit();

					// 이미지 뷰를 갱신 합니다. // Update image view
					view3DResult.Invalidate(true);
				}

				if(res.IsFail())
					break;

				for(int i = 1; i < i32EvaluationCaseCount; ++i)
				{
					arrView3DResults[0].SynchronizePointOfView(ref arrView3DResults[i]);
					arrView3DResults[0].SynchronizeWindow(ref arrView3DResults[i]);
				}

				// 3D 뷰 중 하나가 종료될 때까지 기다림 // Wait until any of the three 3D views is closed
				while(arrView3DResults[0].IsAvailable() &&
					  arrView3DResults[1].IsAvailable() &&
					  arrView3DResults[2].IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

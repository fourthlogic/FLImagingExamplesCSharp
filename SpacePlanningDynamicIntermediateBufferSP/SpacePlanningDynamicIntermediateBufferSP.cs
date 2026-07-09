using System;
using System.Collections.Generic;
using System.Diagnostics;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.ThreeDim;
using FLImagingCLR.ThreeDim.SpacePlanning;

namespace FLImagingExamplesCSharp
{
	class SpacePlanningDynamicIntermediateBufferSP
	{
		const int i32BinDestination = 0;
		const int i32BinBuffer = 1;
		const int i32BinCount = 2;

		const float f32BinWorldSpacingX = 18f;
		const float f32Pi = 3.14159265358979f;
		const float f32AnimationArcHeight = 4f;
		const double f64AnimationDurationMs = 480.0;
		const int i32AnimationSleepMs = 1;
		const float f32SourcePreviewMaxTiltDegree = 20f;
		const ulong u64SourceItemTypeRandomSeed = 0x9d84a3d390df0c46UL;
		const ulong u64SourceRotationRandomSeed = 0x6a09e667f3bcc909UL;

		enum ESourceState
		{
			NeedNewSource,
			HasSource,
		}

		class SSourceSlot
		{
			public ESourceState eState;
			public int i32ItemType;
			public int i32ArrivalIndex;
			public CFLGeometry3DQuaternion<float> quatLocalRotation;
		}

		class SRotationBasis
		{
			public TPoint3<float> tpAxisX;
			public TPoint3<float> tpAxisY;
			public TPoint3<float> tpAxisZ;
		}

		class SBinFrame
		{
			public TPoint3<float> tpWorldPivot;
			public TPoint3<float> tpBinPivot;
			public SRotationBasis basis;
		}

		class SAnimationPose
		{
			public TPoint3<float> tpWorldCenter;
			public CFLGeometry3DQuaternion<float> quatRotation;
		}

		class SRuntimeModelSpecs
		{
			public SBinSpec<float>[] arrBinSpecs = new SBinSpec<float>[i32BinCount];
			public List<SItemSpec<float>> listItemSpecs = new List<SItemSpec<float>>();
		}

		class SItemInstance
		{
			public int i32ItemSpecIndex;
			public EAxisRotation eRotation;
			public TPoint3<float> tpMin;
			public TPoint3<float> tpMax;
		}

		class SBinState
		{
			public List<SItemInstance> listItems = new List<SItemInstance>();
			public List<int> listCountAbove = new List<int>();

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

			public CResult GetFirstPickableIndexOfType(int i32ItemSpecIndex, ref int i32PlacedIndex)
			{
				for(int i = 0; i < listItems.Count; ++i)
				{
					if(listItems[i].i32ItemSpecIndex == i32ItemSpecIndex && listCountAbove[i] == 0)
					{
						i32PlacedIndex = i;
						return new CResult(EResult.OK);
					}
				}

				i32PlacedIndex = -1;
				return new CResult(EResult.DoesNotExist);
			}

			public CResult RemovePickableAt(int i32PlacedIndex)
			{
				if(i32PlacedIndex < 0 || i32PlacedIndex >= listItems.Count || listCountAbove[i32PlacedIndex] != 0)
					return new CResult(EResult.DoesNotExist);

				SItemInstance removed = listItems[i32PlacedIndex];
				listItems.RemoveAt(i32PlacedIndex);
				listCountAbove.RemoveAt(i32PlacedIndex);

				for(int i = 0; i < listItems.Count; ++i)
				{
					if(IsBelow(listItems[i], removed))
						listCountAbove[i]--;
				}

				return new CResult(EResult.OK);
			}
		}

		delegate void FnAnimateMove(int i32ItemType, SAnimationPose poseStart, SAnimationPose poseEnd);

		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		static float Clamp01(double f64Value)
		{
			if(f64Value <= 0.0)
				return 0f;
			if(f64Value >= 1.0)
				return 1f;

			return (float)f64Value;
		}

		static TPoint3<float> Add(TPoint3<float> tpA, TPoint3<float> tpB)
		{
			return new TPoint3<float>(tpA.x + tpB.x, tpA.y + tpB.y, tpA.z + tpB.z);
		}

		static TPoint3<float> Sub(TPoint3<float> tpA, TPoint3<float> tpB)
		{
			return new TPoint3<float>(tpA.x - tpB.x, tpA.y - tpB.y, tpA.z - tpB.z);
		}

		static TPoint3<float> Scale(TPoint3<float> tpValue, float f32Scale)
		{
			return new TPoint3<float>(tpValue.x * f32Scale, tpValue.y * f32Scale, tpValue.z * f32Scale);
		}

		static float Dot(TPoint3<float> tpA, TPoint3<float> tpB)
		{
			return tpA.x * tpB.x + tpA.y * tpB.y + tpA.z * tpB.z;
		}

		static TPoint3<float> GetBinWorldOrigin(int i32BinIndex)
		{
			return new TPoint3<float>(f32BinWorldSpacingX * i32BinIndex, 0f, 0f);
		}

		static TPoint3<float> Lerp(TPoint3<float> tpStart, TPoint3<float> tpEnd, float f32T)
		{
			return new TPoint3<float>(
				tpStart.x + (tpEnd.x - tpStart.x) * f32T,
				tpStart.y + (tpEnd.y - tpStart.y) * f32T,
				tpStart.z + (tpEnd.z - tpStart.z) * f32T);
		}

		static SRotationBasis MakeRotationBasis(TPoint3<float> tpAxisX, TPoint3<float> tpAxisY, TPoint3<float> tpAxisZ)
		{
			SRotationBasis basis = new SRotationBasis();
			basis.tpAxisX = tpAxisX;
			basis.tpAxisY = tpAxisY;
			basis.tpAxisZ = tpAxisZ;
			return basis;
		}

		static CFLGeometry3DQuaternion<float> MakeQuaternionFromBasis(SRotationBasis basis)
		{
			CMatrixFor3D<float> matRotation = new CMatrixFor3D<float>(
				basis.tpAxisX.x, basis.tpAxisY.x, basis.tpAxisZ.x,
				basis.tpAxisX.y, basis.tpAxisY.y, basis.tpAxisZ.y,
				basis.tpAxisX.z, basis.tpAxisY.z, basis.tpAxisZ.z);

			CFLGeometry3DQuaternion<float> quat = new CFLGeometry3DQuaternion<float>();
			quat.SetMatrix(matRotation);
			quat.Normalize();
			return quat;
		}

		static SRotationBasis MakeRotationBasisFromQuaternion(CFLGeometry3DQuaternion<float> quat)
		{
			CMatrixFor3D<float> matRotation = new CMatrixFor3D<float>();
			quat.GetMatrix(ref matRotation);

			return MakeRotationBasis(
				new TPoint3<float>(matRotation[0, 0], matRotation[1, 0], matRotation[2, 0]),
				new TPoint3<float>(matRotation[0, 1], matRotation[1, 1], matRotation[2, 1]),
				new TPoint3<float>(matRotation[0, 2], matRotation[1, 2], matRotation[2, 2]));
		}

		static TPoint3<float> TransformBinDirection(SRotationBasis binBasis, TPoint3<float> tpLocalDirection)
		{
			return Add(
				Add(Scale(binBasis.tpAxisX, tpLocalDirection.x), Scale(binBasis.tpAxisY, tpLocalDirection.y)),
				Scale(binBasis.tpAxisZ, tpLocalDirection.z));
		}

		static TPoint3<float> BinLocalDirectionFromWorld(SRotationBasis binBasis, TPoint3<float> tpWorldDirection)
		{
			return new TPoint3<float>(
				Dot(tpWorldDirection, binBasis.tpAxisX),
				Dot(tpWorldDirection, binBasis.tpAxisY),
				Dot(tpWorldDirection, binBasis.tpAxisZ));
		}

		static CFL3DCamera GetWorldCamera()
		{
			CFL3DCamera cam = new CFL3DCamera();
			cam.SetProjectionType(E3DCameraProjectionType.Perspective);
			cam.SetDirection(new CFLPoint3<float>(0f, -0.85f, -0.53f));
			cam.SetDirectionUp(new CFLPoint3<float>(0f, 0.53f, -0.85f));
			cam.SetPosition(new CFLPoint3<float>(14f, 55f, 30f));
			return cam;
		}

		static CFLGeometry3DQuaternion<float> MakeBinFrameRotation(int i32BinIndex)
		{
			CFLGeometry3DQuaternion<float> quat = new CFLGeometry3DQuaternion<float>();
			if(i32BinIndex == i32BinDestination)
				quat.SetEulerAngles(EEulerSequence.Extrinsic_XYZ, 6f, 8f, 5f);
			else
				quat.SetEulerAngles(EEulerSequence.Extrinsic_XYZ, -7f, 5f, -6f);

			quat.Normalize();
			return quat;
		}

		static SBinFrame GetBinFrame(int i32BinIndex)
		{
			SBinFrame frame = new SBinFrame();
			frame.tpWorldPivot = GetBinWorldOrigin(i32BinIndex);
			frame.tpBinPivot = new TPoint3<float>(0f, 0f, 0f);
			frame.basis = MakeRotationBasisFromQuaternion(MakeBinFrameRotation(i32BinIndex));
			return frame;
		}

		static TPoint3<float> WorldFromBinLocal(int i32BinIndex, TPoint3<float> tpLocal)
		{
			SBinFrame frame = GetBinFrame(i32BinIndex);
			TPoint3<float> tpDelta = Sub(tpLocal, frame.tpBinPivot);
			return Add(frame.tpWorldPivot, TransformBinDirection(frame.basis, tpDelta));
		}

		static TPoint3<float> BinLocalFromWorld(int i32BinIndex, TPoint3<float> tpWorld)
		{
			SBinFrame frame = GetBinFrame(i32BinIndex);
			TPoint3<float> tpDeltaWorld = Sub(tpWorld, frame.tpWorldPivot);
			return Add(frame.tpBinPivot, BinLocalDirectionFromWorld(frame.basis, tpDeltaWorld));
		}

		static CFLGeometry3DQuaternion<float> MakeWorldRotationFromLocalRotation(int i32BinIndex, CFLGeometry3DQuaternion<float> quatLocalRotation)
		{
			CFLGeometry3DQuaternion<float> quatWorldRotation = MakeBinFrameRotation(i32BinIndex) * quatLocalRotation;
			quatWorldRotation.Normalize();
			return quatWorldRotation;
		}

		static CFLGeometry3DQuaternion<float> MakeQuaternionFromRotationVector(TPoint3<float> tpRotationVector)
		{
			float f32Angle = (float)Math.Sqrt(Dot(tpRotationVector, tpRotationVector));
			CFLGeometry3DQuaternion<float> quat = new CFLGeometry3DQuaternion<float>();
			if(f32Angle <= 1e-6f)
			{
				quat.SetEulerAngles(EEulerSequence.Extrinsic_XYZ, 0f, 0f, 0f);
			}
			else
			{
				float f32InvAngle = 1f / f32Angle;
				quat.SetAxisAndAngle(
					new CFLGeometry3DVector<float>(tpRotationVector.x * f32InvAngle, tpRotationVector.y * f32InvAngle, tpRotationVector.z * f32InvAngle),
					f32Angle);
			}
			quat.Normalize();
			return quat;
		}

		static CFLGeometry3DQuaternion<float> DrawSourcePreviewLocalRotation(CXorshiroRandomGenerator rng)
		{
			float f32MaxTiltRadian = f32SourcePreviewMaxTiltDegree * f32Pi / 180f;
			float f32Tilt = (float)Math.Sqrt(rng.GenerateUniformRandomValueF32(0f, 1f)) * f32MaxTiltRadian;
			float f32Azimuth = rng.GenerateUniformRandomValueF32(0f, 2f * f32Pi);
			TPoint3<float> tpRotationVector = new TPoint3<float>(
				f32Tilt * (float)Math.Cos(f32Azimuth),
				0f,
				f32Tilt * (float)Math.Sin(f32Azimuth));
			return MakeQuaternionFromRotationVector(tpRotationVector);
		}

		static CFLGeometry3DQuaternion<float> GetAxisRotationLocalQuaternion(EAxisRotation eRotation)
		{
			SRotationBasis basis;

			switch(eRotation)
			{
			case EAxisRotation.ZYX:
				basis = MakeRotationBasis(
					new TPoint3<float>(0f, 0f, 1f),
					new TPoint3<float>(0f, 1f, 0f),
					new TPoint3<float>(-1f, 0f, 0f));
				break;
			case EAxisRotation.XZY:
				basis = MakeRotationBasis(
					new TPoint3<float>(1f, 0f, 0f),
					new TPoint3<float>(0f, 0f, 1f),
					new TPoint3<float>(0f, -1f, 0f));
				break;
			case EAxisRotation.ZXY:
				basis = MakeRotationBasis(
					new TPoint3<float>(0f, 1f, 0f),
					new TPoint3<float>(0f, 0f, 1f),
					new TPoint3<float>(1f, 0f, 0f));
				break;
			case EAxisRotation.YXZ:
				basis = MakeRotationBasis(
					new TPoint3<float>(0f, 1f, 0f),
					new TPoint3<float>(-1f, 0f, 0f),
					new TPoint3<float>(0f, 0f, 1f));
				break;
			case EAxisRotation.YZX:
				basis = MakeRotationBasis(
					new TPoint3<float>(0f, 0f, 1f),
					new TPoint3<float>(1f, 0f, 0f),
					new TPoint3<float>(0f, 1f, 0f));
				break;
			case EAxisRotation.XYZ:
			default:
				basis = MakeRotationBasis(
					new TPoint3<float>(1f, 0f, 0f),
					new TPoint3<float>(0f, 1f, 0f),
					new TPoint3<float>(0f, 0f, 1f));
				break;
			}

			return MakeQuaternionFromBasis(basis);
		}

		static TPoint3<float> ApplyTransform(CMatrixFor3D<float> matRotation, TPoint3<float> tpOffset, TPoint3<float> tpLocal)
		{
			return new TPoint3<float>(
				tpOffset.x + matRotation[0, 0] * tpLocal.x + matRotation[0, 1] * tpLocal.y + matRotation[0, 2] * tpLocal.z,
				tpOffset.y + matRotation[1, 0] * tpLocal.x + matRotation[1, 1] * tpLocal.y + matRotation[1, 2] * tpLocal.z,
				tpOffset.z + matRotation[2, 0] * tpLocal.x + matRotation[2, 1] * tpLocal.y + matRotation[2, 2] * tpLocal.z);
		}

		static TPoint3<float> GetObjectExtents(SItemSpec<float> itemSpec, CFLGeometry3DQuaternion<float> quatRotation)
		{
			CMatrixFor3D<float> matRotation = new CMatrixFor3D<float>();
			quatRotation.GetMatrix(ref matRotation);

			float f32HalfWidth = itemSpec.width * 0.5f;
			float f32HalfHeight = itemSpec.height * 0.5f;
			float f32HalfDepth = itemSpec.depth * 0.5f;

			return new TPoint3<float>(
				f32HalfWidth * Math.Abs(matRotation[0, 0]) + f32HalfHeight * Math.Abs(matRotation[0, 1]) + f32HalfDepth * Math.Abs(matRotation[0, 2]),
				f32HalfWidth * Math.Abs(matRotation[1, 0]) + f32HalfHeight * Math.Abs(matRotation[1, 1]) + f32HalfDepth * Math.Abs(matRotation[1, 2]),
				f32HalfWidth * Math.Abs(matRotation[2, 0]) + f32HalfHeight * Math.Abs(matRotation[2, 1]) + f32HalfDepth * Math.Abs(matRotation[2, 2]));
		}

		static SAnimationPose MakePoseFromBinLocalAabbMin(SItemSpec<float> itemSpec, int i32BinIndex, TPoint3<float> tpAabbMinBinLocal, CFLGeometry3DQuaternion<float> quatLocalRotation)
		{
			SAnimationPose pose = new SAnimationPose();
			pose.quatRotation = MakeWorldRotationFromLocalRotation(i32BinIndex, quatLocalRotation);
			pose.tpWorldCenter = WorldFromBinLocal(i32BinIndex, Add(tpAabbMinBinLocal, GetObjectExtents(itemSpec, quatLocalRotation)));
			return pose;
		}

		static SAnimationPose LerpArc(SAnimationPose start, SAnimationPose end, float f32T, float f32ArcHeight)
		{
			SAnimationPose pose = new SAnimationPose();
			pose.tpWorldCenter = Lerp(start.tpWorldCenter, end.tpWorldCenter, f32T);
			pose.tpWorldCenter.y += f32ArcHeight * (float)Math.Sin(f32Pi * f32T);
			pose.quatRotation = new CFLGeometry3DQuaternion<float>();
			pose.quatRotation.SetSphericalLinearInterpolation(start.quatRotation, end.quatRotation, f32T);
			return pose;
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

		static TPoint3<float> GetSourcePreviewLocalPos(SItemSpec<float> itemSpec, SBinSpec<float> binSpecBuffer)
		{
			TPoint3<float> tpSize = GetRotatedItemSize(itemSpec, EAxisRotation.XYZ);
			return new TPoint3<float>(binSpecBuffer.width + 2f, 0f, (binSpecBuffer.depth - tpSize.z) * 0.5f);
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

		static bool IsBelow(SItemInstance lower, SItemInstance upper)
		{
			bool bXOverlap = (lower.tpMin.x < upper.tpMax.x) && (upper.tpMin.x < lower.tpMax.x);
			bool bZOverlap = (lower.tpMin.z < upper.tpMax.z) && (upper.tpMin.z < lower.tpMax.z);
			bool bUpperIsAbove = upper.tpMin.y >= lower.tpMax.y - 0.001f;

			return bXOverlap && bZOverlap && bUpperIsAbove;
		}

		static bool WouldCoverBufferItem(SBinState binBuffer, SItemInstance item)
		{
			for(int i = 0; i < binBuffer.listItems.Count; ++i)
			{
				if(IsBelow(binBuffer.listItems[i], item))
					return true;
			}

			return false;
		}

		static SPlacementInfo MakePlacementInfo(int i32BinIndex, SItemInstance item)
		{
			SPlacementInfo placement = new SPlacementInfo();
			placement.i32BinIndex = i32BinIndex;
			placement.i32ItemIndex = item.i32ItemSpecIndex;
			placement.eRotation = item.eRotation;
			placement.tpPosition = item.tpMin;
			return placement;
		}

		static CResult InitializeCoordinateConverter(CSpacePlanningBaseSP alg, ref CSpacePlanningCoordinateConverterSP converter)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				if((res = alg.GetCoordinateConverter(ref converter)).IsFail())
					break;

				for(int i = 0; i < alg.GetBinSpecCount(); ++i)
				{
					SBinFrame frame = GetBinFrame(i);
					if((res = converter.SetBinTransform(i, frame.tpWorldPivot, frame.tpBinPivot, frame.basis.tpAxisZ, frame.basis.tpAxisY)).IsFail())
						break;
				}

				if(res.IsFail())
					break;

				for(int i = 0; i < alg.GetItemSpecCount(); ++i)
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

		static void InitializeDefaultSourceItemChances(List<float> itemChances)
		{
			itemChances.Clear();
			itemChances.Add(4f);
			itemChances.Add(3f);
			itemChances.Add(3f);
			itemChances.Add(2f);
		}

		static CResult LearnDefaultModel(CSpacePlanningDynamicSP alg, List<float> itemChances)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				SBinSpec<float>[] arrDefaultBinSpecs = new SBinSpec<float>[]
				{
					new SBinSpec<float>(9f, 12f, 10f),
					new SBinSpec<float>(6f, 5f, 6f),
				};

				for(int i = 0; i < arrDefaultBinSpecs.Length; ++i)
				{
					if((res = alg.AddBinSpec(arrDefaultBinSpecs[i])).IsFail())
						break;
				}

				if(res.IsFail())
					break;

				SItemSpec<float>[] arrDefaultItemSpecs = new SItemSpec<float>[]
				{
					new SItemSpec<float>(3.0f, 3.0f, 4.0f, 1f, ERotationAllowance.VerticalAxisOnly),
					new SItemSpec<float>(2.0f, 4.3f, 5.9f, 1f, ERotationAllowance.VerticalAxisOnly),
					new SItemSpec<float>(4.0f, 3.0f, 3.5f, 1f, ERotationAllowance.VerticalAxisOnly),
					new SItemSpec<float>(5.0f, 3.0f, 2.5f, 1f, ERotationAllowance.FullRotation),
				};

				for(int i = 0; i < arrDefaultItemSpecs.Length; ++i)
				{
					if((res = alg.AddItemSpec(arrDefaultItemSpecs[i])).IsFail())
						break;
				}

				if(res.IsFail())
					break;

				InitializeDefaultSourceItemChances(itemChances);
				SRandomSequenceParameters parameters = SRandomSequenceParameters.CreateInfinite(itemChances, 2);
				if((res = alg.SetRandomSequenceParameters(parameters)).IsFail())
					break;

				if((res = alg.Learn()).IsFail())
					break;

				res = alg.SelectStrategy(alg.GetOptimalStrategyId());
			}
			while(false);

			return res;
		}

		static CResult LoadRuntimeSpecsFromModel(CSpacePlanningBaseSP alg, SRuntimeModelSpecs modelSpecs)
		{
			if(alg.GetBinSpecCount() < i32BinCount || alg.GetItemSpecCount() <= 0)
				return new CResult(EResult.InvalidData);

			for(int i = 0; i < i32BinCount; ++i)
				modelSpecs.arrBinSpecs[i] = alg.GetBinSpec(i);

			modelSpecs.listItemSpecs.Clear();
			for(int i = 0; i < alg.GetItemSpecCount(); ++i)
				modelSpecs.listItemSpecs.Add(alg.GetItemSpec(i));

			return new CResult(EResult.OK);
		}

		static bool IsSameItemSpec(SItemSpec<float> lhs, SItemSpec<float> rhs)
		{
			return lhs.width == rhs.width &&
				lhs.height == rhs.height &&
				lhs.depth == rhs.depth &&
				lhs.load == rhs.load &&
				lhs.eAllowed == rhs.eAllowed;
		}

		static CResult ValidateSameItemSpecs(CSpacePlanningBaseSP alg, List<SItemSpec<float>> listItemSpecs)
		{
			if(alg.GetItemSpecCount() != listItemSpecs.Count)
				return new CResult(EResult.Mismatched);

			for(int i = 0; i < alg.GetItemSpecCount(); ++i)
			{
				if(!IsSameItemSpec(alg.GetItemSpec(i), listItemSpecs[i]))
					return new CResult(EResult.Mismatched);
			}

			return new CResult(EResult.OK);
		}

		static CResult PushBinToView(CGUIView3D view3D, CSpacePlanningCoordinateConverterSP converter, CFL3DObjectGroup flogBins, SBinState bin, int i32BinIndex)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				int i32BinObjIndex = -1;
				view3D.PushObject(flogBins.GetObjectByIndex(i32BinIndex), ref i32BinObjIndex);
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
					listPlacements.Add(MakePlacementInfo(i32BinIndex, bin.listItems[i]));

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

		static CResult CaptureObjectLocalVertices(CGUIView3D view3D, int i32ObjIndex, TPoint3<float> tpWorldCenter, List<TPoint3<float>> listLocalVertices)
		{
			CGUIView3DObject viewObject = view3D.GetView3DObject(i32ObjIndex);
			if(viewObject == null)
				return new CResult(EResult.DoesNotExist);

			CFL3DObject object3D = viewObject.Get3DObject();
			if(object3D == null)
				return new CResult(EResult.DoesNotExist);

			listLocalVertices.Clear();
			List<TPoint3<float>> listVertices = object3D.m_listVertex;
			for(int i = 0; i < listVertices.Count; ++i)
				listLocalVertices.Add(Sub(listVertices[i], tpWorldCenter));

			return new CResult(EResult.OK);
		}

		static CResult UpdateItemObjectPose(CGUIView3D view3D, int i32ObjIndex, List<TPoint3<float>> listLocalVertices, SAnimationPose pose)
		{
			CResult res = new CResult(EResult.UnknownError);

			CGUIView3DObject viewObject = view3D.GetView3DObject(i32ObjIndex);
			if(viewObject == null)
				return new CResult(EResult.DoesNotExist);

			CFL3DObject object3D = viewObject.Get3DObject();
			if(object3D == null || object3D.m_listVertex.Count != listLocalVertices.Count)
				return new CResult(EResult.DoesNotExist);

			CMatrixFor3D<float> matRotation = new CMatrixFor3D<float>();
			if((res = pose.quatRotation.GetMatrix(ref matRotation)).IsFail())
				return res;

			for(int i = 0; i < listLocalVertices.Count; ++i)
			{
				TPoint3<float> tpVertex = ApplyTransform(matRotation, pose.tpWorldCenter, listLocalVertices[i]);
				if((res = object3D.SetVertexAt(i, tpVertex)).IsFail())
					return res;
			}

			viewObject.UpdateVertex(true);
			return new CResult(EResult.OK);
		}

		static CResult PushItemObjectAtPose(CGUIView3D view3D, CSpacePlanningCoordinateConverterSP converter, SItemSpec<float> itemSpec, int i32ItemSpecIndex, int i32RenderBinIndex, SAnimationPose pose, float f32Opacity, ref int i32ObjIndex, List<TPoint3<float>> listLocalVertices)
		{
			CResult res = new CResult(EResult.UnknownError);
			i32ObjIndex = -1;

			do
			{
				TPoint3<float> tpCenterBinLocal = BinLocalFromWorld(i32RenderBinIndex, pose.tpWorldCenter);
				TPoint3<float> tpUnrotatedMinBinLocal = new TPoint3<float>(
					tpCenterBinLocal.x - itemSpec.width * 0.5f,
					tpCenterBinLocal.y - itemSpec.height * 0.5f,
					tpCenterBinLocal.z - itemSpec.depth * 0.5f);

				SPlacementInfo placement = new SPlacementInfo();
				placement.i32BinIndex = i32RenderBinIndex;
				placement.i32ItemIndex = i32ItemSpecIndex;
				placement.eRotation = EAxisRotation.XYZ;
				placement.tpPosition = tpUnrotatedMinBinLocal;

				List<SPlacementInfo> listPlacement = new List<SPlacementInfo>();
				listPlacement.Add(placement);

				CFL3DObjectGroup flogItem = new CFL3DObjectGroup();
				if((res = converter.MakeItemObjectGroup(listPlacement, ref flogItem)).IsFail())
					break;

				view3D.PushObject(flogItem.GetObjectByIndex(0), ref i32ObjIndex);
				CGUIView3DObject viewObject = view3D.GetView3DObject(i32ObjIndex);
				if(viewObject != null)
					viewObject.SetOpacity(f32Opacity);

				List<TPoint3<float>> listCapturedLocalVertices = new List<TPoint3<float>>();
				if((res = CaptureObjectLocalVertices(view3D, i32ObjIndex, pose.tpWorldCenter, listCapturedLocalVertices)).IsFail())
					break;

				SRotationBasis renderBinBasis = GetBinFrame(i32RenderBinIndex).basis;
				for(int i = 0; i < listCapturedLocalVertices.Count; ++i)
					listCapturedLocalVertices[i] = BinLocalDirectionFromWorld(renderBinBasis, listCapturedLocalVertices[i]);

				if((res = UpdateItemObjectPose(view3D, i32ObjIndex, listCapturedLocalVertices, pose)).IsFail())
					break;

				view3D.UpdateObject(i32ObjIndex);

				if(listLocalVertices != null)
				{
					listLocalVertices.Clear();
					listLocalVertices.AddRange(listCapturedLocalVertices);
				}

				res = new CResult(EResult.OK);
			}
			while(false);

			return res;
		}

		static CResult PushSourcePreviewToView(CGUIView3D view3D, CSpacePlanningCoordinateConverterSP converter, SItemSpec<float> itemSpec, int i32ItemSpecIndex, SBinSpec<float> binSpecBuffer, CFLGeometry3DQuaternion<float> quatLocalRotation)
		{
			SAnimationPose poseSource = MakePoseFromBinLocalAabbMin(itemSpec, i32BinBuffer, GetSourcePreviewLocalPos(itemSpec, binSpecBuffer), quatLocalRotation);
			int i32ObjIndex = -1;
			return PushItemObjectAtPose(view3D, converter, itemSpec, i32ItemSpecIndex, i32BinBuffer, poseSource, 0.85f, ref i32ObjIndex, null);
		}

		static CResult PushInFlightItemToView(CGUIView3D view3D, CSpacePlanningCoordinateConverterSP converter, SItemSpec<float> itemSpec, int i32ItemSpecIndex, SAnimationPose pose, ref int i32ObjIndex, List<TPoint3<float>> listLocalVertices)
		{
			return PushItemObjectAtPose(view3D, converter, itemSpec, i32ItemSpecIndex, i32BinDestination, pose, 0.95f, ref i32ObjIndex, listLocalVertices);
		}

		static CResult RebuildInteractiveState(CSpacePlanningDynamicSP alg, SBinState[] arrBins, int i32ExcludedBinIndex, int i32ExcludedPlacedIndex)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				if((res = alg.ClearInteractiveStates()).IsFail())
					break;

				if((res = alg.Execute()).IsFail())
					break;

				bool bFailed = false;
				for(int i32BinIndex = 0; i32BinIndex < i32BinCount; ++i32BinIndex)
				{
					SBinState bin = arrBins[i32BinIndex];
					for(int i = 0; i < bin.listItems.Count; ++i)
					{
						if(i32BinIndex == i32ExcludedBinIndex && i == i32ExcludedPlacedIndex)
							continue;

						SPlacementInfo placement = MakePlacementInfo(i32BinIndex, bin.listItems[i]);
						if((res = alg.PushItem(placement.i32ItemIndex, 1)).IsFail() ||
						   (res = alg.AddPlacement(placement)).IsFail())
						{
							bFailed = true;
							break;
						}
					}

					if(bFailed)
						break;
				}

				if(bFailed)
					break;

				res = new CResult(EResult.OK);
			}
			while(false);

			return res;
		}

		static CResult FindRecommendedPlacementInBin(CSpacePlanningDynamicSP alg, int i32BinIndex, ref SPlacementInfo placement)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				List<SPlacementInfo> listCandidates = new List<SPlacementInfo>();
				if((res = alg.GetRecommendedNextPlacements(256, ref listCandidates)).IsFail())
					break;

				for(int i = 0; i < listCandidates.Count; ++i)
				{
					if(listCandidates[i].i32BinIndex == i32BinIndex)
					{
						placement = listCandidates[i];
						return new CResult(EResult.OK);
					}
				}

				res = new CResult(EResult.DoesNotExist);
			}
			while(false);

			return res;
		}

		static CResult TryPlaceSourceInBuffer(CSpacePlanningDynamicSP alg, SBinState[] arrBins, List<SItemSpec<float>> listItemSpecs, SBinSpec<float> binSpecBuffer, SSourceSlot sourceSlot, ref bool bPlaced, Action fnOnStep, FnAnimateMove fnAnimateMove)
		{
			CResult res = new CResult(EResult.UnknownError);
			bPlaced = false;
			int i32SourceItemType = sourceSlot.i32ItemType;
			int i32ArrivalIndex = sourceSlot.i32ArrivalIndex;

			do
			{
				if((res = RebuildInteractiveState(alg, arrBins, -1, -1)).IsFail())
					break;

				if((res = alg.PushItem(i32SourceItemType, 1)).IsFail())
					break;

				SPlacementInfo placement = null;
				res = FindRecommendedPlacementInBin(alg, i32BinBuffer, ref placement);
				if(res.IsFail())
				{
					if(res == new CResult(EResult.DoesNotExist) || res == new CResult(EResult.FullOfCapacity))
					{
						Console.WriteLine("[source] arrival {0,2}: Buffer cannot accept item type {1}.", i32ArrivalIndex, i32SourceItemType);
						res = new CResult(EResult.OK);
					}

					break;
				}

				SItemInstance item = MakeItemInstance(listItemSpecs, placement);
				if(WouldCoverBufferItem(arrBins[i32BinBuffer], item))
				{
					Console.WriteLine("[source] arrival {0,2}: item type {1} can be placed in Buffer, but would cover a buffered item.", i32ArrivalIndex, i32SourceItemType);
					res = new CResult(EResult.OK);
					break;
				}

				if((res = alg.AddPlacement(placement)).IsFail())
					break;

				if(fnAnimateMove != null)
				{
					SItemSpec<float> itemSpec = listItemSpecs[placement.i32ItemIndex];
					SAnimationPose poseStart = MakePoseFromBinLocalAabbMin(itemSpec, i32BinBuffer, GetSourcePreviewLocalPos(itemSpec, binSpecBuffer), sourceSlot.quatLocalRotation);
					SAnimationPose poseEnd = MakePoseFromBinLocalAabbMin(itemSpec, placement.i32BinIndex, placement.tpPosition, GetAxisRotationLocalQuaternion(placement.eRotation));
					sourceSlot.eState = ESourceState.NeedNewSource;
					fnAnimateMove(placement.i32ItemIndex, poseStart, poseEnd);
				}
				else
				{
					sourceSlot.eState = ESourceState.NeedNewSource;
				}

				arrBins[i32BinBuffer].AddInstance(item);

				Console.WriteLine("[source] arrival {0,2}: Source item type {1} rotation {2} -> Buffer [{3:F1}, {4:F1}, {5:F1}]  (Destination:{6}, Buffer:{7})",
					i32ArrivalIndex, placement.i32ItemIndex, (int)placement.eRotation,
					placement.tpPosition.x, placement.tpPosition.y, placement.tpPosition.z,
					arrBins[i32BinDestination].listItems.Count, arrBins[i32BinBuffer].listItems.Count);

				if(fnOnStep != null)
					fnOnStep();

				bPlaced = true;
				res = new CResult(EResult.OK);
			}
			while(false);

			return res;
		}

		static CResult MoveOnePendingItemToDestination(CSpacePlanningDynamicSP alg, SBinState[] arrBins, List<SItemSpec<float>> listItemSpecs, SBinSpec<float> binSpecBuffer, SSourceSlot sourceSlot, Action fnOnStep, FnAnimateMove fnAnimateMove)
		{
			CResult res = new CResult(EResult.UnknownError);
			int i32SourceItemType = sourceSlot.i32ItemType;
			int i32ArrivalIndex = sourceSlot.i32ArrivalIndex;

			do
			{
				if((res = RebuildInteractiveState(alg, arrBins, -1, -1)).IsFail())
					break;

				bool bFailed = false;
				for(int i = 0; i < arrBins[i32BinBuffer].listItems.Count; ++i)
				{
					if(arrBins[i32BinBuffer].listCountAbove[i] != 0)
						continue;

					if((res = alg.PushItem(arrBins[i32BinBuffer].listItems[i].i32ItemSpecIndex, 1)).IsFail())
					{
						bFailed = true;
						break;
					}
				}

				if(bFailed)
					break;

				if((res = alg.PushItem(i32SourceItemType, 1)).IsFail())
					break;

				SPlacementInfo placement = null;
				res = FindRecommendedPlacementInBin(alg, i32BinDestination, ref placement);
				if(res.IsFail())
				{
					if(res == new CResult(EResult.DoesNotExist) || res == new CResult(EResult.FullOfCapacity))
						res = new CResult(EResult.FullOfCapacity);
					break;
				}

				if((res = alg.AddPlacement(placement)).IsFail())
					break;

				int i32BufferPickIndex = -1;
				bool bUseBufferedItem = arrBins[i32BinBuffer].GetFirstPickableIndexOfType(placement.i32ItemIndex, ref i32BufferPickIndex).IsOK();

				int i32StartBinIndex = i32BinBuffer;
				TPoint3<float> tpStartMinBinLocal;
				CFLGeometry3DQuaternion<float> quatStartLocal;

				if(bUseBufferedItem)
				{
					SItemInstance itemStart = arrBins[i32BinBuffer].listItems[i32BufferPickIndex];
					tpStartMinBinLocal = itemStart.tpMin;
					quatStartLocal = GetAxisRotationLocalQuaternion(itemStart.eRotation);

					if((res = arrBins[i32BinBuffer].RemovePickableAt(i32BufferPickIndex)).IsFail())
						break;
				}
				else
				{
					if(placement.i32ItemIndex != i32SourceItemType)
					{
						res = new CResult(EResult.DoesNotExist);
						break;
					}

					SItemSpec<float> itemSpec = listItemSpecs[placement.i32ItemIndex];
					tpStartMinBinLocal = GetSourcePreviewLocalPos(itemSpec, binSpecBuffer);
					quatStartLocal = sourceSlot.quatLocalRotation;
					sourceSlot.eState = ESourceState.NeedNewSource;
				}

				if(fnAnimateMove != null)
				{
					SItemSpec<float> itemSpec = listItemSpecs[placement.i32ItemIndex];
					SAnimationPose poseStart = MakePoseFromBinLocalAabbMin(itemSpec, i32StartBinIndex, tpStartMinBinLocal, quatStartLocal);
					SAnimationPose poseEnd = MakePoseFromBinLocalAabbMin(itemSpec, placement.i32BinIndex, placement.tpPosition, GetAxisRotationLocalQuaternion(placement.eRotation));
					fnAnimateMove(placement.i32ItemIndex, poseStart, poseEnd);
				}

				arrBins[i32BinDestination].AddInstance(MakeItemInstance(listItemSpecs, placement));

				if((res = RebuildInteractiveState(alg, arrBins, -1, -1)).IsFail())
					break;

				Console.WriteLine("[destination] arrival {0,2}: {1} item type {2} rotation {3} -> Destination [{4:F1}, {5:F1}, {6:F1}]  (Destination:{7}, Buffer:{8})",
					i32ArrivalIndex, bUseBufferedItem ? "Buffered" : "Source",
					placement.i32ItemIndex, (int)placement.eRotation,
					placement.tpPosition.x, placement.tpPosition.y, placement.tpPosition.z,
					arrBins[i32BinDestination].listItems.Count, arrBins[i32BinBuffer].listItems.Count);

				if(fnOnStep != null)
					fnOnStep();

				res = new CResult(EResult.OK);
			}
			while(false);

			return res;
		}

		static CResult ProcessSourceArrival(CSpacePlanningDynamicSP alg, SBinState[] arrBins, List<SItemSpec<float>> listItemSpecs, SBinSpec<float> binSpecBuffer, SSourceSlot sourceSlot, Action fnOnStep, FnAnimateMove fnAnimateMove)
		{
			CResult res = new CResult(EResult.UnknownError);

			int i32MaxAttemptCount = arrBins[i32BinBuffer].listItems.Count + 2;
			for(int i32Attempt = 0; i32Attempt < i32MaxAttemptCount; ++i32Attempt)
			{
				bool bPlacedInBuffer = false;
				if((res = TryPlaceSourceInBuffer(alg, arrBins, listItemSpecs, binSpecBuffer, sourceSlot, ref bPlacedInBuffer, fnOnStep, fnAnimateMove)).IsFail())
					return res;

				if(bPlacedInBuffer)
					return new CResult(EResult.OK);

				if((res = MoveOnePendingItemToDestination(alg, arrBins, listItemSpecs, binSpecBuffer, sourceSlot, fnOnStep, fnAnimateMove)).IsFail())
					return res;

				if(sourceSlot.eState == ESourceState.NeedNewSource)
					return new CResult(EResult.OK);
			}

			return new CResult(EResult.FullOfCapacity);
		}

		static int DrawSourceItemType(CXorshiroRandomGenerator rng, List<float> itemChances, int i32ItemSpecCount)
		{
			if(i32ItemSpecCount <= 0)
				return -1;

			if(itemChances.Count == i32ItemSpecCount)
			{
				float f32TotalChance = 0f;
				for(int i = 0; i < itemChances.Count; ++i)
					f32TotalChance += itemChances[i] > 0f ? itemChances[i] : 0f;

				if(f32TotalChance > 0f)
				{
					float f32Pick = rng.GenerateUniformRandomValueF32(0f, f32TotalChance);
					float f32Accumulated = 0f;
					for(int i = 0; i < itemChances.Count; ++i)
					{
						f32Accumulated += itemChances[i] > 0f ? itemChances[i] : 0f;
						if(f32Pick <= f32Accumulated)
							return i;
					}

					return itemChances.Count - 1;
				}
			}

			return rng.GenerateUniformRandomValueI32(0, i32ItemSpecCount - 1);
		}

		[STAThread]
		static void Main(string[] args)
		{
			CLibraryUtilities.Initialize();

			CGUIView3D view3DResult = new CGUIView3D();
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				CSpacePlanningDynamicSP alg = new CSpacePlanningDynamicSP();

				List<float> itemChances = new List<float>();
				if((res = LearnDefaultModel(alg, itemChances)).IsFail())
				{
					ErrorPrint(res, "Failed to learn the default model.\n");
					break;
				}

				SRuntimeModelSpecs modelSpecs = new SRuntimeModelSpecs();
				if((res = LoadRuntimeSpecsFromModel(alg, modelSpecs)).IsFail())
				{
					ErrorPrint(res, "Failed to load runtime specs from the model.\n");
					break;
				}

				if((res = ValidateSameItemSpecs(alg, modelSpecs.listItemSpecs)).IsFail())
				{
					ErrorPrint(res, "Loaded item specs do not match the runtime specs.\n");
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

				if((res = view3DResult.Create(600, 0, 1300, 650)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				view3DResult.SetRenderingTransparencyMode(ERenderingTransparencyMode.DepthPeelingOIT);
				view3DResult.SetRenderingResolutionScale(2);
				view3DResult.GetLayer(0).DrawTextCanvas(new CFLPoint<double>(0, 0), "Dynamic SP - Source / Buffer / Destination", EColor.YELLOW, EColor.BLACK, 20);

				SBinState[] arrBins = new SBinState[i32BinCount];
				for(int i = 0; i < i32BinCount; ++i)
					arrBins[i] = new SBinState();

				bool bZoomFitted = false;
				SSourceSlot sourceSlot = new SSourceSlot();
				sourceSlot.eState = ESourceState.NeedNewSource;
				sourceSlot.i32ItemType = -1;
				sourceSlot.i32ArrivalIndex = 0;

				Action fnDraw = delegate()
				{
					if(!view3DResult.IsAvailable())
						return;

					view3DResult.Lock();
					view3DResult.ClearObjects();

					PushBinToView(view3DResult, converter, flogBins, arrBins[i32BinDestination], i32BinDestination);
					PushBinToView(view3DResult, converter, flogBins, arrBins[i32BinBuffer], i32BinBuffer);

					if(sourceSlot.eState == ESourceState.HasSource)
						PushSourcePreviewToView(view3DResult, converter, modelSpecs.listItemSpecs[sourceSlot.i32ItemType], sourceSlot.i32ItemType, modelSpecs.arrBinSpecs[i32BinBuffer], sourceSlot.quatLocalRotation);

					CGUIView3DLayer layer3DStatus = view3DResult.GetLayer(1);
					layer3DStatus.Clear();

					float f32TotalDestination = 0f, f32UsedDestination = 0f;
					float f32TotalBuffer = 0f, f32UsedBuffer = 0f;
					alg.GetCurrentVolumeUsage(i32BinDestination, ref f32TotalDestination, ref f32UsedDestination);
					alg.GetCurrentVolumeUsage(i32BinBuffer, ref f32TotalBuffer, ref f32UsedBuffer);

					string strStatus;
					if(sourceSlot.eState == ESourceState.HasSource)
					{
						strStatus = string.Format("Destination: {0} items, {1:F1} / {2:F1}  |  Buffer: {3} items, {4:F1} / {5:F1}  |  Source {6}: item type {7}",
							arrBins[i32BinDestination].listItems.Count, f32UsedDestination, f32TotalDestination,
							arrBins[i32BinBuffer].listItems.Count, f32UsedBuffer, f32TotalBuffer,
							sourceSlot.i32ArrivalIndex, sourceSlot.i32ItemType);
					}
					else
					{
						strStatus = string.Format("Destination: {0} items, {1:F1} / {2:F1}  |  Buffer: {3} items, {4:F1} / {5:F1}",
							arrBins[i32BinDestination].listItems.Count, f32UsedDestination, f32TotalDestination,
							arrBins[i32BinBuffer].listItems.Count, f32UsedBuffer, f32TotalBuffer);
					}
					layer3DStatus.DrawTextCanvas(new CFLPoint<double>(0, 25), strStatus, EColor.YELLOW, EColor.BLACK, 16);

					if(!bZoomFitted && (sourceSlot.eState == ESourceState.HasSource || arrBins[i32BinDestination].listItems.Count > 0 || arrBins[i32BinBuffer].listItems.Count > 0))
					{
						view3DResult.SetCamera(GetWorldCamera());
						bZoomFitted = true;
					}

					view3DResult.Unlock();
					view3DResult.Invalidate(true);
				};

				Action fnRender = delegate()
				{
					fnDraw();
					if(view3DResult.IsAvailable())
						CThreadUtilities.Sleep(500);
				};

				FnAnimateMove fnAnimateMove = delegate(int i32ItemType, SAnimationPose poseStart, SAnimationPose poseEnd)
				{
					if(!view3DResult.IsAvailable())
						return;

					fnDraw();

					int i32InFlightObjIndex = -1;
					List<TPoint3<float>> listLocalVertices = new List<TPoint3<float>>();
					view3DResult.Lock();
					CResult resPush = PushInFlightItemToView(view3DResult, converter, modelSpecs.listItemSpecs[i32ItemType], i32ItemType, poseStart, ref i32InFlightObjIndex, listLocalVertices);
					view3DResult.Unlock();
					if(resPush.IsFail())
						return;

					view3DResult.Invalidate(true);

					Stopwatch swAnimation = Stopwatch.StartNew();
					while(view3DResult.IsAvailable())
					{
						float f32T = Clamp01(swAnimation.Elapsed.TotalMilliseconds / f64AnimationDurationMs);
						SAnimationPose poseNext = LerpArc(poseStart, poseEnd, f32T, f32AnimationArcHeight);

						view3DResult.LockUpdate();
						CResult resUpdate = UpdateItemObjectPose(view3DResult, i32InFlightObjIndex, listLocalVertices, poseNext);
						view3DResult.UnlockUpdate();

						if(resUpdate.IsFail())
							break;

						view3DResult.UpdateObject(i32InFlightObjIndex);

						if(f32T >= 1f)
							break;

						CThreadUtilities.Sleep(i32AnimationSleepMs);
					}
				};

				if((res = RebuildInteractiveState(alg, arrBins, -1, -1)).IsFail())
				{
					ErrorPrint(res, "Failed to initialize the interactive state.\n");
					break;
				}

				fnRender();

				CXorshiroRandomGenerator rngSourceItemType = new CXorshiroRandomGenerator();
				rngSourceItemType.Seed(u64SourceItemTypeRandomSeed);
				CXorshiroRandomGenerator rngSourceRotation = new CXorshiroRandomGenerator();
				rngSourceRotation.Seed(u64SourceRotationRandomSeed);
				int i32NextArrivalIndex = 1;
				while(view3DResult.IsAvailable())
				{
					if(sourceSlot.eState == ESourceState.NeedNewSource)
					{
						sourceSlot.eState = ESourceState.HasSource;
						sourceSlot.i32ItemType = DrawSourceItemType(rngSourceItemType, itemChances, modelSpecs.listItemSpecs.Count);
						sourceSlot.i32ArrivalIndex = i32NextArrivalIndex;
						sourceSlot.quatLocalRotation = DrawSourcePreviewLocalRotation(rngSourceRotation);
						++i32NextArrivalIndex;

						fnRender();

						Console.WriteLine("[source] arrival {0,2}: item type {1}", sourceSlot.i32ArrivalIndex, sourceSlot.i32ItemType);
					}

					if((res = ProcessSourceArrival(alg, arrBins, modelSpecs.listItemSpecs, modelSpecs.arrBinSpecs[i32BinBuffer], sourceSlot, fnRender, fnAnimateMove)).IsFail())
					{
						if(res == new CResult(EResult.FullOfCapacity))
							Console.WriteLine("Arrival {0}: Destination and Buffer cannot accept the source item. Stopping.", sourceSlot.i32ArrivalIndex);
						else if(view3DResult.IsAvailable())
							ErrorPrint(res, "Failed to process the source item.\n");

						break;
					}
				}

				Console.WriteLine("Dynamic intermediate buffer packing complete. Destination:{0}, Buffer:{1}",
					arrBins[i32BinDestination].listItems.Count,
					arrBins[i32BinBuffer].listItems.Count);

				while(view3DResult.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;
using FLImagingCLR.ThreeDim;

namespace PointCloudBasedHandEyeCalibrator3D
{
	class PointCloudBasedHandEyeCalibrator3D
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
			Dictionary<EEulerSequence, String> dictEulerString = new Dictionary<EEulerSequence, String>();

			dictEulerString[EEulerSequence.Extrinsic_XYZ] = "Ext_XYZ";
			dictEulerString[EEulerSequence.Extrinsic_XZY] = "Ext_XZY";
			dictEulerString[EEulerSequence.Extrinsic_YZX] = "Ext_YZX";
			dictEulerString[EEulerSequence.Extrinsic_YXZ] = "Ext_YXZ";
			dictEulerString[EEulerSequence.Extrinsic_ZXY] = "Ext_ZXY";
			dictEulerString[EEulerSequence.Extrinsic_ZYX] = "Ext_ZYX";
			dictEulerString[EEulerSequence.Extrinsic_XYX] = "Ext_XYX";
			dictEulerString[EEulerSequence.Extrinsic_XZX] = "Ext_XZX";
			dictEulerString[EEulerSequence.Extrinsic_YZY] = "Ext_YZY";
			dictEulerString[EEulerSequence.Extrinsic_YXY] = "Ext_YXY";
			dictEulerString[EEulerSequence.Extrinsic_ZYZ] = "Ext_ZYZ";
			dictEulerString[EEulerSequence.Extrinsic_ZXZ] = "Ext_ZXZ";

			dictEulerString[EEulerSequence.Intrinsic_XYZ] = "Int_XYZ";
			dictEulerString[EEulerSequence.Intrinsic_XZY] = "Int_XZY";
			dictEulerString[EEulerSequence.Intrinsic_YZX] = "Int_YZX";
			dictEulerString[EEulerSequence.Intrinsic_YXZ] = "Int_YXZ";
			dictEulerString[EEulerSequence.Intrinsic_ZXY] = "Int_ZXY";
			dictEulerString[EEulerSequence.Intrinsic_ZYX] = "Int_ZYX";
			dictEulerString[EEulerSequence.Intrinsic_XYX] = "Int_XYX";
			dictEulerString[EEulerSequence.Intrinsic_XZX] = "Int_XZX";
			dictEulerString[EEulerSequence.Intrinsic_YZY] = "Int_YZY";
			dictEulerString[EEulerSequence.Intrinsic_YXY] = "Int_YXY";
			dictEulerString[EEulerSequence.Intrinsic_ZYZ] = "Int_ZYZ";
			dictEulerString[EEulerSequence.Intrinsic_ZXZ] = "Int_ZXZ";

			CFL3DObject floLearn = new CFL3DObject();
			CGUIView3D view3D = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				CSurfaceMatch3D match = new CSurfaceMatch3D();

				// 오일러 각 순서 설정 // Set the euler sequence
				match.SetEulerSequence(EEulerSequence.Extrinsic_XYZ);

				// Learn Object 로드 // Load learn object
				if((eResult = floLearn.Load("../../ExampleImages/PointCloudBasedHandEyeCalibrator3D/Learn.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object.\n");
					break;
				}

				match.SetLearnObject(ref floLearn);

				// 피벗 설정 // Set the pivot point.
				CFLPoint3<double> flpPivot = new CFLPoint3<double>(-7.880958, -43.990047, 546.119202);
				match.SetLearnPivot(flpPivot);

				if((eResult = match.Learn()).IsFail())
				{
					ErrorPrint(eResult, "Failed to learn.\n");
					break;
				}

				// PointCloudBasedHandEyeCalibrator3D 객체 생성 // Create PointCloudBasedHandEyeCalibrator3D object
				CPointCloudBasedHandEyeCalibrator3D PointCloudBasedHandEyeCalibrator3D = new CPointCloudBasedHandEyeCalibrator3D();

				// 엔드 이펙터 포즈 로드 // Load the end effector pose
				if((eResult = PointCloudBasedHandEyeCalibrator3D.LoadEndEffectorPose("../../ExampleImages/PointCloudBasedHandEyeCalibrator3D/EndEffectorPose.csv")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the file.\n");
					break;
				}

				PointCloudBasedHandEyeCalibrator3D.Set3DMatchModel(ref match);

				// Source object 로드 // load the source object			
				int i32SourceCount = 9;

				for(int i = 0; i < i32SourceCount; ++i)
				{
					CFL3DObject floSource = new CFL3DObject();

					string flsFileName = string.Format("s{0}.ply", i + 1);

					if((eResult = floSource.Load("../../ExampleImages/PointCloudBasedHandEyeCalibrator3D/" + flsFileName)).IsFail())
					{
						ErrorPrint(eResult, "Failed to load the object.\n");
						break;
					}

					PointCloudBasedHandEyeCalibrator3D.AddSourceObject(ref floSource);
				}

				// 캘리브레이션 모드 설정 // Set the calibration mode
				PointCloudBasedHandEyeCalibrator3D.SetCalibrationMode(CHandEyeCalibrator3D.ECalibrationMode.EyeInHand);

				// 최적화 방법 설정 // Set the optimization method
				PointCloudBasedHandEyeCalibrator3D.SetOptimizationMethod(EOptimizationMethod.Nonlinear);

				// 회전 타입 설정 // Set the rotation type
				PointCloudBasedHandEyeCalibrator3D.SetRotationType(ERotationType.EulerAngle);

				// 엔드 이펙터 각 단위 설정 // Set the end effector angle unit
				PointCloudBasedHandEyeCalibrator3D.SetEndEffectorAngleUnit(EAngleUnit.Degree);

				// 오일러 각 순서 설정 // Set the euler sequence
				PointCloudBasedHandEyeCalibrator3D.SetEulerSequence(EEulerSequence.Extrinsic_XYZ);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = PointCloudBasedHandEyeCalibrator3D.Calibrate()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Point Cloud Based Hand Eye Calibrator 3D.");
					break;
				}

				// 결과 3D 뷰 생성 // Create result 3D view
				if((eResult = view3D.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Result 3D view.\n");
					break;
				}

				if(view3D.IsAvailable())
				{
					CGUIView3DLayer view3DLayer = view3D.GetLayer(0);

					CMatrix<double> matResultRotationVector = new CMatrix<double>();
					TPoint3<double> tp3ResultTranslationVector = new TPoint3<double>();
					List<double> listResultEulerAngle = new List<double>();
					double f64RotationError = 0;
					double f64TranslationError = 0;
					// 캘리브레이션 결과 얻어오기 // Get the calibration result
					PointCloudBasedHandEyeCalibrator3D.GetResultHandToEyeRotationVector(ref matResultRotationVector);
					PointCloudBasedHandEyeCalibrator3D.GetResultHandToEyeTranslationVector(ref tp3ResultTranslationVector);
					PointCloudBasedHandEyeCalibrator3D.GetResultHandToEyeEulerAngle(ref listResultEulerAngle);
					PointCloudBasedHandEyeCalibrator3D.GetResultRotationError(ref f64RotationError);
					PointCloudBasedHandEyeCalibrator3D.GetResultTranslationError(ref f64TranslationError);

					// 3D View의 canvas rect 영역 얻어오기 // Get the canvas rect region
					CFLRect<int> flrCanvasRegion = view3D.GetClientRectCanvasRegion();

					CFLPoint<double> flpImageSize = new CFLPoint<double>(flrCanvasRegion.GetWidth(), flrCanvasRegion.GetHeight());

					String strTranslate = String.Format("Translation Vector\n{0,11:0.000000}\n{1,11:0.000000}\n{2,11:0.000000}", tp3ResultTranslationVector.x, tp3ResultTranslationVector.y, tp3ResultTranslationVector.z);
					String strEuler = String.Format("Euler Angle\n{0,11:0.000000}\n{1,11:0.000000}\n{2,11:0.000000}", listResultEulerAngle[0], listResultEulerAngle[1], listResultEulerAngle[2]);
					String strRotationVector = String.Format("Rotation Vector(End effector to camera)\n{0,11:0.000000}\n{1,11:0.000000}\n{2,11:0.000000}", matResultRotationVector.GetValue(0, 0), matResultRotationVector.GetValue(1, 0), matResultRotationVector.GetValue(2, 0));
					String strError = String.Format("Rotation Error\n{0,11:0.000000}\nTranslation Error\n{1,11:0.000000}", f64RotationError, f64TranslationError);

					view3DLayer.DrawTextCanvas(new CFLPoint<double>(0, 0), strRotationVector, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_TOP);
					view3DLayer.DrawTextCanvas(new CFLPoint<double>(0, flpImageSize.y), strEuler, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM);
					view3DLayer.DrawTextCanvas(new CFLPoint<double>(flpImageSize.x, flpImageSize.y), strTranslate, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.RIGHT_BOTTOM);
					view3DLayer.DrawTextCanvas(new CFLPoint<double>(flpImageSize.x, 0), strError, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.RIGHT_TOP);

					CFL3DObject fl3DOCalibrationBoard = new CFL3DObject();
					TPoint3<double> tp3BoardCenter = new TPoint3<double>();

					PointCloudBasedHandEyeCalibrator3D.GetResultCalibration3DObject(ref fl3DOCalibrationBoard, ref tp3BoardCenter);
					String strIdx = "";

					strIdx = String.Format("Calibration Board");
					view3DLayer.DrawText3D(tp3BoardCenter, strIdx, EColor.RED, 0, 9);
					view3D.PushObject(fl3DOCalibrationBoard);

					for(int i = 0; i < PointCloudBasedHandEyeCalibrator3D.GetSourceObjectCount(); i++)
					{
						TPoint3<double> tp3RobotCenter = new TPoint3<double>(), tp3CamCenter = new TPoint3<double>();
						CFL3DObject fl3DORobot = new CFL3DObject(), fl3DCam = new CFL3DObject();
						TPoint3<float> tp3Cam = new TPoint3<float>(), tp3Board = new TPoint3<float>();

						// 결과 3D 객체 얻어오기 // Get the result 3D object

						if(PointCloudBasedHandEyeCalibrator3D.GetResultCamera3DObject(i, ref fl3DCam, ref tp3CamCenter).IsOK())
						{
							// 카메라 포즈 추정에 실패할 경우 NOK 출력 // NOK output if camera pose estimation fails
							if((PointCloudBasedHandEyeCalibrator3D.GetResultReprojectionPoint(i, ref tp3Cam, ref tp3Board)).IsFail())
							{
								strIdx = String.Format("Cam {0} (NOK)", i);
								view3DLayer.DrawText3D(tp3CamCenter, strIdx, EColor.CYAN, 0, 9);
							}
							else
							{
								strIdx = String.Format("Cam {0}", i);
								view3DLayer.DrawText3D(tp3CamCenter, strIdx, EColor.YELLOW, 0, 9);
								view3D.PushObject(fl3DCam);
							}

							view3D.PushObject(new CGUIView3DObjectLine(tp3Cam, tp3Board, EColor.CYAN));
						}

						if(PointCloudBasedHandEyeCalibrator3D.GetEndEffector3DObject(i, ref fl3DORobot, ref tp3RobotCenter).IsOK())
						{
							strIdx = String.Format("End Effector {0}", i);
							view3DLayer.DrawText3D(tp3RobotCenter, strIdx, EColor.BLUE, 0, 9);
							view3D.PushObject(fl3DORobot);
						}
					}

					view3D.Invalidate();
					view3D.ZoomFit();

					// 이미지 뷰가 종료될 때 까지 기다림
					while(view3D.IsAvailable())
						CThreadUtilities.Sleep(1);
				}
			}
			while(false);
		}
	}
}

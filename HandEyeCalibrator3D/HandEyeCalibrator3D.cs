﻿using System;
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

namespace FLImagingExamplesCSharp
{
	class HandEyeCalibrator3D
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

			CFLImage fliSource = new CFLImage();
			CGUIViewImage viewImage = new CGUIViewImage();
			CGUIView3D view3D = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load the image
				if((eResult = fliSource.Load("../../ExampleImages/HandEyeCalibrator3D/ChessBoard.flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// HandEyeCalibrator3D 객체 생성 // Create HandEyeCalibrator3D object
				CHandEyeCalibrator3D handEyeCalibrator3D = new CHandEyeCalibrator3D();

				// 엔드 이펙터 포즈 로드 // Load the end effector pose
				if((eResult = handEyeCalibrator3D.LoadEndEffectorPose("../../ExampleImages/HandEyeCalibrator3D/EndEffectorPose.csv")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the file.\n");
					break;
				}

				// 처리할 이미지 설정
				handEyeCalibrator3D.SetSourceImage(ref fliSource);

				// Camera Matrix 설정 // Set the camera matrix
				CFLPoint<double> flpFocalLength = new CFLPoint<double>(428.668823242188, 428.268188476563);
				CFLPoint<double> flpPrincipalPoint = new CFLPoint<double>(422.934997558594, 240.188659667969);

				handEyeCalibrator3D.SetCalibrationCameraMatrix(flpFocalLength, flpPrincipalPoint);

				// 셀 간격 설정 // Set the board cell pitch
				handEyeCalibrator3D.SetCalibrationBoardCellPitch(15, 15);

				// 캘리브레이션 객체 타입 설정 // Set the calibration object type
				handEyeCalibrator3D.SetCalibrationObjectType(ECalibrationObjectType.ChessBoard);

				// 최적화 방법 설정 // Set the optimization method
				handEyeCalibrator3D.SetOptimizationMethod(EOptimizationMethod.Nonlinear);

				// 회전 타입 설정 // Set the rotation type
				handEyeCalibrator3D.SetRotationType(ERotationType.RotationVector);

				// 엔드 이펙터 각 단위 설정 // Set the end effector angle unit
				handEyeCalibrator3D.SetEndEffectorAngleUnit(EAngleUnit.Radian);

				// 오일러 각 순서 설정 // Set the euler sequence
				handEyeCalibrator3D.SetEulerSequence(EEulerSequence.Extrinsic_XYZ);

				//왜곡 계수 설정 // Set the distortion coefficient
				List<double> listDistortionCoefficient = new List<double>();

				listDistortionCoefficient.Add(-0.0538526475429535);
				listDistortionCoefficient.Add(0.0590364411473274);
				listDistortionCoefficient.Add(0.000375126546714455);
				listDistortionCoefficient.Add(0.000785713375080377);
				listDistortionCoefficient.Add(-0.0189481563866138);

				handEyeCalibrator3D.SetCalibrationDistortionCoefficient(listDistortionCoefficient);

				int i32PageCount = fliSource.GetPageCount();

				// 이미지 뷰 생성 // Create image view
				if((eResult = viewImage.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Source image view.\n");
					break;
				}

				// 결과 3D 뷰 생성 // Create result 3D view
				if((eResult = view3D.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Result 3D view.\n");
					break;
				}

				// 이미지 포인터 설정 // Set image pointer
				viewImage.SetImagePtr(ref fliSource);

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
				CGUIViewImageLayer layerViewSource = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerViewSource.Clear();

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas 는 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((eResult = layerViewSource.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = handEyeCalibrator3D.Calibrate()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Hand Eye Calibrator 3D.");
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
					handEyeCalibrator3D.GetResultHandToEyeRotationVector(ref matResultRotationVector);
					handEyeCalibrator3D.GetResultHandToEyeTranslationVector(ref tp3ResultTranslationVector);
					handEyeCalibrator3D.GetResultHandToEyeEulerAngle(ref listResultEulerAngle);
					handEyeCalibrator3D.GetResultRotationError(ref f64RotationError);
					handEyeCalibrator3D.GetResultTranslationError(ref f64TranslationError);

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

					handEyeCalibrator3D.GetResultCalibration3DObject(ref fl3DOCalibrationBoard, ref tp3BoardCenter);
					String strIdx = "";

					strIdx = String.Format("Calibration Board");
					view3DLayer.DrawText3D(tp3BoardCenter, strIdx, EColor.RED, 0, 9);
					view3D.PushObject(fl3DOCalibrationBoard);

					for(int i = 0; i < i32PageCount; i++)
					{
						TPoint3<double> tp3RobotCenter = new TPoint3<double>(), tp3CamCenter = new TPoint3<double>();
						CFL3DObject fl3DORobot = new CFL3DObject(), fl3DCam = new CFL3DObject();
						TPoint3<float> tp3Cam = new TPoint3<float>(), tp3Board = new TPoint3<float>();

						// 결과 3D 객체 얻어오기 // Get the result 3D object

						if(handEyeCalibrator3D.GetResultCamera3DObject(i, ref fl3DCam, ref tp3CamCenter).IsOK())
						{
							// 카메라 포즈 추정에 실패할 경우 NOK 출력 // NOK output if camera pose estimation fails
							if((handEyeCalibrator3D.GetResultReprojectionPoint(i, ref tp3Cam, ref tp3Board)).IsFail())
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

						if(handEyeCalibrator3D.GetEndEffector3DObject(i, ref fl3DORobot, ref tp3RobotCenter).IsOK())
						{
							strIdx = String.Format("End Effector {0}", i);
							view3DLayer.DrawText3D(tp3RobotCenter, strIdx, EColor.BLUE, 0, 9);
							view3D.PushObject(fl3DORobot);
						}
					}

					view3D.Invalidate();
					view3D.ZoomFit();
				}

				// 이미지 뷰가 종료될 때 까지 기다림
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

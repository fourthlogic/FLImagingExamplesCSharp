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

namespace VertexMatch3D
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

		[STAThread]
		static void Main(string[] args)
		{
			Dictionary<EEulerSequence, String> dictEulerString = new Dictionary<EEulerSequence, String>();

			dictEulerString[EEulerSequence.Extrinsic_XYZ] = "XYZ";
			dictEulerString[EEulerSequence.Extrinsic_XZY] = "XZY";
			dictEulerString[EEulerSequence.Extrinsic_YZX] = "YZX";
			dictEulerString[EEulerSequence.Extrinsic_YXZ] = "YXZ";
			dictEulerString[EEulerSequence.Extrinsic_ZXY] = "ZXY";
			dictEulerString[EEulerSequence.Extrinsic_ZYX] = "ZYX";
			dictEulerString[EEulerSequence.Extrinsic_XYX] = "XYX";
			dictEulerString[EEulerSequence.Extrinsic_XZX] = "XZX";
			dictEulerString[EEulerSequence.Extrinsic_YZY] = "YZY";
			dictEulerString[EEulerSequence.Extrinsic_YXY] = "YXY";
			dictEulerString[EEulerSequence.Extrinsic_ZYZ] = "ZYZ";
			dictEulerString[EEulerSequence.Extrinsic_ZXZ] = "ZXZ";

			CFLImage fliSource = new CFLImage();
			CGUIViewImage viewImage = new CGUIViewImage();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// 이미지 로드 // Load the image
				if((eResult = fliSource.Load("../../ExampleImages/CameraPose3D/ChessBoard(9).flif")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// CameraPose3D 객체 생성 // Create CameraPose3D object
				CCameraPose3D CameraPose3D = new CCameraPose3D();

				// Camera Matrix 설정 // Set the camera matrix
				CFLPoint<double> flpFocalLength = new CFLPoint<double>(617.8218, 618.2815);
				CFLPoint<double> flpPrincipalPoint = new CFLPoint<double>(319.05237, 243.0472);
				CameraPose3D.SetCameraMatrix(flpPrincipalPoint, flpFocalLength);

				// 셀 간격 설정 // Set the board cell pitch
				CameraPose3D.SetBoardCellPitch(5, 5);

				// 캘리브레이션 객체 타입 설정 // Set the calibration object type
				CameraPose3D.SetCalibrationObjectType(ECalibrationObjectType.ChessBoard);

				// 이미지 전처리 타입 설정 // Set the image preprocessing method
				CameraPose3D.SetPreprocessingMethod(ECalibrationPreprocessingMethod.ShadingCorrection);

				int i32PageCount = fliSource.GetPageCount();

				CFLPoint<double> flpOrigin = new CFLPoint<double>(0, 0);

				for(int i = 0; i < i32PageCount; i++)
				{
					// 이미지 뷰 생성 // Create image view
					if((eResult = viewImage.Create(100, 0, 612, 512)).IsFail())
					{
						ErrorPrint(eResult, "Failed to create the Source 3D view.\n");
						break;
					}

					// 페이지 선택
					fliSource.SelectPage(i);

					// 처리할 이미지 설정
					CameraPose3D.SetSourceImage(ref fliSource);

					// 이미지 포인터 설정 // Set image pointer
					viewImage.SetImagePtr(ref fliSource);

					// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
					if((eResult = CameraPose3D.Execute()).IsFail())
					{
						ErrorPrint(eResult, "Failed to execute Camera Pose 3D.");
						break;
					}

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
					if((eResult = layerViewSource.DrawTextCanvas(flpOrigin, "Source Image", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw text.\n");
						break;
					}

					// 결과 객체 영역 가져오기 // Get the result board region
					CFLQuad<double> flqBoardRegion = new CFLQuad<double>();
					CameraPose3D.GetResultBoardRegion(out flqBoardRegion);

					// 결과 코너점 가져오기 // Get the result corner points
					CFLFigureArray flfaCornerPoints = new CFLFigureArray();
					CameraPose3D.GetResultCornerPoints(out flfaCornerPoints);

					// 결과 객체 영역 그리기 // Draw the result board region
					layerViewSource.DrawFigureImage(flqBoardRegion, EColor.BLUE, 3);

					// 결과 코너점 그리기 // Draw the result corner points
					flfaCornerPoints.Flatten();

					for(long k = 0; k < flfaCornerPoints.GetCount(); ++k)
						layerViewSource.DrawFigureImage(flfaCornerPoints.GetAt(k).GetCenter().MakeCrossHair(5, true), EColor.ORANGE, 1);

					// 오일러 각 순서 설정 // Set the euler sequence
					EEulerSequence eEulerSequence = EEulerSequence.Extrinsic_XYZ;

					// 결과 가져오기 // Get the results
					List<double> listResultRotationVector = new List<double>(), listResultTranslationVector = new List<double>(), listResultEulerAngle = new List<double>();
					CMatrix<double> matResultRotationMatrix = new CMatrix<double>();

					CameraPose3D.GetResultRotationVector(out listResultRotationVector);
					CameraPose3D.GetResultRotationMatrix(out matResultRotationMatrix);
					CameraPose3D.GetResultTranslationVector(out listResultTranslationVector);
					CameraPose3D.GetResultEulerAngle(eEulerSequence, out listResultEulerAngle);

					CFLPoint<double> flpImageSize = new CFLPoint<double>(fliSource);
					flpImageSize.x *= 2;
					flpImageSize.y *= 2;

					String strTranslate = String.Format("Translation Vector\n[{0,11:0.000000}]\n[{1,11:0.000000}]\n[{2,11:0.000000}]", listResultTranslationVector[0], listResultTranslationVector[1], listResultTranslationVector[2]);
					String strEuler = String.Format("Euler Angle({0})\n[{1,11:0.000000}]\n[{2,11:0.000000}]\n[{3,11:0.000000}]", dictEulerString[eEulerSequence], listResultEulerAngle[0], listResultEulerAngle[1], listResultEulerAngle[2]);
					String strRotationMatrix = String.Format("Rotation Matrix\n[{0,9:0.000000}, {1,9:0.000000}, {2,9:0.000000}]\n[{3,9:0.000000}, {4,9:0.000000}, {5,9:0.000000}]\n[{6,9:0.000000}, {7,9:0.000000}, {8,9:0.000000}]", matResultRotationMatrix.GetValue(0, 0), matResultRotationMatrix.GetValue(0, 1), matResultRotationMatrix.GetValue(0, 2), matResultRotationMatrix.GetValue(1, 0), matResultRotationMatrix.GetValue(1, 1), matResultRotationMatrix.GetValue(1, 2), matResultRotationMatrix.GetValue(2, 0), matResultRotationMatrix.GetValue(2, 1), matResultRotationMatrix.GetValue(2, 2));
					String strRotationVector = String.Format("Rotation Vector\n[{0,11:0.000000}]\n[{1,11:0.000000}]\n[{2,11:0.000000}]", listResultRotationVector[0], listResultRotationVector[1], listResultRotationVector[2]);

					layerViewSource.DrawTextImage(new TPoint<double>(flpImageSize.x, 0), strTranslate, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.RIGHT_TOP, "Courier New");
					layerViewSource.DrawTextImage(new TPoint<double>(0, flpImageSize.y), strEuler, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM, "Courier New");
					layerViewSource.DrawTextImage(new TPoint<double>(0, 0), strRotationVector, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.LEFT_TOP, "Courier New");
					layerViewSource.DrawTextImage(new TPoint<double>(flpImageSize.x, flpImageSize.y), strRotationMatrix, EColor.YELLOW, EColor.BLACK, 15, false, 0, EGUIViewImageTextAlignment.RIGHT_BOTTOM, "Courier New");

					if(viewImage.IsAvailable())
						viewImage.Invalidate();

					// 이미지 뷰가 종료될 때 까지 기다림
					while(viewImage.IsAvailable())
						Thread.Sleep(1);
				}
			}
			while(false);
		}
	}
}
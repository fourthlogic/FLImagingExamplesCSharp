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
using FLImagingCLR.Devices;


namespace FLImagingExamplesCSharp
{
	class LineScanCameraCalibrator
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}
		enum EView
		{
			Calibration,
			Source,
			Destination,
			Count
		};

		[STAThread]
		static void Main(string[] args)
		{

			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare the image object
			CFLImage[] fliImage = new CFLImage[3];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[3];

			for(int i = 0; i < 3; i++)
			{
				fliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			string[] pArrStrViewName = new string[3];

			pArrStrViewName[0] = "Calibration View";
			pArrStrViewName[1] = "Source View";
			pArrStrViewName[2] = "Destination View";

			CResult res = new CResult();

			do
			{
				// Learn 이미지 로드 // Load the Learn image
				if((res = fliImage[(int)EView.Calibration].Load("../../ExampleImages/LineScanCameraCalibrator/Calibration Image.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				fliImage[(int)EView.Source].Assign(fliImage[(int)EView.Calibration]);

				for(int i = 0; i < (int)EView.Count; ++i)
				{
					// Learn 이미지 뷰 생성 // Create the Learn image view
					if((res = arrViewImage[i].Create(300 + 480 * i, 0, 300 + 480 * (i + 1), 360)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					// Learn 이미지 뷰에 이미지를 디스플레이 // Display the image in the Learn image view
					if((res = arrViewImage[i].SetImagePtr(ref fliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}

					// 두 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoints of the two image views.

					if(i != 0)
					{
						if((res = arrViewImage[i].SynchronizePointOfView(ref arrViewImage[0])).IsFail())
						{
							ErrorPrint(res, "Failed to synchronize view\n");
							break;
						}
					}
				}

				if((res = arrViewImage[(int)EView.Calibration].ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// calibration
				// 캘리브레이션 객체 생성 // Create Calibrator object
				CLineScanCameraCalibrator lineScanCameraCalibrator = new CLineScanCameraCalibrator();

				// 캘리브레이션 이미지 설정 // Set calibration image
				lineScanCameraCalibrator.SetCalibrationImage(ref fliImage[(int)EView.Calibration]);
				// 보드의 셀 간격 설정(mm) // Sets the board cell pitch (mm).
				lineScanCameraCalibrator.SetCellPitch(10);
				// 카메라의 픽셀 정밀도 설정(mm) // Sets the camera pixel accuracy (mm).
				lineScanCameraCalibrator.SetPixelAccuracy(0.19);

				// 측정 기준 위치 설정. x축 좌표는 무시되며 y축 좌표기준으로 이미지 범위의 line을 검사
				// Sets the measurement reference position. The x-coordinate is ignored, and the image is inspected along the y-coordinate.
				CFLPoint<double> flp = new CFLPoint<double>(0, 160);
				lineScanCameraCalibrator.SetMeasurementPosition(flp);

				// 캘리브레이션 // Calibration
				if((res = lineScanCameraCalibrator.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Undistortion failed\n");
					break;
				}

				// 캘리브레이션 결과 출력 // Display the calibration result.
				CGUIViewImageLayer layer = arrViewImage[(int)EView.Calibration].GetLayer(0);

				// 셀 간격의 픽셀 크기 // Cell pitch in pixels.
				double f64PixelPerPitch = lineScanCameraCalibrator.GetCellPitch() / lineScanCameraCalibrator.GetPixelAccuracy();

				// 실제 측정되는 범위 // Actual measurement range.
				CFLLine<double> fllMeasurementLine = new CFLLine<double>();

				fllMeasurementLine.flpPoints[0].x = 0;
				fllMeasurementLine.flpPoints[1].x = (double)fliImage[(int)EView.Calibration].GetWidth();

				fllMeasurementLine.flpPoints[0].y = fllMeasurementLine.flpPoints[1].y = flp.y;

				// 측정된 결과 제어점 // Measured control points.
				CFLPointArray flpaCalibratedPoints = new CFLPointArray();
				lineScanCameraCalibrator.GetCalibratedControlPoints(ref flpaCalibratedPoints);

				CFLFigureArray flfaCrosshair = new CFLFigureArray();
				flpaCalibratedPoints.MakeCrossHairElementwise(ref flfaCrosshair, f64PixelPerPitch / 10, true);

				layer.DrawFigureImage(fllMeasurementLine, EColor.RED, 2);
				layer.DrawFigureImage(flfaCrosshair, EColor.CYAN, 2);

				// execute (undistortion)
				// 연산 이미지, 연산 결과 이미지 설정 // Set source image, destination image
				lineScanCameraCalibrator.SetSourceImage(ref fliImage[(int)EView.Source]);
				lineScanCameraCalibrator.SetDestinationImage(ref fliImage[(int)EView.Destination]);

				// 왜곡 보정 동작 // Undistortion
				if((res = lineScanCameraCalibrator.Execute()).IsFail())
				{
					ErrorPrint(res, "Undistortion failed\n");
					break;
				}

				// 뷰 이름 출력 // display view name
				for(int i = 0; i < (int)EView.Count; ++i)
				{
					CGUIViewImageLayer currentLayer = arrViewImage[i].GetLayer(0);

					currentLayer.DrawTextImage(new CFLPoint<double>(), pArrStrViewName[i], EColor.YELLOW, EColor.BLACK, 20);
					arrViewImage[i].Invalidate();
				}

				// The image view is waiting until close.
				bool bAvailable = true;

				while(bAvailable)
				{
					for(int i = 0; i < (int)EView.Count; ++i)
						bAvailable &= arrViewImage[i].IsAvailable();

					CThreadUtilities.Sleep(1);
				}
			}
			while(false);
		}
	}
}

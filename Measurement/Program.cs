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
using CResult = FLImagingCLR.CResult;


namespace OperationAdd
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

		public static bool Calibration(COrthogonalCalibrator orthogonalCalibrator, CFLImage fliBoardImage)
		{
			bool bResult = false;
			CResult res = new CResult();

			do
			{
			// Learn 이미지 설정 // Learn image settings
			if((res = orthogonalCalibrator.SetCalibrationImage(ref fliBoardImage)).IsFail())
			{
				ErrorPrint(res, "Failed to set image");
				break;
				}

			// 직교 보정 계산을 할 Learn 이미지 설정 // Learn image settings for orthogonal correction
			if((res = orthogonalCalibrator.SetOrthogonalCorrectionImage(ref fliBoardImage)).IsFail())
			{
				ErrorPrint(res, "Failed to set image\n");
				break;
			}

			// Calibator할 대상 종류를 설정합니다. // Set the target type for Calibator.
			orthogonalCalibrator.SetGridType(CCameraCalibrator.EGridType.ChessBoard);
			// 결과에 대한 학습률을 설정합니다. // Set the learning rate for the result.
			orthogonalCalibrator.SetOptimalSolutionAccuracy(1e-5);

			// Calibration 실행 // Execute Calibration
			if((res = orthogonalCalibrator.Calibrate()).IsFail())
			{
				ErrorPrint(res, "Calibration failed\n");
				break;
			}

			bResult = true;
			}
			while(false) ;

			return bResult;
		}

		public static bool Undistortion(COrthogonalCalibrator orthogonalCalibrator, CFLImage fliSourceImage, ref CFLImage fliDestinationImage)
		{
			bool bResult = false;
			CResult res = new CResult();

			do
			{
				// Source 이미지 설정 // Set Source image
				orthogonalCalibrator.SetSourceImage(ref fliSourceImage);

				// Destination 이미지 설정 // Set destination image
				orthogonalCalibrator.SetDestinationImage(ref fliDestinationImage);

				// Interpolation 알고리즘 설정 // Set the Interpolation Algorithm
				if((res = orthogonalCalibrator.SetInterpolationMethod(EInterpolationMethod.Bilinear)).IsFail())
				{
					ErrorPrint(res, "Failed to set interpolation method\n");
					break;
				}

				// Undistortion 실행 // Execute Undistortion
				if((res = orthogonalCalibrator.Execute()).IsFail())
				{
					ErrorPrint(res, "Undistortion failed\n");
					break;
				}

				bResult = true;
			}
			while(false);

			return bResult;
		}

		public static bool CalculatePixelAccuracy(COrthogonalCalibrator orthogonalCalibrator, double f64BoardCellPitch, ref double f64PixelAccuracy)
		{
			bool bResult = false;

			do
			{
				COrthogonalCalibrator.CCalibratorGridResult gridResult = new COrthogonalCalibrator.CCalibratorGridResult();

				orthogonalCalibrator.GetResultGridPoints(orthogonalCalibrator.GetSourceImage().GetSelectedPageIndex(), 0, out gridResult);
				
				if(gridResult.flfaGridData.GetCount() > 0)
				{
					CFLFigureArray flfaGrid = (CFLFigureArray)gridResult.flfaGridData.GetAt(0);

					CFLPoint<double> flp1, flp2;
					orthogonalCalibrator.ConvertCoordinate((CFLPoint<double>)flfaGrid.Front(), out flp1);
					orthogonalCalibrator.ConvertCoordinate((CFLPoint<double>)flfaGrid.Back(), out flp2);

					CFLLine<double> fliLine2 = new CFLLine<double>(flp1, flp2);

					CFLImage pFliDst = orthogonalCalibrator.GetDestinationImage();

					pFliDst.PushBackFigure(CFigureUtilities.ConvertFigureObjectToString(fliLine2));
					f64PixelAccuracy = f64BoardCellPitch / (fliLine2.GetLength() / (double)(flfaGrid.GetCount() - 1));
					bResult = true;
				}
				else
					bResult = false;

			}
			while(false);

			return bResult;
		}

		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare image object
			CFLImage fliDistortionChessBoard = new CFLImage();
			CFLImage fliUndistortedChessBoard = new CFLImage();
			CFLImage fliDistortedMeasurementImage = new CFLImage();
			CFLImage fliUndistortedMeasurementImage = new CFLImage();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewImageDistortionChessBoard = new CGUIViewImage();
			CGUIViewImage viewImageUndistortionChessBoard = new CGUIViewImage();
			CGUIViewImage viewImageDistortionMeasurement = new CGUIViewImage();
			CGUIViewImage viewImageUndistortionMeasurement = new CGUIViewImage();

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult res;

			do
			{
				{
					// Image View 생성 // Create image view
					if((res = viewImageDistortionChessBoard.Create(0, 0, 500, 500)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					if((res = viewImageUndistortionChessBoard.Create(500, 0, 1000, 500)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					if((res = viewImageDistortionMeasurement.Create(0, 500, 500, 1000)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					if((res = viewImageUndistortionMeasurement.Create(500, 500, 1000, 1000)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					// 이미지 포인터 설정 // Image pointer settings
					viewImageDistortionChessBoard.SetImagePtr(ref fliDistortionChessBoard);
					viewImageUndistortionChessBoard.SetImagePtr(ref fliUndistortedChessBoard);

					viewImageDistortionMeasurement.SetImagePtr(ref fliDistortedMeasurementImage);
					viewImageUndistortionMeasurement.SetImagePtr(ref fliUndistortedMeasurementImage);
				}

				// Orthogonal Calibrator 클래스 선언 // Declare COrthogonal Calibrator class instance.
				COrthogonalCalibrator orthogonalCalibrator = new COrthogonalCalibrator ();

				// Learn 이미지 로드 // Load the Learn image
				if((res = fliDistortionChessBoard.Load("C:\\Users\\Public\\Documents\\FLImaging\\ExampleImages\\Measurement\\ChessBoard.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if(!Calibration(orthogonalCalibrator, fliDistortionChessBoard))
					break;

				if(!Undistortion(orthogonalCalibrator, fliDistortionChessBoard, ref fliUndistortedChessBoard))
					break;

				// Board cell pitch 설정 // Board cell pitch settings
				const double f64BoardCellPitch = 15;
				double f64PixelAccuracy = 0;

				if(!CalculatePixelAccuracy(orthogonalCalibrator, f64BoardCellPitch, ref f64PixelAccuracy))
					break;

				// 측정 이미지 로드 // Load the measurement image
				if((res = fliDistortedMeasurementImage.Load("C:\\Users\\Public\\Documents\\FLImaging\\ExampleImages\\Measurement\\Measurement.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if(!Undistortion(orthogonalCalibrator, fliDistortedMeasurementImage, ref fliUndistortedMeasurementImage))
					break;

				// Rectangle Gauge 클래스 선언 // Declare Rectangle Gauge class instance.
				CRectangleGauge rectangleGauge = new CRectangleGauge();

				// Source 이미지 설정 // Set Source image			
				rectangleGauge.SetSourceImage(ref fliUndistortedMeasurementImage);

				// 측정할 영역을 설정합니다. // Set the area to measure.
				CFLRect<double> flrMeasureRegion = new CFLRect<double>(1095.69367959050714, 1337.99846331160370, 1970.73350513123319, 1924.77041713468020, -8.06731650598383);
				rectangleGauge.SetMeasurementRegion(flrMeasureRegion, 20.000000);

				// 알고리즘 수행 // Execute the algorithm
				if((res = rectangleGauge.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Rectangle gauge.\n");
					break;
				}

				// 실행 결과를 가져옵니다. // Get the execution result.
				CFLRect<double> flrResult = new CFLRect<double>();
				rectangleGauge.GetMeasuredObject(out flrResult, 0);

				CFLPoint<double> flpLeftTop = new CFLPoint<double>(flrResult.left, flrResult.top);
				CFLPoint<double> flpRightTop = new CFLPoint<double>(flrResult.right, flrResult.top);
				CFLPoint<double> flpLeftBottom = new CFLPoint<double>(flrResult.left, flrResult.bottom);
				CFLPoint<double> flpRightBottom = new CFLPoint<double>(flrResult.right, flrResult.bottom);

				CFLLine<double> fliTop = new CFLLine<double>(flpLeftTop, flpRightTop);
				CFLLine<double> fliRight = new CFLLine<double>(flpRightTop, flpRightBottom);
				CFLLine<double> fliBottom = new CFLLine<double>(flpLeftBottom, flpRightBottom);
				CFLLine<double> fliLeft = new CFLLine<double>(flpLeftTop, flpLeftBottom);

				// 측정된 사각형의 실제 길이를 계산합니다. // Calculate the actual length of the measured rectangle.
				double f64LeftLength = fliLeft.GetLength() * f64PixelAccuracy;
				double f64TopLength = fliTop.GetLength() * f64PixelAccuracy;
				double f64RightLength = fliRight.GetLength() * f64PixelAccuracy;
				double f64BottomLength = fliBottom.GetLength() * f64PixelAccuracy;

				// 이미지 뷰 정보 표시 // Display image view information		
				CGUIViewImageLayer layerDistortionChessBoard = viewImageDistortionChessBoard.GetLayer(0);
				CGUIViewImageLayer layerUndistortionChessBoard = viewImageUndistortionChessBoard.GetLayer(0);
				CGUIViewImageLayer layerDistortionMeasurement = viewImageDistortionMeasurement.GetLayer(0);
				CGUIViewImageLayer layerUndistortionMeasurement = viewImageUndistortionMeasurement.GetLayer(0);
				CFLPoint<double> flpPoint = new CFLPoint<double>(0, 0);
				
				if((res = layerDistortionChessBoard.DrawTextImage(flpPoint, "Distortion ChessBoard", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layerUndistortionChessBoard.DrawTextImage(flpPoint, string.Format("Undistortion ChessBoard \nBoard Cell Pitch : {0,6:0.000000} \nPixel Accuracy : {1,6:0.000000}", f64BoardCellPitch, f64PixelAccuracy), EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layerDistortionMeasurement.DrawTextImage(flpPoint, "Distortion Measurement", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layerUndistortionMeasurement.DrawTextImage(flpPoint, "Undistortion & Measurement Result", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layerUndistortionMeasurement.DrawTextImage(fliLeft.GetCenter(), string.Format("{0,6:0.000000} (mm)", f64LeftLength), EColor.YELLOW, EColor.BLACK, 12)).IsFail() ||
				   (res = layerUndistortionMeasurement.DrawTextImage(fliTop.GetCenter(), string.Format("{0,6:0.000000} (mm)", f64TopLength), EColor.YELLOW, EColor.BLACK, 12)).IsFail() ||
				   (res = layerUndistortionMeasurement.DrawTextImage(fliRight.GetCenter(), string.Format("{0,6:0.000000} (mm)", f64RightLength), EColor.YELLOW, EColor.BLACK, 12)).IsFail() ||
				   (res = layerUndistortionMeasurement.DrawTextImage(fliBottom.GetCenter(), string.Format("{0,6:0.000000} (mm)", f64BottomLength), EColor.YELLOW, EColor.BLACK, 12)).IsFail() ||
				   (res = layerUndistortionMeasurement.DrawFigureImage(flrResult, EColor.CYAN, 5)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다.
				viewImageDistortionChessBoard.Invalidate(true);
				viewImageDistortionMeasurement.Invalidate(true);
				viewImageUndistortionChessBoard.Invalidate(true);
				viewImageUndistortionMeasurement.Invalidate(true);

				viewImageDistortionChessBoard.ZoomFit();
				viewImageDistortionMeasurement.ZoomFit();
				viewImageUndistortionChessBoard.ZoomFit();
				viewImageUndistortionMeasurement.ZoomFit();

				// 이미지 뷰가 꺼지면 종료로 간주
				while(viewImageDistortionChessBoard.IsAvailable() || viewImageUndistortionChessBoard.IsAvailable() || viewImageDistortionMeasurement.IsAvailable() || viewImageUndistortionMeasurement.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

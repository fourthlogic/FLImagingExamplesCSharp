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

namespace FLImagingExamplesCSharp
{
	class StereoCalibrator3D
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		public struct SGridDisplay
		{
			public long i64ImageIdx;
			public CStereoCalibrator3D.SGridResult sGridResult;
		};

		static CResult DrawGridPoints(SGridDisplay sGridDisplay, CGUIViewImageLayer pLayer)
		{
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				if(sGridDisplay.sGridResult.arrGridData.Count == 0)
				{
					res = new CResult(EResult.NoData);
					break;
				}

				// 그리기 색상 설정 // Set drawing color
				EColor[] u32ArrColor = { EColor.RED, EColor.LIME, EColor.CYAN };

				long i64GridRow = sGridDisplay.sGridResult.i64Rows;
				long i64GridCol = sGridDisplay.sGridResult.i64Columns;
				double f64AvgDistance = sGridDisplay.sGridResult.f64AvgDistance;
				CFLQuad<double> flqBoardRegion = sGridDisplay.sGridResult.pFlqBoardRegion;
				double f64Angle = flqBoardRegion.flpPoints[0].GetAngle(flqBoardRegion.flpPoints[1]);
				double f64Width = flqBoardRegion.flpPoints[0].GetDistance(flqBoardRegion.flpPoints[1]);

				// Grid 그리기 // Draw grid
				for(long i64Row = 0; i64Row < i64GridRow; ++i64Row)
				{
					for(long i64Col = 0; i64Col < i64GridCol - 1; ++i64Col)
					{
						long i64GridIdx = i64Row * i64GridCol + i64Col;
						CFLPoint<double> flpGridPoint1 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][(int)i64Col]);
						CFLPoint<double> flpGridPoint2 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][(int)i64Col + 1]);
						CFLLine<double> fllDrawLine = new CFLLine<double>(flpGridPoint1, flpGridPoint2);
						pLayer.DrawFigureImage(fllDrawLine, EColor.BLACK, 5);
						pLayer.DrawFigureImage(fllDrawLine, u32ArrColor[i64GridIdx % 3], 3);
					}

					if(i64Row < i64GridRow - 1)
					{
						CFLPoint<double> flpGridPoint1 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][(int)i64GridCol - 1]);
						CFLPoint<double> flpGridPoint2 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row + 1][0]);
						CFLLine<double> fllDrawLine = new CFLLine<double>(flpGridPoint1, flpGridPoint2);
						pLayer.DrawFigureImage(fllDrawLine, EColor.BLACK, 5);
						pLayer.DrawFigureImage(fllDrawLine, EColor.YELLOW, 3);
					}
				}

				EColor u32ColorText = EColor.YELLOW;
				double f64PointDist = 0;
				double f64Dx = 0;
				double f64Dy = 0;

				// Grid Point 인덱싱 // Index Grid Point
				for(long i64Row = 0; i64Row < i64GridRow; ++i64Row)
				{
					CFLPoint<double> flpGridPoint1 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][0]);
					CFLPoint<double> flpGridPoint2 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][1]);
					double f64TempAngle = flpGridPoint1.GetAngle(flpGridPoint2);

					for(long i64Col = 0; i64Col < i64GridCol; ++i64Col)
					{
						long i64GridIdx = i64Row * i64GridCol + i64Col;

						if(i64Col < i64GridCol - 1)
						{
							flpGridPoint1 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][(int)i64Col]);
							flpGridPoint2 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][(int)i64Col + 1]);

							f64Dx = flpGridPoint2.x - flpGridPoint1.x;
							f64Dy = flpGridPoint2.y - flpGridPoint1.y;
							f64PointDist = Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy);
						}

						if(i64Row != 0)
						{
							flpGridPoint1 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row][(int)i64Col]);
							flpGridPoint2 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[(int)i64Row - 1][(int)i64Col]);

							f64Dx = flpGridPoint2.x - flpGridPoint1.x;
							f64Dy = flpGridPoint2.y - flpGridPoint1.y;
							f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
						}
						else
						{
							flpGridPoint1 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[0][(int)i64Col]);
							flpGridPoint2 = new CFLPoint<double>(sGridDisplay.sGridResult.arrGridData[1][(int)i64Col]);

							f64Dx = flpGridPoint2.x - flpGridPoint1.x;
							f64Dy = flpGridPoint2.y - flpGridPoint1.y;
							f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
						}

						string strGridIdx;
						strGridIdx = String.Format("{0}", i64GridIdx);
						u32ColorText = u32ArrColor[i64GridIdx % 3];

						if(i64Col == i64GridCol - 1)
							u32ColorText = EColor.YELLOW;

						pLayer.DrawTextImage(flpGridPoint1, strGridIdx, u32ColorText, EColor.BLACK, (float)(int)(f64PointDist / 2), true, f64TempAngle);
					}
				}

				// Board Region 그리기 // Draw Board Region
				string stringData = String.Format("({0} X {1})", (int)i64GridCol, (int)i64GridRow);
				pLayer.DrawFigureImage(flqBoardRegion, EColor.BLACK, 3);
				pLayer.DrawFigureImage(flqBoardRegion, EColor.YELLOW, 1);
				pLayer.DrawTextImage(flqBoardRegion.flpPoints[0], stringData, EColor.YELLOW, EColor.BLACK, (float)(int)(f64Width / 16), true, f64Angle, EGUIViewImageTextAlignment.LEFT_BOTTOM, null, 1, 1, EGUIViewImageFontWeight.EXTRABOLD);

				res = new CResult(EResult.OK);
			}
			while(false);

			return res;
		}

		public class CMessageReceiver : CFLBase
		{
			// CMessageReceiver 생성자 // CMessageReceiver constructor
			public CMessageReceiver(ref CGUIViewImage viewImage)
			{
				m_viewImage = viewImage;

				// 메세지를 전달 받기 위해 CBroadcastManager 에 구독 등록 // Subscribe to CBroadcast Manager to receive messages
				CBroadcastManager.Subscribe(this);
			}

			// CMessageReceiver 소멸자 // CMessageReceiver destructor
			~CMessageReceiver()
			{
				// 객체가 소멸할때 메세지 수신을 중단하기 위해 구독을 해제한다. // Unsubscribe to stop receiving messages when object disappears.
				CBroadcastManager.Unsubscribe(this);
			}

			// 메세지가 들어오면 호출되는 함수 OnReceiveBroadcast 오버라이드 하여 구현 // Override OnReceiveBroadcast that is called when a message in called
			public override void OnReceiveBroadcast(CBroadcastMessage message)
			{
				do
				{
					// message 가 null 인지 확인 // Verify message is null
					if(message == null)
						break;

					// 설정된 뷰가 null 인지 확인 // Verify view is null
					if(m_viewImage == null)
						break;

					// 메세지의 채널을 확인 // Check message's channel
					switch(message.GetChannel())
					{
					case (long)EGUIBroadcast.ViewImage_PostPageChange:
						{
							// GetCaller 가 등록한 이미지뷰인지 확인 // Verify that GetCaller is a registered image view
							if(message.GetCaller() != m_viewImage)
								break;

							CFLImage fliLearnImage = m_viewImage.GetImage();

							if(fliLearnImage == null)
								break;

							long i64CurPage = fliLearnImage.GetSelectedPageIndex();

							// 이미지뷰의 0번 레이어 가져오기 // Get layer 0th of image view
							CGUIViewImageLayer layer = m_viewImage.GetLayer((int)(i64CurPage % 10));

							for(long i = 0; i < 10; ++i)
								m_viewImage.GetLayer((int)i).Clear();

							for(long i64Idx = 0; i64Idx < (long)fliLearnImage.GetPageCount(); ++i64Idx)
							{
								if(m_psGridDisplay[(int)i64Idx].i64ImageIdx == i64CurPage)
									DrawGridPoints(m_psGridDisplay[(int)i64Idx], layer);
							}

							// 이미지 뷰를 갱신 // Update image view
							m_viewImage.Invalidate(true);
						}
						break;
					}
				}
				while(false);
			}

			public SGridDisplay[] m_psGridDisplay;

			CGUIViewImage m_viewImage;
		};

		static CResult Calibration(CStereoCalibrator3D stereoCalibrator3D, CFLImage fliLearnImage, CFLImage fliLearn2Image)
		{
			// 수행 결과 객체 선언 // Declare execution result object
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// Learn 이미지 설정 // Set Learn image
				if((res = stereoCalibrator3D.SetLearnImage(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Learn image.\n");
					break;
				}

				// Learn 2 이미지 설정 // Set Learn 2 image
				if((res = stereoCalibrator3D.SetLearnImage2(ref fliLearn2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set Learn 2 image.\n");
					break;
				}

				// Calibration의 최적해 정확도 값 설정 // Set optimal solution accuracy of calibration
				if((res = stereoCalibrator3D.SetOptimalSolutionAccuracy(1e-5)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration optimal solution accuracy.\n");
					break;
				}

				// Calibration에 사용되는 Grid Type 설정 // Set grid type used in calibration
				if((res = stereoCalibrator3D.SetGridType(CStereoCalibrator3D.EGridType.ChessBoard)).IsFail())
				{
					ErrorPrint(res, "Failed to set calibration grid type.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 Calibration 수행 // Calibration algorithm according to previously set parameters
				if((res = stereoCalibrator3D.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Failed to calibrate Stereo Calibrator 3D.\n");
					break;
				}
			}
			while(false);

			return res;
		}

		static CResult Undistortion(CStereoCalibrator3D stereoCalibrator3D, CFLImage fliSourceImage, CFLImage fliSource2Image, CFLImage fliDestinationImage, CFLImage fliDestination2Image)
		{
			// 수행 결과 객체 선언 // Declare execution result object
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// Source 이미지 설정 // Set Source image
				if((res = stereoCalibrator3D.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source image.\n");
					break;
				}

				// Source 이미지 2 설정 // Set Source 2 image
				if((res = stereoCalibrator3D.SetSourceImage2(ref fliSource2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source 2 image.\n");
					break;
				}

				// Destination 이미지 설정 // Set Destination image
				if((res = stereoCalibrator3D.SetDestinationImage(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination image.\n");
					break;
				}

				// Destination 이미지 2 설정 // Set Destination 2 image
				if((res = stereoCalibrator3D.SetDestinationImage2(ref fliDestination2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set Destination 2 image.\n");
					break;
				}

				// Interpolation 메소드 설정 // Set interpolation method
				if((res = stereoCalibrator3D.SetInterpolationMethod(EInterpolationMethod.Bilinear)).IsFail())
				{
					ErrorPrint(res, "Failed to set interpolation method.\n");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = stereoCalibrator3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Stereo Calibrator 3D.\n");
					break;
				}
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

			// 이미지 객체 선언 // Declare image object
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliSourceImage = new CFLImage();
			CFLImage fliDestinationImage = new CFLImage();
			CFLImage fliLearn2Image = new CFLImage();
			CFLImage fliSource2Image = new CFLImage();
			CFLImage fliDestination2Image = new CFLImage();

			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewLearnImage = new CGUIViewImage();
			CGUIViewImage viewDestinationImage = new CGUIViewImage();
			CGUIViewImage viewLearn2Image = new CGUIViewImage();
			CGUIViewImage viewDestination2Image = new CGUIViewImage();

			// 수행 결과 객체 선언 // Declare execution result object
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// Learn 이미지 로드 // Load Learn image
				if((res = fliLearnImage.Load("../../ExampleImages/StereoCalibrator3D/Left.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Learn2 이미지 로드 // Load Learn2 image
				if((res = fliLearn2Image.Load("../../ExampleImages/StereoCalibrator3D/Right.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Learn 이미지 뷰 생성 // Create Learn image view
				if((res = viewLearnImage.Create(300, 0, 300 + 480 * 1, 360)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Learn 2 이미지 뷰 생성 // Create Learn 2 image view
				if((res = viewLearn2Image.Create(300 + 480, 0, 300 + 480 * 2, 360)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create Destination image view
				if((res = viewDestinationImage.Create(300, 360, 780, 720)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 2 이미지 뷰 생성 // Create Destination 2 image view
				if((res = viewDestination2Image.Create(780, 360, 1260, 720)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Learn 이미지 뷰에 이미지를 디스플레이 // Display image in Learn image view
				if((res = viewLearnImage.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Learn 2 이미지 뷰에 이미지를 디스플레이 // Display image in Learn 2 image view
				if((res = viewLearn2Image.SetImagePtr(ref fliLearn2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display image in Destination image view
				if((res = viewDestinationImage.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 2 이미지 뷰에 이미지를 디스플레이 // Display image in Destination 2 image view
				if((res = viewDestination2Image.SetImagePtr(ref fliDestination2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewLearnImage.SynchronizeWindow(ref viewLearn2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewLearnImage.SynchronizeWindow(ref viewDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 뷰 윈도우의 위치를 동기화 // Synchronize positions of two views
				if((res = viewLearnImage.SynchronizeWindow(ref viewDestination2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window between views.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 Page를 동기화 한다 // Synchronize pages of two image views
				if((res = viewLearnImage.SynchronizePageIndex(ref viewLearn2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index between image views.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 Page를 동기화 한다 // Synchronize pages of two image views
				if((res = viewLearnImage.SynchronizePageIndex(ref viewDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index between image views.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 Page를 동기화 한다 // Synchronize pages of two image views
				if((res = viewLearnImage.SynchronizePageIndex(ref viewDestination2Image)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize page index between image views.\n");
					break;
				}

				Console.WriteLine("Processing.....\n");

				// Stereo Calibrator 3D 객체 생성 // Create Stereo Calibrator 3D object
				CStereoCalibrator3D stereoCalibrator3D = new CStereoCalibrator3D();

				// Stereo Calibrator 3D Calibration 수행 // Calibrate Stereo Calibrator 3D
				if(Calibration(stereoCalibrator3D, fliLearnImage, fliLearn2Image).IsFail())
					break;

				// Source 이미지를 Learn 이미지와 동일하도록 설정 (얕은 복사) // Assign Learn image to Source image (Shallow Copy)
				if((res = fliSourceImage.Assign(fliLearnImage, false)).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image.\n");
					break;
				}

				// Source 2 이미지를 Learn 2 이미지와 동일하도록 설정 (얕은 복사) // Assign Learn 2 image to Source 2 image (Shallow Copy)
				if((res = fliSource2Image.Assign(fliLearn2Image, false)).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image.\n");
					break;
				}

				// Stereo Calibrator 3D Undistortion 수행 // Undistort Stereo Calibrator 3D
				if(Undistortion(stereoCalibrator3D, fliSourceImage, fliSource2Image, fliDestinationImage, fliDestination2Image).IsFail())
					break;

				// 뷰에 격자 탐지 결과 출력 // Display grid detection result in view
				SGridDisplay[] sArrGridDisplay = new SGridDisplay[5];

				for(long i64ImgIdx = 0; i64ImgIdx < (long)fliLearnImage.GetPageCount(); ++i64ImgIdx)
				{
					sArrGridDisplay[i64ImgIdx] = new SGridDisplay();
					sArrGridDisplay[i64ImgIdx].sGridResult = new CStereoCalibrator3D.SGridResult();
					stereoCalibrator3D.GetResultGridPoints(ref sArrGridDisplay[i64ImgIdx].sGridResult, i64ImgIdx);
					sArrGridDisplay[i64ImgIdx].i64ImageIdx = i64ImgIdx;
				}

				SGridDisplay[] sArrGridDisplay2 = new SGridDisplay[5];

				for(long i64ImgIdx = 0; i64ImgIdx < (long)fliLearn2Image.GetPageCount(); ++i64ImgIdx)
				{
					sArrGridDisplay2[i64ImgIdx] = new SGridDisplay();
					sArrGridDisplay2[i64ImgIdx].sGridResult = new CStereoCalibrator3D.SGridResult();
					stereoCalibrator3D.GetResultGridPoints2(ref sArrGridDisplay2[i64ImgIdx].sGridResult, i64ImgIdx);
					sArrGridDisplay2[i64ImgIdx].i64ImageIdx = i64ImgIdx;
				}

				// Message Receiver 객체 생성 // Create Message Receiver object
				CMessageReceiver msgReceiver = new CMessageReceiver(ref viewLearnImage);
				CMessageReceiver msgReceiver2 = new CMessageReceiver(ref viewLearn2Image);

				msgReceiver.m_psGridDisplay = sArrGridDisplay;
				msgReceiver2.m_psGridDisplay = sArrGridDisplay2;

				// 화면에 출력하기 위해 이미지 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released
				CGUIViewImageLayer layerLearn = viewLearnImage.GetLayer(0);
				CGUIViewImageLayer layerLearn2 = viewLearn2Image.GetLayer(0);
				CGUIViewImageLayer layerDestination = viewDestinationImage.GetLayer(0);
				CGUIViewImageLayer layerDestination2 = viewDestination2Image.GetLayer(0);

				// Chess Board Grid 출력 // Display chess board grid
				DrawGridPoints(sArrGridDisplay[0], layerLearn);
				DrawGridPoints(sArrGridDisplay2[0], layerLearn2);

				// Calibration data 출력 // Display calibration data
				CStereoCalibrator3D.SIntrinsicParameters sIntrinsicParam = stereoCalibrator3D.GetResultIntrinsicParameters();
				CStereoCalibrator3D.SDistortionCoefficients sDistortCoeef = stereoCalibrator3D.GetResultDistortionCoefficients();

				CStereoCalibrator3D.SIntrinsicParameters sIntrinsicParam2 = stereoCalibrator3D.GetResultIntrinsicParameters2();
				CStereoCalibrator3D.SDistortionCoefficients sDistortCoeef2 = stereoCalibrator3D.GetResultDistortionCoefficients2();

				CStereoCalibrator3D.SRotationParameters sRotationParam = stereoCalibrator3D.GetResultRotationParameters();
				CStereoCalibrator3D.SRotationParameters sRotationParam2 = stereoCalibrator3D.GetResultRotationParameters2();

				CStereoCalibrator3D.STranslationParameters sTranslationParam = stereoCalibrator3D.GetResultTranslationParameters();
				CStereoCalibrator3D.STranslationParameters sTranslationParam2 = stereoCalibrator3D.GetResultTranslationParameters2();

				double f64ReprojError = stereoCalibrator3D.GetResultReProjectionError();

				string strMatrix = "";
				string strDistVal = "";
				string strMatrix2 = "";
				string strDistVal2 = "";
				string strRotatMatrix = "";
				string strTranslVal = "";
				string strRotatMatrix2 = "";
				string strTranslVal2 = "";

				strMatrix += String.Format("{0:N13}, ", sIntrinsicParam.f64FocalLengthX);
				strMatrix += String.Format("{0:N13}, ", sIntrinsicParam.f64Skew);
				strMatrix += String.Format("{0:N13}, ", sIntrinsicParam.f64PrincipalPointX);
				strMatrix += String.Format("{0:N13}, ", 0);
				strMatrix += String.Format("{0:N13}, ", sIntrinsicParam.f64FocalLengthY);
				strMatrix += String.Format("{0:N13}, ", sIntrinsicParam.f64PrincipalPointY);
				strMatrix += String.Format("{0:N13}, ", 0);
				strMatrix += String.Format("{0:N13}, ", 0);
				strMatrix += String.Format("{0:N13}", 1);

				strMatrix2 += String.Format("{0:N13}, ", sIntrinsicParam2.f64FocalLengthX);
				strMatrix2 += String.Format("{0:N13}, ", sIntrinsicParam2.f64Skew);
				strMatrix2 += String.Format("{0:N13}, ", sIntrinsicParam2.f64PrincipalPointX);
				strMatrix2 += String.Format("{0:N13}, ", 0);
				strMatrix2 += String.Format("{0:N13}, ", sIntrinsicParam2.f64FocalLengthY);
				strMatrix2 += String.Format("{0:N13}, ", sIntrinsicParam2.f64PrincipalPointY);
				strMatrix2 += String.Format("{0:N13}, ", 0);
				strMatrix2 += String.Format("{0:N13}, ", 0);
				strMatrix2 += String.Format("{0:N13}", 1);

				strDistVal += String.Format("{0:N13}, ", sDistortCoeef.f64K1);
				strDistVal += String.Format("{0:N13}, ", sDistortCoeef.f64K2);
				strDistVal += String.Format("{0:N13}, ", sDistortCoeef.f64P1);
				strDistVal += String.Format("{0:N13}, ", sDistortCoeef.f64P2);
				strDistVal += String.Format("{0:N13}", sDistortCoeef.f64K3);

				strDistVal2 += String.Format("{0:N13}, ", sDistortCoeef2.f64K1);
				strDistVal2 += String.Format("{0:N13}, ", sDistortCoeef2.f64K2);
				strDistVal2 += String.Format("{0:N13}, ", sDistortCoeef2.f64P1);
				strDistVal2 += String.Format("{0:N13}, ", sDistortCoeef2.f64P2);
				strDistVal2 += String.Format("{0:N13}", sDistortCoeef2.f64K3);

				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R0);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R1);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R2);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R3);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R4);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R5);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R6);
				strRotatMatrix += String.Format("{0:N13}, ", sRotationParam.f64R7);
				strRotatMatrix += String.Format("{0:N13}", sRotationParam.f64R8);

				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R0);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R1);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R2);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R3);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R4);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R5);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R6);
				strRotatMatrix2 += String.Format("{0:N13}, ", sRotationParam2.f64R7);
				strRotatMatrix2 += String.Format("{0:N13}", sRotationParam2.f64R8);

				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T0);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T1);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T2);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T3);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T4);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T5);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T6);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T7);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T8);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T9);
				strTranslVal += String.Format("{0:N8}, ", sTranslationParam.f64T10);
				strTranslVal += String.Format("{0:N8}", sTranslationParam.f64T11);

				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T0);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T1);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T2);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T3);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T4);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T5);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T6);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T7);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T8);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T9);
				strTranslVal2 += String.Format("{0:N8}, ", sTranslationParam2.f64T10);
				strTranslVal2 += String.Format("{0:N8}", sTranslationParam2.f64T11);

				Console.Write("Intrinsic parameters : {0}\n", strMatrix);
				Console.Write("Distortion Coefficients : {0}\n", strDistVal);
				Console.Write("Rotation parameters : {0}\n", strRotatMatrix);
				Console.Write("Translation parameters : {0}\n\n", strTranslVal);
				Console.Write("Intrinsic parameters 2 : {0}\n", strMatrix2);
				Console.Write("Distortion Coefficients 2 : {0}\n", strDistVal2);
				Console.Write("Rotation parameters 2 : {0}\n", strRotatMatrix2);
				Console.Write("Translation parameters 2 : {0}\n\n", strTranslVal2);
				Console.Write("Re-Projection Error : {0:8}", f64ReprojError);

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerLearn.DrawTextCanvas(new CFLPoint<double>(0, 0), "Learn Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰 정보 표시 // Display image view information
				if((res = layerLearn2.DrawTextCanvas(new CFLPoint<double>(0, 0), "Learn 2 Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerDestination.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerDestination2.DrawTextCanvas(new CFLPoint<double>(0, 0), "Destination 2 Image", EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}


				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewDestinationImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 새로 생성한 이미지를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created image
				if((res = viewDestination2Image.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit image view.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewLearnImage.Invalidate(true);
				viewLearn2Image.Invalidate(true);
				viewDestinationImage.Invalidate(true);
				viewDestination2Image.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(viewLearnImage.IsAvailable() && viewLearn2Image.IsAvailable() && viewDestinationImage.IsAvailable() && viewDestination2Image.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

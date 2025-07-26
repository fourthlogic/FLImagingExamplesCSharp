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
			public CStereoCalibrator3D.SGridResult sGridData;
		};

		static bool DrawGridPoints(SGridDisplay sGridDisplay, CGUIViewImageLayer pLayer)
		{
			bool bOK = false;

			// 결과 enum 선언
			CResult res = new CResult();

			do
			{
				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				pLayer.Clear();

				// 그리기 색상 설정 // Set drawing color
				EColor[] colorPool = new EColor[3];
				colorPool[0] = EColor.RED;
				colorPool[1] = EColor.LIME;
				colorPool[2] = EColor.CYAN;

				int i64GridRow = (int)sGridDisplay.sGridData.i64Rows;
				int i64GridCol = (int)sGridDisplay.sGridData.i64Columns;

				// Grid 그리기 // Draw grid
				for(int i64Row = 0; i64Row < i64GridRow; ++i64Row)
				{
					for(int i64Col = 0; i64Col < i64GridCol - 1; ++i64Col)
					{
						int i64GridIdx = i64Row * i64GridCol + i64Col;
						TPoint<double> pFlpGridPoint1 = (sGridDisplay.sGridData.arrGridData[i64Row][i64Col]);
						TPoint<double> pFlpGridPoint2 = (sGridDisplay.sGridData.arrGridData[i64Row][i64Col + 1]);
						CFLLine<double> fllDrawLine = new CFLLine<double>(pFlpGridPoint1, pFlpGridPoint2);

						if((res = pLayer.DrawFigureImage(fllDrawLine, EColor.BLACK, 5)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure.\n");
							break;
						}

						if((res = pLayer.DrawFigureImage(fllDrawLine, colorPool[i64GridIdx % 3], 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure.\n");
							break;
						}
					}

					if(i64Row < i64GridRow - 1)
					{
						TPoint<double> pFlpGridPoint1 = (sGridDisplay.sGridData.arrGridData[i64Row][i64GridCol - 1]);
						TPoint<double> pFlpGridPoint2 = (sGridDisplay.sGridData.arrGridData[i64Row + 1][0]);
						CFLLine<double> fllDrawLine = new CFLLine<double>();
						fllDrawLine.Set(pFlpGridPoint1, pFlpGridPoint2);

						if((res = pLayer.DrawFigureImage(fllDrawLine, EColor.BLACK, 5)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure.\n");
							break;
						}

						if((res = pLayer.DrawFigureImage(fllDrawLine, EColor.YELLOW, 3)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure.\n");
							break;
						}
					}
				}

				EColor colorText = EColor.YELLOW;
				colorPool[2] = EColor.CYAN;
				double f64PointDist = 0;
				double f64Dx = 0;
				double f64Dy = 0;

				// Grid Point 인덱싱 // Index Grid Point
				for(int i64Row = 0; i64Row < i64GridRow; ++i64Row)
				{
					TPoint<double> tpGridPoint1 = (sGridDisplay.sGridData.arrGridData[i64Row][0]);
					TPoint<double> tpGridPoint2 = (sGridDisplay.sGridData.arrGridData[i64Row][1]);
					CFLPoint<double> flpGridPoint1 = new CFLPoint<double>(tpGridPoint1.x, tpGridPoint1.y);
					CFLPoint<double> flpGridPoint2 = new CFLPoint<double>(tpGridPoint2.x, tpGridPoint2.y);

					double f64AngleIner = flpGridPoint1.GetAngle(flpGridPoint2);

					for(int i64Col = 0; i64Col < i64GridCol; ++i64Col)
					{
						long i64GridIdx = i64Row * i64GridCol + i64Col;

						if(i64Col < i64GridCol - 1)
						{
							tpGridPoint1 = (sGridDisplay.sGridData.arrGridData[i64Row][i64Col]);
							tpGridPoint2 = (sGridDisplay.sGridData.arrGridData[i64Row][i64Col + 1]);

							f64Dx = tpGridPoint2.x - tpGridPoint1.x;
							f64Dy = tpGridPoint2.y - tpGridPoint1.y;
							f64PointDist = Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy);
						}

						if(i64Row > 0)
						{
							tpGridPoint1 = (sGridDisplay.sGridData.arrGridData[i64Row][i64Col]);
							tpGridPoint2 = (sGridDisplay.sGridData.arrGridData[i64Row - 1][i64Col]);

							f64Dx = tpGridPoint2.x - tpGridPoint1.x;
							f64Dy = tpGridPoint2.y - tpGridPoint1.y;
							f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
						}
						else
						{
							tpGridPoint1 = (sGridDisplay.sGridData.arrGridData[0][i64Col]);
							tpGridPoint2 = (sGridDisplay.sGridData.arrGridData[1][i64Col]);

							f64Dx = tpGridPoint2.x - tpGridPoint1.x;
							f64Dy = tpGridPoint2.y - tpGridPoint1.y;
							f64PointDist = Math.Min(f64PointDist, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
						}

						string wstrGridIdx;
						wstrGridIdx = String.Format("{0}", i64GridIdx);
						colorText = colorPool[i64GridIdx % 3];

						if(i64Col == i64GridCol - 1)
							colorText = EColor.YELLOW;

						if((res = pLayer.DrawTextImage(tpGridPoint1, wstrGridIdx, colorText, EColor.BLACK, (int)(f64PointDist / 2), true, f64AngleIner)).IsFail())
						{
							ErrorPrint(res, "Failed to draw figure.\n");
							break;
						}
					}
				}

				// Board Region 그리기 // Draw Board Region
				CFLQuad<double> flqBoardRegion = sGridDisplay.sGridData.pFlqBoardRegion;
				CFLPoint<double> flpPoint1 = new CFLPoint<double>(flqBoardRegion.flpPoints[0]);
				CFLPoint<double> flpPoint2 = new CFLPoint<double>(flqBoardRegion.flpPoints[1]);
				double f64Angle = flpPoint1.GetAngle(flpPoint2);
				string wstringData;
				wstringData = string.Format("({0} X {1})", sGridDisplay.sGridData.i64Columns, sGridDisplay.sGridData.i64Rows);

				if((res = pLayer.DrawFigureImage(flqBoardRegion, EColor.YELLOW, 3)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure.\n");
					break;
				}

				if((res = pLayer.DrawTextImage(flpPoint1, wstringData, EColor.YELLOW, EColor.BLACK, 15, false, f64Angle, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}
			}
			while(false);

			return bOK;
		}

		public class CMessageReceiver : CFLBase
		{
			// CMessageReceiver 생성자 // CMessageReceiver constructor
			public CMessageReceiver(ref CGUIViewImage viewImage)
			{
				m_viewImage = viewImage;

				// 메세지를 전달 받기 위해 CBroadcastManager 에 구독 등록 //Subscribe to CBroadcast Manager to receive messages
				CBroadcastManager.Subscribe(this);
			}

			// CMessageReceiver 소멸자 // CMessageReceiver Destructor
			~CMessageReceiver()
			{
				// 객체가 소멸할때 메세지 수신을 중단하기 위해 구독을 해제한다. // Unsubscribe to stop receiving messages when the object disappears.
				CBroadcastManager.Unsubscribe(this);
			}

			public void SetGrid(ref SGridDisplay[] sGridDisplay)
			{
				m_vctGridDisplay = sGridDisplay;
			}

			// 메세지가 들어오면 호출되는 함수 OnReceiveBroadcast 오버라이드하여 구현 // Implemented by overriding the function OnReceive Broadcast that is invoked when a message is received
			public override void OnReceiveBroadcast(CBroadcastMessage message)
			{
				do
				{
					// message 가 null 인지 확인 // Verify message is null
					if(message == null)
						break;

					// GetCaller() 가 등록한 이미지뷰인지 확인 // Verify that GetCaller() is a registered image view
					if(message.GetCaller() != m_viewImage)
						break;

					// 메세지의 채널을 확인 // Check the channel of the message
					switch(message.GetChannel())
					{
					case (uint)EGUIBroadcast.ViewImage_PostPageChange:
						{
							// 메세지를 호출한 객체를 CGUIViewImage 로 캐스팅 // Casting the object that called the message as CGUIViewImage
							CGUIViewImage viewImage = message.GetCaller() as CGUIViewImage;

							// viewImage 가 null 인지 확인 // Verify viewImage is null
							if(viewImage == null)
								break;

							CFLImage fliTmp = viewImage.GetImage();

							if(fliTmp == null)
								break;

							int i64CurPage = fliTmp.GetSelectedPageIndex();

							// 이미지뷰의 0번 레이어 가져오기 // Get layer 0th of image view
							CGUIViewImageLayer layer = viewImage.GetLayer((int)(i64CurPage % 10));

							for(int i = 0; i < 10; ++i)
								viewImage.GetLayer((int)i).Clear();

							for(int i64Idx = 0; i64Idx < (int)fliTmp.GetPageCount(); ++i64Idx)
							{
								if(m_vctGridDisplay[(int)i64Idx].i64ImageIdx == i64CurPage)
									DrawGridPoints(m_vctGridDisplay[(int)i64Idx], layer);
							}

							// 이미지뷰를 갱신 // Update the image view.
							viewImage.Invalidate();
						}
						break;
					}
				}
				while(false);
			}

			SGridDisplay[] m_vctGridDisplay;
			CGUIViewImage m_viewImage;
		}

		static bool Calibration(CStereoCalibrator3D sSC, CFLImage fliLearnImage, CFLImage fliLearnImage2)
		{
			bool bResult = false;

			// 결과 enum 선언
			CResult res = new CResult();

			do
			{
				// Learn 이미지 설정 // Set learn image
				if((res = sSC.SetLearnImage(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image.\n");
					break;
				}

				// Learn 이미지 설정 // Set learn image 2
				if((res = sSC.SetLearnImage2(ref fliLearnImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image.\n");
					break;
				}

				// Optimal Solution Accuracy 설정 // Set the optical solution accuracy
				if((res = sSC.SetOptimalSolutionAccuracy(1e-5)).IsFail())
				{
					ErrorPrint(res, "Failed to set Optimal Solution Accuracy.\n");
					break;
				}

				// Grid Type 설정 // Set the grid type
				if((res = sSC.SetGridType(CStereoCalibrator3D.EGridType.ChessBoard)).IsFail())
				{
					ErrorPrint(res, "Failed to set Grid Type.\n");
					break;
				}

				// Calibration 실행 // Execute calibration
				if((res = sSC.Calibrate()).IsFail())
				{
					ErrorPrint(res, "Calibration failed.\n");
					break;
				}

				bResult = true;
			}
			while(false);

			return bResult;
		}

		static bool Undistortion(CStereoCalibrator3D sSC, CFLImage fliSourceImage, CFLImage fliSourceImage2, CFLImage fliDestinationImage, CFLImage fliDestinationImage2)
		{
			bool bResult = false;

			// 결과 enum 선언 // Declare result enum;
			CResult res = new CResult();

			do
			{
				// Source 이미지 설정 // Set source image
				if((res = sSC.SetSourceImage(ref fliSourceImage)).IsFail())
				{
					ErrorPrint(res, "Failed to load image.\n");
					break;
				}

				// Source 이미지 2 설정 // Set source image 2
				if((res = sSC.SetSourceImage2(ref fliSourceImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to load image.\n");
					break;
				}

				// Destination 이미지 설정 // Set the destination image
				if((res = sSC.SetDestinationImage(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to load image.\n");
					break;
				}

				// Destination 이미지 2 설정 // Set destination image 2
				if((res = sSC.SetDestinationImage2(ref fliDestinationImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to load image.\n");
					break;
				}

				// Interpolation 알고리즘 설정 // Set interpolation algorithm
				if((res = sSC.SetInterpolationMethod(EInterpolationMethod.Bilinear)).IsFail())
				{
					ErrorPrint(res, "Failed to set interpolation method.\n");
					break;
				}

				// Undistortion 실행 // Execute undistortion
				if((res = sSC.Execute()).IsFail())
				{
					ErrorPrint(res, "Undistortion failed.\n");
					break;
				}

				bResult = true;
			}
			while(false);

			return bResult;
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliLearnImage = new CFLImage(), fliSourceImage = new CFLImage(), fliDestinationImage = new CFLImage();
			CFLImage fliLearnImage2 = new CFLImage(), fliSourceImage2 = new CFLImage(), fliDestinationImage2 = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageLearn2 = new CGUIViewImage();
			CGUIViewImage viewImageDestination = new CGUIViewImage();
			CGUIViewImage viewImageDestination2 = new CGUIViewImage();

			// Camera Calibrator 객체 생성 // Create Camera Calibrator object
			CStereoCalibrator3D sSC = new CStereoCalibrator3D();
			CMessageReceiver msgReceiver = new CMessageReceiver(ref viewImageLearn);
			CMessageReceiver msgReceiver2 = new CMessageReceiver(ref viewImageLearn2);

			// 결과 enum 선언 // Declare result enum;
			CResult res = new CResult();

			do
			{
				// Learn 이미지 로드 // Load learn image
				if((res = fliLearnImage.Load("../../ExampleImages/StereoCalibrator3D/Left.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Learn 이미지 2 로드 // Load learn image 2
				if((res = fliLearnImage2.Load("../../ExampleImages/StereoCalibrator3D/Right.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Page 0 선택 // Select page 0
				fliLearnImage.SelectPage(0);
				fliLearnImage2.SelectPage(0);

				Console.WriteLine("Processing....\n");

				// Stereo calibration 수행 // Execute stereo calibration
				if(!Calibration(sSC, fliLearnImage, fliLearnImage2))
					break;

				// Source 이미지에 Learn 이미지를 복사 (얕은 복사) // Copy the learn image to source image (Shallow Copy)
				fliSourceImage.Assign(fliLearnImage, false);

				// Source 이미지 2에 Learn 이미지 2를 복사 (얕은 복사) // Copy the learn image 2 to source image 2 (Shallow Copy)
				fliSourceImage2.Assign(fliLearnImage2, false);

				CMultiVar<long> mvBlank = new CMultiVar<long>(0);

				// Destination 이미지 생성 // Create destination image
				if((res = fliDestinationImage.Create(fliSourceImage.GetWidth(), fliSourceImage.GetHeight(), mvBlank, fliSourceImage.GetPixelFormat())).IsFail())
				{
					ErrorPrint(res, "Failed to create the image file.\n");
					break;
				}

				// Destination 이미지 생성 // Create destination image
				if((res = fliDestinationImage2.Create(fliSourceImage.GetWidth(), fliSourceImage.GetHeight(), mvBlank, fliSourceImage.GetPixelFormat())).IsFail())
				{
					ErrorPrint(res, "Failed to create the image file.\n");
					break;
				}

				// Undistortion 수행 // Execute undistortion
				if(!Undistortion(sSC, fliSourceImage, fliSourceImage2, fliDestinationImage, fliDestinationImage2))
					break;

				// 화면에 격자 탐지 결과 출력 // Display the result of grid detection
				SGridDisplay[] sArrGridDisplay = new SGridDisplay[5];
				sArrGridDisplay[0] = new SGridDisplay();
				sArrGridDisplay[1] = new SGridDisplay();
				sArrGridDisplay[2] = new SGridDisplay();
				sArrGridDisplay[3] = new SGridDisplay();
				sArrGridDisplay[4] = new SGridDisplay();

				SGridDisplay[] sArrGridDisplay2 = new SGridDisplay[5];
				sArrGridDisplay2[0] = new SGridDisplay();
				sArrGridDisplay2[1] = new SGridDisplay();
				sArrGridDisplay2[2] = new SGridDisplay();
				sArrGridDisplay2[3] = new SGridDisplay();
				sArrGridDisplay2[4] = new SGridDisplay();

				for(long i64ImgIdx = 0; i64ImgIdx < (long)fliLearnImage.GetPageCount(); ++i64ImgIdx)
				{
					sArrGridDisplay[i64ImgIdx].sGridData = new CStereoCalibrator3D.SGridResult();
					sSC.GetResultGridPoints(ref sArrGridDisplay[i64ImgIdx].sGridData, i64ImgIdx);
					sArrGridDisplay[i64ImgIdx].i64ImageIdx = i64ImgIdx;
				}

				for(long i64ImgIdx = 0; i64ImgIdx < (long)fliLearnImage2.GetPageCount(); ++i64ImgIdx)
				{
					sArrGridDisplay2[i64ImgIdx].sGridData = new CStereoCalibrator3D.SGridResult();
					sSC.GetResultGridPoints2(ref sArrGridDisplay2[i64ImgIdx].sGridData, i64ImgIdx);
					sArrGridDisplay2[i64ImgIdx].i64ImageIdx = i64ImgIdx;
				}

				msgReceiver.SetGrid(ref sArrGridDisplay);
				msgReceiver2.SetGrid(ref sArrGridDisplay2);

				// Learn 이미지 뷰 생성 // Create learn image view
				if((res = viewImageLearn.Create(300, 0, 300 + 480 * 1, 360)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Learn 이미지 2 뷰 생성 // Create learn image 2 view
				if((res = viewImageLearn2.Create(300 + 480, 0, 300 + 480 * 2, 360)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Learn 이미지 뷰에 이미지를 디스플레이 // Display learn image on the image view
				if((res = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Learn 이미지 2 뷰에 이미지를 디스플레이 // Display the image on the learn image 2 view
				if((res = viewImageLearn2.SetImagePtr(ref fliLearnImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer pLayer = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer pLayer2 = viewImageLearn2.GetLayer(0);

				// Chess Board Grid 출력
				DrawGridPoints(sArrGridDisplay[0], pLayer);
				DrawGridPoints(sArrGridDisplay2[0], pLayer2);

				viewImageLearn.Invalidate();

				// Source 이미지 뷰 생성 // Create source image view
				if((res = viewImageDestination.Create(300, 360, 780, 720)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 이미지 뷰 생성 // Create destination image view
				if((res = viewImageDestination2.Create(780, 360, 1260, 720)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Source 이미지 뷰에 이미지를 디스플레이 // Display the image in the source image view
				if((res = viewImageDestination.SetImagePtr(ref fliDestinationImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Destination 이미지 뷰에 이미지를 디스플레이 // Display the image in the destination image view
				if((res = viewImageDestination2.SetImagePtr(ref fliDestinationImage2)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageLearn.SynchronizePointOfView(ref viewImageLearn2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImageDestination.SynchronizePointOfView(ref viewImageDestination2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageLearn2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageDestination)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageDestination2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 페이지 인덱스를 동기화 한다 // Synchronize the page index of the two image view windows
				if((res = viewImageLearn.SynchronizePageIndex(ref viewImageLearn2)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view.\n");
					break;
				}

				// calibration data 출력 // Display the calibration data
				CStereoCalibrator3D.SIntrinsicParameters sIntrinsicParam = sSC.GetResultIntrinsicParameters();
				CStereoCalibrator3D.SDistortionCoefficients sDistortCoeef = sSC.GetResultDistortionCoefficients();

				CStereoCalibrator3D.SIntrinsicParameters sIntrinsicParam2 = sSC.GetResultIntrinsicParameters2();
				CStereoCalibrator3D.SDistortionCoefficients sDistortCoeef2 = sSC.GetResultDistortionCoefficients2();

				CStereoCalibrator3D.SRotationParameters sRotationParam = sSC.GetResultRotationParameters();
				CStereoCalibrator3D.SRotationParameters sRotationParam2 = sSC.GetResultRotationParameters2();

				CStereoCalibrator3D.STranslationParameters sTranslationParam = sSC.GetResultTranslationParameters();
				CStereoCalibrator3D.STranslationParameters sTranslationParam2 = sSC.GetResultTranslationParameters2();

				double f64ReprojError = sSC.GetResultReProjectionError();

				string strMatrix = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", sIntrinsicParam.f64FocalLengthX, sIntrinsicParam.f64Skew, sIntrinsicParam.f64PrincipalPointX, 0, sIntrinsicParam.f64FocalLengthY, sIntrinsicParam.f64PrincipalPointY, 0, 0, 1);

				string strDistVal = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", sDistortCoeef.f64K1, sDistortCoeef.f64K2, sDistortCoeef.f64P1, sDistortCoeef.f64P2, sDistortCoeef.f64K3, sDistortCoeef.f64K4, sDistortCoeef.f64K5, sDistortCoeef.f64K6, sDistortCoeef.f64S1, sDistortCoeef.f64S2, sDistortCoeef.f64S3, sDistortCoeef.f64S4, sDistortCoeef.f64Gx, sDistortCoeef.f64Gy);

				string strMatrix2 = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", sIntrinsicParam.f64FocalLengthX, sIntrinsicParam.f64Skew, sIntrinsicParam.f64PrincipalPointX, 0, sIntrinsicParam.f64FocalLengthY, sIntrinsicParam.f64PrincipalPointY, 0, 0, 1);

				string strDistVal2 = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}", sDistortCoeef2.f64K1, sDistortCoeef2.f64K2, sDistortCoeef2.f64P1, sDistortCoeef2.f64P2, sDistortCoeef2.f64K3, sDistortCoeef2.f64K4, sDistortCoeef2.f64K5, sDistortCoeef2.f64K6, sDistortCoeef2.f64S1, sDistortCoeef2.f64S2, sDistortCoeef2.f64S3, sDistortCoeef2.f64S4, sDistortCoeef2.f64Gx, sDistortCoeef2.f64Gy);

				string strRotatMatrix = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", sRotationParam.f64R0, sRotationParam.f64R1, sRotationParam.f64R2, sRotationParam.f64R3, sRotationParam.f64R4, sRotationParam.f64R5, sRotationParam.f64R6, sRotationParam.f64R7, sRotationParam.f64R8);

				string strRotatMatrix2 = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}", sRotationParam2.f64R0, sRotationParam2.f64R1, sRotationParam2.f64R2, sRotationParam2.f64R3, sRotationParam2.f64R4, sRotationParam2.f64R5, sRotationParam2.f64R6, sRotationParam2.f64R7, sRotationParam2.f64R8);

				string strTranslVal = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", sTranslationParam.f64T0, sTranslationParam.f64T1, sTranslationParam.f64T2, sTranslationParam.f64T3, sTranslationParam.f64T4, sTranslationParam.f64T5, sTranslationParam.f64T6, sTranslationParam.f64T7, sTranslationParam.f64T8, sTranslationParam.f64T9, sTranslationParam.f64T10, sTranslationParam.f64T11);

				string strTranslVal2 = String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", sTranslationParam2.f64T0, sTranslationParam2.f64T1, sTranslationParam2.f64T2, sTranslationParam2.f64T3, sTranslationParam2.f64T4, sTranslationParam2.f64T5, sTranslationParam2.f64T6, sTranslationParam2.f64T7, sTranslationParam2.f64T8, sTranslationParam2.f64T9, sTranslationParam2.f64T10, sTranslationParam2.f64T11);

				Console.WriteLine("Intrinsic Parameters");
				Console.WriteLine("{0}", strMatrix);
				Console.WriteLine("Distortion Coefficients");
				Console.WriteLine("{0}", strDistVal);
				Console.WriteLine("Rotation Parameters");
				Console.WriteLine("{0}", strRotatMatrix);
				Console.WriteLine("Translation Parameters");
				Console.WriteLine("{0}\n", strTranslVal);
				Console.WriteLine("Intrinsic Parameters 2");
				Console.WriteLine("{0}", strMatrix2);
				Console.WriteLine("Distortion Coefficients 2");
				Console.WriteLine("{0}", strDistVal2);
				Console.WriteLine("Rotation Parameters 2");
				Console.WriteLine("{0}", strRotatMatrix2);
				Console.WriteLine("Translation Parameters 2");
				Console.WriteLine("{0}\n", strTranslVal2);
				Console.WriteLine("Re-Projection Error");
				Console.WriteLine("{0}", f64ReprojError);

				long i64Height = fliDestinationImage.GetHeight();
				long i64Width = fliDestinationImage.GetWidth();

				for(int i32Iter = 0; i32Iter < 2; ++i32Iter)
				{
					for(int i32Index = 0; i32Index < 20; ++i32Index)
					{
						CFLLine<double> fllHorizonLine = new CFLLine<double>(0, i64Height / 20 * i32Index, i64Width, i64Height / 20 * i32Index);

						CGUIViewImageLayer layerDst = (i32Iter == 0 ? viewImageDestination.GetLayer(0) : viewImageDestination2.GetLayer(0));
						layerDst.DrawFigureImage(fllHorizonLine, EColor.LIME, 1);
					}
				}

				viewImageLearn.Invalidate();
				viewImageLearn2.Invalidate();
				viewImageDestination.Invalidate();
				viewImageDestination2.Invalidate();

				////// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImageLearn.IsAvailable() && viewImageLearn2.IsAvailable() && viewImageDestination.IsAvailable() && viewImageDestination2.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

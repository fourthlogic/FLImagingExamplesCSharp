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

namespace CameraCalibrator
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
            // 이미지 객체 선언 // Declare the image object
            CFLImage fliImage = new CFLImage();

            // 이미지 뷰 선언 // Declare the image view
            CGUIViewImage viewImage = new CGUIViewImage();
            CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/GridOfRectangleDetector/GridOfRectangle.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 1040, 480)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// Grid Of Rectangle Detector 객체 생성 // Create Grid Of Rectangle Detector Object
				CGridOfRectangleDetector gridofRectangle = new CGridOfRectangleDetector();

				// 처리할 이미지 설정 // Set the image to process
				gridofRectangle.SetSourceImage(ref fliImage);

				// 알고리즘 수행 // Execute the Algoritm
				if((res = gridofRectangle.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Grid Of Rectangle Detector.");
					break;
				}

				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				layer.Clear();

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to know where the ROI area is
				CFLQuad<double> flqRegion = new CFLQuad<double>();
				long i64ResultRow = 0;
				long i64ResultCol = 0;
				double f64AverageCellPitch;
				List<List<TPoint<double>>> flaPoints = new List<List<TPoint<double>>>();

				// 페이지 0번 보드 갯수를 가져옴. // Page 0 Gets the number of boards.
				Int64 i64PageIndex = 0;
				Int64 i64BoardCount = gridofRectangle.GetResultBoardCount(i64PageIndex);

				for(Int64 i32BoardIndex = 0; i32BoardIndex < i64BoardCount; i32BoardIndex++)
				{
					gridofRectangle.GetResultCenterPoints(i64PageIndex, i64BoardCount, ref flaPoints);
					gridofRectangle.GetResultBoardRegion(i64PageIndex, i64BoardCount, ref flqRegion);
					i64ResultRow = gridofRectangle.GetResultBoardRows(i64PageIndex, i64BoardCount);
					i64ResultCol = gridofRectangle.GetResultBoardColumns(i64PageIndex, i64BoardCount);
					f64AverageCellPitch = gridofRectangle.GetResultBoardAverageCellPitch(i64PageIndex, i64BoardCount);

					CFLPoint<double> flpPoint0 = new CFLPoint<double>(flqRegion.flpPoints[0]);
					CFLPoint<double> flpPoint1 = new CFLPoint<double>(flqRegion.flpPoints[1]);

					double f64Width = flpPoint0.GetDistance(flpPoint1);
					double f64Angle = flpPoint0.GetAngle(flpPoint1);

					if((res = layer.DrawFigureImage(flqRegion, EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure.\n");
						break;
					}

					if((res = layer.DrawFigureImage(flqRegion, EColor.YELLOW, 1)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure.\n");
						break;
					}

					if((res = layer.DrawTextImage(flpPoint0, String.Format("({0} X {1}) Pitch [{2}]", i64ResultCol, i64ResultRow, f64AverageCellPitch), EColor.YELLOW, EColor.BLACK, (int)(f64Width / 16), true, f64Angle, EGUIViewImageTextAlignment.LEFT_BOTTOM)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text.\n");
						break;
					}

					int i32LineTransition;
					int i32VertexNumber;
					EColor[] crTable = new EColor[3];
					crTable[0] = EColor.RED;
					crTable[1] = EColor.LIME;
					crTable[2] = EColor.CYAN;

					i32LineTransition = 0;
					i32VertexNumber = 0;

					double f64Pitch = 0;
					CFLPoint<double> flpLastPoint = new CFLPoint<double>();

					for(long j = 0; j < flaPoints.Count(); ++j)
					{
						List<TPoint<double>> fla2 = flaPoints[(int)j];

						if(j > 0)
						{
							CFLPoint<double> fla20 = new CFLPoint<double>();
							fla20.x = fla2[0].x;
							fla20.y = fla2[0].y;

							CFLLine<double> fll = new CFLLine<double>(flpLastPoint, fla20);

							if((res = layer.DrawFigureImage(fll, EColor.BLACK, 5)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure.\n");
								break;
							}

							if((res = layer.DrawFigureImage(fll, EColor.YELLOW, 3)).IsFail())
							{
								ErrorPrint(res, "Failed to draw figure.\n");
								break;
							}
						}

						for(long k = 0; k < fla2.Count(); ++k)
						{
							if(k > 0)
							{
								CFLPoint<double> fla2k = new CFLPoint<double>();
								fla2k.x = fla2[(int)k].x;
								fla2k.y = fla2[(int)k].y;

								CFLLine<double> fll = new CFLLine<double>(flpLastPoint, fla2k);

								if((res = layer.DrawFigureImage(fll, EColor.BLACK, 5)).IsFail())
								{
									ErrorPrint(res, "Failed to draw figure.\n");
									break;
								}

								if((res = layer.DrawFigureImage(fll, crTable[i32LineTransition++ % 3], 3)).IsFail())
								{
									ErrorPrint(res, "Failed to draw figure.\n");
									break;
								}
							}

							CFLPoint<double> fla2kk = new CFLPoint<double>();
							fla2kk.x = fla2[(int)k].x;
							fla2kk.y = fla2[(int)k].y;

							flpLastPoint = fla2kk;
						}
					}

					i32LineTransition = 0;

					for(long j = 0; j < flaPoints.Count(); ++j)
					{
						List<TPoint<double>> fla2 = flaPoints[(int)j];
						CFLPoint<double> fla2Point0 = new CFLPoint<double>();
						CFLPoint<double> fla2Point1 = new CFLPoint<double>();
						fla2Point0.x = fla2[0].x;
						fla2Point0.y = fla2[0].y;
						fla2Point1.x = fla2[1].x;
						fla2Point1.y = fla2[1].y;

						double f64AngleText = fla2Point0.GetAngle(fla2Point1);

						for(long k = 0; k < fla2.Count(); ++k)
						{
							EColor crTextColor = crTable[(i32LineTransition++) % 3];

							int i32CheckValue = (i32VertexNumber + 1) % fla2.Count();

							if(i32CheckValue == 0)
								crTextColor = EColor.YELLOW;
							else
							{
								double f64Dx = fla2[(int)k + 1].x - fla2[(int)k].x;
								double f64Dy = fla2[(int)k + 1].y - fla2[(int)k].y;

								f64Pitch = Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy);
							}

							if(j == 0)
							{
								double f64Dx = flaPoints[1][(int)k].x - flaPoints[0][(int)k].x;
								double f64Dy = flaPoints[1][(int)k].y - flaPoints[0][(int)k].y;

								f64Pitch = Math.Min(f64Pitch, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
							}
							else
							{
								double f64Dx = flaPoints[(int)j][(int)k].x - flaPoints[(int)j - 1][(int)k].x;
								double f64Dy = flaPoints[(int)j][(int)k].y - flaPoints[(int)j - 1][(int)k].y;

								f64Pitch = Math.Min(f64Pitch, Math.Sqrt(f64Dx * f64Dx + f64Dy * f64Dy));
							}

							CFLPoint<double> flpDisPlay = new CFLPoint<double>();
							flpDisPlay.x = fla2[(int)k].x;
							flpDisPlay.y = fla2[(int)k].y;

							if((res = layer.DrawTextImage(flpDisPlay, String.Format("{0}", i32VertexNumber++), crTextColor, EColor.BLACK, (int)(f64Pitch / 3), true, f64Angle)).IsFail())
							{
								ErrorPrint(res, "Failed to draw text.\n");
								break;
							}

							if(k > 0)
							{
								CFLPoint<double> flpAngle0 = new CFLPoint<double>(fla2[(int)k].x, fla2[(int)k].y);
								CFLPoint<double> flpAngle1 = new CFLPoint<double>(fla2[(int)k - 1].x, fla2[(int)k - 1].y);

								f64Angle = flpAngle1.GetAngle(flpAngle0);
							}
						}

						--i32LineTransition;
					}
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImage.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
    }
}

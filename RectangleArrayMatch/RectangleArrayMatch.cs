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

namespace FLImagingExamplesCSharp
{
	class RectangleArrayMatch
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

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliLearnImage = new CFLImage();
			CFLImage fliFindImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageLearn = new CGUIViewImage();
			CGUIViewImage viewImageFind = new CGUIViewImage();

			CResult res;

			do
			{
				// 이미지 로드 // Load image
				if((res = fliLearnImage.Load("../../ExampleImages/Matching/Rectangle Array_0.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				if((res = fliFindImage.Load("../../ExampleImages/Matching/Rectangle Array_1.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageLearn.Create(400, 0, 912, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageFind.Create(912, 0, 1680, 576)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImageLearn.SetImagePtr(ref fliLearnImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImageFind.SetImagePtr(ref fliFindImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				if((res = viewImageLearn.SynchronizeWindow(ref viewImageFind)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				CGUIViewImageLayer layerLearn = viewImageLearn.GetLayer(0);
				CGUIViewImageLayer layerFind = viewImageFind.GetLayer(1);

				layerLearn.Clear();
				layerFind.Clear();

				CFLPoint<double> flp00 = new CFLPoint<double>(0, 0);

				if((res = layerLearn.DrawTextCanvas(flp00, "Measurement Array", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layerFind.DrawTextCanvas(flp00, "FIND", EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// Rectangle Array Match 객체 생성 // Create Rectangle Array Match Object
				CRectangleArrayMatch arrayMatch = new CRectangleArrayMatch();

				// 학습할 영역을 설정합니다. // Set the area to learn.
				CFLFigureArray flfaMeasurement = new CFLFigureArray();
				CFLRect<double> flrRect00 = new CFLRect<double>(587.479194, 364.452004, 929.550836, 616.575019);
				CFLRect<double> flrRect01 = new CFLRect<double>(583.464651, 1215.493013, 924.560595, 1467.566788);
				CFLRect<double> flrRect02 = new CFLRect<double>(1531.503352, 655.504324, 1872.516362, 908.626989);
				CFLRect<double> flrRect03 = new CFLRect<double>(1241.471070, 1222.460787, 1580.517129, 1474.488487);

				flfaMeasurement.PushBack(flrRect00);
				flfaMeasurement.PushBack(flrRect01);
				flfaMeasurement.PushBack(flrRect02);
				flfaMeasurement.PushBack(flrRect03);

				CFLPoint<double> flpCameraPivot = new CFLPoint<double>(0, 0);
				CRectangleArrayMatch.EFitting eFitting = CRectangleArrayMatch.EFitting.Enable;

				// 검출 시 사용될 파라미터를 설정합니다. // Set the parameters to be used for detection.
				// 탐색할 이미지를 설정합니다. // Set the image to browse.
				arrayMatch.SetSourceImage(ref fliFindImage);
				// 측정 배열을 설정합니다. // Set up the measurement array.
				arrayMatch.SetArray(flfaMeasurement);
				// 탐색 시, MeasurementArray의 기본 각도를 설정합니다. // On navigation, set the default angle of the MeasurementArray.
				arrayMatch.SetBaseAngle(0.0);
				// 중심의 초기값을 이미지 중심으로 할지 설정합니다. // Set whether the initial value of the center is the center of the image.
				arrayMatch.EnablePivotImageCenter(true);
				// 중심 오프셋을 설정합니다. // Set the center offset.
				arrayMatch.SetPivotOffset(flpCameraPivot);
				// 최소 스코어 점수를 설정합니다. // Set the minimum score score.
				arrayMatch.SetMinScore(0.5);
				// 탐색 시, 각도 탐색 범위를 설정합니다. // When searching, set the angle search range.
				arrayMatch.SetObjectAngleTolerance(180);
				// 탐색 시, Fitting Enable/Disable을 설정합니다. // When searching, set Fitting Enable/Disable.
				arrayMatch.SetFitting(eFitting);
				// 탐색 시, 허용 이동량 범위를 설정합니다. // When searching, set the allowable movement range.
				arrayMatch.SetAllowingObjectDistanceError(-1);

				for(long i64Index = 0; i64Index < flfaMeasurement.GetCount(); ++i64Index)
				{
					if(flfaMeasurement.GetAt(i64Index).GetDeclType() != EFigureDeclType.Rect)
						break;

					CFLRect<double> pFlr = (CFLRect<double>)flfaMeasurement.GetAt(i64Index);

					// 배열 측정 영역이 어디인지 알기 위해 디스플레이 한다 // Display to see where the array measurement area is
					if((res = layerLearn.DrawFigureImage(flfaMeasurement, EColor.BLUE, 3, EColor.BLUE, EGUIViewImagePenStyle.Solid, 0.25f, 0.25f)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure.\n");
						break;
					}
				}

				// 알고리즘 수행 // Execute the Algoritm
				if((res = arrayMatch.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}


				// 검출 결과 배열의 개수를 가져옵니다. // Get the number of detection result arrays.
				long i64ResultCount = arrayMatch.GetResultCount();
				double f64Score = 0, f64Angle = 0;
				// 검출 결과 배열의 점수를 가져옵니다. // Get the score of the detection result array.
				arrayMatch.GetResultArrayScore(ref f64Score);
				// 검출 결과 배열의 각도를 가져옵니다. // Get the angle of the detection result array.
				arrayMatch.GetResultArrayAngle(ref f64Angle);

				for(int i = 0; i < i64ResultCount; ++i)
				{
					CRectangleArrayMatch.SResult sResult = new CRectangleArrayMatch.SResult();

					// 검출 결과 중 배열 하나를 가져옵니다. // Get an array of detection results.
					arrayMatch.GetResult(i, ref sResult);
					CFLPoint<double> flpRegionCenter = sResult.pFlrMeasuredRegion.GetCenter();
					string strDisplayResult;
					strDisplayResult = String.Format("Array Element ID : {0}\n Score : {1}\n Angle : {2}", (int)sResult.i64Index, sResult.f64Score, sResult.f64Angle);

					// 검출 결과 중 배열 하나의 결과를 디스플레이 합니다. // Display the result of one array among the detection results.
					if((res = layerFind.DrawFigureImage(sResult.pFlrMeasuredRegion, EColor.BLACK, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure.\n");
						break;
					}

					// 검출 결과 중 배열 하나의 결과를 디스플레이 합니다. // Display the result of one array among the detection results.
					if((res = layerFind.DrawFigureImage(sResult.pFlrMeasuredRegion, EColor.CYAN, 1)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure.\n");
						break;
					}

					// 검출 결과 중 배열 하나의 결과의 중심점을 디스플레이 합니다. // Display the center point of the result of one of the detection results.
					if((res = layerFind.DrawTextImage(flpRegionCenter, strDisplayResult, EColor.YELLOW, EColor.BLACK, 11)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text.\n");
						break;
					}

					string strDisplayResultArray;

					if(i == 0)
					{
						strDisplayResultArray = String.Format("Array Score : {0}  Array Angle : {1}", f64Score, f64Angle);
						Console.WriteLine("{0}", strDisplayResultArray);

						strDisplayResult = String.Format("Array Score : {0}\n Array Angle : {1}", f64Score, f64Angle);

						CFLQuad<double> flqRegion = new CFLQuad<double>(sResult.pFlrMeasuredRegion);

						double f64MaxY = -10000000, f64MaxX = 1000000;

						double[] arrX = new double[4];
						double[] arrY = new double[4];

						arrX[0] = flqRegion.flpPoints[0].x;
						arrX[1] = flqRegion.flpPoints[1].x;
						arrX[2] = flqRegion.flpPoints[2].x;
						arrX[3] = flqRegion.flpPoints[3].x;

						arrY[0] = flqRegion.flpPoints[0].y;
						arrY[1] = flqRegion.flpPoints[1].y;
						arrY[2] = flqRegion.flpPoints[2].y;
						arrY[3] = flqRegion.flpPoints[3].y;

						for(int i32Search = 0; i32Search < 4; ++i32Search)
						{
							if(f64MaxY > arrY[i32Search])
								f64MaxY = arrY[i32Search];
						}

						for(int i32Search = 0; i32Search < 4; ++i32Search)
						{
							if(f64MaxY == arrY[i32Search])
							{
								if(f64MaxX > arrX[i32Search])
									f64MaxX = arrX[i32Search];
							}
						}

						CFLPoint<double> flpArrayResult = new CFLPoint<double>(f64MaxX, f64MaxY - 10);

						// 검출 결과 중 배열 하나의 결과의 정보를 디스플레이 합니다. // Display the information of the result of one of the detection results.
						if((res = layerFind.DrawTextImage(flpArrayResult, strDisplayResult, EColor.GOLD, EColor.BLACK, 14)).IsFail())
						{
							ErrorPrint(res, "Failed to draw text.\n");
							break;
						}
					}



					string strDisplayResultElement;
					strDisplayResultElement = String.Format("Array Element ID : {0} Score : {1} Angle : {2}", (int)sResult.i64Index, sResult.f64Score, sResult.f64Angle);
					Console.WriteLine(" - {0}", strDisplayResultElement);
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImageLearn.Invalidate(true);
				viewImageFind.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImageLearn.IsAvailable() && viewImageFind.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

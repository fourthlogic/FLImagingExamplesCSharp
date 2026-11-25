using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ThreeDim;
using CResult = FLImagingCLR.CResult;

namespace FLImagingExamplesCSharp
{
	class LeastSquares3D
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

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D[] view3D = { new CGUIView3D(), new CGUIView3D() };

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 3D 뷰 생성 // Create 3D views
				if((res = view3D[0].Create(100, 0, 700, 600)).IsFail() || (res = view3D[1].Create(700, 0, 1300, 600)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// 곡선 (100s, 40sin(πs), 20s^2), s = 0 ~ 1 위의 점들에 노이즈를 추가하여 데이터 생성
				// Generate data by adding noise to points on the curve (100s, 40sin(πs), 20s^2), s = 0 ~ 1
				CFLPointArray3<double> flpaData = new CFLPointArray3<double>();

				for(int i = 0; i <= 40; ++i)
				{
					double f64S = i / 40.0;
					double f64X = 100.0 * f64S + CRandomGenerator.Double(-1.0, 1.0);
					double f64Y = 40.0 * Math.Sin(Math.PI * f64S) + CRandomGenerator.Double(-1.0, 1.0);
					double f64Z = 20.0 * f64S * f64S + CRandomGenerator.Double(-1.0, 1.0);

					// 일부 점은 곡선에서 크게 벗어나도록 만든다. // Make some points deviate greatly from the curve.
					if(i == 8)
						f64Z += 25.0;
					else if(i == 22)
						f64Y -= 30.0;
					else if(i == 35)
						f64Z -= 25.0;

					flpaData.PushBack(new CFLPoint3<double>(f64X, f64Y, f64Z));
				}

				// CLeastSquares3D<double> 객체 생성 // Create CLeastSquares3D<double> object
				CLeastSquares3D<double> leastSquares3D = new CLeastSquares3D<double>();

				// 데이터를 할당 // Assign data
				if((res = leastSquares3D.Assign(flpaData)).IsFail())
				{
					ErrorPrint(res, "Failed to assign data.\n");
					break;
				}

				// 피팅할 다항식의 차수 // Degree of the polynomial to fit
				const int i32Degree = 3;

				// 결과 점을 얻을 매개변수 t 목록 (-0.3 ~ 1.3, 0.01 간격)
				// 데이터의 t 는 누적 거리 비율(0 ~ 1)로 지정되므로, 범위 밖의 t 는 데이터 끝부분 너머의 점을 추정합니다.
				// List of parameter t to get result points (-0.3 to 1.3, 0.01 interval)
				// Since t of the data is the cumulative distance ratio (0 to 1), t outside the range gives points beyond the ends of the data.
				List<double> listTargetT = new List<double>();

				for(int i = -30; i <= 130; ++i)
					listTargetT.Add(i / 100.0);

				// 각 뷰에서 사용할 Outlier 파라미터 // Outlier parameters for each view
				string[] arrStrViewName = { "Default", "Outlier Exclusion" };
				long[] arrI64OutlierThresholdCount = { 0, 3 };
				double[] arrF64OutlierThreshold = { 3.0, 3.0 };

				// 점 크기 및 색상 (R, G, B) // Point size and colors (R, G, B)
				const float f32PointSize = 5.0f;
				TPoint3<byte> tpColorData = new TPoint3<byte>(0, 255, 255);    // CYAN
				TPoint3<byte> tpColorOutlier = new TPoint3<byte>(255, 0, 0);   // RED
				TPoint3<byte> tpColorInside = new TPoint3<byte>(0, 255, 0);    // LIME
				TPoint3<byte> tpColorOutside = new TPoint3<byte>(0, 0, 255);   // BLUE

				for(int i = 0; i < 2; ++i)
				{
					// 각 축의 다항식 계수를 얻는다. (x(t), y(t), z(t), 내림차순)
					// Get polynomial coefficients of each axis. (x(t), y(t), z(t), descending order)
					List<double> listCoefX = new List<double>();
					List<double> listCoefY = new List<double>();
					List<double> listCoefZ = new List<double>();
					List<long> listOutlierIndices = new List<long>();

					if((res = leastSquares3D.GetParametricPoly(i32Degree, ref listCoefX, ref listCoefY, ref listCoefZ, ECoefficientOrder.Descending, arrI64OutlierThresholdCount[i], arrF64OutlierThreshold[i], ref listOutlierIndices)).IsFail())
					{
						ErrorPrint(res, "Failed to calculate.\n");
						break;
					}

					// 매개변수 t 에 대한 결과 점들을 얻는다. // Get result points for parameter t
					CFLPointArray3<double> flpaResult = new CFLPointArray3<double>();

					if((res = leastSquares3D.GetParametricPoints(i32Degree, listTargetT, ref flpaResult, arrI64OutlierThresholdCount[i], arrF64OutlierThreshold[i])).IsFail())
					{
						ErrorPrint(res, "Failed to calculate.\n");
						break;
					}

					// 결과 점 : 데이터 범위(t = 0 ~ 1) 안은 LIME, 밖은 BLUE
					// Result points : LIME inside the data range (t = 0 to 1), BLUE outside
					List<TPoint3<float>> listResultVertices = new List<TPoint3<float>>();
					List<TPoint3<byte>> listResultColors = new List<TPoint3<byte>>();

					for(long j = 0; j < flpaResult.GetCount(); ++j)
					{
						CFLPoint3<double> flpResult = flpaResult.GetAt(j);
						double f64T = listTargetT[(int)j];
						bool bInside = f64T >= 0.0 && f64T <= 1.0;

						listResultVertices.Add(new TPoint3<float>((float)flpResult.x, (float)flpResult.y, (float)flpResult.z));
						listResultColors.Add(bInside ? tpColorInside : tpColorOutside);
					}

					// 데이터 점 : CYAN, 제외된 Outlier 점 : RED // Data points : CYAN, Excluded outlier points : RED
					List<TPoint3<float>> listDataVertices = new List<TPoint3<float>>();
					List<TPoint3<byte>> listDataColors = new List<TPoint3<byte>>();

					for(long j = 0; j < flpaData.GetCount(); ++j)
					{
						CFLPoint3<double> flpData = flpaData.GetAt(j);

						listDataVertices.Add(new TPoint3<float>((float)flpData.x, (float)flpData.y, (float)flpData.z));
						listDataColors.Add(tpColorData);
					}

					foreach(long i64Index in listOutlierIndices)
						listDataColors[(int)i64Index] = tpColorOutlier;

					// 점 크기를 지정하여 3D 뷰에 점들을 추가 // Add points to the 3D view with the specified point size
					CGUIView3DObject view3DObjectResult = new CGUIView3DObject(new CFL3DObject(listResultVertices, listResultColors));
					view3DObjectResult.SetPointSize(f32PointSize);

					CGUIView3DObject view3DObjectData = new CGUIView3DObject(new CFL3DObject(listDataVertices, listDataColors));
					view3DObjectData.SetPointSize(f32PointSize);

					if((res = view3D[i].PushObject(view3DObjectResult)).IsFail() || (res = view3D[i].PushObject(view3DObjectData)).IsFail())
					{
						ErrorPrint(res, "Failed to push the object.\n");
						break;
					}

					// 화면에 출력하기 위해 View 에서 레이어 0번을 얻어옴 // Obtain layer number 0 from view for display
					// 이 객체는 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to a view and does not need to be released separately
					CGUIView3DLayer layer = view3D[i].GetLayer(0);

					string strParameter;

					if(arrI64OutlierThresholdCount[i] != 0)
						strParameter = string.Format("Outlier Threshold Count : {0}, Outlier Threshold : {1:F3}", arrI64OutlierThresholdCount[i], arrF64OutlierThreshold[i]);
					else
						strParameter = string.Format("Outlier Threshold Count : {0}", arrI64OutlierThresholdCount[i]);

					string strOutlierCount = string.Format("Outlier Count : {0}", listOutlierIndices.Count);

					layer.DrawTextCanvas(new CFLPoint<double>(0, 0), arrStrViewName[i], EColor.YELLOW, EColor.BLACK, 20);
					layer.DrawTextCanvas(new CFLPoint<double>(0, 30), strParameter, EColor.YELLOW, EColor.BLACK, 13);
					layer.DrawTextCanvas(new CFLPoint<double>(0, 50), strOutlierCount, EColor.YELLOW, EColor.BLACK, 13);
					layer.DrawTextCanvas(new CFLPoint<double>(0, 70), "Data : CYAN, Outlier : RED", EColor.YELLOW, EColor.BLACK, 13);
					layer.DrawTextCanvas(new CFLPoint<double>(0, 90), "Result (t = 0 ~ 1) : LIME, Result (extrapolated) : BLUE", EColor.YELLOW, EColor.BLACK, 13);

					// Console 출력 // Console output
					Console.WriteLine("[{0}]", arrStrViewName[i]);
					Console.WriteLine(strParameter);
					Console.WriteLine(strOutlierCount);

					string[] arrStrAxis = { "x", "y", "z" };
					List<double>[] arrListCoef = { listCoefX, listCoefY, listCoefZ };

					for(int k = 0; k < 3; ++k)
					{
						List<double> listCoef = arrListCoef[k];
						Console.WriteLine("{0}(t) = {1:F6}t^3 {2:+0.000000;-0.000000}t^2 {3:+0.000000;-0.000000}t {4:+0.000000;-0.000000}", arrStrAxis[k], listCoef[0], listCoef[1], listCoef[2], listCoef[3]);
					}

					Console.WriteLine();
				}

				if(res.IsFail())
					break;

				// 두 뷰의 시점을 맞춤 // Fit the viewpoint of the two views
				view3D[0].ZoomFit();
				view3D[0].SynchronizePointOfView(ref view3D[1]);

				// 두 뷰를 하나의 창으로 취급 // Treat the two views as one window
				view3D[0].SynchronizeWindow(ref view3D[1]);

				for(int i = 0; i < 2; ++i)
					view3D[i].Invalidate();

				// 3D 뷰가 하나라도 꺼지면 종료로 간주 // Consider closed when any of the 3D views are turned off
				while(view3D[0].IsAvailable() && view3D[1].IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

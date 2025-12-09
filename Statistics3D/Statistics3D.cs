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
	class Statistics3D
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

			// 3D 객체 선언 // Declare 3D object
			CFL3DObject floSource = new CFL3DObject();

			// 3D 뷰 선언 // Declare 3D view
			CGUIView3D view3DSrc = new CGUIView3D();

			do
			{
				// 수행 결과 객체 선언 // Declare execution result object
				CResult res = new CResult(EResult.UnknownError);

				// Source 3D Object 로드 // Load Source 3D Object
				if((res = floSource.Load("../../ExampleImages/Statistics3D/Sphere.ply")).IsFail())
				{
					ErrorPrint(res, "Failed to load the 3D object file.\n");
					break;
				}

				// Source 3D 뷰 생성 // Create Source 3D view
				if((res = view3DSrc.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// Statistics 3D 객체 생성 // Create Statistics 3D object
				CStatistics3D statistics3D = new CStatistics3D();

				// Source 3D Object 설정 // Set Source 3D Object
				if((res = statistics3D.SetSourceObject(ref floSource)).IsFail())
				{
					ErrorPrint(res, "Failed to set Source 3D Object.\n");
					break;
				}

				// 사전 계산 여부 설정 // Set pre calculated value hold flag
				if((res = statistics3D.EnablePreCalculatedHold(true)).IsFail())
				{
					ErrorPrint(res, "Failed to set pre calculated flag.\n");
					break;
				}

				// 위치 데이터 불러오기 // Get position data
				TPoint3<double> tpPositionMin = new TPoint3<double>();
				TPoint3<double> tpPositionMax = new TPoint3<double>();
				TPoint3<double> tpPositionSum = new TPoint3<double>();
				TPoint3<double> tpPositionSumOfSquares = new TPoint3<double>();
				TPoint3<double> tpPositionMean = new TPoint3<double>();
				TPoint3<double> tpPositionMedian = new TPoint3<double>();
				TPoint3<double> tpPositionVariance = new TPoint3<double>();
				TPoint3<double> tpPositionStandardDeviation = new TPoint3<double>();
				TPoint3<double> tpPositionCoefficientOfVariance = new TPoint3<double>();
				TPoint3<double> tpPositionLowerQuartile = new TPoint3<double>();
				TPoint3<double> tpPositionUpperQuartile = new TPoint3<double>();

				double f64PositionCovarianceXY = 0;
				double f64PositionCovarianceXZ = 0;
				double f64PositionCovarianceYZ = 0;

				double f64PositionCorrelationCoefficientXY = 0;
				double f64PositionCorrelationCoefficientXZ = 0;
				double f64PositionCorrelationCoefficientYZ = 0;

				if((res = statistics3D.GetPointPositionMin(ref tpPositionMin)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's min");
					break;
				}

				if((res = statistics3D.GetPointPositionMax(ref tpPositionMax)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's max");
					break;
				}

				if((res = statistics3D.GetPointPositionSum(ref tpPositionSum)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's sum");
					break;
				}

				if((res = statistics3D.GetPointPositionSumOfSquares(ref tpPositionSumOfSquares)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's sum of squares");
					break;
				}

				if((res = statistics3D.GetPointPositionMean(ref tpPositionMean)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's mean");
					break;
				}

				if((res = statistics3D.GetPointPositionMedian(ref tpPositionMedian)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's median");
					break;
				}

				if((res = statistics3D.GetPointPositionVariance(ref tpPositionVariance)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's variance");
					break;
				}

				if((res = statistics3D.GetPointPositionStandardDeviation(ref tpPositionStandardDeviation)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's standard deviation");
					break;
				}

				if((res = statistics3D.GetPointPositionCoefficientOfVariance(ref tpPositionCoefficientOfVariance)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's coefficient of variance");
					break;
				}

				if((res = statistics3D.GetPointPositionLowerQuartile(ref tpPositionLowerQuartile)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's lower quartile");
					break;
				}

				if((res = statistics3D.GetPointPositionUpperQuartile(ref tpPositionUpperQuartile)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's upper quartile");
					break;
				}

				statistics3D.SetCorrelatedPointPosition(CStatistics3D.EPointPosition.X, CStatistics3D.EPointPosition.Y);

				if((res = statistics3D.GetPointPositionCovariance(ref f64PositionCovarianceXY)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's covariance");
					break;
				}

				statistics3D.SetCorrelatedPointPosition(CStatistics3D.EPointPosition.X, CStatistics3D.EPointPosition.Z);

				if((res = statistics3D.GetPointPositionCovariance(ref f64PositionCovarianceXZ)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's covariance");
					break;
				}

				statistics3D.SetCorrelatedPointPosition(CStatistics3D.EPointPosition.Y, CStatistics3D.EPointPosition.Z);

				if((res = statistics3D.GetPointPositionCovariance(ref f64PositionCovarianceYZ)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's covariance");
					break;
				}

				statistics3D.SetCorrelatedPointPosition(CStatistics3D.EPointPosition.X, CStatistics3D.EPointPosition.Y);

				if((res = statistics3D.GetPointPositionCorrelationCoefficient(ref f64PositionCorrelationCoefficientXY)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's correlation coefficient");
					break;
				}

				statistics3D.SetCorrelatedPointPosition(CStatistics3D.EPointPosition.X, CStatistics3D.EPointPosition.Z);

				if((res = statistics3D.GetPointPositionCorrelationCoefficient(ref f64PositionCorrelationCoefficientXZ)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's correlation coefficient");
					break;
				}

				statistics3D.SetCorrelatedPointPosition(CStatistics3D.EPointPosition.Y, CStatistics3D.EPointPosition.Z);

				if((res = statistics3D.GetPointPositionCorrelationCoefficient(ref f64PositionCorrelationCoefficientYZ)).IsFail())
				{
					ErrorPrint(res, "Failed to get point position's correlation coefficient");
					break;
				}

				// 색 데이터 불러오기 // Get color data
				CMultiVar<double> mvColorMin = new CMultiVar<double>();
				CMultiVar<double> mvColorMax = new CMultiVar<double>();
				CMultiVar<double> mvColorSum = new CMultiVar<double>();
				CMultiVar<double> mvColorSumOfSquares = new CMultiVar<double>();
				CMultiVar<double> mvColorMean = new CMultiVar<double>();
				CMultiVar<double> mvColorMedian = new CMultiVar<double>();
				CMultiVar<double> mvColorVariance = new CMultiVar<double>();
				CMultiVar<double> mvColorStandardDeviation = new CMultiVar<double>();
				CMultiVar<double> mvColorCoefficientOfVariance = new CMultiVar<double>();
				CMultiVar<double> mvColorLowerQuartile = new CMultiVar<double>();
				CMultiVar<double> mvColorUpperQuartile = new CMultiVar<double>();

				double f64ColorCovarianceBG = 0;
				double f64ColorCovarianceBR = 0;
				double f64ColorCovarianceGR = 0;

				double f64ColorCorrelationCoefficientBG = 0;
				double f64ColorCorrelationCoefficientBR = 0;
				double f64ColorCorrelationCoefficientGR = 0;

				if((res = statistics3D.GetPointColorMin(ref mvColorMin)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's min");
					break;
				}

				if((res = statistics3D.GetPointColorMax(ref mvColorMax)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's max");
					break;
				}

				if((res = statistics3D.GetPointColorSum(ref mvColorSum)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's sum");
					break;
				}

				if((res = statistics3D.GetPointColorSumOfSquares(ref mvColorSumOfSquares)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's sum of squares");
					break;
				}

				if((res = statistics3D.GetPointColorMean(ref mvColorMean)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's mean");
					break;
				}

				if((res = statistics3D.GetPointColorMedian(ref mvColorMedian)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's median");
					break;
				}

				if((res = statistics3D.GetPointColorVariance(ref mvColorVariance)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's variance");
					break;
				}

				if((res = statistics3D.GetPointColorStandardDeviation(ref mvColorStandardDeviation)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's standard deviation");
					break;
				}

				if((res = statistics3D.GetPointColorCoefficientOfVariance(ref mvColorCoefficientOfVariance)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's coefficient of variance");
					break;
				}

				if((res = statistics3D.GetPointColorLowerQuartile(ref mvColorLowerQuartile)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's lower quartile");
					break;
				}

				if((res = statistics3D.GetPointColorUpperQuartile(ref mvColorUpperQuartile)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's upper quartile");
					break;
				}

				statistics3D.SetCorrelatedPointColor(CStatistics3D.EPointColor.B, CStatistics3D.EPointColor.G);

				if((res = statistics3D.GetPointColorCovariance(ref f64ColorCovarianceBG)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's covariance");
					break;
				}

				statistics3D.SetCorrelatedPointColor(CStatistics3D.EPointColor.B, CStatistics3D.EPointColor.R);

				if((res = statistics3D.GetPointColorCovariance(ref f64ColorCovarianceBR)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's covariance");
					break;
				}

				statistics3D.SetCorrelatedPointColor(CStatistics3D.EPointColor.G, CStatistics3D.EPointColor.R);

				if((res = statistics3D.GetPointColorCovariance(ref f64ColorCovarianceGR)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's covariance");
					break;
				}

				statistics3D.SetCorrelatedPointColor(CStatistics3D.EPointColor.B, CStatistics3D.EPointColor.G);

				if((res = statistics3D.GetPointColorCorrelationCoefficient(ref f64ColorCorrelationCoefficientBG)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's correlation coefficient");
					break;
				}

				statistics3D.SetCorrelatedPointColor(CStatistics3D.EPointColor.B, CStatistics3D.EPointColor.R);

				if((res = statistics3D.GetPointColorCorrelationCoefficient(ref f64ColorCorrelationCoefficientBR)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's correlation coefficient");
					break;
				}

				statistics3D.SetCorrelatedPointColor(CStatistics3D.EPointColor.G, CStatistics3D.EPointColor.R);

				if((res = statistics3D.GetPointColorCorrelationCoefficient(ref f64ColorCorrelationCoefficientGR)).IsFail())
				{
					ErrorPrint(res, "Failed to get point color's correlation coefficient");
					break;
				}

				// 면 데이터 불러오기 // Get face data
				double f64SurfaceArea = statistics3D.GetSurfaceArea();

				// 콘솔에 데이터 출력 // Print data to console
				Console.Write(" < Point Position Data >\n");

				Console.Write("Min ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionMin.x, tpPositionMin.y, tpPositionMin.z);
				Console.Write("Max ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionMax.x, tpPositionMax.y, tpPositionMax.z);
				Console.Write("Sum ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionSum.x, tpPositionSum.y, tpPositionSum.z);
				Console.Write("Sum Of Squares ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionSumOfSquares.x, tpPositionSumOfSquares.y, tpPositionSumOfSquares.z);
				Console.Write("Mean ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionMean.x, tpPositionMean.y, tpPositionMean.z);
				Console.Write("Median ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionMedian.x, tpPositionMedian.y, tpPositionMedian.z);
				Console.Write("Variance ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionVariance.x, tpPositionVariance.y, tpPositionVariance.z);
				Console.Write("Standard Deviation ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionStandardDeviation.x, tpPositionStandardDeviation.y, tpPositionStandardDeviation.z);
				Console.Write("Coefficient Of Variance ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionCoefficientOfVariance.x, tpPositionCoefficientOfVariance.y, tpPositionCoefficientOfVariance.z);
				Console.Write("Lower Quartile ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionLowerQuartile.x, tpPositionLowerQuartile.y, tpPositionLowerQuartile.z);
				Console.Write("Upper Quartile ->\tX: {0:N7}\tY: {1:N7}\tZ: {2:N7}\n", tpPositionUpperQuartile.x, tpPositionUpperQuartile.y, tpPositionUpperQuartile.z);

				Console.Write("Covariance ->\tXY: {0:N7}\tXZ: {1:N7}\tYZ: {2:N7}\n", f64PositionCovarianceXY, f64PositionCovarianceXZ, f64PositionCovarianceYZ);
				Console.Write("Correlation Coefficient ->\tXY: {0:N7}\tXZ: {1:N7}\tYZ: {2:N7}\n", f64PositionCorrelationCoefficientXY, f64PositionCorrelationCoefficientXZ, f64PositionCorrelationCoefficientYZ);

				Console.Write("\n");

				Console.Write(" < Point Color Data >\n");

				Console.Write("Min ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorMin.GetAt(0), mvColorMin.GetAt(1), mvColorMin.GetAt(2));
				Console.Write("Max ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorMax.GetAt(0), mvColorMax.GetAt(1), mvColorMax.GetAt(2));
				Console.Write("Sum ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorSum.GetAt(0), mvColorSum.GetAt(1), mvColorSum.GetAt(2));
				Console.Write("Sum Of Squares ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorSumOfSquares.GetAt(0), mvColorSumOfSquares.GetAt(1), mvColorSumOfSquares.GetAt(2));
				Console.Write("Mean ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorMean.GetAt(0), mvColorMean.GetAt(1), mvColorMean.GetAt(2));
				Console.Write("Median ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorMedian.GetAt(0), mvColorMedian.GetAt(1), mvColorMedian.GetAt(2));
				Console.Write("Variance ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorVariance.GetAt(0), mvColorVariance.GetAt(1), mvColorVariance.GetAt(2));
				Console.Write("Standard Deviation ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorStandardDeviation.GetAt(0), mvColorStandardDeviation.GetAt(1), mvColorStandardDeviation.GetAt(2));
				Console.Write("Coefficient Of Variance ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorCoefficientOfVariance.GetAt(0), mvColorCoefficientOfVariance.GetAt(1), mvColorCoefficientOfVariance.GetAt(2));
				Console.Write("Lower Quartile ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorLowerQuartile.GetAt(0), mvColorLowerQuartile.GetAt(1), mvColorLowerQuartile.GetAt(2));
				Console.Write("Upper Quartile ->\tB: {0:N7}\tG: {1:N7}\tR: {2:N7}\n", mvColorUpperQuartile.GetAt(0), mvColorUpperQuartile.GetAt(1), mvColorUpperQuartile.GetAt(2));

				Console.Write("Covariance ->\tBG: {0:N7}\tBR: {1:N7}\tGR: {2:N7}\n", f64ColorCovarianceBG, f64ColorCovarianceBR, f64ColorCovarianceGR);
				Console.Write("Correlation Coefficient ->\tBG: {0:N7}\tBR: {1:N7}\tGR: {2:N7}\n", f64ColorCorrelationCoefficientBG, f64ColorCorrelationCoefficientBR, f64ColorCorrelationCoefficientGR);

				Console.Write("\n");

				Console.Write(" < Face Data >\n");

				Console.Write("Surface Area: {0:N7}\n", f64SurfaceArea);

				Console.Write("\n");


				// 화면에 출력하기 위해 3D 뷰에서 레이어 0번을 얻어옴 // Obtain layer 0 number from 3D view for display
				// 이 객체는 3D 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an 3D view and does not need to be released
				CGUIView3DLayer layer3DSource = view3DSrc.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear figures drawn on existing layer
				layer3DSource.Clear();

				// 3D 뷰 정보 표시 // Display 3D view information
				if((res = layer3DSource.DrawTextCanvas(new CFLPoint<double>(0, 0), "Source 3D Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 입력 3D 객체 출력 // Print input 3D Object
				if((res = view3DSrc.PushObject(floSource)).IsFail())
				{
					ErrorPrint(res, "Failed to display the 3D Object.\n");
					break;
				}

				// 새로 생성한 3D Object를 가지는 뷰 Zoom Fit 실행 // Activate Zoom Fit for view with newly created 3D object
				if((res = view3DSrc.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit 3D view.\n");
					break;
				}

				// 3D 뷰를 갱신 // Update 3D view
				view3DSrc.Invalidate(true);

				// 뷰가 닫히기 전까지 종료하지 않고 대기 // Wait until a view is closed before exiting
				while(view3DSrc.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

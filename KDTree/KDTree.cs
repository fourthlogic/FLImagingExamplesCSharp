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

namespace FLImagingExamplesCSharp
{
	class KDTree
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

			CResult res;

			do
			{
				// ------------------------------------------------------------------
				// 1단계: 데이터 준비 및 KDTree 구축 (Build)
				// Step 1: Prepare Data and Build KDTree
				// ------------------------------------------------------------------
				Console.WriteLine("[Step 1] Prepare Data and Build KDTree");

				List<TPoint<double>> listVertices = new List<TPoint<double>>
				{
					new TPoint<double>(0.0, 0.0),   // Index 0
                    new TPoint<double>(1.0, 1.0),   // Index 1
                    new TPoint<double>(2.0, 2.0),   // Index 2
                    new TPoint<double>(3.0, 1.0),   // Index 3
                    new TPoint<double>(5.0, 5.0),   // Index 4
                    new TPoint<double>(10.0, 10.0)  // Index 5
                };

				// 입력받은 정점 목록 및 인덱스 출력 // Output the input list of vertices and indices
				Console.WriteLine(" - Input Vertices:");

				for(int i = 0; i < listVertices.Count; ++i)
					Console.WriteLine($"   Index {i} : ({listVertices[i].x:F1}, {listVertices[i].y:F1})");

				// CKDTree 선언 (2D Point, double 타입) // Declare CKDTree (2D Point, double type)
				CKDTree<TPoint<double>> kdtree = new CKDTree<TPoint<double>>();

				// 트리 구축 // Build tree
				if((res = kdtree.Build(listVertices)).IsFail())
				{
					ErrorPrint(res, "Failed to build KDTree.\n");
					break;
				}

				Console.WriteLine($" - The number of elements in the constructed data : {kdtree.GetCount()}\n");


				// ------------------------------------------------------------------
				// 2단계: 단일 최근접 정점 탐색 (GetNearestPointAndIndex)
				// Step 2: Single Nearest Neighbor Search
				// ------------------------------------------------------------------
				Console.WriteLine("[Step 2] Single Nearest Neighbor Search");

				TPoint<double> tpQuery1 = new TPoint<double>(1.2, 0.9);
				Console.WriteLine($" - Target Query: ({tpQuery1.x:F1}, {tpQuery1.y:F1})");

				// 좌표와 인덱스를 동시에 구함 // Get both coordinates and index
				var pairNearestResult = kdtree.GetNearestPointAndIndex(tpQuery1);

				Console.WriteLine($" - Nearest Point: ({pairNearestResult.Key.x:F1}, {pairNearestResult.Key.y:F1})");
				Console.WriteLine($" - Nearest Index: {pairNearestResult.Value}\n");


				// ------------------------------------------------------------------
				// 3단계: K-최근접 정점 탐색 (GetNearestNeighborsPointsAndIndices)
				// Step 3: K-Nearest Neighbors Search
				// ------------------------------------------------------------------
				Console.WriteLine("[Step 3] K-Nearest Neighbors Search (K = 3)");

				TPoint<double> tpQuery2 = new TPoint<double>(1.5, 1.5);
				long i64K = 3;
				Console.WriteLine($" - Target Query: ({tpQuery2.x:F1}, {tpQuery2.y:F1})");

				List<TPoint<double>> listKNNPoints = new List<TPoint<double>>();
				List<ulong> listKNNIndices = new List<ulong>();

				kdtree.GetNearestNeighborsPointsAndIndices(tpQuery2, i64K, ref listKNNPoints, ref listKNNIndices);

				for(int i = 0; i < listKNNPoints.Count; ++i)
					Console.WriteLine($"   [{i + 1}] Index: {listKNNIndices[i]} -> ({listKNNPoints[i].x:F1}, {listKNNPoints[i].y:F1})");

				Console.WriteLine();


				// ------------------------------------------------------------------
				// 4단계: 영역/범위 탐색 (GetPointsAndIndicesInRange)
				// Step 4: Bounding Box Range Search
				// ------------------------------------------------------------------
				Console.WriteLine("[Step 4] Bounding Box Range Search");

				TPoint<double> tpLowerBound = new TPoint<double>(0.5, 0.5);
				TPoint<double> tpUpperBound = new TPoint<double>(3.5, 2.5);
				Console.WriteLine($" - Range Lower Bound: ({tpLowerBound.x:F1}, {tpLowerBound.y:F1})");
				Console.WriteLine($" - Range Upper Bound: ({tpUpperBound.x:F1}, {tpUpperBound.y:F1})");

				List<TPoint<double>> listRangePoints = new List<TPoint<double>>();
				List<ulong> listRangeIndices = new List<ulong>();

				// 모든 매칭 점을 찾으려면 i64Count에 -1 전달 // Pass -1 to i64Count to find all matching points
				kdtree.GetPointsAndIndicesInRange(tpLowerBound, tpUpperBound, ref listRangePoints, ref listRangeIndices, -1);

				Console.WriteLine($" - Number of points found in range: {listRangePoints.Count}");

				for(int i = 0; i < listRangePoints.Count; ++i)
					Console.WriteLine($"   - Index: {listRangeIndices[i]} -> ({listRangePoints[i].x:F1}, {listRangePoints[i].y:F1})");

				Console.WriteLine();


				// ------------------------------------------------------------------
				// 5단계: 반경 탐색 (GetPointsAndIndicesInRadius)
				// Step 5: Radius Search
				// ------------------------------------------------------------------
				Console.WriteLine("[Step 5] Radius Search");

				TPoint<double> tpCenter = new TPoint<double>(0.0, 0.0);
				double f64Radius = 3.0;
				Console.WriteLine($" - Center Point: ({tpCenter.x:F1}, {tpCenter.y:F1})");
				Console.WriteLine($" - Radius: {f64Radius:F1}");

				List<TPoint<double>> listRadiusPoints = new List<TPoint<double>>();
				List<ulong> listRadiusIndices = new List<ulong>();

				kdtree.GetPointsAndIndicesInRadius(tpCenter, f64Radius, ref listRadiusPoints, ref listRadiusIndices, -1);

				Console.WriteLine($" - Number of points found in radius: {listRadiusPoints.Count}");

				for(int i = 0; i < listRadiusPoints.Count; ++i)
					Console.WriteLine($"   - Index: {listRadiusIndices[i]} -> ({listRadiusPoints[i].x:F1}, {listRadiusPoints[i].y:F1})");

				Console.WriteLine();


				// ------------------------------------------------------------------
				// 6단계: 기하 연산 (OperateAdd)
				// Step 6: Geometric Operation (OperateAdd)
				// ------------------------------------------------------------------
				Console.WriteLine("[Step 6] Translate All Nodes (OperateAdd)");

				TPoint<double> tpOffset = new TPoint<double>(10.0, 10.0);
				Console.WriteLine($" - Offset Vector: ({tpOffset.x:F1}, {tpOffset.y:F1})");

				// 모든 점에 (10, 10) 이동 적용 // Apply (10, 10) offset to all points
				kdtree.OperateAdd(tpOffset);

				// 이동 후 동일한 Query 점(1.2, 0.9)으로 다시 탐색해 확인 // Re-query near the same query point (1.2, 0.9) to check result
				Console.WriteLine($" - Re-querying near: ({tpQuery1.x:F1}, {tpQuery1.y:F1})");
				var pairShiftedResult = kdtree.GetNearestPointAndIndex(tpQuery1);

				Console.WriteLine($" - Shifted Nearest Point: ({pairShiftedResult.Key.x:F1}, {pairShiftedResult.Key.y:F1})");
				Console.WriteLine($" - Index: {pairShiftedResult.Value}");

				Console.WriteLine("\n========================================\n");

				Console.Write("Press Enter to exit...");
				Console.ReadLine();
			}
			while(false);
		}
	}
}

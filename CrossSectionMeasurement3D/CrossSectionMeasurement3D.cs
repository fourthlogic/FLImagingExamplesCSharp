using System;
using System.Collections.Generic;
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
	class CrossSectionMeasurement3D
	{
		public static void ErrorPrint(CResult cResult, string str)
		{
			if(str.Length > 1)
				Console.WriteLine(str);

			Console.WriteLine("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());
			Console.WriteLine("\n");
			Console.ReadKey();
		}

		public static void AddSegment(List<TPoint3<float>> listVertices, List<int> listSegmentIndices, List<int> listSegmentElementCount, List<TPoint3<byte>> listSegmentColors, TPoint3<float> tp3Start, TPoint3<float> tp3End, TPoint3<byte> tp3Color)
		{
			int i32VertexIndex = listVertices.Count;
			listVertices.Add(tp3Start);
			listVertices.Add(tp3End);
			listSegmentIndices.Add(i32VertexIndex);
			listSegmentIndices.Add(i32VertexIndex + 1);
			listSegmentElementCount.Add(2);
			listSegmentColors.Add(tp3Color);
		}

		public static TPoint3<byte> ColorToPoint3(EColor color)
		{
			int i32Color = (int)color;
			return new TPoint3<byte>((byte)(i32Color & 0xff), (byte)((i32Color >> 8) & 0xff), (byte)((i32Color >> 16) & 0xff));
		}

		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			CFL3DObject floSourceObject = new CFL3DObject();
			CFL3DObject floResultObject = new CFL3DObject();
			CGUIView3D view3D = new CGUIView3D();
			CResult eResult = new CResult();

			do
			{
				// 3D Object 로드 // Load the 3D object
				if((eResult = floSourceObject.Load("../../ExampleImages/SurfaceMatch3D/Car example.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// 3D 뷰 생성 // Create the 3D view
				if((eResult = view3D.Create(0, 0, 1024, 768)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the 3D view.\n");
					break;
				}

				if((eResult = view3D.PushObject(floSourceObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// CrossSectionMeasurement3D 객체 생성 // Create CrossSectionMeasurement3D object
				CCrossSectionMeasurement3D crossSectionMeasurement3D = new CCrossSectionMeasurement3D();
				crossSectionMeasurement3D.SetSourceObject(ref floSourceObject);
				TPoint3<float> tp3CrossSectionCenter = new TPoint3<float>(-122.0f, -48.0f, -342.0f);
				TPoint3<float> tp3CrossSectionNormal = new TPoint3<float>(0.0f, 1.0f, 1.0f);
				crossSectionMeasurement3D.SetCrossSection(tp3CrossSectionCenter, tp3CrossSectionNormal);
				crossSectionMeasurement3D.SetMeasurementPlane(CCrossSectionMeasurement3D.EMeasurementDirectionFromSection.Vertical);
				crossSectionMeasurement3D.SetSectionMeasurementMode(CCrossSectionMeasurement3D.ESectionMeasurementMode.GlobalSpan);
				crossSectionMeasurement3D.SetMeasurementInterval(3.0f);
				crossSectionMeasurement3D.SetMinLength(0.1f);
				crossSectionMeasurement3D.SetMaxLength(1e+09f);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = crossSectionMeasurement3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Cross Section Measurement 3D.");
					break;
				}

				List<List<TPoint3<float>>> listIntersectionLines = new List<List<TPoint3<float>>>();
				List<List<TPoint3<float>>> listMeasurementPoints = new List<List<TPoint3<float>>>();
				List<List<float>> listMeasurementDistances = new List<List<float>>();

				crossSectionMeasurement3D.GetResultIntersectionLines(ref listIntersectionLines);
				crossSectionMeasurement3D.GetResultMeasurementPoints(ref listMeasurementPoints);
				crossSectionMeasurement3D.GetResultMeasurementDistances(ref listMeasurementDistances);

				List<TPoint3<float>> listVertices = new List<TPoint3<float>>();
				List<int> listSegmentIndices = new List<int>();
				List<int> listSegmentElementCount = new List<int>();
				List<TPoint3<byte>> listSegmentColors = new List<TPoint3<byte>>();
				TPoint3<byte> tp3IntersectionColor = ColorToPoint3(EColor.LIGHTGREEN);
				TPoint3<byte> tp3MeasurementColor = ColorToPoint3(EColor.CYAN);

				foreach(List<TPoint3<float>> listLine in listIntersectionLines)
				{
					for(int i = 0; i + 1 < listLine.Count; ++i)
						AddSegment(listVertices, listSegmentIndices, listSegmentElementCount, listSegmentColors, listLine[i], listLine[i + 1], tp3IntersectionColor);
				}

				int i32MeasurementCount = 0;
				foreach(List<TPoint3<float>> listPoints in listMeasurementPoints)
				{
					for(int i = 0; i + 1 < listPoints.Count; i += 2)
					{
						AddSegment(listVertices, listSegmentIndices, listSegmentElementCount, listSegmentColors, listPoints[i], listPoints[i + 1], tp3MeasurementColor);
						++i32MeasurementCount;
					}
				}

				floResultObject.SetVertices(listVertices);
				floResultObject.SetSegmentIndices(listSegmentIndices);
				floResultObject.SetSegmentElementCountInformation(listSegmentElementCount);
				floResultObject.SetSegmentColors(listSegmentColors);

				CGUIView3DObject viewResultObject = new CGUIView3DObject(floResultObject);
				viewResultObject.SetTopologyType(ETopologyType3D.Segment);

				if((eResult = view3D.PushObject(viewResultObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the result object.\n");
					break;
				}

				view3D.GetView3DObject(0).SetOpacity(0.5f);

				CGUIView3DLayer layer3D = view3D.GetLayer(0);
				layer3D.Clear();
				layer3D.DrawTextCanvas(new CFLPoint<double>(0, 0), "Cross Section Measurement 3D", EColor.YELLOW, EColor.BLACK, 20);
				layer3D.DrawTextCanvas(new CFLPoint<double>(0, 30), string.Format("Intersection Lines : {0}\nMeasurement Planes : {1}\nMeasurements : {2}", listIntersectionLines.Count, listMeasurementPoints.Count, i32MeasurementCount), EColor.YELLOW, EColor.BLACK, 15);
				for(int i = 0; i < listMeasurementPoints.Count && i < listMeasurementDistances.Count; ++i)
				{
					List<TPoint3<float>> listPoints = listMeasurementPoints[i];
					List<float> listDistances = listMeasurementDistances[i];

					for(int j = 0, d = 0; j + 1 < listPoints.Count && d < listDistances.Count; j += 2, ++d)
					{
						TPoint3<double> tp3Text = new TPoint3<double>((listPoints[j].x + listPoints[j + 1].x) * .5, (listPoints[j].y + listPoints[j + 1].y) * .5, (listPoints[j].z + listPoints[j + 1].z) * .5);
						layer3D.DrawText3D(tp3Text, string.Format("{0:F3}", listDistances[d]), EColor.DEEPPINK, EColor.BLACK, 10);
					}
				}

				view3D.ZoomFit();
				view3D.Invalidate(true);

				while(view3D.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

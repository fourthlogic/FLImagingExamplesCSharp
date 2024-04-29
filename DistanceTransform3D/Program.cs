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
using System.Diagnostics;
using System.Collections;

namespace MultiFocus
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


		public static void DrawResult(CGUIView3D pView3D, List<TPoint3<float>> pFlaPlyData, List<TPoint3<float>> arrResult, String strDirection)
		{
			float[,] arr2F32DataRange = new float[3, 2] {{ float.MaxValue, float.MinValue },{ float.MaxValue, float.MinValue},{
				float.MaxValue, float.MinValue} };

			for(int i = 0; i < arrResult.Count; ++i)
			{
				arr2F32DataRange[0, 0] = Math.Min(arr2F32DataRange[0, 0], arrResult[i].x);
				arr2F32DataRange[0, 1] = Math.Max(arr2F32DataRange[0, 1], arrResult[i].x);
				arr2F32DataRange[1, 0] = Math.Min(arr2F32DataRange[1, 0], arrResult[i].y);
				arr2F32DataRange[1, 1] = Math.Max(arr2F32DataRange[1, 1], arrResult[i].y);
				arr2F32DataRange[2, 0] = Math.Min(arr2F32DataRange[2, 0], arrResult[i].z);
				arr2F32DataRange[2, 1] = Math.Max(arr2F32DataRange[2, 1], arrResult[i].z);
			}

			String strRangeX = String.Format("X : [{0}, {1}]", arr2F32DataRange[0, 0], arr2F32DataRange[0, 1]);
			String strRangeY = String.Format("Y : [{0}, {1}]", arr2F32DataRange[1, 0], arr2F32DataRange[1, 1]);
			String strRangeZ = String.Format("Z : [{0}, {1}]", arr2F32DataRange[2, 0], arr2F32DataRange[2, 1]);

			pView3D.GetLayer(0).DrawTextCanvas(new CFLPoint<double>(10, 20), "Data Ranges", (EColor)8454143, EColor.BLACK, 13);
			pView3D.GetLayer(0).DrawTextCanvas(new CFLPoint<double>(10, 35), strRangeX, (EColor)8454143, EColor.BLACK, 13);
			pView3D.GetLayer(0).DrawTextCanvas(new CFLPoint<double>(10, 50), strRangeY, (EColor)8454016, EColor.BLACK, 13);
			pView3D.GetLayer(0).DrawTextCanvas(new CFLPoint<double>(10, 65), strRangeZ, (EColor)16744576, EColor.BLACK, 13);

			int i32SelectedAxis;

			if(strDirection == "Delta X")
				i32SelectedAxis = 0;
			else if(strDirection == "Delta Y")
				i32SelectedAxis = 1;
			else // dZ
				i32SelectedAxis = 2;

			List<TPoint3<float>> flaColors = new List<TPoint3<float>>();

			for(int i = 0; i < arrResult.Count; ++i)
			{
				float f32Intensity = (arrResult[i].z - arr2F32DataRange[i32SelectedAxis, 0]) / (arr2F32DataRange[i32SelectedAxis, 1] - arr2F32DataRange[i32SelectedAxis, 0]);

				if(f32Intensity < 0)
					f32Intensity = 0;
				if(f32Intensity > 1)
					f32Intensity = 1;

				float f32Temp;
				float f32Segment = (1.0f / 6.0f);

				float[] arrF32Color = new float[3];

				for(int j = 0; j < 3; ++j)
				{
					arrF32Color[j] = (f32Intensity - (j * 2 - 1) * f32Segment) / f32Segment;
					arrF32Color[j] = arrF32Color[j] > 1 ? 1 : arrF32Color[j];
					arrF32Color[j] = arrF32Color[j] < 0 ? 0 : arrF32Color[j];
					f32Temp = (f32Intensity - (j * 2 + 1) * f32Segment) / f32Segment;
					f32Temp = f32Temp > 1 ? 1 : f32Temp;
					f32Temp = f32Temp < 0 ? 0 : f32Temp;
					arrF32Color[j] -= f32Temp;
				}

				TPoint3<float> tpColor = new TPoint3<float>(arrF32Color[0], arrF32Color[1], arrF32Color[2]);

				flaColors.Add(tpColor);
			}

			CFL3DObject fl3DO = new CFL3DObject();
			fl3DO.Assign(pFlaPlyData, flaColors);

			pView3D.PushObject(fl3DO);
		}


		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();
			CFL3DObject fl3DObject = new CFL3DObject();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// Source 3D 이미지 뷰 생성 // Create the Source 3D image view
				if((res = view3DSrc.Create(400, 200, 1300, 800)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// Destination 3D 이미지 뷰 생성 // Create the destination 3D image view
				if((res = view3DDst.Create(400, 200, 1300, 800)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}


				fl3DObject.Load("../../ExampleImages/DistanceTransform3D/binary-vertex.ply");

				// Distance Transform 3D 객체 생성 // Create Distance Transform 3D object
				CDistanceTransform3D DistanceTransform3D = new CDistanceTransform3D();

				TPoint3<float> tpPosition = new TPoint3<float>(0.000000f, 0.000000f, 0.000000f);
				TPoint3<float> tpDirection = new TPoint3<float>(-0.100000f, 0.000000f, -1.000000f);
				TPoint3<float> tpUpVector = new TPoint3<float>(0.000000f, 1.000000f, 0.000000f);

				// Source 객체 설정 // Set the source object
				DistanceTransform3D.SetSourceObject(ref fl3DObject);
				// 카메라 위치 설정 // Set the camera position
				DistanceTransform3D.SetPosition(ref tpPosition);
				// 카메라 방향 설정 // Set the camera direction
				DistanceTransform3D.SetDirection(ref tpDirection);
				// 카메라 업 벡터 설정 // Set the camera up vector
				DistanceTransform3D.SetUpVector(ref tpUpVector);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = DistanceTransform3D.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute MultiFocus.\n");
					break;
				}

				List<TPoint3<float>> arrResult = new List<TPoint3<float>>();
				// 거리 결과 가져오기 // Get the distance
				res = DistanceTransform3D.GetResultDistanceAxis(out arrResult);

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DSrc.Clear();
				layer3DDst.Clear();

				// 거리 결과를 그려준다 // Draw distance result
				DrawResult(view3DDst, fl3DObject.GetVertices(), arrResult, "Delta Z");

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다. // With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				if((res = view3DDst.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit of the image view.\n");
					break;
				}

				view3DSrc.PushObject(fl3DObject);
				view3DSrc.ZoomFit();

				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layer3DSrc.DrawTextCanvas(flp, ("Source Object"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = layer3DDst.DrawTextCanvas(flp, ("Destination Object"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSrc.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSrc.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

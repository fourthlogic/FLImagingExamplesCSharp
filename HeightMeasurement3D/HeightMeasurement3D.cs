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
	class HeightMeasurement3D
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
			CFL3DObject floMeasurementObject = new CFL3DObject();
			CFL3DObject floReferenceObject = new CFL3DObject();

			// 3D 뷰 선언 // Declare 3D view	
			CGUIView3D view3DMeasurement = new CGUIView3D();
			CGUIView3D view3DReference = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// 3D Object 로드 // Load the 3D object
				if((eResult = floReferenceObject.Load("../../ExampleImages/HeightMeasurement3D/Source.fl3do")).IsFail() ||
					(eResult = floMeasurementObject.Load("../../ExampleImages/HeightMeasurement3D/Measurement.fl3do")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// 3D 뷰 생성 // Create the 3D view
				if((eResult = view3DReference.Create(0, 0, 512, 512)).IsFail() ||
					(eResult = view3DMeasurement.Create(512, 0, 1024, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the 3D view.\n");
					break;
				}

				if((eResult = view3DReference.PushObject(floReferenceObject)).IsFail() ||
					(eResult = view3DMeasurement.PushObject(floMeasurementObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				CHeightMeasurement3D heightMeasurement3D = new CHeightMeasurement3D();

				// Reference plane 설정 // Set the reference plane
				heightMeasurement3D.SetReferencePlane(ref floReferenceObject);
				// Measurement plane 설정 // Set the Measurement plane
				heightMeasurement3D.SetMeasurementPlane(ref floMeasurementObject);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = heightMeasurement3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Height Measurement 3D.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
				CGUIView3DLayer layer3DMeasurement = view3DMeasurement.GetLayer(0);
				CGUIView3DLayer layer3DReference = view3DReference.GetLayer(0);


				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				
				layer3DMeasurement.Clear();
				layer3DReference.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flpLeftTop = new CFLPoint<double>();
				CFLPoint<double> flpResultPosition = new CFLPoint<double>(0, 30);

				if((eResult = layer3DReference.DrawTextCanvas(flpLeftTop, "Reference Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layer3DMeasurement.DrawTextCanvas(flpLeftTop, "Measurement Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				// 결과 출력 // Display the results.
				
				float f32ResultDihedralAngle = heightMeasurement3D.GetResultDihedralAngle();
				float f32ResultDistance = heightMeasurement3D.GetResultDistance();
				float f32ResultMeasurementToReference = heightMeasurement3D.GetResultReferenceToMeasurementDistance();
				float f32ResultReferenceToMeasurement = heightMeasurement3D.GetResultMeasurementToReferenceDistance();
				TPoint3<float> tp3Increment = heightMeasurement3D.GetResultIncrement();

				string strResult = string.Format("Dihedral Angle : {0,6:0.000000}\nDistance : {1,6:0.000000}\nIncrement : {2,6:0.000000}, {3,6:0.000000}, {4,6:0.000000}\nMeasurement To Reference : {5,6:0.000000}\nReference To Measurement : {6,6:0.000000}", f32ResultDihedralAngle, f32ResultDistance, tp3Increment.x, tp3Increment.y, tp3Increment.z, f32ResultMeasurementToReference, f32ResultReferenceToMeasurement);

				if((eResult = layer3DMeasurement.DrawTextCanvas(flpResultPosition, strResult, EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				view3DMeasurement.ZoomFit();
				view3DReference.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DReference.Invalidate(true);
				view3DMeasurement.Invalidate(true);

				view3DMeasurement.SynchronizePointOfView(ref view3DReference);

				//이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DReference.IsAvailable() && view3DMeasurement.IsAvailable())
					CThreadUtilities.Sleep(1);

			}
			while(false);
		}
	}
}

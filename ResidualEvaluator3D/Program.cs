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

namespace ResidualEvaluator3D
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
			// 3D 객체 선언 // Declare 3D object
			CFL3DObject fl3DOReferenceObject = new CFL3DObject();
			CFL3DObject fl3DOTargetObject = new CFL3DObject();

			// 3D 뷰 선언 // Declare 3D view	
			CGUIView3D view3DTarget = new CGUIView3D();
			CGUIView3D view3DReference = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// Reference Object 로드 // Load the reference object
				if((eResult = fl3DOReferenceObject.Load("../../ExampleImages/ResidualEvaluator3D/ReferencePoints.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// Target Object 로드 // Load the target object
				if((eResult = fl3DOTargetObject.Load("../../ExampleImages/ResidualEvaluator3D/MeasuredPoints.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// Reference 3D 뷰 생성
				if((eResult = view3DReference.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Reference 3D view.\n");
					break;
				}

				// Target 3D 뷰 생성
				if((eResult = view3DTarget.Create(1124, 0, 1636, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Target 3D view.\n");
					break;
				}

				// Reference Object 3D 뷰 생성 // Create the reference object 3D view
				if((eResult = view3DReference.PushObject(fl3DOReferenceObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// Target Object 3D 뷰 생성 // Create the target object 3D view
				if((eResult = view3DTarget.PushObject(fl3DOTargetObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// ResidualEvaluator3D 객체 생성 // Create ResidualEvaluator3D object
				CResidualEvaluator3D ResidualEvaluator3D = new CResidualEvaluator3D();

				// Destination object 설정 // Set the destination object
				ResidualEvaluator3D.SetLearnObject(ref fl3DOReferenceObject);
				// Target object 설정 // Set the target object
				ResidualEvaluator3D.SetSourceObject(ref fl3DOTargetObject);
				// 최대 결과 개수 설정 // Set the max count of match result
				ResidualEvaluator3D.SetResidualType(CResidualEvaluator3D.EResidualType.RMSE);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = ResidualEvaluator3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Surface Match 3D.");
					break;
				}

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
                CGUIView3DLayer layer3DTarget = view3DTarget.GetLayer(0);
				CGUIView3DLayer layer3DReference = view3DReference.GetLayer(0);


				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DTarget.Clear();
				layer3DReference.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flp = new CFLPoint<double>();

				if((eResult = layer3DTarget.DrawTextCanvas(flp, "Target Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layer3DReference.DrawTextCanvas(flp, "Reference Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				double f64Residual = ResidualEvaluator3D.GetResidual();

				string strChannel = String.Format("Residual : {0}", f64Residual);

				TPoint3<float> tp3Min = new TPoint3<float>(), tp3Max = new TPoint3<float>();
				TPoint3<double> tp3Center = new TPoint3<double>();

				fl3DOTargetObject.GetBoundingBox(out tp3Min, out tp3Max);

				tp3Center.x = (tp3Min.x + tp3Max.x) / 2;
				tp3Center.y = (tp3Min.y + tp3Max.y) / 2;
				tp3Center.z = (tp3Min.z + tp3Max.z) / 2;

				// 추정된 포즈 행렬 및 score 출력
				if((eResult = layer3DTarget.DrawText3D(tp3Center, strChannel, EColor.YELLOW, EColor.BLACK, 15)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				view3DTarget.ZoomFit();
                view3DReference.ZoomFit();
                
				
				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DReference.Invalidate(true);
				view3DTarget.Invalidate(true);

				//이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DReference.IsAvailable() && view3DTarget.IsAvailable())
					Thread.Sleep(1);

			}
			while(false);
		}
	}
}

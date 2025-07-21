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

namespace Registration3D
{
	class Registration3D
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
			CFL3DObject floDestinationObject = new CFL3DObject();
			CFL3DObject floLearnObject = new CFL3DObject();
			CFL3DObject floSourceObject = new CFL3DObject();

			// 3D 뷰 선언 // Declare 3D view	
			CGUIView3D view3DDst = new CGUIView3D();
			CGUIView3D view3DLearn = new CGUIView3D();
			CGUIView3D view3DSource = new CGUIView3D();


			do
			{
				// 알고리즘 동작 결과 // Algorithm execution result
				CResult eResult = new CResult();

				// Learn Object 로드 // Load the Learn object
				if((eResult = floLearnObject.Load("../../ExampleImages/Registration3D/Left.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// Source Object 로드 // Load the Source object
				if((eResult = floSourceObject.Load("../../ExampleImages/Registration3D/Right.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// Learn 3D 뷰 생성
				if((eResult = view3DLearn.Create(0, 0, 500, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Learn 3D view.\n");
					break;
				}

				// Source 3D 뷰 생성
				if((eResult = view3DSource.Create(500, 0, 1000, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Source 3D view.\n");
					break;
				}

				// Destination 3D 뷰 생성
				if((eResult = view3DDst.Create(0, 500, 500, 1000)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Destination 3D view.\n");
					break;
				}

				// Learn Object 3D 뷰 생성 // Create the learn object 3D view
				if((eResult = view3DLearn.PushObject(floLearnObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// Source Object 3D 뷰 생성 // Create the source object 3D view
				if((eResult = view3DSource.PushObject(floSourceObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// registration3D 객체 생성 // Create registration3D object
				CRegistration3D registration3D = new CRegistration3D();

				// Learn Pivot 설정 // Set the learn pivot
				CFLPoint3<double> flpPivot = new CFLPoint3<double>(571.250000, 178.750000, 1169.250000);
				registration3D.SetLearnPivot(flpPivot);

				// Learn object 설정 // Set the learn object
				registration3D.SetLearnObject(ref floLearnObject);

				// Source object 설정 // Set the source object
				registration3D.SetSourceObject(ref floSourceObject);

				// 샘플링 거리 설정 // Set the sampling distance
				registration3D.SetSamplingDistance(0.03f);

				// 특징 기술자 타입 설정 // Set the feature descriptor type
				registration3D.SetDescriptorType(CRegistration3D.EDescriptorType.SHOT352);

				// 반경 자동 계산 여부 설정 // Set to auto-calculate radius
				registration3D.EnableNormalEstimationAutoRadius(false);
				registration3D.EnableSHOTLRFAutoRadius(false);
				registration3D.EnableSHOTAutoRadius(false);

				// 법선 벡터 추정 반경 설정 // Set to normal vector estimation radius
				registration3D.SetNormalEstimationRadius(30.000000f);

				// 지역 참조 프레임 추정 반경 설정 // Set to Local Reference Frame estimation radius
				registration3D.SetSHOTLRFRadius(70.000000f);

				// SHOT 반경 설정 // Set to SHOT radius
				registration3D.SetSHOTRadius(100.000000f);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = registration3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Registration 3D.");
					break;
				}

				view3DLearn.ZoomFit();
				view3DSource.ZoomFit();
				view3DDst.ZoomFit();

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);
				CGUIView3DLayer layer3DLearn = view3DLearn.GetLayer(0);
				CGUIView3DLayer layer3DSource = view3DSource.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DDst.Clear();
				layer3DLearn.Clear();
				layer3DSource.Clear();

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas 는 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic

				CFLPoint<double> flp = new CFLPoint<double>();

				if((eResult = layer3DLearn.DrawTextCanvas(flp, "Learn Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layer3DSource.DrawTextCanvas(flp, "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				if((eResult = layer3DDst.DrawTextCanvas(flp, "Destination Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(eResult, "Failed to draw text.\n");
					break;
				}

				TPoint3<byte> tp3LearnColor = new TPoint3<byte>(0, 255, 255);
				TPoint3<byte> tp3SourceColor = new TPoint3<byte>(255, 255, 0);
				CFL3DObject floMergedResult = new CFL3DObject();

				registration3D.GetMergedResult3DObject(true, true, tp3LearnColor, tp3SourceColor, ref floMergedResult);

				// 3D 오브젝트 뷰에 Destination 오브젝트 디스플레이
				if(view3DDst.PushObject(floMergedResult).IsFail())
				{
					ErrorPrint(eResult, "Failed to set object on the 3d view.\n");
					break;
				}

				view3DLearn.ZoomFit();
				view3DSource.ZoomFit();
				view3DDst.ZoomFit();

				view3DDst.SynchronizePointOfView(ref view3DLearn);
				view3DDst.SynchronizePointOfView(ref view3DSource);

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSource.Invalidate(true);
				view3DLearn.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림
				while(view3DLearn.IsAvailable() && view3DSource.IsAvailable() && view3DDst.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

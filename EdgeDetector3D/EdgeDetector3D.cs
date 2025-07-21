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

namespace EdgeDetector3D
{
	class EdgeDetector3D
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
			CFL3DObject fl3DODestinationObject = new CFL3DObject();
			CFL3DObject fl3DOSourceObject = new CFL3DObject();

			// 3D 뷰 선언 // Declare 3D view	
			CGUIView3D view3DDst = new CGUIView3D();
			CGUIView3D view3DSource = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// Source Object 로드 // Load the Source object
				if((eResult = fl3DOSourceObject.Load("../../ExampleImages/EdgeDetector3D/Sample.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// Source 3D 뷰 생성
				if((eResult = view3DSource.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Source 3D view.\n");
					break;
				}

				// Dst 3D 뷰 생성
				if((eResult = view3DDst.Create(1124, 0, 1636, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Destination 3D view.\n");
					break;
				}

				// Source Object 3D 뷰 생성 // Create the source object 3D view
				if((eResult = view3DSource.PushObject(fl3DOSourceObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// EdgeDetector3D 객체 생성 // Create EdgeDetector3D object
				CEdgeDetector3D EdgeDetector3D = new CEdgeDetector3D();

				// Destination object 설정 // Set the learn object
				EdgeDetector3D.SetDestinationObject(ref fl3DODestinationObject);
				// Source object 설정 // Set the source object
				EdgeDetector3D.SetSourceObject(ref fl3DOSourceObject);
				// Min score 설정 // Set the min score
				EdgeDetector3D.SetEdgeThreshold(0.01f);
				// 장면 샘플링 거리 설정 // Set the scene sampling distance
				EdgeDetector3D.SetNearestNeighborCount(32);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = EdgeDetector3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Edge Detector 3D.");
					break;
				}

                // 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
                // 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
                CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);
				CGUIView3DLayer layer3DSource = view3DSource.GetLayer(0);

				
				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DDst.Clear();
				layer3DSource.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				CFLPoint<double> flp = new CFLPoint<double>();

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

				// 3D 오브젝트 뷰에 결과 Object와 비교를 위한 Source 오브젝트 디스플레이
				if((eResult = view3DDst.PushObject((CFL3DObject)EdgeDetector3D.GetSourceObject())).IsFail())
				{
					ErrorPrint(eResult, "Failed to set object on the 3D View.\n");
					break;
				}

				// 3D 오브젝트 뷰에 결과 오브젝트 디스플레이
				if((eResult = view3DDst.PushObject((CFL3DObject)EdgeDetector3D.GetDestinationObject())).IsFail())
				{
					ErrorPrint(eResult, "Failed to set object on the 3D View.\n");
					break;
				}

				view3DDst.ZoomFit();
                view3DSource.ZoomFit();
                
				
				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSource.Invalidate(true);
				view3DDst.Invalidate(true);

				//이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSource.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);

			}
			while(false);
		}
	}
}

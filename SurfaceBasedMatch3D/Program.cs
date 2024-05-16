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

namespace SurfaceBasedMatch3D
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
			CFL3DObject fl3DOLearnObject = new CFL3DObject();
			CFL3DObject fl3DOSourceObject = new CFL3DObject();

			// 3D 뷰 선언 // Declare 3D view	
			CGUIView3D view3DDst = new CGUIView3D();
			CGUIView3D view3DLearn = new CGUIView3D();
			CGUIView3D view3DSource = new CGUIView3D();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult eResult = new CResult();

			do
			{
				// Learn Object 로드 // Load the learn object
				if((eResult = fl3DOLearnObject.Load("../../ExampleImages/SurfaceBasedMatch3D/Car wheel example.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the image file.\n");
					break;
				}

				// Source Object 로드 // Load the Source object
				if((eResult = fl3DOSourceObject.Load("../../ExampleImages/SurfaceBasedMatch3D/Car example.ply")).IsFail())
				{
					ErrorPrint(eResult, "Failed to load the object file.\n");
					break;
				}

				// Learn 3D 뷰 생성
				if((eResult = view3DLearn.Create(100, 0, 612, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Source 3D view.\n");
					break;
				}

				// Source 3D 뷰 생성
				if((eResult = view3DSource.Create(612, 0, 1124, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Learn 3D view.\n");
					break;
				}

				// Dst 3D 뷰 생성
				if((eResult = view3DDst.Create(1124, 0, 1636, 512)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the Dst 3D view.\n");
					break;
				}

				// Learn Object 3D 뷰 생성 // Create the learn object 3D view
				if((eResult = view3DLearn.PushObject(fl3DOLearnObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// Source Object 3D 뷰 생성 // Create the source object 3D view
				if((eResult = view3DSource.PushObject(fl3DOSourceObject)).IsFail())
				{
					ErrorPrint(eResult, "Failed to display the 3D object.\n");
					break;
				}

				// SurfaceBasedMatch3D 객체 생성 // Create SurfaceBasedMatch3D object
				CSurfaceBasedMatch3D SurfaceBasedMatch3D = new CSurfaceBasedMatch3D();

				// Learn object 설정 // Set the learn object
				SurfaceBasedMatch3D.SetLearnObject(ref fl3DOLearnObject);
				// Source object 설정 // Set the source object
				SurfaceBasedMatch3D.SetSourceObject(ref fl3DOSourceObject);
				// Min score 설정 // Set the min score
				SurfaceBasedMatch3D.SetMinScore(0.5);
				// 최대 결과 개수 설정 // Set the max count of match result
				SurfaceBasedMatch3D.SetMaxObject(4);
				// 학습 샘플링 거리 설정 // Set the learn sampling distance
				SurfaceBasedMatch3D.SetLearnSamplingDistance(0.03);
				// 장면 샘플링 거리 설정 // Set the scene sampling distance
				SurfaceBasedMatch3D.SetSceneSamplingDistance(0.03);
				// 키포인트 비율 설정 // Set the keypoint ratio.
				SurfaceBasedMatch3D.SetKeypointRatio(0.5);
				// 엣지 학습 여부 설정 // Set the edge train
				SurfaceBasedMatch3D.EnableTrainEdge(false);
				// 엣지 장면 여부 설정 // Set the edge scene
				SurfaceBasedMatch3D.EnableEdgeBasedMatch(false);
				// 클러스터링 범위 설정 // Set the clustering range
				SurfaceBasedMatch3D.SetClusterRange(2);
				// 포즈 조정 반복 횟수 설정 // Set the iteration value of pose refinement
				SurfaceBasedMatch3D.SetIteration(5);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = SurfaceBasedMatch3D.Learn()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Surface Based Match 3D.");
					break;
				}

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((eResult = SurfaceBasedMatch3D.Execute()).IsFail())
				{
					ErrorPrint(eResult, "Failed to execute Surface Based Match 3D.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately		
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);
				CGUIView3DLayer layer3DLearn = view3DLearn.GetLayer(0);
				CGUIView3DLayer layer3DSource = view3DSource.GetLayer(0);


				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DDst.Clear();
				layer3DLearn.Clear();
				layer3DSource.Clear();

				// View 정보를 디스플레이 합니다. // Display View information.
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다.// The function DrawTextCanvas below draws a String based on the screen coordinates.
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

				// 3D 오브젝트 뷰에 결과 Object와 비교를 위한 Source 오브젝트 디스플레이
				if((eResult = view3DDst.PushObject((CFL3DObject)SurfaceBasedMatch3D.GetSourceObject())).IsFail())
				{
					ErrorPrint(eResult, "Failed to set image object on the image view.\n");
					break;
				}

				CSurfaceBasedMatch3D.SPoseMatrixParameters sResult = new CSurfaceBasedMatch3D.SPoseMatrixParameters();

				double f64ArrRotX;
				double f64ArrRotY;
				double f64ArrRotZ;
				double f64Score;
				double f64Residual;

				long i64ResultCount = SurfaceBasedMatch3D.GetResultCount();

				if(i64ResultCount == 0)
				{
					ErrorPrint(eResult, "Failed to estimate pose matrix.\n");
					break;
				}

				for(long i = 0; i < i64ResultCount; i++)
				{
					CFL3DObject fl3DOLearnTransform = new CFL3DObject();
					CFLPoint3<double> flpTrans = new CFLPoint3<double>();
					TPoint3<double> tp3Center = new TPoint3<double>();

					// 추정된 포즈 행렬 가져오기
					if((eResult = SurfaceBasedMatch3D.GetResultPoseMatrix(i, out sResult)).IsFail())
					{
						ErrorPrint(eResult, "Failed to estimate pose matrix.\n");
						break;
					}

					f64Residual = sResult.f64Residual;
					f64Score = sResult.f64Score;
					f64ArrRotX = sResult.f64Rx;
					f64ArrRotY = sResult.f64Ry;
					f64ArrRotZ = sResult.f64Rz;
					flpTrans.x = sResult.f64Tx;
					flpTrans.y = sResult.f64Ty;
					flpTrans.z = sResult.f64Tz;

					// 추정한 포즈 결과를 Console창에 출력한다 // Print the estimated pose matrix to the console window
					Console.WriteLine(" ▷ Pose Matrix {0}", i);
					Console.WriteLine("  1. R : Rotation, T : Translation\n");
					Console.WriteLine("    Rx   : {0}", f64ArrRotX);
					Console.WriteLine("    Ry   : {0}", f64ArrRotY);
					Console.WriteLine("    Rz   : {0}", f64ArrRotZ);
					Console.WriteLine("    Tx   : {0}", flpTrans.x);
					Console.WriteLine("    Ty   : {0}", flpTrans.y);
					Console.WriteLine("    Tz   : {0}", flpTrans.z);
					Console.WriteLine("    Score : {0}", f64Score);
					Console.WriteLine("    Residual : {0}", f64Residual);
					Console.WriteLine("\n");

					if((eResult = SurfaceBasedMatch3D.GetResultObject(i, out fl3DOLearnTransform, out tp3Center)).IsFail())
					{
						ErrorPrint(eResult, "Failed to set object on the 3d view.\n");
						break;
					}

					if((eResult = view3DDst.PushObject(fl3DOLearnTransform)).IsFail())
					{
						ErrorPrint(eResult, "Failed to set object on the 3d view.\n");
						break;
					}

					string strChannel = String.Format("R({0}, {1}, {2}) , \nT({3}, {4}, {5})\nScore : {6}\nResidual {7}:"
									   , f64ArrRotX, f64ArrRotY, f64ArrRotZ, flpTrans.x, flpTrans.y, flpTrans.z, f64Score, f64Residual);

					// 추정된 포즈 행렬 및 score 출력
					if((eResult = layer3DDst.DrawText3D(tp3Center, strChannel, EColor.YELLOW, EColor.BLACK, 15)).IsFail())
					{
						ErrorPrint(eResult, "Failed to draw text.\n");
						break;
					}
				}


				view3DDst.ZoomFit();
				view3DLearn.ZoomFit();
				view3DSource.ZoomFit();

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DLearn.Invalidate(true);
				view3DSource.Invalidate(true);
				view3DDst.Invalidate(true);

				//이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSource.IsAvailable() && view3DLearn.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);

			}
			while(false);
		}
	}
}

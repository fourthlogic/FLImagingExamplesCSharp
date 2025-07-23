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
using System.Collections;

namespace Blob
{
	class Blob_SortClusterMode
	{
		enum EType
		{
			Src0 = 0,
			Src1,
			Src2,
			Src3,
			ETypeCount,
		};

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
			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
			{
				arrFliImage[i] = new CFLImage();
				arrViewImage[i] = new CGUIViewImage();
			}

			CResult res = new CResult();

			do
			{
				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					string strFileName = string.Format("../../ExampleImages/Blob/Blob Sort {0}.flif", i + 1);

					// 이미지 로드 // Load image
					if((res = arrFliImage[i].Load(strFileName)).IsFail())
					{
						ErrorPrint(res, "Failed to load the image file.\n");
						break;
					}

					int i32X = i % 2;
					int i32Y = i / 2;

					// 이미지 뷰 생성 // Create image view
					if((res = arrViewImage[i].Create(i32X * 400 + 400, i32Y * 400, i32X * 400 + 400 + 400, i32Y * 400 + 400)).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
					if((res = arrViewImage[i].SetImagePtr(ref arrFliImage[i])).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						break;
					}

					if((res = arrViewImage[i].ZoomFit()).IsFail())
					{
						ErrorPrint(res, "Failed to zoom fit\n");
						break;
					}

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if(i != (int)EType.Src0)
					{
						if((res = arrViewImage[(int)EType.Src0].SynchronizePointOfView(ref arrViewImage[i])).IsFail())
						{
							ErrorPrint(res, "Failed to synchronize view\n");
							break;
						}

						// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
						if((res = arrViewImage[(int)EType.Src0].SynchronizeWindow(ref arrViewImage[i])).IsFail())
						{
							ErrorPrint(res, "Failed to synchronize window.\n");
							break;
						}
					}
				}

				for(int k = 0; k < (int)EType.ETypeCount; ++k)
				{
					// Blob 객체 생성 // Create Blob object
					CBlob sBlob = new CBlob();

					// 처리할 이미지 설정 // Set the image to process
					sBlob.SetSourceImage(ref arrFliImage[k]);

					// 논리 조건 설정
					sBlob.SetLogicalCondition(ELogicalCondition.Greater);
					// 임계값 설정,  위의 조건과 아래의 조건이 합쳐지면 100보다 같거나 큰 객체를 검출
					sBlob.SetThreshold(100);

					// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
					if((res = sBlob.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute Blob.");
						break;
					}

					// Blob 결과를 얻어오기 위해 FigureArray 선언
					CFLFigureArray flfaSortClusterModeBoundaryRects = new CFLFigureArray();

					// SortClusterMode 함수를 통해 영역을 정렬 (객체의 Center좌표를 바탕으로 1순위 Y오름차순, 2순위 X오름차순)
					if((res = sBlob.SortClusterMode(CBlob.ESortClusterModeMethod.Center_Y_Asc_X_Asc)).IsFail())
					{
						ErrorPrint(res, "Failed to sort rects from the Blob object.");
						break;
					}

					// 복구된 Blob 결과들 중 Boundary Rectangle 을 얻어옴
					if((res = sBlob.GetResultBoundaryRects(ref flfaSortClusterModeBoundaryRects)).IsFail())
					{
						ErrorPrint(res, "Failed to get boundary rects from the Blob object.");
						break;
					}

					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					CGUIViewImageLayer layerSortClusterMode = arrViewImage[k].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					layerSortClusterMode.Clear();
					CFLPoint<double> flp = new CFLPoint<double>();

					if((res = layerSortClusterMode.DrawTextCanvas(flp, ("SortClusterMode (Y Asc, X Asc)"), EColor.YELLOW, EColor.BLACK, 30)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text on the image view.\n");
						break;
					}

					// flfaBoundaryRects 는 Figure들의 배열이기 때문에 Layer에 넣기만 해도 모두 드로윙이 가능하다.
					// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
					// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
					// 여기서 0.25이므로 옅은 반투명 상태라고 볼 수 있다.
					// 파라미터 순서 : 레이어 . Figure 객체 . 선 색 . 선 두께 . 면 색 . 펜 스타일 . 선 알파값(불투명도) . 면 알파값 (불투명도)
					if((res = layerSortClusterMode.DrawFigureImage(flfaSortClusterModeBoundaryRects, EColor.RED, 1, EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
						break;
					}

					// Rect 정보값을 각각 확인하는 코드
					for(Int64 i = 0; i < flfaSortClusterModeBoundaryRects.GetCount(); ++i)
					{
						CFLFigureArray pFlfa = (CFLFigureArray)flfaSortClusterModeBoundaryRects.GetAt(i);
						Int64 i64ObjectCount = pFlfa.GetCount();

						for(Int64 j = 0; j < i64ObjectCount; ++j)
						{
							CFLRect<Int32> pFlrRect = (CFLRect<Int32>)pFlfa.GetAt(j);

							if(pFlrRect != null)
								Console.Write("Recover No. [{0}][{1}] : ({2},{3},{4},{5})\n", i, j, pFlrRect.left, pFlrRect.top, pFlrRect.right, pFlrRect.bottom);

							layerSortClusterMode.DrawTextImage((pFlrRect.GetCenter()), string.Format("({0},{1})", i, j), EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
						}
					}

					// 이미지 뷰를 갱신 합니다. // Update image view
					arrViewImage[k].RedrawWindow();
				}

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bRun = true;

				while(bRun)
				{
					for(int i = 0; i < (int)EType.ETypeCount; ++i)
						bRun &= arrViewImage[i].IsAvailable();

					CThreadUtilities.Sleep(1);
				}
			}
			while(false);
		}
	}
}

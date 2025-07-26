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

namespace Blob
{
	class Blob_Sort
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

			// 이미지 객체 선언 // Declare the image object
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();
			CGUIViewImage viewImageRecover = new CGUIViewImage();
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/Blob/Ball.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(200, 0, 968, 576)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImageRecover.Create(968, 0, 1736, 576)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				if((res = viewImageRecover.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImage.SynchronizePointOfView(ref viewImageRecover)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				if((res = viewImageRecover.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// Blob 객체 생성 // Create Blob object
				CBlob sBlob = new CBlob();

				// 처리할 이미지 설정 // Set the image to process
				sBlob.SetSourceImage(ref fliImage);

				// ROI 범위 설정
				CFLRect<Int32> flrROI = new CFLRect<int>(450, 425, 1024, 800);

				// 처리할 ROI 설정
				sBlob.SetSourceROI(flrROI);

				// 논리 조건 설정
				sBlob.SetLogicalCondition(ELogicalCondition.GreaterEqual);
				// 임계값 설정,  위의 조건과 아래의 조건이 합쳐지면 100보다 같거나 큰 객체를 검출
				sBlob.SetThreshold(100);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sBlob.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Blob.");
					break;
				}

				// 50보다 같거나 큰 장변 길이를 가진 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.BoundaryRectWidth, 50, ELogicalCondition.GreaterEqual)).IsFail())
				{
					ErrorPrint(res, "Blob filtering algorithm error occurs.");
					break;
				}

				// 50보다 같거나 큰 단변 길이를 가진 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.BoundaryRectHeight, 50, ELogicalCondition.GreaterEqual)).IsFail())
				{
					ErrorPrint(res, "Blob filtering algorithm error occurred.");
					break;
				}

				// 면적이 50보다 작은 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.Area, 50, ELogicalCondition.LessEqual)).IsFail())
				{
					ErrorPrint(res, "Blob filtering algorithm error occurred.");
					break;
				}

				// Blob 결과를 얻어오기 위해 FigureArray 선언
				CFLFigureArray flfaSortedBoundaryRects = new CFLFigureArray();
				CFLFigureArray flfaRecoverBoundaryRects = new CFLFigureArray();

				List<Int32> flaItem = new List<int>();
				List<Int32> flaOrder = new List<int>();

				// 첫 번째 조건을 Bound rect center y좌표, 내림차순 정렬				
				flaItem.Add((Int32)CBlob.EFilterItem.BoundaryRectCenterY);
				flaOrder.Add((Int32)CBlob.EOrder.Descending);

				// 두 번째 조건을 Bound rect center x좌표, 내림차순 정렬
				flaItem.Add((Int32)CBlob.EFilterItem.BoundaryRectCenterX);
				flaOrder.Add((Int32)CBlob.EOrder.Descending);

				// Blob 결과를 정렬
				if((res = sBlob.Sort(flaItem, flaOrder)).IsFail())
				{
					ErrorPrint(res, "Failed to sort from the Blob object.");
					break;
				}

				// Blob 결과들 중 Boundary Rectangle 을 얻어옴
				if((res = sBlob.GetResultBoundaryRects(ref flfaSortedBoundaryRects)).IsFail())
				{
					ErrorPrint(res, "Failed to get boundary rects from the Blob object.");
					break;
				}

				// Blob 정렬 상태를 초기 상태로 복구
				if((res = sBlob.Sort(CBlob.EFilterItem.Unselected, CBlob.EOrder.Ascending)).IsFail())
				{
					ErrorPrint(res, "Failed to sort from the Blob object.");
					break;
				}

				// 복구된 Blob 결과들 중 Boundary Rectangle 을 얻어옴
				if((res = sBlob.GetResultBoundaryRects(ref flfaRecoverBoundaryRects)).IsFail())
				{
					ErrorPrint(res, "Failed to get boundary rects from the Blob object.");
					break;
				}


				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);
				CGUIViewImageLayer layerRecover = viewImageRecover.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();
				layerRecover.Clear();
				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layer.DrawTextCanvas(flp, ("Sort"), EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text on the image view.\n");
					break;
				}

				if((res = layerRecover.DrawTextCanvas(flp, ("No Sort"), EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text on the image view.\n");
					break;
				}

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능
				if((res = layer.DrawFigureImage(flrROI, EColor.BLUE)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
					break;
				}

				if((res = layerRecover.DrawFigureImage(flrROI, EColor.BLUE)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
					break;
				}

				// flfaBoundaryRects 는 Figure들의 배열이기 때문에 Layer에 넣기만 해도 모두 드로윙이 가능하다.
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 여기서 0.25이므로 옅은 반투명 상태라고 볼 수 있다.
				// 파라미터 순서 : 레이어 . Figure 객체 . 선 색 . 선 두께 . 면 색 . 펜 스타일 . 선 알파값(불투명도) . 면 알파값 (불투명도)
				if((res = layer.DrawFigureImage(flfaSortedBoundaryRects, EColor.RED, 1, EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
					break;
				}

				if((res = layerRecover.DrawFigureImage(flfaRecoverBoundaryRects, EColor.RED, 1, EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
					break;
				}

				// Rect 정보값을 각각 확인하는 코드
				for(Int64 i = 0; i < flfaSortedBoundaryRects.GetCount(); ++i)
				{
					CFLRect<Int32> pFlrRect = (CFLRect<Int32>)flfaSortedBoundaryRects.GetAt(i);

					if(pFlrRect != null)
						Console.Write("Sorted No. {0} : ({1},{2},{3},{4})\n", i, pFlrRect.left, pFlrRect.top, pFlrRect.right, pFlrRect.bottom);

					layer.DrawTextImage((pFlrRect.GetCenter()), string.Format("{0}", i), EColor.CYAN);

					pFlrRect = (CFLRect<Int32>)flfaRecoverBoundaryRects.GetAt(i);

					if(pFlrRect != null)
						Console.Write("Recover No. {0} : ({1},{2},{3},{4})\n", i, pFlrRect.left, pFlrRect.top, pFlrRect.right, pFlrRect.bottom);

					layerRecover.DrawTextImage((pFlrRect.GetCenter()), string.Format("{0}", i), EColor.CYAN);
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage.Invalidate(true);
				viewImageRecover.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable() && viewImageRecover.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

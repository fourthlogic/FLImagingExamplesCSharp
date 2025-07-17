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
			// 이미지 객체 선언 // Declare the image object
			CFLImage fliImage = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImage = new CGUIViewImage();
			CResult res = new CResult();

			do
			{
				// 이미지 로드 // Load image
				if((res = fliImage.Load("../../ExampleImages/Blob/AlignBall.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(200, 0, 812, 512)).IsFail())
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

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = viewImage.ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// BlobSubsampled 객체 생성 // Create BlobSubsampled object
				CBlobSubsampled sBlob = new CBlobSubsampled();

				// 처리할 이미지 설정 // Set the image to process
				sBlob.SetSourceImage(ref fliImage);

				// 논리 조건 설정
				sBlob.SetLogicalCondition(ELogicalCondition.Less);
				// 임계값 설정,  위의 조건과 아래의 조건이 합쳐지면 127보다 작은 객체를 검출
				sBlob.SetThreshold(127);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = (res = sBlob.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute Blob.");
					break;
				}

				// 20보다 같거나 작은 장변 길이를 가진 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.BoundaryRectWidth, 20, ELogicalCondition.LessEqual)).IsFail())
				{
					ErrorPrint(res, "Blob filtering algorithm error occurs.");
					break;
				}

				// circularity 가 0.9보다 작은 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.Circularity, 0.9, ELogicalCondition.Less)).IsFail())
				{
					ErrorPrint(res, "Blob filtering algorithm error occurred.");
					break;
				}

				// Blob 결과를 얻어오기 위해 FigureArray 선언
				CFLFigureArray flfaContour = new CFLFigureArray();

				List<Int32> flaItem = new List<int>();
				List<Int32> flaOrder = new List<int>();

				// Blob 결과의 Median, LowerQuartile, UpperQuartile 을 얻어오기 위한 Statistics 객체 선언
				CImageStatistics imgStatistics = new CImageStatistics();
				Int64 i64ResultCount = sBlob.GetResultCount();

				// Statistics 소스 이미지 설정
				imgStatistics.SetSourceImage(ref fliImage);

				// Blob 결과들 중 Contour 를 얻어옴
				if((res = sBlob.GetResultContours(ref flfaContour)).IsFail())
				{
					ErrorPrint(res, "Failed to get boundary rects from the Blob object.");
					break;
				}

				string strMedian, strLowerQuartile, strUpperQuartile;

				// 각 Contour에 맞는 Median, LowerQuartile, UpperQuartile 을 얻어오는 코드
				for(Int64 i = 0; i < i64ResultCount; ++i)
				{
					imgStatistics.SetSourceROI(flfaContour.GetAt(i));

					CMultiVar<double> mvMedian = new CMultiVar<double>();

					if((res = imgStatistics.GetMedian(ref mvMedian)).IsFail())
					{
						ErrorPrint(res, "Failed to get Median Value from the Blob object.");
						break;
					}

					strMedian = String.Format("No. {0} Median : {1}", i, mvMedian.GetAt(0));
					Console.WriteLine(strMedian);

					CMultiVar<double> mvLowerQuartile = new CMultiVar<double>();

					if((res = imgStatistics.GetLowerQuartile(ref mvLowerQuartile)).IsFail())
					{
						ErrorPrint(res, "Failed to get LowerQuartile Value from the Blob object.");
						break;
					}

					strLowerQuartile = String.Format("No. {0} LowerQuartile : {1}", i, mvLowerQuartile.GetAt(0));
					Console.WriteLine(strLowerQuartile);

					CMultiVar<double> mvUpperQuartile = new CMultiVar<double>();

					if((res = imgStatistics.GetUpperQuartile(ref mvUpperQuartile)).IsFail())
					{
						ErrorPrint(res, "Failed to get UpperQuartile Value from the Blob object.");
						break;
					}

					strUpperQuartile = String.Format("No. {0} UpperQuartile : {1}", i, mvUpperQuartile.GetAt(0));
					Console.WriteLine(strUpperQuartile);
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();
				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layer.DrawTextCanvas(flp, ("Source"), EColor.YELLOW, EColor.BLACK, 30)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text on the image view.\n");
					break;
				}

				// flfaBoundaryRects 는 Figure들의 배열이기 때문에 Layer에 넣기만 해도 모두 드로윙이 가능하다.
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 여기서 0.25이므로 옅은 반투명 상태라고 볼 수 있다.
				// 파라미터 순서 : 레이어 . Figure 객체 . 선 색 . 선 두께 . 면 색 . 펜 스타일 . 선 알파값(불투명도) . 면 알파값 (불투명도)
				if((res = layer.DrawFigureImage(flfaContour, EColor.RED, 1, EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
					break;
				}

				// Rect 정보값을 각각 확인하는 코드
				for(Int64 i = 0; i < flfaContour.GetCount(); ++i)
				{
					CFLRegion pFlrRect = (CFLRegion)flfaContour.GetAt(i);

					layer.DrawTextImage((pFlrRect.GetCenter()), string.Format("{0}", i), EColor.CYAN);
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage.IsAvailable())
					CThreadUtilities.Sleep(1);
			}
			while(false);
		}
	}
}

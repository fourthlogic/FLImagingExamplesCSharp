﻿using System;
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
	class Blob_BoundaryRect_ResultInfo
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
				if((res = viewImage.Create(400, 0, 1424, 768)).IsFail())
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

				// Blob 객체 생성 // Create Blob object
				CBlob sBlob = new CBlob();

				// 처리할 이미지 설정 // Set the image to process
				sBlob.SetSourceImage(ref fliImage);
				// 논리 조건 설정
				sBlob.SetLogicalCondition(ELogicalCondition.Less);
				// 임계값 설정,  위의 조건과 아래의 조건이 합쳐지면 127보다 작은 객체를 검출
				sBlob.SetThreshold(127);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sBlob.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Blob.");
					break;
				}

				// 면적이 500보다 작은 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.Area, 500, ELogicalCondition.LessEqual)).IsFail())
				{
					ErrorPrint(res, "Blob filtering algorithm error occurred.");
					break;
				}

				// Blob 결과를 얻어오기 위해 FigureArray 선언
				CFLFigureArray flfaBoundaryRects = new CFLFigureArray();

				// Blob 결과들 중 Boundary Rectangle 을 얻어옴
				if((res = sBlob.GetResultBoundaryRects(ref flfaBoundaryRects)).IsFail())
				{
					ErrorPrint(res, "Failed to get boundary rects from the Blob object.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// flfaBoundaryRects 는 Figure들의 배열이기 때문에 Layer에 넣기만 해도 모두 드로윙이 가능하다.
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 여기서 0.25이므로 옅은 반투명 상태라고 볼 수 있다.
				// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
				if((res = layer.DrawFigureImage(flfaBoundaryRects, EColor.RED, 1, EColor.RED, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
					break;
				}

				// Rect 정보값을 각각 확인하는 코드
				for(Int64 i = 0; i < flfaBoundaryRects.GetCount(); ++i)
				{
					CFLRect<Int32> flrRect = (CFLRect<Int32>)flfaBoundaryRects.GetAt(i);

					if(flrRect != null)
					{
						Console.Write("No. [{0}]\n", i);
						Console.Write("LeftTop     : ({0},{1})\n", flrRect.left, flrRect.top);
						Console.Write("RightBottom : ({0},{1})\n", flrRect.right, flrRect.bottom);
						Console.Write("Width  : {0}\n", flrRect.GetWidth());
						Console.Write("Height : {0}\n", flrRect.GetHeight());
						Console.Write("Center : ({0},{1})\n\n", flrRect.GetCenter().x, flrRect.GetCenter().y);

						string strNumber = string.Format("[{0}]", i);
						string strLeftTop = string.Format("({0}, {1})", flrRect.left, flrRect.top);
						string strRightBottom = string.Format("({0}, {1})", flrRect.right, flrRect.bottom);
						string strInfo = string.Format("Width : {0}\nHeight : {1}\nCenter : ({2}, {3})", flrRect.GetWidth(), flrRect.GetHeight(), flrRect.GetCenter().x, flrRect.GetCenter().y);

						CFLPoint<double> flpLeftTop = new CFLPoint<double>(flrRect.left, flrRect.top);
						CFLPoint<double> flpRightBottom = new CFLPoint<double>(flrRect.right, flrRect.bottom);
						CFLPoint<double> flpRightTop = new CFLPoint<double>(flrRect.right, flrRect.top);

						// 아래 함수 DrawTextImage는 Image좌표를 기준으로 하는 Text를 Drawing 한다는 것을 의미한다.
						// 파라미터 순서 : 레이어 -> 문자열 좌표 -> 문자열 지정 -> 문자열 색 -> 면 색 -> 폰트 크기 -> 실제 크기로 그릴지의 여부 -> 각도 -> 문자열의 위치 기준
						layer.DrawTextImage(flrRect.GetCenter(), strNumber, EColor.CYAN, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
						layer.DrawTextImage(flpLeftTop, strLeftTop, EColor.YELLOW, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.RIGHT_BOTTOM);
						layer.DrawTextImage(flpRightBottom, strRightBottom, EColor.YELLOW, EColor.BLACK);
						layer.DrawTextImage(flpRightTop, strInfo, EColor.LIME, EColor.BLACK, 12, false, 0, EGUIViewImageTextAlignment.LEFT_BOTTOM);
					}
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

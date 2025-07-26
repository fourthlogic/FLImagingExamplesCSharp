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

namespace BlobSubsampled
{
	class BlobSubsampled_GravityCenter
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
				if((res = viewImage.Create(200, 0, 968, 576)).IsFail())
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

				// 임계값 설정,  위의 조건과 아래의 조건이 합쳐지면 50보다 작은 객체를 검출
				sBlob.SetThreshold(50);

				// Subsampling Level 설정
				sBlob.SetSubsamplingLevel(3);

				// BlobSubsampled Result Type mask 생성 (Contour, GravityCenter)
				Int32 i32ResultTypeMask = (Int32)CBlob.EBlobResultType.Contour | (Int32)CBlob.EBlobResultType.GravityCenter;

				// Result Type 설정
				sBlob.SetResultType((CBlob.EBlobResultType)i32ResultTypeMask);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sBlob.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute BlobSubsampled.");
					break;
				}

				// BoundaryRect의 20보다 작은 너비를 가진 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.BoundaryRectWidth, 20, ELogicalCondition.Less)).IsFail())
				{
					ErrorPrint(res, "BlobSubsampled filtering algorithm error occurs.");
					break;
				}

				// BoundaryRect의 20보다 작은 높이를 가진 객체들을 제거
				if((res = sBlob.Filter(CBlob.EFilterItem.BoundaryRectHeight, 20, ELogicalCondition.Less)).IsFail())
				{
					ErrorPrint(res, "BlobSubsampled filtering algorithm error occurred.");
					break;
				}

				// BlobSubsampled 결과를 얻어오기 위해 FigureArray 선언
				CFLFigureArray flfaContours = new CFLFigureArray();
				CFLFigureArray flfGravityCenter = new CFLFigureArray();

				// BlobSubsampled 결과들 중 Contours 를 얻어옴
				if((res = sBlob.GetResultContours(ref flfaContours)).IsFail())
				{
					ErrorPrint(res, "Failed to get contours from the BlobSubsampled object.");
					break;
				}

				// BlobSubsampled 결과들 중 Gravity Center 를 얻어옴
				if((res = sBlob.GetResultGravityCenters(ref flfGravityCenter)).IsFail())
				{
					ErrorPrint(res, "Failed to get gravity center from the BlobSubsampled object.");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer.Clear();

				// flfaContours 는 Figure들의 배열이기 때문에 Layer에 넣기만 해도 모두 드로윙이 가능하다.
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 여기서 0.25이므로 옅은 반투명 상태라고 볼 수 있다.
				// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
				if((res = layer.DrawFigureImage(flfaContours, EColor.BLUE, 1, EColor.BLUE, EGUIViewImagePenStyle.Solid, 1.0f, .25f)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figure objects on the image view.\n");
					break;
				}

				string str = "";
				string flsTextResult = "";

				// Image View 객체에 Index, Gravity Center 순서대로 출력
				for(Int64 i = 0; i < flfaContours.GetCount(); ++i)
				{
					CFLPoint<double> flpContoursCenter = new CFLPoint<double>(flfaContours.GetAt(i));
					CFLPoint<double> flpfGravityCenter = new CFLPoint<double>(flfGravityCenter.GetAt(i));
					CFLFigureArray flfCrossHair = new CFLFigureArray();

					flfCrossHair = flpfGravityCenter.MakeCrossHair(10, true);

					// Image View 출력
					str = string.Format("[{0}]\n\n\n\n", i);
					flsTextResult = string.Format("\n\n\n\n\n\nGravity Center\nX : {0:F2} Y : {1:F2}", flpfGravityCenter.x, flpfGravityCenter.y);

					layer.DrawTextImage(flpContoursCenter, str, EColor.LIME, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
					layer.DrawTextImage(flpContoursCenter, flsTextResult, EColor.YELLOW, EColor.BLACK, 10, false, 0, EGUIViewImageTextAlignment.CENTER_CENTER);
					layer.DrawFigureImage(flfCrossHair, EColor.ORANGERED, 1, EColor.ORANGERED, EGUIViewImagePenStyle.Solid, 1, 0.25f);

					// 콘솔에 출력
					Console.Write("[{0}] Gravity Center x : {1:F2}\ty : {2:F2} \n", i, flpfGravityCenter.x, flpfGravityCenter.y);
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

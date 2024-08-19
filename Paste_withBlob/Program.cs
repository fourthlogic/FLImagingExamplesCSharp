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

namespace Paste
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

		enum EType
		{
			Source = 0,
			Destination1,
			Destination2,
			ETypeCount,
		}

		[STAThread]
		static void Main(string[] args)
		{
			// 이미지 객체 선언 // Declare the image object
			CFLImage[] arrFliImage = new CFLImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrFliImage[i] = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage[] arrViewImage = new CGUIViewImage[(int)EType.ETypeCount];

			for(int i = 0; i < (int)EType.ETypeCount; ++i)
				arrViewImage[i] = new CGUIViewImage();

			do
			{
				CResult res;

				// Source 이미지 로드 // Load the source image
				if((res = arrFliImage[(int)EType.Source].Load("../../ExampleImages/Paste/ChessBoard.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination1 이미지 로드. // Load the destination1 image
				if((res = arrFliImage[(int)EType.Destination1].Load("../../ExampleImages/Paste/Floor.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// Destination2 이미지를 Destination1 이미지와 동일한 이미지로 생성
				if((res = (arrFliImage[(int)EType.Destination2].Assign(arrFliImage[(int)EType.Destination1]))).IsFail())
				{
					ErrorPrint(res, "Failed to assign the image file.\n");
					break;
				}

				bool bError = false;

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					int x = i % 3;
					int y = i / 3;

					// 이미지 뷰 생성 // Create image view
					if((res = (arrViewImage[i].Create(x * 512 + 100, y * 512, x * 512 + 100 + 512, y * 512 + 512))).IsFail())
					{
						ErrorPrint(res, "Failed to create the image view.\n");
						bError = true;
						break;
					}

					// 이미지 뷰에 이미지를 디스플레이 // Display an image in an image view
					if((res = (arrViewImage[i].SetImagePtr(ref arrFliImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to set image object on the image view.\n");
						bError = true;
						break;
					}

					if(i == (int)EType.Source)
						continue;

					// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
					if((res = (arrViewImage[(int)EType.Source].SynchronizePointOfView(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize view\n");
						bError = true;
						break;
					}

					// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
					if((res = (arrViewImage[(int)EType.Source].SynchronizeWindow(ref arrViewImage[i]))).IsFail())
					{
						ErrorPrint(res, "Failed to synchronize window.\n");
						bError = true;
						break;
					}
				}

				if(bError)
					break;

				// Image 크기에 맞게 view의 크기를 조정 // Zoom the view to fit the image size
				if((res = arrViewImage[(int)EType.Source].ZoomFit()).IsFail())
				{
					ErrorPrint(res, "Failed to zoom fit\n");
					break;
				}

				// Blob 객체 생성 // Create Blob object
				CBlob sBlob = new CBlob();

				// 처리할 이미지 설정 // Set the image to process
				if((res = sBlob.SetSourceImage(ref arrFliImage[(int)EType.Source])).IsFail())
				{
					ErrorPrint(res, "Failed to set source image.\n");
					break;
				}

				// Threshold 모드 설정. 여기서는 2중 Threshold에 두개의 조건의 And 조건을 참으로 설정한다.
				// Threshold mode setting. Here, set the AND condition of the two conditions to true in the double threshold.
				sBlob.SetThresholdMode(EThresholdMode.Dual_And);
				// 논리 조건 설정 // Set logical condition
				//sBlob.SetLogicalCondition(ELogicalCondition.Greater, EThresholdIndex.First);
				//sBlob.SetLogicalCondition(ELogicalCondition.Less, EThresholdIndex.Second);
				sBlob.SetLogicalCondition(ELogicalCondition.Greater, ELogicalCondition.Less);
				// 임계값 설정,  위의 조건과 아래의 조건이 합쳐지면 130보다 크고 240보다 작은 객체를 검출
				// Set the threshold, when the above and below conditions are combined, objects greater than 130 and less than 240 are detected
				//sBlob.SetThreshold(130.0, EThresholdIndex.First);
				//sBlob.SetThreshold(240.0, EThresholdIndex.Second);
				sBlob.SetThreshold(130, 240);

				// 가운데 구멍난 Contour를 지원하기 위해 Perforated 모드 설정
				// Set perforated mode to support perforated contours
				sBlob.SetResultType(CBlob.EBlobResultType.Contour);
				sBlob.SetContourResultType(CBlob.EContourResultType.Perforated);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sBlob.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Blob.");
					break;
				}

				// Blob 결과를 얻어오기 위해 FigureArray 선언 // Declare FigureArray to get blob result
				CFLFigureArray flfaContours;

				// Blob 결과들 중 Contour를 얻어옴 // Get Contour from Blob results
				if((res = sBlob.GetResultContours(out flfaContours)).IsFail())
				{
					ErrorPrint(res, "Failed to get boundary rects from the Blob object.");
					break;
				}

				// Paste 객체 생성 // Create Paste object
				CPaste sPaste = new CPaste();

				// 처리할 이미지 설정 // Set the image to process
				sPaste.SetSourceImage(ref arrFliImage[(int)EType.Source]);
				// Blob 결과인 flfaContours를 Src ROI로 설정 // Set blob result flfaContours as Src ROI
				sPaste.SetSourceROI(flfaContours);
				// Destination 이미지 설정 // Set the destination image
				sPaste.SetDestinationImage(ref arrFliImage[(int)EType.Destination2]);
				// Destination Pivot 설정을 위한 Blob 결과의 Center Point 설정 // Set the Center Point of the blob result to set the Destination Pivot
				CFLPoint<double> flpPivot = new CFLPoint<double>(flfaContours.GetCenter());
				// Destination Pivot 설정 // Set the destination pivot
				sPaste.SetDestinationPivot(flpPivot);
				// 항상 공백 영역을 지정한 색으로 채우도록 설정할 것인지 선택 // Choose whether to always fill blank areas with the specified color
				sPaste.EnableFillBlankColorMode(false);
				// 공백 영역 색상 지정 // Set the blank color
				CMultiVar<double> mv = new CMultiVar<double>(0, 0, 0);
				sPaste.SetBlankColor(mv);

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = sPaste.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute Paste.");
					break;
				}

				CGUIViewImageLayer[] arrLayer = new CGUIViewImageLayer[(int)EType.ETypeCount];

				for(int i = 0; i < (int)EType.ETypeCount; ++i)
				{
					// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
					// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
					arrLayer[i] = arrViewImage[i].GetLayer(0);

					// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
					arrLayer[i].Clear();
				}

				// ROI영역이 어디인지 알기 위해 디스플레이 한다 // Display to find out where ROI is
				// FLImaging의 Figure 객체들은 어떤 도형모양이든 상관없이 하나의 함수로 디스플레이가 가능 // FLimaging's Figure objects can be displayed as a function regardless of the shape
				// 아래 함수 DrawFigureImage는 Image좌표를 기준으로 하는 Figure를 Drawing 한다는 것을 의미하며 // The function DrawFigureImage below means drawing a picture based on the image coordinates
				// 맨 마지막 두개의 파라미터는 불투명도 값이고 1일경우 불투명, 0일경우 완전 투명을 의미한다. // The last two parameters are opacity values, which mean opacity for 1 day and complete transparency for 0 day.
				// 파라미터 순서 : 레이어 -> Figure 객체 -> 선 색 -> 선 두께 -> 면 색 -> 펜 스타일 -> 선 알파값(불투명도) -> 면 알파값 (불투명도) // Parameter order: Layer -> Figure object -> Line color -> Line thickness -> Face color -> Pen style -> Line alpha value (opacity) -> Area alpha value (opacity)
				if((res = arrLayer[(int)EType.Source].DrawFigureImage(flfaContours, EColor.LIME)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination2].DrawFigureImage(flfaContours, EColor.LIME)).IsFail())
				{
					ErrorPrint(res, "Failed to draw figures objects on the image view.\n");
					break;
				}

				CFLPoint<double> flp = new CFLPoint<double>();

				// View 정보를 디스플레이 한다. // Display view information
				// 아래 함수 DrawTextCanvas은 Screen좌표를 기준으로 하는 String을 Drawing 한다. // The function DrawTextCanvas below draws a String based on the screen coordinates.
				// 색상 파라미터를 EGUIViewImageLayerTransparencyColor 으로 넣어주게되면 배경색으로 처리함으로 불투명도를 0으로 한것과 같은 효과가 있다. // If the color parameter is added as EGUIViewImageLayerTransparencyColor, it has the same effect as setting the opacity to 0 by processing it as a background color.
				// 파라미터 순서 : 레이어 -> 기준 좌표 Figure 객체 -> 문자열 -> 폰트 색 -> 면 색 -> 폰트 크기 -> 실제 크기 유무 -> 각도 ->
				//                 얼라인 -> 폰트 이름 -> 폰트 알파값(불투명도) -> 면 알파값 (불투명도) -> 폰트 두께 -> 폰트 이텔릭
				// Parameter order: layer -> reference coordinate Figure object -> string -> font color -> Area color -> font size -> actual size -> angle ->
				//                  Align -> Font Name -> Font Alpha Value (Opaqueness) -> Cotton Alpha Value (Opaqueness) -> Font Thickness -> Font Italic
				if((res = arrLayer[(int)EType.Source].DrawTextCanvas(flp, ("Source Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination1].DrawTextCanvas(flp, ("Destination Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				if((res = arrLayer[(int)EType.Destination2].DrawTextCanvas(flp, ("Result Image"), EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// Contours 정보값을 각각 확인하는 코드 // Code to check each Contours information value
				for(Int64 i = 0; i < flfaContours.GetCount(); ++i)
				{
					CFLRegion pFlrg = (CFLRegion)flfaContours.GetAt(i);

					// 폴리곤의 정점 정보를 콘솔에 출력 // Print polygon vertex information to the console
					Console.Write("Blob Result No. {0} : [\n", i);

					for(Int64 j = 0; j < pFlrg.GetCount(); ++j)
					{
						if(j != 0)
							Console.Write(",");

						CFLPoint<double> pFlpVertex = (CFLPoint<double>)pFlrg.GetAt(j);

						Console.Write("({0}, {1})", pFlpVertex.x, pFlpVertex.y);
					}

					Console.Write("]\n\n");

					CFLRect<double> flr = new CFLRect<double>();

					pFlrg.GetBoundaryRect(out flr);

					CFLPoint<double> flpDraw = new CFLPoint<double>(flr.left, (flr.top + flr.bottom) * .5);

					string strNumber = string.Format("{0}", i);

					arrLayer[(int)EType.Source].DrawTextImage(flpDraw, strNumber, EColor.BLACK, EColor.YELLOW, 12, false, 0, EGUIViewImageTextAlignment.CENTER, null, 1.0f, 1.0f, EGUIViewImageFontWeight.BOLD, false);
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				for(int i = 0; i < (int)EType.ETypeCount; ++i)
					arrViewImage[i].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				bool bAvailable = true;
				while(bAvailable)
				{
					for(int i = 0; i < (int)EType.ETypeCount; ++i)
					{
						bAvailable = arrViewImage[i].IsAvailable();

						if(!bAvailable)
							break;
					}

					Thread.Sleep(1);
				}
			}
			while(false);
		}
	}
}

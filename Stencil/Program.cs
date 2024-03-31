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

namespace Stencil
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
			// 이미지 뷰 선언 // Declare image view
			CGUIViewImage viewImage = new CGUIViewImage();

			// 수행 결과 객체 선언 // Declare the execution result object
			CResult res;

			do
			{
				// Image View 생성 // Create image view
				if((res = viewImage.Create(200, 0, 800, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layer = viewImage.GetLayer(0);

				// 스텐실 클래스 선언 // Declare CStencil class instance. 
				CStencil stencil = new CStencil();

				// 줄 간격 설정 // Set the line spacing.
				if((res = stencil.SetLineSpacing(0.2f)).IsFail())
				{
					ErrorPrint(res, "Failed to set line spacing.\n");
					break;
				}

				// 자간 설정 // Set the letter spacing.
				if((res = stencil.SetLetterSpacing(0.3f)).IsFail())
				{
					ErrorPrint(res, "Failed to set letter spacing.\n");
					break;
				}

				// 폰트 크기 설정 // Set the font size.
				if((res = stencil.SetFontSize(24)).IsFail())
				{
					ErrorPrint(res, "Failed to set font size.\n");
					break;
				}

				// Arial 폰트를 미리 로드 // Load font "Arial"
				if((res = stencil.LoadFont("Arial")).IsFail())
				{
					ErrorPrint(res, "Failed to load font : Arial.\n");
					break;
				}

				// Cambria 폰트를 미리 로드 // Load font "Cambria"
				if((res = stencil.LoadFont("Cambria")).IsFail())
				{
					ErrorPrint(res, "Failed to load font : Cambria.\n");
					break;
				}

				// 미리 로드한 Arial 폰트 선택 // Select preloaded font "Arial"
				if((res = stencil.SelectFont("Arial")).IsFail())
				{
					ErrorPrint(res, "Failed to select font : Arial.\n");
					break;
				}

				// 폰트 이름 문자열 선언 // Declare string to get font name.
				String strFontName = "";

				// 선택한 폰트의 이름 얻어 오기 // Get selected font name.
				if((res = stencil.GetSelectedFontName(out strFontName)).IsFail())
				{
					ErrorPrint(res, "Failed to get selected font name.\n");
					break;
				}

				// 도형 선언 // Declare CFLFigureArray instance.
				CFLFigureArray flfaRes;

				// 문자열 선언 // Declare the string to convert to figure.
				String strText = "[Arial]\nFourthLogic CStencil class...";

				// 선택한 폰트로 문자열을 도형으로 변환 // Convert the text to figure
				if((res = stencil.ConvertStringToFigure(strText, out flfaRes)).IsFail())
				{
					ErrorPrint(res, "Failed to convert string to figure.\n");
					break;
				}

				// 레이어에 도형을 그리기 // Draw the figure on a layer.
				layer.DrawFigureImage(flfaRes, EColor.BLACK, 1, EColor.YELLOW);

				// 미리 로드한 Cambria 폰트 선택
				if((res = stencil.SelectFont("Cambria")).IsFail())
				{
					ErrorPrint(res, "Failed to select font : Cambria.\n");
					break;
				}

				// 줄 간격 설정 // Set the line spacing.
				if((res = stencil.SetLineSpacing(1.0f)).IsFail())
				{
					ErrorPrint(res, "Failed to set line spacing.\n");
					break;
				}

				// 자간 설정 // Set the letter spacing.
				if((res = stencil.SetLetterSpacing(0)).IsFail())
				{
					ErrorPrint(res, "Failed to set letter spacing.\n");
					break;
				}

				// 문자열 내용 변경 // Set the text to convert to figure.
				strText = "[Cambria]\nFourthLogic CStencil class...";

				// 선택한 폰트로 문자열을 도형으로 변환 // Convert the string to figure.
				if((res = stencil.ConvertStringToFigure(strText, out flfaRes)).IsFail())
				{
					ErrorPrint(res, "Failed to convert string to figure.\n");
					break;
				}

				// 얻어 온 도형의 위치를 조정 // Offset the position of the converted figure.
				flfaRes.Offset((double)0, (double)stencil.GetFontSize() * 5);

				// 레이어에 도형을 그리기 // Draw the figure on a layer.
				layer.DrawFigureImage(flfaRes, EColor.BLACK, 1, EColor.CYAN);

				// 이미지 뷰를 갱신 합니다.
				viewImage.Invalidate(true);

				// 이미지 뷰가 꺼지면 종료로 간주
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);

		}
	}
}

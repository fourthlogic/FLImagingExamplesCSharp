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

namespace Figure
{
	class FigureSaveLoad
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
			CGUIViewImage[] viewImage = new CGUIViewImage[2];

			viewImage[0] = new CGUIViewImage();
			viewImage[1] = new CGUIViewImage();


			CResult res = new CResult(EResult.UnknownError);

			do
			{
				// 이미지 뷰 생성 // Create image view
				if((res = viewImage[0].Create(400, 0, 912, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				if((res = viewImage[1].Create(912, 0, 1424, 384)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// SourceView, DstView 의 0번 레이어 가져오기 // Get Layer 0 of SourceView, DstView
				CGUIViewImageLayer SrcLayer0 = viewImage[0].GetLayer(0);
				CGUIViewImageLayer DstLayer0 = viewImage[1].GetLayer(0);

				SrcLayer0.DrawTextCanvas(new TPoint<double>(0, 0), "Figure To Save", EColor.YELLOW, EColor.BLACK, 15);
				DstLayer0.DrawTextCanvas(new TPoint<double>(0, 0), "Loaded Figure", EColor.YELLOW, EColor.BLACK, 15);

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views
				if((res = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view\n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 맞춤 // Synchronize the positions of the two image view windows
				if((res = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window.\n");
					break;
				}

				//////// Save
				// Figure 생성 // Create figure
				CFLRect<double> flr = new CFLRect<double>(50, 50, 100, 100);

				CFLCircle<double> flc = new CFLCircle<double>(150.0, 100.0, 30.0, 0.0, 0.0, 80.0, EArcClosingMethod.Center);

				CFLEllipse<double> fle = new CFLEllipse<double>(300, 150, 100, 50, 0, 180, 60, EArcClosingMethod.EachOther);

				CFLFigureArray flfa = new CFLFigureArray();

				flfa.PushBack(flc);
				flfa.PushBack(fle);

				String strFigure;
				Console.WriteLine("Figure To Save\n");

				strFigure = String.Format("Rect : {0}\n", CFigureUtilities.ConvertFigureObjectToString(flr));
				Console.WriteLine("{0}", strFigure);

				strFigure = String.Format("Figure Array : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfa));
				Console.WriteLine("{0}", strFigure);

				// SourceView의 0번 레이어에 그리기 // Draw on Layer 0 of SourceView
				SrcLayer0.DrawFigureImage(flr, EColor.RED);
				SrcLayer0.DrawFigureImage(flfa, EColor.BLUE);

				// 경로 없이 파일명만 넣고 저장하는 것도 가능 // It is also possible to put only the file name without path and save it
				flr.Save("FLRect.fig");

				// 확장자명 없이 저장하는 것도 가능 // It is also possible to save without an extension name
				res = flfa.Save("FigureArray");

				//////// Load
				//다른 DeclType 인 파일을 Load할 경우 반환값이 EResult_OK 가 아닌 다른 반환값을 반환
				// When loading a file with a different DeclType, return value other than EResult_OK is returned
				CFLRect<double> flrLoad = new CFLRect<double>();

				// Rect 에 FigureArray 로드했으므로 실패 // Failed because we loaded FigureArray into Rect
				res = flrLoad.Load("FigureArray");

				// Rect 에 Rect 파일을 로드했으므로 파일을 로드했으므로 성공 EResult_OK 반환
				// Loaded the Rect file into Rect, so we loaded the file, so return EResult_OK
				res = flrLoad.Load("FLRect");

				//다른 DeclType 인 파일을 Load할 경우 반환값이 EResult_OK 가 아닌 다른 반환값을 반환
				// When loading a file with a different DeclType, return value other than EResult_OK is returned
				CFLFigureArray flfaLoad = new CFLFigureArray();

				// FigureArray 에 Rect 파일을 로드했으므로 실패 // Failed because Rect file was loaded into FigureArray
				res = flfaLoad.Load("FLRect");

				// FigureArray 에 FigureArray 파일을 로드했으므로 성공 EResult_OK 반환
				// Success returned EResult_OK because FigureArray file was loaded into FigureArray
				res = flfaLoad.Load("FigureArray");

				Console.WriteLine("Loaded Figure\n");

				strFigure = String.Format("Rect : {0}\n", CFigureUtilities.ConvertFigureObjectToString(flrLoad));
				Console.WriteLine("{0}", strFigure);

				strFigure = String.Format("Figure Array : {0}\n\n", CFigureUtilities.ConvertFigureObjectToString(flfaLoad));
				Console.WriteLine("{0}", strFigure);

				// DestinationView의 0번 레이어에 그리기 // Draw on Layer 0 of DestinationView
				DstLayer0.DrawFigureImage(flrLoad, EColor.MAGENTA);
				DstLayer0.DrawFigureImage(flfaLoad, EColor.LIME);

				// 이미지 뷰를 갱신 합니다. // Update image view
				viewImage[0].Invalidate(true);
				viewImage[1].Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to close
				while(viewImage[0].IsAvailable() || viewImage[1].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

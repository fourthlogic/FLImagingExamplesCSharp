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

namespace ShapeMatch
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
				if((res = fliImage.Load("../../ExampleImages/ShapeMatch/Circle Match.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImage.Create(400, 0, 1424, 768)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // display the image in the imageview
				if((res = viewImage.SetImagePtr(ref fliImage)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view.");
					break;
				}

				// Shape Match 객체 생성 // Create Shape Match object
				CShapeMatchCircle shapeMatch = new CShapeMatchCircle();

				// 학습할 원의 반지름 설정 // Set the radius of the circle to learn
				shapeMatch.SetRadius(30);

				// 도형 학습 // Learn shape
				if((res = shapeMatch.Learn()).IsFail())
				{
					ErrorPrint(res, "Failed to Learn.");
					break;
				}

				// 검출할 이미지 설정 // Set image to detect
				shapeMatch.SetSourceImage(ref fliImage);
				// 검출 시 사용될 유효 변경 크기범위를 설정합니다. // Set the effective change size range to be used for detection.
				shapeMatch.SetScaleRatio(0.9, 1.1);
				// 검출할 객체의 색상을 설정합니다. // Sets the color of the object to be detected.
				shapeMatch.SetObjectColor(EShapeMatchObjectColor.Bright);

				// 알고리즘 수행 // Execute the algorithm
				if((res = shapeMatch.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to Execute.");
					break;
				}

				CGUIViewImageLayer layer = viewImage.GetLayer(0);
				long i64ResultCount = shapeMatch.GetResultCount();

				for(long i = 0; i < i64ResultCount; ++i)
				{
					CShapeMatchCircleResult matchResult;

					shapeMatch.GetResult(i, out matchResult);

					// 도형 검출 결과를 Console창에 출력합니다. // Output the shape detection result to the console window.
					Console.Write(" < Instance : {0} >\n", i);
					Console.Write("  1. Shape Type : Circle\n");
					Console.Write("    Pivot X: {0}\n", matchResult.flpPivot.x);
					Console.Write("    Pivot Y: {0}\n", matchResult.flpPivot.y);
					Console.Write("    Radius    : {0}\n", matchResult.flcResultObject.radius);
					Console.Write("  2. Score : {0}\n  3. Scale : {1}\n\n", matchResult.f32Score, matchResult.f32Scale);

					if((res = layer.DrawFigureImage(matchResult.flcResultObject, EColor.CYAN, 3)).IsFail())
					{
						ErrorPrint(res, "Failed to draw figure\n");
						break;
					}

					string str = String.Format("Score : {0}\nScale : {1}\nPivot : ({2}, {3})", matchResult.f32Score, matchResult.f32Scale, matchResult.flpPivot.x, matchResult.flpPivot.y);

					if((res = layer.DrawTextImage(matchResult.flpPivot, str, EColor.YELLOW, EColor.BLACK, 15)).IsFail())
					{
						ErrorPrint(res, "Failed to draw text\n");
						break;
					}
				}

				// 이미지 뷰를 갱신 합니다. // Update the image view.
				viewImage.Invalidate(true);
				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the imageview to close
				while(viewImage.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

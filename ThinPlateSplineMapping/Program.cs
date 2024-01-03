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

namespace ThinPlateSplineMapping
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
			CGUIViewImage[] viewImage = { new CGUIViewImage(), new CGUIViewImage(), new CGUIViewImage() };

			CResult eResult;

			do
			{
				// Source Coordinate View 생성 // Create Source Coordinate View
				if((eResult = viewImage[0].Create(200, 0, 700, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Destination Coordinate View 생성 // Create Destination Coordinate View
				if((eResult = viewImage[1].Create(700, 0, 1200, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// Restore Coordinate View 생성 // Create Restore Coordinate View
				if((eResult = viewImage[2].Create(1200, 0, 1700, 500)).IsFail())
				{
					ErrorPrint(eResult, "Failed to create the image view.\n");
					break;
				}

				// 각 이미지 뷰의 시점을 동기화 한다. // Synchronize the viewpoint of each image view.
				if((eResult = viewImage[0].SynchronizePointOfView(ref viewImage[1])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view\n");
					break;
				}
				if((eResult = viewImage[1].SynchronizePointOfView(ref viewImage[2])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize view\n");
					break;
				}

				// 각 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the position of each image view window
				if((eResult = viewImage[0].SynchronizeWindow(ref viewImage[1])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}
				if((eResult = viewImage[1].SynchronizeWindow(ref viewImage[2])).IsFail())
				{
					ErrorPrint(eResult, "Failed to synchronize window.\n");
					break;
				}

				// 화면상에 잘 보이도록 좌표 95배율을 적용 // Apply 95 magnification to the coordinates so that they can be seen clearly on the screen
				double f64Scale = 95;
				// 화면상에 잘 보이도록 시점 Offset 조정 // Adjust the viewpoint offset so that it can be seen clearly on the screen
				double f64Offset = 50;

				viewImage[0].SetScale(f64Scale);
				viewImage[0].SetOffset(new CFLPoint<double>(f64Offset, f64Offset));

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer[] layer = { viewImage[0].GetLayer(0), viewImage[1].GetLayer(0), viewImage[2].GetLayer(0) };

				// 화면상 좌표(고정 좌표)에 Source 좌표 View 임을 표시
				layer[0].DrawTextCanvas(new CFLPoint<int>(0, 0), "Source Coordinate", EColor.YELLOW, EColor.BLACK, 30);
				// 화면상 좌표(고정 좌표)에 Target 좌표 View 임을 표시
				layer[1].DrawTextCanvas(new CFLPoint<int>(0, 0), "Target Coordinate", EColor.YELLOW, EColor.BLACK, 30);
				// 화면상 좌표(고정 좌표)에 Restore 좌표 View 임을 표시
				layer[2].DrawTextCanvas(new CFLPoint<int>(0, 0), "Restore Coordinate (from Target)", EColor.YELLOW, EColor.BLACK, 30);



				// 좌표 매핑용 클래스 선언 // Class declaration for coordinate mapping
				CThinPlateSplineMapping tps = new CThinPlateSplineMapping();

				// 그리드를 (5,5)로 초기화 // initialize the grid to (5,5)
				CFLPoint<int> flpGridSize = new CFLPoint<int>(5, 5);

				// 만약 기존 저장된 매핑 데이터가 있다면 해당 데이터를 로드합니다. // If there is previously saved mapping data, load the data.
				// 두번째 실행부터는 파일이 생성될 것이기 때문에 아래 세팅과정을 수행하지 않고 지나가게 됩니다. // Since the file will be created from the second execution, the setting process below will be skipped.
				// 계속 새로 데이터를 생성하는것을 테스트 하려 한다면 아래 Load함수와 관련된 if문 1줄을 삭제하면 됩니다. // If you want to test continuously creating new data, you can delete one line of the if statement related to the Load function below.
				if((eResult = tps.Load("MappingData.fltps")).IsFail())
				{
					CFLPoint<int> flpGridIndex = new CFLPoint<int>();
					for(int y = 0; y < flpGridSize.y; ++y)
					{
						flpGridIndex.y = y;

						for(int x = 0; x < flpGridSize.x; ++x)
						{
							flpGridIndex.x = x;

							// Grid Index와 같은 좌표로 Source 좌표를 설정 // Set the source coordinates to the same coordinates as the Grid Index
							CFLPoint<double> flpSource = new CFLPoint<double>(flpGridIndex.x, flpGridIndex.y);
							// Grid Index와 같은 좌표에서 미세한 랜덤 값을 부여해서 좌표를 왜곡 // Distort the coordinates by giving fine random values ​​at the same coordinates as the Grid Index
							CFLPoint<double> flpDistortion = new CFLPoint<double>(flpGridIndex.x + CRandomGenerator.Double(-0.15, 0.15), flpGridIndex.y + CRandomGenerator.Double(-0.15, 0.15));

							// 위에서 설정한 좌표들을 바탕으로 ThinPlateSplineMapping 클래스에 하나의 Vertex를 설정
							// Set one Control Point in the ThinPlateSplineMapping class based on the coordinates set above
							tps.AddControlPoint(flpSource, flpDistortion);
						}
					}

					// 설정한 데이터를 매핑 가능하도록 마무리 작업을 진행합니다.
					// 반드시 이 함수를 호출해서 결과가 OK가 나와야 매핑 사용이 가능합니다.
					// We proceed with the finishing work so that the set data can be mapped.
					// You must call this function and the result must be OK to use the mapping.
					if((eResult = tps.Finish()).IsFail())
					{
						ErrorPrint(eResult, "Failed to finalize\n");
						break;
					}

					// Finalize 까지 완료된 상태라면 Save를 통해 파일에 저장이 가능하며,
					// 추후 Load함수를 통해 로드 시 위의 Initialize -> Set -> Finalize 과정을 생략할 수 있습니다.
					// If Finalize is completed, it can be saved to a file through Save.
					// When loading through the Load function later, the above Initialize -> Set -> Finalize process can be omitted.
					if((eResult = tps.Save("MappingData.fltps")).IsFail())
					{
						ErrorPrint(eResult, "Failed to save mapping data\n");
						break;
					}
				}

				// 세팅이 완료된 ThinPlateSplineMapping 클래스를 이용해 변환을 하는 단계입니다.
				// This step is to convert using the ThinPlateSplineMapping class that has been set.

				// ThinPlateSplineMapping 클래스에 설정된 Vertex 정보를 화면에 Display
				// Display the vertex information set in the ThinPlateSplineMapping class on the screen
				for(int k = 0; k < tps.GetControlPointCount(); ++k)
				{
					CFLPoint<double> flpSource = new CFLPoint<double>();
					CFLPoint<double> flpTarget = new CFLPoint<double>();

					tps.GetControlPoint(k, out flpSource, out flpTarget);

					for(int i = 0; i < 3; ++i)
					{
						// Target Vertex를 각 View Layer에 Drawing // Drawing the target vertex to each view layer
						if((eResult = layer[i].DrawFigureImage(flpTarget, EColor.BLUE, 3)).IsFail())
						{
							ErrorPrint(eResult, "Failed to draw figure objects on the image view.\n");
							break;
						}

						// Source Vertex를 각 View Layer에 Drawing // Drawing source vertex to each view layer
						if((eResult = layer[i].DrawFigureImage(flpSource, EColor.RED, 3)).IsFail())
						{
							ErrorPrint(eResult, "Failed to draw figure objects on the image view.\n");
							break;
						}
					}

					Console.WriteLine("Source Vertex : ({0:.000},{1:.000})", flpSource.x, flpSource.y);
					Console.WriteLine("Target Vertex : ({0:.000},{1:.000})", flpTarget.x, flpTarget.y);
				}


				// 정점 사이를 대략 10등분, 즉 하나의 영역에 100개의 보간영역을 테스트
				// Divide the vertices into approximately 10 equal parts, that is, test 100 interpolation areas in one area
				double f64Slice = 10;

				CFLPoint<double> flpdSource = new CFLPoint<double>(); // Source 좌표 // Source coordinates
				CFLPoint<double> flpdTarget = new CFLPoint<double>(); // Target 좌표 // Target coordinates
				CFLPoint<double> flpdConvertedSource = new CFLPoint<double>(); // Target 좌표를 다시 Source로 변환, 검증 용도의 좌표 // Convert target coordinates back to source, coordinates for verification purposes

				for(int y = 0; y <= (flpGridSize.y - 1) * f64Slice; ++y)
				{
					flpdSource.y = (y / f64Slice);

					for(int x = 0; x <= (flpGridSize.x - 1) * f64Slice; ++x)
					{
						flpdSource.x = (x / f64Slice);

						// 연산을 수행할 Source 좌표를 View에 Display // Display the source coordinates to perform the operation on the View
						if((eResult = layer[0].DrawFigureImage(flpdSource, EColor.YELLOW)).IsFail())
						{
							ErrorPrint(eResult, "Failed to draw figure objects on the image view.\n");
							break;
						}

						// Source 좌표의 공간을 Target 좌표 공간으로 변환 // Convert the space of source coordinates to target coordinate space
						if(tps.ConvertSourceToTarget(flpdSource, out flpdTarget).IsOK())
						{
							// Source 좌표에서 Target 좌표로 변환된 좌표를 View에 Display // Display coordinates converted from source coordinates to target coordinates on the View
							if((eResult = layer[1].DrawFigureImage(flpdTarget, EColor.LIME)).IsFail())
							{
								ErrorPrint(eResult, "Failed to draw figure objects on the image view.\n");
								break;
							}

							// 변환된 Target 좌표를 그대로 Source 좌표로 변환해서 자신의 위치로 제대로 돌아오는지 검증
							// Verify that the converted target coordinates are converted to source coordinates as they are and return to their own position properly
							if(tps.ConvertTargetToSource(flpdTarget, out flpdConvertedSource).IsOK())
							{
								Console.WriteLine("Source ({0:.000},{1:.000}) -> Target ({2:.000},{3:.000}) -> Source ({4:.000},{5:.000})", flpdSource.x, flpdSource.y, flpdTarget.x, flpdTarget.y, flpdConvertedSource.x, flpdConvertedSource.y);

								// 변환된 좌표를 View에 Display // Display the converted coordinates in the View
								if((eResult = layer[2].DrawFigureImage(flpdConvertedSource, EColor.CYAN)).IsFail())
								{
									ErrorPrint(eResult, "Failed to draw figure objects on the image view.\n");
									break;
								}
							}
						}
					}
				}

				// 이미지 뷰들을 갱신 합니다. // Update the image views.
				for(int i = 0; i < 3; ++i)
					viewImage[i].Invalidate(true);

				// 이미지 뷰가 셋중에 하나라도 꺼지면 종료로 간주 // Consider closed when any of the three image views are turned off
				while(viewImage[0].IsAvailable() && viewImage[1].IsAvailable() && viewImage[2].IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

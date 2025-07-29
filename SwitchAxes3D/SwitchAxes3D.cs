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
using FLImagingCLR.ThreeDim;
using System.Diagnostics;
using System.Collections;

namespace FLImagingExamplesCSharp
{
	class SwitchAxes3D
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

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst = new CGUIViewImage();
			CGUIView3D view3DSrc = new CGUIView3D();
			CGUIView3D view3DDst = new CGUIView3D();
			CFL3DObject floSrc = new CFL3DObject();
			CFL3DObject floDst = new CFL3DObject();

			// 알고리즘 동작 결과 // Algorithm execution result
			CResult res = new CResult();

			do
			{
				// Source 3D 뷰 생성 // Create the Source 3D view
				if((res = view3DSrc.Create(100, 0, 600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				// Destination 3D 뷰 생성 // Create the destination 3D view
				if((res = view3DDst.Create(600, 0, 1100, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the 3D view.\n");
					break;
				}

				floSrc.Load("../../ExampleImages/DistanceTransform3D/binary-vertex.ply");

				// 알고리즘 객체 생성 // Create algorithm object
				CSwitchAxes3D distanceTransform = new CSwitchAxes3D();

				// 파라미터 설정 // Set parameter
				distanceTransform.SetSourceObject(ref floSrc);
				distanceTransform.SetDestinationObject(ref floDst);
                distanceTransform.SetAxisMappings(CSwitchAxes3D.EAxisMapping.From_PX, CSwitchAxes3D.EAxisMapping.From_NY, CSwitchAxes3D.EAxisMapping.Deduce, false);

                // 앞서 설정된 파라미터대로 알고리즘 수행 // Execute algorithm according to previously set parameters
                if ((res = distanceTransform.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.\n");
					break;
				}

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIView3DLayer layer3DSrc = view3DSrc.GetLayer(0);
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layer3DSrc.Clear();
				layer3DDst.Clear();

				// Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다. // With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
				view3DSrc.PushObject(floSrc);
				view3DSrc.ZoomFit();

				view3DDst.PushObject(floDst);
				view3DDst.ZoomFit();

				CFLPoint<double> flp = new CFLPoint<double>();

				if((res = layer3DSrc.DrawTextCanvas(flp, "Source Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail() ||
				   (res = layer3DDst.DrawTextCanvas(flp, "Destination Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 합니다. // Update image view
				view3DSrc.Invalidate(true);
				view3DDst.Invalidate(true);

				// 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
				while(view3DSrc.IsAvailable() && view3DDst.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}

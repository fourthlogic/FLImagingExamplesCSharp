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
using FLImagingCLR.ThreeDim;
using System.Diagnostics;
using System.Collections;

namespace FLImagingExamplesCSharp
{
	class PointCloudGenerator3D
	{
        public static void ErrorPrint(CResult cResult, string str)
		{
            if (str.Length > 1)
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
			CGUIView3D view3DDst = new CGUIView3D();

            // 알고리즘 동작 결과 // Algorithm execution result
            CResult res = new CResult();

            do
			{
                if ((res = view3DDst.Create(600, 0, 1100, 500)).IsFail())
				{
                    ErrorPrint(res, "Failed to create the image view.\n");
                    break;
                }

                view3DDst.PushObject(new CFL3DObject());
                var viewObject = view3DDst.GetView3DObject(0);
                var floDst = viewObject.Get3DObject();

                CPointCloudGenerator3D pointCloudGenerator = new CPointCloudGenerator3D();

                // 파라미터 설정 // Set parameter
                pointCloudGenerator.SetDestinationObject(ref floDst);
                pointCloudGenerator.EnableColorGeneration(true);
				pointCloudGenerator.EnableNormalGeneration(false);

				pointCloudGenerator.AddPredefinedObject(new CPointCloudGenerator3D.SCountInfo(true, 0, 0, 0), EPredefinedObject.Regular_DodecaHedron, new TPoint3<Byte>(255, 255, 255));
				pointCloudGenerator.AddPredefinedObject(new CPointCloudGenerator3D.SCountInfo(false, 4000, 0, 0), EPredefinedObject.Regular_DodecaHedron, new TPoint3<Byte>(255, 0, 0));
				pointCloudGenerator.AddPredefinedObject(new CPointCloudGenerator3D.SCountInfo(false, 0, 20000, 0), EPredefinedObject.Regular_DodecaHedron, new TPoint3<Byte>(0, 255, 0));
				pointCloudGenerator.AddPredefinedObject(new CPointCloudGenerator3D.SCountInfo(false, 0, 0, 100000), EPredefinedObject.Regular_DodecaHedron, new TPoint3<Byte>(0, 0, 255));

				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIView3DLayer layer3DDst = view3DDst.GetLayer(0);

                // 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
                layer3DDst.Clear();

                // Destination 이미지가 새로 생성됨으로 Zoom fit 을 통해 디스플레이 되는 이미지 배율을 화면에 맞춰준다. // With the newly created Destination image, the image magnification displayed through Zoom fit is adjusted to the screen.
                view3DDst.PushObject(floDst);
                view3DDst.ZoomFit();

                CFLPoint<double> flp = new CFLPoint<double>();

                if ((res = layer3DDst.DrawTextCanvas(flp, "Destination Object", EColor.YELLOW, EColor.BLACK, 20)).IsFail())
				{
                    ErrorPrint(res, "Failed to draw text.\n");
                    break;
                }

				// 앞서 설정된 파라미터 대로 알고리즘 수행 // Execute algorithm according to previously set parameters
				if((res = pointCloudGenerator.Execute()).IsFail())
				{
					ErrorPrint(res, "Failed to execute.");
					break;
				}

                // 출력 뷰의 시점을 계산 // Calculate the viewpoint of destination view
				viewObject.UpdateAll();
				view3DDst.UpdateObject(0);
				view3DDst.ZoomFit();

                // 이미지 뷰, 3D 뷰가 종료될 때 까지 기다림 // Wait for the image and 3D view to close
                while(view3DDst.IsAvailable())
				{
					if((res = pointCloudGenerator.Execute()).IsFail())
					{
						ErrorPrint(res, "Failed to execute.");
						break;
					}

					if(!view3DDst.IsAvailable())
						break;

					viewObject.UpdateVertex(true);
					view3DDst.UpdateObject(0);

					view3DDst.UpdateScreen();

					Thread.Sleep(1);
                }
            }
            while (false);
        }
	}
}

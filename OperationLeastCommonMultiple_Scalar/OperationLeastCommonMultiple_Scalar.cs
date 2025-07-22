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

namespace OperationLeastCommonMultiple_Scalar
{
	class OperationLeastCommonMultiple_Scalar
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
			CFLImage fliSrcImage = new CFLImage();
			CFLImage fliDstImage0 = new CFLImage();
			CFLImage fliDstImage1 = new CFLImage();

			// 이미지 뷰 선언 // Declare the image view
			CGUIViewImage viewImageSrc = new CGUIViewImage();
			CGUIViewImage viewImageDst0 = new CGUIViewImage();
			CGUIViewImage viewImageDst1 = new CGUIViewImage();

			CResult res;

			do
			{
				// Source 이미지 로드 // Load the source image
				if((res = fliSrcImage.Load("../../ExampleImages/OperationLeastCommonMultiple/Gradient.flif")).IsFail())
				{
					ErrorPrint(res, "Failed to load the image file.\n");
					break;
				}

				// 이미지 뷰 생성 // Create image view
				if((res = viewImageSrc.Create(100, 0, 600, 500)).IsFail() ||
					(res = viewImageDst0.Create(600, 0, 1100, 500)).IsFail() ||
					(res = viewImageDst1.Create(1100, 0, 1600, 500)).IsFail())
				{
					ErrorPrint(res, "Failed to create the image view.\n");
					break;
				}

				// 두 이미지 뷰의 시점을 동기화 한다 // Synchronize the viewpoints of the two image views. .
				// ref 파라미터를 입력 받는 함수는 리턴이 tuple로 생성되며 [return], [ref 0], ... [ref n-1] 형태로 tuple 을 반환한다. // A function that receives ref parameters returns a tuple structured as [return], [ref 0], ... [ref n-1].
				if((res= viewImageSrc.SynchronizePointOfView(ref viewImageDst0)).IsFail() ||
					(res= viewImageSrc.SynchronizePointOfView(ref viewImageDst1)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize view. \n");
					break;
				}

				// 두 이미지 뷰 윈도우의 위치를 동기화 한다 // Synchronize the positions of the two image view windows
				// ref 파라미터를 입력 받는 함수는 리턴이 tuple로 생성되며 [return], [ref 0], ... [ref n-1] 형태로 tuple 을 반환한다. // A function that receives ref parameters returns a tuple structured as [return], [ref 0], ... [ref n-1].
				if((res= viewImageSrc.SynchronizeWindow(ref viewImageDst0)).IsFail() ||
					(res= viewImageSrc.SynchronizeWindow(ref viewImageDst1)).IsFail())
				{
					ErrorPrint(res, "Failed to synchronize window. \n");
					break;
				}

				// 이미지 뷰에 이미지를 디스플레이 // Display the image in the image view
				// ref 파라미터를 입력 받는 함수는 리턴이 tuple로 생성되며 [return], [ref 0], ... [ref n-1] 형태로 tuple 을 반환한다. // A function that receives ref parameters returns a tuple structured as [return], [ref 0], ... [ref n-1].
				if((res = viewImageSrc.SetImagePtr(ref fliSrcImage)).IsFail() ||
					(res = viewImageDst0.SetImagePtr(ref fliDstImage0)).IsFail() ||
					(res = viewImageDst1.SetImagePtr(ref fliDstImage1)).IsFail())
				{
					ErrorPrint(res, "Failed to set image object on the image view. \n");
					break;
				}


				// 알고리즘 객체 생성 // Create algorithm object
				COperationLeastCommonMultiple algObject = new COperationLeastCommonMultiple();

				if((res = algObject.SetSourceImage(ref fliSrcImage)).IsFail()) break;
				if((res = algObject.SetDestinationImage(ref fliDstImage0)).IsFail()) break;
				if((res = algObject.SetOperationSource(EOperationSource.Scalar)).IsFail()) break;
				CMultiVar<double> mvScalarValue1 = new CMultiVar<double>(5);
				if((res = algObject.SetScalarValue(mvScalarValue1)).IsFail()) break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = (algObject.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}

				if((res = algObject.SetDestinationImage(ref fliDstImage1)).IsFail()) break;
				CMultiVar<double> mvScalarValue2 = new CMultiVar<double>(17);
				if((res = algObject.SetScalarValue(mvScalarValue2)).IsFail()) break;

				// 알고리즘 수행 // Execute the algorithm
				if((res = (algObject.Execute())).IsFail())
				{
					ErrorPrint(res, "Failed to execute the algorithm.");
					break;
				}


				// 화면에 출력하기 위해 Image View에서 레이어 0번을 얻어옴 // Obtain layer 0 number from image view for display
				// 이 객체는 이미지 뷰에 속해있기 때문에 따로 해제할 필요가 없음 // This object belongs to an image view and does not need to be released separately
				CGUIViewImageLayer layerSrc = viewImageSrc.GetLayer(0);
				CGUIViewImageLayer layerDst0 = viewImageDst0.GetLayer(0);
				CGUIViewImageLayer layerDst1 = viewImageDst1.GetLayer(0);

				// 기존에 Layer에 그려진 도형들을 삭제 // Clear the figures drawn on the existing layer
				layerSrc.Clear();
				layerDst0.Clear();
				layerDst1.Clear();

				// 이미지 뷰 정보 표시 // Display image view information
				CFLPoint<double> flpZero = new CFLPoint<double>(0, 0);
				if((res = (layerSrc.DrawTextCanvas(flpZero, "Source Image", EColor.YELLOW, EColor.BLACK, 20))).IsFail() ||
					(res = (layerDst0.DrawTextCanvas(flpZero, "Destination1 Image(LCM 5)", EColor.YELLOW, EColor.BLACK, 20))).IsFail() ||
					(res = (layerDst1.DrawTextCanvas(flpZero, "Destination2 Image(LCM 17)", EColor.YELLOW, EColor.BLACK, 20))).IsFail())
				{
					ErrorPrint(res, "Failed to draw text.\n");
					break;
				}

				// 이미지 뷰를 갱신 // Update image view
				viewImageSrc.Invalidate(true);
				viewImageDst0.Invalidate(true);
				viewImageDst1.Invalidate(true);

				// 이미지 뷰가 종료될 때 까지 기다림 // Wait for the image view to 
				while(viewImageSrc.IsAvailable() && viewImageDst0.IsAvailable() && viewImageDst1.IsAvailable())
					Thread.Sleep(1);
			}
			while(false);
		}
	}
}
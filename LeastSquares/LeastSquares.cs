using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Numerics;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace FLImagingExamplesCSharp
{
	class LeastSquares
	{
		[STAThread]
		static void Main(string[] args)
		{
			// You must call the following function once
			// before using any features of the FLImaging(R) library
			CLibraryUtilities.Initialize();

			String[] arrStrEquation = {
				"Linear equation",
				"Quadratic equation",
				"Cubic equation",
				"Quartic equation",
				"Quintic equation"
			};

			while(true)
			{
				do
				{
					Console.Write("Please input generate sample data count: ");

					// 계수 문자열을 입력 받는다. // Receive the count string.
					String strInput = Console.ReadLine();

					if(strInput.Length == 0)
					{
						Console.WriteLine("Please check the input.\n");
						break;
					}

					int i32DataCount = 0;

					if(int.TryParse(strInput, out i32DataCount) == false)
					{
						Console.WriteLine("Please check the input.\n");
						break;
					}

					if(i32DataCount <= 0)
					{
						Console.WriteLine("Please check the input.\n");
						break;
					}

					// 입력 받은 개수만큼 데이터를 생성한다. // Generate data as many as the input number.

					double[] arrF64DataX = new double[i32DataCount];
					double[] arrF64DataY = new double[i32DataCount];

					String strSampleData = "";

					double f64PrevX = 0.0;
					double f64PrevY = 0.0;

					Random random = new Random();

					for(int i = 0; i < i32DataCount; ++i)
					{
						if(strSampleData.Length != 0)
							strSampleData += ", ";

						arrF64DataX[i] = f64PrevX + (random.Next(i32DataCount) / 10.0);

						if(random.Next() % 2 != 0)
							arrF64DataY[i] = f64PrevY + (random.Next(i32DataCount) / 10.0);
						else
							arrF64DataY[i] = f64PrevY - (random.Next(i32DataCount) / 10.0);

						f64PrevX = arrF64DataX[i];
						f64PrevY = arrF64DataY[i];

						String strFormat = "";
						strFormat = String.Format("({0}, {1})", arrF64DataX[i], arrF64DataY[i]);

						strSampleData += strFormat;
					}

					Console.WriteLine("Sample Data");
					Console.WriteLine(strSampleData);
					Console.Write("\n");

					// LeastSquaresD 객체 생성 // Create LeastSquaresD object
					CLeastSquares<double> ls = new CLeastSquares<double>();

					// 데이터를 할당 // Assign data
					ls.Assign(arrF64DataX, arrF64DataY, i32DataCount);

					for(int i = 1; i <= 5; ++i)
					{
						// 계수 값을 받기 위해 List 생성 // Create List to receive coefficient values
						List<double> listF64Output = new List<double>();

						// R square 값을 받기 위해 double 선언 // Declare double to receive R square value
						double f64TRSqr = 0.0;

						// 다항식 계수를 얻는다. // Get polynomial coefficients
						ls.GetPoly(i, ref listF64Output, ref f64TRSqr);

						String strEquation = "";

						int i32Count = listF64Output.Count();

						if(i32Count == 0)
							continue;

						List<Complex> listCoef = new List<Complex>();

						// 얻어온 계수로 다항식을 만든다. // Create a polynomial with the obtained coefficients.
						for(int j = 0; j < i32Count; ++j)
						{
							double f64Coef = listF64Output[j];

							listCoef.Add(new Complex(f64Coef, 0.0));

							if(f64Coef == 0.0)
								continue;

							if(strEquation.Length != 0 && f64Coef > 0.0)
								strEquation += " + ";

							String strFormat = "";

							if(j == i32Count - 2)
							{
								strFormat = String.Format("{0}*x", f64Coef);
							}
							else if(j == i32Count - 1)
							{
								strFormat = String.Format("{0}", f64Coef);
							}
							else
							{
								strFormat = String.Format("{0}*x^{1}", f64Coef, i32Count - 1 - j);
							}

							strEquation += strFormat;
						}

						if(strEquation.Length == 0)
							continue;

						String strDegree = arrStrEquation[i - 1];

						String strR = "";
						strR = String.Format("R square value: {0}", f64TRSqr);

						Console.WriteLine(strDegree);
						Console.WriteLine(strR);
						Console.WriteLine(strEquation);

						// 방정식의 해를 얻기위해 List<Complex> 생성 // Create List<Complex> to get solution of equation
						List<Complex> listEquationResult = new List<Complex>();

						// 방정식의 해를 얻어온다. // Get the solution of the equation.
						CEquation.Solve(listCoef, ref listEquationResult);

						// 방정식의 해를 표시한다. // Display the solution of the equation.
						String strResult = "Result \n";

						for(int j = 0; j < listEquationResult.Count; ++j)
						{
							Complex cpxResult = listEquationResult[j];

							String strCpx = "";
							if(cpxResult.Imaginary == 0.0)
								strCpx = String.Format("{0}", cpxResult.Real);
							else if(cpxResult.Imaginary > 0.0)
								strCpx = String.Format("{0}+{1}i", cpxResult.Real, cpxResult.Imaginary);
							else
								strCpx = String.Format("{0}{1}i", cpxResult.Real, cpxResult.Imaginary);

							strResult += strCpx + "\n";
						}

						strResult += "\n";

						Console.WriteLine(strResult);
					}
				}
				while(false);
			}
		}
	}
}

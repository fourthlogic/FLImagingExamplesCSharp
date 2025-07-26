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
    class Equation
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

            while (true)
			{
                String strResult = "";

                do
				{
                    Console.WriteLine("Please input equation coefficient.");
                    Console.WriteLine("ex) 7.2, 3.8, 10, 2.4");
                    Console.WriteLine("    7.2*x^3 + 3.8*x^2 + 10*x + 2.4\n");
                    Console.Write("Input: ");

					// 계수 문자열을 입력 받는다. // Receive the count string.
					String strInput = Console.ReadLine();

                    if (strInput.Length == 0)
                        break;

					// 계수 값을 담기위해 List<Complex> 생성 // Create List<Complex> to hold coefficient values
					List<Complex> listCoef = new List<Complex>();

					// 입력 받은 문자열을 ',' 으로 구분하여 double 값으로 변환한다. // Separate the input string with ',' and convert it to a double value.
					String[] arrStrInput = strInput.Split(',');

                    foreach (var input in arrStrInput)
					{
                        if (input.Length == 0)
                            break;

                        if (input.Equals("\n"))
                            break;

                        double f64Input = 0;

                        if (Double.TryParse(input, out f64Input) == false)
                            continue;

                        listCoef.Add(new Complex(f64Input, 0.0));
                    }

					// 최상위 계수가 0 이면 제거해준다. // If the top coefficient is 0, remove it.
					while(listCoef.Count != 0)
					{
                        Complex cpxValue = listCoef[0];

                        if (cpxValue.Real != 0.0)
                            break;

                        listCoef.RemoveAt(0);
                    }

                    int i32Count = listCoef.Count;

                    if (i32Count < 2)
                        break;

                    Console.Write("\n");

					// 입력 받은 계수로 수식을 만들어서 표시한다. // Create and display a formula with the entered coefficients.
					String strDegree = "";
                    if (listCoef.Count < 7)
                        strDegree = arrStrEquation[i32Count - 2];
                    else
                        strDegree = String.Format("{0}th degree equation", i32Count - 1);

                    Console.WriteLine(strDegree);

                    String strEquation = "";

                    for (int i = 0; i < i32Count; ++i)
					{
                        double f64Coef = listCoef[i].Real;

                        if (f64Coef == 0.0)
                            continue;

                        if (strEquation.Length != 0 && f64Coef > 0.0)
                            strEquation += " + ";

                        String strFormat = "";
                        if (i == i32Count - 2)
						{
                            strFormat = String.Format("{0}*x", f64Coef);
                        }
                        else if (i == i32Count - 1)
						{
                            strFormat = String.Format("{0}", f64Coef);
                        }
                        else
						{
                            strFormat = String.Format("{0}*x^{1}", f64Coef, i32Count - 1 - i);
                        }

                        strEquation += strFormat;
                    }

                    Console.WriteLine(strEquation);
                    Console.Write("\n");

					// 방정식의 해를 얻기위해 List<Complex> 생성 // Create List<Complex> to get solution of equation
					List<Complex> listEquationResult = new List<Complex>();

					// 방정식의 해를 얻어온다. // Get the solution of the equation.
					CEquation.Solve(listCoef, ref listEquationResult);

                    if (listEquationResult.Count == 0)
                        break;

					// 방정식의 해를 표시한다. // Display the solution of the equation.
					strResult = "Result \n";

                    for (int i = 0; i < listEquationResult.Count; ++i)
					{
                        Complex cpxResult = listEquationResult[i];

                        String strCpx = "";
                        if (cpxResult.Imaginary == 0.0)
                            strCpx = String.Format("{0}", cpxResult.Real);
                        else if (cpxResult.Imaginary > 0.0)
                            strCpx = String.Format("{0}+{1}i", cpxResult.Real, cpxResult.Imaginary);
                        else
                            strCpx = String.Format("{0}{1}i", cpxResult.Real, cpxResult.Imaginary);

                        strResult += strCpx + "\n";
                    }
                }
                while (false);

                if (strResult.Length == 0)
                    strResult = "Please check the input.\n";

                Console.WriteLine(strResult);
            }
        }
    }
}

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

namespace LeastSquares
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
		{
			// 조합 객체 선언 // Declare a combination object
			CCombination C = new CCombination();

            while (true)
            {
                string flstrResult = "";

                do
                {
                    Console.Write("Please input n and k as n, k.\n");
                    Console.Write("Combination : k objects are selected from a set of n objects to produce subsets withref ordering.\n");
                    Console.Write("ex) 6, 2\n");
                    Console.Write("Input: ");

					// n, k 문자열을 입력 받는다. // Receive n, k strings.
					String strInput = Console.ReadLine();

                    if (strInput == "")
                        break;

					// 입력 받은 문자열을 ',' 으로 구분하여 int 값으로 변환한다. // Separates the input string with ',' and converts it to an int value.
					String[] arrStrInput = strInput.Split(',');

                    int n = -1;
                    int k = -1;
                    int nCount = 0;

                    foreach (var input in arrStrInput)
                    {
                        if (input.Length == 0)
                            break;

                        if (input.Equals("\n"))
                            break;

                        if (nCount == 0)
                            int.TryParse(input, ref n);
                        else if (nCount == 1)
                            int.TryParse(input, ref k);
                        else
                            break;

                        nCount++;
                    }

                    if (k <= 0 || n <= 0 || n < k)
                    {
                        flstrResult = "\nCount : 0";
                        break;
                    }


					// nCk, n 개에서 k 개를 선택하는 조합 // nCk, a combination of selecting k objects from n objects
					C.SetMax(n);
                    C.SetSelection(k);

					// 조합을 계산 // Calculate combinations
					C.Calculate();

					// 조합 결과값 얻기 // Get combination result
					List<List<int>> listCombination = new List<List<int>>();
                    CResult cResult = C.GetResult(ref listCombination);

                    string flstrCombination = "";
                    int i64CombinationCnt = 0;

                    for (int i = 0; i < listCombination.Count; i++)
                    {
                        flstrCombination += "(";

                        for (int j = 0; j < listCombination[i].Count; j++)
                            flstrCombination += String.Format(" {0} ", listCombination[i][j]);

                        flstrCombination += ")\n";

                        i64CombinationCnt++;
                    }

                    string flstrCnt = String.Format("\nCount : {0}", i64CombinationCnt);

                    flstrResult = flstrCombination + flstrCnt;
                }
                while (false);

                if (flstrResult == "")
                    flstrResult = "Please check the input.\n";

                flstrResult += "\n\n";

                Console.Write(flstrResult);
            }
        }
    }
}

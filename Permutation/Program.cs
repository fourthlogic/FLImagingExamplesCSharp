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

namespace Permutation
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			// 순열 객체 선언 // Declare permutation object
			CPermutation P = new CPermutation();

			while(true)
			{
				string flstrResult = "";

				do
				{
					Console.Write("Please input n and k as n, k.\n");
					Console.Write("Permutation : k objects are selected from a set of n objects to produce subsets with ordering.\n");
					Console.Write("ex) 6, 2\n");
					Console.Write("Input: ");

					// n, k 문자열을 입력 받는다. // Receive n, k strings.
					String strInput = Console.ReadLine();

					if(strInput == "")
						break;

					// 입력 받은 문자열을 ',' 으로 구분하여 int 값으로 변환한다. // Separates the input string with ',' and converts it to an int value.
					String[] arrStrInput = strInput.Split(',');

					int n = -1;
					int k = -1;
					int nCount = 0;

					foreach(var input in arrStrInput)
					{
						if(input.Length == 0)
							break;

						if(input.Equals("\n"))
							break;

						if(nCount == 0)
							int.TryParse(input, out n);
						else if(nCount == 1)
							int.TryParse(input, out k);
						else
							break;

						nCount++;
					}

					if(k <= 0 || n <= 0 || n < k)
					{
						flstrResult = "\nCount : 0";
						break;
					}

					// nPk, n 개에서 k 개를 선택하는 순열 // nPk, a permutation of selecting k objects from n objects
					P.SetMax(n);
					P.SetSelection(k);

					// 순열을 계산 // Calculate the permutation
					P.Calculate();

					// 순열 결과값 얻기 // Get permutation result
					List<List<int>> listPermutation = new List<List<int>>();
					CResult cResult = P.GetResult(ref listPermutation);

					string flstrPermutation = "";
					int i64PermutationCnt = 0;

					for(int i = 0; i < listPermutation.Count; i++)
					{
						flstrPermutation += "(";

						for(int j = 0; j < listPermutation[i].Count; j++)
							flstrPermutation += String.Format(" {0} ", listPermutation[i][j]);

						flstrPermutation += ")\n";

						i64PermutationCnt++;
					}

					string flstrCnt = String.Format("\nCount : {0}", i64PermutationCnt);

					flstrResult = flstrPermutation + flstrCnt;
				}
				while(false);

				if(flstrResult == "")
					flstrResult = "Please check the input.\n";

				flstrResult += "\n\n";

				Console.Write(flstrResult);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace GraphView
{
    class Program
    {
		[STAThread]
		static void Main(string[] args)
        {
            var formImageView = new FormGraphView();

            Application.Run(formImageView);
            // 예제 코드는 FormGraphView.cs 에 있습니다.
        }
    }
}

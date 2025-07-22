using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;
using System.Runtime.InteropServices;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace SNAPViewIntoDialog
{
	public partial class FormSNAPViewIntoDialog : Form
	{
		public void ErrorMessageBox(CResult cResult, string str)
		{
			string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

			if(str.Length > 1)
				strMessage += str;

			MessageBox.Show(strMessage, "Error");
		}

        public FormSNAPViewIntoDialog()
		{
			InitializeComponent();

            this.Load += new System.EventHandler(this.FormSNAPViewLoad);
			this.CenterToScreen();
		}

        private void FormSNAPViewLoad(object sender, EventArgs e)
		{
            DockSNAPViewToThis();
		}
        private void DockSNAPViewToThis()
		{
            m_viewSNAP = new CGUIViewSNAP();

            // 스냅 뷰 생성 // Create SNAP view
            CResult res = m_viewSNAP.CreateAndFitParent((ulong)this.Handle);

			// 스냅 파일 로드 // Load SNAP file
			res = m_viewSNAP.Load("C:\\Users\\Public\\Documents\\FLImaging\\FLImagingExamplesSNAP\\Advanced Functions\\Object\\Blob.flsf");

			m_viewSNAP.ZoomFit();

			if(res.IsFail())
				ErrorMessageBox(res, "");
		}

        private CGUIViewSNAP m_viewSNAP;
	}
}

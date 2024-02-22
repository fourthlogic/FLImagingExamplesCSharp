using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FLImagingCLR;
using FLImagingCLR.Base;
using FLImagingCLR.Foundation;
using FLImagingCLR.GUI;
using FLImagingCLR.ImageProcessing;
using FLImagingCLR.AdvancedFunctions;

namespace ImageViewIntoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void ErrorMessageBox(CResult cResult, string str)
        {
            string strMessage = String.Format("Error code : {0}\nError name : {1}\n", cResult.GetResultCode(), cResult.GetString());

            if (str.Length > 1)
                strMessage += str;

            MessageBox.Show(strMessage, "Error");
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CResult res;

            do
            {
                m_viewImage = new CGUIViewImage();

                // 이미지 뷰를 생성합니다.
                if ((res = m_viewImage.Create(5, 0, 505, 305)).IsFail())
                    break;

                // 메인 윈도우의 핸들을 얻어옵니다.
                IntPtr hWindow = new WindowInteropHelper(this).Handle;

                if (hWindow == null)
                {
                    res = new CResult(EResult.InvalidHandle);
                    break;
                }

                // 메인 윈도우를 이미지 뷰의 부모로 설정합니다.
                if ((res = m_viewImage.SetParentWindow((ulong)hWindow)).IsFail())
                    break;

                // 키 이벤트를 부모에게 통지하도록 설정합니다.
                m_viewImage.EnableKeyEventParentNotification(true);

                // 이미지 뷰에 Ctrl + S 단축키가 동작하지 않도록 설정합니다.
                List<int> listIgnoreShortcut = new List<int>();
                listIgnoreShortcut.Add((int)System.Windows.Forms.Keys.ControlKey);
                listIgnoreShortcut.Add((int)System.Windows.Forms.Keys.S);
                m_viewImage.AddIgnoreShortcut(listIgnoreShortcut);

                // 키 다운 이벤트 핸들러를 등록합니다.
                m_viewImage.AddKeyDownEventHandler(this.ViewImage_KeyDownEvent);
            }
            while (false);

            if(res.IsFail())
                ErrorMessageBox(res, "");
        }

        private void ViewImage_KeyDownEvent(int i32Key)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && i32Key == (int)System.Windows.Forms.Keys.S)
            {
                string strMessage = String.Format("Key Down Event: Ctrl + {0}", (char)i32Key);
                MessageBox.Show(strMessage);
            }
        }

        private CGUIViewImage m_viewImage;
    }
}

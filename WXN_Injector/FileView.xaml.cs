using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using WXN_Injector.core;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace WXN_Injector
{
    public partial class FileView : UserControl
    {
        public string _filepath { get; set; }
        public FileView(bool fileSelected = false, string filepath = null)
        {
            InitializeComponent();
            this._filepath = filepath;
            if (fileSelected)
            {
                FileSelectedPanel.Visibility = Visibility.Visible;
                CloseFileBtn.Visibility = Visibility.Visible;
                InjectButton.Visibility = Visibility.Visible;
                label_Filename.Content = Path.GetFileName(filepath);
                System.IO.FileInfo fInf = new System.IO.FileInfo(filepath);
                string sLen = fInf.Length.ToString();
                if (fInf.Length >= (1 << 30))
                    sLen = string.Format("{0}Gb", fInf.Length >> 30);
                else
                if (fInf.Length >= (1 << 20))
                    sLen = string.Format("{0}Mb", fInf.Length >> 20);
                else
                if (fInf.Length >= (1 << 10))
                    sLen = string.Format("{0}Kb", fInf.Length >> 10);
                label_filesize.Content = sLen;
            }
            else
            {
                EmptyPanel.Visibility = Visibility.Visible;
            }
        }

        private void EmptyPanel_OnClick(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                //openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.Filter = "DLL File (*.dll)|*.dll";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    globals.SelectedDllPath.Value = openFileDialog.FileName;

                }
            }
            //globals.SelectedDllPath.Value = "C:\\Users\\wxdxh\\CLionProjects\\ImGuiBaseDX9\\cmake-build-release-msvc_x64\\ImGuiBaseDX9.dll";
        }

        private void CloseFileBtn_OnClick(object sender, RoutedEventArgs e)
        {
            globals.SelectedDllPath.Value = "";
        }

        private void InjectButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (null != globals.selectedItem)
            {
                Injector.Inject(globals.selectedItem.ProcessName, this._filepath);
            }
            else
            {
                MessageBox.Show("No Process Selected");
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WXN_Injector.core;

namespace WXN_Injector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();
            WindowTitleLabel.Content = this.Title;
            RefreshProcesses();
            FilePreviewFrame.Child = new FileView();
            globals.SelectedDllPath.ValueChanged += () =>
            {
                if (!String.IsNullOrEmpty(globals.SelectedDllPath.Value))
                {
                    FilePreviewFrame.Child = new FileView(true, globals.SelectedDllPath.Value);
                }
                else
                {
                    FilePreviewFrame.Child = new FileView();
                }
            };
        }
        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public void RefreshProcesses(string search = null)
        {
            ProcessListView.Items.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                //Console.WriteLine();
                if (search != null)
                {
                    if (Contains(proc.ProcessName, search, StringComparison.OrdinalIgnoreCase))
                    {
                        ProcessListView.Items.Add(
                            new ProcessInfoTemplate(proc.ProcessName, proc.Id)
                        );
                    }
                }
                else
                {
                    ProcessListView.Items.Add(
                        new ProcessInfoTemplate(proc.ProcessName, proc.Id)
                    );
                }
                

            }
        }
        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            /*
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
            */
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show(SearchBox.Text);
            if (String.IsNullOrEmpty(SearchBox.Text))
            {
                RefreshProcesses();
            }
            else
            {
                RefreshProcesses(SearchBox.Text);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(FilePreviewFrame.ActualHeight + "--" + FilePreviewFrame.ActualWidth);
        }

        private void ProcessListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            globals.selectedItem = (ProcessInfoTemplate)ProcessListView.SelectedItem;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessInfoTemplate selectedProc = (ProcessInfoTemplate)ProcessListView.SelectedItem;
            System.Windows.Forms.MessageBox.Show(experimental.CheckGraphicsApi(selectedProc.ProcessName));
        }

    }
}
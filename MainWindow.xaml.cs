using DataAnalyzer.Domain;
using DataAnalyzer.ViewModels;
using DataAnalyzer.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace data_analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WindowChrome windowChrome;
        private readonly IDataReader<double> textDataReader;
        private readonly IDataReader<double> binaryDataReader;

        public MainWindow()
        {
            InitializeComponent();

            textDataReader = new TextDoubleDataReader();
            binaryDataReader = new BinaryDoubleDataReader();

            windowChrome = new()
            {
                CaptionHeight = 0,
                CornerRadius = new(0),
                GlassFrameThickness = new(0)
            };
            WindowChrome.SetWindowChrome(this, windowChrome);
        }


        private void OutputData(IList<double> data)
        {
            EditorWindow editorWindow = 
                new(new EditorWindowViewModel(data.ToList<double>()));
            this.Close();
            editorWindow.Show();
        }


        private void IntroWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                btMaximize_Click(this, new RoutedEventArgs());
                return;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                double x_prop = e.GetPosition(this).X / this.Width;
                double y_prop = e.GetPosition(this).Y / this.Height;

                btMaximize_Click(this, new RoutedEventArgs());

                this.Left = e.GetPosition(this).X - (this.Width * x_prop);
                this.Top = e.GetPosition(this).Y - (this.Height * y_prop);
            }

            this.DragMove();
        }

        private void btLoadTextFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select a Text File",
                Filter = "Text Files (*.txt)|*.txt",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (fileDialog.ShowDialog() == true)
            {
                OutputData(textDataReader.GetData(fileDialog.FileName));
            }
        }

        private void btLoadBinaryFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select a Binary File",
                Filter = "Binary Files (*.dat)|*.dat",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (fileDialog.ShowDialog() == true)
            {
                OutputData(binaryDataReader.GetData(fileDialog.FileName));
            }
        }

        private void btMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                WindowChrome.SetWindowChrome(this, windowChrome);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                WindowChrome.SetWindowChrome(this, null);
            }
        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

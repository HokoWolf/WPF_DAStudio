using DataAnalyzer.Domain.DataReading;
using DataAnalyzer.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;

namespace DataAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly EditorWindowViewModel viewModel;
        private readonly WindowChrome windowChrome;
        private readonly IDataReader<double> textDataReader;
        private readonly IDataReader<double> binaryDataReader;

        public EditorWindow(EditorWindowViewModel vm)
        {
            InitializeComponent();

            viewModel = vm;
            this.DataContext = viewModel;

            textDataReader = new TextDoubleDataReader();
            binaryDataReader = new BinaryDoubleDataReader();

            windowChrome = new()
            {
                CaptionHeight = 0,
                CornerRadius = new(0),
                GlassFrameThickness = new(0)
            };
        }


        private void OutputData(IList<double> data)
        {
            viewModel.Data = data.ToList<double>();
        }


        private void EditWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is Grid)
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

        private void miLoadTextFile_Click(object sender, RoutedEventArgs e)
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

        private void miLoadBinaryFile_Click(object sender, RoutedEventArgs e)
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
    }
}

using DataAnalyzer;
using DataAnalyzer.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private WindowChrome windowChrome;

        public MainWindow()
        {
            InitializeComponent();
            
            windowChrome = new WindowChrome();
            windowChrome.CaptionHeight = 0;
            windowChrome.CornerRadius = new(0);
            windowChrome.GlassFrameThickness = new(0);
            WindowChrome.SetWindowChrome(this, windowChrome);
        }


        private static List<double> GetTextData(string filename)
        {
            List<double> data = new();

            try
            {
                using (TextReader reader = File.OpenText(filename))
                {
                    string? input;

                    while ((input = reader.ReadLine()) != null)
                    {
                        data.Add(double.Parse(input));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return data;
        }

        private static List<double> GetBinaryData(string filename)
        {
            List<double> data = new();

            try
            {
                using (BinaryReader reader = new (File.OpenRead(filename)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        try
                        {
                            data.Add(reader.ReadDouble());
                        }
                        catch (EndOfStreamException)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return data;
        }

        private void OutputData(List<double> data)
        {
            EditorWindowViewModel vm = new(data);
            EditorWindow editorWindow = new(vm);
            this.Close();
            editorWindow.Show();
        }


        private void IntroWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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
                OutputData(GetTextData(fileDialog.FileName));
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
                OutputData(GetBinaryData(fileDialog.FileName));
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

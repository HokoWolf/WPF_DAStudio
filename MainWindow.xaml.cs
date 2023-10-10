using DataAnalyzer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace data_analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private List<double> GetTextData(string filename)
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

        private List<double> GetBinaryData(string filename)
        {
            List<double> data = new();

            try
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filename)))
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
            EditorWindow editorWindow = new EditorWindow();

            foreach (var item in data)
            {
                editorWindow.lstDataOutput.Items.Add(item);
            }
            this.Close();
            editorWindow.Show();
        }


        private void btLoadTextFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a Text File";
            fileDialog.Filter = "Text Files (*.txt)|*.txt";
            fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (fileDialog.ShowDialog() == true)
            {
                OutputData(GetTextData(fileDialog.FileName));
            }
        }

        private void btLoadBinaryFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a Binary File";
            fileDialog.Filter = "Binary Files (*.dat)|*.dat";
            fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (fileDialog.ShowDialog() == true)
            {
                OutputData(GetBinaryData(fileDialog.FileName));
            }
        }

        private void btMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void IntroWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}

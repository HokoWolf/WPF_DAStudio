using DataAnalyzer.ViewModels;
using LiveCharts.Wpf.Charts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace DataAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for IdentifyDestributionWindow.xaml
    /// </summary>
    public partial class IdentifyDestributionWindow : Window
    {
        public IdentifyDestributionWindow(IdentifyWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }


        private void IdentifyDestribution_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void grPaper_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double aspectRatio = 16.0 / 9.0;

            double width = Math.Min(grPaper.ActualWidth, grPaper.ActualHeight * aspectRatio);
            double height = width / aspectRatio;

            chPaper.Width = width;
            chPaper.Height = height;
        }
    }
}

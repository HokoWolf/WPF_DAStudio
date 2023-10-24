using DataAnalyzer.ViewModels;
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
        private readonly WindowChrome windowChrome;


        public IdentifyDestributionWindow(EditorWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;

            windowChrome = new()
            {
                CaptionHeight = 0,
                CornerRadius = new(0),
                GlassFrameThickness = new(0)
            };
            WindowChrome.SetWindowChrome(this, windowChrome);
        }


        private void IdentifyDestribution_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
    }
}

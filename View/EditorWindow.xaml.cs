using DataAnalyzer.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace DataAnalyzer
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly WindowChrome windowChrome;
        private readonly EditorWindowViewModel vm;

        public EditorWindow(EditorWindowViewModel viewModel)
        {
            InitializeComponent();

            windowChrome = new WindowChrome();
            windowChrome.CaptionHeight = 0;
            windowChrome.CornerRadius = new(0);
            windowChrome.GlassFrameThickness = new(0);

            vm = viewModel;
            this.DataContext = vm;
        }

        private void EditWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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

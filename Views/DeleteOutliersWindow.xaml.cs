using DataAnalyzer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace DataAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for DeleteOutliersWindow.xaml
    /// </summary>
    public partial class DeleteOutliersWindow : Window
    {
        public DeleteOutliersWindow(EditorWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void DeleteOutliers_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            // code for success
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

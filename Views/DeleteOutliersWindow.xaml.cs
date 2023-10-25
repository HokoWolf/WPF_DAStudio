using DataAnalyzer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shell;

namespace DataAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for DeleteOutliersWindow.xaml
    /// </summary>
    public partial class DeleteOutliersWindow : Window
    {
        public DeleteOutliersWindow(OutliersWindowViewModel vm)
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
            var result = MessageBox.Show("Ви дійсно хочете видалити аномальні значення?", "Підтвердження", 
                MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

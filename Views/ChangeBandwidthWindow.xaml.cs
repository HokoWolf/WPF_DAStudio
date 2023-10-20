using DataAnalyzer.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for ChangeClassesCountWindow.xaml
    /// </summary>
    public partial class ChangeBandwidthWindow : Window
    {
        public ChangeBandwidthWindow(EditorWindowViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }


        private void ChangeClassCount_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            var bindingExpression = txtClassCount.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateSource();
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

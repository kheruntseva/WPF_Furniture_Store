using System.Windows;

namespace mywpf
{
    public partial class ProductDetailWindow : Window
    {
        private readonly Window _previousWindow;

        public ProductDetailWindow(RectangleItem product, Window previousWindow)
        {
            InitializeComponent();
            DataContext = product;
            _previousWindow = previousWindow;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _previousWindow.Show();
            Close();
        }
    }
}
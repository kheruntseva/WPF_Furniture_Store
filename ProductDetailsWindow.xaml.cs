using System.Windows;

namespace mywpf
{
    public partial class ProductDetailsWindow : Window
    {
        public ProductDetailsWindow(RectangleItem product, MainViewModel viewModel, User currentUser)
        {
            InitializeComponent();
            DataContext = new ProductDetailsViewModel(product, viewModel, currentUser);
        }
    }
}
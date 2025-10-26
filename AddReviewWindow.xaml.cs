using System.Windows;
using System.Windows.Input;

namespace mywpf
{
    public partial class AddReviewWindow : Window
    {
        public AddReviewWindow(AddReviewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;


            if (viewModel.SaveReviewCommand is ICommand command)
            {
                command.CanExecuteChanged += (s, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"SaveReviewCommand CanExecute: {command.CanExecute(null)}");
                };

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
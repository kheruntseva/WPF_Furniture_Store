using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity; // Ensure this is included for EF Core
using System;

namespace mywpf
{
    public class ProductDetailsViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private readonly User _currentUser; // Added to store the user
        private ObservableCollection<Review> _reviews;

        public RectangleItem Product { get; }
        public ObservableCollection<Review> Reviews
        {
            get => _reviews;
            set
            {
                _reviews = value;
                OnPropertyChanged(nameof(Reviews));
            }
        }

        public ICommand AddToCartCommand => _mainViewModel.AddToCartCommand;
        public ICommand AddReviewCommand { get; }

        public ProductDetailsViewModel(RectangleItem product, MainViewModel viewModel, User currentUser)
        {
            Product = product;
            _mainViewModel = viewModel;
            _currentUser = currentUser; // Store the user
            Reviews = new ObservableCollection<Review>();
            LoadReviews();
            AddReviewCommand = new RelayCommand(AddReview);
        }

        private void LoadReviews()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var reviews = context.Reviews
                        .Include(r => r.User)
                        .Where(r => r.RectangleItemId == Product.Id)
                        .ToList();
                    Reviews = new ObservableCollection<Review>(reviews);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddReview(object parameter)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Войдите в систему, чтобы оставить отзыв.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var addReviewViewModel = new AddReviewViewModel(_currentUser, Product, review => Reviews.Add(review));
            var addReviewWindow = new AddReviewWindow(addReviewViewModel);
            addReviewWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
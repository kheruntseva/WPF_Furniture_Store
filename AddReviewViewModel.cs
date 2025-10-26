using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace mywpf
{
    public class AddReviewViewModel : INotifyPropertyChanged
    {
        private string _reviewText;
        private int? _rating;
        private readonly User _user;
        private readonly RectangleItem _item;
        private readonly Action<Review> _onReviewSaved;

        public string ItemName => _item.Name;
        public string ReviewText
        {
            get => _reviewText;
            set
            {
                _reviewText = value;
                OnPropertyChanged(nameof(ReviewText));
                ((RelayCommand)SaveReviewCommand).RaiseCanExecuteChanged();
            }
        }
        public int? Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged(nameof(Rating));
                ((RelayCommand)SaveReviewCommand).RaiseCanExecuteChanged();
            }
        }
        public ICommand SaveReviewCommand { get; }

        public AddReviewViewModel(User user, RectangleItem item, Action<Review> onReviewSaved)
        {
            _user = user;
            _item = item;
            _onReviewSaved = onReviewSaved;
            _rating = 1; // Default rating to ensure CanSaveReview passes
            SaveReviewCommand = new RelayCommand(SaveReview, CanSaveReview);
        }

        private void SaveReview(object parameter)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var review = new Review
                    {
                        UserId = _user.Id,
                        RectangleItemId = _item.Id,
                        ReviewText = ReviewText,
                        Rating = Rating.Value,
                        ReviewDate = DateTime.Now
                    };
                    context.Reviews.Add(review);
                    context.SaveChanges();
                    _onReviewSaved(review);
                }
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveReview failed: {ex.Message}");
                MessageBox.Show("Ошибка при сохранении отзыва. Попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveReview(object parameter)
        {
            bool canSave = !string.IsNullOrWhiteSpace(ReviewText) && Rating.HasValue;
            System.Diagnostics.Debug.WriteLine($"CanSaveReview: ReviewText='{ReviewText}', Rating={Rating}, CanSave={canSave}");
            return canSave;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Input;
//using System.Data.Entity;
//using Microsoft.Xaml.Behaviors.Core;

//namespace mywpf
//{
//    public partial class ManageReviewsWindow : Window
//    {
//        public ObservableCollection<ReviewViewModel> Reviews { get; set; }

//        public ManageReviewsWindow()
//        {
//            InitializeComponent();
//            Reviews = new ObservableCollection<ReviewViewModel>();
//            DataContext = this;
//            LoadReviews();
//        }

//        private void LoadReviews()
//        {
//            try
//            {
//                using (var context = new AppDbContext())
//                {
//                    var reviewData = context.Reviews
//                        .Include(r => r.RectangleItem)
//                        .Include(r => r.User)
//                        .ToList();

//                    var reviews = reviewData.Select(r => new ReviewViewModel
//                    {
//                        Id = r.Id,
//                        RectangleItemId = r.RectangleItemId,
//                        UserId = r.UserId,
//                        ProductName = r.RectangleItem != null ? r.RectangleItem.Name : "Неизвестный товар",
//                        Username = r.User != null ? r.User.Username : "Неизвестный пользователь",
//                        Rating = r.Rating,
//                        ReviewText = r.ReviewText,
//                        ReviewDate = r.ReviewDate
//                    }).ToList();

//                    Reviews.Clear();
//                    foreach (var review in reviews)
//                    {
//                        review.DeleteReviewCommand = new ActionCommand(parameter =>
//                        {
//                            if (parameter is ReviewViewModel reviewToDelete)
//                            {
//                                if (MessageBox.Show($"Вы уверены, что хотите удалить отзыв от {reviewToDelete.Username} для товара {reviewToDelete.ProductName}?",
//                                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
//                                {
//                                    try
//                                    {
//                                        using (var deleteContext = new AppDbContext())
//                                        {
//                                            var dbReview = deleteContext.Reviews.Find(reviewToDelete.Id);
//                                            if (dbReview != null)
//                                            {
//                                                deleteContext.Reviews.Remove(dbReview);
//                                                deleteContext.SaveChanges();
//                                                Reviews.Remove(reviewToDelete);
//                                            }
//                                        }
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        MessageBox.Show($"Ошибка при удалении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//                                    }
//                                }
//                            }
//                        });
//                        Reviews.Add(review);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void CloseButton_Click(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }
//    }

//    public class ReviewViewModel
//    {
//        public int Id { get; set; }
//        public Guid RectangleItemId { get; set; }
//        public Guid UserId { get; set; }
//        public string ProductName { get; set; }
//        public string Username { get; set; }
//        public int Rating { get; set; }
//        public string ReviewText { get; set; }
//        public DateTime ReviewDate { get; set; }
//        public ICommand DeleteReviewCommand { get; set; }
//    }
//}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.Entity;
using Microsoft.Xaml.Behaviors.Core;

namespace mywpf
{
    public partial class ManageReviewsWindow : Window
    {
        public ObservableCollection<ReviewViewModel> Reviews { get; set; }

        public ManageReviewsWindow()
        {
            InitializeComponent();
            Reviews = new ObservableCollection<ReviewViewModel>();
            DataContext = this;
            LoadReviews();
        }

        private void LoadReviews()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var reviewData = context.Reviews
                        .Include(r => r.RectangleItem)
                        .Include(r => r.User)
                        .ToList();

                    var reviews = reviewData.Select(r => new ReviewViewModel
                    {
                        Id = r.Id,
                        RectangleItemId = r.RectangleItemId,
                        UserId = r.UserId,
                        ProductName = r.RectangleItem != null ? r.RectangleItem.Name : "Неизвестный товар",
                        Username = r.User != null ? r.User.Username : "Неизвестный пользователь",
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        ReviewDate = r.ReviewDate
                    }).ToList();

                    Reviews.Clear();
                    foreach (var review in reviews)
                    {
                        review.DeleteReviewCommand = new ActionCommand(parameter =>
                        {
                            if (parameter is ReviewViewModel reviewToDelete)
                            {
                                if (MessageBox.Show($"Вы уверены, что хотите удалить отзыв от {reviewToDelete.Username} для товара {reviewToDelete.ProductName}? (ID: {reviewToDelete.Id})",
                                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        using (var deleteContext = new AppDbContext())
                                        {
                                            var dbReview = deleteContext.Reviews.Find(reviewToDelete.Id);
                                            if (dbReview == null)
                                            {
                                                MessageBox.Show($"Отзыв с ID {reviewToDelete.Id} не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                                return;
                                            }

                                            deleteContext.Reviews.Remove(dbReview);
                                            int rowsAffected = deleteContext.SaveChanges();
                                            if (rowsAffected > 0)
                                            {
                                                Reviews.Remove(reviewToDelete);
                                                MessageBox.Show("Отзыв успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                            }
                                            else
                                            {
                                                MessageBox.Show("Не удалось удалить отзыв из базы данных. Изменения не сохранены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"Ошибка при удалении отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Неверный параметр команды.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                        Reviews.Add(review);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отзывов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ReviewViewModel
    {
        public int Id { get; set; }
        public Guid RectangleItemId { get; set; }
        public Guid UserId { get; set; }
        public string ProductName { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
        public ICommand DeleteReviewCommand { get; set; }
    }
}
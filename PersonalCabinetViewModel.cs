//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Input;

//namespace mywpf
//{
//    public class PersonalCabinetViewModel
//    {
//        private readonly User _user;
//        public string Username => _user.Username;
//        public ObservableCollection<OrderHistory> OrderHistory { get; }

//        public ICommand AddReviewCommand { get; }

//        public PersonalCabinetViewModel(User user)
//        {
//            _user = user;
//            OrderHistory = new ObservableCollection<OrderHistory>(user.OrderHistory);
//            AddReviewCommand = new RelayCommand(AddReview);
//        }

//        private void AddReview(object parameter)
//        {
//            if (parameter is OrderHistory orderHistory)
//            {
//                using (var context = new AppDbContext())
//                {
//                    var item = context.RectangleItems
//                        .AsNoTracking()
//                        .FirstOrDefault(i => i.Id == orderHistory.RectangleItemId);
//                    if (item == null) return;

//                    var viewModel = new AddReviewViewModel(_user, new RectangleItem(item) { Name = orderHistory.Name }, review =>
//                    {
//                        OrderHistory.Add(new OrderHistory
//                        {
//                            RectangleItemId = review.RectangleItemId,
//                            Name = orderHistory.Name,
//                            Price = orderHistory.Price,
//                            Category = orderHistory.Category,
//                            PurchaseDate = orderHistory.PurchaseDate
//                        });
//                    });
//                    var window = new AddReviewWindow(viewModel);
//                    window.ShowDialog();
//                }
//            }
//        }
//    }
//}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace mywpf
{
    public class PersonalCabinetViewModel
    {
        private readonly User _user;
        public string Username => _user.Username;
        public ObservableCollection<OrderHistory> OrderHistory { get; }

        public ICommand AddReviewCommand { get; }

        public PersonalCabinetViewModel(User user)
        {
            _user = user;
            OrderHistory = new ObservableCollection<OrderHistory>(user.OrderHistory);
            AddReviewCommand = new RelayCommand(AddReview);
        }

        private void AddReview(object parameter)
        {
            if (parameter is OrderHistory orderHistory)
            {
                using (var context = new AppDbContext())
                {
                    var item = context.RectangleItems
                        .AsNoTracking()
                        .FirstOrDefault(i => i.Id == orderHistory.RectangleItemId);
                    if (item == null) return;

                    var viewModel = new AddReviewViewModel(_user, new RectangleItem(item) { Name = orderHistory.Name }, review =>
                    {
                        // Optional: Update OrderHistory if needed
                    });
                    var window = new AddReviewWindow(viewModel);
                    window.ShowDialog();
                }
            }
        }
    }
}
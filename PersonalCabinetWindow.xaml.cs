//using System.Windows;

//namespace mywpf
//{
//    public partial class PersonalCabinetWindow : Window
//    {
//        public PersonalCabinetWindow(User user)
//        {
//            InitializeComponent();
//            DataContext = user;
//        }

//        private void CloseButton_Click(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }
//    }
//}
using System.Windows;

namespace mywpf
{
    public partial class PersonalCabinetWindow : Window
    {
        public PersonalCabinetWindow(User user)
        {
            InitializeComponent();
            DataContext = new PersonalCabinetViewModel(user);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
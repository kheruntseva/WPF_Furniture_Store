//using System.Data.Entity;
//using System.Linq;
//using System.Windows;

//namespace mywpf
//{
//    public partial class AuthorizationWindow : Window
//    {
//        public bool IsAdmin { get; private set; }
//        public User AuthenticatedUser { get; private set; }

//        public AuthorizationWindow()
//        {
//            InitializeComponent();
//        }

//        private void LoginButton_Click(object sender, RoutedEventArgs e)
//        {
//            string username = UsernameTextBox.Text.Trim();
//            string password = PasswordBox.Password;

//            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
//            {
//                MessageBox.Show("Введите имя пользователя и пароль");
//                return;
//            }

//            if (username == "admin" && password == "admin")
//            {
//                using (var context = new AppDbContext())
//                {
//                    var adminUser = context.Users.FirstOrDefault(u => u.Username == "admin");
//                    if (adminUser == null)
//                    {
//                        adminUser = new User { Username = "admin", IsAdmin = true };
//                        context.Users.Add(adminUser);
//                        context.SaveChanges();
//                    }
//                    IsAdmin = true;
//                    AuthenticatedUser = adminUser;
//                    MessageBox.Show("Добро пожаловать, администратор!");
//                    DialogResult = true;
//                    Close();
//                }
//                return;
//            }

//            using (var context = new AppDbContext())
//            {
//                var user = context.Users.FirstOrDefault(u => u.Username == username);
//                if (user != null && password == "user") // Simplified password check
//                {
//                    IsAdmin = user.IsAdmin;
//                    AuthenticatedUser = user;
//                    MessageBox.Show($"Добро пожаловать, {username}!");
//                    DialogResult = true;
//                    Close();
//                }
//                else
//                {
//                    MessageBox.Show("Неверное имя пользователя или пароль");
//                }
//            }
//        }

//        private void RegisterButton_Click(object sender, RoutedEventArgs e)
//        {
//            string username = UsernameTextBox.Text.Trim();
//            string password = PasswordBox.Password;

//            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
//            {
//                MessageBox.Show("Введите имя пользователя и пароль");
//                return;
//            }

//            using (var context = new AppDbContext())
//            {
//                if (context.Users.Any(u => u.Username == username))
//                {
//                    MessageBox.Show("Пользователь с таким именем уже существует");
//                    return;
//                }

//                var newUser = new User { Username = username, Password=password, IsAdmin = false };
//                context.Users.Add(newUser);
//                context.SaveChanges();

//                AuthenticatedUser = newUser;
//                MessageBox.Show("Регистрация успешна!");
//                UsernameTextBox.Text = "";
//                PasswordBox.Password = "";
//                DialogResult = true;
//                Close();
//            }
//        }
//    }
//}

using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace mywpf
{
    public partial class AuthorizationWindow : Window
    {
        public bool IsAdmin { get; private set; }
        public User AuthenticatedUser { get; private set; }

        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            //            if (username == "admin" && password == "admin")
            //            {
            //                using (var context = new appdbcontext())
            //                {
            //                    var adminuser = context.users.firstordefault(u => u.username == "admin");
            //                    if (adminuser == null)
            //                    {
            //                        adminuser = new user { username = "admin", isadmin = true };
            //                        context.users.add(adminuser);
            //                        context.savechanges();
            //                    }
            //                    isadmin = true;
            //                    authenticateduser = adminuser;
            //                    messagebox.show("добро пожаловать, администратор!");
            //                    dialogresult = true;
            //                    close();
            //                }
            //                return;
            //            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль");
                return;
            }

            using (var context = new AppDbContext())
            {
                var user = context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Username == username && u.Password == password);

                if (user != null)
                {
                    IsAdmin = user.IsAdmin;
                    AuthenticatedUser = user;
                    MessageBox.Show($"Добро пожаловать, {username}!");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль");
                }
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль");
                return;
            }

            using (var context = new AppDbContext())
            {
                if (context.Users.Any(u => u.Username == username))
                {
                    MessageBox.Show("Пользователь с таким именем уже существует");
                    return;
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = password,
                    IsAdmin = username == "admin" 
                };
                context.Users.Add(newUser);
                context.SaveChanges();

                AuthenticatedUser = newUser;
                IsAdmin = newUser.IsAdmin;
                MessageBox.Show("Регистрация успешна!");
                UsernameTextBox.Text = "";
                PasswordBox.Password = "";
                DialogResult = true;
                Close();
            }
        }
    }
}
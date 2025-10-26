using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace mywpf
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            var lightTheme = new ResourceDictionary { Source = new Uri("LightTheme.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(lightTheme);
            ThemeButton.Content = "темная тема";
        }
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            // Поиск текущего словаря темы
            var mergedDicts = this.Resources.MergedDictionaries;
            ResourceDictionary currentTheme = null;
            foreach (var dict in mergedDicts)
            {
                if (dict.Source != null && (dict.Source.OriginalString == "LightTheme.xaml" || dict.Source.OriginalString == "DarkTheme.xaml"))
                {
                    currentTheme = dict;
                    break;
                }
            }

            // Удаление текущего словаря темы
            if (currentTheme != null)
            {
                mergedDicts.Remove(currentTheme);
            }

            // Добавление нового словаря темы
            ResourceDictionary newTheme;
            if (ThemeButton.Content.ToString() == "темная тема")
            {
                newTheme = new ResourceDictionary { Source = new Uri("DarkTheme.xaml", UriKind.Relative) };
                ThemeButton.Content = "светлая тема";
            }
            else
            {
                newTheme = new ResourceDictionary { Source = new Uri("LightTheme.xaml", UriKind.Relative) };
                ThemeButton.Content = "темная тема";
            }
            mergedDicts.Add(newTheme);
        }


        private void OpenAuthButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authWindow = new AuthorizationWindow();
            bool? result = authWindow.ShowDialog();

            if (result == true)
            {
                _viewModel.IsAdmin = authWindow.IsAdmin;
                MessageBox.Show("Авторизация прошла успешно!");
            }
            else
            {
                MessageBox.Show("Авторизация отменена");
            }
        }
        //private void Button_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button && button.Tag?.ToString() == "MouseOverEnabled")
        //    {
        //        Logger.Log($"Кнопка '{button.Content}' активирована (MouseOver и Enabled)");
        //    }
        //}

    }


    //public class RectangleItem
    //{
    //    public string Image { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string Price { get; set; }
    //    public string Availability { get; set; }
    //    public string Rating { get; set; }
    //    public string Category { get; set; }
    //    public string Color { get; set; }
    //    public string Amount { get; set; }
    //    public string Size { get; set; }
    //}
}
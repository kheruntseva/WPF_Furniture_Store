//using System;
//using System.IO;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;

//namespace mywpf
//{
//    public partial class EditProductWindow : Window
//    {
//        public RectangleItem Product { get; private set; }
//        public bool IsNewProduct { get; private set; }

//        // Property to bind ColorComboBox to string color names
//        public string ColorName
//        {
//            get
//            {
//                if (Product.Color == Colors.Black) return "Черный";
//                if (Product.Color == Colors.White) return "Белый";
//                if (Product.Color == Colors.Brown) return "Дерево";
//                return "Черный"; // Default
//            }
//            set
//            {
//                if (value == "Черный")
//                    Product.Color = Colors.Black;
//                else if (value == "Белый")
//                    Product.Color = Colors.White;
//                else if (value == "Дерево")
//                    Product.Color = Colors.Brown;
//                else
//                    Product.Color = Colors.Black;
//            }
//        }

//        public EditProductWindow(RectangleItem product = null)
//        {
//            InitializeComponent();
//            IsNewProduct = product == null;
//            Product = product != null ? new RectangleItem(product) : new RectangleItem();
//            DataContext = this; // Bind to window for ColorName and Product

//            // Устанавливаем категорию
//            foreach (ComboBoxItem item in CategoryComboBox.Items)
//            {
//                if (item.Content.ToString() == Product.Category)
//                {
//                    CategoryComboBox.SelectedItem = item;
//                    break;
//                }
//            }

//            // Устанавливаем цвет (handled by ColorName binding)
//            ColorComboBox.SelectedValue = ColorName;

//            // Устанавливаем наличие
//            foreach (ComboBoxItem item in AvailabilityComboBox.Items)
//            {
//                if (item.Content.ToString() == Product.Availability)
//                {
//                    AvailabilityComboBox.SelectedItem = item;
//                    break;
//                }
//            }
//            if (AvailabilityComboBox.SelectedItem == null)
//            {
//                AvailabilityComboBox.SelectedIndex = 0;
//            }

//            // Показываем изображение
//            if (!string.IsNullOrWhiteSpace(Product.Image) && File.Exists(Product.Image))
//            {
//                try
//                {
//                    ImagePreview.Source = new BitmapImage(new Uri(Product.Image, UriKind.RelativeOrAbsolute));
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
//                    Product.Image = string.Empty;
//                }
//            }
//        }

//        private void SaveButton_Click(object sender, RoutedEventArgs e)
//        {
//            // Валидация
//            if (string.IsNullOrWhiteSpace(Product.Name))
//            {
//                MessageBox.Show("Введите название товара.");
//                return;
//            }
//            if (string.IsNullOrWhiteSpace(Product.Description))
//            {
//                MessageBox.Show("Введите описание товара.");
//                return;
//            }
//            decimal price;
//            if (!decimal.TryParse(Product.Price, System.Globalization.NumberStyles.Any,
//                System.Globalization.CultureInfo.InvariantCulture, out price) || price < 0)
//            {
//                MessageBox.Show("Введите корректную цену (положительное число, например, 15000.00).");
//                return;
//            }

//            try
//            {
//                // Форматируем цену
//                Product.Price = price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
//                // Устанавливаем значения из ComboBox
//                Product.Category = (CategoryComboBox.SelectedItem as ComboBoxItem) != null
//                    ? (CategoryComboBox.SelectedItem as ComboBoxItem).Content.ToString()
//                    : "Кухня";
//                Product.Availability = (AvailabilityComboBox.SelectedItem as ComboBoxItem) != null
//                    ? (AvailabilityComboBox.SelectedItem as ComboBoxItem).Content.ToString()
//                    : "В наличии";

//                DialogResult = true;
//                Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
//            }
//        }

//        private void DeleteButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?",
//                "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
//            {
//                Product = null; // Signals deletion
//                DialogResult = true;
//                Close();
//            }
//        }

//        private void CancelButton_Click(object sender, RoutedEventArgs e)
//        {
//            DialogResult = false;
//            Close();
//        }

//        private void ImageBorder_DragEnter(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//            {
//                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                if (files.Length == 1 && IsImageFile(files[0]))
//                {
//                    e.Effects = DragDropEffects.Copy;
//                }
//                else
//                {
//                    e.Effects = DragDropEffects.None;
//                }
//            }
//            else
//            {
//                e.Effects = DragDropEffects.None;
//            }
//            e.Handled = true;
//        }

//        private void ImageBorder_Drop(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//            {
//                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                if (files.Length == 1 && IsImageFile(files[0]))
//                {
//                    try
//                    {
//                        string relativePath = SaveImageToAppFolder(files[0]);
//                        Product.Image = relativePath;
//                        ImagePathTextBox.Text = relativePath;
//                        ImagePreview.Source = new BitmapImage(new Uri(files[0], UriKind.Absolute));
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
//                    }
//                }
//            }
//        }

//        private bool IsImageFile(string filePath)
//        {
//            if (string.IsNullOrEmpty(filePath)) return false;
//            string extension = Path.GetExtension(filePath).ToLower();
//            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp";
//        }

//        private string SaveImageToAppFolder(string sourcePath)
//        {
//            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
//            if (!Directory.Exists(imagesFolder))
//            {
//                Directory.CreateDirectory(imagesFolder);
//            }

//            string fileName = Path.GetFileName(sourcePath);
//            string destPath = Path.Combine(imagesFolder, fileName);

//            // Если файл уже существует, добавляем уникальный суффикс
//            int counter = 1;
//            while (File.Exists(destPath))
//            {
//                string baseName = Path.GetFileNameWithoutExtension(fileName);
//                string extension = Path.GetExtension(fileName);
//                fileName = baseName + "_" + counter + extension;
//                destPath = Path.Combine(imagesFolder, fileName);
//                counter++;
//            }

//            File.Copy(sourcePath, destPath);
//            return Path.Combine("Images", fileName).Replace('\\', '/');
//        }
//    }
//}


//using System;
//using System.IO;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media.Imaging;

//namespace mywpf
//{
//    public partial class EditProductWindow : Window
//    {
//        public RectangleItem Product { get; private set; }
//        public bool IsNewProduct { get; private set; }

//        public EditProductWindow(RectangleItem product = null)
//        {
//            InitializeComponent();
//            IsNewProduct = product == null;
//            Product = product != null ? product : new RectangleItem();

//            // Заполняем поля
//            NameTextBox.Text = Product.Name;
//            DescriptionTextBox.Text = Product.Description;
//            PriceTextBox.Text = Product.Price;
//            CategoryComboBox.SelectedItem = CategoryComboBox.Items.Cast<ComboBoxItem>()
//                .FirstOrDefault(i => i.Content.ToString() == Product.Category);
//        }

//        private void SaveButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
//                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
//                !decimal.TryParse(PriceTextBox.Text, System.Globalization.NumberStyles.Any,
//                    System.Globalization.CultureInfo.InvariantCulture, out decimal price))
//            {
//                MessageBox.Show("Заполните все поля корректно");
//                return;
//            }

//            Product.Name = NameTextBox.Text;
//            Product.Description = DescriptionTextBox.Text;
//            Product.Price = price.ToString(System.Globalization.CultureInfo.InvariantCulture);
//            Product.Category = (CategoryComboBox.SelectedItem as ComboBoxItem) != null
//                ? (CategoryComboBox.SelectedItem as ComboBoxItem).Content.ToString()
//                : "Кухня";

//            DialogResult = true;
//            Close();
//        }

//        private void DeleteButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?",
//                "Подтверждение удаления", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
//            {
//                DialogResult = true;
//                Product = null;
//                Close();
//            }
//        }

//        private void CancelButton_Click(object sender, RoutedEventArgs e)
//        {
//            DialogResult = false;
//            Close();
//        }

//        private void ImageBorder_DragEnter(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//            {
//                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//                if (files.Length == 1 && IsImageFile(files[0]))
//                {
//                    e.Effects = DragDropEffects.Copy;
//                }
//                else
//                {
//                    e.Effects = DragDropEffects.None;
//                }
//            }
//            else
//            {
//                e.Effects = DragDropEffects.None;
//            }
//            e.Handled = true;
//        }

//        private bool IsImageFile(string filePath)
//        {
//            if (string.IsNullOrEmpty(filePath)) return false;
//            string extension = Path.GetExtension(filePath).ToLower();
//            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp";
//        }

//        private string SaveImageToAppFolder(string sourcePath)
//        {
//            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
//            if (!Directory.Exists(imagesFolder))
//            {
//                Directory.CreateDirectory(imagesFolder);
//            }

//            string fileName = Path.GetFileName(sourcePath);
//            string destPath = Path.Combine(imagesFolder, fileName);

//            // Если файл уже существует, добавляем уникальный суффикс
//            int counter = 1;
//            while (File.Exists(destPath))
//            {
//                string baseName = Path.GetFileNameWithoutExtension(fileName);
//                string extension = Path.GetExtension(fileName);
//                destPath = Path.Combine(imagesFolder, baseName + "_" + counter + extension);
//                counter++;
//            }

//            File.Copy(sourcePath, destPath);
//            return destPath;
//        }
//    }
//}

//using System;
//using System.IO;
//using System.Windows;
//using System.Windows.Controls;

//namespace mywpf
//{
//    public partial class EditProductWindow : Window
//    {
//        public RectangleItem Product { get; set; }
//        public bool IsNewProduct { get; private set; }

//        public EditProductWindow(RectangleItem item = null)
//        {
//            InitializeComponent();
//            IsNewProduct = item == null;
//            Product = item != null ? new RectangleItem(item) : new RectangleItem();
//            DataContext = Product;

//            // Set ComboBox selections
//            foreach (ComboBoxItem categoryItem in CategoryComboBox.Items)
//            {
//                if (categoryItem.Content.ToString() == Product.Category)
//                {
//                    CategoryComboBox.SelectedItem = categoryItem;
//                    break;
//                }
//            }
//            if (CategoryComboBox.SelectedItem == null)
//            {
//                CategoryComboBox.SelectedIndex = 0; // Default to "Кухня"
//            }

//            foreach (ComboBoxItem colorItem in ColorComboBox.Items)
//            {
//                if (colorItem.Content.ToString() == Product.Color)
//                {
//                    ColorComboBox.SelectedItem = colorItem;
//                    break;
//                }
//            }
//            if (ColorComboBox.SelectedItem == null)
//            {
//                ColorComboBox.SelectedIndex = 0; // Default to "Черный"
//            }

//            foreach (ComboBoxItem availabilityItem in AvailabilityComboBox.Items)
//            {
//                if (availabilityItem.Content.ToString() == Product.Availability)
//                {
//                    AvailabilityComboBox.SelectedItem = availabilityItem;
//                    break;
//                }
//            }
//            if (AvailabilityComboBox.SelectedItem == null)
//            {
//                AvailabilityComboBox.SelectedIndex = 0; // Default to "В наличии"
//            }
//        }

//        private void SaveButton_Click(object sender, RoutedEventArgs e)
//        {
//            // Validation
//            if (string.IsNullOrWhiteSpace(Product.Name))
//            {
//                MessageBox.Show("Введите название товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }
//            if (string.IsNullOrWhiteSpace(Product.Description))
//            {
//                MessageBox.Show("Введите описание товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }

//            // Get the Price TextBox (since Product.Price is decimal, we need the string input)
//        var priceTextBox = (TextBox)FindName("PriceTextBox");
//if (priceTextBox == null)
//{
//    MessageBox.Show("Текстовое поле для цены не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//    return;
//}

//string priceText = priceTextBox.Text.Trim(); // Remove leading/trailing spaces
//if (string.IsNullOrWhiteSpace(priceText) || 
//    !decimal.TryParse(priceText, System.Globalization.NumberStyles.Any, 
//        System.Globalization.CultureInfo.CurrentCulture, out decimal price) || 
//    price <= 0)
//{
//    MessageBox.Show($"Введите корректную цену (положительное число, например, {15000.00m.ToString("F2", System.Globalization.CultureInfo.CurrentCulture)}). Текущий ввод: '{priceText}'",
//        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
//    return;
//}
//            if (string.IsNullOrWhiteSpace(Product.Image) || !File.Exists(Product.Image))
//            {
//                MessageBox.Show("Укажите действительный путь к изображению.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
//                return;
//            }

//            // Update Product properties
//            Product.Price = price; // Assign decimal directly
//            Product.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Кухня";
//            Product.Color = (ColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Черный";
//            Product.Availability = (AvailabilityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "В наличии";

//            DialogResult = true;
//            Close();
//        }

//        private void DeleteButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?",
//                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
//            {
//                Product = null;
//                DialogResult = true;
//                Close();
//            }
//        }

//        private void CancelButton_Click(object sender, RoutedEventArgs e)
//        {
//            DialogResult = false;
//            Close();
//        }

//        private void BrowseImage_Click(object sender, RoutedEventArgs e)
//        {
//            var dialog = new Microsoft.Win32.OpenFileDialog
//            {
//                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
//            };
//            if (dialog.ShowDialog() == true)
//            {
//                try
//                {
//                    string destPath = SaveImageToAppFolder(dialog.FileName);
//                    Product.Image = destPath;
//                    // Update TextBox manually since binding may not refresh
//                    var imageTextBox = (TextBox)FindName("ImageTextBox");
//                    if (imageTextBox != null)
//                    {
//                        imageTextBox.Text = destPath;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        private string SaveImageToAppFolder(string sourcePath)
//        {
//            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
//            if (!Directory.Exists(imagesFolder))
//            {
//                Directory.CreateDirectory(imagesFolder);
//            }

//            string fileName = Path.GetFileName(sourcePath);
//            string destPath = Path.Combine(imagesFolder, fileName);

//            // Handle duplicate file names
//            int counter = 1;
//            while (File.Exists(destPath))
//            {
//                string baseName = Path.GetFileNameWithoutExtension(fileName);
//                string extension = Path.GetExtension(fileName);
//                fileName = $"{baseName}_{counter}{extension}";
//                destPath = Path.Combine(imagesFolder, fileName);
//                counter++;
//            }

//            File.Copy(sourcePath, destPath);
//            return Path.Combine("Images", fileName).Replace('\\', '/');
//        }
//    }
//}
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace mywpf
{
    public partial class EditProductWindow : Window
    {
        public RectangleItem Product { get; set; }
        public bool IsNewProduct { get; private set; }

        public EditProductWindow(RectangleItem item = null)
        {
            InitializeComponent();
            IsNewProduct = item == null;
            Product = item != null ? new RectangleItem
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                Color = item.Color,
                Image = item.Image,
                Availability = item.Availability
            } : new RectangleItem();
            DataContext = Product;

            // Set ComboBox selections
            foreach (ComboBoxItem categoryItem in CategoryComboBox.Items)
            {
                if (categoryItem.Content.ToString() == Product.Category)
                {
                    CategoryComboBox.SelectedItem = categoryItem;
                    break;
                }
            }
            if (CategoryComboBox.SelectedItem == null)
            {
                CategoryComboBox.SelectedIndex = 0; // Default to "Кухня"
            }

            foreach (ComboBoxItem colorItem in ColorComboBox.Items)
            {
                if (colorItem.Content.ToString() == Product.Color)
                {
                    ColorComboBox.SelectedItem = colorItem;
                    break;
                }
            }
            if (ColorComboBox.SelectedItem == null)
            {
                ColorComboBox.SelectedIndex = 0; // Default to "Черный"
            }

            foreach (ComboBoxItem availabilityItem in AvailabilityComboBox.Items)
            {
                if (availabilityItem.Content.ToString() == Product.Availability)
                {
                    AvailabilityComboBox.SelectedItem = availabilityItem;
                    break;
                }
            }
            if (AvailabilityComboBox.SelectedItem == null)
            {
                AvailabilityComboBox.SelectedIndex = 0; // Default to "В наличии"
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Product.Name))
            {
                MessageBox.Show("Введите название товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Product.Description))
            {
                MessageBox.Show("Введите описание товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the Price TextBox
            var priceTextBox = (TextBox)FindName("PriceTextBox");
            if (priceTextBox == null)
            {
                MessageBox.Show("Текстовое поле для цены не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string priceText = priceTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(priceText) ||
                !decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal price) ||
                price <= 0)
            {
                MessageBox.Show($"Введите корректную цену (положительное число, например, {15000.00m.ToString("F2", CultureInfo.CurrentCulture)}). Текущий ввод: '{priceText}'",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(Product.Image) || !File.Exists(Product.Image))
            {
                MessageBox.Show("Укажите действительный путь к изображению.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update Product properties
            Product.Price = price;
            Product.Category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Кухня";
            Product.Color = (ColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Черный";
            Product.Availability = (AvailabilityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "В наличии";

            DialogResult = true;
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этот товар?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Product = null;
                DialogResult = true;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string destPath = SaveImageToAppFolder(dialog.FileName);
                    Product.Image = destPath;
                    // Update TextBox manually since binding may not refresh
                    var imageTextBox = (TextBox)FindName("ImageTextBox");
                    if (imageTextBox != null)
                    {
                        imageTextBox.Text = destPath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string SaveImageToAppFolder(string sourcePath)
        {
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            string fileName = Path.GetFileName(sourcePath);
            string destPath = Path.Combine(imagesFolder, fileName);

            // Handle duplicate file names
            int counter = 1;
            while (File.Exists(destPath))
            {
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                fileName = $"{baseName}_{counter}{extension}";
                destPath = Path.Combine(imagesFolder, fileName);
                counter++;
            }

            File.Copy(sourcePath, destPath);
            return Path.Combine("Images", fileName).Replace('\\', '/');
        }
    }
}
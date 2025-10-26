using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Newtonsoft.Json;
using static mywpf.AppDbContext;

namespace mywpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<RectangleItem> _items;
        private ObservableCollection<RectangleItem> _filteredItems;
        private string _searchText;
        private string _selectedCategory;
        private string _selectedColor = "Все";
        private string _priceFilter;
        private bool _isAdmin;
        private User _loggedInUser;
        private readonly Stack<ActionRecord> _undoStack;
        private readonly Stack<ActionRecord> _redoStack;
        private readonly Dictionary<string, ObservableCollection<RectangleItem>> _languageItems;
        private readonly IRectangleItemRepository _rectangleItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ObservableCollection<RectangleItem> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<RectangleItem> FilteredItems
        {
            get => _filteredItems;
            private set
            {
                _filteredItems = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
                using (StreamWriter writer = new StreamWriter("log.txt", true))
                {
                    writer.WriteLine($"Поисковый запрос изменен на '{value}' в {DateTime.Now}");
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    _selectedColor = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public string PriceFilter
        {
            get => _priceFilter;
            set
            {
                if (_priceFilter != value)
                {
                    _priceFilter = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AuthButtonText));
                ((RelayCommand)EditProductCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)AddProductCommand)?.RaiseCanExecuteChanged();
            }
        }

        //public User LoggedInUser
        //{
        //    get => _loggedInUser;
        //    set
        //    {
        //        _loggedInUser = value;
        //        OnPropertyChanged();
        //        OnPropertyChanged(nameof(AuthButtonText));
        //        ((RelayCommand)OpenAuthOrCabinetCommand)?.RaiseCanExecuteChanged();
        //        if (_loggedInUser != null/* && _loggedInUser.PurchasedProducts == null*/)
        //        {
        //            _loggedInUser.PurchasedProducts = new List<RectangleItem>();
        //        }
        //    }
        //}

        public User LoggedInUser
        {
            get => _loggedInUser;
            set
            {
                _loggedInUser = value;
                if (_loggedInUser != null)
                {
                    // Initialize PurchasedProducts if null
                    if (_loggedInUser.PurchasedProducts == null)
                    {
                        _loggedInUser.PurchasedProducts = new List<RectangleItem>();
                    }
                    if (_loggedInUser.OrderHistory == null)
                    {
                        _loggedInUser.OrderHistory = new List<OrderHistory>();
                    }

                    // Load cart items into a temporary list
                    var cartItems = new List<RectangleItem>();
                    var orderHistoryItems = new List<OrderHistory>();
                    using (var context = new AppDbContext())
                    {
                        var user = context.Users
                            .Include(u => u.PurchasedProducts.Select(p => p.ItemTranslations))
                            .Include(u => u.OrderHistory)
                            .AsNoTracking()
                            .FirstOrDefault(u => u.Id == _loggedInUser.Id);
                        if (user != null)
                        {
                            foreach (var dbItem in user.PurchasedProducts)
                            {
                                var translation = dbItem.ItemTranslations
                                    .FirstOrDefault(t => t.LanguageCode == LanguageManager.CurrentLanguage);
                                var localItem = new RectangleItem(dbItem)
                                {
                                    Name = translation?.Name ?? dbItem.Name ?? "Без названия",
                                    Description = translation?.Description,
                                    Category = translation?.Category,
                                    Availability = translation?.Availability
                                };
                                cartItems.Add(localItem);
                            }
                            orderHistoryItems.AddRange(user.OrderHistory);
                        }
                    }

                    // Replace PurchasedProducts with new list
                    _loggedInUser.PurchasedProducts = new List<RectangleItem>(cartItems);
                    _loggedInUser.OrderHistory = new List<OrderHistory>(orderHistoryItems);
                    Debug.WriteLine($"LoggedInUser cart initialized: {cartItems.Count} items");
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(AuthButtonText));
                ((RelayCommand)OpenAuthOrCabinetCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string AuthButtonText
        {
            get => LoggedInUser != null || IsAdmin ? "Личный кабинет" : "Войти";
        }

        public ICommand SearchCommand { get; private set; }
        public ICommand FilterCategoryCommand { get; private set; }
        public ICommand ChangeLanguageCommand { get; private set; }
        public ICommand AddProductCommand { get; private set; }
        public ICommand EditProductCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand OpenAuthOrCabinetCommand { get; private set; }
        public ICommand AddToCartCommand { get; private set; }
        public ICommand OpenProductDetailsCommand { get; private set; }
        public ICommand OpenCartCommand { get; private set; }
        public ICommand PayForCartCommand { get; private set; }
        public ICommand OpenItemDetailsCommand { get; }


        public MainViewModel(/*IRectangleItemRepository rectangleItemRepository*/)
        {
            //_rectangleItemRepository = rectangleItemRepository;
            _unitOfWork = new UnitOfWork();
            OpenProductDetailsCommand = new RelayCommand(OpenProductDetails);
            _items = new ObservableCollection<RectangleItem>();
            _filteredItems = new ObservableCollection<RectangleItem>();
            _undoStack = new Stack<ActionRecord>();
            _redoStack = new Stack<ActionRecord>();
            _languageItems = new Dictionary<string, ObservableCollection<RectangleItem>>
            {
                { "ru", new ObservableCollection<RectangleItem>() },
                { "en", new ObservableCollection<RectangleItem>() }
            };

            SearchCommand = new RelayCommand(_ => ApplyFilters());
            FilterCategoryCommand = new RelayCommand(FilterByCategory);
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);
            AddProductCommand = new RelayCommand(AddProduct, _ => IsAdmin);
            EditProductCommand = new RelayCommand(EditProduct, _ => IsAdmin);
            UndoCommand = new RelayCommand(Undo, _ => _undoStack.Count > 0);
            RedoCommand = new RelayCommand(Redo, _ => _redoStack.Count > 0);
            OpenAuthOrCabinetCommand = new RelayCommand(OpenAuthOrCabinet);
            AddToCartCommand = new RelayCommand(AddToCart);
            OpenCartCommand = new RelayCommand(OpenCart);
            PayForCartCommand = new RelayCommand(PayForCart);
            OpenItemDetailsCommand = new RelayCommand(OpenItemDetails);

            InitializeDatabase();
            LoadItemsFromDatabase();
            LanguageManager.ChangeLanguage("ru");
            PriceFilter = WithoutSort;
            ApplyFilters();
        }
       
        private void OpenItemDetails(object parameter)
        {
            if (parameter is RectangleItem item)
            {
                var window = new ProductDetailsWindow(item, this, LoggedInUser);
                window.ShowDialog();
            }
        }

        private ICommand _manageReviewsCommand;
        public ICommand ManageReviewsCommand
        {
            get
            {
                return _manageReviewsCommand ?? (_manageReviewsCommand = new ActionCommand(() =>
                {
                    if (IsAdmin)
                    {
                        var manageReviewsWindow = new ManageReviewsWindow();
                        manageReviewsWindow.ShowDialog();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Доступно только для администраторов.", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    }
                }));
            }
        }
        private void PayForCart(object parameter)
        {
            if (LoggedInUser == null || !LoggedInUser.PurchasedProducts.Any())
            {
                MessageBox.Show("Корзина пуста или вы не вошли в аккаунт.");
                return;
            }

            using (var context = new AppDbContext())
            {
                var user = context.Users
                    .Include(u => u.PurchasedProducts)
                    .FirstOrDefault(u => u.Id == LoggedInUser.Id);

                if (user != null)
                {
                    // Save cart items to OrderHistory
                    foreach (var item in LoggedInUser.PurchasedProducts)
                    {
                        var dbItem = context.RectangleItems
                            .Include(i => i.ItemTranslations)
                            .FirstOrDefault(i => i.Id == item.Id);
                        if (dbItem != null)
                        {
                            var translation = dbItem.ItemTranslations
                                .FirstOrDefault(t => t.LanguageCode == LanguageManager.CurrentLanguage);
                            context.OrderHistory.Add(new OrderHistory
                            {
                                UserId = user.Id,
                                RectangleItemId = item.Id,
                                PurchaseDate = DateTime.Now,
                                Name = translation?.Name ?? item.Name ?? "Без названия",
                                Price = item.Price,
                                Color = item.Color,
                                Category = translation?.Category,
                                Availability = translation?.Availability
                            });
                        }
                    }

                    // Clear PurchasedProducts
                    user.PurchasedProducts.Clear();
                    context.SaveChanges();

                    // Update local state
                    LoggedInUser.PurchasedProducts = new List<RectangleItem>();
                    // Reload OrderHistory
                    var orderHistoryItems = context.OrderHistory
                        .Where(o => o.UserId == LoggedInUser.Id)
                        .AsNoTracking()
                        .ToList();
                    LoggedInUser.OrderHistory = new List<OrderHistory>(orderHistoryItems);

                    MessageBox.Show("Корзина успешно оплачена!");
                    OnPropertyChanged(nameof(LoggedInUser));
                }
            }
        }

        private void OpenProductDetails(object parameter)
        {
            if (parameter is RectangleItem product)
            {
                var detailsWindow = new ProductDetailsWindow(product, this, LoggedInUser);
                detailsWindow.ShowDialog();
            }
        }
        private void OpenCart(object parameter)
        {
            if (LoggedInUser != null)
            {
                var cartItems = new List<RectangleItem>(); // Temporary list to hold items
                using (var context = new AppDbContext())
                {
                    // Reload the user with their PurchasedProducts and translations
                    var user = context.Users
                        .Include(u => u.PurchasedProducts.Select(p => p.ItemTranslations))
                        .AsNoTracking() // Prevent tracking to avoid context issues
                        .FirstOrDefault(u => u.Id == LoggedInUser.Id);

                    if (user != null)
                    {
                        foreach (var dbItem in user.PurchasedProducts)
                        {
                            var translation = dbItem.ItemTranslations
                                .FirstOrDefault(t => t.LanguageCode == LanguageManager.CurrentLanguage);
                            var localItem = new RectangleItem(dbItem)
                            {
                                Name = translation?.Name ?? dbItem.Name ?? "Без названия",
                                Description = translation?.Description,
                                Category = translation?.Category,
                                Availability = translation?.Availability
                            };
                            cartItems.Add(localItem);
                            Debug.WriteLine($"Added item to cart: Id={localItem.Id}, Name={localItem.Name}");
                        }
                    }
                }

                // Update PurchasedProducts outside the using block
                LoggedInUser.PurchasedProducts.Clear();
                foreach (var item in cartItems)
                {
                    LoggedInUser.PurchasedProducts.Add(item);
                }
                Debug.WriteLine($"Local PurchasedProducts count: {LoggedInUser.PurchasedProducts.Count}");

                var cartWindow = new CartWindow(this);
                cartWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, войдите в аккаунт, чтобы просмотреть корзину.");
            }
        }
        //private void InitializeDatabase()
        //{
        //    try
        //    {
        //        using (var context = new AppDbContext())
        //        {
        //            if (!context.RectangleItems.Any())
        //            {
        //                string ruJsonPath = ConfigurationManager.AppSettings?["JsonFilePathRu"] ?? "Data/rectangles.ru.json";
        //                string enJsonPath = ConfigurationManager.AppSettings?["JsonFilePathEn"] ?? "Data/rectangles.en.json";

        //                if (File.Exists(ruJsonPath))
        //                {
        //                    string ruJson = File.ReadAllText(ruJsonPath);
        //                    var ruItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ruJson);
        //                    foreach (var item in ruItems)
        //                    {
        //                        var rectangleItem = new RectangleItem
        //                        {
        //                            Id = Guid.Parse(item["Id"].ToString()),
        //                            Price = Convert.ToDecimal(item["Price"]),
        //                            Color = item["Color"].ToString(),
        //                            Image = item["Image"]?.ToString()
        //                        };
        //                        context.RectangleItems.Add(rectangleItem);

        //                        var ruTranslation = new ItemTranslation
        //                        {
        //                            RectangleItemId = rectangleItem.Id,
        //                            LanguageCode = "ru",
        //                            Name = item["Name"].ToString(),
        //                            Description = item["Description"]?.ToString(),
        //                            Category = item["Category"]?.ToString(),
        //                            Availability = item["Availability"]?.ToString()
        //                        };
        //                        context.ItemTranslations.Add(ruTranslation);
        //                    }
        //                }

        //                if (File.Exists(enJsonPath))
        //                {
        //                    string enJson = File.ReadAllText(enJsonPath);
        //                    var enItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(enJson);
        //                    foreach (var item in enItems)
        //                    {
        //                        var itemId = Guid.Parse(item["Id"].ToString());
        //                        var existingItem = context.RectangleItems.Local.FirstOrDefault(i => i.Id == itemId);
        //                        if (existingItem != null)
        //                        {
        //                            var enTranslation = new ItemTranslation
        //                            {
        //                                RectangleItemId = itemId,
        //                                LanguageCode = "en",
        //                                Name = item["Name"].ToString(),
        //                                Description = item["Description"]?.ToString(),
        //                                Category = item["Category"]?.ToString(),
        //                                Availability = item["Availability"]?.ToString()
        //                            };
        //                            context.ItemTranslations.Add(enTranslation);
        //                        }
        //                    }
        //                }
        //            }

        //            if (!context.Users.Any())
        //            {
        //                string usersJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        //                if (File.Exists(usersJsonPath))
        //                {
        //                    string usersJson = File.ReadAllText(usersJsonPath);
        //                    var users = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(usersJson);
        //                    foreach (var user in users)
        //                    {
        //                        context.Users.Add(new User
        //                        {
        //                            Username = user["Username"].ToString(),
        //                            IsAdmin = Convert.ToBoolean(user["IsAdmin"])
        //                        });
        //                    }
        //                }
        //            }

        //            context.SaveChanges();
        //            Debug.WriteLine("InitializeDatabase: Imported data from JSON to database");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"InitializeDatabase error: {ex.Message}");
        //        MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}");
        //    }
        //}
        private void InitializeDatabase()
        {
            try
            {
                //используется UnitOfWork
                if (!_unitOfWork.RectangleItems.GetAll().Any())
                {
                    string ruJsonPath = ConfigurationManager.AppSettings?["JsonFilePathRu"] ?? "Data/rectangles.ru.json";
                    string enJsonPath = ConfigurationManager.AppSettings?["JsonFilePathEn"] ?? "Data/rectangles.en.json";

                    if (File.Exists(ruJsonPath))
                    {
                        string ruJson = File.ReadAllText(ruJsonPath);
                        var ruItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(ruJson);
                        foreach (var item in ruItems)
                        {
                            var rectangleItem = new RectangleItem
                            {
                                Id = Guid.Parse(item["Id"].ToString()),
                                Price = Convert.ToDecimal(item["Price"]),
                                Color = item["Color"].ToString(),
                                Image = item["Image"]?.ToString()
                            };
                            _unitOfWork.RectangleItems.Add(rectangleItem);

                            var ruTranslation = new ItemTranslation
                            {
                                RectangleItemId = rectangleItem.Id,
                                LanguageCode = "ru",
                                Name = item["Name"].ToString(),
                                Description = item["Description"]?.ToString(),
                                Category = item["Category"]?.ToString(),
                                Availability = item["Availability"]?.ToString()
                            };
                            _unitOfWork.Context.Set<ItemTranslation>().Add(ruTranslation); // Direct access for ItemTranslation
                        }
                    }

                    if (File.Exists(enJsonPath))
                    {
                        string enJson = File.ReadAllText(enJsonPath);
                        var enItems = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(enJson);
                        foreach (var item in enItems)
                        {
                            var itemId = Guid.Parse(item["Id"].ToString());
                            var existingItem = _unitOfWork.RectangleItems.GetById(itemId);
                            if (existingItem != null)
                            {
                                var enTranslation = new ItemTranslation
                                {
                                    RectangleItemId = itemId,
                                    LanguageCode = "en",
                                    Name = item["Name"].ToString(),
                                    Description = item["Description"]?.ToString(),
                                    Category = item["Category"]?.ToString(),
                                    Availability = item["Availability"]?.ToString()
                                };
                                _unitOfWork.Context.Set<ItemTranslation>().Add(enTranslation);
                            }
                        }
                    }
                }

                if (!_unitOfWork.Users.GetAll().Any())
                {
                    string usersJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
                    if (File.Exists(usersJsonPath))
                    {
                        string usersJson = File.ReadAllText(usersJsonPath);
                        var users = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(usersJson);
                        foreach (var user in users)
                        {
                            _unitOfWork.Users.Add(new User
                            {
                                Username = user["Username"].ToString(),
                                IsAdmin = Convert.ToBoolean(user["IsAdmin"])
                            });
                        }
                    }
                }

                _unitOfWork.Save();
                Debug.WriteLine("InitializeDatabase: Imported data from JSON to database");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InitializeDatabase error: {ex.Message}");
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}");
            }
        }
        private void AddToCart(object parameter)
        {
            if (parameter is RectangleItem item && LoggedInUser != null)
            {
                using (var context = new AppDbContext())
                {
                    var user = context.Users
                        .Include(u => u.PurchasedProducts)
                        .FirstOrDefault(u => u.Id == LoggedInUser.Id);

                    if (user != null)
                    {
                        if (!user.PurchasedProducts.Any(p => p.Id == item.Id))
                        {
                            var dbItem = context.RectangleItems
                                .Include(i => i.ItemTranslations)
                                .FirstOrDefault(i => i.Id == item.Id);

                            if (dbItem != null)
                            {
                                var translation = dbItem.ItemTranslations
                                    .FirstOrDefault(t => t.LanguageCode == LanguageManager.CurrentLanguage);
                                string itemName = translation?.Name ?? item.Name ?? "Без названия";

                                user.PurchasedProducts.Add(dbItem);
                                try
                                {
                                    context.SaveChanges();
                                    var localItem = new RectangleItem(dbItem)
                                    {
                                        Name = itemName,
                                        Description = translation?.Description,
                                        Category = translation?.Category,
                                        Availability = translation?.Availability
                                    };
                                    LoggedInUser.PurchasedProducts.Add(localItem);
                                    MessageBox.Show($"Товар '{itemName}' добавлен в корзину!");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"AddToCart error: {ex.Message}");
                                    MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Товар не найден в базе данных.");
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Товар '{item.Name}' уже в корзине!");
                        }
                    }
                }
            }
            else if (LoggedInUser == null)
            {
                MessageBox.Show("Пожалуйста, войдите в аккаунт, чтобы добавить товар в корзину.");
            }
        }

      
        private void OpenAuthOrCabinet(object parameter)
        {
            if (LoggedInUser != null || IsAdmin)
            {
                var cabinetWindow = new PersonalCabinetWindow(LoggedInUser);
                cabinetWindow.ShowDialog();
            }
            else
            {
                var authWindow = new AuthorizationWindow();
                if (authWindow.ShowDialog() == true)
                {
                    IsAdmin = authWindow.IsAdmin;
                    using (var context = new AppDbContext())
                    {
                        var authenticatedUser = context.Users
                            .AsNoTracking()
                            .FirstOrDefault(u => u.Id == authWindow.AuthenticatedUser.Id);
                        if (authenticatedUser != null)
                        {
                            LoggedInUser = authenticatedUser; // Setter will load cart items
                        }
                    }
                }
            }
        }

        private void ChangeLanguage(object parameter)
        {
            if (parameter is string langCode)
            {
                string oldColor = SelectedColor;
                string oldCategory = SelectedCategory;
                string oldPriceFilterKey = MapPriceFilterToKey(PriceFilter);

                LanguageManager.ChangeLanguage(langCode);
                LoadItemsFromDatabase();
                RefreshAllTexts();

                SelectedColor = AvailableColors.Contains(oldColor) ? oldColor : AvailableColors.First();
                SelectedCategory = oldCategory;
                PriceFilter = MapKeyToPriceFilter(oldPriceFilterKey);
                OnPropertyChanged(nameof(PriceFilter));
                ApplyFilters();
            }
        }

        private string MapPriceFilterToKey(string filter)
        {
            return filter == WithoutSort ? "None" : filter == SortAsc ? "Asc" : "Desc";
        }

        private string MapKeyToPriceFilter(string key)
        {
            return key == "Asc" ? SortAsc : key == "Desc" ? SortDesc : WithoutSort;
        }

        private void LoadItemsFromDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var items = context.RectangleItems
                        .Include(i => i.ItemTranslations)
                        .ToList();

                    Items.Clear();
                    _languageItems["ru"].Clear();
                    _languageItems["en"].Clear();

                    foreach (var item in items)
                    {
                        var ruTranslation = item.ItemTranslations.FirstOrDefault(t => t.LanguageCode == "ru");
                        var enTranslation = item.ItemTranslations.FirstOrDefault(t => t.LanguageCode == "en");

                        if (ruTranslation != null)
                        {
                            var ruItem = new RectangleItem(item)
                            {
                                Name = ruTranslation.Name,
                                Description = ruTranslation.Description,
                                Category = ruTranslation.Category,
                                Availability = ruTranslation.Availability
                            };
                            _languageItems["ru"].Add(ruItem);
                        }

                        if (enTranslation != null)
                        {
                            var enItem = new RectangleItem(item)
                            {
                                Name = enTranslation.Name,
                                Description = enTranslation.Description,
                                Category = enTranslation.Category,
                                Availability = enTranslation.Availability
                            };
                            _languageItems["en"].Add(enItem);
                        }

                        if (LanguageManager.CurrentLanguage == "ru" && ruTranslation != null)
                        {
                            var displayItem = new RectangleItem(item)
                            {
                                Name = ruTranslation.Name,
                                Description = ruTranslation.Description,
                                Category = ruTranslation.Category,
                                Availability = ruTranslation.Availability
                            };
                            Items.Add(displayItem);
                        }
                        else if (LanguageManager.CurrentLanguage == "en" && enTranslation != null)
                        {
                            var displayItem = new RectangleItem(item)
                            {
                                Name = enTranslation.Name,
                                Description = enTranslation.Description,
                                Category = enTranslation.Category,
                                Availability = enTranslation.Availability
                            };
                            Items.Add(displayItem);
                        }
                    }

                    ApplyFilters();
                    Debug.WriteLine($"Loaded {Items.Count} items from database for {LanguageManager.CurrentLanguage}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadItemsFromDatabase error: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AddProduct(object parameter)
        {
            EditProductWindow editWindow = new EditProductWindow();
            if (editWindow.ShowDialog() == true && editWindow.Product != null)
            {
                var newItem = editWindow.Product;
                using (var context = new AppDbContext())
                {
                    var rectangleItem = new RectangleItem
                    {
                        Id = newItem.Id,
                        Price = newItem.Price,
                        Color = newItem.Color,
                        Image = newItem.Image
                    };
                    context.RectangleItems.Add(rectangleItem);

                    var translation = new ItemTranslation
                    {
                        RectangleItemId = newItem.Id,
                        LanguageCode = LanguageManager.CurrentLanguage,
                        Name = newItem.Name,
                        Description = newItem.Description,
                        Category = newItem.Category,
                        Availability = newItem.Availability
                    };
                    context.ItemTranslations.Add(translation);

                    var otherLang = LanguageManager.CurrentLanguage == "ru" ? "en" : "ru";
                    var translatedItem = TranslateItem(newItem, otherLang);
                    var otherTranslation = new ItemTranslation
                    {
                        RectangleItemId = newItem.Id,
                        LanguageCode = otherLang,
                        Name = translatedItem.Name,
                        Description = translatedItem.Description,
                        Category = translatedItem.Category,
                        Availability = translatedItem.Availability
                    };
                    context.ItemTranslations.Add(otherTranslation);

                    context.SaveChanges();
                }

                Items.Add(newItem);
                _languageItems[LanguageManager.CurrentLanguage].Add(newItem);
                _languageItems[LanguageManager.CurrentLanguage == "ru" ? "en" : "ru"].Add(TranslateItem(newItem, LanguageManager.CurrentLanguage == "ru" ? "en" : "ru"));
                _undoStack.Push(new ActionRecord
                {
                    ActionType = ActionType.Add,
                    Item = new RectangleItem(newItem)
                });
                _redoStack.Clear();
                ApplyFilters();
                Debug.WriteLine($"AddProduct: Added item {newItem.Name}");
                ((RelayCommand)UndoCommand)?.RaiseCanExecuteChanged();
                ((RelayCommand)RedoCommand)?.RaiseCanExecuteChanged();
            }
        }

        private void EditProduct(object parameter)
        {
            if (parameter is RectangleItem item)
            {
                Debug.WriteLine($"EditProduct: Attempting to edit item {item.Name} (Id: {item.Id})");
                EditProductWindow editWindow = new EditProductWindow(item);
                if (editWindow.ShowDialog() == true)
                {
                    using (var context = new AppDbContext())
                    {
                        if (editWindow.Product == null)
                        {
                            var dbItem = context.RectangleItems.Find(item.Id);
                            if (dbItem != null)
                            {
                                context.ItemTranslations.RemoveRange(context.ItemTranslations.Where(t => t.RectangleItemId == item.Id));
                                context.RectangleItems.Remove(dbItem);
                                Items.Remove(Items.FirstOrDefault(i => i.Id == item.Id));
                                _languageItems["ru"].Remove(_languageItems["ru"].FirstOrDefault(i => i.Id == item.Id));
                                _languageItems["en"].Remove(_languageItems["en"].FirstOrDefault(i => i.Id == item.Id));
                                _undoStack.Push(new ActionRecord
                                {
                                    ActionType = ActionType.Delete,
                                    Item = new RectangleItem(item),
                                    Index = -1
                                });
                                _redoStack.Clear();
                                context.SaveChanges();
                                Debug.WriteLine($"EditProduct: Removed item {item.Name}");
                            }
                        }
                        else
                        {
                            var dbItem = context.RectangleItems.Find(item.Id);
                            if (dbItem != null)
                            {
                                dbItem.Price = editWindow.Product.Price;
                                dbItem.Color = editWindow.Product.Color;
                                dbItem.Image = editWindow.Product.Image;

                                var translation = context.ItemTranslations
                                    .FirstOrDefault(t => t.RectangleItemId == item.Id && t.LanguageCode == LanguageManager.CurrentLanguage);
                                if (translation != null)
                                {
                                    translation.Name = editWindow.Product.Name;
                                    translation.Description = editWindow.Product.Description;
                                    translation.Category = editWindow.Product.Category;
                                    translation.Availability = editWindow.Product.Availability;
                                }
                                else
                                {
                                    context.ItemTranslations.Add(new ItemTranslation
                                    {
                                        RectangleItemId = item.Id,
                                        LanguageCode = LanguageManager.CurrentLanguage,
                                        Name = editWindow.Product.Name,
                                        Description = editWindow.Product.Description,
                                        Category = editWindow.Product.Category,
                                        Availability = editWindow.Product.Availability
                                    });
                                }

                                var otherLang = LanguageManager.CurrentLanguage == "ru" ? "en" : "ru";
                                var translatedItem = TranslateItem(editWindow.Product, otherLang);
                                var otherTranslation = context.ItemTranslations
                                    .FirstOrDefault(t => t.RectangleItemId == item.Id && t.LanguageCode == otherLang);
                                if (otherTranslation != null)
                                {
                                    otherTranslation.Name = translatedItem.Name;
                                    otherTranslation.Description = translatedItem.Description;
                                    otherTranslation.Category = translatedItem.Category;
                                    otherTranslation.Availability = translatedItem.Availability;
                                }
                                else
                                {
                                    context.ItemTranslations.Add(new ItemTranslation
                                    {
                                        RectangleItemId = item.Id,
                                        LanguageCode = otherLang,
                                        Name = translatedItem.Name,
                                        Description = translatedItem.Description,
                                        Category = translatedItem.Category,
                                        Availability = translatedItem.Availability
                                    });
                                }

                                var itemInItems = Items.FirstOrDefault(i => i.Id == item.Id);
                                if (itemInItems != null)
                                {
                                    int index = Items.IndexOf(itemInItems);
                                    var oldItem = new RectangleItem(itemInItems);
                                    Items[index] = editWindow.Product;

                                    var ruItem = _languageItems["ru"].FirstOrDefault(i => i.Id == item.Id);
                                    if (ruItem != null)
                                    {
                                        int ruIndex = _languageItems["ru"].IndexOf(ruItem);
                                        _languageItems["ru"][ruIndex] = LanguageManager.CurrentLanguage == "ru" ? editWindow.Product : TranslateItem(editWindow.Product, "ru");
                                    }
                                    else
                                    {
                                        _languageItems["ru"].Add(TranslateItem(editWindow.Product, "ru"));
                                    }

                                    var enItem = _languageItems["en"].FirstOrDefault(i => i.Id == item.Id);
                                    if (enItem != null)
                                    {
                                        int enIndex = _languageItems["en"].IndexOf(enItem);
                                        _languageItems["en"][enIndex] = LanguageManager.CurrentLanguage == "en" ? editWindow.Product : TranslateItem(editWindow.Product, "en");
                                    }
                                    else
                                    {
                                        _languageItems["en"].Add(TranslateItem(editWindow.Product, "en"));
                                    }

                                    _undoStack.Push(new ActionRecord
                                    {
                                        ActionType = ActionType.Edit,
                                        Item = new RectangleItem(editWindow.Product),
                                        PreviousItem = oldItem,
                                        Index = index
                                    });
                                    _redoStack.Clear();
                                    context.SaveChanges();
                                    Debug.WriteLine($"EditProduct: Updated item {item.Name} to {editWindow.Product.Name}");
                                }
                            }
                        }
                    }
                    ApplyFilters();
                    ((RelayCommand)UndoCommand)?.RaiseCanExecuteChanged();
                    ((RelayCommand)RedoCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private RectangleItem TranslateItem(RectangleItem item, string targetLang)
        {
            var translated = new RectangleItem(item);
            if (targetLang == "en")
            {
                translated.Name = item.Name == "Кухонный стол" ? "Kitchen Table" : item.Name;
                translated.Description = item.Description == "Деревянный стол для кухни" ? "Wooden table for kitchen" : item.Description;
                translated.Category = item.Category == "Кухня" ? "Kitchen" : item.Category;
                translated.Availability = item.Availability == "В наличии" ? "In stock" : item.Availability;
            }
            else if (targetLang == "ru")
            {
                translated.Name = item.Name == "Kitchen Table" ? "Кухонный стол" : item.Name;
                translated.Description = item.Description == "Wooden table for kitchen" ? "Деревянный стол для кухни" : item.Description;
                translated.Category = item.Category == "Kitchen" ? "Кухня" : item.Category;
                translated.Availability = item.Availability == "In stock" ? "В наличии" : item.Availability;
            }
            return translated;
        }

        private void ApplyFilters()
        {
            if (Items == null || Items.Count == 0)
            {
                FilteredItems = new ObservableCollection<RectangleItem>();
                return;
            }

            List<RectangleItem> filtered = Items.Where(item =>
            {
                bool searchMatch = string.IsNullOrEmpty(SearchText) ||
                                  (item.Name?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                   item.Description?.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);

                bool categoryMatch = string.IsNullOrEmpty(SelectedCategory) ||
                                     item.Category?.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase) == true;

                bool colorMatch = SelectedColor == AllText ||
                                  item.Color.Equals(SelectedColor, StringComparison.OrdinalIgnoreCase);

                return searchMatch && categoryMatch && colorMatch;
            }).ToList();

            string priceFilterKey = MapPriceFilterToKey(PriceFilter);
            if (priceFilterKey == "Asc")
            {
                filtered = filtered.OrderBy(item => item.Price).ToList();
            }
            else if (priceFilterKey == "Desc")
            {
                filtered = filtered.OrderByDescending(item => item.Price).ToList();
            }

            FilteredItems = new ObservableCollection<RectangleItem>(filtered);
            Debug.WriteLine($"ApplyFilters: Items={Items.Count}, FilteredItems={FilteredItems.Count}");
        }

        private void FilterByCategory(object category)
        {
            SelectedCategory = category?.ToString();
        }

        private void Undo(object parameter)
        {
            // Placeholder: Implement undo logic
        }

        private void Redo(object parameter)
        {
            // Placeholder: Implement redo logic
        }

        public List<string> AvailableColors
        {
            get => new List<string>
            {
                AllText,
                LanguageManager.GetTranslation("UI.Colors.Black"),
                LanguageManager.GetTranslation("UI.Colors.White"),
                LanguageManager.GetTranslation("UI.Colors.Wood")
            };
        }

        public List<string> AvailablePrice
        {
            get => new List<string>
            {
                WithoutSort,
                SortAsc,
                SortDesc
            };
        }

        public string WithoutSort => LanguageManager.GetTranslation("UI.WithoutSort");
        public string SortDesc => LanguageManager.GetTranslation("UI.SortDesc");
        public string SortAsc => LanguageManager.GetTranslation("UI.SortAsc");
        public string Search => LanguageManager.GetTranslation("UI.Search");
        public string Price => LanguageManager.GetTranslation("UI.Price");
        public string Color => LanguageManager.GetTranslation("UI.Color");
        public string KitchenText => LanguageManager.GetTranslation("UI.Categories.Kitchen");
        public string BedroomText => LanguageManager.GetTranslation("UI.Categories.Bedroom");
        public string BathText => LanguageManager.GetTranslation("UI.Categories.Bath");
        public string AllText => LanguageManager.GetTranslation("UI.All");
        public string ColorLabel => LanguageManager.GetTranslation("UI.Color");

        private void RefreshAllTexts()
        {
            OnPropertyChanged(nameof(WithoutSort));
            OnPropertyChanged(nameof(SortDesc));
            OnPropertyChanged(nameof(SortAsc));
            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(KitchenText));
            OnPropertyChanged(nameof(BedroomText));
            OnPropertyChanged(nameof(BathText));
            OnPropertyChanged(nameof(AllText));
            OnPropertyChanged(nameof(AvailableColors));
            OnPropertyChanged(nameof(ColorLabel));
            OnPropertyChanged(nameof(AvailablePrice));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ActionRecord
        {
            public ActionType ActionType { get; set; }
            public RectangleItem Item { get; set; }
            public RectangleItem PreviousItem { get; set; }
            public int Index { get; set; }
        }

        public enum ActionType
        {
            Add,
            Edit,
            Delete
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    public class StringToDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal price)
            {
                return price;
            }
            return 0m;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal price)
            {
                return price;
            }
            return 0m;
        }
    }
}

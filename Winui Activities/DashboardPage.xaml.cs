using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using WinUi_Inventory_Management.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        private User _loggedInUser;
        public List<Order> Orders;
        private List<Order> _allOrders;
        AppDbContext _dbContext;
        private int? _selectedMonth = null;

        public DashboardPage()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            _allOrders = new List<Order>();
            Orders = new List<Order>();

        }

       

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is User user)
            {
                _loggedInUser = user;

                // Set data in UI
                WelcomeText.Text = $"Welcome, {_loggedInUser.FullName}";
                try
                {
                    if (!string.IsNullOrEmpty(_loggedInUser.ImageName))
                    {
                        StorageFolder profileFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("ProfileImage");
                        StorageFile imageFile = await profileFolder.GetFileAsync(_loggedInUser.ImageName);

                        using var stream = await imageFile.OpenAsync(FileAccessMode.Read);
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        ProfileImage.Source = bitmap;
                    }
                    else
                    {
                        ProfileImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ProfileImage/profile_picture.png"));
                    }
                }
                catch
                {
                    ProfileImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/ProfileImage/profile_picture.png"));
                }

                var existingOrder = _dbContext.Orders.FirstOrDefault(u => u.UserId == _loggedInUser.Id);
                if (existingOrder == null)
                {
                    P0.Text = "0";
                    NoOrdersText.Visibility = Visibility.Visible;
                    return; 
                }
                NoOrdersText.Visibility = Visibility.Collapsed;
                var totalProduct = _dbContext.Orders.Count(Orders => Orders.UserId == _loggedInUser.Id);
                var userTotalPrice = _dbContext.Orders
                    .Where(o => o.UserId == _loggedInUser.Id)
                    .Sum(o => o.TotalPrice);
                var userTotalDiscount = _dbContext.Orders
                    .Where(o => o.UserId == _loggedInUser.Id)
                    .Sum(o => o.TotalDiscount);

                if (totalProduct > 0)
                {
                    P0.Text = $"{totalProduct}";
                }
                else
                {
                    P0.Text = "No products found.";
                }
                P1.Text = $"{userTotalPrice}";
                P2.Text = $"{userTotalDiscount}";

                _allOrders = GetOrders();
                ApplyMonthFilter();
                
            }
        }

        private void ApplyFilters()
        {
            string searchText = MyAutoSuggestBox.Text?.Trim().ToLower();
            IEnumerable<Order> filtered = _allOrders;

            // Apply month filter first
            if (_selectedMonth.HasValue)
            {
                filtered = filtered.Where(o => o.CreatedAt.Month == _selectedMonth.Value);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(o =>
                    o.OrderName.ToLower().Contains(searchText) ||
                    o.TotalPrice.ToString().Contains(searchText)
                );
            }

            // Update Orders and ListView
            Orders = filtered.ToList();
            MyOrdersListView.ItemsSource = Orders; // ensure ListView refreshes

            if (Orders.Count == 0)
            {
                NoOrdersText.Visibility = Visibility.Visible;
                return;
            }

            NoOrdersText.Visibility = Visibility.Collapsed;

            // Update totals (P0, P1, P2) based on filtered data
            var totalProduct = Orders.Count(Orders => Orders.UserId == _loggedInUser.Id);
            var userTotalPrice = Orders.Sum(o => o.TotalPrice);
            var userTotalDiscount = Orders.Sum(o => o.TotalDiscount);

            P0.Text = $"{totalProduct}";
            P1.Text = $"{userTotalPrice}";
            P2.Text = $"{userTotalDiscount}";
        }


        private List<Order> GetOrders()
        {
            var localSetting = ApplicationData.Current.LocalSettings;
            var userId = int.Parse(localSetting.Values["UserId"].ToString());
            using var context = new AppDbContext();
            var Orders = context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToList();
            if (Orders.Count == 0)
            {
                NoOrdersText.Visibility = Visibility.Visible;
                return [];
            }
            NoOrdersText.Visibility = Visibility.Collapsed;
            return Orders;

        }

        private void ApplyMonthFilter()
        {
            IEnumerable<Order> filtered = _allOrders;

            if (_selectedMonth.HasValue)
            {
                filtered = filtered.Where(o => o.CreatedAt.Month == _selectedMonth.Value);
            }

            Orders = filtered.ToList();
            MyOrdersListView.ItemsSource = Orders; // refresh x:Bind
            if (Orders.Count == 0)
            {
                NoOrdersText.Visibility = Visibility.Visible;
                return;
            }
            NoOrdersText.Visibility = Visibility.Collapsed;
        }

        private void MonthSelected(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem item && int.TryParse(item.Tag.ToString(), out int month))
            {
                _selectedMonth = month == 0 ? (int?)null : month;
                MonthDropDown.Content = item.Text;
                ApplyFilters();
            }
        }
        private void MyAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                ApplyFilters();
            }
        }




    }
}

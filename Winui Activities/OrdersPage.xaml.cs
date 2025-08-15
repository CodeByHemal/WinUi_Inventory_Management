using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using WinRT.WinUi_Inventory_ManagementVtableClasses;
using WinUi_Inventory_Management.Models;
using WinUi_Inventory_Management.ViewModels;

namespace WinUi_Inventory_Management.Winui_Activities
{
    public sealed partial class OrdersPage : Page
    {
        public OrdersViewModel ViewModel { get; } = new OrdersViewModel();
        private User _loggedInUser;
        private AppDbContext _context;

        public OrdersPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            _context = new AppDbContext();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User user)
            {
                _loggedInUser = user;
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
            }
        }

        // Generic helper to execute commands
        private void ExecuteProductCommand(object sender, IRelayCommand<Product> command)
        {
            if (sender is Button btn && btn.DataContext is Product product)
            {
                command.Execute(product);
            }
        }

        private void Increment_Quantity_Btn(object sender, RoutedEventArgs e)
            => ExecuteProductCommand(sender, ViewModel.IncrementQuantityCommand);

        private void Decrement_Quantity_Btn(object sender, RoutedEventArgs e)
            => ExecuteProductCommand(sender, ViewModel.DecrementQuantityCommand);

        private void Delete_Product_Btn(object sender, RoutedEventArgs e)
            => ExecuteProductCommand(sender, ViewModel.RemoveProductCommand);

        private void Save_Order(object sender, RoutedEventArgs e)
        {
            if (OrderInput.Text.Trim() == "")
            {
                OrderError.Visibility = Visibility.Visible;
                OrderError.Text = "Please enter a valid order name.";
                return;
            }

            if (ViewModel.Products.Count == 0)
            {
                OrderError.Visibility = Visibility.Visible;
                OrderError.Text = "Please add at least one product to the order.";
                return;
            }

            OrderError.Visibility = Visibility.Collapsed;

            // Save the order

            var order = new Order
            {
                OrderName = OrderInput.Text.Trim(),
                TotalPrice = ViewModel.AllProductTotalPrice,
                UserId = _loggedInUser.Id,
                TotalDiscount = ViewModel.TotalDiscountAmount,
                CreatedAt = DateTime.Now,
                Items = ViewModel.Products.Select(p => new OrderItem
                {
                    ItemName = p.Name, 
                    ItemPrice = Convert.ToDecimal(p.Price),
                    ItemQuantity = p.Quantity,
                    Discount = Convert.ToDecimal(p.Discount),
                    Total = Convert.ToDecimal(p.TotalPrice)
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            var savedOrder = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == order.Id);

            if (savedOrder != null)
            {
                // show success dialog

                ShowMessage($"Order '{savedOrder.OrderName}' saved successfully!");
            }

            ViewModel.Clear();
        }
        private async void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Order Status",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();

            
        }
    }
}

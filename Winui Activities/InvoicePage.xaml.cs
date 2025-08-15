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
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinUi_Inventory_Management.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InvoicePage : Page
    {
        private User _loggedInUser;
        public List<Order> Orders;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                NoInvoices.Visibility = Visibility.Visible;
                return new List<Order>();
            }
            NoInvoices.Visibility = Visibility.Collapsed;
            return Orders;
        }
       

        public InvoicePage()
        {
            InitializeComponent();
            Orders = GetOrders();
        }


        private async void DownloadInvoice(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            

            if (button?.DataContext is Order order && _loggedInUser != null)
            {
                // 1. Ask user where to save
                var savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.SuggestedFileName = $"Invoice_{order.Id}_{DateTime.Now:yyyyMMddHHmmss}";
                savePicker.FileTypeChoices.Add("PDF", new[] { ".pdf" });

                // WinUI 3 needs window handle
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file == null)
                    return; // User cancelled

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    // 2. Generate PDF in memory
                    InvoiceGenerator.GenerateInvoicePDF(order, _loggedInUser, stream);
                }

                // 3. Open PDF automatically
                await Windows.System.Launcher.LaunchFileAsync(file);
            }
        }
    }
}

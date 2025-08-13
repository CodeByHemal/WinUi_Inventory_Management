using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage;
using System;
using System.Linq;
using Windows.Storage;
using System.Text.RegularExpressions;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        AppDbContext _dbContext;
        public LoginPage()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
        }

        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignupPage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text.Trim();
            string password = PasswordInput.Password;

            bool hasError = false;

            // Reset error messages
            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;

            // Email validation
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailError.Text = "Email is required.";
                EmailError.Visibility = Visibility.Visible;
                hasError = true;
            }
            else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                EmailError.Text = "Please enter a valid email address.";
                EmailError.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError.Text = "Password is required.";
                PasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }

            if (!hasError)
            {
                // Fetch user from DB
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == email);

                if (existingUser == null)
                {
                    EmailError.Text = "Email not registered.";
                    EmailError.Visibility = Visibility.Visible;
                    return;
                }

                if (existingUser.Password != password)
                {
                    PasswordError.Text = "Incorrect password.";
                    PasswordError.Visibility = Visibility.Visible;
                    return;
                }

                // Success
                ShowMessage("Login Successful!");
                //store user info in local settings
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserEmail"] = existingUser.Email;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserFullName"] = existingUser.FullName;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserId"] = existingUser.Id;
                Frame.Navigate(typeof(MainLayoutPage),existingUser); // Change to your main page
            }
        }

        private async void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Login Status",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

    }

}
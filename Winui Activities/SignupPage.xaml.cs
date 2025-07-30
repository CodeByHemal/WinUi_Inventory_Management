using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using WinUi_Inventory_Management.Data.Models;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignupPage : Page
    {
       
        public SignupPage()
        {
            InitializeComponent();
            
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            string fullname = FullName.Text;
            string email = Email.Text;
            string password = Password.Password; // If PasswordBox used
            string confirmPassword = ConfirmPassword.Password;

            bool hasError = false;

            // Reset all error messages
            NameError.Visibility = Visibility.Collapsed;
            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;
            ConfirmPasswordError.Visibility = Visibility.Collapsed;
            
            
            if (string.IsNullOrWhiteSpace(fullname))
            {
                NameError.Text = "Fullname is required.";
                NameError.Visibility = Visibility.Visible;
                hasError = true;
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                EmailError.Text = "Email is required.";
                EmailError.Visibility = Visibility.Visible;
                hasError = true;
            }
            else if (!email.Contains("@"))
            {
                EmailError.Text = "Please enter a valid email address.";
                EmailError.Visibility = Visibility.Visible;
                hasError = true;
            } else if(string.IsNullOrWhiteSpace(password))
            { 
                PasswordError.Text = "Password is required.";
                PasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }


            // Perform actual login check here (API call or local validation)
            if (!hasError)
            {
                using (var db = new AppDbContext())
                {
                    bool exists = db.Users.Any(u => u.Email == email);
                    if (exists)
                    {
                        ShowMessage("Email is already registered.");
                        return;
                    }

                    // Add new user
                    var newUser = new User
                    {
                        Email = email,
                        Password = password // 🔐 In production, hash this!
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }
                Frame.Navigate(typeof(LoginPage));
            }
        }
        private async void ShowMessage(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Registration",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}

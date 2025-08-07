using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Text.RegularExpressions;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignupPage : Page
    {
        AppDbContext _dbContext;

        public SignupPage()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }
        private void Register(object sender, RoutedEventArgs e)
        {
            string fullname = FullName.Text.Trim();
            string email = Email.Text.Trim();
            string password = Password.Password;
            string confirmPassword = ConfirmPassword.Password;

            bool hasError = false;

            // Regex patterns
            string namePattern = @"^[A-Za-z\s]{3,50}$";
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{6,}$"; // Min 6 chars, 1 upper, 1 lower, 1 digit, 1 special

            // Reset error visibility
            NameError.Visibility = Visibility.Collapsed;
            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;
            ConfirmPasswordError.Visibility = Visibility.Collapsed;

            // Full name validation
            if (string.IsNullOrWhiteSpace(fullname) || !Regex.IsMatch(fullname, namePattern))
            {
                NameError.Text = "Enter a valid full name (only letters, 3–50 chars).";
                NameError.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, emailPattern))
            {
                EmailError.Text = "Enter a valid email address.";
                EmailError.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(password) || !Regex.IsMatch(password, passwordPattern))
            {
                PasswordError.Text = "Password must be 6+ chars, with upper, lower, digit & special char.";
                PasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Confirm password match
            if (confirmPassword != password)
            {
                ConfirmPasswordError.Text = "Passwords do not match.";
                ConfirmPasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }

            if (!hasError)
            {
                // Check if email is already registered
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    EmailError.Text = "User with this email already exists.";
                    EmailError.Visibility = Visibility.Visible;
                    return;
                }

                // Save new user
                _dbContext.Users.Add(new User
                {
                    FullName = fullname,
                    Email = email,
                    Password = password,
                    ImageName = "profile_picture.png"
                });

                _dbContext.SaveChanges();
                ShowMessage("User registered successfully.");
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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignupPage));
        }
     
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text;
            string password = PasswordInput.Password; // If PasswordBox used

            bool hasError = false;

            // Reset all error messages
            EmailError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(email))
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
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(password))
            {
                PasswordError.Text = "Password is required.";
                PasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }


            // Perform actual login check here (API call or local validation)
            if (!hasError)
            {
                Frame.Navigate(typeof(MainLayoutPage));
            }
        }


    }

}
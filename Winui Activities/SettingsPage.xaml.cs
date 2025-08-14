using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinUi_Inventory_Management.Models;


// Make sure this is at the top


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class SettingsPage : Page
    {

        private User _loggedInUser;
        AppDbContext _dbContext;
        public SettingsPage()
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is User user)
            {
                _loggedInUser = user;

                Fullname.Text = _loggedInUser.FullName;
                Email.Text = _loggedInUser.Email;
                Password.Text = _loggedInUser.Password;

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

        private async void PickAndSaveImage(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Save image to LocalFolder/ProfileImage
                string fileName = Guid.NewGuid().ToString() + file.FileType;
                StorageFolder profileFolder = await ApplicationData.Current.LocalFolder
                    .CreateFolderAsync("ProfileImage", CreationCollisionOption.OpenIfExists);
                await file.CopyAsync(profileFolder, fileName, NameCollisionOption.ReplaceExisting);

                // Update user in DB
                var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == _loggedInUser.Id);
                if (userToUpdate != null)
                {
                    userToUpdate.ImageName = fileName;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserImageName"] = userToUpdate.Id;

                    _dbContext.SaveChanges();
                    _loggedInUser.ImageName = fileName;
                }

                // Show new image in UI
                StorageFile savedImage = await profileFolder.GetFileAsync(fileName);
                using var stream = await savedImage.OpenAsync(FileAccessMode.Read);
                BitmapImage bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
                ProfileImage.Source = bitmap;



                ShowMessage("Profile image updated successfully.");
            }
        }


        private void SaveProfile(object sender, RoutedEventArgs e)
        {
            string fullname = Fullname.Text.Trim();
            string email = Email.Text.Trim();
            string password = Password.Text;

            bool hasError = false;

            // Regex patterns
            string namePattern = @"^[A-Za-z\s]{3,50}$";
            //string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{6,}$"; // Min 6 chars, 1 upper, 1 lower, 1 digit, 1 special

            // Reset error visibility
            NameError.Visibility = Visibility.Collapsed;
            PasswordError.Visibility = Visibility.Collapsed;

            // Full name validation
            if (string.IsNullOrWhiteSpace(fullname) || !Regex.IsMatch(fullname, namePattern))
            {
                NameError.Text = "Enter a valid full name (only letters, 3–50 chars).";
                NameError.Visibility = Visibility.Visible;
                hasError = true;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(password) || !Regex.IsMatch(password, passwordPattern))
            {
                PasswordError.Text = "Password must be 6+ chars, with upper, lower, digit & special char.";
                PasswordError.Visibility = Visibility.Visible;
                hasError = true;
            }

            if (!hasError)
            {
                var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.Id == _loggedInUser.Id);
                if (userToUpdate != null)
                {
                    userToUpdate.FullName = fullname;
                    userToUpdate.Email = email;
                    userToUpdate.Password = password;

                    _dbContext.SaveChanges();
                    ShowMessage("Profile updated successfully.");
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserEmail"] = userToUpdate.Email;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserFullName"] =userToUpdate.FullName;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["UserId"] = userToUpdate.Id;
                    Frame.Navigate(typeof(DashboardPage), userToUpdate);
                }
                else
                {
                    ShowMessage("Error: User not found.");
                }
                
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


        private void Logout(object sender, RoutedEventArgs e)
        {


        }

    } 
}


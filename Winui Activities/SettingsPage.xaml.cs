using Microsoft.Data.SqlClient;
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
using Windows.Storage.Pickers;
using Microsoft.Data.SqlClient; // Make sure this is at the top


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private User _loggedInUser;
        private string _selectedImageFileName = null;
        public SettingsPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is User user)
            {
                _loggedInUser = user;

                // Fill in the user's data
                FullName.Text = _loggedInUser.FullName;
                Email.Text = _loggedInUser.Email;
                Password.Text = _loggedInUser.Password; // Optional: hide or mask for security


                var uri = new Uri($"ms-appx:///Assets/ProfileImage/{_loggedInUser.ImageName}");
                ProfileImage.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(uri);
            }
        }



        private async void OnProfileImageClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            // WinUI 3 requirement for HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _selectedImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Name); // Unique image name
                StorageFolder profileImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfileImage", CreationCollisionOption.OpenIfExists);
                await file.CopyAsync(profileImageFolder, _selectedImageFileName, NameCollisionOption.ReplaceExisting);

                // Update image preview
                var bitmap = new BitmapImage();
                using var stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmap.SetSourceAsync(stream);
                ProfileImage.Source = bitmap;
            }
        }



        private string _connectionString = "Server=(LocalDB)\\MSSQLLocalDB;Integrated Security = true;AttachDbFilename={DB_PATH};Database=RetailRythmDB;MultipleActiveResultSets=True;";// replace path

        private async void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullName.Text;
            string email = Email.Text;
            string password = Password.Text;
            string imageName = _selectedImageFileName ?? _loggedInUser.ImageName;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE Users
                             SET FullName = @FullName,
                                 Email = @Email,
                                 Password = @Password,
                                 ImageName = @ImageName
                             WHERE Email = @Email"; // use your actual PK column

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@ImageName", imageName);// assume you pass UserID to the page

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                var dialog = new ContentDialog
                {
                    Title = "Profile Saved",
                    Content = "Your profile has been updated successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to update profile: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
            }
        }




    } 
}


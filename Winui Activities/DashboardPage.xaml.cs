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
        public DashboardPage()
        {
            InitializeComponent();
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
            }
        }
    }
}

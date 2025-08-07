using Microsoft.UI.Xaml;

namespace WinUi_Inventory_Management
{
    public sealed partial class App : Application
    {
        public static Window MainWindow { get; private set; } // ? Add this

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var context = new AppDbContext();
            context.Database.EnsureCreated();

            MainWindow = new MainWindow(); // ? Save reference here
            MainWindow.Activate();
        }
    }
}

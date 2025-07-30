using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;


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
            Frame.Navigate(typeof(MainLayoutPage));
        }

    }
    
    //public partial class ValidationFormWidgetViewModel : ObservableValidator
    //{
    //    private readonly IDialogService DialogService;

    //    public ValidationFormWidgetViewModel(IDialogService dialogService)
    //    {
    //        DialogService = dialogService;
    //    }

    //    public event EventHandler? FormSubmissionCompleted;
    //    public event EventHandler? FormSubmissionFailed;

    //    [ObservableProperty]
    //    [Required]
    //    [MinLength(2)]
    //    [MaxLength(100)]
    //    private string? firstName;

    //    [ObservableProperty]
    //    [Required]

    //    [MinLength(2)]
    //    [MaxLength(100)]
    //    private string? lastName;

    //    [ObservableProperty]
    //    [Required]
    //    [EmailAddress]
    //    private string? email;

    //    [ObservableProperty]
    //    [Required]
    //    [Phone]
    //    private string? phoneNumber;

    //    [RelayCommand]
    //    private void Submit()
    //    {
    //        ValidateAllProperties();

    //        if (HasErrors)
    //        {
    //            FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
    //        }
    //        else
    //        {
    //            FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
    //        }
    //    }

    //    [RelayCommand]
    //    private void ShowErrors()
    //    {
    //        string message = string.Join(Environment.NewLine, GetErrors().Select(e => e.ErrorMessage));

    //        _ = DialogService.ShowMessageDialogAsync("Validation errors", message);
    //    }
    //}
}
namespace ReviewApp.Views;
using ReviewApp.ViewModels;

public partial class LoginPage : ContentPage
{

    public LoginPage(bool logout=false)
    {
        InitializeComponent();
    BindingContext = MyServiceLocator.Services.GetService<LoginViewModel>();
        LogoutNotice.IsVisible=logout;

  }
}

namespace ReviewApp.Views;

public partial class SignupPage : ContentPage
{
	readonly ViewModels.SignupPageViewModel _signupViewModel;
	public SignupPage(ViewModels.SignupPageViewModel SUVM)
	{
		InitializeComponent();

		_signupViewModel = SUVM;
		BindingContext = _signupViewModel;
		Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);

    }
}

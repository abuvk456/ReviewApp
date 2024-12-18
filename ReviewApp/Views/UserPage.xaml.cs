using ReviewApp.ViewModels;

namespace ReviewApp.Views;

public partial class UserPage : ContentPage
{
    private readonly UsersViewModel _usersViewModel;
    public UserPage(UsersViewModel usersViewModel)
	{
		InitializeComponent();
		_usersViewModel = usersViewModel;
		BindingContext = _usersViewModel;
	}

    private void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
    {
        _usersViewModel.ShowingNewMessagePopup = false;
    }
}
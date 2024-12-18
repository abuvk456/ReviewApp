using ReviewApp.ViewModels;

namespace ReviewApp.Views;

public partial class RecommendedUsers : ContentPage
{
	private readonly UsersViewModel _usersViewModel;
	public RecommendedUsers(UsersViewModel usersViewModel)
	{
		InitializeComponent();
		_usersViewModel = usersViewModel;

	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
	   await _usersViewModel.LoadRecomendations();
    }

}
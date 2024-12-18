namespace ReviewApp.Views;
using CommonModel;
using ReviewApp.ViewModels;

public partial class FollowingsPage : ContentPage
{
    private readonly UsersViewModel _usersViewModel;

    public FollowingsPage(UsersViewModel usersViewModel)
    {
        InitializeComponent();
        _usersViewModel = usersViewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _usersViewModel.LoadFollowings();
    }
}
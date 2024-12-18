namespace ReviewApp.Views;
using ViewModels;

public partial class NotificationsPage : ContentPage
{
	readonly NotificationViewModel notificationViewModel;
    public NotificationsPage()
	{

		InitializeComponent();
		notificationViewModel = MyServiceLocator.Services.GetService<NotificationViewModel>();
		BindingContext = notificationViewModel;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();


        await notificationViewModel.LoadNotifications();
    }
}
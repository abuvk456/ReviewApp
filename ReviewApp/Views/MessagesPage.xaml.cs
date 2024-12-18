using ReviewApp.ViewModels;

namespace ReviewApp.Views;

public partial class MessagesPage : ContentPage
{
    MessagesViewModel _messagesViewModel;
    public MessagesPage()
    {
        _messagesViewModel = MyServiceLocator.Services.GetService<MessagesViewModel>();
        BindingContext = _messagesViewModel;

        InitializeComponent();
   
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();


        await _messagesViewModel.LoadData();
    }
}
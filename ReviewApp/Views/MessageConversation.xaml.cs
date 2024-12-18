namespace ReviewApp.Views;
using ViewModels;

public partial class MessageConversation : ContentPage
{
	public MessageConversation()
	{
        BindingContext = MyServiceLocator.Services.GetService<MessagesViewModel>();
        InitializeComponent();
    }
}
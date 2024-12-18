using ReviewApp.Services;
using ReviewApp.ViewModels;
using ReviewApp.Views;

namespace ReviewApp;

public partial class App : Application
{
	public App(IUserService userService)
	{
		InitializeComponent();
		MainPage = MyServiceLocator.Services.GetService<LoginPage>();




    }
}


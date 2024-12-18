using ReviewApp.Views;
using ReviewApp.Services;
using ReviewApp.ViewModels;

namespace ReviewApp;

public partial class AppShell : Shell
{
    private readonly IUserService userService;



    public AppShell()
	{
		InitializeComponent();
        
        var UVM = MyServiceLocator.Services.GetService<UsersViewModel>();
        BindingContext = UVM;
        
        Routing.RegisterRoute(nameof(TopicsPage), typeof(TopicsPage));
        Routing.RegisterRoute(nameof(TopicDetail), typeof(TopicDetail));
        Routing.RegisterRoute(nameof(MessagesPage), typeof(MessagesPage));
        Routing.RegisterRoute(nameof(NotificationsPage), typeof(NotificationsPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(WatchListView), typeof(WatchListView));
        Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
        Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));
        Routing.RegisterRoute("main/"+nameof(RecommendedUsers), typeof(RecommendedUsers));
        Routing.RegisterRoute("main/" + nameof(FollowingsPage), typeof(FollowingsPage));

    }

    private void AppShell_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
       // throw new NotImplementedException();
    }

    private void Shell_Navigating(object sender, ShellNavigatingEventArgs e)
    {
       
    }
    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

      
    }
    

    async void navigate(System.Object sender, System.EventArgs e)
    {
        try
        {
            var menuitem = sender as MenuItem;
            var command = menuitem.CommandParameter as string;

            var addtopicpage = MyServiceLocator.Services.GetService<AddTopic>();
            var TopicsPage = MyServiceLocator.Services.GetService<TopicsPage>();


            switch (command)
            {
                case "logout":
                    Application.Current.MainPage = new Views.LoginPage(true);
                    //await Shell.Current.GoToAsync($"//{nameof(SignupPage)}");
                    break; 
                case "mytopics":
                    var tpvm=MyServiceLocator.Services.GetService<TopicsViewModel>();
                    
                    await Shell.Current.GoToAsync("//TopicsPage");
                    tpvm.LoadTopicsByUser(Globals.CurrentUser);

                    break;

                case "addtopic":
                    await Shell.Current.Navigation.PushAsync(addtopicpage);
                    break;

                case "topics":
                    await Shell.Current.GoToAsync("//TopicsPage");
                    break;
                case "messages":
                    await Shell.Current.Navigation.PushAsync(new Views.MessagesPage());
                    break;
                case "notifications":
                    await Shell.Current.Navigation.PushAsync(new Views.NotificationsPage());
                    break;
                case "watchlist":
                    await Shell.Current.GoToAsync($"WatchListView?UserID={Globals.CurrentUserID}");
                    break;
                case "recomendedusers":
                    await Shell.Current.GoToAsync($"main/{nameof(RecommendedUsers)}");
                    //await Shell.Current.Navigation.PushAsync(new Views.RecommendedUsers());
                    break;
                case "Followers":
                    await Shell.Current.GoToAsync($"main/{nameof(FollowingsPage)}");
                    //await Shell.Current.Navigation.PushAsync(new Views.RecommendedUsers());
                    break;
            }
        }
        catch
        {

        }
		
    }

  
}


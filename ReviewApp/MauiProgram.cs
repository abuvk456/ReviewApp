using System.ComponentModel.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using ReviewApp.Repository;
using ReviewApp.Services;
using ReviewApp.ViewModels;
using ReviewApp.Views;

namespace ReviewApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();


        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("fa_solid.ttf", "FontAwesome");
            })

            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<Services.TMDBHelper>();

        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IMessageService, MessageService>();
        builder.Services.AddTransient<INotificationRepository, NotificationApiClient>();
        builder.Services.AddTransient<IWatchlistRepository, WatchlistApiClient>();
        builder.Services.AddTransient<INotificationService, NotificationService>();
        builder.Services.AddTransient<IWatchlistService, WatchlistService>();
        builder.Services.AddTransient<ITopicRepository, TopicApiClient>();
        builder.Services.AddTransient<ITopicService, TopicService>();
        builder.Services.AddTransient<ICommentRepository, CommentApiClient>();
        builder.Services.AddTransient<ICommentService, CommentService>();
        return builder;
    }



    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<TopicsViewModel>();
        builder.Services.AddSingleton<UsersViewModel>();
        builder.Services.AddSingleton<MessagesViewModel>();
        builder.Services.AddSingleton<NotificationViewModel>();
        builder.Services.AddSingleton<WatchlistViewModel>();
        builder.Services.AddSingleton<SignupPageViewModel>();
        builder.Services.AddSingleton<IMauiInitializeService, MyServiceLocator>();


        return builder;
    }

    private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<Views.AddTopic>();
        builder.Services.AddSingleton<Views.TopicDetail>();
        builder.Services.AddSingleton<Views.TopicsPage>();
        builder.Services.AddSingleton<Views.RecommendedUsers>();
        builder.Services.AddSingleton<Views.FollowingsPage>();
        builder.Services.AddSingleton<SignupPage>();
        builder.Services.AddSingleton<LoginPage>();
        return builder;
    }

}

// IMauiInitializeService will register immediately after MAUI has built its service container, so this class will be ready to go for all of your code
public class MyServiceLocator : Microsoft.Maui.Hosting.IMauiInitializeService
{
    public static IServiceProvider Services { get; private set; }

    public void Initialize(IServiceProvider services)
    {
        Services = services;
    }
}

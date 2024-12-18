using Microsoft.Extensions.DependencyInjection;
using ReviewApp.ViewModels;


namespace ReviewApp.Views;
public interface IMovies
{
    
}
public partial class HomePage : ContentPage
{
    public MoviesViewModel MoviesViewModel { get; } = new MoviesViewModel(new Services.TMDBHelper());
    private readonly MoviesViewModel viewModel;

    public HomePage()
    {
		InitializeComponent();
       // var a= MauiUIApplicationDelegate.Current.Services.GetServices
      //  viewModel = (MoviesViewModel)MauiUIApplicationDelegate.Current.g<MoviesViewModel>();
        BindingContext = MoviesViewModel;
    }
}

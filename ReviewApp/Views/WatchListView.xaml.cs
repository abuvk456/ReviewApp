using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using ReviewApp.Services;
using ReviewApp.ViewModels;
using CommonModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Handlers.Items;

namespace ReviewApp.Views;
[QueryProperty("UserID", "UserID")]
public partial class WatchListView : ContentPage
{
    private WatchlistViewModel _viewModel;
    private int _user;

    public int UserID
    {
        get { return _user; }
        set { _user = value; }
    }

    public WatchListView()
    {
        InitializeComponent();
        var deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        var columns=(int)(deviceWidth / 400);
        collview.ItemsLayout = new GridItemsLayout(columns, ItemsLayoutOrientation.Vertical);
        _viewModel = MyServiceLocator.Services.GetService<WatchlistViewModel>();
        BindingContext = _viewModel;
  

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {

            await _viewModel.LoadWatchlistAsync(UserID);
            return;
            // Set the item source for the collection view
      //      collview.ItemsSource = _viewModel.WatchList;

            // Calculate the number of columns based on device width
            var deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
            var columns = (int)(deviceWidth / 200); // Assuming each column has a fixed width of 200

            // Set the layout of the collection view to GridItemsLayout
            collview.ItemsLayout = new GridItemsLayout(columns, ItemsLayoutOrientation.Vertical);

            // Set the data template for the collection view items
            collview.ItemTemplate = new DataTemplate(() =>
            {
                var VSL = new VerticalStackLayout() { Margin = new Thickness(3),MaximumWidthRequest=200 };
                var image = new Image();
                image.SetBinding(Image.SourceProperty, "Movie.TopicImage");
                image.Aspect = Aspect.AspectFit;

                VSL.Children.Add(image);
                var label = new Label();
                label.TextColor = Colors.Black;
                label.FontSize = 16;
                label.Margin = new Thickness(10);
                label.SetBinding(Label.TextProperty, "Movie.Title");

                VSL.Add(label);


                return VSL;
            });
            sv.Content = collview;
        }
        catch
        {

        }
    }
}

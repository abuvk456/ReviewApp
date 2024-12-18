using ReviewApp.ViewModels;

namespace ReviewApp.Views;

public partial class TopicsPage : ContentPage
{
  private readonly TopicsViewModel topicsViewModel;
	public TopicsPage(TopicsViewModel _topicsViewModel)
	{
		InitializeComponent();
        var deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
        var columns = (int)(deviceWidth / 480);
        SysTopicsCollection.ItemsLayout = new GridItemsLayout(columns, ItemsLayoutOrientation.Vertical);
        topicsViewModel = _topicsViewModel;
        BindingContext = topicsViewModel;

    }

  private void MovieTapGestureRecognizer_Tapped(object sender, EventArgs e)
  {
   // SysTopicsCollection.IsVisible = !SysTopicsCollection.IsVisible;
  }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}

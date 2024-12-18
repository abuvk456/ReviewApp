
using CommonModel;
using ReviewApp.ViewModels;

namespace ReviewApp.Views;

public partial class AddTopic : ContentPage
{
  private readonly TopicsViewModel topicsViewModel;
	public AddTopic(TopicsViewModel _topicsViewModel)
	{
		InitializeComponent();
        topicsViewModel = _topicsViewModel;
        BindingContext = topicsViewModel;
  }

    private void AddTopic_Appearing(object sender, EventArgs e)
    {
        
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        
    }

    void RadioButton_CheckedChanged(System.Object sender, Microsoft.Maui.Controls.CheckedChangedEventArgs e)
    {
    }

  private async void CancelClicked(object sender, EventArgs e)
  {
   // topicsViewModel.SelectedTopic = new TopicVM() { Topic = new Model.Topic() };

 
  }
}

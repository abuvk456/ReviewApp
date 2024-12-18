using CommonModel;
using ReviewApp.Services;
using ReviewApp.ViewModels;

namespace ReviewApp.Views;

public partial class TopicDetail : ContentPage
{
	private readonly TopicsViewModel _topicsViewModel;
    private readonly ICommentService commentService;
    private Topic SelectedTopic { get; set; }
	public TopicDetail(ICommentService _commentService, TopicsViewModel topicsViewModel, int? relatedDataID=0)
	{
        _topicsViewModel = topicsViewModel;
        commentService = _commentService;
        InitializeComponent();
        
        this.BindingContext= _topicsViewModel;
        if (relatedDataID >0m)
        {
            //todo : load topic from database
            
        }
        _topicsViewModel.SelectedTopic?.LoadComments(commentService);
    }
  private async void OnBackButtonClicked(object sender, EventArgs e)
  {
   MyServiceLocator.Services.GetService<TopicsViewModel>().AddTopic();
    await Shell.Current.Navigation.PopAsync();
  }
}

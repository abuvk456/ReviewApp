namespace ReviewApp.Views;

public partial class YoutubeVideoPlayer : ContentPage
{
	
	public YoutubeVideoPlayer(string URL)
	{
		InitializeComponent();
        LoadVideo(URL);

    }
    private async void LoadVideo(string URL)
    {
        var source = new HtmlWebViewSource();

        // Use the YouTube embed code for the video you want to play
        source.Html = @"<iframe width='560' height='315' src='https://www.youtube.com/embed/NEISQLxwVqU' title='YouTube video player' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share' allowfullscreen></iframe>";

        // Set the WebView source to the YouTube embed code
        YoutubePlayerWV.Source = source;
    }
}
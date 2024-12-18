using System;
using ReviewApp.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ReviewApp.Views;
using CommonModel;
using ReviewApp.Services;
using Microsoft.Maui.Layouts;

namespace ReviewApp.ViewModels
{


    public class TopicsViewModel : BaseViewModel
    {
        private readonly ITopicService _topicService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;
        public TopicsViewModel(IUserService userService, ITopicService topicService, ICommentService commentService)
        {
            _topicService = topicService;
            _userService = userService;
            _commentService = commentService;
            Topics = new ObservableCollection<TopicVM>();
            LoadCommentsCommand = new Command(async () => { await SelectedTopic.LoadComments(_commentService); });
            AddTopicCommand = new Command(AddTopic);
            DeleteTopicCommand = new Command<TopicVM>(DeleteTopic);
            SaveTopicCommand = new Command(SaveTopic);
            CancelCommand = new Command(Cancel);
            ViewTopicCommnad = new Command(ViewTopic);
            SearchCommand = new Command(searchTopic);
            LoadUserInfoCommand = new Command(LoadUserInfoAsync);
            ClearFilterCommand = new Command(() => { SelectedUser = null; LoadTopics(); });
            PlayVideoCommand = new Command(PlayVideoFun);
            PostCommentCommand = new Command(HandleComments);
            AddToWatchListCommand = new Command(AddToWatchList);
            
            DeleteCommentCommand = new Command<Comment>((obj) => { obj.IsDeleted = true; HandleComments(obj); });
            EditCommentCommand = new Command<Comment>((obj) => { SelectedComment = obj; obj.IsNew = false; });
            SelectPictureCommand = new Command(PickPicture);
            //LoadPapularMovies();
            SearchQuery = "";
            LoadTopics();
        }
        public ICommand SelectPictureCommand { get; set; }

        private async void PickPicture()
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                SelectedTopic.TopicImage = localFilePath;
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                OnPropertyChanged("SelectedTopic");
                await sourceStream.CopyToAsync(localFileStream);
            }
        }
        private async void AddToWatchList(object obj)
        {
            try
            {
                if (obj.GetType() == typeof(TopicVM))
                {
                    var topic = obj as TopicVM;
                    var WatchListVM = MyServiceLocator.Services.GetService<WatchlistViewModel>();
                    await WatchListVM.AddToWatchlistAsync(Globals.CurrentUserID, topic.Topic.TopicId, DateTime.Now);
                    await App.Current.MainPage.DisplayAlert($"Saved!", $"{topic.Title} Added to Watch List", "ok");
                }
            }
            catch

            {

            }
        }

        private Comment _SelectedComment = new Comment { IsNew = true };

        public Comment SelectedComment
        {
            get { return _SelectedComment; }
            set { _SelectedComment = value; OnPropertyChanged(); }
        }

        private async void HandleComments(object obj)
        {
            if (obj.GetType() == typeof(Comment))
            {
                var cmt = obj as Comment;
                var NewComment = new Comment { CommentedBy = Globals.CurrentUserID, CommentText = cmt.CommentText, TopicId = SelectedTopic.Topic.TopicId };

                if (cmt.IsDeleted)
                {
                    await _commentService.DeleteCommentAsync(cmt.CommentId);
                    SelectedTopic.Comments.Remove(cmt);
                    //delete go here
                }
                else if (cmt.IsNew)
                {
                    //var ID = await ApiClient.AddComments(NewComment);
                    //NewComment.CommentId = (int)ID;
                   await _commentService.AddCommentAsync(NewComment);

                }
                else
                {
                    //updated goes here
                }
                SelectedComment = new Comment { IsNew = true, CommentText = "" };
                //     SelectedTopic.Comments.Add(NewComment);
                //   OnPropertyChanged(nameof(SelectedTopic));
                await SelectedTopic.LoadComments(_commentService);
            }


        }

        private async void PlayVideoFun(object obj)
        {
            if (SelectedTopic != null && !string.IsNullOrEmpty(SelectedTopic.TopicVideo))
            {
                await Launcher.OpenAsync(new Uri(SelectedTopic.TopicVideo));
                //         await Shell.Current.Navigation.PushAsync(new YoutubeVideoPlayer(SelectedTopic.TopicVideo));

            }
        }

        private ObservableCollection<object> _SysTopics;
        public ObservableCollection<object> SysTopics
        {
            get
            {
                return _SysTopics;
            }
            set
            {
                if (_SysTopics != value)
                {
                    _SysTopics = value;
                    OnPropertyChanged();
                }
            }
        }
        public async void LoadPapularMovies()
        {
            _SysTopics = new ObservableCollection<object>();

            ShowLoader = true;
            // TODO: Load movies from a data source and add them to the Movies collection
            await Task.Run(async () =>
            {

                var topmovies = await TMDBHelper.TmdbClient.GetPopularMoviesAsync();
                foreach (var movie in topmovies)
                {
                    _SysTopics.Add(new MovieTMDB { Id = movie.Id, PosterPath = movie.GetFullPosterUrl(), Title = movie.Title, IMDbRating = (int)movie.VoteAverage, Overview = movie.Overview });
                }


                OnPropertyChanged(nameof(SysTopics));

            });
            ShowLoader = false;
        }

        Services.TMDBHelper TMDBHelper = new Services.TMDBHelper();
        private ObservableCollection<TopicVM> _topics;
        public ObservableCollection<TopicVM> Topics
        {
            get { return _topics?.OrderByDescending(t => t.CreatedDate).ToObservableCollection(); }
            set
            {

                _topics = value;
                OnPropertyChanged();

            }
        }


        private TopicVM _selectedTopic = new TopicVM() { IsNew = true };
        public TopicVM SelectedTopic
        {
            get { return _selectedTopic; }
            set
            {

                _selectedTopic = value;
                OnPropertyChanged();

            }
        }

        private bool _allTypeSelected = true;
        private bool _movieTypeSelected;
        private bool _tvTypeSelected;
        private bool _otherTypeSelected;



        private void SetTypeSelected(bool allSelected, bool movieSelected, bool tvSelected, bool otherSelected)
        {
            _allTypeSelected = allSelected;
            _movieTypeSelected = movieSelected;
            _tvTypeSelected = tvSelected;
            _otherTypeSelected = otherSelected;
            OnPropertyChanged(nameof(AllTypeSelected));
            OnPropertyChanged(nameof(MovieTypeSelected));
            OnPropertyChanged(nameof(TVTypeSelected));
            OnPropertyChanged(nameof(OtherTypeSelected));
        }

        public bool AllTypeSelected
        {
            get => _allTypeSelected;
            set
            {
                SelectedTopicType = TopicType.All;
                SetTypeSelected(value, false, false, false);

            }
        }

        public bool MovieTypeSelected
        {
            get => _movieTypeSelected;
            set
            {
                SelectedTopicType = TopicType.Movie;
                SetTypeSelected(false, value, false, false);

            }
        }

        public bool TVTypeSelected
        {
            get => _tvTypeSelected;
            set
            {
                SelectedTopicType = TopicType.TVShow;
                SetTypeSelected(false, false, value, false);


            }
        }

        public bool OtherTypeSelected
        {
            get => _otherTypeSelected;
            set
            {
                SelectedTopicType = TopicType.Other;
                SetTypeSelected(false, false, false, value);

            }
        }


        private TopicType _SelectedTopicType = TopicType.All;
        private TopicType SelectedTopicType
        {
            get { return _SelectedTopicType; }
            set
            {
                if (_SelectedTopicType != value)
                {
                    _SelectedTopicType = value;
                    LoadTopics();
                }
            }
        }


        public ICommand AddTopicCommand { get; set; }
        public ICommand ViewTopicCommnad { get; set; }
        public ICommand DeleteTopicCommand { get; set; }
        public ICommand SaveTopicCommand { get; set; }
        public ICommand AddToWatchListCommand { get; set; }
        public ICommand PostCommentCommand { get; set; }
        public ICommand DeleteCommentCommand { get; set; }
        public ICommand EditCommentCommand { get; set; }

        public ICommand LoadCommentsCommand { get; set; }
        public Command CancelCommand { get; private set; }
        public Command SearchCommand { get; private set; }
        public Command ClearFilterCommand { get; private set; }
        public Command PlayVideoCommand { get; private set; }

        private async void ViewTopic(object obj)
        {
            SelectedTopic = obj as TopicVM;
            SelectedTopic.IsNew = false;
            //SelectedTopic.LoadComments();
            var TopicDetail = MyServiceLocator.Services.GetService<TopicDetail>();
            await Shell.Current.Navigation.PushAsync(TopicDetail);

        }
        public ICommand LoadUserInfoCommand { get; }

        private ApiClient ApiClient = new ApiClient();

        private async void LoadUserInfoAsync(object obj)
        {
            var UVM = MyServiceLocator.Services.GetService<UsersViewModel>();
            await UVM.LoadUserInfoAsync(obj);



        }


        //show all users content
        private User _SelectedUser;

        public User SelectedUser
        {
            get { return _SelectedUser; }
            set
            {
                _SelectedUser = value;
                ShowFilteredByUser = value != null;
                OnPropertyChanged();
            }
        }
        public async void LoadTopics()
        {
            ShowLoader = true;
            int selectedUserId = 0;

            if (SelectedUser != null)
            {
                selectedUserId = SelectedUser.UserId;
            }

            try
            {

                var topicsfromAPI = await _topicService.GetAllTopics(SelectedTopicType, selectedUserId, 0, SearchQuery);
                Topics = new ObservableCollection<TopicVM>();
                foreach (var item in topicsfromAPI)
                {
                    _topics.Add(new TopicVM { Topic = item, IsNew = false });
                    OnPropertyChanged(nameof(Topics));
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
            }

            ShowLoader = false;
        }


        private async void LoadTopics_DP()
        {
            ShowLoader = true;
            int SelectedUserID = 0;
            try
            {
                if (SelectedUser != null)
                {
                    SelectedUserID = SelectedUser.UserId;
                }
                await Task.Run(async () =>
                {
                    var topicsfromAPI = await new ApiClient().GetAllTopics(SelectedTopicType, SelectedUserID, 0, SearchQuery);
                    Topics = new ObservableCollection<TopicVM>();
                    foreach (var item in topicsfromAPI)
                    {
                        _topics.Add(new TopicVM { Topic = item, IsNew = false });
                        OnPropertyChanged(nameof(Topics));
                    }
                });
            }
            catch (Exception ex)
            {

            }

            ShowLoader = false;
        }

        public async void AddTopic()
        {
            var UVM = MyServiceLocator.Services.GetService<UsersViewModel>();


            var topic = new Topic
            {
                Title = "",
                CreatedBy = UVM.CurrentUser.UserId,
                CreatedDate = DateTime.Now,
                IsActive = true,
                TopicImage = "",
                TopicVideo = "",
                UsernName = UVM.CurrentUser.Username,
                Name = UVM.CurrentUser.FullName,
                Type = TopicType.Movie,
                TopicType = TopicType.Movie.ToString(),

            };

            var topicVM = new TopicVM { Topic = topic };
            topicVM.IsNew = true;
            topicVM.TopicType = TopicType.Movie;
            SelectedTopic = topicVM;
            SelectedTopic.IsMovie = true;


            var addtopicpage = MyServiceLocator.Services.GetService<AddTopic>();

            await Shell.Current.Navigation.PushAsync(addtopicpage);

        }

        private async void DeleteTopic(TopicVM obj)
        {
            if (obj != null)
            {
                if (await App.Current.MainPage.DisplayAlert($"Delete Confirmation", $"Are you sure that you want to delete {obj.Title}?", "Yes", "No"))
                {
                    await _topicService.DeleteTopicAsync(obj.Topic.TopicId);
                    _topics.Remove(obj);
                    SelectedTopic = null;
                    OnPropertyChanged(nameof(Topics));
                }
           
                
            }
        }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get
            {
                return _searchQuery;

            }
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }
        private async void searchTopic(object obj)
        {
            if (obj != null)
            {
                if (obj?.ToString() == "Movie")
                    MovieTypeSelected = true;
                else if (obj?.ToString() == "TVShow")
                    TVTypeSelected = false;
                else if (obj?.ToString() == "Other")
                    OtherTypeSelected = true;
                else
                    AllTypeSelected = true;
            }
            LoadTopics();
        }


        private async void SaveTopic()
        {
            if (SelectedTopic != null)
            {
                ShowLoader = true;
                var api = new ApiClient();
                await Task.Run(async () =>
                {
                    if (SelectedTopic.IsNew)
                    {
                        var ID = await _topicService.AddTopicAsync(SelectedTopic.Topic);
                        if (ID.GetType() == typeof(int) && (int)ID > 0)
                        {
                            SelectedTopic.Topic.TopicId = (int)ID;
                            _topics.Add(SelectedTopic);
                        }
                    }
                    else
                    {
                        await _topicService.UpdateTopicAsync(SelectedTopic.Topic);
                    }

                });
                ShowLoader = false;
                OnPropertyChanged(nameof(Topics));
                await App.Current.MainPage.DisplayAlert("Saved!", "Data Saved", "ok");

                await Shell.Current.GoToAsync("//TopicsPage");
            }
        }
        private async void Cancel()
        {

            // Shell.Current.CurrentItem = Shell.Current.CurrentItem.Items.FirstOrDefault();

            await Shell.Current.GoToAsync("//TopicsPage");
        }

        public void LoadTopicsByUser(User user)
        {
            SelectedUser = user;
            SelectedTopic = null;
            LoadTopics();

        }


        private bool _ShowFilteredByUser = false;

        public bool ShowFilteredByUser
        {
            get { return _ShowFilteredByUser; }
            set { _ShowFilteredByUser = value; OnPropertyChanged(); }
        }

    }
}


using ReviewApp.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReviewApp.Views;
using CommonModel;
using ReviewApp.Services;

namespace ReviewApp.ViewModels
{
    public class TopicVM : BaseViewModel
    {


        public TopicVM()
        {
            _topic = new Topic();
            _topic.Type = TopicType.Movie;
            IMDBSearchCommand = new Command(SearchTMDB);
            
            StartTopicCommand = new Command(StartTopicFromSearchResult);
            ShowSearchButton = true;
            UserName = Globals.CurrentUser.FullName;
            _topic.CreatedBy = Globals.CurrentUser.UserId;
        }
       
        public bool IsDeleteAllowed
        {
            get { return Globals.CurrentUserID == Topic?.CreatedBy; }
            
        }

        public bool IsNew = true;
        private ObservableCollection<Comment> _Comments = new ObservableCollection<Comment>();
        public async Task LoadComments(ICommentService _commentService)
        {
            _Comments.Clear();
            try
            {
                
                
               
                var CommentsfromAPI = await _commentService.GetCommentsAsync(topicId : Topic.TopicId);
                
                foreach(var cmt in CommentsfromAPI)
                {
                    cmt.IsEditAllowed = cmt.CommentedBy == Globals.CurrentUserID;
                    //Comments.Add(cmt);
                }
                Comments = new ObservableCollection<Comment>(CommentsfromAPI);
                
            }
            catch
            {

            }
            finally
            {
                
            }
        }
        public ObservableCollection<Comment> Comments
        {
            get
            {
                return _Comments.OrderByDescending(c => c.CommentDateTime)
                               .ToObservableCollection();
            }
            set
            {
                _Comments = value;
                OnPropertyChanged();
            }
        }

        public int IMDBRating
        {
            get
            {
                return Topic.IMDBRating;
            }
            set
            {
                Topic.IMDBRating = value;
                OnPropertyChanged();
            }
        }

        private async void StartTopicFromSearchResult(object obj)
        {
            try
            {
                IsNew = true;

                if (obj.GetType() == typeof(MovieTMDB))
                {
                    var searchtopicresult = (MovieTMDB)obj;
                    TopicType = TopicType.Movie;
                    Description = searchtopicresult.Overview;
                    Title = searchtopicresult.Title;
                    TopicImage = searchtopicresult.GetFullPosterUrl();
                    TMDBID = searchtopicresult.IMDbId;
                    IDMBID = searchtopicresult.Id.ToString();
                    ShowSearchResult = false;
                    IMDBRating = (int)searchtopicresult.IMDbRating;
                    TopicVideo = await TMDBHelper.TmdbClient.GetMovieTrailerUrlAsync(IDMBID, TopicType == TopicType.TVShow);
                }
                else
                {
                    var searchtopicresult = (TmdbTVShow)obj;
                    Description = searchtopicresult.Overview;
                    Title = searchtopicresult.Name;
                    TopicImage = searchtopicresult.GetFullPosterUrl();
                    TMDBID = searchtopicresult.Id.ToString();
                    IDMBID = searchtopicresult.Id.ToString();
                    ShowSearchResult = false;
                    // IMDBRating = (int)searchtopicresult.IMDbRating;
                    //  TopicVideo = await TMDBHelper.TmdbClient.GetMovieTrailerUrlAsync(IDMBID, TopicType == TopicType.TVShow);
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        public TopicVM(Topic __topic)
        {
            _topic = __topic;
        }
        private Topic _topic;
        public Topic Topic
        {
            get { return _topic; }
            set
            {
                if (_topic != value)
                {
                    _topic = value;
                    OnPropertyChanged();
                    //  SearchTitleCommand.Execute(null);
                }
            }
        }

        bool isMovie = true;
        bool isTVShow = true;
        bool isOther = true;

        public bool IsMovie
        {
            get
            {
                return isMovie;
            }
            set
            {
                isMovie = value;
                if (value)
                {
                    IsTvShow = IsOther = false;
                    TopicType = TopicType.Movie;
                }
                OnPropertyChanged();
            }
        }


        public string UserName
        {
            get { return Topic.UsernName; }
            set { Topic.UsernName = value; OnPropertyChanged(); }
        }
        //User Full Name
        public string Name
        {
            get { return Topic.Name; }
            set { Topic.Name = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get { return Topic.Email; }
            set { Topic.Email = value; OnPropertyChanged(); }
        }


        public bool IsTvShow
        {
            get
            {
                return isTVShow;
            }
            set
            {
                isTVShow = value;
                if (value)
                {
                    IsMovie = IsOther = false;
                    TopicType = TopicType.TVShow;
                }
                OnPropertyChanged();
            }
        }
        public bool IsOther
        {
            get
            {
                return isOther;
            }
            set
            {
                isOther = value;
                if (value)
                {
                    IsTvShow = IsMovie = false;
                    TopicType = TopicType.Other;
                }

                OnPropertyChanged();
            }
        }
        public TopicType TopicType
        {
            get { return Topic.Type; }
            set
            {
                Topic.Type = value;

                OnPropertyChanged();
                if (Topic.Type == TopicType.Movie || Topic.Type == TopicType.TVShow)
                    ShowSearchButton = true;
                else
                {
                    ShowSearchButton = false;

                    ShowSearchResult = false;
                }

            }
        }

        public string Description
        {
            get { return Topic.Description; }
            set
            {
                if (Topic.Description != value)
                {
                    Topic.Description = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get { return Topic.Title; }
            set
            {
                if (Topic.Title != value)
                {
                    Topic.Title = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TopicImage
        {
            get { return Topic.TopicImage; }
            set
            {
                if (Topic.TopicImage != value)
                {
                    Topic.TopicImage = value;
                    OnPropertyChanged();
                }
            }
        }


        public string TopicVideo
        {
            get { return Topic.TopicVideo; }
            set
            {
                if (Topic.TopicVideo != value)
                {
                    Topic.TopicVideo = value;

                }
                OnPropertyChanged();
            }
        }

        public int CreatedBy
        {
            get { return Topic.CreatedBy; }
            set
            {
                if (Topic.CreatedBy != value)
                {
                    Topic.CreatedBy = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand IMDBSearchCommand { get; set; }
        public ICommand StartTopicCommand { get; set; }
        public bool _ShowSearchResult = false;
        public bool ShowSearchResult
        {
            get { return _ShowSearchResult; }
            set { _ShowSearchResult = value; OnPropertyChanged(); }
        }
        public DateTime CreatedDate
        {
            get { return Topic.CreatedDate; }
            set
            {
                if (Topic.CreatedDate != value)
                {
                    Topic.CreatedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get { return Topic.IsActive; }
            set
            {
                if (Topic.IsActive != value)
                {
                    Topic.IsActive = value;
                    OnPropertyChanged();
                }
            }
        }


        public string IDMBID
        {
            get { return Topic.IDMBID; }
            set
            {
                if (Topic.IDMBID != value)
                {
                    Topic.IDMBID = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TMDBID
        {
            get { return Topic.TMDBID; }
            set
            {
                if (Topic.TMDBID != value)
                {
                    Topic.TMDBID = value;
                    OnPropertyChanged();
                }
            }

        }
        public bool _ShowSearchButton = false;
        public bool ShowSearchButton
        {
            get { return _ShowSearchButton; }
            set { _ShowSearchButton = value; OnPropertyChanged(); }
        }

        Services.TMDBHelper TMDBHelper = new Services.TMDBHelper();
        private ObservableCollection<object> _searchResults;
        public ObservableCollection<object> SearchResults
        {
            get
            {
                return _searchResults;
            }
            set
            {
                if (_searchResults != value)
                {
                    _searchResults = value;
                    OnPropertyChanged();
                }
            }
        }
        private async void SearchTMDB(object obj)
        {

            IsSearching = true;
            _searchResults = new ObservableCollection<object>();

            ShowSearchResult = true;
            await Task.Run(() =>
            {
                try
                {
                    if (TopicType == TopicType.Movie)
                    {
                        var searchResults = TMDBHelper.TmdbClient.SearchMoviesByNameAsync(Title).Result;
                        foreach (var item in searchResults)
                        {
                            _searchResults.Add(item);
                        }
                    }

                    else if (TopicType == TopicType.TVShow)
                    {
                        var searchResults = TMDBHelper.TmdbClient.SearchTVShowsByNameAsync(Title).Result;
                        foreach (var item in searchResults)
                        {

                            _searchResults.Add(item);
                        }
                    }
                }

                catch (Exception ex)
                {


                }
                isSearching = false;
                OnPropertyChanged(nameof(SearchResults));
                OnPropertyChanged(nameof(IsSearching));

            });


        }
        bool isSearching = false;
        public bool IsSearching
        {
            get
            {
                return isSearching;
            }
            set
            {
                isSearching = value;
                OnPropertyChanged();
            }
        }

    }
}


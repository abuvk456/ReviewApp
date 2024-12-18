using ReviewApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ReviewApp.Model;


namespace ReviewApp.ViewModels
{

    public class UsersViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        private readonly WatchlistViewModel _watchListViewModel;

        private ObservableCollection<User> _RecomendatedUsers;


        public ObservableCollection<User> RecomendatedUsers
        {
            get { return _RecomendatedUsers; }
            set { _RecomendatedUsers = value; OnPropertyChanged(); }
        }
        private ObservableCollection<User> _FollowedByUsers;


        public ObservableCollection<User> FollowedByUsers
        {
            get { return _FollowedByUsers; }
            set { _FollowedByUsers = value; OnPropertyChanged(); }
        }

     
   
        private bool _IsFollowed;

        public bool IsFollowed
        {
            get { return _IsFollowed; }
            set { _IsFollowed = value; OnPropertyChanged(); }
        }

        private bool _IsNoFollowed=true;

        public bool IsNoFollowed
        {
            get { return _IsNoFollowed; }
            set { _IsNoFollowed = value; OnPropertyChanged(); }
        }




        public ICommand LoadWatchListCommand { get; }
        public ICommand LoadTopicsCommand { get; }
        public ICommand VoteUpCommand { get; }
        public ICommand VoteDownCommand { get; }
        public ICommand FollowUserCommand { get; }
        public ICommand UnFollowCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand ViewProfileCommand { get; }
        public ICommand ShowMessagePopupCommand { get; }
        public ICommand ViewWatchListCommand { get; }
        public ICommand GoBackCommand { get; }
        public async Task LoadRecomendations()
        {
            var RUs= await _userService.LoadRecomendations(CurrentUser.UserId);
            RecomendatedUsers = RUs.ToObservableCollection();
        }


        public async Task LoadFollowings()
        {
            if (SelectedUser != null)
            {
                var RUs = await _userService.GetUserFollowings(SelectedUser.UserId);
                FollowedByUsers = RUs.ToObservableCollection();


                IsFollowed = FollowedByUsers.Any(u => u.UserId == CurrentUser.UserId);
                IsNoFollowed = !IsFollowed;
            }
            else
            {
                var RUs = await _userService.GetUserFollowings(CurrentUser.UserId);
                FollowedByUsers = RUs.ToObservableCollection();
            }
        }

        private bool _VoteAllowed;

        public bool VoteAllowed
        {
            get { return _VoteAllowed; }
            set { _VoteAllowed = value; OnPropertyChanged(); }
        }


        public UsersViewModel(
            IUserService userService, WatchlistViewModel watchlistViewModel
          )
        {
            _userService = userService;
            _watchListViewModel = watchlistViewModel;
            LoadTopicsCommand = new Command(async () => await LoadTopicsAsync());
            ViewWatchListCommand = new Command(async () => {


                await Shell.Current.GoToAsync($"WatchListView?UserID={SelectedUser.UserId}");
            });
            VoteUpCommand = new Command(async () => await VoteAsync(true));
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync("//TopicsPage"));
            VoteDownCommand = new Command(async () => await VoteAsync(false));
            ViewProfileCommand = new Command<object>(async (id) => await LoadUserInfoAsync(id));
            FollowUserCommand = new Command<int>(async (UserID) => await FollowUser(UserID));
            UnFollowCommand = new Command<int>(async (UserID) => await FollowUser(UserID*-1));
            SendMessageCommand = new Command<object>(async (obj) =>
                    {
                        if(obj?.ToString()=="cancel")
                            {
                            ShowingNewMessagePopup = false;
                            }
                        else
                        await SendMessage();
                    }
                );

            ShowMessagePopupCommand = new Command(async () => ShowingNewMessagePopup = true);
        }

        private async Task FollowUser(int userID)
        {
            try
            {
               if( await _userService.FollowUserAsync(CurrentUser.UserId, userID))
                {
                    if(userID>0)
                    await App.Current.MainPage.DisplayAlert("Saved!", $"You are now following : {SelectedUser.FullName}", "ok");
                    else
                        await App.Current.MainPage.DisplayAlert("Saved!", $"You have unfollowed : {SelectedUser.FullName}", "ok");
                }
               else
                {
                    await App.Current.MainPage.DisplayAlert("info!", $"You are already following {SelectedUser.FullName}", "ok");
                }
            }
            catch
            {

            }
            await LoadFollowings();
            if (SelectedUser != null)
            {
                if (userID < 0) userID = userID * -1;
                SelectedUser.UserProfileInfo = await _userService.GetUserProfileInfo((int)userID);
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        private bool _ShowingNewMessagePopup;

        public bool ShowingNewMessagePopup
        {
            get { return _ShowingNewMessagePopup; }
            set { _ShowingNewMessagePopup = value; OnPropertyChanged(); }
        }

        private User _CurrentUser { get; set; }
        private User _SelectedUser { get; set; }
        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { SetProperty(ref _currentUser, value); }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set { SetProperty(ref _selectedUser, value); }
        }
        private bool _ShowUserLoader = false;
        public bool ShowUserLoader
        {
            get => _ShowUserLoader;

            set { _ShowUserLoader = value; OnPropertyChanged(); }
        }
        private bool _ShowTopicsLoader = false;
        public bool ShowTopicsLoader
        {
            get => _ShowTopicsLoader;

            set { _ShowTopicsLoader = value; OnPropertyChanged(); }
        }
        private bool _ShowWatchListLoader = false;
        public bool ShowWatchListLoader
        {
            get => _ShowWatchListLoader;

            set { _ShowWatchListLoader = value; OnPropertyChanged(); }
        }
        public async Task LoadUserInfoAsync(object UserID)
        {
            if(UserID == null) 
                return;
            ShowUserLoader = true;
            try
            {
                await Shell.Current.Navigation.PushAsync(new Views.UserPage(this));
                //await Shell.Current.GoToAsync($"{nameof(Views.UserPage)}");

                var user = await _userService.GetUserByIdAsync((int)UserID);

                SelectedUser = user;
                if (user != null)
                {
                    user.UserProfileInfo = await _userService.GetUserProfileInfo((int)UserID);
                    OnPropertyChanged(nameof(SelectedUser));
                    await LoadFollowings();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowUserLoader = false;
                    VoteAllowed = (int)UserID != Globals.CurrentUserID;
            }

        }

        //  private async Task LoadWatchListAsync()
        //  {
        //    try
        //    {
        //      var watchList = await _watchListService.GetWatchListAsync(UserViewModel.UserId);
        //      WatchListViewModel = new WatchListViewModel(watchList);
        //    }
        //    catch (Exception ex)
        //    {
        //      await HandleExceptionAsync(ex);
        //    }
        //  }

        private async Task LoadTopicsAsync()
        {
            try
            {
                var UVM = MyServiceLocator.Services.GetService<TopicsViewModel>();
                UVM.LoadTopicsByUser(SelectedUser);
                await Shell.Current.GoToAsync("//TopicsPage");
            }
            catch (Exception ex)
            {

            }
        }

        private async Task VoteAsync(bool isUpvote)
        {
            try
            {
                if (SelectedUser == null)
                    return;
                if (SelectedUser.UserId == CurrentUser.UserId)
                    return;
                if (!await _userService.VoteUser(new Vote { VotedBy = CurrentUser.UserId, VotedFor = SelectedUser.UserId, IsUpVote = isUpvote }))
                    return;
                if (SelectedUser != null)
                {
                    SelectedUser.UserProfileInfo = await _userService.GetUserProfileInfo(SelectedUser.UserId);
                    OnPropertyChanged("SelectedUser.UserProfileInfo");
                    OnPropertyChanged(nameof(SelectedUser));
                    if (isUpvote)
                    {
                        SelectedUser.UpvoteCount++;
                    }
                    else
                    {
                        SelectedUser.DownvoteCount++;
                    }
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        private Message _newMessage = new Message();

        public Message NewMessage
        {
            get { return _newMessage; }
            set { _newMessage = value; OnPropertyChanged(); }
        }


        private async Task SendMessage()
        {
            try
            {
                var MVM = MyServiceLocator.Services.GetService<MessagesViewModel>();

                NewMessage.SentTo = SelectedUser.UserId;
                NewMessage.SentBy = CurrentUser.UserId;
                NewMessage.SentByName = CurrentUser.FullName;
                NewMessage.SentToName = SelectedUser.FullName;
                NewMessage.SentToName = SelectedUser.FullName;
                NewMessage.SentDatetime = DateTime.Now;
                NewMessage.MessageText = NewMessage.MessageText;

                if (await MVM.SendDirectMessage(NewMessage))
                {
                    await App.Current.MainPage.DisplayAlert("Saved!", "Message Sent", "ok");
                }
                NewMessage = new Message();
            }

            catch
            {

            }
            ShowingNewMessagePopup = false;
        }
    }
}

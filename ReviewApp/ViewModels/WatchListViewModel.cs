using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonModel;
using ReviewApp.Services;
using ReviewApp.Views;

namespace ReviewApp.ViewModels
{
    public class WatchlistViewModel : BaseViewModel
    {
        private readonly IWatchlistService _watchlistService;
        public int Columns { get
            {
                var deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
                return (int)(deviceWidth / 200); 

            }
        }

        public WatchlistViewModel(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;


            UpdateWatchlistCommand = new Command<WatchlistEntry>(UpdateWatchlistAsync);
            DeleteFromWatchlistCommand = new Command<WatchlistEntry>(DeleteFromWatchlistAsync);
            RefreshWatchList = new Command(async () => await LoadWatchlistAsync());
            PerformActionCommand = new Command<string>(async (action) => await PerformAction(action));
        }

        private async Task PerformAction(string action)
        {
            switch (action)
            {
                case "back":

                    await Shell.Current.GoToAsync($"//TopicsPage");
                    break;
            }
        }

        public ObservableCollection<WatchlistEntry> _Watchlist=new ObservableCollection<WatchlistEntry>();
        public ObservableCollection<WatchlistEntry> WatchList

        {
            get
            {
                return _Watchlist;
            }
            set
            {
                _Watchlist = value; OnPropertyChanged();
            }
        }
      
        public ICommand UpdateWatchlistCommand { get; }
        public ICommand DeleteFromWatchlistCommand { get; }
        public ICommand RefreshWatchList { get; }
        public ICommand PerformActionCommand { get; }
        public async Task LoadWatchlistAsync(int userId=-1)
        {
            if (userId <= 0 || userId == Globals.CurrentUserID)
            {
                userId = Globals.CurrentUserID;
                IsDeleteAllowed = true;
            }
            else
                IsDeleteAllowed = false;
            
          
            WatchList = new ObservableCollection<WatchlistEntry>(await _watchlistService.GetWatchlistByUserIdAsync(userId));
          
        }
        private bool _IsDeleteAllowed;

        public bool IsDeleteAllowed
        {
            get { return _IsDeleteAllowed; }
            set { _IsDeleteAllowed = value; OnPropertyChanged(); }
        }



        public async Task AddToWatchlistAsync(int UserID, int TopicID, DateTime WatchedDateTime)
        {
            await _watchlistService.AddToWatchlistAsync(UserID, TopicID, WatchedDateTime);
            await LoadWatchlistAsync(UserID);
        }

        private async void UpdateWatchlistAsync(WatchlistEntry watchlistEntry)
        {
            await _watchlistService.UpdateWatchlistAsync(watchlistEntry.UserID, watchlistEntry.TopicID, watchlistEntry.WatchedDateTime);
            await LoadWatchlistAsync(watchlistEntry.UserID);
        }

        private async void DeleteFromWatchlistAsync(WatchlistEntry watchlistEntry)
        {
            ShowLoader = true;
            await _watchlistService.DeleteFromWatchlistAsync(watchlistEntry.UserID, watchlistEntry.TopicID);
            
            ShowLoader = false;
        }

    }
}

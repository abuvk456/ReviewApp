using System;
using System.Windows.Input;

namespace ReviewApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
   //     public MoviesViewModel MoviesViewModel { get; } = new MoviesViewModel(new Services.TMDBHelper());
        //public TVShowsViewModel TVShowsViewModel { get; } = new TVShowsViewModel();
        private bool isMenuOpen = true;

        public ICommand ViewDetailCommand { get; }

       
        public MainPageViewModel()
        {
            
        }
    }

}


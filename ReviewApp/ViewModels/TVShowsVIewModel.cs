using System;
using ReviewApp.Model;
using System.Collections.ObjectModel;

namespace ReviewApp.ViewModels
{
	public class TVShowsViewModel
    {
        public ObservableCollection<TVShow> TVShows { get; }

        public TVShowsViewModel()
        {
            TVShows = new ObservableCollection<TVShow>();
            LoadTVShows();
        }

        private void LoadTVShows()
        {
            // TODO: Load TV shows from a data source and add them to the TVShows collection
        }
    }
}


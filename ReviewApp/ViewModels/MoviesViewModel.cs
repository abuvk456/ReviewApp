using System;
using System.Collections.ObjectModel;
using ReviewApp.Model;

namespace ReviewApp.ViewModels
{
    
	public class MoviesViewModel:BaseViewModel
	{
        public readonly Services.TMDBHelper tMDBHelper;
        private ObservableCollection<Movie> movies;
        public ObservableCollection<Movie> Movies {
            get
            {
                return movies;
            }
            set { movies = value;OnPropertyChanged(); }
        }

        public MoviesViewModel(Services.TMDBHelper _tMDBHelper)
        {
            tMDBHelper = _tMDBHelper;
            Movies = new ObservableCollection<Movie>();
            LoadMovies();
        }
        private bool isLoading = false;
        public bool IsLoading { get { return isLoading; } set {
                isLoading = value;
                OnPropertyChanged();

            } }



        private async void LoadMovies()
        {
            IsLoading = true;
            // TODO: Load movies from a data source and add them to the Movies collection
            await Task.Run(async () => {

                var topmovies = await tMDBHelper.TmdbClient.GetPopularMoviesAsync();
                foreach(var movie in topmovies)
                {
                    movies.Add(new Movie { ImageUrl =movie.GetFullPosterUrl(), Title = movie.Title,Rating=(int)movie.VoteAverage, Genre=movie.Overview });
                }
                

                OnPropertyChanged("Movies");
                IsLoading = false;
            });
           
            
        }
    }
}


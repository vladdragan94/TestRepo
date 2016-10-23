using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.IRepositories
{
    public interface IMovies
    {
        List<Entities.Movie> GetMovies(string searchTerm = null, string genre = null, int? yearStart = null, int? yearEnd = null, string awards = null, string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultMoviesPageSize);

        int GetFilteredMoviesCount(string searchTerm = null, string genre = null, int? yearStart = null, int? yearEnd = null, string awards = null);

        Entities.Movie GetMovieDetails(string movieId, string userId = null);

        void AddMovie(Entities.Movie imdbMovie);

        decimal? RateMovie(string movieId, string userId, int newRating);

        decimal? UnrateMovie(string movieId, string userId);

        void AddGenres(List<string> genres);

        List<Entities.Movie> GetWatchlistMovies(string userId);

        void AddMovieToWatchlist(string movieId, string userId);

        void RemoveMovieFromWatchlist(string movieId, string userId);

        List<Entities.Movie> GetLatestMovies(string userId = null);

        List<Entities.Movie> GetUserRatings(string userId);

        void UpdateLatestMovies(List<Entities.Movie> movies);

        DateTime GetLatestMoviesDate();

        List<string> GetMoviesGenres();
    }
}

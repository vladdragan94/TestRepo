using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.Entities;
using MovieDictionary.IRepositories;


namespace MovieDictionary.DAL
{
    public class MoviesRepository : IMovies
    {
        public List<Entities.Movie> GetMovies(string searchTerm = null, string genre = null, int? yearStart = null, int? yearEnd = null, string awards = null, string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultMoviesPageSize)
        {
            using (var dataContext = new MoviesEntities())
            {
                orderType = orderType ?? Constants.OrderTypes.Votes;

                Func<Movies, object> orderBy = null;

                if (orderType == Constants.OrderTypes.Votes)
                    orderBy = item => item.UsersRatings.Count;
                else if (orderType == Constants.OrderTypes.Rating)
                    orderBy = item => item.Rating;
                else if (orderType == Constants.OrderTypes.Date)
                    orderBy = item => item.Released;

                return dataContext.Movies
                .Where(item =>
                (!string.IsNullOrEmpty(searchTerm) ? item.Title.Contains(searchTerm) : true) &&
                (!string.IsNullOrEmpty(genre) ? item.Genre.Contains(genre) : true) &&
                (yearStart.HasValue ? item.Year >= yearStart : true) &&
                (yearEnd.HasValue ? item.Year <= yearEnd : true) &&
                (!string.IsNullOrEmpty(awards) ? item.Awards.Contains(awards) : true))
                .OrderByDescending(orderBy)
                .ThenByDescending(item => item.Released)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .Select(item => new Movie()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Year = item.Year,
                    Rating = item.Rating,
                    Votes = item.UsersRatings.Count,
                    Poster = item.Poster,
                    CurrentUserRating = userId != null ? (item.UsersRatings.Any(el => el.UserId == userId) ? (int?)item.UsersRatings.FirstOrDefault(el => el.UserId == userId).Rating : null) : null,
                    IsInWatchlist = userId != null ? item.UsersWatchlists.Any(el => el.UserId == userId) : false
                })
                .ToList();
            }
        }

        public int GetFilteredMoviesCount(string searchTerm = null, string genre = null, int? yearStart = null, int? yearEnd = null, string awards = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.Movies
                .Where(item =>
                (!string.IsNullOrEmpty(searchTerm) ? item.Title.Contains(searchTerm) : true) &&
                (!string.IsNullOrEmpty(genre) ? item.Genre.Contains(genre) : true) &&
                (yearStart.HasValue ? item.Year >= yearStart : true) &&
                (yearEnd.HasValue ? item.Year <= yearEnd : true) &&
                (!string.IsNullOrEmpty(awards) ? item.Awards.Contains(awards) : true))
                .Count();
            }
        }

        public Entities.Movie GetMovieDetails(string movieId, string userId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                var movie = dataContext.Movies.Find(movieId);

                if (movie == null)
                    return null;

                return new Entities.Movie()
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Year = movie.Year,
                    Runtime = movie.Runtime,
                    Released = movie.Released,
                    Genre = movie.Genre,
                    Rating = movie.Rating,
                    Votes = movie.UsersRatings.Count,
                    Poster = movie.Poster,
                    Plot = movie.Plot,
                    Director = movie.Director,
                    Writer = movie.Writer,
                    Actors = movie.Actors,
                    Awards = movie.Awards,
                    CurrentUserRating = userId != null ? (movie.UsersRatings.Any(el => el.UserId == userId) ? (int?)movie.UsersRatings.FirstOrDefault(el => el.UserId == userId).Rating : null) : null,
                    ReviewedByCurrentUser = userId != null ? movie.Reviews.Any(item => item.UserId == userId) : false,
                    NumberOfReviews = movie.Reviews.Count,
                    IsInWatchlist = userId != null ? movie.UsersWatchlists.Any(el => el.UserId == userId) : false,
                    InCinema = movie.LatestMovies.Any() && movie.Released <= DateTime.Now,
                    ComingSoon = movie.LatestMovies.Any() && movie.Released > DateTime.Now,
                    Trailer = movie.Trailer
                };
            }
        }

        public void AddMovie(Entities.Movie imdbMovie)
        {
            using (var dataContext = new MoviesEntities())
            {
                var movie = dataContext.Movies.Find(imdbMovie.Id);

                if (movie == null)
                {
                    movie = new Movies();
                    dataContext.Movies.Add(movie);
                }

                movie.Id = imdbMovie.Id;
                movie.Title = imdbMovie.Title;
                movie.Year = imdbMovie.Year;
                movie.Runtime = imdbMovie.Runtime;
                movie.Released = imdbMovie.Released;
                movie.Genre = imdbMovie.Genre;
                movie.Poster = imdbMovie.Poster;
                movie.Plot = imdbMovie.Plot;
                movie.Director = imdbMovie.Director;
                movie.Writer = imdbMovie.Writer;
                movie.Actors = imdbMovie.Actors;
                movie.Awards = imdbMovie.Awards;
                movie.Trailer = imdbMovie.Trailer;

                dataContext.SaveChanges();
            }
        }

        public decimal? RateMovie(string movieId, string userId, int newRating)
        {
            using (var dataContext = new MoviesEntities())
            {
                var userRating = dataContext.UsersRatings.FirstOrDefault(item => item.MovieId == movieId && item.UserId == userId);
                var movie = dataContext.Movies.Find(movieId);

                if (userRating == null)
                {
                    userRating = new UsersRatings();
                    userRating.MovieId = movieId;
                    userRating.UserId = userId;
                    userRating.DateAdded = DateTime.Now;
                    dataContext.UsersRatings.Add(userRating);
                }

                userRating.Rating = newRating;

                dataContext.SaveChanges();

                movie.Rating = (decimal)movie.UsersRatings.Average(item => item.Rating);

                dataContext.SaveChanges();

                return movie.Rating;
            }
        }

        public decimal? UnrateMovie(string movieId, string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var movie = dataContext.Movies.Find(movieId);
                var userRating = dataContext.UsersRatings.FirstOrDefault(item => item.MovieId == movieId && item.UserId == userId);

                if (userRating == null)
                    return movie.Rating;

                dataContext.UsersRatings.Remove(userRating);
                dataContext.SaveChanges();

                movie.Rating = movie.UsersRatings.Any() ? (decimal?)movie.UsersRatings.Average(item => item.Rating) : null;

                dataContext.SaveChanges();

                return movie.Rating;
            }
        }

        public void AddGenres(List<string> genres)
        {
            using (var dataContext = new MoviesEntities())
            {
                foreach (var movieGenre in genres)
                {
                    var genre = dataContext.Genres.FirstOrDefault(item => item.Name == movieGenre);

                    if (genre != null)
                        continue;

                    genre = new Genres();
                    genre.Name = movieGenre;
                    dataContext.Genres.Add(genre);
                }

                dataContext.SaveChanges();
            }
        }

        public void AddMovieToWatchlist(string movieId, string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var movieToWatch = dataContext.UsersWatchlists.FirstOrDefault(item => item.MovieId == movieId && item.UserId == userId);

                if (movieToWatch != null)
                    return;

                movieToWatch = new UsersWatchlists()
                {
                    MovieId = movieId,
                    UserId = userId,
                    DateAdded = DateTime.Now
                };

                dataContext.UsersWatchlists.Add(movieToWatch);
                dataContext.SaveChanges();
            }
        }

        public void RemoveMovieFromWatchlist(string movieId, string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                var movieToWatch = dataContext.UsersWatchlists.FirstOrDefault(item => item.MovieId == movieId && item.UserId == userId);

                if (movieToWatch == null)
                    return;

                dataContext.UsersWatchlists.Remove(movieToWatch);
                dataContext.SaveChanges();
            }
        }

        public List<Entities.Movie> GetWatchlistMovies(string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersWatchlists.Where(item => item.UserId == userId).OrderByDescending(item => item.DateAdded).Select(item => item.Movies).Select(item => new Entities.Movie()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Year = item.Year,
                    Genre = item.Genre,
                    Runtime = item.Runtime,
                    Director = item.Director,
                    Actors = item.Actors,
                    Rating = item.Rating,
                    Poster = item.Poster,
                    CurrentUserRating = item.UsersRatings.Any(el => el.UserId == userId) ? (int?)item.UsersRatings.FirstOrDefault(el => el.UserId == userId).Rating : null,
                    IsInWatchlist = true
                }).ToList();
            }
        }

        public List<Entities.Movie> GetLatestMovies(string userId = null)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.LatestMovies.Select(item => item.Movies).OrderByDescending(item => item.Released).Select(item => new Entities.Movie()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Year = item.Year,
                    Director = item.Director,
                    Actors = item.Actors,
                    Runtime = item.Runtime,
                    Genre = item.Genre,
                    Rating = item.Rating,
                    Votes = item.UsersRatings.Count,
                    Poster = item.Poster,
                    CurrentUserRating = userId != null ? (item.UsersRatings.Any(el => el.UserId == userId) ? (int?)item.UsersRatings.FirstOrDefault(el => el.UserId == userId).Rating : null) : null,
                    IsInWatchlist = userId != null ? item.UsersWatchlists.Any(el => el.UserId == userId) : false
                }).ToList();
            }
        }

        public List<Entities.Movie> GetUserRatings(string userId)
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.UsersRatings.Where(item => item.UserId == userId).OrderByDescending(item => item.DateAdded).Select(item => item.Movies).Select(item => new Entities.Movie()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Year = item.Year,
                    Genre = item.Genre,
                    Runtime = item.Runtime,
                    Director = item.Director,
                    Actors = item.Actors,
                    Rating = item.Rating,
                    Poster = item.Poster,
                    CurrentUserRating = item.UsersRatings.FirstOrDefault(el => el.UserId == userId).Rating,
                    IsInWatchlist = item.UsersWatchlists.Any(el => el.UserId == userId)
                }).ToList();
            }
        }

        public void UpdateLatestMovies(List<Entities.Movie> movies)
        {
            using (var dataContext = new MoviesEntities())
            {
                dataContext.LatestMovies.RemoveRange(dataContext.LatestMovies);
                dataContext.SaveChanges();

                foreach (var movie in movies)
                {
                    if (dataContext.LatestMovies.FirstOrDefault(item => item.MovieId == movie.Id) != null)
                        continue;

                    var movieToAdd = new LatestMovies()
                    {
                        MovieId = movie.Id,
                        DateAdded = DateTime.Now
                    };

                    dataContext.LatestMovies.Add(movieToAdd);
                }

                var latestMovies = dataContext.ApplicationConfigurations.FirstOrDefault(item => item.Name == Entities.Constants.ApplicationConfigurations.LatestMoviesDate);

                if (latestMovies == null)
                {
                    latestMovies = new ApplicationConfigurations()
                    {
                        Name = Entities.Constants.ApplicationConfigurations.LatestMoviesDate
                    };
                }

                latestMovies.Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);

                dataContext.SaveChanges();
            }
        }

        public DateTime GetLatestMoviesDate()
        {
            using (var dataContext = new MoviesEntities())
            {
                var latestMoviesDate = dataContext.ApplicationConfigurations.FirstOrDefault(item => item.Name == Entities.Constants.ApplicationConfigurations.LatestMoviesDate);

                if (latestMoviesDate == null)
                {
                    latestMoviesDate = new ApplicationConfigurations()
                    {
                        Name = Entities.Constants.ApplicationConfigurations.LatestMoviesDate,
                        Value = DateTime.Now.AddDays((-1) * Entities.Constants.ApplicationConfigurations.DefaultLatestMoviesRefreshIntervalInDays).ToString(CultureInfo.InvariantCulture)
                    };

                    dataContext.ApplicationConfigurations.Add(latestMoviesDate);
                    dataContext.SaveChanges();
                }

                var parsedDate = latestMoviesDate.Value.ToString(CultureInfo.InvariantCulture);

                return Convert.ToDateTime(parsedDate, CultureInfo.InvariantCulture);
            }
        }

        public List<string> GetMoviesGenres()
        {
            using (var dataContext = new MoviesEntities())
            {
                return dataContext.Genres.Select(item => item.Name).ToList();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.Entities;
using System.IO;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Linq.Expressions;
using MovieDictionary.IRepositories;

namespace MovieDictionary.BL
{
    public class MoviesManager
    {
        private IMovies repository;

        private IUsers usersRepository;

        private INotifications notificationsRepository;

        public MoviesManager()
        {
            this.repository = new DAL.MoviesRepository() as IMovies;
            this.usersRepository = new DAL.UsersRepository() as IUsers;
            this.notificationsRepository = new DAL.NotificationsRepository();
        }

        public MoviesManager(IMovies moviesRepository, IUsers usersRepository, INotifications notificationsRepository)
        {
            this.repository = moviesRepository as IMovies;
            this.usersRepository = usersRepository as IUsers;
            this.notificationsRepository = notificationsRepository as INotifications;
        }

        public List<Movie> GetMovies(string searchTerm = null, string genre = null, int? yearStart = null, int? yearEnd = null, string awards = null, string userId = null, string orderType = null, int pageNumber = 0, bool? showMore = null)
        {
            List<Movie> movies = null;

            orderType = orderType ?? Constants.OrderTypes.Votes;

            if (!showMore.HasValue || showMore == false)
            {
                movies = repository.GetMovies(searchTerm, genre, yearStart, yearEnd, awards, userId, orderType, pageNumber);
                //.Where(item => !string.IsNullOrWhiteSpace(searchTerm) ? item.Title.Split(' ').Any(t => t.ToLowerInvariant().StartsWith(searchTerm)) : true).ToList();
            }

            if (((movies == null || movies.Count == 0) && !showMore.HasValue) || (showMore.HasValue && showMore == true && searchTerm != null))
            {
                movies = GetMovieSearchSuggestions(searchTerm);

                if (movies == null || movies.Count == 0)
                    return null;

                movies.ForEach(item => { item.Id = item.imdbID; item.IsInWatchlist = false; item.Rating = null; });
            }

            if (movies != null)
            {
                movies.ForEach(item => item.Poster = (SaveMoviePoster(item.Id, item.Poster) ? item.Poster : Constants.MovieProperties.DefaultPoster));
            }

            return movies;
        }

        public int GetFilteredMoviesCount(string searchTerm = null, string genre = null, int? yearStart = null, int? yearEnd = null, string awards = null)
        {
            return repository.GetFilteredMoviesCount(searchTerm, genre, yearStart, yearEnd, awards);
        }

        public Movie GetMovieDetails(string movieId, string userId = null)
        {
            var movie = repository.GetMovieDetails(movieId, userId);

            if (movie == null)
            {
                movie = ImportMovieById(movieId);

                if (movie == null)
                    return null;

                movie.IsInWatchlist = false;
                movie.Rating = null;
                movie.Votes = 0;
            }

            movie.Poster = (movie.Poster == Constants.MoviesApi.InfoNotAvailable ? Constants.MovieProperties.DefaultPoster : movie.Poster);

            return movie;
        }

        public decimal? RateMovie(string movieId, string userId, int rating)
        {
            if (rating > 10 || rating < 1)
                return null;

            var newRating = repository.RateMovie(movieId, userId, rating);

            var task = new Task(() =>
            {
                usersRepository.CheckForBadges(userId, true, false, false);
            });
            task.Start();

            return newRating;
        }

        public decimal? UnrateMovie(string movieId, string userId)
        {
            return repository.UnrateMovie(movieId, userId);
        }

        public List<Movie> GetWatchlistMovies(string userId)
        {
            return repository.GetWatchlistMovies(userId);
        }

        public void AddMovieToWatchlist(string movieId, string userId)
        {
            repository.AddMovieToWatchlist(movieId, userId);
        }

        public void RemoveMovieFromWatchlist(string movieId, string userId)
        {
            repository.RemoveMovieFromWatchlist(movieId, userId);
        }

        public List<Movie> GetLatestMovies(string userId = null)
        {
            var latestMoviesDate = repository.GetLatestMoviesDate();

            if ((DateTime.Now - latestMoviesDate).Days >= Constants.ApplicationConfigurations.DefaultLatestMoviesRefreshIntervalInDays)
            {
                ImportLatestMovies();
            }

            var movies = repository.GetLatestMovies(userId);
            movies.ForEach(item => item.Poster = (SaveMoviePoster(item.Id, item.Poster) ? item.Poster : Constants.MovieProperties.DefaultPoster));

            return movies;
        }

        public List<Movie> GetUserRatings(string userId)
        {
            var movies = repository.GetUserRatings(userId);
            movies.ForEach(item => item.Poster = (item.Poster == Constants.MoviesApi.InfoNotAvailable ? Constants.MovieProperties.DefaultPoster : item.Poster));

            return movies;
        }

        public List<string> GetMoviesGenres()
        {
            return repository.GetMoviesGenres();
        }

        private void ImportLatestMovies()
        {
            var latestMovies = new List<Movie>();

            var url = Constants.CinemaCity.LatestMoviesUrl;

            string response = string.Empty;

            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                response = client.DownloadString(url);
            }

            var document = new HtmlDocument();
            document.LoadHtml(response);

            var titles = document.DocumentNode.Descendants().Where(e => e.Attributes.Contains(Constants.CinemaCity.RequiredAttribute) && e.Attributes[Constants.CinemaCity.RequiredAttribute].Value.Contains(Constants.CinemaCity.MovieTitleClass)).Select(e => e.InnerHtml).Distinct().ToList();

            for (int i = 0; i < titles.Count; i++)
            {
                var movieTitle = titles[i].Replace(Constants.CinemaCity.MovieSpec2D, string.Empty).Replace(Constants.CinemaCity.MovieSpec3D, string.Empty).Replace(Constants.CinemaCity.MovieSpec4DX, string.Empty);
                var movie = ImportMovieByTitle(movieTitle, true);

                if (movie != null && (movie.Year == DateTime.Now.Year || movie.Year == DateTime.Now.Year + 1 || movie.Year == DateTime.Now.Year - 1))
                {
                    latestMovies.Add(movie);
                }
            }

            if (latestMovies.Count > 0)
            {
                var distinctLatestMovies = latestMovies.GroupBy(item => item.Id).Select(item => item.First()).ToList();
                repository.UpdateLatestMovies(distinctLatestMovies);

                var task = new Task(() =>
                {
                    var latestMoviesIds = distinctLatestMovies.Select(item => item.imdbID).ToList();
                    var usersToAlert = usersRepository.GetUsersToAlertForNewMovies(latestMoviesIds);

                    foreach (var key in usersToAlert.Keys)
                    {
                        foreach (var value in usersToAlert[key])
                            notificationsRepository.AddNotification(key, Constants.NotificationTypes.MovieInCinema, value);
                    }
                });
                task.Start();
            }
        }

        private Movie ImportMovie(string url)
        {
            string jsonResponse = string.Empty;

            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                jsonResponse = client.DownloadString(url);
            }

            Movie movie;

            try
            {
                var settings = new JsonSerializerSettings() { Error = (sender, errorArgs) => { errorArgs.ErrorContext.Handled = true; } };
                movie = JsonConvert.DeserializeObject<Movie>(jsonResponse, settings);
            }
            catch
            {
                return null;
            }

            if (movie.Title == null || movie.Genre == null || movie.Year == 0 || movie.Runtime == null)
                return null;

            movie.Id = movie.imdbID;

            if (movie.Awards != Constants.MoviesApi.InfoNotAvailable)
                movie.Awards = movie.Awards.Split(Constants.MoviesApi.AwardsCharSeparator).ElementAt(0);

            if (movie.Poster != Constants.MoviesApi.InfoNotAvailable)
            {
                movie.Poster = movie.Poster.Replace(Constants.MoviesApi.PosterFormatResponse, Constants.MoviesApi.DefaultPosterFormat);
                var result = SaveMoviePoster(movie.Id, movie.Poster);

                if (!result)
                {
                    movie.Poster = Constants.MovieProperties.DefaultPoster;
                }
            }

            var task = new Task(() =>
            {
                try { movie.Trailer = ImportMovieTrailer(movie.Title, movie.Year.ToString()); } catch { }                
                repository.AddMovie(movie);
                var genres = movie.Genre.Split(Constants.MoviesApi.GenresCharSeparator).Select(item => item.Trim()).Where(item => item.ToLower() != Constants.MoviesApi.InfoNotAvailable.ToLower()).ToList();
                repository.AddGenres(genres);
            });
            task.Start();

            return movie;
        }

        private Movie ImportMovieById(string movieId)
        {
            var url = string.Format(Constants.MoviesApi.MovieIDUrl, movieId);

            return ImportMovie(url);
        }

        private Movie ImportMovieByTitle(string movieTitle, bool isRecentMovie = false)
        {
            string url = string.Empty;
            Movie movie = null;

            if (isRecentMovie)
            {
                url = string.Format(Constants.MoviesApi.MovieTitleYearUrl, movieTitle, DateTime.Now.Year);
                movie = ImportMovie(url);

                if (movie != null)
                    return movie;

                url = string.Format(Constants.MoviesApi.MovieTitleYearUrl, movieTitle, DateTime.Now.Year + 1);
                movie = ImportMovie(url);

                if (movie != null)
                    return movie;

                url = string.Format(Constants.MoviesApi.MovieTitleYearUrl, movieTitle, DateTime.Now.Year - 1);
                movie = ImportMovie(url);

                if (movie != null)
                    return movie;
            }

            url = string.Format(Constants.MoviesApi.MovieTitleUrl, movieTitle);

            return ImportMovie(url);
        }

        private List<Movie> GetMovieSearchSuggestions(string searchTerm)
        {
            var url = string.Format(Constants.MoviesApi.MovieSearchUrl, searchTerm);

            string jsonResponse = string.Empty;

            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                jsonResponse = client.DownloadString(url);
            }

            var searchResult = JsonConvert.DeserializeObject<MovieSearchResults>(jsonResponse);

            return searchResult.Search;
        }

        private string ImportMovieTrailer(string movieTitle, string movieYear)
        {
            var url = string.Format(Constants.Youtube.TrailerSearchUrl, movieTitle + "+" + movieYear + "+" + Constants.Youtube.TrailerKeyword);

            string response = string.Empty;

            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                response = client.DownloadString(url);
            }

            var document = new HtmlDocument();
            document.LoadHtml(response);

            var trailerItems = document.DocumentNode.Descendants().Where(e => 
                e.Attributes.Contains(Constants.Youtube.RequiredAttribute) 
                && e.Attributes[Constants.Youtube.RequiredAttribute].Value.Contains(Constants.Youtube.ResultItemContentClass)
            );

            if (trailerItems == null || trailerItems.Count() == 0)
                return null;

            HtmlNode movieTrailerItem;

            try
            {
                movieTrailerItem = trailerItems.FirstOrDefault(e =>
                        e.ChildNodes.FirstOrDefault(t =>
                                t.Attributes.Contains(Constants.Youtube.RequiredAttribute)
                                && t.Attributes[Constants.Youtube.RequiredAttribute].Value.Contains(Constants.Youtube.ResultItemChannelClass)
                             )
                            .ChildNodes.FirstOrDefault(el => el.Attributes.Contains(Constants.Youtube.LinkRequiredAttribute)).InnerHtml.ToLower().StartsWith(Constants.Youtube.TrailersChannel.ToLower())
                        && e.ChildNodes.FirstOrDefault(t =>
                                t.Attributes.Contains(Constants.Youtube.RequiredAttribute)
                                && t.Attributes[Constants.Youtube.RequiredAttribute].Value.Contains(Constants.Youtube.ResultItemTitleClass)
                             )
                            .ChildNodes.FirstOrDefault(el => el.Attributes.Contains(Constants.Youtube.LinkRequiredAttribute)).InnerHtml.ToLower().StartsWith(movieTitle.ToLower())
                        && e.ChildNodes.FirstOrDefault(t =>
                                t.Attributes.Contains(Constants.Youtube.RequiredAttribute)
                                && t.Attributes[Constants.Youtube.RequiredAttribute].Value.Contains(Constants.Youtube.ResultItemTitleClass)
                             )
                            .ChildNodes.FirstOrDefault(el => el.Attributes.Contains(Constants.Youtube.LinkRequiredAttribute)).InnerHtml.ToLower().Contains(Constants.Youtube.TrailerKeyword)
                );
            }
            catch
            {
                return null;
            }

            if (movieTrailerItem == null)
                return null;

            var trailerHref = movieTrailerItem.Descendants().FirstOrDefault(e =>
            e.Attributes.Contains(Constants.Youtube.RequiredAttribute) &&
            e.Attributes[Constants.Youtube.RequiredAttribute].Value.Contains(Constants.Youtube.ResultItemTitleClass))
            .ChildNodes.FirstOrDefault(e => e.Attributes.Contains(Constants.Youtube.LinkRequiredAttribute));

            if (trailerHref == null)
                return null;

            var trailerId = trailerHref.Attributes[Constants.Youtube.LinkRequiredAttribute].Value.Split(Constants.Youtube.TrailerIdSeparator, StringSplitOptions.None).ElementAt(1);

            return trailerId;
        }

        private bool SaveMoviePoster(string movieId, string poster)
        {
            if (poster == Constants.MoviesApi.InfoNotAvailable)
                return false;

            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.MovieProperties.DefaultPostersDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var fileName = Path.Combine(directory, movieId + Constants.MovieProperties.DefaultPosterExtension);

            if (File.Exists(fileName))
                return true;

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(poster, fileName);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

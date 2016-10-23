using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieDictionary.Entities;

namespace MovieDictionary.Controllers
{
    public class MoviesController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Search", new { showMore = false });
        }

        public ActionResult Search(string title = null, string genre = null, string awards = null, int? yearStart = null, int? yearEnd = null, string orderType = null, int pageNumber = 0, bool? showMore = null)
        {
            try
            {                
                var userId = User != null ? User.Identity.GetUserId() : null;
                var movies = new BL.MoviesManager().GetMovies(title, genre, yearStart, yearEnd, awards, userId, orderType, pageNumber, showMore);

                if (Request.IsAjaxRequest())
                    return Json(movies != null ? movies.Take(Entities.Constants.ApplicationConfigurations.DefaultSearchSuggestionPageSize) : new List<Movie>(), JsonRequestBehavior.AllowGet);

                var model = new Models.SearchMoviesModel()
                {
                    Movies = movies,
                    PageNumber = pageNumber,
                    Title = title,
                    SelectedGenre = genre,
                    Awards = awards,
                    YearStart = yearStart,
                    YearEnd = yearEnd,
                    OrderType = orderType
                };

                if (showMore.HasValue && showMore.Value == false)
                {
                    model.NumberOfMovies = new BL.MoviesManager().GetFilteredMoviesCount(title, genre, yearStart, yearEnd, awards);
                    model.Genres = new BL.MoviesManager().GetMoviesGenres();
                    model.CanFilter = true;
                }
                else
                {
                    model.NumberOfMovies = movies != null ? movies.Count : 0;
                    model.CanFilter = false;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);

                if (Request.IsAjaxRequest())
                {
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
                }

                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public ActionResult Details(string movieId)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var movie = new BL.MoviesManager().GetMovieDetails(movieId, userId);

                if (movie == null)
                {
                    logger.Error(string.Format("Error getting movie {0}", movieId));
                    ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                    return View("Error");
                }

                movie.Reviews = new BL.ReviewsManager().GetMovieReviews(movie.Id, userId, Entities.Constants.OrderTypes.Votes, 0);

                ViewBag.Title = movie.Title;

                return View(movie);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public ActionResult Watchlist()
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return View();

                var movies = new BL.MoviesManager().GetWatchlistMovies(User.Identity.GetUserId());

                return View(movies);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public JsonResult AddMovieToWatchlist(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.MoviesManager();
                manager.AddMovieToWatchlist(movieId, User.Identity.GetUserId());

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveMovieFromWatchlist(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.MoviesManager();
                manager.RemoveMovieFromWatchlist(movieId, User.Identity.GetUserId());

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RateMovie(string movieId, int rating)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var movieRating = new BL.MoviesManager().RateMovie(movieId, User.Identity.GetUserId(), rating);

                if (movieRating.HasValue)
                    return Json(new { Success = true, Rating = movieRating }, JsonRequestBehavior.AllowGet);

                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnrateMovie(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var rating = new BL.MoviesManager().UnrateMovie(movieId, User.Identity.GetUserId());
                rating = rating ?? 0;

                return Json(new { Success = true, Rating = rating }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMovieDetails(string movieId)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var movie = new BL.MoviesManager().GetMovieDetails(movieId, userId);

                if (movie == null)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);

                return Json(new { Success = true, Movie = movie }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError });
            }
        }
    }
}
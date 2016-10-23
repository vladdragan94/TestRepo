using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieDictionary.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var movies = new BL.MoviesManager().GetLatestMovies(userId);

                return View(movies);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public ActionResult Info(string message = null)
        {
            ViewBag.Message = message ?? Entities.Constants.ErrorMessages.DefaultError;

            return View("Info");
        }
    }
}
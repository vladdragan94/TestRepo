using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieDictionary.Controllers
{
    public class UsersController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                var users = new BL.UsersManager().GetTopUsers();
                return View(users);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public ActionResult UserProfile(string userId = null)
        {
            try
            {
                if ((User == null || !User.Identity.IsAuthenticated) && userId == null)
                {
                    ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                    return View("Error");
                }

                var profile = new BL.UsersManager().GetUserProfile(userId, User.Identity.GetUserId());

                return View(profile);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return View("Error");
            }
        }

        public ActionResult Ratings()
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                {
                    ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.NotLoggedIn;
                    return View("Error");
                }

                var movies = new BL.MoviesManager().GetUserRatings(User.Identity.GetUserId());

                return View(movies);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public JsonResult Search(string searchTerm)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var users = new BL.UsersManager().GetUsers(searchTerm, userId);

                return Json(new { Success = true, Users = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddFriend(string userId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.UsersManager();
                manager.AddFriend(User.Identity.GetUserId(), userId);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveFriend(string userId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.UsersManager();
                manager.RemoveFriend(User.Identity.GetUserId(), userId);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RecommendMovie(string movieId, List<string> friends)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.UsersManager();
                manager.RecommendMovie(User.Identity.GetUserId(), movieId, friends);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LikeRecommendation(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.UsersManager();
                manager.LikeRecommendation(User.Identity.GetUserId(), movieId);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DislikeRecommendation(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.UsersManager();
                manager.DislikeRecommendation(User.Identity.GetUserId(), movieId);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetFriendsForRecommendation(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var users = new BL.UsersManager().GetFriendsForRecommendation(User.Identity.GetUserId(), movieId);

                return Json(new { Success = true, Users = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetFriendsWhoRecommendedMovie(string movieId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var users = new BL.UsersManager().GetFriendsWhoRecommendedMovie(User.Identity.GetUserId(), movieId);

                return Json(new { Success = true, Users = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MarkNotificationAsSeen(int notificationId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.UsersManager();
                manager.MarkNotificationAsSeen(notificationId);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
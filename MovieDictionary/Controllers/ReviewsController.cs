using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieDictionary.Entities;
using Microsoft.AspNet.Identity;

namespace MovieDictionary.Controllers
{
    public class ReviewsController : BaseController
    {
        public JsonResult AddReview(string movieId, string title, int rating, string content)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var review = new Review()
                {
                    MovieId = movieId,
                    UserId = User.Identity.GetUserId(),
                    Title = title,
                    Rating = rating,
                    Content = content,
                    DateAdded = DateTime.Now,
                };

                var result = new BL.ReviewsManager().AddReview(review);

                if (result.HasValue)
                {
                    return Json(new { Success = true, ReviewId = result }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LikeReview(int reviewId, bool liked)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.ReviewsManager();
                manager.LikeReview(reviewId, User.Identity.GetUserId(), liked);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnlikeReview(int reviewId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.ReviewsManager();
                manager.UnlikeReview(reviewId, User.Identity.GetUserId());

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteReview(int reviewId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var result = new BL.ReviewsManager().DeleteReview(reviewId, User.Identity.GetUserId());

                if (result)
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetReviews(string movieId, int reviewsPageNumber = 0, string reviewsOrderType = null)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var reviews = new BL.ReviewsManager().GetMovieReviews(movieId, userId, reviewsOrderType, reviewsPageNumber);

                return Json(new { Success = true, Reviews = reviews }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
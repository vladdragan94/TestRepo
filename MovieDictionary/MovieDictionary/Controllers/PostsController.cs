using Microsoft.AspNet.Identity;
using MovieDictionary.Entities;
using MovieDictionary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieDictionary.Controllers
{
    public class PostsController : BaseController
    {
        public ActionResult Index()
        {
            try
            {
                var forumStats = new BL.PostsManager().GetForumStats();

                var model = new ForumModel()
                {
                    ForumStats = forumStats
                };

                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public ActionResult Details(int postId)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var post = new BL.PostsManager().GetPostDetails(postId, userId);

                return View(post);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                ViewBag.ErrorMessage = Entities.Constants.ErrorMessages.DefaultError;
                return View("Error");
            }
        }

        public JsonResult AddPost(string title, string content, bool isQuestion, int? postId = null)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var post = new Post()
                {
                    UserId = User.Identity.GetUserId(),
                    DateAdded = DateTime.Now,
                    Title = title,
                    Content = content,
                    IsQuestion = isQuestion,
                    PostId = postId
                };

                var result = new BL.PostsManager().AddPost(post);

                if (result.HasValue)
                {
                    return Json(new { Success = true, PostId =  result.Value}, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LikePost(int postId, bool liked)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.PostsManager();
                manager.LikePost(postId, User.Identity.GetUserId(), liked);

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnlikePost(int postId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var manager = new BL.PostsManager();
                manager.UnlikePost(postId, User.Identity.GetUserId());

                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MarkAsAnswer(int postId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var result = new BL.PostsManager().MarkAsAnswer(postId, User.Identity.GetUserId());

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

        public JsonResult UnmarkAnswer(int postId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var result = new BL.PostsManager().UnmarkAnswer(postId, User.Identity.GetUserId());

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

        public JsonResult DeletePost(int postId)
        {
            try
            {
                if (User == null || !User.Identity.IsAuthenticated)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);

                var result = new BL.PostsManager().DeletePost(postId, User.Identity.GetUserId());

                if (result)
                {
                    return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.NotLoggedIn }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetQuestions(int pageNumber = 0, string orderType = null)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var questions = new BL.PostsManager().GetQuestions(userId, orderType, pageNumber);

                return Json(new { Success = true, Questions = questions }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPosts(int pageNumber = 0, string orderType = null)
        {
            try
            {
                var userId = User != null ? User.Identity.GetUserId() : null;
                var posts = new BL.PostsManager().GetPosts(userId, orderType, pageNumber);

                return Json(new { Success = true, Posts = posts }, JsonRequestBehavior.AllowGet);                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError }, JsonRequestBehavior.AllowGet);   
            }
        }
    }
}
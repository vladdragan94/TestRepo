using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Web.Helpers;
using System.Web.Routing;

namespace MovieDictionary.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            var action = filterContext.RequestContext.RouteData.Values["action"] as string;
            var controller = filterContext.RequestContext.RouteData.Values["controller"] as string;

            if ((filterContext.Exception is HttpAntiForgeryException) && action == "Login" && controller == "Account"
                && filterContext.RequestContext.HttpContext.User != null && User.Identity.IsAuthenticated
                && Request.IsAjaxRequest())
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = Json(new { Success = false, Message = Entities.Constants.ErrorMessages.AlreadyLoggedIn });
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var currentUrl = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.RouteUrl("Default", ((Route)RouteTable.Routes["Default"]).Defaults);
            var requestedUrl = Request.Url.ToString();

            if (requestedUrl.Contains("Login?ReturnUrl"))
                filterContext.Result = new RedirectResult(currentUrl);
        }
    }
}
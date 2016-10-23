using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MovieDictionary.Models;
using System.Threading;
using System.Security.Cryptography;
using System.Web.Helpers;

namespace MovieDictionary.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public PartialViewResult Login()
        {
            return PartialView("_Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage)) });
                }

                var firstAttempt = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);

                if (firstAttempt == SignInStatus.Success)
                    return Json(new { Success = true });

                var user = UserManager.FindByEmail(model.Username);

                if (user == null)
                    return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.InvalidLoginAttempt });

                var secondAttempt = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);

                if (secondAttempt == SignInStatus.Success)
                    return Json(new { Success = true });

                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.InvalidLoginAttempt });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError });
            }
        }

        [AllowAnonymous]
        public PartialViewResult Register()
        {
            return PartialView("_Register");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return Json(new { Success = true });
                }

                AddErrors(result);
            }

            return Json(new { Success = false, Message = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage)) });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> FacebookLogin(string email, string username)
        {
            var facebookEmail = string.Join("_", "Facebook", "1074891809221294", email);
            var password = Crypto.Hash(facebookEmail + Entities.Constants.ApplicationConfigurations.CryptoString);
            var firstAttempt = await SignInManager.PasswordSignInAsync(username, password, true, shouldLockout: false);

            if (firstAttempt == SignInStatus.Success)
                return Json(new { Success = true });

            var user = new ApplicationUser { UserName = username, Email = facebookEmail };
            var result = await UserManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                return Json(new { Success = true });
            }

            return Json(new { Success = false, Message = Entities.Constants.ErrorMessages.DefaultError });
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Redirect(Request.UrlReferrer.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
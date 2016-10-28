using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using DriverManagementWebApp.Service;
using DriverManagementWebApp.Models;

namespace DriverManagementWebApp.Controllers.EIP
{
    [Authorize]
    public class AccountController : BaseController
    {
        AccountService _accountService;
        public AccountController()
        {
            _accountService = new AccountService();
        }

        #region Login

        /// <summary>
        /// Check If User email address is Existing
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult EmailExists(string emailAddress)
        {
            bool isUserExisted = false;
            account user = _accountService.GetUserByUserName(emailAddress);
            isUserExisted = user != null;

            return Json(isUserExisted, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Check If User email address is Existing
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public JsonResult CheckPassword(string username, string password)
        {
            bool isValid = true;

            isValid = _accountService.DoLogin(username, password);

            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return DoLogin(model, returnUrl);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //Request for cookie
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie != null)
            {
                try
                {
                    //some times no cookie in browser
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);

                    //Get login info from cookie
                    LoginViewModel model = new LoginViewModel();
                    model.Email = ticket.Name;
                    model.Password = ticket.UserData;
                    model.RememberMe = ticket.IsPersistent;

                    return DoLogin(model, returnUrl);
                }
                catch (Exception)
                {

                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        private ActionResult DoLogin(LoginViewModel model, string returnUrl)
        {
            bool isValid = _accountService.DoLogin(model.Email, model.Password);
            if (isValid)
            {
                account user = _accountService.GetUserByUserName(model.Email);
                if (user != null)
                {
                    UserInfoModel userInfo = new UserInfoModel
                    {
                        IsDispatchManager = user.IsDispatchManager,
                        UserID = user.UserID,
                        UserName = user.UserName
                    };
                    Session["User"] = userInfo;
                    HttpSession.SetInSession(userInfo);
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    if (user.IsDispatchManager)
                    {
                        return RedirectToAction("Index", "Parcel");
                    }
                    #endregion Remember Me
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            //Request for cookie
            Response.Cookies["ASPXPIKESADMINAUTH"].Expires = DateTime.Now.AddDays(-1);

            Session.Abandon();
            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                account user = _accountService.GetUserByUserName(model.Email);

                if (user == null)
                {
                    ViewBag.NotExistingUser = "The Email Address does not exist.";
                    // Don't reveal that the user does not exist or is not confirmed
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _accountService.GetUserByUserName(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            user.Password = model.Password;
            var result = _accountService.UpdateUser(user);
            if (result)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            return View();
        }
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ChangePassword()
        {
            UserInfoModel user = HttpSession.GetFromSession<UserInfoModel>();
            if (user == null) return RedirectToAction("Login");

            return View();
        }

        // POST: /Account/DoChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DoChangePassword(ChangePasswordViewModel model)
        {
            UserInfoModel user = HttpSession.GetFromSession<UserInfoModel>();
            if (user == null) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }
            var isValidOldPassword = _accountService.DoLogin(user.UserName, model.OldPassword);

            if (isValidOldPassword)
            {
                account ca_user = _accountService.GetUserByUserName(user.UserName);

                ca_user.Password = model.NewPassword;
                var result = _accountService.UpdateUser(ca_user);

                if (result)
                {
                    return RedirectToAction("Index", "EIP");
                }
            }
            else
            {
                ModelState.AddModelError("OldPassword", "The Old Password is not correct.");
            }
            return View("ChangePassword", model);
        }


    }
    public static class HttpSession
    {
        const string UserLoginKey = "UserLoginKey";

        public static void SetInSession(object value)
        {
            HttpContext.Current.Session[UserLoginKey] = value;
        }

        public static T GetFromSession<T>()
        {
            return (T)HttpContext.Current.Session[UserLoginKey];
        }
    }
}
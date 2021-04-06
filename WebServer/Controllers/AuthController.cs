using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebServer.Models;
using WebServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace WebServer.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        public AuthController(IConfiguration configuration)
        {
            _userService = new UserService(configuration);
        }
        // GET: AuthController/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");

        }
        // POST: AuthController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel userModel)
        {
            string returnUrl = "/";
            LoginConnector _loginConnector = new LoginConnector();
            var result = _loginConnector.VerifyLogin(userModel.Email, userModel.Password);
            if (result.ReturnStatus.Equals(ReturnStatus.Ok))
            {
                var userToken = _userService.LoginUser(result.Value.UserName, result.Value.Password);
                if (userToken != null)
                {
                    //Save token in session object
                    HttpContext.Session.SetString("JWToken", userToken);
                }
                return Redirect(returnUrl);
            }
            else if (result.ReturnStatus.Equals(ReturnStatus.DbError))
            {
                TempData["Error"] = "Datenbank Verbindung verloren";
                return View("~/Views/User/LoginView.cshtml");
            }
            else
            {
                TempData["Error"] = "Email oder Passwort ist nicht korrekt";
                return View("~/Views/User/LoginView.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserModel CurrentUser)
        {
            if (CurrentUser.Password != CurrentUser.ConfirmPassword)
            {
                TempData["Error"] = "Ungleiche Passwörter";
                return View("~/Views/User/RegisterView.cshtml");
            }

            var result = _userService.RegisterUser(CurrentUser.UserName, CurrentUser.Email, CurrentUser.Password);

            if (result.ReturnStatus.Equals(ReturnStatus.Ok))
            {
                TempData["Error"] = "Erfolgreich Registriert";
                return Redirect("~/Home/LoginView");
            }
            else if (result.ReturnStatus.Equals(ReturnStatus.Existing))
            {
                TempData["Error"] = "Email oder Benutzer Name bereits vorhanden";
                return View("~/Views/User/RegisterView.cshtml");
            }
            else
            {
                TempData["Error"] = "Datenbank Verbindung verloren";
                return View("~/Views/User/RegisterView.cshtml");
            }
        }
    }
}

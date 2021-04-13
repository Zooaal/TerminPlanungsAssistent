using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("AuthController/")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        public AuthController(IConfiguration configuration)
        {
            _userService = new UserService(configuration);
        }
        // GET: AuthController/Logout
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");

        }
        // POST: AuthController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Login")]
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
        [Route("Register")]
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

        // POST: AuthController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditUser")]
        public ActionResult EditUser(UserModel userModel)
        {
            LoginConnector _loginConnector = new LoginConnector();
            if (userModel.Password != userModel.ConfirmPassword)
            {
                TempData["Error"] = "Ungleiche Passwörter";
                var currentUser = _loginConnector.Find(User.Claims.First(c => c.Type == ClaimTypes.Email).Value).Value;
                ViewData["user"] = currentUser;
                return View("~/Views/Home/ManageUser.cshtml");
            }
            var user = _loginConnector.Find(User.Claims.First(c => c.Type == ClaimTypes.Email).Value).Value;
            userModel.Meetings = user.Meetings;
            _loginConnector.UpsertUser(userModel);
            return RedirectToAction("Index", "Home");
        }

        // POST: AuthController/GetKey
        [HttpPost]
        [Route("GetKey")]
        public ActionResult GetKey([FromBody]UserModel userModel)
        {
            LoginConnector _loginConnector = new LoginConnector();
            var result = _loginConnector.VerifyLogin(userModel.Email, userModel.Password);
            if (result.ReturnStatus.Equals(ReturnStatus.Ok))
            {
                var userToken = _userService.LoginUser(result.Value.UserName, result.Value.Password);
                if (userToken != null)
                {
                    return Ok(userToken);
                }
                return BadRequest("Not Authorized");
            }
            else
            {
                return BadRequest("Not Authorized");
            }
        }
    }
}

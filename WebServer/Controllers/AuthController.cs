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
            // Objekt für die User abhandlung
            _userService = new UserService(configuration);
        }
        // GET: AuthController/Logout
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            // Die momentane Session leeren
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
            // Verifizierung der User Daten für das Login
            var result = _loginConnector.VerifyLogin(userModel.Email, userModel.Password);
            // Überprüfen ob der User verifiziert ist
            if (result.ReturnStatus.Equals(ReturnStatus.Ok))
            {
                // Token erstellen lassen und in die Session speichern
                var userToken = _userService.LoginUser(result.Value.UserName, result.Value.Password);
                if (userToken != null)
                { 
                    HttpContext.Session.SetString("JWToken", userToken);
                }
                return Redirect(returnUrl);
            } // Error Rückgabe wenn der User nicht verifiziert ist
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
            // Überprüfen auf gleiche Passwörter
            if (CurrentUser.Password != CurrentUser.ConfirmPassword)
            {
                TempData["Error"] = "Ungleiche Passwörter";
                return View("~/Views/User/RegisterView.cshtml");
            }
            // User in der Datenbank registrieren
            var result = _userService.RegisterUser(CurrentUser.UserName, CurrentUser.Email, CurrentUser.Password);
            // Resultat der Registrierung auswerten
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
            // Überprüfen ob die Passwörter gleich sind
            if (userModel.Password != userModel.ConfirmPassword)
            {
                // Seite mit Fehlermeldung und den Daten neu öffnen
                TempData["Error"] = "Ungleiche Passwörter";
                var idString = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Guid id = Guid.Parse(idString);
                var currentUser = _loginConnector.FindById(id).Value;
                ViewData["user"] = currentUser;
                return View("~/Views/Home/ManageUser.cshtml");
            }
            // Die Datenbank mit den neuen Daten updaten und den Benutzer zur Home Website zurück
            var idString2 = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Guid id2 = Guid.Parse(idString2);
            var user = _loginConnector.FindById(id2).Value;
            userModel.Meetings = user.Meetings;
            _loginConnector.UpsertUser(userModel);
            return RedirectToAction("Index", "Home");
        }
        // Postman Methode um einen JWT zu bekommen
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

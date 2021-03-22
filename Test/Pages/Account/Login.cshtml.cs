using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Test.Connectoren;
using Test.Models;

namespace Test.Pages
{
    // https://www.youtube.com/watch?v=BWa7Mu-oMHk
    public class LoginModel : PageModel
    {
        private readonly LoginConnector _loginConnector;
        private readonly ILogger<LoginModel> _logger;
        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
            _loginConnector = new LoginConnector(_logger);
        }
        [BindProperty]
        public UserModel CurrentUser { get; set; }
        public void OnGet(string handler)
        {
            if(!string.IsNullOrEmpty(handler))
            {
                if (handler.Contains("Erfolg"))
                {
                    TempData["Info"] = handler;
                }
            }
        }
        
        public async Task<IActionResult> OnPost(string returnUrl)
        {
            var result = _loginConnector.VerifyLogin(CurrentUser.Email, CurrentUser.Password);
            if (result.ReturnStatus.Equals(ReturnStatus.Ok))
            {
                var User = result.Value;
                var claims = new List<Claim>();
                claims.Add(new Claim("email", User.Email));
                claims.Add(new Claim(ClaimTypes.Email, User.Email));
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
                if(string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = "/";
                }
                return Redirect(returnUrl);
            }
            else if(result.ReturnStatus.Equals(ReturnStatus.DbError))
            {
                TempData["Error"] = "Datenbank Verbindung verloren";
                return Page();
            }
            else
            {
                TempData["Error"] = "Email oder Passwort ist nicht korrekt";
                return Page();
            }
        }

        public async Task<IActionResult> OnGetLogout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}

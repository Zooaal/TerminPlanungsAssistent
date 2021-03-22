using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Test.Connectoren;
using Test.Models;

namespace Test.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public UserModel CurrentUser { get; set; }
        private readonly LoginConnector _loginConnector;
        private readonly ILogger<RegisterModel> _logger;
        public RegisterModel(ILogger<RegisterModel> logger)
        {
            _logger = logger;
            _loginConnector = new LoginConnector(_logger);
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if(CurrentUser.Password != CurrentUser.ConfirmPassword)
            {
                TempData["Error"] = "Ungleiche Passwörter";
                return Page();
            }

            var result = _loginConnector.Register(CurrentUser.UserName, CurrentUser.Email, CurrentUser.Password);

            if (result.ReturnStatus.Equals(ReturnStatus.Ok))
            {
                var returnUrl = "/Account/Login";
                string msg = "Erfolgreich Registriert";
                return RedirectToPage(returnUrl, msg);
            }
            else if(result.ReturnStatus.Equals(ReturnStatus.Existing))
            {
                TempData["Error"] = "Email oder Benutzer Name bereits vorhanden";
                return Page();
            }
            else
            {
                TempData["Error"] = "Datenbank Verbindung verloren";
                return Page();
            }
        }
    }
}

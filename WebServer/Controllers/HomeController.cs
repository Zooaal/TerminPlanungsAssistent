using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebServer.Models;
using WebServer.Services;
using System.Security.Claims;
using BC = BCrypt.Net.BCrypt;

namespace WebServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // View der Main Page
        public IActionResult Index()
        {
            return View();
        }
        // Anzeigen der Loginoberfläche
        public IActionResult LoginView()
        {
            return View("~/Views/User/LoginView.cshtml");
        }
        // Anzeigen der Registrierungsoberfläche
        public IActionResult RegisterView()
        {
            return View("~/Views/User/RegisterView.cshtml");
        }
        // Seite um den momentanen Benutzer mit Daten zu bearbeiten laden
        [Authorized]
        public IActionResult ManageUserView()
        {
            var loginConnector = new LoginConnector();
            var idString = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Guid id = Guid.Parse(idString);
            var user = loginConnector.FindById(id).Value;
            ViewData["user"] = user;
            return View("~/Views/Home/ManageUser.cshtml");
        }
        // Seite mit den Verfügbaren und eigenen Meetings
        [Authorized]
        public IActionResult Meetings()
        {
            var meetingConnector = new MeetingConnector();
            var user = new UserModel();
            // Wenn der User Authorisiert ist
            if (User.Identity.IsAuthenticated)
            {
                // Den Momentanen User aus der Datenbank holen
                var idString = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Guid id = Guid.Parse(idString);
                var loginConnector = new LoginConnector(); 
                user = loginConnector.FindById(id).Value;
            }
            // Meetings des Momentanen Users holen
            var meetings = meetingConnector.GetUserMeetings(user.Email).Value;
            // Falls der Benutzer ein Admin ist alle Mettings holen
            if (User.HasClaim(Roles.ADMIN, Roles.ADMIN))
            {
                var result = meetingConnector.GetAllMeetings().Value;
                meetings = result.FindAll(m => m.Taken);
            }
            // Alle nicht vergebenen Meetings holen
            var allMeetings = meetingConnector.GetAllNotTakenMeetings().Value;
            //Meetings alle in der List nach Datum aufsteigend Sortieren
            meetings = meetings.OrderBy(x => x.DateTime).ToList();
            allMeetings = allMeetings.OrderBy(x => x.DateTime).ToList();
            // Meetings für die CSHTML Seite speichern 
            ViewData["allMeetings"] = allMeetings;
            ViewData["meetings"] = meetings;
            return View();
        }
        [Authorized]
        public IActionResult AdminMeetings()
        {
            // Alle Meetings holen und speichern für die CSHTML Seite
            var meetingConnector = new MeetingConnector();
            var meetings = meetingConnector.GetAllMeetings().Value;
            meetings = meetings.OrderBy(x => x.DateTime).ToList();
            ViewData["meetings"] = meetings;
            return View();
        }
        // Anzeigen der Error Seite
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

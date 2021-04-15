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

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult LoginView()
        {
            return View("~/Views/User/LoginView.cshtml");
        }
        public IActionResult RegisterView()
        {
            return View("~/Views/User/RegisterView.cshtml");
        }
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
        [Authorized]
        public IActionResult Meetings()
        {
            var meetingConnector = new MeetingConnector();
            var user = new UserModel();
            if (User.Identity.IsAuthenticated)
            {
                var idString = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Guid id = Guid.Parse(idString);
                //var dict = User.Claims.Where(p => p.Type == ClaimTypes.Email).ToDictionary(p => p.Type, p => p.Value);
                //email = dict.Values.First();
                var loginConnector = new LoginConnector(); 
                user = loginConnector.FindById(id).Value;
            }
            var meetings = meetingConnector.GetUserMeetings(user.Email).Value;
            if (User.HasClaim(Roles.ADMIN, Roles.ADMIN))
            {
                var result = meetingConnector.GetAllMeetings().Value;
                meetings = result.FindAll(m => m.Taken);
            }
            var allMeetings = meetingConnector.GetAllNotTakenMeetings().Value;
            meetings = meetings.OrderBy(x => x.DateTime).ToList();
            allMeetings = allMeetings.OrderBy(x => x.DateTime).ToList();
            ViewData["allMeetings"] = allMeetings;
            ViewData["meetings"] = meetings;
            return View();
        }
        [Authorized]
        public IActionResult AdminMeetings()
        {
            var meetingConnector = new MeetingConnector();
            var meetings = meetingConnector.GetAllMeetings().Value;
            meetings = meetings.OrderBy(x => x.DateTime).ToList();
            ViewData["meetings"] = meetings;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

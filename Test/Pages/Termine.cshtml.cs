using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.Connectoren;
using Test.Models;

namespace Test.Pages
{
    [Authorize]
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly MeetingConnector meetingConnector;
        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
            meetingConnector = new MeetingConnector(_logger);
        }

        public List<MeetingModel> Meetings { get; set; }
        public UserModel CurrentUser { get; set; }
        public void OnGet()
        {
            string email = "";
            if(User.Identity.IsAuthenticated)
            {
                var dict = User.Claims.Where(p => p.Type == ClaimTypes.Email).ToDictionary(p => p.Type, p => p.Value);
                email = dict.Values.First();
            }
            Meetings = meetingConnector.GetUserMeetings(email).Value;
            ViewData["meetings"] = Meetings;
        }
    }
}

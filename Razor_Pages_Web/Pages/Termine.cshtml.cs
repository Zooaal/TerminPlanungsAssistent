using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Razor_Pages_Web.Connectoren;
using Razor_Pages_Web.Models;

namespace Razor_Pages_Web.Pages
{
    [Authorize]
    public class TermineModel : PageModel
    {
        private readonly ILogger<TermineModel> _logger;
        private readonly MeetingConnector meetingConnector;
        public TermineModel(ILogger<TermineModel> logger)
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

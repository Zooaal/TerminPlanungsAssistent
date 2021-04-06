using Microsoft.AspNetCore.Mvc;
using WebServer.Models;
using WebServer.Services;

namespace WebServer.Controllers
{

    public class MeetingController : Controller
    {
        private readonly MeetingConnector meetingConnector;
        public MeetingController()
        {
            meetingConnector = new MeetingConnector();
        }
        // GET: MeetingController/AllMeetings
        [HttpGet]
        public ActionResult AllMeetings()
        {
            var meetings = meetingConnector.GetAllMeetings();
            return Ok(meetings.Value);
        }

        // POST: MeetingController/Create
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MeetingModel meeting)
        {
            meetingConnector.InsertMeeting(meeting);
            var meetings = meetingConnector.GetAllMeetings().Value;
            ViewData["meetings"] = meetings;
            return View("~/Views/Home/AdminMeetings.cshtml");
        }

        // POST: MeetingController/Edit
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MeetingModel meeting)
        {
            // Return Edit View with Meeting
            var currentMeeting = meetingConnector.GetMeetingById(meeting).Value;
            return View();
        }

        // POST: MeetingController/Delete
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(MeetingModel meeting)
        {
            try
            {
                var result = meetingConnector.DeleteMeetingById(meeting);
                var meetings = meetingConnector.GetAllMeetings().Value;
                ViewData["meetings"] = meetings;
                return View("~/Views/Home/AdminMeetings.cshtml");
            }
            catch
            {
                return View("Error");
            }
        }
        // POST: MeetingController/Select
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult Select(MeetingModel meeting)
        {
            try
            {
                var result = meetingConnector.UpsertMeeting(meeting);
                var meetings = meetingConnector.GetAllMeetings().Value;
                ViewData["meetings"] = meetings;
                return View("~/Views/Home/Meetings.cshtml");
            }
            catch
            {
                return View("Error");
            }
        }
    }
}

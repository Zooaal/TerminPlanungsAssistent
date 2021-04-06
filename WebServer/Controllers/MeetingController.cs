using System.Linq;
using System.Security.Claims;
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
            return RedirectToAction("AdminMeetings", "Home");
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
            ViewData["meeting"] = currentMeeting;
            return View("~/Views/Home/ModifyMeeting.cshtml");
        }
        // POST: MeetingController/Edit
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult EditMeeting(MeetingModel meeting)
        {
            meetingConnector.UpsertMeeting(meeting);
            return RedirectToAction("AdminMeetings", "Home");
            return View("~/Views/Home/AdminMeetings.cshtml");
        }
        // POST: MeetingController/Cancel
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult CancelMeeting(MeetingModel meeting)
        {
            try
            {
                var model = meetingConnector.GetMeetingById(meeting);
                model.Value.Taken = false;
                meetingConnector.UpsertMeeting(model.Value);
                meetingConnector.CancelMeetingForUser(model.Value,
                    new UserModel() { UserName = User.Claims.First(c => c.Type == ClaimTypes.Name).Value });
                return RedirectToAction("Meetings", "Home");
                return View("~/Views/Home/Meetings.cshtml");
            }
            catch
            {
                return View("Error");
            }
        }
        // POST: MeetingController/Delete
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(MeetingModel meeting)
        {
            try
            {
                meetingConnector.DeleteMeetingById(meeting);
                return RedirectToAction("AdminMeetings", "Home");
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
                var model =  meetingConnector.GetMeetingById(meeting);
                model.Value.Taken = true;
                meetingConnector.UpsertMeeting(model.Value);
                meetingConnector.UpsertMeetingForUser(model.Value,
                    new UserModel() {UserName = User.Claims.First(c => c.Type == ClaimTypes.Name).Value});
                return RedirectToAction("Meetings", "Home");
                return View("~/Views/Home/Meetings.cshtml");
            }
            catch
            {
                return View("Error");
            }
        }
    }
}

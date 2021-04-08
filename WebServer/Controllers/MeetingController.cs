using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models;
using WebServer.Services;

namespace WebServer.Controllers
{
    [ApiController]
    [Route("MeetingController/")]
    public class MeetingController : Controller
    {
        private readonly MeetingConnector meetingConnector;
        public MeetingController()
        {
            meetingConnector = new MeetingConnector();
        }
        // GET: MeetingController/AllMeetings
        [HttpGet]
        [Route("All")]
        public ActionResult AllMeetings()
        {
            var meetings = meetingConnector.GetAllMeetings();
            return Ok(meetings.Value);
        }

        // POST: MeetingController/Create
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        [Route("Create")]
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
        [Route("Edit/{id}")]
        public ActionResult Edit(Guid id)
        {
            // Return Edit View with Meeting
            var currentMeeting = meetingConnector.GetMeetingById(new MeetingModel(){ID = id}).Value;
            ViewData["meeting"] = currentMeeting;
            return View("~/Views/Home/ModifyMeeting.cshtml");
        }
        // POST: MeetingController/Edit/{id}
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        [Route("EditMeeting")]
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
        [Route("CancelMeeting/{id}")]
        public ActionResult CancelMeeting(Guid id)
        {
            try
            {
                var model = meetingConnector.GetMeetingById(new MeetingModel(){ID = id});
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
        [Route("Delete/{id}")]
        public ActionResult Delete(Guid id)
        {
            try
            {
                meetingConnector.DeleteMeetingById(new MeetingModel(){ID = id});
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
        [Route("Select/{id}")]
        public ActionResult Select(Guid id)
        {
            try
            {
                var model =  meetingConnector.GetMeetingById(new MeetingModel(){ID = id});
                if (model.Value.Taken)
                    return RedirectToAction("Meetings", "Home");

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

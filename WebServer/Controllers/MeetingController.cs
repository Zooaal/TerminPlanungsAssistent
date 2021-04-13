using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebServer.Models;
using WebServer.Services;

namespace WebServer.Controllers
{
    [Route("MeetingController/")]
    public class MeetingController : Controller
    {
        private readonly MeetingConnector meetingConnector;
        public MeetingController()
        {
            meetingConnector = new MeetingConnector();
        }
        // GET: MeetingController/All
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
        // POST: MeetingController/EditMeeting/{meeting}
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
        //Region for Postman Requests
        #region Postman
        // POST: MeetingController/UpdateMeeting
        [HttpPost]
        [AuthorizePostman]
        [Route("UpdateMeeting")]
        public ActionResult UpdateMeeting([FromBody] MeetingModel meeting)
        {
            try
            {
                meetingConnector.UpsertMeeting(meeting);
                return Ok("Meeting: " + meeting.ToJson() + " updatet");
            }
            catch
            {
                return BadRequest("Db Error");
            }
        }
        // POST: MeetingController/CreateMeeting
        [HttpPost]
        [AuthorizePostman]
        [Route("CreateMeeting")]
        public ActionResult CreateMeeting([FromBody]MeetingModel meeting)
        {
            try
            {
                meetingConnector.InsertMeeting(meeting);
                return Ok("Meeting: " + meeting.ToJson() + " erstellt");
            }
            catch
            {
                return BadRequest("Db Error");
            }
        }

        // POST: MeetingController/DeleteMeeting
        [HttpPost]
        [AuthorizePostman]
        [Route("DeleteMeeting")]
        public ActionResult DeleteMeeting([FromQuery]Guid id)
        {
            try
            {
                if (meetingConnector.GetMeetingById(new MeetingModel(){ID = id}).Value.Taken == true)
                {
                    return BadRequest("Meeting ist schon gebucht");
                }
                meetingConnector.DeleteMeetingById(new MeetingModel() { ID = id });
                return Ok("Meeting mit Id: " + id.ToJson() + " gelöscht");
            }
            catch
            {
                return BadRequest("Db Error");
            }
        }

        // GET: MeetingController/AllAuth
        [HttpGet]
        [AuthorizePostman]
        [Route("AllAuth")]
        public ActionResult AllAuthMeetings()
        {
            var meetings = meetingConnector.GetAllMeetings();
            return Ok(meetings.Value);
        }
        #endregion
    }
}

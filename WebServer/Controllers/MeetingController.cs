using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        // GET: MeetingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MeetingController/Create
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MeetingModel meeting)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: MeetingController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MeetingModel meeting)
        {
            // Return Edit View with Meeting
            var currentMeeting = meetingConnector.GetMeetingById(meeting).Value;
            return View();
        }

        // POST: MeetingController/Delete
        [HttpPost]
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
    }
}

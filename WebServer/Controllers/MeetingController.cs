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
        // Alle Meetings anzeigen ohne eine Authorisierung Nur für Test zwecke dieses Projekts also in wirklicher veröffentlichung rauszunehmen
        // GET: MeetingController/All
        [HttpGet]
        [Route("All")]
        public ActionResult AllMeetings()
        {
            var meetings = meetingConnector.GetAllMeetings();
            return Ok(meetings.Value);
        }

        // Meeting auswahl für die Kalender ansicht
        [HttpGet]
        [Authorized]
        [Route("AllEvents")]
        public IActionResult AllEvents()
        {
            //Überprüfen ob Admin
            if (User.HasClaim("ADMIN", "ADMIN"))
            {
                //Alle für den Admin freien und gebuchten Meetings holen
                var meetings = meetingConnector.GetAllMeetings().Value;
                var takenMeetings = meetings.Where(m => m.Taken == true).ToList();
                var freeMeetings = meetings.Where(m => m.Taken == false).ToList();
                //Alle gebuchten Meetings in das Kalender format
                var events1 = takenMeetings.Select(e => new
                {
                    id = e.Id.ToString(),
                    title = "Gebuchter Termin",
                    start = e.DateTime.ToString("yyyy-MM-dd HH:mm"),
                    end = e.DateTime.AddHours(e.TimeSpan).ToString("yyyy-MM-dd HH:mm"),
                    color = "red"
                }).ToList();
                //Alle freien Meetings in das Kalender format
                var events2 = freeMeetings.Select(e => new
                {
                    id = e.Id.ToString(),
                    title = "Freier Termin",
                    start = e.DateTime.ToString("yyyy-MM-dd HH:mm"),
                    end = e.DateTime.AddHours(e.TimeSpan).ToString("yyyy-MM-dd HH:mm"),
                    color = "green"
                }).ToList();
                // Alle Meetings zusammen führen und als JSON zurück senden
                var events = events1;
                events.AddRange(events2);
                return Json(events);
            }
            else
            {
                // Alle freien und die gebuchten Termine des USeres holen
                var meetings = meetingConnector.GetAllMeetings().Value;
                var freeMeetings = meetings.Where(m => m.Taken == false).ToList();
                var ownMeetings = meetingConnector.GetUserMeetings(User.Claims
                    .First(c => c.Type == ClaimTypes.Email).Value.ToString()).Value;
                //Alle freien Meetings in das Kalender format
                var events1 = freeMeetings.Select(e => new
                {
                    id = e.Id.ToString(),
                    title = "Freier Termin",
                    start = e.DateTime.ToString("yyyy-MM-dd HH:mm"),
                    end = e.DateTime.AddHours(e.TimeSpan).ToString("yyyy-MM-dd HH:mm"),
                    color = "green"
                }).ToList();
                //Alle eigen gebuchte Meetings in das Kalender format
                var events2 = ownMeetings.Select(e => new
                {
                    id = e.Id.ToString(),
                    title = "Mein Termin",
                    start = e.DateTime.ToString("yyyy-MM-dd HH:mm"),
                    end = e.DateTime.AddHours(e.TimeSpan).ToString("yyyy-MM-dd HH:mm"),
                    color = "blue"
                }).ToList();
                // Alle Meetings zusammen führen und als JSON zurück senden
                var events = events1;
                events.AddRange(events2);
                return Json(events);
            }
        }
        // POST: MeetingController/Create
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public ActionResult Create(MeetingModel meeting)
        {
            // Erstellen eines Meetings in die Datenbank
            meetingConnector.InsertMeeting(meeting);
            return RedirectToAction("AdminMeetings", "Home");
        }

        // POST: MeetingController/Edit
        [HttpPost]
        [Authorized]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public ActionResult Edit(Guid id)
        {
            // Return Edit View mit dem passenden Meeting
            var currentMeeting = meetingConnector.GetMeetingById(new MeetingModel(){Id = id}).Value;
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
            // Das editierte Meeting speichern
            meetingConnector.UpsertMeeting(meeting);
            return RedirectToAction("AdminMeetings", "Home");
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
                // Abzuwählendes Meeting aus der Datenbank holen und taken wert auf false setzen
                var model = meetingConnector.GetMeetingById(new MeetingModel(){Id = id});
                model.Value.Taken = false;
                // Meeting wieder geändert abspeichern
                meetingConnector.UpsertMeeting(model.Value);
                // Danach Meeting bei dem Benutzer aus der Liste der Datenbank entfernen
                var idString = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Guid id2 = Guid.Parse(idString);
                LoginConnector loginConnector = new LoginConnector();
                var user = loginConnector.FindById(id2).Value;
                meetingConnector.CancelMeetingForUser(model.Value, user);
                // Seite neu anzeigen
                return RedirectToAction("Meetings", "Home");
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
                // Meeting aus Datenbank löschen
                meetingConnector.DeleteMeetingById(new MeetingModel(){Id = id});
                return RedirectToAction("AdminMeetings", "Home");
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
                // Meeting was ausgewählt werden soll aus der Datenbank holen
                var model =  meetingConnector.GetMeetingById(new MeetingModel(){Id = id});
                // Überprüfen ob es schon gesetzt ist
                if (model.Value.Taken)
                    return RedirectToAction("Meetings", "Home");
                // Meeting auf taken setzen und wieder updaten
                model.Value.Taken = true;
                meetingConnector.UpsertMeeting(model.Value);
                // Bei dem Benutzer das Meeting hinzufügen
                var idString = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                Guid id2 = Guid.Parse(idString);
                LoginConnector loginConnector = new LoginConnector();
                var user = loginConnector.FindById(id2).Value;
                meetingConnector.UpsertMeetingForUser(model.Value, user);
                return RedirectToAction("Meetings", "Home");
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

        // Delete: MeetingController/DeleteMeeting
        [HttpDelete]
        [AuthorizePostman]
        [Route("DeleteMeeting")]
        public ActionResult DeleteMeeting([FromQuery]Guid id)
        {
            try
            {
                if (meetingConnector.GetMeetingById(new MeetingModel(){Id = id}).Value.Taken == true)
                {
                    return BadRequest("Meeting ist schon gebucht");
                }
                meetingConnector.DeleteMeetingById(new MeetingModel() { Id = id });
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

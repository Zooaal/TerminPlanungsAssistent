using DataAccessLibary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //MongoCRUD db = new MongoCRUD("TerminPlanungsAssistent");
            //MeetingModel meeting1 = new MeetingModel { DateTime = new DateTime(2021, 03, 20), TimeSpan = new TimeSpan(1, 30, 0), Taken = true };
            //db.InsertRecord("Meetings", meeting1);
            //MeetingModel meeting2 = new MeetingModel { DateTime = new DateTime(2021, 04, 25), TimeSpan = new TimeSpan(0, 30, 0), Taken = true };
            //db.InsertRecord("Meetings", meeting2);

            //var User = db.LoadRecordByEmail<UserModel>("Users", "TimoZaoral@gmail.de");
            //User.Meetings.AddRange(new List<Guid> { meeting1.ID, meeting2.ID });
            //db.UpsertRecord("Users", User.ID, User);

            //db.InsertRecord("Users", new UserModel { Email = "TimoZaoral@gmail.com", Password = "passwort", Meetings = new List<Guid> { meeting1.ID, meeting2.ID } });

            //var recs = db.LoadRecords<MeetingModel>("Meetings");
            //var oneRec = db.LoadRecordById<MeetingModel>("Meetings", recs.First().ID);
            //oneRec.DateTime = new DateTime(2021, 03, 22, 11, 30, 0, DateTimeKind.Utc);
            //db.UpsertRecord("Meetings", oneRec.ID, oneRec);          
        }
    }
}

using DataAccessLibary;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Models;

namespace Test.Connectoren
{
    public class MeetingConnector
    {
        private readonly MongoCRUD _db;
        private readonly ILogger _logger;
        public MeetingConnector(ILogger logger)
        {
            _db = new MongoCRUD("TerminPlanungsAssistent");
            _logger = logger;
        }

        public LogikReturn<List<MeetingModel>> GetUserMeetings(string email)
        {
            try
            {
                var user = _db.LoadRecordByEmail<UserModel>("Users", email);
                List<MeetingModel> meetings = new List<MeetingModel>();
                foreach (var id in user.Meetings)
                {
                    meetings.Add(_db.LoadRecordById<MeetingModel>("Meetings", id));
                }
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.Ok, meetings);
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.DbError, null);
            }
        }
    }
}

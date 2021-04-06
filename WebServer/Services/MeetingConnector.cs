using DataAccessLibary;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Services
{
    public class MeetingConnector
    {
        private readonly MongoCRUD _db;
        public MeetingConnector()
        {
            _db = new MongoCRUD("TerminPlanungsAssistent");
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
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.DbError, null);
            }
        }

        public LogikReturn<List<MeetingModel>> GetAllMeetings()
        {
            try
            {
                List<MeetingModel> meetings = new List<MeetingModel>();
                meetings = _db.LoadRecords<MeetingModel>("Meetings");
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.Ok, meetings);
            }
            catch (Exception e)
            {
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.DbError, null);
            }
        }
        public LogikReturn<List<MeetingModel>> GetAllNotTakenMeetings()
        {
            try
            {
                List<MeetingModel> meetings = new List<MeetingModel>();
                meetings = _db.LoadRecords<MeetingModel>("Meetings");
                var result = meetings.Where(m => m.Taken == false).ToList();
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.Ok, result);
            }
            catch (Exception e)
            {
                return new LogikReturn<List<MeetingModel>>(ReturnStatus.DbError, null);
            }
        }

        public LogikReturn<MeetingModel> GetMeetingById(MeetingModel model)
        {
            try
            {
                var meeting = _db.LoadRecordById<MeetingModel>("Meetings", model.ID);
                return new LogikReturn<MeetingModel>(ReturnStatus.Ok, meeting);
            }
            catch (Exception e)
            {
                return new LogikReturn<MeetingModel>(ReturnStatus.DbError, null);
            }
        }
        public LogikReturn<MeetingModel> DeleteMeetingById(MeetingModel model)
        {
            try
            {
                _db.DeleteRecord<MeetingModel>("Meetings", model.ID);
                return new LogikReturn<MeetingModel>(ReturnStatus.Ok, null);
            }
            catch (Exception e)
            {
                return new LogikReturn<MeetingModel>(ReturnStatus.DbError, null);
            }
        }
        public LogikReturn<MeetingModel> InsertMeeting(MeetingModel model)
        {
            try
            {
                _db.InsertRecord<MeetingModel>("Meetings", model);
                return new LogikReturn<MeetingModel>(ReturnStatus.Ok, null);
            }
            catch (Exception e)
            {
                return new LogikReturn<MeetingModel>(ReturnStatus.DbError, null);
            }
        }
        public LogikReturn<MeetingModel> UpsertMeeting(MeetingModel model)
        {
            try
            {
                _db.UpsertRecord<MeetingModel>("Meetings", model.ID, model);
                return new LogikReturn<MeetingModel>(ReturnStatus.Ok, null);
            }
            catch (Exception e)
            {
                return new LogikReturn<MeetingModel>(ReturnStatus.DbError, null);
            }
        }
        public LogikReturn<MeetingModel> UpsertMeetingForUser(MeetingModel model, UserModel user)
        {
            try
            {
                var currentUser = _db.LoadRecordByUserName<UserModel>("Users", user.UserName);
                currentUser.Meetings.Add(model.ID);
                _db.UpsertRecord<UserModel>("Users", currentUser.Id, currentUser);
                return new LogikReturn<MeetingModel>(ReturnStatus.Ok, null);
            }
            catch (Exception e)
            {
                return new LogikReturn<MeetingModel>(ReturnStatus.DbError, null);
            }
        }
        public LogikReturn<MeetingModel> CancelMeetingForUser(MeetingModel model, UserModel user)
        {
            try
            {
                var currentUser = _db.LoadRecordByUserName<UserModel>("Users", user.UserName);
                currentUser.Meetings.Remove(model.ID);
                _db.UpsertRecord<UserModel>("Users", currentUser.Id, currentUser);
                return new LogikReturn<MeetingModel>(ReturnStatus.Ok, null);
            }
            catch (Exception e)
            {
                return new LogikReturn<MeetingModel>(ReturnStatus.DbError, null);
            }
        }
    }
}

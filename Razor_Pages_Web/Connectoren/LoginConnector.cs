using DataAccessLibary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Razor_Pages_Web.Models;
using BC = BCrypt.Net.BCrypt;

namespace Razor_Pages_Web.Connectoren
{
    public class LoginConnector
    {
        private readonly MongoCRUD _db;
        private readonly ILogger _logger;
        public LoginConnector(ILogger logger)
        {
            _db = new MongoCRUD("TerminPlanungsAssistent");
            _logger = logger;
        }

        public LogikReturn<UserModel> VerifyLogin(string email, string password)
        {
            try
            {
                var User = _db.LoadRecordByEmail<UserModel>("Users", email);
                if (User == null)
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Fail, null);
                }
                else if (BC.Verify(password, User.Password))
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Ok, User);
                }
                else
                    return new LogikReturn<UserModel>(ReturnStatus.Fail, null);
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }

        public LogikReturn<UserModel> Register(string username, string email, string password )
        {
            try
            {
                var Users = _db.LoadRecords<UserModel>("Users");
                var badList = Users.Where(u => u.UserName == username).ToList();
                badList.AddRange(Users.Where(u => u.Email == email).ToList());
                if (badList.Count > 0)
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Existing, null);
                }
                var user = new UserModel()
                {
                    Email = email,
                    UserName = username,
                    Password = BC.HashPassword(password),
                    Meetings = new List<Guid>()
                };
                _db.InsertRecord<UserModel>("Users", new UserModel() 
                    { Email = email, 
                    UserName = username, 
                    Password = BC.HashPassword(password), 
                    Meetings = new List<Guid>() });
                return new LogikReturn<UserModel>(ReturnStatus.Ok, user);
            }catch(Exception e)
            {
                _logger.LogError(e.ToString());
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }
    }
}

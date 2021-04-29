using DataAccessLibary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;
using BC = BCrypt.Net.BCrypt;

namespace WebServer.Services
{
    public class LoginConnector
    {
        private readonly MongoCRUD _db;
        // Verbindung zur Datenbank im Konstruktor
        public LoginConnector()
        {
            _db = new MongoCRUD("TerminPlanungsAssistent");
        }
        // Methode um einen User zu suchen nach der ID und rückmeldung mit eigen definierten status Codes und Dem Value(User)
        public LogikReturn<UserModel> FindById(Guid userId)
        {
            try
            {
                var result = _db.LoadRecordById<UserModel>("Users", userId);
                if (result != null)
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Ok, result);
                }
                else
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Fail, null);
                }
            }
            catch (Exception e)
            {
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }
        // Methode um einen User zu suchen nach der Email oder dem Namen und rückmeldung mit eigen definierten status Codes und Dem Value(User)
        public LogikReturn<UserModel> Find(string userId)
        {
            try
            {
                var result = userId.Contains("@") ? _db.LoadRecordByEmail<UserModel>("Users", userId) : _db.LoadRecordByUserName<UserModel>("Users", userId);
                if (result != null)
                    return new LogikReturn<UserModel>(ReturnStatus.Ok, result);
                else
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Fail, null);
                }
            }
            catch (Exception e)
            {
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }
        // Verifiszierung des Logins mit Hilfe von Bcrypt
        public LogikReturn<UserModel> VerifyLogin(string userId, string password)
        {
            try
            {
                // User aus Datenbank holen mit email oder name
                var result = userId.Contains("@") ? _db.LoadRecordByEmail<UserModel>("Users", userId) : _db.LoadRecordByUserName<UserModel>("Users", userId);
                if (result == null)
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Fail, null);
                }
                else if (BC.Verify(password, result.Password))
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Ok, result);
                }
                else
                    return new LogikReturn<UserModel>(ReturnStatus.Fail, null);
            }
            catch(Exception e)
            {
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }
        // Registrieren eines Users
        public LogikReturn<UserModel> Register(string username, string email, string password )
        {
            try
            {
                // Überprüfen ob Name oder Email schon vorhanden
                var Users = _db.LoadRecords<UserModel>("Users");
                var badList = Users.Where(u => u.UserName == username).ToList();
                badList.AddRange(Users.Where(u => u.Email == email).ToList());
                if (badList.Count > 0)
                {
                    return new LogikReturn<UserModel>(ReturnStatus.Existing, null);
                }
                //Erstellen und hinzufügen des neuen Benutzers
                var user = new UserModel()
                {
                    Email = email,
                    UserName = username,
                    Password = BC.HashPassword(password),
                    Meetings = new List<Guid>(),
                    AccessLevel = Roles.CUST
                };
                _db.InsertRecord<UserModel>("Users", new UserModel() 
                    { Email = email, 
                    UserName = username, 
                    Password = BC.HashPassword(password), 
                    Meetings = new List<Guid>(),
                    AccessLevel = Roles.CUST
                });
                return new LogikReturn<UserModel>(ReturnStatus.Ok, user);
            }catch(Exception e)
            {
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }
        // Updaten des Users
        public LogikReturn<UserModel> UpsertUser(UserModel model)
        {
            try
            {
                model.Password = BC.HashPassword(model.Password);
                model.ConfirmPassword = BC.HashPassword(model.ConfirmPassword);
                _db.UpsertRecord<UserModel>("Users", model.Id, model);
                return new LogikReturn<UserModel>(ReturnStatus.Ok, null);
            }
            catch (Exception e)
            {
                return new LogikReturn<UserModel>(ReturnStatus.DbError, null);
            }
        }
    }
}

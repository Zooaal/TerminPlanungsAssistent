using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Services;

namespace WebServer.Models
{
    public class UserModel
    {
        //[BsonId]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<Guid> Meetings { get; set; }
        public string AccessLevel { get; set; }
    }
}

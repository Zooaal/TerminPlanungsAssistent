using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class MeetingModel
    {
        // Angabe für Datenbank als ID
        [BsonId]
        public Guid Id { get; set; }
        // Angabe für Deutschzeit
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateTime { get; set; }
        public double TimeSpan { get; set; }
        public Boolean Taken { get; set; }
    }
}

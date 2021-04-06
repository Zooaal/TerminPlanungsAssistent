using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class MeetingModel
    {
        [BsonId]
        public Guid ID { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateTime { get; set; }
        public double TimeSpan { get; set; }
        public Boolean Taken { get; set; }
    }
}

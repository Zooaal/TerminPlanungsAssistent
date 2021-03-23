using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Razor_Pages_Web.Models
{
    public class MeetingModel
    {
        [BsonId]
        public Guid ID { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public Boolean Taken { get; set; }
    }
}

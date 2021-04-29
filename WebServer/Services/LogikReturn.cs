using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Services
{
    // Return Typ mit rückgabestatus und dem möglichen Value
    public class LogikReturn<TValue>
    {
        public LogikReturn(ReturnStatus returnStatus, TValue value)
        {           
            Value = value;
            ReturnStatus = returnStatus;
        }
        public TValue Value { get; }
        public ReturnStatus ReturnStatus { get; }
    }
    public enum ReturnStatus
    {
        Ok,
        DbError,
        Fail,
        Existing
    }
}

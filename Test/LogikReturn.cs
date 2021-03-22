using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
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

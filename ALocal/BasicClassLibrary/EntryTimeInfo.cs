using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryTimeInfo : IEntryTimeInfo
    {
        private DayOfWeek _broadcastWeekday;
        private DateTime _broadcastTime;

        public DayOfWeek BroadcastWeekday
        {
            get { return _broadcastWeekday; }
            set { _broadcastWeekday = value; }
        }

        public DateTime BroadcastTime
        {
            get { return _broadcastTime; }
            set { _broadcastTime = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
   public class EntryTimeInfoManager:Manager<EntryTimeInfo>
    {
        //构造函数
        public EntryTimeInfoManager() : base(new AppDbContext()) { }


        //根据条目ID查询
        public readonly static Func<int, Func<EntryTimeInfo, bool> > ByEntryId =
            (entryId => (eti => eti.EntryId == entryId));
 
        //根据播放星期来查询
        public readonly static Func<DayOfWeek,Func<EntryTimeInfo, bool> > ByBroadcastWeekday =
           (weekday => (eti => eti.BroadcastWeekday == weekday));

        // 根据精确时间点查询（只比较时间部分）
        public readonly static Func<DateTime, Func<EntryTimeInfo, bool>> ByExactBroadcastTime =
            time => eti => eti.BroadcastTime.TimeOfDay == time.TimeOfDay;

        // 根据时间段查询（如23:00-23:59）
        public readonly static Func<TimeSpan, TimeSpan, Func<EntryTimeInfo, bool>> ByBroadcastTimeRange =
            (start, end) => eti => eti.BroadcastTime.TimeOfDay >= start && eti.BroadcastTime.TimeOfDay <= end;

    }
}

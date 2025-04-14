using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEntryTimeInfo : IEntryNavigation
    {
        // 获取和设置播出星期的属性，使用枚举类型来表示星期
        DayOfWeek BroadcastWeekday { get; set; }
        // 获取和设置播出时间的属性，这里使用TimeSpan来表示时间
        TimeSpan BroadcastTime { get; set; }
    }

}

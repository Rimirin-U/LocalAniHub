using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    //public partial class AppDbContext : DbContext
    //{
    //    public DbSet<EntryTimeInfo> EntryTimeInfos{ get; set; }
    //}
    public class EntryTimeInfo : IEntryNavigation,IEntityWithId
    {
        public int Id { get; set; }//主键
        public int? EntryId { get; set; }//外键
        public Entry? Entry { get; set; }//导航属性
        public DayOfWeek BroadcastWeekday { get; set; }// 播出星期
        public DateTime BroadcastTime { get; set; }//播出时间点（23：56）
        // 构造函数，初始化所有必要属性
        public EntryTimeInfo(int? entryId, Entry? entry, DayOfWeek broadcastWeekday, DateTime broadcastTime)
        {
            EntryId = entryId;
            Entry = entry;
            BroadcastWeekday = broadcastWeekday;
            BroadcastTime = broadcastTime;
        }
    }
    //关于数据库的上下文
    public partial class AppDbContext : DbContext
    {
        public DbSet<EntryTimeInfo> EntryTimeInfos { get; set; }
    }
}

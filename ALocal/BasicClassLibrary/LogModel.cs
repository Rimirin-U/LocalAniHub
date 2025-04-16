using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    // 日志基类
    public abstract class LogEntry
    {
        //public DateTime Timestamp { get; set; }
    }

    // 观看记录
    public class WatchLogEntry : LogEntry
    {
        public int  EpisodeId { get; set; }
    }

    // 评价记录
    public class ReviewLogEntry : LogEntry
    {
        public int  NoteId { get; set; }
    }

    // 评分记录
    public class RatingLogEntry : LogEntry
    {
        public int  EntryId { get; set; }
        public int Score { get; set; }
    }
}

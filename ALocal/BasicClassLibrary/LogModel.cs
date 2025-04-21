using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    //修改后
    public abstract class LogEntry
    {
        public DateTimeOffset Timestamp { get; } // 时间戳
        public string LogType { get; }          // 日志类型
        public string RenderedMessage { get; internal set; } // 渲染后的日志消息
        public string Properties { get; internal set; } // 附加属性信息

        // 构造函数
        protected LogEntry(DateTimeOffset timestamp, string logType)
        {
            Timestamp = timestamp; // 初始化时间戳
            LogType = logType;     // 初始化日志类型
            RenderedMessage = "";  // 初始化渲染后的日志消息
            Properties = "";       // 初始化附加属性信息
        }
    }

    // 观看日志条目类
    public class WatchLogEntry : LogEntry
    {
        public int EpisodeId { get; } // 剧集 ID

        public WatchLogEntry(int episodeId)
            : base(DateTimeOffset.UtcNow, "WatchLogEntry") // 调用父类构造函数
        {
            EpisodeId = episodeId; // 初始化剧集 ID
        }
    }

    // 评价日志条目类
    public class ReviewLogEntry : LogEntry
    {
        public int NoteId { get; } // 笔记 ID

        public ReviewLogEntry(int noteId)
            : base(DateTimeOffset.UtcNow, "ReviewLogEntry") // 调用父类构造函数
        {
            NoteId = noteId;       // 初始化笔记 ID
        }
    }

    // 评分日志条目类
    public class RatingLogEntry : LogEntry
    {
        public int EntryId { get; } // 条目 ID
        public int Score { get; }   // 评分

        public RatingLogEntry(int entryId, int score)
            : base(DateTimeOffset.UtcNow, "RatingLogEntry") // 调用父类构造函数
        {
            EntryId = entryId;     // 初始化条目 ID
            Score = score;         // 初始化评分
        }
    }
    /*
    //修改前
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
    }*/
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class LogManager : Manager<LogEntry>
    {
        public LogManager(AppDbContext context) : base(context) { }

        // 添加观影日志
        public void AddWatchLog(int episodeId)
        {
            var logEntry = new WatchLogEntry { EpisodeId = episodeId };
            Add(logEntry);
        }

        // 添加评价日志
        public void AddReviewLog(int noteId)
        {
            var logEntry = new ReviewLogEntry { NoteId = noteId };
            Add(logEntry);
        }

        // 添加评分日志
        public void AddRatingLog(int entryId, int score)
        {
            var logEntry = new RatingLogEntry { EntryId = entryId, Score = score };
            Add(logEntry);
        }

        // 查询日志（支持可选参数）
        public IEnumerable<LogEntry> GetLogs(
            DateTimeOffset? startTime = null,
            DateTimeOffset? endTime = null,
            string? logType = null)
        {
            // 构建查询条件
            Func<LogEntry, bool> predicate = log =>
                (!startTime.HasValue || log.Timestamp >= startTime.Value) &&
                (!endTime.HasValue || log.Timestamp <= endTime.Value) &&
                (string.IsNullOrEmpty(logType) || log.LogType == logType);

            return Query(predicate);
        }
        // 查询指定类型的最早日志
        public LogEntry? FindEarliestByType(string logType)
        {
            return context.Logs
                .Where(le => le.LogType == logType)
                .OrderBy(le => le.Timestamp)
                .FirstOrDefault();
        }

        // 查询指定类型的最晚日志
        public LogEntry? FindLatestByType(string logType)
        {
            return context.Logs
                .Where(le => le.LogType == logType)
                .OrderByDescending(le => le.Timestamp)
                .FirstOrDefault();
        }
    }
}

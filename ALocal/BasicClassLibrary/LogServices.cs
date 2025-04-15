using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SQLite;
using Serilog.Context;
using Serilog.Parsing;

namespace BasicClassLibrary
{
    // 日志记录服务
    public static class LoggerService
    {
        // 静态日志记录器实例
        private static readonly ILogger _logger = LoggerConfig.CreateLogger();
        // 预定义常用模板提升性能
        private static readonly MessageTemplate _baseTemplate
            = new MessageTemplateParser().Parse("[{LogType}] {Data}");
        // 记录观看日志方法
        public static void LogWatch(int episodeId)
        {
            // 创建观看日志条目对象
            var logEntry = new WatchLogEntry { EpisodeId = episodeId };
            // 调用内部记录方法，指定日志类型为"Watch"
            LogWithType("Watch", logEntry);
        }

        public static void LogReview(int noteId)
        {
            var logEntry = new ReviewLogEntry { NoteId = noteId };
            LogWithType("Review", logEntry);
        }

        public static void LogRating(int entryId, int score)
        {
            var logEntry = new RatingLogEntry { EntryId = entryId, Score = score };
            LogWithType("Rating", logEntry);
        }
        // 通用日志记录方法（泛型约束T必须继承自LogEntry）
        private static void LogWithType<T>(string logType, T logEntry) where T : LogEntry
        {
            var logEvent = new LogEvent(
                DateTimeOffset.Now,  // 当前时间作为时间戳
                LogEventLevel.Information,// 日志级别设为Information
                null,      // 异常对象（无异常时为null
                _baseTemplate,
                new List<LogEventProperty>// 日志属性列表
                {
                    new LogEventProperty("LogType", new ScalarValue(logType)),// 添加LogType属性
                    new LogEventProperty("Data", new ScalarValue(logEntry)) // 添加数据对象
                });
            // 使用日志上下文推入LogType属性
            using (LogContext.PushProperty("LogType", logType))
            {
                // 写入日志事件
                _logger.Write(logEvent);
            }
        }
        // 日志查询服务
        // 获取日志的泛型方法
        public static List<T> GetLogs<T>(string logType, DateTime? start = null, DateTime? end = null) where T : LogEntry
        {
            // 数据库连接字符串
            const string connectionString = "Data Source=logs.db";
            // 构建基础查询语句
            var query = new StringBuilder("SELECT * FROM Logs WHERE LogType = @logType");
            // 参数列表（初始包含logType参数）
            var parameters = new List<SQLiteParameter> { new SQLiteParameter("@logType", logType) };
            // 添加时间范围过滤条件
            if (start.HasValue)
            {
                query.Append(" AND Timestamp >= @start");
                parameters.Add(new SQLiteParameter("@start", start.Value));
            }

            if (end.HasValue)
            {
                query.Append(" AND Timestamp <= @end");
                parameters.Add(new SQLiteParameter("@end", end.Value));
            }
            // 准备返回结果列表
            var logs = new List<T>();
            // 使用数据库连接
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                // 创建命令对象
                using (var cmd = new SQLiteCommand(query.ToString(), conn))
                {
                    // 添加参数集合
                    cmd.Parameters.AddRange(parameters.ToArray());
                    // 执行查询
                    using (var reader = cmd.ExecuteReader())
                    {
                        // 遍历结果集
                        while (reader.Read())
                        {
                            //从LogEvent列获取JSON数据
                            var json = reader["LogEvent"].ToString();
                            // 反序列化为指定类型对象
                            var logEntry = JsonConvert.DeserializeObject<T>(json);
                            // 设置时间戳（从数据库原始字段获取）
                            logEntry.Timestamp = Convert.ToDateTime(reader["Timestamp"]);
                            logs.Add(logEntry);
                            // 添加到结果列表
                        }
                    }
                }
            }
            return logs;
        }
    }
}

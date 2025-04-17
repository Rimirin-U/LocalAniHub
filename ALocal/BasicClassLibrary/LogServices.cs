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
using Microsoft.Data.Sqlite;
using System.Data;

namespace BasicClassLibrary
{
    //查询结果的完整信息:（包括时间戳、日志类型等）
    public class LogResult<T> where T : LogEntry
    {
        public DateTime Timestamp { get; set; }
        public required string LogType { get; set; }
        public required T LogEntry { get; set; }
    }
    public static class LoggerService
    {
        private static readonly ILogger _logger;
        // 静态构造函数：在类首次加载时执行
        static LoggerService()
        {
            try
            {
                // 初始化数据库
                LoggerConfig.InitializeDatabase();

                // 创建日志记录器
                _logger = LoggerConfig.CreateLogger();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to initialize LoggerService: {ex.Message}");
                throw; // 抛出异常，让调用方处理
            }
        }
        // 记录观看日志
        public static void AddLogWatch(int episodeId)
        {
            var logEntry = new WatchLogEntry { EpisodeId = episodeId };
            Log(logEntry);
        }

        // 记录评价日志
        public static void AddLogReview(int noteId)
        {
            var logEntry = new ReviewLogEntry { NoteId = noteId };
            Log(logEntry);
        }

        // 记录评分日志
        public static void AddLogRating (int entryId, int score)
        {
            var logEntry = new RatingLogEntry { EntryId = entryId, Score = score };
            Log(logEntry);
        }
        // 通用日志记录方法（使用泛型 T 自动推断日志类型）
        private static void Log<T>(T logEntry) where T : LogEntry
        {
            try
            {
                // 使用 T 的类型名称作为日志类型
                string logType = typeof(T).Name;
                //构造日志事件
                var logEvent = new LogEvent(
                    DateTimeOffset.Now,//当前时间戳
                    Serilog.Events.LogEventLevel.Information,//日志级别为 Information
                    null,
                    new MessageTemplateParser().Parse("[{LogType}] {Data}"),//日志模板为 [LogType] Data。
                    new List<LogEventProperty>//包含两个属性：LogType 和 Data
                    {
                    new LogEventProperty("LogType", new ScalarValue(logType)),
                    new LogEventProperty("Data", new ScalarValue(logEntry))
                    });

                using (LogContext.PushProperty("LogType", logType))
                {
                    _logger.Write(logEvent);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log event of type {typeof(T).Name}: {ex.Message}");
            }
        }
        // 查询日志（使用泛型 T 自动推断日志类型）
        public static List<LogResult<T>> GetLogs<T>(DateTime? start = null, DateTime? end = null) where T : LogEntry
        {
            const string connectionString = "Data Source=logs.db";
            // var logs = new List<T>();
            var logs = new List<LogResult<T>>();

            try
            {
                // 使用 T 的类型名称作为日志类型
                string logType = typeof(T).Name;
                var query = BuildQuery(logType, start, end, out var parameters);

                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                /*var json = reader["LogEvent"]?.ToString();
                                if (!string.IsNullOrEmpty(json) && TryParseLogEntry<T>(json, reader, out var logEntry))
                                {
                                    logs.Add(logEntry);
                                }*/
                                var timestamp = reader["Timestamp"] != DBNull.Value ? (DateTime)reader["Timestamp"] : default;
                                var json = reader["LogEvent"]?.ToString();
                                if (!string.IsNullOrEmpty(json) && TryParseLogEntry<T>(json, reader, out var logEntry))
                                {
                                    logs.Add(new LogResult<T>
                                    {
                                        Timestamp = timestamp,
                                        LogType = logType,
                                        LogEntry = logEntry
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error retrieving logs for type {typeof(T).Name}: {ex.Message}");
            }

            return logs;
        }

        // 构建查询字符串
        private static string BuildQuery(string logType, DateTime? start, DateTime? end, out List<SQLiteParameter> parameters)
        {
            var query = new StringBuilder("SELECT * FROM Logs WHERE LogType = @logType");
            parameters = new List<SQLiteParameter> { new SQLiteParameter("@logType", DbType.String) { Value = logType } };

            if (start.HasValue)
            {
                query.Append(" AND Timestamp >= @start");
                parameters.Add(new SQLiteParameter("@start", DbType.DateTime) { Value = start.Value });
            }

            if (end.HasValue)
            {
                query.Append(" AND Timestamp <= @end");
                parameters.Add(new SQLiteParameter("@end", DbType.DateTime) { Value = end.Value });
            }

            return query.ToString();
        }

        // 反序列化日志条目
        private static bool TryParseLogEntry<T>(string json, SQLiteDataReader reader, out T logEntry) where T : LogEntry
        {
            logEntry = null;

            try
            {
                logEntry = JsonConvert.DeserializeObject<T>(json);
                return logEntry != null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to parse log entry: {ex.Message}");
            }

            return false;
        }
    }
}
/*
 假设程序运行成功，SQLite 数据库文件 logs.db 中将生成如下数据：
 
Id	Timestamp	        LogType	        LogEvent
1	2025-04-16 15:49:00	WatchLogEntry	{"EpisodeId": 123}
2	2025-04-16 16:00:00	ReviewLogEntry	{"NoteId": 456}
3	2025-04-16 16:10:00	RatingLogEntry	{"EntryId": 789, "Score": 5}

 */


// 日志记录服务
/*public static class LoggerService
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
}*/


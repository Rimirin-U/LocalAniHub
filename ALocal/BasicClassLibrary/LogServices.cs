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
    //修改后
    public class LoggerService
    {
        private static ILogger _logger;

        // 静态构造函数，初始化日志服务
        static LoggerService()
        {
            try
            {
                _logger = LoggerConfig.CreateLogger(); // 创建日志器
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to initialize LoggerService: {ex.Message}");
                throw; // 抛出异常以便调用方处理
            }
        }

        // 添加观看日志
        public static void AddLogWatch(int episodeId)
        {
            var logEntry = new WatchLogEntry(episodeId); // 创建观看日志条目
            Log(logEntry); // 记录日志
        }

        // 添加评价日志
        public static void AddLogReview(int noteId)
        {
            var logEntry = new ReviewLogEntry(noteId); // 创建评价日志条目
            Log(logEntry); // 记录日志
        }

        // 添加评分日志
        public static void AddLogRating(int entryId, int score)
        {
            var logEntry = new RatingLogEntry(entryId, score); // 创建评分日志条目
            Log(logEntry); // 记录日志
        }
        private static void Log<T>(T logEntry) where T : LogEntry
        {
            try
            {
                string logType = typeof(T).Name; // 获取日志类型（类名）
                string renderedMessage = $"[{logType}] {logEntry.Timestamp}"; // 设置 RenderedMessage
                string properties = JsonConvert.SerializeObject(new { EpisodeId = (logEntry as WatchLogEntry)?.EpisodeId, NoteId = (logEntry as ReviewLogEntry)?.NoteId, EntryId = (logEntry as RatingLogEntry)?.EntryId, Score = (logEntry as RatingLogEntry)?.Score }); // 序列化附加属性信息

                // 使用 Serilog 写入日志
                _logger.ForContext("Level", "Information") // 假设所有日志都是 Information 级别
                       .ForContext("Exception", "") // 如果没有异常，可以留空
                       .ForContext("RenderedMessage", renderedMessage)
                       .ForContext("Properties", properties)
                       .Information("{RenderedMessage}", renderedMessage);

                Console.WriteLine("Log entry successfully written to database.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to log event of type {typeof(T).Name}: {ex.Message}");
                Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public static List<T> GetLogs<T>(DateTime? startDate = null, DateTime? endDate = null) where T : LogEntry
        {
            var logs = new List<T>(); // 存储查询结果

            using (var conn = new SQLiteConnection(LoggerConfig.connectionString)) // 创建数据库连接
            {
                conn.Open(); // 打开连接
                using (var cmd = conn.CreateCommand()) // 创建命令对象
                {
                    // 构建动态 SQL 查询
                    var sql = @"
                    SELECT * FROM Logs 
                    WHERE Properties LIKE @logType"; // 注意：这里需要更新为更精确的匹配方式

                    if (startDate.HasValue)
                    {
                        sql += " AND Timestamp >= @startDate"; // 添加起始时间条件
                    }

                    if (endDate.HasValue)
                    {
                        sql += " AND Timestamp <= @endDate"; // 添加结束时间条件
                    }

                    cmd.CommandText = sql; // 设置 SQL 查询语句

                    // 绑定日志类型参数
                    cmd.Parameters.AddWithValue("@logType", $"%\"{typeof(T).Name}\"%");

                    // 绑定起始时间参数（如果存在）
                    if (startDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate.Value);
                    }

                    // 绑定结束时间参数（如果存在）
                    if (endDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@endDate", endDate.Value);
                    }

                    using (var reader = cmd.ExecuteReader()) // 执行查询
                    {
                        while (reader.Read()) // 遍历查询结果
                        {
                            var logEventJson = reader["Properties"].ToString(); // 获取 Properties 字段的值

                            var logEntry = DeserializeLogEntry<T>(logEventJson); // 反序列化日志事件
                            if (logEntry != null) // 确保反序列化结果非空
                            {
                                logs.Add(logEntry); // 添加到结果列表
                            }
                        }
                    }
                }
            }

            return logs; // 返回查询结果
        }

        // 反序列化日志事件
        private static T? DeserializeLogEntry<T>(string json) where T : LogEntry
        {
            try
            {
                var logEntry = JsonConvert.DeserializeObject<T>(json); // 将 JSON 转换为对象
                if (logEntry == null)
                {
                    throw new InvalidOperationException("Deserialized log entry is null.");
                }
                return logEntry;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to deserialize log entry: {ex.Message}");
                return default; // 返回默认值（null）
            }
        }
    }
    /*
    //修改前
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
    }*/
}


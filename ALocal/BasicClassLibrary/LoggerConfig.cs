using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SQLite;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Data.SQLite;

namespace BasicClassLibrary
{
    //修改后
    public class LoggerConfig
    {
        internal static readonly string connectionString = "Data Source=logs.db";

        // 配置 Serilog 日志器，写入 SQLite 数据库
        public static ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug() // 设置最低日志级别为 Debug
                .WriteTo.SQLite(
                    sqliteDbPath: Path.Combine(AppContext.BaseDirectory, "logs.db"), // 使用绝对路径
                    tableName: "Logs",      // 日志表名
                    storeTimestampInUtc: true, // 使用 UTC 时间戳
                    batchSize: 1 // 批量写入大小
                )
                .CreateLogger(); // 创建并返回日志器
        }
        //如果表不存在，则自动创建，如果表已存在，则直接写入日志
    }
    /*
    //修改前
     public class LoggerConfig
     {
         // 初始化数据库并手动创建表
         public static void InitializeDatabase()
         {
             const string connectionString = "Data Source=logs.db";
             const string createTableQuery = @"
             CREATE TABLE IF NOT EXISTS Logs (
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 Timestamp DATETIME NOT NULL DEFAULT (datetime('now')), -- 自动填充时间戳
                 LogType TEXT NOT NULL,
                 LogEvent TEXT NOT NULL
             );";

             try
             {
                 using (var conn = new SQLiteConnection(connectionString))
                 {
                     conn.Open();
                     using (var cmd = new SQLiteCommand(createTableQuery, conn))
                     {
                         cmd.ExecuteNonQuery();
                     }
                 }
             }
             catch (Exception ex)
             {
                 Console.Error.WriteLine($"Failed to initialize database: {ex.Message}");
                 throw; // 抛出异常，让调用方处理
             }
         }

         // 创建日志记录器
         public static ILogger CreateLogger()
         {
             return new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .Enrich.WithProperty("LogType", "") // 默认值为空字符串
                 .WriteTo.SQLite(
                     sqliteDbPath: "logs.db",
                     tableName: "Logs",
                     storeTimestampInUtc: true
                 )
                 .CreateLogger();
         }
     }*/
}




    

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
    //后续这个文件可能需要看情况修改，因为自动建表一直存在问题，所以我就用了手动建表
    //如果后续存在问题，再想办法修改回自动建表
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
     }
}


   /* public class LoggerConfig
    {
        public static ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("LogType", "")
                .WriteTo.SQLite(
                    sqliteDbPath: "logs.db",
                    tableName: "Logs",
                    storeTimestampInUtc: true,
                    columnOptions: new ColumnOptions
                    {
                        AdditionalColumns = new Collection<SqlColumn>
                        {
                                new SqlColumn("LogType", SqlDbType.Text) // 使用正确的 SQLite 数据类型
                        }
                    },
                    autoCreateSqlTable: true
                )
                .CreateLogger();
        }
    }*/



    

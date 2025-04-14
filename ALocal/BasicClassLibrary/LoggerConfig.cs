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

namespace BasicClassLibrary
{
    public class LoggerConfig
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
                    maxDatabaseSize: 100,
                    columnOptions: new ColumnOptions//这有个bug一直解决不了
                    {
                        Store = { 
                            StandardColumn.TimeStamp,
                            StandardColumn.Level,
                            StandardColumn.Message,
                            StandardColumn.LogEvent },

                        AdditionalColumns = new Collection<SqlColumn>
                        {
                            new SqlColumn("LogType", (System.Data.SqlDbType)SqliteType.Text)
                        }
                    },
                    autoCreateSqlTable: true
                    )
                .CreateLogger();
        }
    }
}

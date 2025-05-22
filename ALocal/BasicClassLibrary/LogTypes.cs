﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    // 定义日志基类
    public abstract class LogEntry : IEntityWithId
    {
        public int Id { get; set; } // 主键
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow; // 默认为 UTC 时间
        public string? LogType { get; protected set; } // 日志类型
    }

    // 观影日志
    public class WatchLogEntry : LogEntry
    {
        public int EpisodeId { get; set; }

        public WatchLogEntry()
        {
            LogType = "Watch";
        }
    }

    // 评价日志
    public class ReviewLogEntry : LogEntry
    {
        public int NoteId { get; set; }

        public ReviewLogEntry()
        {
            LogType = "Review";
        }
    }

    // 评分日志
    public class RatingLogEntry : LogEntry
    {
        public int EntryId { get; set; }
        public int Score { get; set; }

        public RatingLogEntry()
        {
            LogType = "Rating";
        }
    }

    //数据库上下文部分
    public partial class AppDbContext : DbContext
    {
        public DbSet<LogEntry> Logs { get; set; }//日志表
       /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=logs.db");
        }*/

        
    }
}

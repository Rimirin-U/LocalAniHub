using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public partial class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "data");
            var dbPath = Path.Combine(dirPath, "data.db");

            // 确保目录存在
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //////////// Log ///////////
            // 配置表继承 (TPH)
            modelBuilder.Entity<LogEntry>()
                .HasDiscriminator<string>("LogType")
                .HasValue<WatchLogEntry>("Watch")
                .HasValue<ReviewLogEntry>("Review")
                .HasValue<RatingLogEntry>("Rating");

            // 配置表结构
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.Property(e => e.Timestamp).HasColumnType("TEXT"); // 存储为 ISO 8601 格式的字符串
            });

            modelBuilder.Entity<WatchLogEntry>(entity =>
            {
                entity.Property(e => e.EpisodeId).IsRequired();
            });

            modelBuilder.Entity<ReviewLogEntry>(entity =>
            {
                entity.Property(e => e.NoteId).IsRequired();
            });

            modelBuilder.Entity<RatingLogEntry>(entity =>
            {
                entity.Property(e => e.EntryId).IsRequired();
                entity.Property(e => e.Score).IsRequired();
            });


            //////////// Entry ///////////
            // 添加 Entry 实体的 KeyWords 字段转换
            modelBuilder.Entity<Entry>()
                .Property(e => e.KeyWords)
                .HasConversion(
                    v => string.Join(',', v), // 写入数据库时转为字符串
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() // 读取数据库时转回 List<string>
                );
        }
    }
}

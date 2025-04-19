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
    }
}

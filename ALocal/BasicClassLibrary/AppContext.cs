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
        public DbSet<Entry> Entries { get; set; }
        public DbSet<EntryMetadata> EntryMetadata { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "data/data.db");
            optionsBuilder.UseSqlite($"Data source={path}");
        }
    }
}

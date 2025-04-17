using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryRating: IEntryNavigation, IEntityWithId
    {
        public int Id { get; set; } // 主键
        public int? EntryId { get; set; } // 外键
        public Entry? Entry { get; set; } // 导航属性
        public double? Score { get; set; } // 评分
    }
    public partial class AppDbContext : DbContext
    {
        public DbSet<EntryRating> EntryRatings { get; set; }
    }
}

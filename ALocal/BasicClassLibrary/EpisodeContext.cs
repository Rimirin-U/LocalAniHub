using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public partial class AppContext : DbContext
    {
        public DbSet<Episode> Episodes { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EpisodeManager : Manager<Episode>
    {
        public EpisodeManager() : base(new AppDbContext()) { }

        public readonly static Func<int, Func<Episode, bool>> ByEntryId = (entryId =>
                                                                          (ep => ep.EntryId == entryId));
    }
}
}

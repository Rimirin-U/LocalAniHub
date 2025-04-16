using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryRatingManager : Manager<EntryRating>
    {
        public EntryRatingManager() : base(new AppDbContext()) { }

        public static Func<EntryRating, bool> ByEntryId(int entryId) => (o => o.EntryId == entryId);
        public static readonly Func<EntryRating, bool> All = (o => true);
    }
}
}

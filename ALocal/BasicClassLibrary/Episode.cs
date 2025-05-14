using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Episode : IEntityWithId,IEntryNavigation
    {
        // Constructor
        public Episode(int? entryId, Entry? entry, int episodeNumber, State state)
        {
            EntryId = entryId;
            Entry = entry;
            EpisodeNumber = episodeNumber;
            State = state;
        }

        public int Id { get; set; }

        // Navigation Properties
        public int? EntryId { get; set; }
        public Entry? Entry { get; set; }

        // Properties
        public int EpisodeNumber { get; set; }
        public State State { get; set; }            // GivenUp视作NotWatched
        public string ShortComment { get; set; } 
    }

    public partial class AppDbContext : DbContext
    {
        public DbSet<Episode> Episodes { get; set; }
    }
}

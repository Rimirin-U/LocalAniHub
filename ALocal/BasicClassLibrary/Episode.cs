using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Episode : IEntryNavigation
    {
        // Constructor
        public Episode(int id, int? entryId, Entry? entry, int episodeNumber)
        {
            Id = id;
            EntryId = entryId;
            Entry = entry;
            EpisodeNumber = episodeNumber;
        }

        public int Id { get; set; }

        // Navigation Properties
        public int? EntryId { get; set; }
        public Entry? Entry { get; set; }

        // Properties
        public int EpisodeNumber { get; set; }
    }
}

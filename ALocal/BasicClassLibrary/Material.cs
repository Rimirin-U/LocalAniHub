using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Material : IEntryNavigation
    {
        // Constructor
        public Material(int? entryId, Entry? entry, string name, string kind, string path)
        {
            EntryId = entryId;
            Entry = entry;
            this.name = name;
            this.kind = kind;
            this.path = path;
        }

        // Navigation Properties
        public int? EntryId { get; set; }
        public Entry? Entry { get; set; }

        // Properties
        public string name { get; set; }
        public string kind { get; set; }
        public string path { get; set; }
    }
}

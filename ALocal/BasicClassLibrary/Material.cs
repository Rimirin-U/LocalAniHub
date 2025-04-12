using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Material : IEntryNavigation
    {
        public Material(int? entryId, Entry? entry, string name, string kind, string path)
        {
            EntryId = entryId;
            Entry = entry;
            this.name = name;
            this.kind = kind;
            this.path = path;
        }

        public int? EntryId { get; set; }
        public Entry? Entry { get; set; }

        public string name { get; set; }
        public string kind { get; set; }
        public string path { get; set; }
    }
}

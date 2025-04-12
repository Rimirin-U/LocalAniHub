using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEntriesNavigation
    {
        public ICollection<int> EntriesId { get; set; }
        public ICollection<Entry> Entries { get; set; }
    }
}

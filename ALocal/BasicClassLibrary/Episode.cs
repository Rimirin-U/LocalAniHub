using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Episode : IEntryNavigation
    {
        public int? EntryId { get; set; }
        public Entry? Entry { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEntrySearch
    {
        public List<EntryInfoSet> search();
    }
}

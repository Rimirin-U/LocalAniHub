using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEntryFetch
    {
        public Task<List<EntryInfoSet>> FetchAsync();
    }
}

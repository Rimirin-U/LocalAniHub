using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEpisodesNavigation
    {
        public ICollection<int> EpisodesId { get; set; }
        public ICollection<Episode> Episodes { get; set; }
    }
}

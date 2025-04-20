using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class TrackerManager : Manager<Tracker>
    {
        public TrackerManager() : base(new AppDbContext()) { }
    }
}

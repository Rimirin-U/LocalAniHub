using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//条目评分
namespace BasicClassLibrary
{
    internal interface IEntryRating : IEntryNavigation
    {
        double? rating { get; set; }
    }
}

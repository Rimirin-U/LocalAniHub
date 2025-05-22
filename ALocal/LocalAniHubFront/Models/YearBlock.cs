using LocalAniHubFront.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalAniHubFront.Models
{
    public class YearBlock
    {
        public string Year { get; set; }
        public ObservableCollection<EntryLine> EntryLines { get; set; }
    }
}

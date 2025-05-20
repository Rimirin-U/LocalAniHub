using BasicClassLibrary;
using LocalAniHubFront.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class SearchViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<EntryInfoSet> entryInfoSets = new();
        // 所有EntryInfoSet的列表
        // 更改查询时间时应重置该列表

        public SearchViewModel()
        {
            // debug
            EntryInfoSets.Add(new("", "", new(), "", Image.FromFile(@"D:\reFSC\ConsoleApp1\UiDesktopApp1\Views\Windows\mygo.jpg"), new Dictionary<string, string>()));
            EntryInfoSets.Add(new("", "", new(), "", Image.FromFile(@"D:\reFSC\ConsoleApp1\UiDesktopApp1\Views\Windows\mygo.jpg"), new Dictionary<string, string>()));
            EntryInfoSets.Add(new("", "", new(), "", Image.FromFile(@"D:\reFSC\ConsoleApp1\UiDesktopApp1\Views\Windows\mygo.jpg"), new Dictionary<string, string>()));
        }
    }
}

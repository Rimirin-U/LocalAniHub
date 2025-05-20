using LibVLCSharp.Shared;
using LocalAniHubFront.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Media;
using System.Drawing;
using BasicClassLibrary;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class EntryInfoCardViewModel: ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<KeyValuePair<string, string>> metadataDict = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(KvImageSource))]
        private Image kvImage;
        public ImageSource KvImageSource => ImageHelper.ToImageSource(KvImage); // 真正用于绑定

        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string subtitle;

        [ObservableProperty]
        private string date;

        [ObservableProperty]
        private string category;

        public EntryInfoCardViewModel(EntryInfoSet entryInfoSet)
        {
            // debug（请替换为实际实现，从EntryInfoSet中读取）
            MetadataDict.Add(new("企划", "BUSHIROAD"));
            MetadataDict.Add(new("导演", "柿本广大"));
            MetadataDict.Add(new("编剧", "绫奈由仁子"));
            MetadataDict.Add(new("音乐", "藤田淳平 藤间仁"));
            MetadataDict.Add(new("CV", "羊宫妃那 立石凛 青木阳菜 小日向美香 林鼓子"));
            KvImage = Image.FromFile(@"D:\reFSC\ConsoleApp1\UiDesktopApp1\Views\Windows\mygo.jpg");
            Title = "BanG Dream! It's MyGO!!!!!";
            Subtitle = "迷途之子!!!!!";
            Date = "2023.6.29";
            Category = "原创动画";
        }
    }
}

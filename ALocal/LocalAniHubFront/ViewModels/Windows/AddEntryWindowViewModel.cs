using LocalAniHubFront.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class AddEntryWindowViewModel : ObservableObject
    {
        public AddEntryWindowViewModel(/*EntryInfoSet entryInfoSet*/)
        {
            // debug
            MetadataFromEntryInfoSet.Add(new("系列构成", "绫奈由仁子"));
            MetadataFromEntryInfoSet.Add(new("企划", "BUSHIROAD"));
            MetadataFromEntryInfoSet.Add(new("动画制作", "SANZIGEN"));
            MetadataFromEntryInfoSet.Add(new("声优", "羊宫妃那 立石凛 青木阳菜 小日向美香 林鼓子"));
            MetadataFromEntryInfoSet.Add(new("导演", "柿本广大"));
            MetadataFromEntryInfoSet.Add(new("音乐", "藤田淳平 藤间仁"));
            KvImage = Image.FromFile(@"C:\Users\95842\Desktop\FSC\shumatsuTrain.jpg");
        }

        // 元数据（来自输入（EntryInfoSet））
        [ObservableProperty]
        private ObservableCollection<KeyValuePair<string, string>> metadataFromEntryInfoSet = new();

        // 元数据（用于构建EntryMetadata对象）
        [ObservableProperty]
        private ObservableCollection<KeyValuePair<string, string>> metadataItems = new();        

        [RelayCommand]
        private void AddEmptyPair()
        {
            MetadataItems.Add(new KeyValuePair<string, string>("", ""));
        }

        public void AddPair(KeyValuePair<string, string> pair)
        {
            MetadataItems.Add(pair);
        }

        public void ChangeKeyVisual(string filePath)
        {
            KvImage = Image.FromFile(filePath);
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(KvImageSource))]
        private Image kvImage;
        public ImageSource KvImageSource => ImageHelper.ToImageSource(KvImage); // 真正用于绑定
    }
}

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
            if (entryInfoSet == null)
                throw new ArgumentNullException(nameof(entryInfoSet));

            // 基础信息映射
            Title = entryInfoSet.TranslatedName ?? "无译名";
            Subtitle = entryInfoSet.OriginalName ?? string.Empty;
            Date = entryInfoSet.ReleaseDate.ToString("yyyy.M.d"); // 格式示例：2023.6.29
            Category = entryInfoSet.Category ?? "未分类";
            KvImage = entryInfoSet.KeyVisualImage; // 主视觉图

            // 处理元数据字典
            if (entryInfoSet.Metadata != null)
            {
                foreach (var kvp in entryInfoSet.Metadata)
                {
                        MetadataDict.Add(kvp);
                }
            }
        }
    }
}

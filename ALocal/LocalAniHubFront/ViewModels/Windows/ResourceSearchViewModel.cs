using BasicClassLibrary;
using LocalAniHub.ViewModels.Components;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LocalAniHub.ViewModels.Windows
{
    public partial class ResourceSearchViewModel : ObservableObject
    {
        public ObservableCollection<TextBoxItemViewModel> TextBoxes { get; } = new();

        [ObservableProperty]
        private ObservableCollection<ResourceItem> resourceItems;/* = new() { 
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][43][GB][1080P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl" },
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][43][GB][720P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl1" },
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][43][GB][2160P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl2" },
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][42][GB][1080P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl3" }
        };*/

        [ObservableProperty]
        private string windowTitle = "资源搜索: BanG Dream! Ave Mujica 第2集";// 示例

        [RelayCommand]
        private void AddTextBox()
        {
            var item = new TextBoxItemViewModel();
            item.RequestDelete = (self, _) => RemoveTextBox(self);
            TextBoxes.Add(item);
        }


        [RelayCommand]
        private void Search()
        {
            // ...
            // ... 以TextBoxes中的所有Text为List<string>，进行资源搜索，以填充resourceItems
        }

        [RelayCommand]
        private void AddDownload(string path)
        {
            // ...
            // 添加下载任务，添加成功后弹窗提示
            Debug.WriteLine($"AddDownloadCommand Called {path}");
        }

        private void RemoveTextBox(TextBoxItemViewModel item)
        {
            var index = TextBoxes.IndexOf(item);
            TextBoxItemViewModel? previous = index > 0 ? TextBoxes[index - 1] : null;
            TextBoxes.Remove(item);
            RequestFocus?.Invoke(previous);
        }

        public Action<TextBoxItemViewModel?>? RequestFocus { get; set; }
    }
}

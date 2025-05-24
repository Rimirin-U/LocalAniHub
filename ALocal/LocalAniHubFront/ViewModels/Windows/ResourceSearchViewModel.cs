using BasicClassLibrary;
using LocalAniHub.ViewModels.Components;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace LocalAniHub.ViewModels.Windows
{
    public partial class ResourceSearchViewModel : ObservableObject
    {
        public ResourceSearchViewModel(int episodeId)
        {
            // ...
            // 需要初始化: WindowTitle
            // 需要初始化：默认添加的关键词，包括：原名、译名、KeyWords、集数的数字
        }
        private int episodeId;

        public ObservableCollection<TextBoxItemViewModel> TextBoxes { get; } = new();

        [ObservableProperty]
        private ObservableCollection<ResourceItem> resourceItems;/* = new() { 
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][43][GB][1080P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl" },
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][43][GB][720P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl1" },
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][43][GB][2160P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl2" },
            new() { Title = "[BeanSub][Kusuriya no Hitorigoto][42][GB][1080P][x264_AAC]", PubDate = new(2025, 5, 24), DownloadUrl = "testUrl3" }
        };*/

        [ObservableProperty]
        private string windowTitle;// 示例:"资源搜索: BanG Dream! Ave Mujica 第2集"

        // 关键词对应的TextBox的ViewModel的列表
        [RelayCommand]
        private void AddTextBox()
        {
            var item = new TextBoxItemViewModel();
            item.RequestDelete = (self, _) => RemoveTextBox(self);
            TextBoxes.Add(item);
            RequestFocus?.Invoke(item);
        }

        [RelayCommand]
        private void Search()
        {
            // ...
            // ... 以TextBoxes中的所有Text为List<string>，进行资源搜索，以填充resourceItems
        }

        [RelayCommand]
        private async Task AddDownload(ResourceItem resourceItem)
        {
            ResourceService resourceService = new(new(), new());
            await resourceService.StartDownloadResource(resourceItem, ResourceService.AfterDownload(episodeId));
            MessageBox.Show("已添加下载");
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

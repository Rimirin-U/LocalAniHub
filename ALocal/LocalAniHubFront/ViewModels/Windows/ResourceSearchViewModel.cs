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
            this.episodeId = episodeId;
            var episodeManager= new EpisodeManager();
            var _episode=episodeManager.FindById(episodeId);
            if (_episode == null)
            {
                throw new InvalidOperationException($"Episode with ID {episodeId} not found.");
            }
            var _entryId = _episode.EntryId;
            var entryManager = new EntryManager();
            var _entry = entryManager.FindById((int)_entryId);
            if (_entry == null)
            {
                throw new InvalidOperationException($"Entry for Episode ID {episodeId} not found.");
            }
            WindowTitle = $"资源搜索: {_entry.TranslatedName} 第{_episode.EpisodeNumber}集";
            var defaultKeywords = new List<string>
            {
                 _entry.OriginalName,
                 _entry.TranslatedName,
                 _episode.EpisodeNumber.ToString()
            };
            defaultKeywords.AddRange(_entry.KeyWords);
            foreach (var keyword in defaultKeywords.Distinct())
            {
                var item = new TextBoxItemViewModel { Text = keyword };
                item.RequestDelete = (self, _) => RemoveTextBox(self);
                TextBoxes.Add(item);
            }

        }
        private int episodeId;

        public ObservableCollection<TextBoxItemViewModel> TextBoxes { get; } = new();

        [ObservableProperty]
        private ObservableCollection<ResourceItem> resourceItems;
        private readonly ResourceSearch _resourceSearcher=new ResourceSearch();
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
        /*private void Search()
        {
            // ...
            // ... 以TextBoxes中的所有Text为List<string>，进行资源搜索，以填充resourceItems
        }*/
        private async Task Search()
        {
            var keywords = TextBoxes.Select(t => t.Text).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

            if (!keywords.Any())
            {
                MessageBox.Show("请输入至少一个关键词进行搜索。");
                return;
            }

            try
            {
                var results = await _resourceSearcher.SearchMultipleKeywordsAsync(keywords);
                ResourceItems = new ObservableCollection<ResourceItem>(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"搜索失败: {ex.Message}");
            }
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

using BasicClassLibrary;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;

namespace LocalAniHubFront.ViewModels
{
    public partial class EntryWindow_ResourceManageViewModel : ObservableObject, INavigationAware
    {
        // 当前条目ID（从导航参数获取）
        [ObservableProperty]
        private int _entryId;

        // 是否自动获取资源（绑定到ToggleSwitch）
        [ObservableProperty]
        private bool _isAutoFetch;

        // 是否自动清除资源（绑定到ToggleSwitch）
        [ObservableProperty]
        private bool _isAutoDelete;

        // 资源数据集合（绑定到ItemsControl）
        [ObservableProperty]
        private ObservableCollection<ResourceDisplayItem> _resourcesData = new();

        // 关联的EntryManager和ResourceManager
        private readonly EntryManager _entryManager = new();
        private readonly ResourceManager _resourceManager = new();
        private readonly EpisodeManager _episodeManager = new();

        // 命令定义
        public ICommand ResourcePlayCommand { get; }
        public ICommand ResourceDeleteCommand { get; }
        public ICommand AddResourceCommand { get; }

        public EntryWindow_ResourceManageViewModel(int entryId)
        {
            EntryId = entryId;

            // 初始化命令
            ResourcePlayCommand = new RelayCommand<int>(PlayResource);
            ResourceDeleteCommand = new RelayCommand<int>(DeleteResource);
            AddResourceCommand = new RelayCommand(AddResources);

            // 加载初始数据
            LoadEntryData();
            LoadResources();
        }

        // 加载条目数据（自动获取/清除设置）
        private void LoadEntryData()
        {
            var entry = _entryManager.FindById(EntryId);
            if (entry != null)
            {
                IsAutoFetch = entry.HasUpdateTime;
                IsAutoDelete = entry.AutoClearResources;
            }
        }

        // 加载资源数据
        private void LoadResources()
        {
            ResourcesData.Clear();

            // 获取当前条目所有集数
            var episodes = _episodeManager.Query(EpisodeManager.ByEntryId(EntryId));

            // 获取每集对应的资源
            foreach (var episode in episodes)
            {
                var resources = _resourceManager.Query(ResourceManager.ByEntryId(episode.Id));
                foreach (var resource in resources)
                {
                    ResourcesData.Add(new ResourceDisplayItem(
                        resource.Id,
                        episode.EpisodeNumber,
                        System.IO.Path.GetFileName(resource.ResourcePath ?? string.Empty)
                    ));
                }
            }
        }

        // 播放资源
        private void PlayResource(int resourceId)
        {
            var resource = _resourceManager.FindById(resourceId);
            if (resource != null && !string.IsNullOrEmpty(resource.ResourcePath))
            {
                // TODO: 调用播放器逻辑
                MessageBox.Show($"播放资源: {resource.ResourcePath}");
            }
        }

        // 删除资源
        private void DeleteResource(int resourceId)
        {
            if (MessageBox.Show("确定要删除此资源吗？", "确认删除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _resourceManager.DeleteResource(resourceId);
                LoadResources(); // 刷新列表
            }
        }

        // 添加资源
        private void AddResources()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "导入资源",
                Filter = "视频文件 (*.mp4;*.avi;*.mov;*.mkv;*.wmv;*.flv)|*.mp4;*.avi;*.mov;*.mkv;*.wmv;*.flv",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    // 获取或创建对应的集数（这里简化处理，实际应根据文件名匹配集数）
                    int episodeNumber = ExtractEpisodeNumberFromFileName(filePath);
                    var episode = _episodeManager.Query(e => e.EntryId == EntryId && e.EpisodeNumber == episodeNumber)
                        .FirstOrDefault();

                    if (episode == null)
                    {
                        episode = new Episode(EntryId, null, episodeNumber);
                        _episodeManager.Add(episode);
                    }

                    // 创建资源记录
                    var resource = new Resource(
                        episode.Id,
                        episode,
                        DateTime.Now,
                        filePath
                    );
                    _resourceManager.Addresource(resource);
                }

                LoadResources(); // 刷新列表
            }
        }

        // 从文件名提取集数（示例实现）
        private int ExtractEpisodeNumberFromFileName(string fileName)
        {
            // 简单实现：尝试从文件名中匹配数字作为集数
            var match = System.Text.RegularExpressions.Regex.Match(fileName, @"\[(\d+)\]");
            return match.Success ? int.Parse(match.Groups[1].Value) : 1;
        }

        // INavigationAware 实现
        public Task OnNavigatedToAsync()
        {
            LoadEntryData();
            LoadResources();
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
    }

    // 资源显示项（用于绑定到UI）
    public record ResourceDisplayItem(
        int ResourceId,
        int EpisodeNumber,
        string ResourceName
    );
}

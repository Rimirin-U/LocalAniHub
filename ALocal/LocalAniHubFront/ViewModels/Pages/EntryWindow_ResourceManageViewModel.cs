using BasicClassLibrary;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;
using System.IO;

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
                var entry=_entryManager.FindById(EntryId);
                if (entry == null)
                {
                    MessageBox.Show("条目不存在，无法添加资源");
                    return;
                }
                foreach (var filePath in openFileDialog.FileNames)
                {
                    try
                    {
                        int episodeNumber = ExtractEpisodeNumberFromFileName(filePath);
                
                        // 验证集数是否在合法范围内
                        if (episodeNumber < 1 || episodeNumber > entry.EpisodeCount)
                        {
                            MessageBox.Show($"文件 {Path.GetFileName(filePath)} 的集数 {episodeNumber} 超出范围 (1-{entry.EpisodeCount})", 
                                          "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                            continue;
                        }

                        // 获取或创建集数记录
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
                    catch (Exception ex)
                    {
                        MessageBox.Show($"处理文件 {Path.GetFileName(filePath)} 时出错: {ex.Message}", 
                                      "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                LoadResources(); // 刷新列表
            }
        }

        // 从文件名提取集数
        private int ExtractEpisodeNumberFromFileName(string fileName)
        {
            // 优先匹配常见字幕组命名格式中的集数（如[01]、EP01等）
            var match = System.Text.RegularExpressions.Regex.Match(fileName,
                @"(?:\[|EP|ep)(\d{1,3})(?:\]|\.|_)");

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            // 次优匹配：纯数字（确保不是其他数字如年份）
            match = System.Text.RegularExpressions.Regex.Match(fileName,
                @"\b(\d{1,3})\b(?!\d{4})"); // 排除4位数字（避免匹配年份）

            return match.Success ? int.Parse(match.Groups[1].Value) : 1; // 默认返回1
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

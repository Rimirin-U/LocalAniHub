using BasicClassLibrary;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Wpf.Ui.Abstractions.Controls;
using System.IO;
using System.Text.RegularExpressions;
using LocalAniHubFront.Views.Windows;
using LocalAniHubFront.ViewModels.Windows;

namespace LocalAniHubFront.ViewModels
{
    public partial class EntryWindow_ResourceManageViewModel : ObservableObject, INavigationAware
    {
        // 当前条目ID（从导航参数获取）
        [ObservableProperty]
        private int _entryId;
        // 标题
        [ObservableProperty]
        private string _subtitle = "默认标题";

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

        public EntryWindow_ResourceManageViewModel(int entryId)
        {
            EntryId = entryId;
            LoadData();
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

        private void LoadData()
        {
            try
            {
                var entry = _entryManager.FindById(EntryId);
                if (entry != null)
                {
                    Subtitle = entry.TranslatedName; // 设置副标题为条目译名
                    IsAutoFetch = entry.HasUpdateTime;
                    IsAutoDelete = entry.AutoClearResources;
                    LoadResources();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"数据加载失败: {ex.Message}");
            }
        }

        // 加载资源数据
        private void LoadResources()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ResourcesData.Clear();
                    var episodes = _episodeManager.Query(EpisodeManager.ByEntryId(EntryId));

                    foreach (var episode in episodes)
                    {
                        var resources = _resourceManager.Query(ResourceManager.ByEntryId(episode.Id));
                        foreach (var resource in resources.Where(r => !string.IsNullOrEmpty(r.ResourcePath)))
                        {
                            ResourcesData.Add(new ResourceDisplayItem(
                                resource.Id,
                                episode.EpisodeNumber,
                                Path.GetFileName(resource.ResourcePath!)
                            ));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"资源加载失败: {ex.Message}");
            }
        }

        [RelayCommand]
        private void PlayResource(int resourceId)
        {
            try
            {
                if (_resourceManager.FindById(resourceId)?.ResourcePath is not { } path || !File.Exists(path))
                {
                    MessageBox.Show("资源文件不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // 创建播放器窗口
                var playerWindow = new PlayerWindow(resourceId)
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                // 窗口关闭时保存观看进度
                playerWindow.Closing += (s, e) =>
                {
                    (playerWindow.DataContext as PlayerViewModel)?.OnWindowClosing();
                };

                playerWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"播放失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void DeleteResource(int resourceId)
        {
            if (MessageBox.Show("确定要删除此资源吗？", "确认删除", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _resourceManager.DeleteResource(resourceId);
                LoadResources(); // 刷新列表
            }
        }

        public void AddResources(string[] filePaths)
        {
            try
            {
                ProcessFiles(filePaths);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessFiles(IEnumerable<string> filePaths)
        {
            var entry = _entryManager.FindById(EntryId);
            if (entry == null) return;

            foreach (var filePath in filePaths)
            {
                try
                {
                    if (!File.Exists(filePath)) continue;

                    var episodeNumber = ExtractEpisodeNumber(filePath);
                    if (episodeNumber < 1 || episodeNumber > entry.EpisodeCount)
                    {
                        MessageBox.Show($"集数 {episodeNumber} 超出范围", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }

                    var episode = GetOrCreateEpisode(episodeNumber);
                    if (episode == null) continue;

                    var resource = new Resource(episode.Id, DateTime.Now, filePath);
                    _resourceManager.Addresource(resource);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"文件处理失败: {filePath}\n{ex.Message}");
                }
            }

            LoadResources();
        }

        private Episode? GetOrCreateEpisode(int episodeNumber)
        {
            var episode = _episodeManager.Query(e => e.EntryId == EntryId && e.EpisodeNumber == episodeNumber)
                .FirstOrDefault();

            if (episode == null)
            {
                episode = new Episode(EntryId,episodeNumber,State.NotWatched);
                _episodeManager.Add(episode);
            }

            return episode;
        }

        // 从文件名提取集数
        private static int ExtractEpisodeNumber(string fileName)
        {
            var match = Regex.Match(fileName, @"(?:\[|EP|ep|第)?(\d{1,3})(?:\]|集|话)?");
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

        [RelayCommand]
        private void SaveResourceName(ResourceDisplayItem item)
        {
            if (item == null) return;

            try
            {
                var resource = _resourceManager.FindById(item.ResourceId);
                if (resource?.ResourcePath is not { } oldPath || !File.Exists(oldPath)) return;

                var newName = item.ResourceName.Trim();
                if (string.IsNullOrEmpty(newName)) throw new ArgumentException("文件名不能为空");
                if (newName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) throw new ArgumentException("非法文件名");

                var newPath = Path.Combine(Path.GetDirectoryName(oldPath)!, newName);
                if (File.Exists(newPath)) throw new IOException("文件已存在");

                File.Move(oldPath, newPath);
                resource.ResourcePath = newPath;
                _resourceManager.Modify(resource);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var index = ResourcesData.IndexOf(item);
                    if (index >= 0)
                    {
                        ResourcesData[index] = item with { ResourceName = newName };
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重命名失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadResources(); // 恢复显示
            }
        }
    }

    // 资源显示项（用于绑定到UI）
    public record ResourceDisplayItem(
        int ResourceId,
        int EpisodeNumber,
        string ResourceName
    );
}

using BasicClassLibrary;
using System.Collections.ObjectModel;
using System.Timers;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class DownloadViewModel : ObservableObject
    {
        private System.Timers.Timer _statusUpdateTimer;

        [ObservableProperty]
        private ObservableCollection<MagnetDownloadManager> downloadManagers = new();

        [RelayCommand]
        private async Task StartPause(MagnetDownloadManager manager)
        {
            if (manager.DownloadStatus.TorrentState == MonoTorrent.Client.TorrentState.Downloading)
            {
                await manager.Pause();
            }
            else if (manager.DownloadStatus.TorrentState == MonoTorrent.Client.TorrentState.Paused)
            {
                await manager.Start();
            }
            else return;
        }

        [RelayCommand]
        private async Task Stop(MagnetDownloadManager manager)
        {
            await manager.Stop();
        }

        public DownloadViewModel()
        {
            // 从ResourceDownloadService获取所有下载任务列表，每秒获取一次
            LoadDownloads();
            // 初始化定时器，每秒更新一次
            _statusUpdateTimer = new(1000);
            _statusUpdateTimer.Elapsed += (sender, e) =>
            {
                LoadDownloads();
                UpdateDownloadStatuses();
            };// 每秒更新一次下载状态 下载任务列表
            _statusUpdateTimer.Start();
        }
        private void LoadDownloads()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var latestDownloads = ResourceDownloadService.Instance.GetAllDownloads();

                // 添加新任务
                foreach (var download in latestDownloads)
                {
                    if (!DownloadManagers.Contains(download))
                    {
                        DownloadManagers.Add(download);
                    }
                }

                // 移除已删除的任务
                for (int i = DownloadManagers.Count - 1; i >= 0; i--)
                {
                    if (!latestDownloads.Contains(DownloadManagers[i]))
                    {
                        DownloadManagers.RemoveAt(i);
                    }
                }
            });
        }

        // 定时更新下载状态
        private void UpdateDownloadStatuses()
        {
            foreach (var manager in DownloadManagers)
            {
                manager.UpdateDownloadStatus();
            }
        }
    }
}

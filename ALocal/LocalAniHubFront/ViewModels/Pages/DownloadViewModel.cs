using BasicClassLibrary;
using System.Collections.ObjectModel;

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
            // 从ResourceDownloadService获取所有下载任务列表，每秒获取一次（**待完成**）
            // 初始化定时器，每秒更新一次
            _statusUpdateTimer = new(1000);
            _statusUpdateTimer.Elapsed += (sender, e) => UpdateDownloadStatuses();// 每秒更新一次下载状态
            _statusUpdateTimer.Start();
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

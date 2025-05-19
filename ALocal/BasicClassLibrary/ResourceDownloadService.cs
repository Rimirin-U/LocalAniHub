using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class ResourceDownloadService
    {
        // 单例模式
        public static ResourceDownloadService Instance => _instance.Value;
        private static readonly Lazy<ResourceDownloadService> _instance = new(() => new ResourceDownloadService());
        private ResourceDownloadService() { }

        private readonly ResourceDownload _resourceDownload = new();
        public List<MagnetDownloadManager> downloadManagers = [];

        // 添加下载任务（并启动下载）
        public Task<MagnetDownloadManager> AddAndStartDownload(string magnetUrl, string? downloadPath = null)
        {
            // 在后台线程创建下载任务
            return Task.Run(async () =>
            {
                MagnetDownloadManager manager = downloadPath == null
                    ? await _resourceDownload.GetMagnetDownloadManager(magnetUrl)
                    : await _resourceDownload.GetMagnetDownloadManager(magnetUrl, downloadPath);
                lock (downloadManagers)
                {
                    downloadManagers.Add(manager);
                }
                return manager;
            });
        }
        // 删除下载任务（并确保停止下载）
        public Task RemoveDownloadAsync(MagnetDownloadManager manager)
        {
            // 在后台线程创建删除任务
            return Task.Run(async () =>
            {
                lock (downloadManagers)
                {
                    downloadManagers.Remove(manager);
                }
                await manager.Stop();
            });
        }
        // 获取全部下载任务
        public IReadOnlyList<MagnetDownloadManager> GetAllDownloads()
        {
            lock (downloadManagers)
            {
                return downloadManagers.ToList();
            }
        }

    }
}

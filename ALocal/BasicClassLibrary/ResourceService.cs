using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using MonoTorrent.Client;

namespace BasicClassLibrary
{
    public class ResourceService
    {
        private readonly ResourceManager _resourceManager;

        // 构造函数
        public ResourceService(ResourceManager resourceManager, ResourceDownload resourceDownload)
        {
            _resourceManager = resourceManager;
        }

        // 将现有资源加入资源管理
        public void AddExistingResource(Resource resource)
        {
            _resourceManager.Addresource(resource);
        }
        // 下载资源
        public async Task StartDownloadResource(ResourceItem resourceItem, EventHandler<TorrentStateChangedEventArgs>? torrentStateChangeEventHandler = null)
        {
            if (resourceItem == null)
            {
                throw new ArgumentNullException(nameof(resourceItem));
            }
            if (string.IsNullOrWhiteSpace(resourceItem.DownloadUrl))
            {
                throw new ArgumentException("Download URL cannot be null or empty.", nameof(resourceItem.DownloadUrl));
            }
            // 使用ResourceDownloadService来下载资源
            await ResourceDownloadService.Instance.AddAndStartDownload(resourceItem.DownloadUrl, torrentStateChangeEventHandler: torrentStateChangeEventHandler);
        }

        // 按最大保存日期清理
        public void CleanupByDays(int daysToKeep)
        {
            if (daysToKeep <= 0)
                throw new ArgumentException("Days to keep must be positive.", nameof(daysToKeep));

            var threshold = DateTime.Now.AddDays(-daysToKeep);
            var resources = _resourceManager.Query(r => true);
            CleanupByTime(resources, threshold);
        }

        // 按最大存储空间(MB)清理
        public void CleanupByMaxSizeMB(int maxSizeMB)
        {
            if (maxSizeMB <= 0)
                throw new ArgumentException("Max size must be positive.", nameof(maxSizeMB));

            var maxSizeBytes = (long)maxSizeMB * 1024 * 1024;//转换为字节
            var resources = _resourceManager.Query(r => true);
            CleanupBySize(resources, maxSizeBytes);
        }

        private void CleanupByTime(List<Resource> resources, DateTime threshold)
        {
            var toDelete = resources.Where(r => r.ImportData < threshold).ToList();
            foreach (var resource in toDelete)
            {
                DeleteResourceWithFile(resource);
            }
            //删除导入日期早于设定日期的资源
        }

        private void CleanupBySize(List<Resource> resources, long maxSizeBytes)
        {
            var validResources = resources
                .Where(r => !string.IsNullOrWhiteSpace(r.ResourcePath) && File.Exists(r.ResourcePath))
                .ToList();
            //资源路径存在并且不为空白
            long totalSize = validResources.Sum(r =>
                r.ResourcePath != null ? new FileInfo(r.ResourcePath).Length : 0);
            //目前资源总大小
            if (totalSize <= maxSizeBytes) return;

            validResources.Sort(ResourceManager.SortByImportData);
            //按导入日期对资源进行排序 假设最早的排在最前面
            foreach (var resource in validResources)
            {
                if (totalSize <= maxSizeBytes) break;

                if (resource.ResourcePath != null)
                {
                    long fileSize = new FileInfo(resource.ResourcePath).Length;
                    DeleteResourceWithFile(resource);
                    totalSize -= fileSize;
                }
            }
            //依次删除最旧的文件，直到总大小不超过限制
        }

        private void DeleteResourceWithFile(Resource resource)
        {
            _resourceManager.DeleteResource(resource.Id);
            if (File.Exists(resource.ResourcePath))
            {
                File.Delete(resource.ResourcePath);
            }
        }

        // 在下载完成后 将资源文件从下载到的默认路径移动到其对应的资源文件夹，并创建资源对象，加入数据库中
        // 本事件在TorrentState每次改变时都会触发，但以上逻辑只应该在TorrentState改变为 已完成 时触发
        public static EventHandler<TorrentStateChangedEventArgs> AfterDownload(int episodeId)
        {
            return delegate (object? sender, TorrentStateChangedEventArgs e)
            {
                // ...
            };
        }
    }
}
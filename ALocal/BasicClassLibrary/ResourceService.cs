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

namespace BasicClassLibrary
{
    public enum CleanupMode
    {
        ByTime,
        BySize
    }
    public class ResourceService
    {
            private readonly ResourceManager _resourceManager;
            private readonly ResourceDownload _resourceDownload;

            // 构造函数
            public ResourceService(ResourceManager resourceManager, ResourceDownload resourceDownload)
            {
                _resourceManager = resourceManager;
                _resourceDownload = resourceDownload;
            }

            // 将现有资源加入资源管理
            public void AddExistingResource(string sourceFilePath, int episode)
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath))
                {
                    throw new ArgumentException("Source file path cannot be null or empty.", nameof(sourceFilePath));
                }

                if (episode <= 0)
                {
                    throw new ArgumentException("Episode number must be positive.", nameof(episode));
                }

                _resourceManager.AddResource(sourceFilePath, episode);
            }

            // 下载指定资源为指定作品的指定集数
            public void DownloadAndAddResource(ResourceItem resourceItem, int episode)
            {
                if (resourceItem == null)
                {
                    throw new ArgumentNullException(nameof(resourceItem));
                }

                if (string.IsNullOrWhiteSpace(resourceItem.DownloadUrl))
                {
                    throw new ArgumentException("Download URL cannot be null or empty.", nameof(resourceItem.DownloadUrl));
                }

                if (episode <= 0)
                {
                    throw new ArgumentException("Episode number must be positive.", nameof(episode));
                }

                // 下载资源
                _resourceDownload.Download(resourceItem.DownloadUrl);
            }

        // 资源清理（按时间/大小）
        public void CleanupResources(CleanupMode mode, object parameter)
        {
            var allResources = _resourceManager.GetAllResources();

            switch (mode)
            {
                case CleanupMode.ByTime:
                    CleanupByTime(allResources, (DateTime)parameter);
                    break;
                case CleanupMode.BySize:
                    CleanupBySize(allResources, (long)parameter);
                    break;
            }
        }

        private void CleanupByTime(List<Resource> resources, DateTime threshold)
        {
            var toDelete = resources.Where(r => r.ImportData < threshold).ToList();
            foreach (var resource in toDelete)
            {
                DeleteResourceWithFile(resource);
            }
        }

        private void CleanupBySize(List<Resource> resources, long maxSizeBytes)
        {
            long totalSize = resources.Sum(r => new FileInfo(r.ResourcePath).Length);
            if (totalSize <= maxSizeBytes) return;

            var orderedResources = resources.OrderBy(r => r.ImportData).ToList();
            foreach (var resource in orderedResources)
            {
                if (totalSize <= maxSizeBytes) break;

                long fileSize = new FileInfo(resource.ResourcePath).Length;
                DeleteResourceWithFile(resource);
                totalSize -= fileSize;
            }
        }

        private void DeleteResourceWithFile(Resource resource)
        {
            _resourceManager.DeleteResource(resource.ResourceId.ToString());
            if (File.Exists(resource.ResourcePath))
            {
                File.Delete(resource.ResourcePath);
            }
        }

        private int _currentId = 0;
        private int GenerateResourceId()
        {
            lock (_resourceManager)
            {
                return ++_currentId;
            }
        }
//        使用示例
//        var resourceManager = new ResourceManagement("D:/Resources");
//        var fetcher = new ResourceFetcher(new ResourceSearch(), new ResourceDownloader());
//        var service = new ResourceService(resourceManager, fetcher);

//        // 添加本地文件
//        service.AddExistingResource("C:/Downloads/video.mp4", "MySeries", 1);

//        // 下载并管理
//        service.DownloadAndAddResource("episode2", "MySeries", 2);

//        // 清理资源（保留最近30天的）
//        service.CleanupResources(CleanupMode.ByTime, DateTime.Now.AddDays(-30));

//        // 清理资源（保持总大小小于1GB）
//        service.CleanupResources(CleanupMode.BySize, 1024L * 1024 * 1024);
//
    }
}
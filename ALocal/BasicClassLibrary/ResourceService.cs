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
    public class ResourceService
    {
        private readonly ResourceManager _resourceManager;
        private readonly BtDownload _resourceDownload;

        // 构造函数
        public ResourceService(ResourceManager resourceManager, BtDownload resourceDownload)
        {
            _resourceManager = resourceManager;
            _resourceDownload = resourceDownload;
        }

        // 将现有资源加入资源管理
        public void AddExistingResource(Resource resource)
        {
            _resourceManager.Addresource(resource);
        }

        // 下载指定资源为指定作品的指定集数
        public void DownloadAndAddResource(ResourceItem resourceItem)
        {
            if (resourceItem == null)
            {
                throw new ArgumentNullException(nameof(resourceItem));
            }
            if (string.IsNullOrWhiteSpace(resourceItem.DownloadUrl))
            {
                throw new ArgumentException("Download URL cannot be null or empty.", nameof(resourceItem.DownloadUrl));
            }
            // 下载资源
            _resourceDownload.Download(resourceItem.DownloadUrl);
        }

        // 按最大保存日期清理
        public void CleanupByDays(int daysToKeep)
        {
            if (daysToKeep <= 0)
                throw new ArgumentException("Days to keep must be positive.", nameof(daysToKeep));

            var threshold = DateTime.Now.AddDays(-daysToKeep);
            var resources = _resourceManager.Query(r => true); // 使用 Query 方法代替 All
            CleanupByTime(resources, threshold);
        }

        // 按最大存储空间(MB)清理
        public void CleanupByMaxSizeMB(int maxSizeMB)
        {
            if (maxSizeMB <= 0)
                throw new ArgumentException("Max size must be positive.", nameof(maxSizeMB));

            var maxSizeBytes = (long)maxSizeMB * 1024 * 1024;
            var resources = _resourceManager.Query(r => true); // 使用 Query 方法代替 All
            CleanupBySize(resources, maxSizeBytes);
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
            var validResources = resources
                .Where(r => !string.IsNullOrWhiteSpace(r.ResourcePath) && File.Exists(r.ResourcePath))
                .ToList();

            long totalSize = validResources.Sum(r =>
                r.ResourcePath != null ? new FileInfo(r.ResourcePath).Length : 0);

            if (totalSize <= maxSizeBytes) return;

            validResources.Sort(ResourceManager.SortByImportData);

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
        }

        private void DeleteResourceWithFile(Resource resource)
        {
            _resourceManager.DeleteResource(resource.Id);
            if (File.Exists(resource.ResourcePath))
            {
                File.Delete(resource.ResourcePath);
            }
        }
    }
}
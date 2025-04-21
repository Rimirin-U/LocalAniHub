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
        private readonly ResourceDownload _resourceDownload;

        // 构造函数
        public ResourceService(ResourceManager resourceManager, ResourceDownload resourceDownload)
        {
            _resourceManager = resourceManager;
            _resourceDownload = resourceDownload;
        }

        // 将现有资源加入资源管理
        public void AddExistingResource(Resource resource)
        {
            _resourceManager.Addresource(resource);
        }
        // 下载资源
        // 修改 DownloadAndAddResource 变为异步
        public async Task DownloadAndAddResource(ResourceItem resourceItem)
        {
            if (resourceItem == null)
            {
                throw new ArgumentNullException(nameof(resourceItem));
            }
            if (string.IsNullOrWhiteSpace(resourceItem.DownloadUrl))
            {
                throw new ArgumentException("Download URL cannot be null or empty.", nameof(resourceItem.DownloadUrl));
            }
            // 下载资源并等待完成
            await _resourceDownload.GetMagnetDownloadManager(resourceItem.DownloadUrl);
        }

        // 按最大保存日期清理
        public void CleanupByDays(int daysToKeep)
        {
            if (daysToKeep <= 0)
                throw new ArgumentException("Days to keep must be positive.", nameof(daysToKeep));

            var threshold = DateTime.Now.AddDays(-daysToKeep);
            var resources = _resourceManager.Query(r=>true);
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
    }
}
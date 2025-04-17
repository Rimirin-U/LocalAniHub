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
        public void AddExistingResource(Resource resource)
        {
            _resourceManager.AddResource(resource);
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

        // 资源清理（按时间/大小）
        public void CleanupResources(CleanupMode mode, object parameter)
        {
            var allResources = _resourceManager.All();

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
            // 获取所有有效资源（有路径且文件存在）
            var validResources = resources
                .Where(r => !string.IsNullOrWhiteSpace(r.ResourcePath) && File.Exists(r.ResourcePath))
                .ToList();

            // 计算总大小
            long totalSize = validResources.Sum(r =>
                r.ResourcePath != null ? new FileInfo(r.ResourcePath).Length : 0);

            // 如果总大小已满足要求，直接返回
            if (totalSize <= maxSizeBytes) return;

            // 使用SortByImportData进行排序（从旧到新）
            validResources.Sort(ResourceManager.SortByImportData);

            // 按顺序删除最旧的资源，直到满足大小要求
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
            _resourceManager.DeleteResource(resource.Id.ToString());
            if (File.Exists(resource.ResourcePath))
            {
                File.Delete(resource.ResourcePath);
            }
        }

//        使用示例
//        //添加本地已有资源
//        service.AddExistingResource(new Resource {
//        Id = "movie-001",
//        ResourcePath = "D:/Movies/Inception.mp4",
//        ImportData = DateTime.Now.AddMonths(-2)
//        });
//        // 下载新资源
//        service.DownloadAndAddResource(new ResourceItem {
//        DownloadUrl = "https://example.com/new-movie.mp4"
//        });
//        // 清理资源（保留最近30天的）
//        service.CleanupResources(CleanupMode.ByTime, DateTime.Now.AddDays(-30));
//
//        // 清理资源（保持总大小小于1GB）
//        service.CleanupResources(CleanupMode.BySize, 1024L * 1024 * 1024);
    }
}
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
        private readonly ResourceManagement _resourceManager;
        private readonly ResourceFetcher _resourceFetcher;

        public ResourceService(ResourceManagement resourceManager, ResourceFetcher resourceFetcher)
        {
            _resourceManager = resourceManager;
            _resourceFetcher = resourceFetcher;
        }

        // 添加现有资源到指定作品和集数
        //sourceFilePath:现有资源的完整物理路径 workName:目标作品的名称 episode:资源所属的集数
        public void AddExistingResource(string sourceFilePath, string workName, int episode)
        {
            // 构造目标路径：父文件夹/作品名称/集数/文件名
            string fileName = Path.GetFileName(sourceFilePath);
            string destDirectory = Path.Combine(_resourceManager._parentFolderPath, workName, $"Episode_{episode}");
            Directory.CreateDirectory(destDirectory);
            string destFilePath = Path.Combine(destDirectory, fileName);

            // 移动文件
            File.Move(sourceFilePath, destFilePath);

            // 创建Resource对象（假设ResourceId由外部生成或自增）
            var resource = new Resource(
                episodeId: episode,
                episode: null, // 导航属性可能需要额外处理
                importData: DateTime.Now,
                path: destFilePath
            )
            { ResourceId = GenerateResourceId() }; // 需实现ID生成逻辑

            _resourceManager.AddResource(destFilePath, resource);
        }

        // 下载并添加资源到指定作品和集数
        public void DownloadAndAddResource(string keyword, string workName, int episode)
        {
            string destDirectory = Path.Combine(_resourceManager._parentFolderPath, workName, $"Episode_{episode}");
            Directory.CreateDirectory(destDirectory);

            // 记录下载前的文件列表
            var filesBefore = Directory.GetFiles(destDirectory);

            // 下载资源
            _resourceFetcher.FetchResource(keyword, destDirectory);

            // 获取新下载的文件
            var filesAfter = Directory.GetFiles(destDirectory);
            var newFile = filesAfter.Except(filesBefore).FirstOrDefault();

            if (newFile == null) throw new FileNotFoundException("Download failed");

            // 创建Resource对象
            var resource = new Resource(
                episodeId: episode,
                episode: null,
                importData: DateTime.Now,
                path: newFile
            )
            { ResourceId = GenerateResourceId() };

            _resourceManager.AddResource(newFile, resource);
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
//需要补充到资源管理：
//public List<Resource> GetAllResources()
//{
//    lock (_lock)
//    {
//        return new List<Resource>(_resources);
//    }
//}
    }
}
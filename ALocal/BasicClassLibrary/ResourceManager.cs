using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BasicClassLibrary
{
    public class ResourceManager: Manager<Resource>
    {
        public ResourceManager() : base(new AppDbContext()){ }
        private void PutResourceIntoDestination(Resource resource)
        {
            // 从全局设置中读取父文件夹路径
            string _parentFolderPath = GlobalSettingsService.Instance.GetValue("defaultResourceParentFolderPath");
            if (string.IsNullOrEmpty(_parentFolderPath))
            {
                throw new ArgumentException("ResourceManagement: 父文件夹路径无效或未设置");
            }

            // 创建以资源ID命名的文件夹
            string resourceFolder = Path.Combine(_parentFolderPath, resource.Id.ToString());
            Directory.CreateDirectory(resourceFolder); // 如果文件夹已存在，则不会抛出异常

            // 获取目标文件路径
            string fileName = Path.GetFileName(resource.ResourcePath);
            string destFilePath = Path.Combine(resourceFolder, fileName);

            // 检查目标路径是否已存在同名文件
            if (File.Exists(destFilePath))
            {
                throw new Exception($"文件命名冲突: {destFilePath}");
            }

            // 移动文件到目标路径
            try
            {
                File.Move(resource.ResourcePath, destFilePath);
                resource.ResourcePath = destFilePath; // 更新资源对象的路径
            }
            catch (Exception ex)
            {
                throw new Exception($"移动文件失败: {ex.Message}");
            }
        }
        //添加资源
        public void Addresource(Resource resource)
        {
            PutResourceIntoDestination(resource);
            Add(resource);
        }
        //删除资源所在的文件夹
        private void DeleteResourceFolder(int id)
        {
            Resource resource = FindById(id);
            // 删除文件
            if (File.Exists(resource.ResourcePath))
            {
                File.Delete(resource.ResourcePath);
            }
        }
        //删除资源
        public void DeleteResource(int id)
        {
            DeleteResourceFolder(id);
            RemoveById(id);
        }

        //按照导入时间排序-升序
        public static readonly Comparison<Resource> SortByImportData = (x, y) =>
    x.ImportData.CompareTo(y.ImportData);

        // public Func<Resource,bool> ByEntryId(int entryId)
    }
}


/* public class ResourceManagement : IDisposable
     {
         private readonly string _parentFolderPath; // 父文件夹路径
         private readonly AppDbContext _dbContext;  // 数据库上下文
         private readonly object _lock = new object(); // 线程安全锁

         public ResourceManagement()
         {
             _parentFolderPath = GlobalSettingsService.Instance.GetValue("defaultParentFolderPath"); // 从全局设置中读取路径
             if (string.IsNullOrEmpty(_parentFolderPath)) throw new ArgumentException("ResourceManagement: wrong parentfolder path ");
             _dbContext = new AppDbContext();                    // 初始化数据库上下文
         }

         // 添加资源-这个实现有点问题（创建资源对象这里还是有问题）
         // 添加资源
         public void AddResource(string sourceFilePath, int episodeId, string translatedName)
         {
             lock (_lock)
             {
                 try
                 {
                     // 验证源文件路径是否有效
                     if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
                         throw new ArgumentException("源文件路径无效或文件不存在");

                     // 创建作品文件夹
                     string workFolder = Path.Combine(_parentFolderPath, translatedName);
                     Directory.CreateDirectory(workFolder);

                     // 目标文件路径
                     string fileName = Path.GetFileName(sourceFilePath);
                     string destFilePath = Path.Combine(workFolder, fileName);

                     // 处理文件名冲突
                     int count = 1;
                     while (File.Exists(destFilePath))
                     {
                         string tempFileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({count++}){Path.GetExtension(fileName)}";
                         destFilePath = Path.Combine(workFolder, tempFileName);
                     }

                     // 移动文件到目标路径
                     File.Move(sourceFilePath, destFilePath);

                     // 创建新的资源对象（怎么读取资源中的EpisodeNumber、这部作品的名字等等信息）！！！
                     var resource = new Resource
                     {
                         EpisodeId = episodeId,
                         ImportData = DateTime.Now,
                         ResourcePath = destFilePath
                     };

                     // 将资源对象添加到数据库
                     _dbContext.Resources.Add(resource);
                     _dbContext.SaveChanges();
                 }
                 catch (Exception ex)
                 {
                     throw new Exception("发生错误", ex);
                 }
             }
         }

         // 按作品名称和集数查找资源ID列表
         public List<int> FindResourcesByWorkNameAndEpisode(string translatedName, int episodeNumber)
         {
             lock (_lock)
             {
                 return _dbContext.Resources
                     .Include(r => r.Episode)
                     .ThenInclude(e => e.Entry)
                     .Where(r => r.Episode.Entry.TranslatedName == translatedName && r.Episode.EpisodeNumber == episodeNumber)
                     .Select(r => r.ResourceId)
                     .ToList();
             }
         }
         // 按ID返回资源元数据对象
         public Resource GetResourceById(int id)
         {
             lock (_lock)
             {
                 return _dbContext.Resources.FirstOrDefault(r => r.ResourceId == id);
             }
         }
         // 删除资源
         public void DeleteResource(int id)
         {
             lock (_lock)
             {
                 var resource = _dbContext.Resources.FirstOrDefault(r => r.ResourceId == id);
                 if (resource == null)
                     throw new ArgumentException("未找到指定ID的资源");

                 // 删除文件
                 if (File.Exists(resource.ResourcePath))
                 {
                     File.Delete(resource.ResourcePath);
                 }

                 // 从数据库中删除资源
                 _dbContext.Resources.Remove(resource);
                 _dbContext.SaveChanges();
             }
         }

         // 实现 IDisposable 接口
         public void Dispose()
         {
             _dbContext.Dispose();
         }

     }*/
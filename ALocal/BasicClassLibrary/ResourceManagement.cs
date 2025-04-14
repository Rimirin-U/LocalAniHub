using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasicClassLibrary
{
    public class Resource;//资源类，后续合并需要删掉
    //几个问题：资源对象是否应该区分元数据（核心数据）和其他数据
    //是否也应该用数据库来存储资源对象
    //本代码中暂未实现数据库的相关操作，将资源全部存储在了资源列表中List<Resource> _resources
    public class ResourceManagement:IDisposable
    {
        private string _parentFolderPath;//父文件夹路径
        private readonly List<Resource> _resources = new List<Resource>();
        private readonly object _lock = new object(); // 线程安全锁
        // 构造函数：初始化父文件夹路径
        public void ResourceManager(string parentFolderPath)
        {
            _parentFolderPath = parentFolderPath;

            // 确保父文件夹存在
            Directory.CreateDirectory(_parentFolderPath);
        }
        public void AddResource(string sourceFilePath, Resource resource)//接收源文件路径（资源拉取下来所存放的路径）
        {
            lock (_lock) // 确保线程安全
            {
                try
                {
                    // 验证输入参数
                    if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
                        throw new ArgumentException("源文件路径无效或文件不存在");

                    // 创建作品文件夹
                    string workFolder = Path.Combine(_parentFolderPath, resource.ResourceName);//根据作品名称创建子文件夹路径-资源对象应该有这部作品的名字
                    Directory.CreateDirectory(workFolder);

                    // 目标文件路径
                    string fileName = Path.GetFileName(sourceFilePath);
                    string destFilePath = Path.Combine(workFolder, fileName);//构建目标文件路径

                    // 处理文件名冲突，通过在文件名后面添加序号解决冲突
                    int count = 1;
                    while (File.Exists(destFilePath))
                    {
                        string tempFileName = $"{Path.GetFileNameWithoutExtension(fileName)} ({count++}){Path.GetExtension(fileName)}";
                        destFilePath = Path.Combine(workFolder, tempFileName);
                    }

                    // 移动文件到目标路径
                    File.Move(sourceFilePath, destFilePath);


                    // 将资源添加到内存列表
                    _resources.Add(resource);

                }
                catch (IOException ex)
                {
                    throw new InvalidOperationException("无法完成资源添加操作", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("发生未知错误", ex);
                }
            }
        }
        // 按作品名称查找资源ID
        public List<string> FindResourcesByWorkName(string workName)
        {
            lock (_lock) // 确保线程安全
            {
                return _resources
                    .Where(r => r.ResourceName == workName)
                    .Select(r => r.ResourceId)
                    .ToList();
            }
        }
        public List<string> FindResourcesByEpisode(int episode)
        {
            lock (_lock) // 确保线程安全
            {
                return _resources
                    .Where(r => r.Episode == episode)
                    .Select(r => r.ResourceId)
                    .ToList();
            }
        }
        //按Id返回对应的资源对象
        public Resource GetResourceById(string id)
        {
            lock (_lock) // 确保线程安全
            {
                return _resources.FirstOrDefault(r => r.ResourceId == id);
            }
        }
        //删除资源
        public void DeleteResource(string id)
        {
            lock (_lock) // 确保线程安全
            {
                var resource = _resources.FirstOrDefault(r => r.ResourceId == id);
                if (resource == null)
                {
                    throw new ArgumentException("未找到指定ID的资源");
                }

                // 删除文件
                if (File.Exists(resource.FilePath))
                {
                    File.Delete(resource.FilePath);
                }

                // 从内存列表中移除资源
                _resources.Remove(resource);
            }
        }
        // 实现 IDisposable 接口，释放资源
        public void Dispose()
        {
            // 清空所有资源（这里可以扩展为持久化保存逻辑）
            _resources.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class MaterialService
    {
        private readonly string _baseMaterialPath= @"/base/Material";

        public MaterialService()
        {
            // 确保素材基础目录存在
            Directory.CreateDirectory(_baseMaterialPath);
        }

        public void AddMaterial(Entry entry, string sourceFilePath)
        {
            // 验证条目文件夹有效性（自动创建目录）
            ValidateEntryFolder(entry);

            string fileName = Path.GetFileName(sourceFilePath);
            string targetPath = GetEntryMaterialPath(entry, fileName);

            if (File.Exists(targetPath))
                throw new IOException("文件已存在：" + fileName);

            File.Move(sourceFilePath, targetPath);
        }

        public void RemoveMaterial(Entry entry, string fileName)
        {
            string filePath = GetEntryMaterialPath(entry, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
            else
                throw new FileNotFoundException("素材不存在：" + fileName);
        }
        // 添加主封面图
        public void AddKeyVisual(Entry entry, string imagePath)
        {
            // 验证图片格式
            if (!IsImageFile(imagePath))
                throw new ArgumentException("文件不是有效的图片格式");

            // 添加素材
            AddMaterial(entry, imagePath);

            // 设置封面图ID（使用文件名作为标识）
            entry.KeyVisualId = Path.GetFileName(imagePath);
        }
        // 批量添加素材
        public void BatchAddMaterials(Entry entry, DateTime startTime, DateTime endTime)
        {
            ValidateEntryFolder(entry);

            string sourceFolder = GlobalSettingsService.BatchSourceFolder; // 从设置中获得指定文件夹
            var files = Directory.EnumerateFiles(sourceFolder)
                .Select(f => new FileInfo(f))
                .Where(f => f.CreationTime >= startTime && f.CreationTime <= endTime);

            foreach (var file in files)
            {
                try
                {
                    AddMaterial(entry, file.FullName);
                }
                catch (Exception ex)
                {
                    // 记录错误并继续处理其他文件
                    Console.WriteLine($"添加文件{file.Name}失败：{ex.Message}");
                }
            }
        }

        private string GetEntryMaterialPath(Entry entry, string fileName)
        {
            return Path.Combine(
                _baseMaterialPath,
                entry.MaterialFolder,
                fileName
            );
        }

        private void ValidateEntryFolder(Entry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.MaterialFolder))
                throw new ArgumentException("条目素材文件夹名称不能为空");

            string entryFolder = Path.Combine(_baseMaterialPath, entry.MaterialFolder);
            Directory.CreateDirectory(entryFolder);
        }
        // 验证文件是否为图片格式
        private bool IsImageFile(string filePath)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string extension = Path.GetExtension(filePath).ToLower();
            return imageExtensions.Contains(extension);
        }
    }
}

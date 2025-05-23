﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
namespace BasicClassLibrary
{
    public class MaterialService
    {
        private readonly string _globalBaseFolder; 
        private readonly string _baseMaterialPath;

        public MaterialService()
        {
            // 初始化 _globalBaseFolder  
            _globalBaseFolder = GlobalSettingsService.Instance.GetValue("globalBaseFolder");
            _baseMaterialPath = Path.Combine(_globalBaseFolder, "Material");
            // 确保素材基础目录存在  
            Directory.CreateDirectory(_baseMaterialPath);
        }

        public void AddMaterial(Entry entry, string sourceFilePath)
        {
            // 验证条目文件夹有效性（自动创建目录）  
            ValidateEntryFolder(entry);

            string fileName = Path.GetFileName(sourceFilePath);
            string targetPath = GetEntryMaterialPath(entry, fileName);

            // 如果目标文件已存在，直接覆盖
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            File.Move(sourceFilePath, targetPath);
        }
        public void AddMaterial(Entry entry, Image image, string fileName)
        {
            // 验证条目文件夹有效性（自动创建目录）
            ValidateEntryFolder(entry);

            // 确保文件名有效
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("文件名不能为空");

            // 获取目标路径
            string targetPath = GetEntryMaterialPath(entry, fileName);

            // 如果文件已存在，抛出异常 
            if (File.Exists(targetPath))
                throw new IOException("文件已存在：" + fileName);

            Bitmap copy = new Bitmap(image);

            // 将Image对象保存到目标路径
            copy.Save(targetPath, ImageFormat.Png); // 部分低级版本不支持
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

            string sourceFolder = GlobalSettingsService.Instance.GetValue("BatchSourceFolder"); // 从设置中获得指定文件夹  
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

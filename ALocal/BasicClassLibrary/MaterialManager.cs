using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//素材管理
namespace BasicClassLibrary
{
    public class MaterialManager : Manager<Material>
    {
        // 构造函数
        public MaterialManager() : base(new AppDbContext()) { }
        // 按条目ID查询素材
        public static readonly Func<int, Func<Material, bool>> ByEntryId =
            entryId => material => material.EntryId == entryId;
        private void PutMaterialIntoDestination(Material material)
        {
            // 从全局设置中读取父文件夹路径
            string _parentFolderPath = GlobalSettingsService.Instance.GetValue("defaultMaterialParentFolderPath");
            if (string.IsNullOrEmpty(_parentFolderPath))
            {
                throw new ArgumentException("MaterialManagement: 父文件夹路径无效或未设置");
            }

            // 创建以作品ID命名的文件夹
            string workFolder = Path.Combine(_parentFolderPath, material.Id.ToString());
            Directory.CreateDirectory(workFolder); // 如果文件夹已存在，则不会抛出异常

            // 获取目标文件路径
            string fileName = Path.GetFileName(material.Path);
            string destFilePath = Path.Combine(workFolder, fileName);

            // 检查目标路径是否已存在同名文件
            if (File.Exists(destFilePath))
            {
                throw new Exception($"文件命名冲突: {destFilePath}");
            }

            // 移动文件到目标路径
            try
            {
                File.Move(material.Path, destFilePath);
                material.Path = destFilePath; // 更新素材对象的路径
            }
            catch (Exception ex)
            {
                throw new Exception($"移动文件失败: {ex.Message}");
            }
        }

        // 添加素材
        public void AddMaterial(Material material)
        {
            PutMaterialIntoDestination(material);
            Add(material);
        }

        // 删除素材所在的文件夹
        private void DeleteMaterialFolder(int id)
        {
            Material? material = FindById(id);  
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material), "未找到指定的素材。");
            }

            // 删除文件  
            if (File.Exists(material.Path))
            {
                File.Delete(material.Path);
            }
        }

        // 删除素材
        public void DeleteMaterial(int id)
        {
            DeleteMaterialFolder(id);
            RemoveById(id);
        }
    }
}

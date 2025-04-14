using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//素材服务(包括素材管理)
namespace BasicClassLibrary
{
    public class MaterialService
    {
        private readonly AppDbContext _context;

        public MaterialService(AppDbContext context)
        {
            _context = context;
        }

        // 添加素材
        public void AddMaterial(Material material)
        {
            _context.Materials.Add(material);
            _context.SaveChanges();
        }

        // 删除素材
        public bool RemoveMaterial(int materialId)
        {
            var material = _context.Materials.FirstOrDefault(m => m.Id == materialId);
            if (material != null)
            {
                _context.Materials.Remove(material);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        // 更新素材
        public void UpdateMaterial(int materialId, Material updatedMaterial)
        {
            var material = _context.Materials.FirstOrDefault(m => m.Id == materialId);
            if (material != null)
            {
                material.Name = updatedMaterial.Name;
                material.Kind = updatedMaterial.Kind;
                material.Path = updatedMaterial.Path;
                _context.SaveChanges();
            }
        }

        // 获取所有素材
        public IEnumerable<Material> GetAllMaterials()
        {
            return _context.Materials.ToList();
        }

        // 获取特定条目的素材
        public IEnumerable<Material> GetMaterialsByEntry(int entryId)
        {
            return _context.Materials.Where(m => m.EntryId == entryId).ToList();
        }
    }
}

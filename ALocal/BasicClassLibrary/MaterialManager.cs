using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//素材服务(包括素材管理)
namespace BasicClassLibrary
{
    public class MaterialManager : Manager<Material>
    {
        // 构造函数
        public MaterialManager() : base(new AppDbContext()) { }
        // 按条目ID查询素材
        public static readonly Func<int, Func<Material, bool>> ByEntryId =
            entryId => material => material.EntryId == entryId;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryMetaDataManager: Manager<EntryMetadata>
    {
        // 构造函数
        public EntryMetaDataManager() : base(new AppDbContext()) { }
        // 按条目ID查询条目元数据
        public static readonly Func<int, Func<EntryMetadata, bool>> ByEntryId =
            id => meta => meta.EntryId == id;
    }
}

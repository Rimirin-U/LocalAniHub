using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Resource
    {
        public int Id { get; set; } // 主键
        public int EntryId { get; set; } // 外键：条目ID
        public int EpisodeNumber { get; set; } // 集数
        public string? FilePath { get; set; } // 资源文件路径
        public bool HasSubtitle { get; set; } // 是否有外挂字幕
        public DateTime ImportDate { get; set; } // 导入时间
        public long Size { get; set; } // 资源大小（字节）

        // 导航属性
        public Entry Entry { get; set; }
    }
    internal class ResourceService
    {
    }
}

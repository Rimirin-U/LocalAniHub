using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IResource : IEpisodeNavigation
    {
        DateTime ImportDate { get; set; } // 导入时间
        int EntryId { get; set; } // 对应作品ID（外键）
        int EpisodeNumber { get; set; } // 集数（第几集）
        bool HasSubtitle { get; set; } // 有无单独字幕文件（外挂字幕）
        string Path { get; set; } // 路径（相对路径）
        int Id { get; set; } // 资源ID
    }
}

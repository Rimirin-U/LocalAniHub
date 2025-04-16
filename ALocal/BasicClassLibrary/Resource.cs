using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// 资源对象
namespace BasicClassLibrary
{
    public class Resource:IEpisodeNavigation
    {
        public int ResourceId { get; set; } // 主键 资源ID
        //IEpisodeNavigation
        public int? EpisodeId { get; set; }    // 外键 对应集数ID
        public Episode? Episode { get; set; }    // 导航属性  
        public DateTime ImportData { get; set; } // 导入时间
        //public bool HasSubtitle { get; set; } // 是否有外挂字幕
        public string? ResourcePath { get; set; } //路径
        //
        public Resource(int? episodeId,Episode? episode,
            DateTime importData,string path)
        {
            EpisodeId = episodeId;
            Episode = episode;
            ImportData = importData;
            ResourcePath = path;
        }
    }
}

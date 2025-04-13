using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//从branch4拉取过来
namespace BasicClassLibrary
{
    public interface IEpisodeNavigation
    {
        public int? EpisodeId { get; set; }    // 外键
        public Episode? Episode { get; set; }    // 导航属性
    }
}

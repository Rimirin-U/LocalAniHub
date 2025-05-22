using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public enum State
    {
        NotWatched,    // 未看
        Watching,      // 在看
        Watched,       // 已看
        GivenUp        // 抛弃
    }


    public class Entry: IEntityWithId              //条目核心
    {
        //核心数据
        public int Id {  get; set; }
        public string TranslatedName { get; set; }//译名
        public string OriginalName {  get; set; }//原名
        public DateTime ReleaseDate { get; set; }//上映时间
        public DateTime CollectionDate { get; set; }//收藏日期
        public string Category {  get; set; }//类别
        public int EpisodeCount {  get; set; }//集数
        public State State { get; set; }         // 观看状态
        public string MaterialFolder { get; set; } // 素材子文件夹名称
        public List<string> KeyWords; // 关键字列表
        //配合素材服务的属性
        public string KeyVisualId { get; set; } // 主封面图ID（主封面图文件名）

        //设置项
        public bool HasUpdateTime {  get; set; }//是否有更新时自动获取资源
        public bool AutoClearResources {  get; set; }//是否自动清除资源
        public Entry(string translatedName,string originalName,DateTime releaseDate,DateTime collectionDate,
            string category,int episodeCount, State state,
            string materialFolder, string keyVisualId, bool hasUpdateTime,bool autoClearResources,List<string> keyWords)    
        {
            TranslatedName = translatedName;
            OriginalName = originalName;
            ReleaseDate = releaseDate;
            CollectionDate = collectionDate;
            Category = category;
            EpisodeCount = episodeCount;
            State = state;
            MaterialFolder = materialFolder;
            KeyVisualId = keyVisualId;
            HasUpdateTime = hasUpdateTime;
            AutoClearResources = autoClearResources;
            KeyWords = keyWords;
        }
    }
    //关于数据库的上下文
    public partial class AppDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; }
    }
}

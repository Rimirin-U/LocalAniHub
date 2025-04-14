using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Entry//条目核心
    {
        //核心数据
        public int ID {  get; set; }
        public string TranslatedName { get; set; }//译名
        public string OriginalName {  get; set; }//原名
        public DateTime ReleaseDate { get; set; }//上映时间
        public DateTime CollectionDate { get; set; }//收藏日期
        public string Category {  get; set; }//类别
        public int EpisodeCount {  get; set; }//集数

        //设置项
        public bool HasUpdateTime {  get; set; }//是否有更新时自动获取资源
        public bool AutoClearResources {  get; set; }//是否自动清除资源
        public Entry(int id,string translatedName,string originalName,DateTime releaseDate,DateTime collectionDate,string category,int episodeCount,bool hasUpdateTime,bool autoClearResources)    
        {
            ID = id;
            TranslatedName = translatedName;
            OriginalName = originalName;
            ReleaseDate = releaseDate;
            CollectionDate = collectionDate;
            Category = category;
            EpisodeCount = episodeCount;
            HasUpdateTime = hasUpdateTime;
            AutoClearResources = autoClearResources;//感觉两个设置属性的初始化有点问题
        }
        //关于数据库的上下文
        public partial class AppDbContext : DbContext
        {
            public DbSet<Entry>Entries { get; set; }
        }


    }
}

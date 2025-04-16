using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryManager : Manager<Entry>
    {
        public EntryManager():base(new AppDbContext()) { }
        // 自定义查询谓词
        //查询所有条目
        public static readonly Func<Entry, bool> All =
            entry => true;
        //按名称查询
        //按译名查询
        //这是一个静态方法，它接受一个字符串参数 name，并返回一个 Func<Entry, bool> 类型的委托。
        //该委托用于筛选 Entry 对象，判断其 TranslatedName 属性是否包含指定的 name，并且不区分大小写。
        public static Func<Entry, bool> ByTranslatedName(string name) =>
            e => e.TranslatedName.Contains(name, StringComparison.OrdinalIgnoreCase);
        //按原名查询
        public static Func<Entry, bool> ByOriginalName(string name) =>
            e => e.OriginalName.Contains(name, StringComparison.OrdinalIgnoreCase);
        //按日期范围查询
        //按上映时间查询
        public static Func<Entry, bool> ByReleaseDateRange(DateTime start, DateTime end) =>
            e => e.ReleaseDate >= start && e.ReleaseDate <= end;
        //按收藏日期查询
        public static Func<Entry, bool> ByCollectionDateRange(DateTime start, DateTime end) =>
            e => e.CollectionDate >= start && e.CollectionDate <= end;

        // 按类别查询
        public static Func<Entry, bool> ByCategory(string category) =>
            e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase);

        // 按集数查询
        public static Func<Entry, bool> ByEpisodeCount(int min, int? max = null) =>
            e => e.EpisodeCount >= min && (max == null || e.EpisodeCount <= max);

    }
    //public class EntryManager
    //{
    //    //增加条目
    //    public void AddEntry(Entry entry)
    //    {
    //        if (entry == null) throw new ArgumentNullException(nameof(entry));

    //        using (var context = new AppDbContext())
    //        {
    //            if (context.Entries.Find(entry.Id) != null)
    //                throw new ArgumentException($"条目ID {entry.Id} 已存在");

    //            context.Entries.Add(entry);
    //            context.SaveChanges();

    //            // 初始化关联数据
    //            context.EntryRatings.Add(new EntryRating { EntryId = entry.Id });
    //            context.EntryTimeInfos.Add(new EntryTimeInfo { EntryId = entry.Id });
    //            context.SaveChanges();
    //        }
    //    }

    //    //删除条目
    //    public void DeleteEntry(int entryId)
    //    {
    //        using (var context = new AppDbContext())
    //        {
    //            var entry = context.Entries.Find(entryId);
    //            if (entry == null)
    //                throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");

    //            // 级联删除关联数据
    //            var metadatas = context.EntryMetadatas.Where(m => m.EntryId == entryId).ToList();
    //            context.EntryMetadatas.RemoveRange(metadatas);

    //            var rating = context.EntryRatings.Find(entryId);
    //            if (rating != null)
    //                context.EntryRatings.Remove(rating);

    //            var timeInfo = context.EntryTimeInfos.Find(entryId);
    //            if (timeInfo != null)
    //                context.EntryTimeInfos.Remove(timeInfo);

    //            context.Entries.Remove(entry);
    //            context.SaveChanges();
    //        }
    //    }

    //    //按ID查询条目，返回条目对象
    //    public Entry GetEntryById(int entryId)
    //    {
    //        using (var context = new AppDbContext())
    //        {
    //            var entry = context.Entries.Find(entryId);
    //            if (entry == null)
    //                throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");
    //            return entry;
    //        }
    //    }

    //    //按规则查询条目，得到条目ID列表
    //    public List<int> QueryEntryIds(Func<Entry, bool> predicate)
    //    {
    //        using (var context = new AppDbContext())
    //        {
    //            return predicate == null
    //                   ? context.Entries.Select(e => e.Id).ToList()
    //                   : context.Entries.Where(predicate).Select(e => e.Id).ToList();
    //        }
    //    }


    /* // 条目评分管理
     public void AddOrUpdateEntryRating(int entryId, EntryRating entryRating)
     {
         if (entryRating == null) throw new ArgumentNullException(nameof(entryRating));

         lock (_lock)
         {
             if (!_entries.ContainsKey(entryId))
                 throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");

             entryRating.EntryId = entryId;//将 entryRating 的 EntryId 属性设置为传入的 entryId
             _ratings[entryId] = entryRating;//将 entryRating 存储到 _ratings 字典中
         }
     }

     public void RemoveEntryRating(int entryId)
     {
         lock (_lock)
         {
             _ratings.Remove(entryId);
             // 重新初始化默认评分
             _ratings[entryId] = new EntryRating { EntryId = entryId };
         }
     }

     public EntryRating GetRating(int entryId)
     {
         lock (_lock)
         {
             return _ratings.TryGetValue(entryId, out var rating)
                    ? rating
                    : new EntryRating { EntryId = entryId };
         }
     }

     // 条目播出时间管理
     public void AddOrUpdateEntryTimeInfo(int entryId, EntryTimeInfo entryTimeInfo)
     {
         if (entryTimeInfo == null) throw new ArgumentNullException(nameof(entryTimeInfo));

         lock (_lock)
         {
             if (!_entries.ContainsKey(entryId))
                 throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");

             entryTimeInfo.EntryId = entryId;
             _timeInfos[entryId] = entryTimeInfo;
         }
     }

     public void RemoveEntryTimeInfo(int entryId)
     {
         lock (_lock)
         {
             _timeInfos.Remove(entryId);
             // 重新初始化默认时间信息
             _timeInfos[entryId] = new EntryTimeInfo { EntryId = entryId };
         }
     }

     public EntryTimeInfo GetTimeInfo(int entryId)
     {
         lock (_lock)
         {
             return _timeInfos.TryGetValue(entryId, out var timeInfo)
                    ? timeInfo
                    : new EntryTimeInfo { EntryId = entryId };
         }
     }

     // 条目元数据管理
     public EntryMetadata GetMetadata(int entryId, string key)
     {
         lock (_lock)
         {
             if (!_entries.ContainsKey(entryId))
                 throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");

             return _metadatas.TryGetValue(entryId, out var metadataList)
                    ? metadataList.FirstOrDefault(m => m.Key == key)
                    : null;
         }
     }*/

}

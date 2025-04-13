using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public interface IEntryManager
    {
        // 基本CRUD操作
        void AddEntry(Entry entry);//添加条目
        void DeleteEntry(int entryId);//删除条目
        Entry GetEntryById(int entryId);//根据ID获取条目
        List<int> QueryEntryIds(Func<Entry, bool> predicate);//按规则查询条目，得到条目ID列表（委托）

        // 条目信息管理
        void AddOrUpdateEntryRating(int entryId, EntryRating entryRating);//添加条目评分信息
        void AddOrUpdateEntryTimeInfo(int entryId, EntryTimeInfo entryTimeInfo);//添加条目播出时间信息
        void RemoveEntryRating(int entryId);//删除条目信息
        void RemoveEntryTimeInfo(int entryId);//删除条目播出时间信息

        //获取条目元数据模块
        EntryMetadata GetMetadata(int entryId, string key);

        // 获取条目评分模块
        EntryRating GetRating(int entryId);

        // 获取条目播出时间信息
        EntryTimeInfo GetTimeInfo(int entryId);
    }

}

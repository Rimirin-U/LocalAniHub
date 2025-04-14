using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryManager : IEntryManager
    {
        private readonly Dictionary<int, Entry> _entries = new Dictionary<int, Entry>();
        private readonly Dictionary<int, List<EntryMetadata>> _metadatas = new Dictionary<int, List<EntryMetadata>>();
        private readonly Dictionary<int, EntryRating> _ratings = new Dictionary<int, EntryRating>();
        private readonly Dictionary<int, EntryTimeInfo> _timeInfos = new Dictionary<int, EntryTimeInfo>();
        private readonly object _lock = new object();

        //增加条目
        public void AddEntry(Entry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            lock (_lock) //使用 lock 语句确保在多线程环境下，同一时间只有一个线程可以执行该代码块，避免数据竞争。
            {
                if (_entries.ContainsKey(entry.Id))
                    throw new ArgumentException($"条目ID {entry.Id} 已存在");

                _entries.Add(entry.Id, entry);

                // 初始化关联数据
                _metadatas[entry.Id] = new List<EntryMetadata>();
                _ratings[entry.Id] = new EntryRating { EntryId = entry.Id };
                _timeInfos[entry.Id] = new EntryTimeInfo { EntryId = entry.Id };
            }
        }

        //删除条目
        public void DeleteEntry(int entryId)
        {
            lock (_lock)
            {
                if (!_entries.Remove(entryId))
                    throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");

                // 级联删除关联数据
                _metadatas.Remove(entryId);
                _ratings.Remove(entryId);
                _timeInfos.Remove(entryId);
            }
        }

        //按ID查询条目，返回条目对象
        public Entry GetEntryById(int entryId)
        {
            lock (_lock)
            {
                return _entries.TryGetValue(entryId, out var entry)
                       ? entry
                       : throw new KeyNotFoundException($"找不到ID为 {entryId} 的条目");
            }
        }

        //按规则查询条目，得到条目ID列表
        public List<int> QueryEntryIds(Func<Entry, bool> predicate)
        {
            lock (_lock)
            {
                return predicate == null
                       ? _entries.Keys.ToList()//如果predicate为空，返回_entries字典中的所有条目的ID（Key）列表
                       : _entries.Values.Where(predicate).Select(e => e.Id).ToList();
            }
        }

        // 条目评分管理
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
        }


    }
}

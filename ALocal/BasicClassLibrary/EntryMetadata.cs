using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//条目元数据类
namespace BasicClassLibrary
{
    public class EntryMetadata : IEntryNavigation, IEntityWithId
    {
        [Key]
        public int Id { get; set; } // 主键

        public int? EntryId { get; set; } // 外键 对应条目ID
        public Entry? Entry { get; set; } // 导航属性 对应条目

        public EntryMetadata(int? entryId)
        {
            EntryId = entryId;
        }

        public EntryMetadata()
        {
        }

        // 核心字典属性，存储元数据
        private Dictionary<string, string> _metadataValues { get; set; } = new();
        // 通过特殊前缀区分标签
        private const string TagPrefix = "__tag__";
        // 维护所有键的列表
        private List<string> _allKeys { get; set; } = new();

        // 索引器实现快速访问
        public string this[string key]
        {
            get => _metadataValues.TryGetValue(key, out string? value) ? value : string.Empty;
            set => _metadataValues[key] = value;
        }
        //eg:set metadata["上映日期"] = "2025-01-01";  // 设置键 "上映日期" 对应的值为 "2025-01-01"
        //  :get var releaseDate = metadata["上映日期"];  // 获取 "上映日期" 对应的值
        //       Console.WriteLine(releaseDate);  // 输出：2025-01-01

        // 将元数据字典转换为 JSON 格式进行存储
        [JsonIgnore]
        [Column("EntryMetadatas", TypeName = "TEXT")]
        public string MetadataJson
        {
            get => JsonConvert.SerializeObject(_metadataValues);
            set
            {
                _metadataValues = string.IsNullOrEmpty(value)
                    ? new Dictionary<string, string>()
                    : JsonConvert.DeserializeObject<Dictionary<string, string>>(value)!;
                // 同步更新所有键的列表
                _allKeys = _metadataValues.Keys.ToList();
            }
        }
        // 获取所有键（不包括标签键）
        public List<string> GetAllKeys()
        {
            return _allKeys.Where(k => !k.StartsWith(TagPrefix)).ToList();
        }

        // 删除指定键的元数据
        public void RemoveMetadata(string key)
        {
            if (!_metadataValues.Remove(key))
            {
                throw new KeyNotFoundException($"键 '{key}' 不存在，删除元数据失败。");
            }
            // 从所有键的列表中移除
            _allKeys.Remove(key);
        }
        //eg:bool isRemoved = metadata.RemoveMetadata("导演");  // 删除 "导演" 键对应的元数据
        //   Console.WriteLine(isRemoved);  // 输出：True 或 False，表示是否成功删除

        // 获取指定键的元数据
        public string? GetMetadataValue(string key)
        {
            return _metadataValues.TryGetValue(key, out var value) ? value : null;
        }
        //eg:var director = metadata.GetMetadataValue("导演");  // 获取 "导演" 对应的值
        //Console.WriteLine(director);  // 输出：张三

        // 添加或更新元数据
        public void AddOrUpdateMetadata(string key, string value)
        {
            _metadataValues[key] = value;
            // 如果键不存在于所有键的列表中，则添加
            if (!_allKeys.Contains(key))
            {
                _allKeys.Add(key);
            }
        }
        //eg:metadata.AddOrUpdateMetadata("编剧", "李四");   // 添加或更新 "编剧" 键对应的值
        //metadata.AddOrUpdateMetadata("上映日期", "2025-02-01");  // 更新已存在的 "上映日期"
        // 新增标签处理函数
        public void AddTag(string tag)
        {
            var key = $"{TagPrefix}{tag}";
            _metadataValues[key] = ""; // 值存空字符串
            // 添加到所有键的列表中
            if (!_allKeys.Contains(key))
            {
                _allKeys.Add(key);
            }
        }
        // 添加标签 eg:metadata.AddTag("动作");
        public void RemoveTag(string tag)
        {
            var key = $"{TagPrefix}{tag}";
            var removed = _metadataValues.Remove(key);
            // 如果删除成功，从所有键的列表中移除
            if (removed)
            {
                _allKeys.Remove(key);
            }
        }
        // 获取所有标签
        public List<string> GetTags()
        {
            return _metadataValues.Keys
                .Where(k => k.StartsWith(TagPrefix))
                .Select(k => k.Substring(TagPrefix.Length))
                .ToList();
        }
    }
    public partial class AppDbContext : DbContext
    {
        public DbSet<EntryMetadata> EntryMetadatas { get; set; }
    }
}


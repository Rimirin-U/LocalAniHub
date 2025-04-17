using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
//条目元数据类
namespace BasicClassLibrary
{
    // 实体类：条目元数据
    //public enum MetadataType
    //{
    //    Date,       // 日期型（数据项为日期）
    //    Time,       // 时间型（数据项为时间）
    //    Text,       // 普通数据（数据项为字符串）
    //    Link,       // 链接数据（数据项为链接）
    //    EntryLink,  // 链接条目（数据项为其他条目（id））
    //    Tag         // tag（无数据项）
    //}
    public class EntryMetadata : IEntryNavigation, IEntityWithId
    {
        public int Id { get; set; } // 主键

        public int? EntryId { get; set; } // 外键
        public Entry? Entry { get; set; } // 导航属性

        // 核心字典属性，存储元数据
        private Dictionary<string, string> _metadataValues { get; set; } = new();

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
        private string MetadataJson
        {
            get => JsonConvert.SerializeObject(_metadataValues);
            set => _metadataValues = string.IsNullOrEmpty(value)
                ? new Dictionary<string, string>()
                : JsonConvert.DeserializeObject<Dictionary<string, string>>(value)!;
        }

        // 删除指定键的元数据
        public void RemoveMetadata(string key)
        {
            if (!_metadataValues.Remove(key))
            {
                throw new KeyNotFoundException($"键 '{key}' 不存在，删除元数据失败。");
            }
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
        }
        //eg:metadata.AddOrUpdateMetadata("编剧", "李四");   // 添加或更新 "编剧" 键对应的值
        //metadata.AddOrUpdateMetadata("上映日期", "2025-02-01");  // 更新已存在的 "上映日期"
    }
    public partial class AppDbContext : DbContext
    {
        public DbSet<EntryMetadata> EntryMetadatas { get; set; }
    }
}

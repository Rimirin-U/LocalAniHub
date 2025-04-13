using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//条目元数据
namespace BasicClassLibrary
{
    public enum MetadataType
    {
        Date,       // 日期型（数据项为日期）
        Time,       // 时间型（数据项为时间）
        Text,       // 普通数据（数据项为字符串）
        Link,       // 链接数据（数据项为链接）url
        EntryLink,  // 链接条目（数据项为其他条目（id））int?
        Tag         // tag（无数据项）
    }
    internal interface IEntryMetadata : IEntryNavigation
    {
        // 新增或更新
        void AddOrUpdateMetadata(string key, MetadataType type, object? value = null);
        // 删除
        bool RemoveMetadata(string key);
        /// 获取所有元数据键（用于界面展示）
        IEnumerable<string> MetadataKeys { get; }
        // 按类型获取元数据子集（用于分类操作）
        IReadOnlyDictionary<string, object?> GetMetadataByType(MetadataType type);
        // 获取单个元数据的类型和值
        (MetadataType type, object? value) GetMetadata(string key);
    }
}

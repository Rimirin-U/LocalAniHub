using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//条目元数据类
namespace BasicClassLibrary
{
    // 实体类：条目元数据
    public enum MetadataType
    {
        Date,       // 日期型（数据项为日期）
        Time,       // 时间型（数据项为时间）
        Text,       // 普通数据（数据项为字符串）
        Link,       // 链接数据（数据项为链接）
        EntryLink,  // 链接条目（数据项为其他条目（id））
        Tag         // tag（无数据项）
    }
    public class EntryMetadata : IEntryNavigation
    {
        public int Id { get; set; } // 主键
        public int? EntryId { get; set; } // 外键 每个元数据属于一个条目
        public string Key { get; set; } // 元数据键 表明元数据的类型
        public MetadataType Type { get; set; } // 元数据类型
        public object Value { get; set; } // 元数据值
        // 导航属性
        public Entry? Entry { get; set; } // 和条目关联
    }
    public class EntryMetadataService
    {
        private readonly AppDbContext _context;
        private readonly int _entryId;

        public EntryMetadataService(AppDbContext context, int entryId)
        {
            _context = context;
            _entryId = entryId;
        }
        // 增加 更新
        public void AddOrUpdateMetadata(string key, MetadataType type, object? value = null)
        {
            var metadata = _context.EntryMetadata.FirstOrDefault(m => m.EntryId == _entryId && m.Key == key);
            if (metadata == null)
            {
                metadata = new EntryMetadata
                {
                    EntryId = _entryId,
                    Key = key,
                    Type = type,
                    Value = value
                };
                _context.EntryMetadata.Add(metadata);
            }
            else
            {
                metadata.Type = type;
                metadata.Value = value;
            }
            _context.SaveChanges();
        }
        // 删除
        public bool RemoveMetadata(string key)
        {
            var metadata = _context.EntryMetadata.FirstOrDefault(m => m.EntryId == _entryId && m.Key == key);
            if (metadata != null)
            {
                _context.EntryMetadata.Remove(metadata);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        // 获取元数据键
        public IEnumerable<string> MetadataKeys
        {
            get
            {
                return _context.EntryMetadata
                    .Where(m => m.EntryId == _entryId)
                    .Select(m => m.Key);
            }
        }
        // 按类型获取元数据子集（用于分类操作）
        public IReadOnlyDictionary<string, object?> GetMetadataByType(MetadataType type)
        {
            return _context.EntryMetadata
                .Where(m => m.EntryId == _entryId && m.Type == type)
                .ToDictionary(m => m.Key, m => m.Value);
        }
        // 获取单个元数据的类型和值
        public (MetadataType type, object? value) GetMetadata(string key)
        {
            var metadata = _context.EntryMetadata.FirstOrDefault(m => m.EntryId == _entryId && m.Key == key);
            if (metadata != null)
            {
                return (metadata.Type, metadata.Value);
            }
            throw new KeyNotFoundException($"Metadata with key '{key}' not found.");
        }
    }
}

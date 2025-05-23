using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;

namespace LocalAniHubFront.Models
{

    public class EntryDataService
    {
        public UnifiedEntry CreateUnifiedEntry(
            Entry entry,
            EntryMetadata metadata,
            EntryRating rating,
            EntryTimeInfo timeInfo,
            List<Episode> episodes)
        {
            var unifiedEntry = new UnifiedEntry();

            // 添加 Entry 的属性
            foreach (var property in typeof(Entry).GetProperties())
            {
                var value = property.GetValue(entry) ?? string.Empty; // 如果值为 null，则使用 string.Empty
                unifiedEntry.AddProperty(property.Name, value);
            }

            //// 添加 EntryMetadata 的属性
            //foreach (var property in typeof(EntryMetadata).GetProperties())
            //{
            //    var value = property.GetValue(metadata) ?? string.Empty; // 如果值为 null，则使用 string.Empty
            //    unifiedEntry.AddProperty(property.Name, value);
            //}
            // 添加 EntryMetadata 的属性（跳过索引器）
            foreach (var property in typeof(EntryMetadata).GetProperties())
            {
                if (property.GetIndexParameters().Length > 0) // 跳过索引器
                    continue;

                var value = property.GetValue(metadata) ?? string.Empty;
                unifiedEntry.AddProperty(property.Name, value);
            }

            // 添加 EntryMetadata 的元数据字典内容
            foreach (var key in metadata.GetAllKeys())
            {
                var value = metadata.GetMetadataValue(key) ?? string.Empty;
                unifiedEntry.AddProperty(key, value);
            }

            // 添加 EntryRating 的属性
            foreach (var property in typeof(EntryRating).GetProperties())
            {
                var value = property.GetValue(rating) ?? string.Empty; // 如果值为 null，则使用 string.Empty
                unifiedEntry.AddProperty(property.Name, value);
            }

            // 添加 EntryTimeInfo 的属性
            foreach (var property in typeof(EntryTimeInfo).GetProperties())
            {
                var value = property.GetValue(timeInfo) ?? string.Empty; // 如果值为 null，则使用 string.Empty
                unifiedEntry.AddProperty(property.Name, value);
            }

            // 添加 Episode 的信息
            unifiedEntry.AddProperty("EpisodeCount", entry.EpisodeCount);

            return unifiedEntry;
        }
    }

}

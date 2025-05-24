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

            //格式化上映时间
            DateTime releaseDate = entry.ReleaseDate;
            string releaseDateTimeStr;
                // 用 entry.ReleaseDate 的年月日 + timeInfo.BroadcastTime 的时分
                var dt = new DateTime(releaseDate.Year, releaseDate.Month, releaseDate.Day);
                releaseDateTimeStr = dt.ToString("yyyy.M.d");
            unifiedEntry.AddProperty("ReleaseDate", releaseDateTimeStr);

            // 格式化播出时间
            DateTime? broadcastTime = timeInfo?.BroadcastTime;
            string broadcastTimeStr = broadcastTime != null
                ? $"{broadcastTime.Value.Hour:D2}:{broadcastTime.Value.Minute:D2}"
                : "";
            unifiedEntry.AddProperty("BroadcastTime", broadcastTimeStr);

            // 格式化播出星期为中文
            string broadcastWeekdayStr = timeInfo != null
                ? DayOfWeekToChinese(timeInfo.BroadcastWeekday)
                : "";
            unifiedEntry.AddProperty("BroadcastWeekday", broadcastWeekdayStr);

            return unifiedEntry;
        }

      
        // 新增：英文星期转中文
        private static string DayOfWeekToChinese(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
                DayOfWeek.Sunday => "周日",
                _ => ""
            };
        }
    }
}

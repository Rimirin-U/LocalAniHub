using BasicClassLibrary;
using LocalAniHubFront.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace LocalAniHubFront.ViewModels.Components
{
    public class Recent_SeasonTableViewModel
    {
        //统一的视图模型集合，绑定到 DataGrid 的 ItemsSource
        public ObservableCollection<UnifiedEntry> UnifiedEntries { get; set; } = new();

        //可见属性的设置列表
        public List<PropertySetting> VisibleProperties { get; set; } = new();

        //全局设置服务实例
        private readonly GlobalSettingsService _settingsService = GlobalSettingsService.Instance;

        // 用户设置的键名
        private const string VisiblePropertiesKey = "RecentSeasonTable_VisibleProperties";

        private readonly EntryService _entryService;
        public Recent_SeasonTableViewModel()
        {
            //    //加载用户设置
            //    LoadUserSettings();

            //    // 示例数据
            //    var entry = new Entry(1, "译名", "原名", DateTime.Now, DateTime.Now, "类别", 12, true, false);
            //    var metadata = new EntryMetadata();
            //    metadata.AddOrUpdateMetadata("导演", "张三");
            //    var rating = new EntryRating { Score = 8.5 };
            //    var timeInfo = new EntryTimeInfo(1, entry, DayOfWeek.Monday, DateTime.Now);
            //    var episodes = new List<Episode>
            //{
            //    new Episode(1, entry, 1),
            //    new Episode(1, entry, 2)

            _entryService = new EntryService(
                new EntryFetch(),
                new EntryManager(),
                new EpisodeManager(),
                new EntryRatingManager(),
                new EntryMetaDataManager(),
                new EntryTimeInfoManager()
            );

            LoadUserSettings();
            LoadEntriesFromDatabase();
        }

        private void LoadEntriesFromDatabase()
        {
            // 从数据库加载条目信息
            var entries = _entryService.EntryManager.Query(EntryManager.All);

            foreach (var entry in entries)
            {
                var metadata = _entryService.EntryMetaDataManager.Query(EntryMetaDataManager.ByEntryId(entry.Id)).FirstOrDefault() ?? new EntryMetadata();
                var rating = _entryService.EntryRatingManager.Query(EntryRatingManager.ByEntryId(entry.Id)).FirstOrDefault() ?? new EntryRating();
                var timeInfo = _entryService.EntryTimeInfoManager.Query(EntryTimeInfoManager.ByEntryId(entry.Id)).FirstOrDefault() ?? new EntryTimeInfo(entry.Id, DayOfWeek.Monday, DateTime.Now);
                var episodes = _entryService.EpisodeManager.Query(EpisodeManager.ByEntryId(entry.Id));

                // 整合数据并添加到集合
                UnifiedEntries.Add(new EntryDataService().CreateUnifiedEntry(entry, metadata, rating, timeInfo, episodes));
            }
        }
        private void LoadUserSettings()
        {
            // 从 GlobalSettingsService 获取用户设置
            //声明一个可空字符串变量 savedPropertiesJson，用于存储从全局设置服务中获取的可见属性设置的 JSON 字符串。
            string? savedPropertiesJson = null;
            try
            {
                savedPropertiesJson = _settingsService.GetValue(VisiblePropertiesKey);
            }
            catch (KeyNotFoundException)
            {
                // 如果没有找到用户设置，使用默认值
                savedPropertiesJson = null;
            }

            if (!string.IsNullOrEmpty(savedPropertiesJson))
            {
                // 如果用户设置存在，反序列化为属性列表
                VisibleProperties = JsonSerializer.Deserialize<List<PropertySetting>>(savedPropertiesJson) ?? GetDefaultProperties();
            }
            else
            {
                // 如果用户设置不存在，加载默认属性
                VisibleProperties = GetDefaultProperties();
            }
        }

         // 保存用户设置的方法
        public void SaveUserSettings()
        {
            // 将 VisibleProperties 序列化为 JSON
            string propertiesJson = JsonSerializer.Serialize(VisibleProperties);
            // 保存到 GlobalSettingsService
            _settingsService.SetValue(VisiblePropertiesKey, propertiesJson);
        }

        //获取默认属性的方法
        //该方法返回一个包含默认可见属性设置的 List<PropertySetting> 对象。
        //每个 PropertySetting 对象包含属性名 Name 和显示名称 DisplayName。
        private List<PropertySetting> GetDefaultProperties()
        {
            return new List<PropertySetting>
        {
            new PropertySetting { Name = "TranslatedName", DisplayName = "译名" },
            new PropertySetting { Name = "OriginalName", DisplayName = "原名" },
            new PropertySetting { Name = "ReleaseDate", DisplayName = "上映时间" },
            new PropertySetting { Name = "Category", DisplayName = "类别" },
            new PropertySetting { Name = "Score", DisplayName = "评分" },
            new PropertySetting { Name = "BroadcastWeekday", DisplayName = "播出星期" },
            new PropertySetting { Name = "BroadcastTime", DisplayName = "播出时间" },
            new PropertySetting { Name = "Episodes", DisplayName = "集数" }
        };
        }
    }

    // 属性设置类，用于定义列的显示名称和绑定的属性名
    public class PropertySetting
    {
        public string? Name { get; set; }// 属性名
        public string? DisplayName { get; set; }// 显示名称
    }
}
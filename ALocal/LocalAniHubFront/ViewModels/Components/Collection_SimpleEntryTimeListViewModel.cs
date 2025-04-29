using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Abstractions.Controls;
using BasicClassLibrary;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class Collection_SimpleEntryTimeListViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;

        // 年份分组的条目集合
        [ObservableProperty]
        private ObservableCollection<YearBlock> _yearBlocks = new();

        // 后端数据获取器
        private readonly EntryFetch _entryFetch;

        public Collection_SimpleEntryTimeListViewModel()
        {
            // 初始化后端数据获取器
            _entryFetch = new EntryFetch();
        }

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async void InitializeViewModel()
        {
            // 从后端加载数据
            var entries = await LoadEntriesAsync();

            // 按年份分组
            var groupedEntries = entries
                .GroupBy(e => e.CollectionDate.Year)//对加载的条目数据按收藏日期的年份进行分组
                .OrderByDescending(g => g.Key)//对分组结果按年份降序排序。
                .Select(g => new YearBlock
                {
                    Year = g.Key.ToString(),
                    EntryLines = new ObservableCollection<EntryLine>(
                        g.OrderBy(e => e.CollectionDate) // 按收藏时间排序
                         .Select(entry => new EntryLine
                         {
                             LineText = $"{entry.OriginalName} ({entry.TranslatedName}) - {entry.ReleaseDate:yyyy-MM-dd}",
                             Id = entry.Id
                         })
                    )
                });

            // 更新绑定数据
            YearBlocks = new ObservableCollection<YearBlock>(groupedEntries);

            _isInitialized = true;
        }

        private async Task<ObservableCollection<Entry>> LoadEntriesAsync()
        {
            try
            {
                // 使用后端数据获取器加载条目数据
                var entryInfoSets = await _entryFetch.FetchAsync();

                // 将 EntryInfoSet 转换为 Entry
                var entries = entryInfoSets.Select(info => new Entry(
                    id: 0, // 假设 ID 需要从其他地方获取？数据库？还是重新生成？有什么用？
                    translatedName: info.TranslatedName,
                    originalName: info.OriginalName,
                    releaseDate: info.ReleaseDate,
                    collectionDate: DateTime.Now, // 假设收藏日期为当前时间
                    category: info.Category,
                    episodeCount: 0, // 假设集数未知
                    hasUpdateTime: false,//假设该条目没有更新时间。
                    autoClearResources: false//假设该条目不会自动清除资源。
                ));

                return new ObservableCollection<Entry>(entries);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载条目数据失败: {ex.Message}");
                return new ObservableCollection<Entry>();
            }
        }
    }
    //private async Task<ObservableCollection<Entry>> LoadEntriesAsync()
    //    {
    //        try
    //        {
    //            using (var dbContext = new AppDbContext(/* 配置选项 */))
    //            {
    //                var entryInfoSets = await _entryFetch.FetchAsync();

    //                var entries = entryInfoSets.Select(info =>
    //                {
    //                    var existingEntry = dbContext.Entries
    //                        .FirstOrDefault(e => e.OriginalName == info.OriginalName);

    //                    if (existingEntry != null)
    //                    {
    //                        return new Entry(
    //                            id: existingEntry.Id,
    //                            translatedName: info.TranslatedName,
    //                            originalName: info.OriginalName,
    //                            releaseDate: info.ReleaseDate,
    //                            collectionDate: existingEntry.CollectionDate,
    //                            category: info.Category,
    //                            episodeCount: existingEntry.EpisodeCount,
    //                            hasUpdateTime: existingEntry.HasUpdateTime,
    //                            autoClearResources: existingEntry.AutoClearResources
    //                        );
    //                    }
    //                    else
    //                    {
    //                        var newEntry = new Entry(
    //                            id: 0,
    //                            translatedName: info.TranslatedName,
    //                            originalName: info.OriginalName,
    //                            releaseDate: info.ReleaseDate,
    //                            collectionDate: DateTime.Now,
    //                            category: info.Category,
    //                            episodeCount: 0,
    //                            hasUpdateTime: false,
    //                            autoClearResources: false
    //                        );

    //                        dbContext.Entries.Add(newEntry);
    //                        dbContext.SaveChanges();

    //                        return newEntry;
    //                    }
    //                });

    //                return new ObservableCollection<Entry>(entries);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            System.Diagnostics.Debug.WriteLine($"加载条目数据失败: {ex.Message}");
    //            return new ObservableCollection<Entry>();
    //        }
    //    }

    public class YearBlock
    {
        public string Year { get; set; }
        public ObservableCollection<EntryLine> EntryLines { get; set; }
    }

    public class EntryLine
    {
        public string LineText { get; set; } // 示例: "原名 (译名) - 上映时间"
        public int Id { get; set; } // 条目 ID
    }
}

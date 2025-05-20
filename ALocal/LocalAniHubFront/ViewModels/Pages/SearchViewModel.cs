using BasicClassLibrary;
using LocalAniHubFront.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class SearchViewModel : ObservableObject
    {
        public List<YearItem> YearItems { get; }
        public List<MonthItem> MonthItems { get; }

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private int selectedMonth;

        [ObservableProperty]
        private ObservableCollection<EntryInfoSet> entryInfoSets = new();
        // 所有EntryInfoSet的列表
        // 更改查询时间时应重置该列表
        // 保持原有命令签名但优化实现
        [RelayCommand(CanExecute = nameof(IsNotSearching))]
        private async Task Search()
        {
            IsSearching = true;
            try
            {
                var sourceType = int.TryParse(
                    GlobalSettingsService.Instance.GetValue("defaultEntryFetchSource"),
                    out var type) ? type : 0;

                var fetcher = new EntryFetch(sourceType, SelectedYear, SelectedMonth);
                var results = await fetcher.FetchAsync();

                EntryInfoSets = new ObservableCollection<EntryInfoSet>(results);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"搜索失败: {ex.Message}");
                EntryInfoSets.Clear();
            }
            finally
            {
                IsSearching = false;
            }
        }
        // 新增加载状态指示
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotSearching))]
        private bool _isSearching;

        public bool IsNotSearching => !IsSearching;

        public SearchViewModel()
        {
            // 初始化年份列表（2020~2025）
            YearItems = Enumerable.Range(2020, 6)
                .Select(y => new YearItem
                {
                    Value = y,
                    DisplayName = $"{y}年"
                }).ToList();
            // 初始化月份列表（1/4/7/10）
            MonthItems = new List<MonthItem>
            {
                new MonthItem { Value = 1, DisplayName = "1月" },
                new MonthItem { Value = 4, DisplayName = "4月" },
                new MonthItem { Value = 7, DisplayName = "7月" },
                new MonthItem { Value = 10, DisplayName = "10月" },
            };
            // 初始化初始值
            var now = DateTime.Now;
            int month = MonthItems
                .Where(m => now.Month >= m.Value)
                .Select(m => m.Value)
                .DefaultIfEmpty(1)
                .Max();// 找到当前月所在或之前的最接近的季度月
            SelectedYear = now.Year;
            SelectedMonth = month;
            /*
            SelectedYearItem = YearItems.FirstOrDefault(y => y.Value == now.Year);
            SelectedMonthItem = MonthItems.FirstOrDefault(m => m.Value == month);
            */

            // 使用EntryFetch类来进行搜索，初始时需要根据默认（当前）年月初始化一个EntryFetch
            // 在每次调用“搜索”时 都要根据当前选择的时间创建新的EntryFetch类
        }
    }

    public class MonthItem
    {
        public int Value { get; set; }
        public string DisplayName { get; set; }

        public override string ToString() => DisplayName;
    }

    public class YearItem
    {
        public int Value { get; set; }
        public string DisplayName { get; set; }

        public override string ToString() => DisplayName;
    }
}

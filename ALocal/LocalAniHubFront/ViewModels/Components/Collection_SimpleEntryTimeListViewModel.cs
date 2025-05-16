using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;
using LocalAniHubFront.Models;

namespace LocalAniHubFront.ViewModels.Components
{
    public partial class Collection_SimpleEntryTimeListViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private ObservableCollection<YearBlock> _yearBlocks = new();

        // 无参数构造函数
        public Collection_SimpleEntryTimeListViewModel() { }

        public Task OnNavigatedToAsync()
        {
            LoadEntries();
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void LoadEntries()
        {
            // 直接使用 EntryManager 读取数据（无需DbContext）
            var entries = new EntryManager()
                .Query(Manager<Entry>.All)  // 获取所有条目
                .OrderByDescending(e => e.CollectionDate)
                .ToList();

            // 按收藏年份分组并排序
            var groupedEntries = entries
                .GroupBy(e => e.CollectionDate.Year)
                .OrderByDescending(g => g.Key);

            YearBlocks.Clear();

            // 构建年份区块
            foreach (var yearGroup in groupedEntries)
            {
                var yearBlock = new YearBlock
                {
                    Year = yearGroup.Key.ToString(),
                    EntryLines = new ObservableCollection<EntryLine>()
                };

                // 添加条目行
                foreach (var entry in yearGroup)
                {
                    yearBlock.EntryLines.Add(new EntryLine
                    {
                        Id = entry.Id, // 直接使用模型定义的 Id 属性
                        LineText = $"{entry.OriginalName} ({entry.TranslatedName}) - {entry.ReleaseDate:yyyy-MM-dd}"
                    });
                }

                YearBlocks.Add(yearBlock);
            }
        }

        //[RelayCommand]
        //private void OpenEntry(int entryId)
        //{
        //    // TODO: 实现打开条目详情逻辑
        //    // 示例：_navigationService.NavigateTo($"EntryDetail?id={entryId}");
        //}
    }

    
}
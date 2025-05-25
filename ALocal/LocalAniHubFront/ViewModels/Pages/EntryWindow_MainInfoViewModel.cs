using BasicClassLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Abstractions.Controls;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class EntryWindow_MainInfoViewModel : ObservableObject
    {
        // 条目基本信息 
        [ObservableProperty]
        private string _subTitle = string.Empty;

        [ObservableProperty]
        private string _kind = string.Empty;

        [ObservableProperty]
        private string _state = string.Empty;

        [ObservableProperty]
        private string _timeString = string.Empty;

        [ObservableProperty]
        private int _score = 0;

        // 标签集合
        [ObservableProperty]
        private ObservableCollection<string> _tags = new();

        // 集数信息
        public enum EpisodeState { Watched = 0, Unwatched = 1, Unreleased = 2 }
        public readonly record struct EpisodeTempData(int Number, int EpisodeId, EpisodeState State);
        [ObservableProperty]
        private ObservableCollection<EpisodeTempData> _episodes = new();
        // 元数据信息
        public record MetadataTempData(string KeyString, string ValueString, int Row, int Column);
        [ObservableProperty]
        private ObservableCollection<MetadataTempData> _metadata = new();

        [ObservableProperty]
        private int _rowCount = 1;

        // 管理器实例
        private readonly EntryManager _entryManager = new();
        private readonly EntryMetaDataManager _metaDataManager = new();
        private readonly EntryRatingManager _ratingManager = new();
        private readonly EpisodeManager _episodeManager = new();
        private readonly EntryTimeInfoManager _timeInfoManager = new();
        private int _entryId;
        private bool _isInitialized = false;
        public EntryWindow_MainInfoViewModel(int entryId)
        {
            _entryId = entryId;
            LoadEntryData(_entryId);
        }

        /* 已改为在构造函数中调用Load系列逻辑
        public async Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                await LoadEntryData(_entryId);
                _isInitialized = true;
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;
        */

        private void LoadEntryData(int entryId)
        {
            // 1. 加载核心条目数据
            var entry = _entryManager.Query(e => e.Id == entryId).FirstOrDefault();
            if (entry == null) return;

            // 2. 设置基础信息
            SubTitle = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle") == "1"
                ? entry.OriginalName
                : entry.TranslatedName;

            Kind = entry.Category;
            // 加载并设置播出时间信息
            var timeInfo = _timeInfoManager.Query(EntryTimeInfoManager.ByEntryId(entryId)).FirstOrDefault();
            if (timeInfo != null)
            {
                TimeString = $"{entry.ReleaseDate:yyyy.M.d}起 每周{GetChineseWeekday(timeInfo.BroadcastWeekday)} {timeInfo.BroadcastTime:HH:mm}";
            }
            else
            {
                TimeString = $"{entry.ReleaseDate:yyyy.M.d}起 时间未设置";
            }

            // 3. 并行加载其他数据

            LoadTags(entryId);
            LoadRating(entryId);
            LoadEpisodes(entryId);
            LoadMetadata(entryId);
        }
        private string GetChineseWeekday(DayOfWeek weekday)
        {
            return weekday switch
            {
                DayOfWeek.Sunday => "日",
                DayOfWeek.Monday => "一",
                DayOfWeek.Tuesday => "二",
                DayOfWeek.Wednesday => "三",
                DayOfWeek.Thursday => "四",
                DayOfWeek.Friday => "五",
                DayOfWeek.Saturday => "六",
                _ => "?"
            };
        }

        private void LoadTags(int entryId)
        {
            var metadata = _metaDataManager.Query(EntryMetaDataManager.ByEntryId(entryId)).FirstOrDefault();
            Tags = metadata != null
                ? new ObservableCollection<string>(metadata.GetTags())
                : new ObservableCollection<string>();
        }
        private void LoadRating(int entryId)
        {
            var rating = _ratingManager.Query(EntryRatingManager.ByEntryId(entryId)).FirstOrDefault();
            Score = (int)(rating?.Score ?? 0);
        }
        private void LoadEpisodes(int entryId)
        {
            // 获取条目数据
            var entry = _entryManager.Query(e => e.Id == entryId).FirstOrDefault();
            if (entry == null) return;

            var episodes = _episodeManager.Query(EpisodeManager.ByEntryId(entryId));

            var episodeList = new List<EpisodeTempData>();

            foreach (var episode in episodes)
            {
                // 使用 entry 计算 airDateTime
                DateTimeOffset? airDateTime = entry.ReleaseDate.AddDays(((episode.EpisodeNumber - 1) * 7.0));
                if (airDateTime.HasValue && airDateTime.Value <= DateTimeOffset.Now)
                {
                    episode.State = BasicClassLibrary.State.Watched;
                }
                else
                {
                    episode.State = BasicClassLibrary.State.NotWatched;
                }

                episodeList.Add(new EpisodeTempData(
                    episode.EpisodeNumber,
                    episode.Id,
                    episode.State == BasicClassLibrary.State.Watched ? EpisodeState.Watched : EpisodeState.Unwatched
                ));
            }

            Episodes = new ObservableCollection<EpisodeTempData>(episodeList);
            CalculateWatchState();
        }
        private void LoadMetadata(int entryId)
        {
            var metadata = _metaDataManager.Query(EntryMetaDataManager.ByEntryId(entryId)).FirstOrDefault();
            if (metadata == null) return;

            var displayData = new ObservableCollection<MetadataTempData>();

            // 用 GetAllKeys() 获取所有键
            var allKeys = metadata.GetAllKeys()
                .Where(key => !key.StartsWith("Tag", StringComparison.OrdinalIgnoreCase))
                .ToList();

            int rowCount = (int)Math.Ceiling(allKeys.Count / 3.0);
            for (int i = 0; i < allKeys.Count; i++)
            {
                int row = i / 3;
                int column = i % 3;
                var key = allKeys[i];
                var value = metadata.GetMetadataValue(key) ?? "";
                displayData.Add(new MetadataTempData(key, value, row, column));
            }

            RowCount = rowCount;
            Metadata = displayData;
        }
        private void CalculateWatchState()
        {
            bool allWatched = _episodes.All(e => e.State == EpisodeState.Watched);
            bool allUnwatched = _episodes.All(e => e.State == EpisodeState.Unwatched);
            bool someWatched = _episodes.Any(e => e.State == EpisodeState.Watched);
            bool someUnwatched = _episodes.Any(e => e.State == EpisodeState.Unwatched);

            if (allWatched)
            {
                State = "已看";
            }
            else if (allUnwatched)
            {
                State = "未看";
            }
            else if (someWatched && someUnwatched)
            {
                State = "在看";
            }
            else
            {
                State = "抛弃"; // 如果有未定义的状态，则默认为抛弃
            }
        }

        [RelayCommand]
        private void EpisodeButton_Click(int episodeId)
        {
            var episode = Episodes.FirstOrDefault(e => e.EpisodeId == episodeId);
            if (episode.State == EpisodeState.Unreleased) return;

            var newState = episode.State == EpisodeState.Watched
                ? EpisodeState.Unwatched
                : EpisodeState.Watched;

            var updatedEpisodes = Episodes.Select(e =>
                e.EpisodeId == episodeId ? e with { State = newState } : e
            ).ToList();

            // 更新数据库观看状态
            UpdateWatchedStatusInDatabase(episodeId, newState == EpisodeState.Watched);

            Episodes = new ObservableCollection<EpisodeTempData>(updatedEpisodes);
        }
        private void UpdateWatchedStatusInDatabase(int episodeId, bool isWatched)
        {
            var episode = _episodeManager.FindById(episodeId);
            if (episode != null)
            {
                // 根据 isWatched 参数更新 State 属性
                episode.State = isWatched ? BasicClassLibrary.State.Watched : BasicClassLibrary.State.NotWatched; // 显式使用 BasicClassLibrary.State
                // 保存更改到数据库
                _episodeManager.Modify(episode);
            }
        }

        [RelayCommand]
        // 添加 SetScore 方法
        public void SetScore(int score)
        {
            // 设置评分
            Score = score;

            // 保存评分到数据库
            var rating = _ratingManager.Query(EntryRatingManager.ByEntryId(_entryId)).FirstOrDefault();
            if (rating == null)
            {
                rating = new EntryRating { EntryId = _entryId };
                _ratingManager.Add(rating);
            }
            else
            {
                rating.Score = score;
                _ratingManager.Modify(rating);
            }
        }
    }
}
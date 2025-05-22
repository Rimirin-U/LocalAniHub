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
    public partial class EntryWindow_MainInfoViewModel : ObservableObject, INavigationAware
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
        private readonly MaterialManager _materialManager = new();
        private readonly EpisodeManager _episodeManager = new();
        private int _entryId;
        private bool _isInitialized = false;
        public EntryWindow_MainInfoViewModel(int entryId)
        {
            _entryId = entryId;
        }
        public async Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                await LoadEntryData(_entryId);
                _isInitialized = true;
            }
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private async Task LoadEntryData(int entryId)
        {
            // 1. 加载核心条目数据
            var entry = await Task.Run(() => _entryManager.Query(e => e.Id == entryId).FirstOrDefault());
            if (entry == null) return;

            // 2. 设置基础信息
            SubTitle = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle") == "1"
                ? entry.OriginalName
                : entry.TranslatedName;

            Kind = entry.Category;
            TimeString = $"{entry.ReleaseDate:yyyy.M.d}起 每周X XX:XX";

            // 3. 并行加载其他数据
            await Task.WhenAll(
                LoadTags(entryId),
                LoadRating(entryId),
                LoadEpisodes(entryId),
                LoadMetadata(entryId)
            );
        }
        private async Task LoadTags(int entryId)
        {
            var metadata = await Task.Run(() => _metaDataManager.Query(EntryMetaDataManager.ByEntryId(entryId)).FirstOrDefault());
            Tags = metadata != null
                ? new ObservableCollection<string>(metadata.GetTags())
                : new ObservableCollection<string>();
        }
        private async Task LoadRating(int entryId)
        {
            var rating = await Task.Run(() => _ratingManager.Query(EntryRatingManager.ByEntryId(entryId)).FirstOrDefault());
            Score = (int)(rating?.Score ?? 0);
        }
        private async Task LoadEpisodes(int entryId)
        {
            var episodes = await Task.Run(() => _episodeManager.Query(EpisodeManager.ByEntryId(entryId)));

            Episodes = new ObservableCollection<EpisodeTempData>(
                episodes.Select(e => new EpisodeTempData(
                    e.EpisodeNumber,
                    e.Id,
                    (EpisodeState)e.State
                ))
            );
        }
        private async Task LoadMetadata(int entryId)
        {
            var metadata = await Task.Run(() => _metaDataManager.Query(EntryMetaDataManager.ByEntryId(entryId)).FirstOrDefault());
            if (metadata == null) return;

            var displayData = new List<MetadataTempData>();

            // Assuming the keys are stored in the private _metadataValues dictionary
            var keys = metadata.GetType()
                               .GetProperty("_metadataValues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                               .GetValue(metadata) as Dictionary<string, string>;

            if (keys != null)
            {
                var filteredKeys = keys.Keys.Where(k => !k.StartsWith("Tag_"));
                int index = 0;
                foreach (var key in filteredKeys)
                {
                    if (keys[key] is string value)
                    {
                        displayData.Add(new MetadataTempData(
                            key, value, index / 3, index % 3
                        ));
                        index++;
                    }
                }
            }

            Metadata = new ObservableCollection<MetadataTempData>(displayData);
            RowCount = (int)Math.Ceiling(displayData.Count / 3.0);
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
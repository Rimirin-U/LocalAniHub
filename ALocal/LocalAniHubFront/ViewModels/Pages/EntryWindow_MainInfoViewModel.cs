using BasicClassLibrary;
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
        public record EpisodeTempData(int Number, int EpisodeId, EpisodeState State);
        [ObservableProperty]
        private ObservableCollection<EpisodeTempData> _episodes = new();

        // 元数据信息
        public record MetadataTempData(string KeyString, string ValueString, int Row, int Column);
        [ObservableProperty]
        private ObservableCollection<MetadataTempData> _metadata = new();

        [ObservableProperty]
        private int _rowCount = 1;

        // 管理器实例
        private readonly EntryMetaDataManager _metaDataManager = new();
        private readonly EntryRatingManager _ratingManager = new();
        private readonly MaterialManager _materialManager = new();
        private readonly AppDbContext _dbContext = new();
        private int _entryId;
        private bool _isInitialized = false;

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
            {
                InitializeViewModel();
            }
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            _isInitialized = true;

            // 这里应该从导航参数获取entryId
            // _entryId = navigationParameter;

            LoadEntryData();
        }
        private EntryInfoSet? entryInfo;
        private void LoadEntryData()
        {
            // 加载条目基本信息
            entryInfo = GetEntryInfo(_entryId); // 需要实现获取条目信息的方法
            var entryMetadata = _metaDataManager.Query(EntryMetaDataManager.ByEntryId(_entryId)).FirstOrDefault();
            // 保存原名和译名
            string OriginalName = entryInfo.OriginalName;
            string TranslatedName = entryInfo.TranslatedName;

            // 根据全局设置决定显示译名还是原名
            var titleSetting = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");
            SubTitle = titleSetting == "1" ? OriginalName : TranslatedName;
            /// 设置种类
            Kind = entryInfo.Category;
            // 设置时间字符串
            TimeString = $"{entryInfo.ReleaseDate:yyyy.M.d}起 每周X XX:XX"; // 需要补充星期和具体时间

            // 加载标签
            if (entryMetadata != null)
            {
                Tags = new ObservableCollection<string>(entryMetadata.GetTags());
            }
            else
            {
                Tags = new ObservableCollection<string>(); // 如果没有元数据，初始化为空集合
            }
            // 加载评分
            var rating = _ratingManager.Query(EntryRatingManager.ByEntryId(_entryId)).FirstOrDefault();
            Score = (int)(rating?.Score ?? 0);

            // 加载集数信息
            LoadEpisodes();

            // 加载元数据
            LoadMetadata();
        }

        private void LoadEpisodes()
        {
            var dbEpisodes = _dbContext.Episodes
                .Where(e => e.EntryId == _entryId)
                .OrderBy(e => e.EpisodeNumber)
                .ToList();

            var displayEpisodes = dbEpisodes.Select(e => new EpisodeTempData(
                Number: e.EpisodeNumber,
                EpisodeId: e.Id,
                State: e.State //Episode属性State
            )).ToList();

            Episodes = new ObservableCollection<EpisodeTempData>(displayEpisodes);
        }
        private void LoadMetadata()
        {
            var metadataList = new List<MetadataTempData>();
            var entryMetadata = _metaDataManager.Query(EntryMetaDataManager.ByEntryId(_entryId)).FirstOrDefault();

            if (entryMetadata != null)
            {
                // 筛选要显示的元数据键
                var displayKeys = new List<string>
                {
                    "导演", "编剧", "制作公司", "原作", "官方网站"
                };

                int index = 0;
                foreach (var key in displayKeys)
                {
                    if (entryMetadata.GetMetadataValue(key) is { } value)
                    {
                        int row = index / 3;
                        int column = index % 3;
                        metadataList.Add(new MetadataTempData(key, value, row, column));
                        index++;
                    }
                }

                // 计算需要的行数
                RowCount = (int)Math.Ceiling(metadataList.Count / 3.0);
            }

            Metadata = new ObservableCollection<MetadataTempData>(metadataList);
        }

        [RelayCommand]
        private void EpisodeButton_Click(int episodeId)
        {
            var episode = Episodes.FirstOrDefault(e => e.EpisodeId == episodeId);
            if (episode?.State == EpisodeState.Unreleased) return;

            var newState = episode!.State == EpisodeState.Watched
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
            // 实现建议：
            // 1. 更新用户观看记录表
            // 2. 或更新Episode的IsWatched字段
        }

        [RelayCommand]
        private void TextBlock_MouseLeftButtonDown(int starValue)
        {
            // 设置评分
            Score = starValue;

            // 保存评分到数据库
            var rating = _ratingManager.Query(EntryRatingManager.ByEntryId(_entryId)).FirstOrDefault();
            if (rating == null)
            {
                rating = new EntryRating { EntryId = _entryId };
                _ratingManager.Add(rating);
            }
            else
            {
                rating.Score = starValue;
                _ratingManager.Modify(rating);
            }
        }

        [RelayCommand]
        private void ResourceButton_Click()
        {
            // 打开资源管理
        }

        [RelayCommand]
        private void NoteButton_Click()
        {
            // 打开笔记管理
        }

        [RelayCommand]
        private void HistoryButton_Click()
        {
            // 打开观看历史
        }

        [RelayCommand]
        private void MaterialButton_Click()
        {
            // 打开素材管理
        }

        private EntryInfoSet GetEntryInfo(int entryId)
        {
            // 这里应该是从数据库或API获取条目信息的实现
            // 返回示例数据
            var metadata = new Dictionary<string, string>
            {
                ["导演"] = "山田尚子",
                ["编剧"] = "吉田玲子",
                ["制作公司"] = "京都动画",
                ["Tag_1"] = "音乐",
                ["Tag_2"] = "青春",
                ["Tag_3"] = "校园"
            };

            return new EntryInfoSet(
                translatedName: "轻音少女",
                originalName: "けいおん!",
                releaseDate: new DateTime(2009, 4, 2),
                category: "TV动画",
                metadata: metadata);
        }
    }
}
using BasicClassLibrary;
using LocalAniHubFront.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LocalAniHubFront.ViewModels.Windows
{
    public partial class AddEntryWindowViewModel : ObservableObject
    {
        private readonly EntryInfoSet _entryInfoSet;

        public AddEntryWindowViewModel() {
            KvImage = System.Drawing.Image.FromFile(@"Assets/DefaultKeyVisual.png");
            InitializeDefaultValues();
        }

        public AddEntryWindowViewModel(EntryInfoSet entryInfoSet)
        {
            _entryInfoSet = entryInfoSet;
            InitializeFromEntryInfoSet();
            InitializeDefaultValues();
        }

        [ObservableProperty]
        private string originalName;

        [ObservableProperty]
        private string translatedName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(KvImageSource))]
        private Image kvImage;
        public ImageSource KvImageSource => ImageHelper.ToImageSource(KvImage); // 真正用于绑定


        [ObservableProperty]
        private DateTime? releaseDate;


        [ObservableProperty]
        private string broadcastTimeString = "00:00"; // 默认值改为字符串形式

        [ObservableProperty]
        private string category;

        [ObservableProperty]
        private int episodeCount = 12;

        // [ObservableProperty]
        // private State state = State.Watching;
        public ObservableCollection<string> States { get; } = new ObservableCollection<string>(new List<string>
        {
          "未看",
          "在看",
          "已看",
          "抛弃"
        });
        private int stateID=1;
        public int StateID
        {
            get => stateID;
            set => SetProperty(ref stateID, value);
        }
        // 根据 StateID 获取对应的 State 枚举值
        public State GetStateFromStateID()
        {
            return (State)StateID;
        }

        [ObservableProperty]
        private bool hasUpdateTime=false;

        [ObservableProperty]
        private bool autoClearResources=false;


        //元数据（用于构建EntryMetadata对象）
        // KeyValuePair为只读，因此自定义了可读写的MutableKeyValuePair<TKey, TValue>
        [ObservableProperty]
        private ObservableCollection<MutableKeyValuePair<string, string>> metadataItems = new();
        // 元数据（来自输入（EntryInfoSet））
        [ObservableProperty]
        private ObservableCollection<KeyValuePair<string, string>> metadataFromEntryInfoSet = new();

        [ObservableProperty]
        private string tagsString;

        public DateTime CollectionDate => DateTime.Now;
        public string MaterialSubFolder =>
       $"[{ReleaseDate?.ToString("yyyyMM")}]{OriginalName}";
        public string KeyVisualFileName =>
        $"{OriginalName}KV.png";
        public DayOfWeek BroadcastWeekday => ReleaseDate.HasValue ? ReleaseDate.Value.DayOfWeek : DayOfWeek.Monday;
        public double Score { get; set; } = 0.0;
        public List<Episode> Episodes { get; private set; } = new();

        [RelayCommand]
        private void AddEmptyPair()
        {
            MetadataItems.Add(new MutableKeyValuePair<string, string>("数据项", ""));
        }

        [RelayCommand]
        private void RemovePair(MutableKeyValuePair<string, string> toRmv)
        {
            MetadataItems.Remove(toRmv);
        }

        public void AddPair(KeyValuePair<string, string> pair)
        {
            MutableKeyValuePair<string, string> newPair = new(pair.Key, pair.Value);
            MetadataItems.Add(newPair);
        }

        public void ChangeKeyVisual(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            KvImage = Image.FromFile(filePath);
            // 更新图像源
            OnPropertyChanged(nameof(KvImageSource));
        }
        private void InitializeFromEntryInfoSet()
        {
            OriginalName = _entryInfoSet.OriginalName;
            TranslatedName = _entryInfoSet.TranslatedName;
            ReleaseDate = _entryInfoSet.ReleaseDate;
            Category = _entryInfoSet.Category;
            KvImage = _entryInfoSet.KeyVisualImage;

            // 初始化元数据
            /*oreach (var pair in _entryInfoSet.Metadata)
             {

                 MetadataItems.Add(new MutableKeyValuePair<string, string>(pair.Key, pair.Value));
             }*/
            foreach (var pair in _entryInfoSet.Metadata)
            {
                MetadataFromEntryInfoSet.Add(pair);
            }
            // 提取 TAGS
            // TagsString = string.Join("/", _entryInfoSet.Metadata.Values.Where(v => !string.IsNullOrEmpty(v)));
            TagsString = string.Join("/",
         _entryInfoSet.Metadata
         .Where(kvp => kvp.Key.Equals("tag", StringComparison.OrdinalIgnoreCase))
         .Select(kvp => kvp.Value)
         .Where(v => !string.IsNullOrEmpty(v)));
        }
        private List<string> GetKeywordsFromTags()
        {
            return TagsString.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void InitializeDefaultValues()
        {
            if (!ReleaseDate.HasValue)
                ReleaseDate = DateTime.Now;

            if (string.IsNullOrEmpty(Category))
                Category = "动画";

            if (EpisodeCount <= 0)
                EpisodeCount = 12;

            if (Score < 0)
                Score = 0;
        }
        private bool TryParseBroadcastTime(out DateTime result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(BroadcastTimeString))
                return false;

            try
            {
                // 可以处理如 "20:30" 或 "8:30 PM"
                var timeSpan = TimeSpan.Parse(BroadcastTimeString.Trim());
                var today = DateTime.Today;
                result = new DateTime(today.Year, today.Month, today.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CheckDataValidity()
        {
            if (string.IsNullOrWhiteSpace(OriginalName))
            {
                MessageBox.Show("原名不能为空！");
                return false;
            }

            if (!ReleaseDate.HasValue)
            {
                MessageBox.Show("首播日期不能为空！");
                return false;
            }

            if (EpisodeCount <= 0)
            {
                MessageBox.Show("集数必须大于0！");
                return false;
            }
            //校验 BroadcastTimeString 是否合法格式
            if (!TryParseBroadcastTime(out _))
            {
                MessageBox.Show("播出时间格式不正确，请输入类似 \"20:30\" 的时间");
                return false;
            }
            //校验 Metadata 键是否为空或重复
            HashSet<string> keys = new(StringComparer.OrdinalIgnoreCase);
            foreach (var item in MetadataItems)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    MessageBox.Show("Metadata 中存在空的键，请修正后重试。");
                    return false;
                }
                if (keys.Contains(item.Key))
                {
                    MessageBox.Show($"Metadata 中存在重复的键：{item.Key}，请勿重复添加。");
                    return false;
                }
                keys.Add(item.Key);
            }

            /*if (BroadcastTimeString.TimeOfDay.TotalSeconds == 0 && !ReleaseDate.HasValue)
            {
                MessageBox.Show("请设置正确的播出时间！");
                return false;
            }*/

            return true;
        }

        public void Save()
        {
            try
            {
                // 解析 BroadcastTimeString 成 DateTime
                if (!TryParseBroadcastTime(out var broadcastTime))
                {
                    MessageBox.Show("播出时间格式错误，无法保存。");
                    return;
                }
                // 构建 Entry 对象
                var entry = new Entry(
                    TranslatedName,
                    OriginalName,
                    ReleaseDate ?? CollectionDate,
                    CollectionDate,
                    Category,
                    EpisodeCount,
                    GetStateFromStateID(),
                    MaterialSubFolder,
                    KeyVisualFileName,
                    HasUpdateTime,
                    AutoClearResources,
                    GetKeywordsFromTags());
                var entryManager = new EntryManager();
                entryManager.Add(entry);
                int entryId = entry.Id;
                if (entryId <= 0)
                {
                    MessageBox.Show("条目保存失败，未获得有效 ID。");
                    return;
                }
                // 保存 KV 图片到文件系统！！！
                var materialService = new MaterialService();
               if (KvImage != null && !string.IsNullOrEmpty(KeyVisualFileName))
               {
                    materialService.AddMaterial(entry, KvImage, KeyVisualFileName);
               }
                // 构建 EntryTimeInfo
                var entryTimeInfo = new EntryTimeInfo(
                    entry.Id,
                    BroadcastWeekday,
                    broadcastTime);
                var timeInfoManager = new EntryTimeInfoManager();
                timeInfoManager.Add(entryTimeInfo);
                // 构建 EntryMetadata
                var entryMetadata = new EntryMetadata(entry.Id);
                foreach (var item in MetadataItems)
                {
                    entryMetadata.AddOrUpdateMetadata(item.Key, item.Value);
                }
                var metaDataManager = new EntryMetaDataManager();
                metaDataManager.Add(entryMetadata);
                // 构建 EntryRating
                var entryRating = new EntryRating(entry.Id, Score);
                var ratingManager = new EntryRatingManager();
                ratingManager.Add(entryRating);
                // 构建所有 Episode
                var episodeManager = new EpisodeManager();
                Episodes.Clear();
                for (int i = 1; i <= EpisodeCount; i++)
                {
                    var episode = new Episode(entryId, i, GetStateFromStateID() == State.Watched ? State.Watched : State.NotWatched);
                    episodeManager.Add(episode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}");
            }
            /*catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "无内部异常信息";
                MessageBox.Show($"数据库更新失败：{ex.Message}\n\n详细信息：{innerMessage}");
            }*/
        }
        //!这里保存的路径可能有点问题
      /* private void SaveKeyVisualToFileSystem()
       {
            if (KvImage == null || string.IsNullOrEmpty(KeyVisualFileName)) return;
            string folderPath = Path.Combine("Material", MaterialSubFolder, "KV");
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{KeyVisualFileName}.png");

            // 使用 System.Drawing.Image.Save() 直接保存图片
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                KvImage.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
            }
       }*/
    }
    public class MutableKeyValuePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public MutableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}

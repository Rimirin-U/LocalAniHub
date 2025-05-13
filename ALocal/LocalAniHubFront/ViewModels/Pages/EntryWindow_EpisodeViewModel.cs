using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
namespace LocalAniHubFront.ViewModels.Pages
{
    // record struct 定义
    public readonly record struct ResourceData(int ResourceId, string ResourceName);
    public readonly record struct NoteData(int NoteId, string NoteName);
    public partial class EntryWindow_EpisodeViewModel : ObservableObject
    {
        // 原始数据源
        private readonly Entry _entry;
        private readonly Episode _episode;
        // 资源和笔记相关服务
        private readonly ResourceManager _resourceManager;
        private readonly NoteManager _noteManager;
        private readonly VideoPlayService _videoPlayService;
        private readonly LogManager _logManager;
        //private readonly INavigationService _navigationService;

        // 资源和笔记的数据源
        public ObservableCollection<ResourceData> ResourcesData { get; } = new();
        public ObservableCollection<NoteData> NotesData { get; } = new();//这里与前端命名不一致，建议前端加个s-NotesData
        // 观看状态相关属性
        [ObservableProperty]
        private string stateSelectedOption;

        public ObservableCollection<string> StateOptionList { get; } =
            new() { "未看", "在看", "已看" };
        [ObservableProperty]
        private string watchedDate = "";

        [ObservableProperty]
        private string watchedTime = "";

        [ObservableProperty]
        private string shortComment;
        // 标题显示模式：Original / Translated
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SubTitle))]
        private string selectedTitleMode;

        public EntryWindow_EpisodeViewModel(
            Entry entry,
            Episode episode,
            ResourceManager resourceManager,
            NoteManager noteManager,
            VideoPlayService videoPlayService,
            LogManager logManager)
        {
            _entry = entry;
            _episode = episode;
            _resourceManager = resourceManager;
            _noteManager = noteManager;
            _videoPlayService = videoPlayService;
            _logManager = logManager;
            //_navigationService = navigationService;
            //保留属性
            //ShortComment = _episode.ShortComment;

            LoadInitialState();

            LoadResources();
            LoadNotes();
        }
        private void LoadInitialState()
        {
            // 加载标题显示模式
            var titleDisplayMode = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");
            if (int.TryParse(titleDisplayMode, out int mode))
            {
                SelectedTitleMode = mode == 0 ? "Original" : "Translated";
            }
            else
            {
                SelectedTitleMode = "Original";
            }

            // 默认观看状态为“未看”
            StateSelectedOption = "未看";
        }
        private void LoadResources()
        {
            var resource = _resourceManager.FindById(_episode.Id);
            if (resource != null)
            {
                ResourcesData.Add(new ResourceData(resource.Id, System.IO.Path.GetFileName(resource.ResourcePath)));
            }
        }
        private void LoadNotes()
        {
            var note = _noteManager.FindById(_episode.Id);
            if(note != null)
            {
                NotesData.Add(new NoteData(note.Id, note.NoteTitle));//这个报错不用管，是因为没有合并后端更新后的代码
            }
        }
        [RelayCommand]
        private void OnResourcePlay(int resourceId)
        {
            var resource = _resourceManager.FindById(resourceId);
            if (resource != null && !string.IsNullOrEmpty(resource.ResourcePath))
            {
                _videoPlayService.Play(resource.ResourcePath);
            }
        }
        [RelayCommand]
        private void OnResourceDelete(int resourceId)
        {
            _resourceManager.DeleteResource(resourceId);
            //ResourcesData.Remove(ResourcesData.FirstOrDefault(r => r.ResourceId == resourceId));
        }
        [RelayCommand]
        private void NoteViewCommand(int noteId)
        {
            // 创建新窗口并传入 NoteId
            var window = new NoteViewWindow(noteId);//NoteViewWindow还未定义，此处报错正常
            window.Show();
        }
        [RelayCommand]
        private void OnNoteDelete(int noteId)
        {
            _noteManager.RemoveById(noteId);
            NotesData.Remove(NotesData.FirstOrDefault(n => n.NoteId == noteId));
        }
        public async Task LoadFirstWatchedTimeAsync(int episodeId)
        {
            var firstWatchLog = await Task.Run(() =>
                _logManager.FindEarliestWatchLogForEpisode(episodeId));//后端增强定义

            if (firstWatchLog != null)
            {
                DateTimeOffset timestamp = firstWatchLog.Timestamp;

                WatchedDate = timestamp.ToString("yyyy-MM-dd");
                WatchedTime = timestamp.ToString("HH:mm");
            }
            else
            {
                WatchedDate = "";
                WatchedTime = "";
            }
        }
        public async void OnStateSelectedOptionChanged(string? oldValue, string newValue)
        {
            if (newValue == "已看" || newValue == "在看")
            {
                // 使用 await 等待异步操作完成
                await LoadFirstWatchedTimeAsync(_episode.Id);
            }
            else
            {
                // 如果是“未看”，清空显示
                WatchedDate = "";
                WatchedTime = "";
            }
        }
        public string SubTitle => SelectedTitleMode switch
        {
            "Original" => _entry.OriginalName,
            "Translated" => _entry.TranslatedName,
            _ => _entry.OriginalName
        };

        public string EpisodeNumberString =>
            $"第{_episode.EpisodeNumber}/{_entry.EpisodeCount}集";

        public string BroadcastState =>
            _entry.ReleaseDate <= DateTimeOffset.Now ? "已播出" : "未播出";

        public string Date => _entry.ReleaseDate.ToString("yyyy-MM-dd") ?? "";

        public string Weekday => _entry.ReleaseDate.DayOfWeek.ToString() ?? "";

        public string Time => _entry.ReleaseDate.ToString("HH:mm") ?? "";


        /*  // 原始数据源
          private readonly Entry _entry;
          private readonly Episode _episode;
          private readonly LogManager _logManager;

          // 用于控制显示名称的方式：Original / Translated
          [ObservableProperty]
          [NotifyPropertyChangedFor(nameof(SubTitle))]
          private string selectedTitleMode;

          [ObservableProperty]
          private string stateSelectedOption;
          public ObservableCollection<string> StateOptionList { get; } =
              new() { "未看", "在看", "已看" };
          [ObservableProperty]
          private string watchedDate = "";

          [ObservableProperty]
          private string watchedTime = "";

          [ObservableProperty]
          private string shortComment;

          public EntryWindow_EpisodeViewModel(Entry entry, Episode episode,LogManager logManager)
          {
              _entry = entry;
              _episode = episode;
              _logManager = logManager;
              //保留属性
              //ShortComment = _episode.ShortComment;


              // 初始化显示模式：从全局设置中读取
              var titleDisplayMode = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");
              if (int.TryParse(titleDisplayMode, out int mode))
              {
                  SelectedTitleMode = mode == 0 ? "Original" : "Translated";
              }
              else
              {
                  // 默认使用原名
                  SelectedTitleMode = "Original";
              }
          }

          // 计算属性：根据SelectedTitleMode返回原名或译名
          public string SubTitle => SelectedTitleMode switch
          {
              "Original" => _entry.OriginalName,
              "Translated" => _entry.TranslatedName,
              _ => _entry.OriginalName
          };

          // 当前是第几集 / 总集数，如“第2/13集”
          public string EpisodeNumberString
          {
              get
              {
                  return $"第{_episode.EpisodeNumber}/{_entry.EpisodeCount}集";
              }
          }

          // 播出状态：已播出 / 未播出
          public string BroadcastState
          {
              get
              {
                  //感觉这个属性有点问题，判断某一集是否已播出？
                  //return _entry.CollectionDate > DateTime.Now ? "未播出" : "已播出";
                  return _entry.ReleaseDate <= DateTime.Now ? "已播出" : "未播出";
              }
          }

          // 上映日期
          public string Date => _entry.ReleaseDate.ToString("yyyy-MM-dd");

          // 星期几
          public string Weekday => _entry.ReleaseDate.DayOfWeek.ToString();

          // 时间
          public string Time => _entry.ReleaseDate.ToString("HH:mm");

          */

    }
}

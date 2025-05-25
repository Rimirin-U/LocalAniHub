using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
//using System.Resources;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalAniHub.Views.Windows;
using LocalAniHubFront.Views.Windows;
using Wpf.Ui;
using static LocalAniHubFront.ViewModels.Pages.EntryWindow_MainInfoViewModel;
namespace LocalAniHubFront.ViewModels.Pages
{
    // record struct 定义
    public readonly record struct ResourceData(int ResourceId, string ResourceName);
    public readonly record struct NoteData(int NoteId, string NoteTitle);
    public partial class EntryWindow_EpisodeViewModel : ObservableObject
    {
        //服务实例
        private readonly ResourceManager _resourceManager = new ResourceManager();
        private readonly NoteManager _noteManager = new NoteManager();
        private readonly LogManager _logManager = new LogManager();//此处报错后端会修改
        private readonly EntryManager _entryManager = new EntryManager();
        private readonly EpisodeManager _episodeManager = new EpisodeManager();


        // 原始数据源
        private Entry? _entry;
        private Episode? _episode;


        // 资源和笔记的数据源
        public ObservableCollection<ResourceData> ResourcesData { get; } = new();
        public ObservableCollection<NoteData> NotesData { get; } = new();
        public ObservableCollection<string> StateOptionList { get; } = new() { "未看", "在看", "已看" };


        // 观看状态相关属性
        [ObservableProperty]
        private string stateSelectedOption;
        [ObservableProperty]
        private string watchedDate;
        [ObservableProperty]
        private string watchedTime;
        private string _shortComment = "";
        public string ShortComment
        {
            get => string.IsNullOrEmpty(_shortComment) ? "无短评" : _shortComment;
            set
            {
                if (_shortComment != value)
                {
                    _shortComment = value;
                    OnPropertyChanged();
                }
            }
        }
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SubTitle))]
        private string selectedTitleMode;// 标题显示模式：Original / Translated


        public EntryWindow_EpisodeViewModel(int episodeId)
        {
            LoadEpisodeAndEntry(episodeId);
            LoadInitialState();

            LoadResources();
            LoadNotes();
            _ = LoadFirstWatchedTimeAsync(_episode?.Id ?? -1);//异步加载观看时间
        }
        private void LoadEpisodeAndEntry(int episodeId)
        {
            _episode = _episodeManager.FindById(episodeId);

            if (_episode != null)
            {
                if (_episode.EntryId.HasValue)
                {
                    _entry = _entryManager.FindById(_episode.EntryId.Value);
                }
                else
                {
                    _entry = null;
                }
            }
            else
            {
                _entry = null;
            }
        }
        private void LoadInitialState()
        {
            // 加载标题显示模式
            var titleDisplayMode = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");
            if (int.TryParse(titleDisplayMode, out int mode))
            {
                SelectedTitleMode = mode == 0 ? "Translated" : "Original";
            }
            else
            {
                SelectedTitleMode = "Translated";
            }

            // 默认观看状态为“未看”
            if (_entry != null)
            {
                StateSelectedOption = _entry.State == State.Watched ? "已看" : "未看";
            }
            else
            {
                StateSelectedOption = "未看";
            }
            if (_episode != null)
            {
                ShortComment = _episode.ShortComment;
            }
        }
        private void LoadResources()
        {
            if (_episode == null) return;
            Func<Resource, bool> predicate = r => r.EpisodeId == _episode.Id;
            // 查询符合条件的所有资源，并按导入时间排序
            List<Resource> resourcesForEpisode = _resourceManager.Query(predicate);
            foreach(var resource in resourcesForEpisode )
            {
                ResourcesData.Add(new ResourceData(resource.Id, System.IO.Path.GetFileName(resource.ResourcePath)));
            }
        }
        private void LoadNotes()
        {
            if (_episode == null) return;
            // 构建谓词：筛选出包含当前 EpisodeId 的笔记
            Func<Note, bool> predicate = note => note.EpisodesId.Contains(_episode.Id);
            // 查询符合条件的所有笔记
            List<Note> notesForEpisode = _noteManager.Query(predicate);
            // 将找到的笔记添加到 NotesData 中
            foreach (var note in notesForEpisode)
            {
                NotesData.Add(new NoteData(note.Id, note.NoteTitle));
            }
        }


        [RelayCommand]
        private void ResourcePlay(int resourceId)
        {
            /*
            var resource = _resourceManager.FindById(resourceId);
            if (resource != null && !string.IsNullOrEmpty(resource.ResourcePath))
            {
                _videoPlayService.Play(resource.ResourcePath);
            }*/
            var player = new PlayerWindow(resourceId);
            player.Show();
        }
        [RelayCommand]
        private void ResourceDelete(int resourceId)
        {
            _resourceManager.DeleteResource(resourceId);
            //ResourcesData.Remove(ResourcesData.FirstOrDefault(r => r.ResourceId == resourceId));
        }
        [RelayCommand]
        private void NoteView(int noteId)
        {
            var window = new MarkdownWindow(noteId, MarkdownWindow_OpenOp.View);
            window.Show();
        }
        [RelayCommand]
        private void NoteDelete(int noteId)
        {
            _noteManager.RemoveById(noteId);
            NotesData.Remove(NotesData.FirstOrDefault(n => n.NoteId == noteId));
        }
        partial void OnStateSelectedOptionChanged(string? oldValue, string newValue)
        {
            if (newValue == "已看" || newValue == "在看")
            {
                var firstWatchLog = _logManager.FindEarliestWatchLogForEpisode(_episode?.Id ?? -1);

                if (firstWatchLog != null)
                {
                    WatchedDate = firstWatchLog.Timestamp.ToString("yyyy-MM-dd");
                    WatchedTime = firstWatchLog.Timestamp.ToString("HH:mm");
                }
                else
                {
                    WatchedDate = "";
                    WatchedTime = "";
                }
            }
            else
            {
                // 如果是“未看”，清空显示
                WatchedDate = "";
                WatchedTime = "";
            }
        }
        [RelayCommand]
        private void AutoSelectResourcePlay()
        {
            if (ResourcesData.Count == 0) MessageBox.Show("本地无可播放资源。");
            else
            {
                int resourceId = ResourcesData[0].ResourceId;
                ResourcePlay(resourceId);
            }
        }
        [RelayCommand]
        private void DownloadResource()
        {
            if (_episode != null)
            {
                var resourceSearchWindow = new ResourceSearchWindow(_episode.Id);
                resourceSearchWindow.Show();
            }
        }
        public void AddResource(string filePath)
        {
            if (_episode != null)
            {
                var resource = new Resource(_episode.Id, DateTime.Now, filePath);
                ResourceManager manager = new();
                manager.Addresource(resource);
            }
        }
        [RelayCommand]
        private void WriteNote()
        {
            if (_episode != null && _entry != null)
            {
                Note note = new();
                note.EpisodesId.Add(_episode.Id);
                string title = $"{_entry.OriginalName} 第{_episode.EpisodeNumber}集 {DateTime.Now:yyyyMMddHHmm}";
                note.NoteTitle = title;
                NoteManager noteManager = new();
                noteManager.Add(note);
                NoteService noteService = new();
                noteService.SaveNote(note, "");
                NoteView(noteManager.Query(n => n.NoteTitle == title)[0].Id);
            }
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


        public string SubTitle => SelectedTitleMode switch
        {
            "Original" => _entry?.OriginalName ?? "",
            "Translated" => _entry?.TranslatedName ?? "",
            _ => _entry?.OriginalName ?? ""
        };
        public string EpisodeNumberString =>
             _entry != null ? $"第{_episode.EpisodeNumber}/{_entry.EpisodeCount}集" : "第?/?集";
        public string BroadcastState
        {
            get
            {
                if (_episode == null || _entry == null)
                    return "";

                try
                {
                    DateTimeOffset? airDateTime = _entry.ReleaseDate.AddDays((_episode.EpisodeNumber - 1) * 7.0);
                    if (airDateTime.HasValue)
                    {
                        // 比较播出时间与当前时间
                        bool isAired = airDateTime.Value <= DateTimeOffset.Now;
                        return isAired ? "已播出" : "未播出";
                    }

                    return "未知";
                }
                catch
                {
                    return "未知";
                }
            }
        }
        public string Date => _entry?.ReleaseDate.AddDays((_episode.EpisodeNumber - 1) * 7.0).ToString("yyyy.M.d") ?? "";
        public string Weekday
        {
            get
            {
                if (_entry == null)
                    return "";

                var day = _entry.ReleaseDate.DayOfWeek;
                return day switch
                {
                    DayOfWeek.Monday => "星期一",
                    DayOfWeek.Tuesday => "星期二",
                    DayOfWeek.Wednesday => "星期三",
                    DayOfWeek.Thursday => "星期四",
                    DayOfWeek.Friday => "星期五",
                    DayOfWeek.Saturday => "星期六",
                    DayOfWeek.Sunday => "星期日",
                    _ => ""
                };
            }
        }
        public string Time => _entry?.ReleaseDate.ToString("HH:mm") ?? "";
    }
}

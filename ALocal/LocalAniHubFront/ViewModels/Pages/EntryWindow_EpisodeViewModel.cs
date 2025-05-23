﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using static LocalAniHubFront.ViewModels.Pages.EntryWindow_MainInfoViewModel;
namespace LocalAniHubFront.ViewModels.Pages
{
    // record struct 定义
    public readonly record struct ResourceData(int ResourceId, string ResourceName);
    public readonly record struct NoteData(int NoteId, string NoteTitle );
    public partial class EntryWindow_EpisodeViewModel : ObservableObject
    {
        //服务实例
        private readonly ResourceManager _resourceManager = new ResourceManager();
        private readonly NoteManager _noteManager = new NoteManager();
        private readonly VideoPlayService _videoPlayService = new VideoPlayService();
        private readonly LogManager _logManager = new LogManager();//此处报错后端会修改
        private readonly EntryManager _entryManager = new EntryManager();
        private readonly EpisodeManager _episodeManager = new EpisodeManager();
        private readonly EntryService _entryService = new EntryService(new EntryFetch(),new EntryManager() ,new EpisodeManager(),new EntryRatingManager(),new EntryMetaDataManager(), new EntryTimeInfoManager());
        // 原始数据源
        private  Entry? _entry;
        private  Episode? _episode;
       /* // 资源和笔记相关服务
        private readonly ResourceManager _resourceManager;
        private readonly NoteManager _noteManager;
        private readonly VideoPlayService _videoPlayService;
        private readonly LogManager _logManager;*/
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
        private string watchedDate;
       
        [ObservableProperty]
        private string watchedTime;

        //[ObservableProperty]
        //private string shortComment;
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
        // 标题显示模式：Original / Translated
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SubTitle))]
        private string selectedTitleMode;

        public EntryWindow_EpisodeViewModel(int episodeId)
        {
            LoadEpisodeAndEntry(episodeId);
            LoadInitialState();

            LoadResources();
            LoadNotes();
             _=LoadFirstWatchedTimeAsync(_episode?.Id??-1);//异步加载观看时间
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
                StateSelectedOption = _entry.State==State.Watched? "已看" : "未看";
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

            var resource = _resourceManager.FindById(_episode.Id);
            if (resource != null)
            {
                ResourcesData.Add(new ResourceData(resource.Id, System.IO.Path.GetFileName(resource.ResourcePath)));
            }
        }
        private void LoadNotes()
        {
            if (_episode == null) return;

            var note = _noteManager.FindById(_episode.Id);
            if (note != null)
            {
                NotesData.Add(new NoteData(note.Id, note.NoteTitle));
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
           // var window = new NoteViewWindow(noteId);//NoteViewWindow还未定义，此处报错正常
           // window.Show();
        }
        [RelayCommand]
        private void OnNoteDelete(int noteId)
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


        /*public string BroadcastState =>
            _entry?.ReleaseDate <= DateTimeOffset.Now ? "已播出" : "未播出";*/
        /* public string BroadcastState
         {
             get
             {
                 if (_episode == null || _entry == null)
                     return "";

                 try
                 {
                    // bool isAired = _entryService.IsEpisodeAired(_entry.Id, _episode.EpisodeNumber);
                     return  ? "已播出" : "未播出";
                 }
                 catch
                 {
                     return "未知";
                 }
             }
         }*/
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

        public string Date =>_entry?.ReleaseDate.AddDays((_episode.EpisodeNumber - 1) * 7.0).ToString("yyyy.M.d")?? "";

        //public string Weekday => _entry?.ReleaseDate.DayOfWeek.ToString() ?? "";
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

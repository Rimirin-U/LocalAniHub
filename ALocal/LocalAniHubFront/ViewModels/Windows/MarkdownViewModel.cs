﻿using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalAniHubFront.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
//using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace LocalAniHubFront.ViewModels.Windows
{
    public record struct EntryItem(int EntryId, string EntryName);
    public record struct EpisodeItem(int EntryId, string EntryName, int EpisodeId, int EpisodeNumber);
    public partial class EntryRowViewModel : ObservableObject
    {
        private readonly Func<EntryItem, object, bool> _isItemSelected;
        [RelayCommand]
        private void RemoveSelf()
        {
            RemoveRequested?.Invoke(this);
        }

        public event Action<object> RemoveRequested;

        private EntryItem? _selectedEntryItem;
        public EntryItem? SelectedEntryItem
        {
            get => _selectedEntryItem;
            set
            {
                if (value.HasValue && _isItemSelected(value.Value, this))
                {
                    MessageBox.Show($"作品“{value.Value.EntryName}”已被其他下拉框选中，请选择不同作品。", "重复选择", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetProperty(ref _selectedEntryItem, null);
                }
                else
                {
                    SetProperty(ref _selectedEntryItem, value);
                }
            }
        }
        public EntryRowViewModel(Func<EntryItem, object, bool> isItemSelected)
        {
            _isItemSelected = isItemSelected;
        }
    }
    public partial class EpisodeRowViewModel : ObservableObject
    {
        private readonly Func<EpisodeItem, object, bool> _isItemSelected;

        public EpisodeRowViewModel(
            IReadOnlyList<EntryItem> allEntries,
            IReadOnlyList<EpisodeItem> allEpisodes,
            Func<EpisodeItem, object, bool> isItemSelected)
        {
            AllEntries = allEntries;
            AllEpisodes = allEpisodes;
            _isItemSelected = isItemSelected;
        }

        [RelayCommand]
        private void RemoveSelf()
        {
            RemoveRequested?.Invoke(this);
        }

        public event Action<object> RemoveRequested;

        private EntryItem? _selectedEntryItem;
        public EntryItem? SelectedEntryItem
        {
            get => _selectedEntryItem;
            set
            {
                if (SetProperty(ref _selectedEntryItem, value) && value.HasValue)
                {
                    var entryId = value.Value.EntryId;
                    EpisodeItems = AllEpisodes.Where(e => e.EntryId == entryId).ToList();
                }
            }
        }

        private IReadOnlyList<EpisodeItem> _episodeItems = new List<EpisodeItem>();
        public IReadOnlyList<EpisodeItem> EpisodeItems
        {
            get => _episodeItems;
            private set => SetProperty(ref _episodeItems, value);
        }

        private EpisodeItem? _selectedEpisodeItem;
        public EpisodeItem? SelectedEpisodeItem
        {
            get => _selectedEpisodeItem;
            set
            {
                if (value.HasValue && _isItemSelected(value.Value, this))
                {
                    MessageBox.Show($"第 {value.Value.EpisodeNumber} 集已被其他下拉框选中，请选择不同集数。", "重复选择", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetProperty(ref _selectedEpisodeItem, null);
                }
                else
                {
                    SetProperty(ref _selectedEpisodeItem, value);
                }
            }
        }

        public IReadOnlyList<EntryItem> AllEntries { get; }
        public IReadOnlyList<EpisodeItem> AllEpisodes { get; }
    }
    public partial class MarkdownViewModel : ObservableObject
    {
        private readonly int _noteId;
        private readonly NoteManager _noteManager;
        [ObservableProperty] private string _noteTitle = "新笔记";
        [ObservableProperty] private string _markdownText = "# 默认内容\n请开始编辑...";
        [ObservableProperty] private int _selectedTabIndex;
        // 全局数据源
        public IReadOnlyList<EntryItem> EntryItemList { get; private set; } = new List<EntryItem>();
        public IReadOnlyList<EpisodeItem> EpisodeItemList { get; private set; } = new List<EpisodeItem>();
        // UI 行集合
        public ObservableCollection<EntryRowViewModel> EntryComboBoxList { get; } = new();
        public ObservableCollection<EpisodeRowViewModel> EpisodeComboBoxList { get; } = new();
        public MarkdownViewModel(int noteId, MarkdownWindow_OpenOp openOp)
        {
            _noteId = noteId;
            _noteManager = new NoteManager();
            var note = _noteManager.FindById(noteId);

            SelectedTabIndex = openOp == MarkdownWindow_OpenOp.View ? 0 : 1;

            if (note != null)
            {
                NoteTitle = note.NoteTitle;
               // MarkdownText = note.Content;
                var noteService = new NoteService();
                MarkdownText = noteService.LoadNoteContent(note);
            }
            

            LoadAllEntries();
            LoadAllEpisodes();

            InitializeEntryComboBoxes(note);
            InitializeEpisodeComboBoxes(note);
            InitializeAutoSave();
        }

        private async void InitializeAutoSave()
        {
            await System.Threading.Tasks.Task.Run(async () =>
            {
                while (true)
                {
                    SaveNote();
                    await System.Threading.Tasks.Task.Delay(5000);
                }
            });
        }
        private void SaveNote()
        {
            try
            {
                var note = _noteManager.FindById(_noteId);
                if (note != null)
                {
                    note.NoteTitle = NoteTitle;//这个报错不用管
                    //note.Content = MarkdownText;
                    var noteService = new NoteService();
                    noteService.SaveNote(note, MarkdownText);
                    note.EntriesId = EntryComboBoxList
                        .Where(vm => vm.SelectedEntryItem.HasValue)
                        .Select(vm => vm.SelectedEntryItem!.Value.EntryId).ToList();

                    note.EpisodesId = EpisodeComboBoxList
                        .Where(vm => vm.SelectedEpisodeItem.HasValue)
                        .Select(vm => vm.SelectedEpisodeItem!.Value.EpisodeId).ToList();

                    //_noteManager.Update(note);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}");
            }
        }
        private void LoadAllEntries()
        {
            var entryManager = new EntryManager();
            var entries = entryManager.Query(e => true); // 获取所有 Entry
            EntryItemList = entries.Select(e => new EntryItem(e.Id, e.TranslatedName)).ToList();
        }
        private void LoadAllEpisodes()
        {
            var episodeManager = new EpisodeManager();
            var episodes = episodeManager.Query(e => true); // 获取所有 Episode
            EpisodeItemList = episodes
                .Where(e => e.Entry != null)
                .Select(e => new EpisodeItem(
                    e.Entry.Id,
                    e.Entry.TranslatedName,
                    e.Id,
                    e.EpisodeNumber))
                .ToList();
        }
        private void InitializeEntryComboBoxes(Note? note)
        {
            if (note?.EntriesId != null)
            {
                foreach (var id in note.EntriesId)
                {
                    EntryItem? item = EntryItemList.FirstOrDefault(e => e.EntryId == id);
                    if (item.HasValue)
                    {
                        var row = CreateNewEntryRow();
                        row.SelectedEntryItem = item;
                        EntryComboBoxList.Add(row);
                    }
                }
            }

            if (EntryComboBoxList.Count == 0)
            {
                EntryComboBoxList.Add(CreateNewEntryRow());
            }
        }
        private void InitializeEpisodeComboBoxes(Note? note)
        {
            if (note?.EpisodesId != null)
            {
                foreach (var id in note.EpisodesId)//这个报错不用管
                {
                    EpisodeItem? item = EpisodeItemList.FirstOrDefault(e => e.EpisodeId == id);
                    if (item.HasValue)
                    {
                        var row = CreateNewEpisodeRow();
                        row.SelectedEntryItem = EntryItemList.FirstOrDefault(e => e.EntryId == item.Value.EntryId);
                        row.SelectedEpisodeItem = item;
                        EpisodeComboBoxList.Add(row);
                    }
                }
            }

            if (EpisodeComboBoxList.Count == 0)
            {
                EpisodeComboBoxList.Add(CreateNewEpisodeRow());
            }
        }
        private EpisodeRowViewModel CreateNewEpisodeRow()
        {
            return new EpisodeRowViewModel(EntryItemList, EpisodeItemList, IsEpisodeItemSelected);
        }
        private EntryRowViewModel CreateNewEntryRow()
        {
            return new EntryRowViewModel(IsEntryItemSelected);
        }
        [RelayCommand]
        private void AddEntryComboBox()
        {
            EntryComboBoxList.Add(CreateNewEntryRow());
        }

        [RelayCommand]
        private void AddEpisodeComboBox()
        {
            EpisodeComboBoxList.Add(CreateNewEpisodeRow());
        }

        private bool IsEpisodeItemSelected(EpisodeItem item, object requester)
        {
            return EpisodeComboBoxList
                .Where(vm => vm != requester)
                .Any(vm => vm.SelectedEpisodeItem.HasValue &&
                           vm.SelectedEpisodeItem.Value.EpisodeId == item.EpisodeId);
        }
        private bool IsEntryItemSelected(EntryItem item, object requester)
        {
            return EntryComboBoxList
                .Where(vm => vm != requester)
                .Any(vm => vm.SelectedEntryItem.HasValue &&
                           vm.SelectedEntryItem.Value.EntryId == item.EntryId);
        }
    }
}
        /*
        // Entry 对象（用于显示）
        public record EntryItem(int Id, string EntryName);

        // 单个关联作品行 ViewModel
        public partial class EntryRowViewModel : ObservableObject
        {
            [ObservableProperty]
            private EntryItem? _selectedEntryItem;

            public ICommand RemoveSelfCommand { get; }

            public EntryRowViewModel(ICommand removeSelfCommand)
            {
                RemoveSelfCommand = removeSelfCommand;
            }
        }
        // 单集对象（用于显示）
        public record EpisodeItem(int Id, int EntryId, string EpisodeNumber);

        // 单个关联单集行 ViewModel
        public partial class EpisodeRowViewModel : ObservableObject
        {
            [ObservableProperty]
            private EntryItem? _selectedEntryItem;

            [ObservableProperty]
            private EpisodeItem? _selectedEpisodeItem;

            public ICommand RemoveSelfCommand { get; }

            public EpisodeRowViewModel(ICommand removeSelfCommand)
            {
                RemoveSelfCommand = removeSelfCommand;
            }
        }

        public partial class MarkdownViewModel : ObservableObject
        {
            private readonly int _noteId;
            private readonly string _filePath;
            private readonly NoteService _noteService;
            private readonly List<EntryItem> _entryItems;
            private readonly List<EpisodeItem> _episodeItems;

            private DispatcherTimer _autoSaveTimer;
            private bool _isDisposed = false;

            // 绑定属性
            [ObservableProperty]
            private string _noteTitle = "笔记标题";

            [ObservableProperty]
            private int _selectedTabIndex = 1; // 默认编辑页

            [ObservableProperty]
            private string _markdownText = string.Empty;
            // 动态 ComboBox 行列表
            public ObservableCollection<EntryRowViewModel> EntryComboBoxList { get; } = new();
            public ObservableCollection<EpisodeRowViewModel> EpisodeComboBoxList { get; } = new();
            // 所有可用选项（静态资源）
            public List<EntryItem> EntryItemList => _entryItems;
            public List<EpisodeItem> EpisodeItemList => _episodeItems;
            public MarkdownViewModel(int noteId, MarkdownWindow_OpenOp openOp)
            {
                _noteId = noteId;

                // 初始化 Tab 选择
                SelectedTabIndex = openOp == MarkdownWindow_OpenOp.View ? 0 : 1;

                // 模拟获取所有 Entry 和 Episode 列表
                _entryItems = LoadAllEntries();
                _episodeItems = LoadAllEpisodes();

                var notesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Notes");
                if (!Directory.Exists(notesDirectory))
                    Directory.CreateDirectory(notesDirectory);

                _filePath = Path.Combine(notesDirectory, $"note_{noteId}.md");

                _noteService = new NoteService(new NoteManager()); // 实际应由 DI 提供

                LoadNote();
                InitializeAutoSave();

                InitializeEntryComboBoxes();
                InitializeEpisodeComboBoxes();
            }
            private void LoadNote()
            {
                if (File.Exists(_filePath))
                {
                    MarkdownText = File.ReadAllText(_filePath);
                }
                else
                {
                    MarkdownText = string.Empty;
                }

                NoteTitle = $"笔记 {_noteId}";
            }
            private void InitializeAutoSave()
            {
                _autoSaveTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(30)
                };
                _autoSaveTimer.Tick += async (s, e) => await SaveFileAsync();
                _autoSaveTimer.Start();
            }
            [RelayCommand]
            private Task  SaveFileAsync()
            {
                try
                {
                    File.WriteAllText(_filePath, MarkdownText ?? "");
                    SaveRelatedEntriesAndEpisodes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存失败：{ex.Message}");
                }

                return Task.CompletedTask;
            }
            private void SaveRelatedEntriesAndEpisodes()
            {
                var note = _noteService.LoadNoteFromMarkdown(NoteTitle);//这个报错不用管
                note.EntriesId = EntryComboBoxList
                    .Where(r => r.SelectedEntryItem != null)
                    .Select(r => r.SelectedEntryItem!.Id)
                    .ToList();

                note.EpisodesId = EpisodeComboBoxList
                    .Where(r => r.SelectedEpisodeItem != null)
                    .Select(r => r.SelectedEpisodeItem!.Id)
                    .ToList();

                _noteService.SaveNoteAsMarkdown(note);//这个报错不用管
            }
            private void InitializeEntryComboBoxes()
            {
                var note = _noteService.LoadNoteFromMarkdown(NoteTitle);//这个报错不用管
                foreach (var entryId in note.EntriesId)
                {
                    var item = _entryItems.FirstOrDefault(e => e.Id == entryId);
                    AddNewEntryRow(item);
                }

            }
            [RelayCommand]
            private void AddEntryComboBox()
            {
                AddNewEntryRow(null);
            }
            private void AddNewEntryRow(EntryItem? preSelectedItem)
            {
                var row = new EntryRowViewModel(RemoveEntryRow);
                row.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(row.SelectedEntryItem))
                        PreventDuplicateSelection(row);
                };

                if (preSelectedItem != null)
                    row.SelectedEntryItem = preSelectedItem;

                EntryComboBoxList.Add(row);
            }
            private void RemoveEntryRow(object? param)
            {
                if (param is EntryRowViewModel row)
                {
                    EntryComboBoxList.Remove(row);
                }
            }
            private void PreventDuplicateSelection(EntryRowViewModel currentRow)
            {
                var selected = currentRow.SelectedEntryItem;
                if (selected == null) return;

                foreach (var row in EntryComboBoxList)
                {
                    if (row != currentRow && row.SelectedEntryItem?.Id == selected.Id)
                    {
                        MessageBox.Show("该作品已被其他下拉框选中，请选择不同作品。");
                        currentRow.SelectedEntryItem = null;
                        return;
                    }
                }
            }
            private void InitializeEpisodeComboBoxes()
            {
                var note = _noteService.LoadNoteFromMarkdown(NoteTitle);//
                foreach (var episodeId in note.EpisodesId)
                {
                    var item = _episodeItems.FirstOrDefault(e => e.Id == episodeId);
                    if (item != null)
                    {
                        var entryItem = _entryItems.FirstOrDefault(e => e.Id == item.EntryId);
                        AddNewEpisodeRow(entryItem, item);
                    }
                }
            }
            [RelayCommand]
            private void AddEpisodeComboBox()
            {
                AddNewEpisodeRow(null, null);
            }
            private void AddNewEpisodeRow(EntryItem? entryItem, EpisodeItem? episodeItem)
            {
                var row = new EpisodeRowViewModel(RemoveEpisodeRow);
                row.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(row.SelectedEpisodeItem))
                        PreventDuplicateEpisodeSelection(row);
                };

                if (entryItem != null)
                    row.SelectedEntryItem = entryItem;
                if (episodeItem != null)
                    row.SelectedEpisodeItem = episodeItem;

                EpisodeComboBoxList.Add(row);
            }
            private void RemoveEpisodeRow(object? param)
            {
                if (param is EpisodeRowViewModel row)
                {
                    EpisodeComboBoxList.Remove(row);
                }
            }
            private void PreventDuplicateEpisodeSelection(EpisodeRowViewModel currentRow)
            {
                var selected = currentRow.SelectedEpisodeItem;
                if (selected == null) return;

                foreach (var row in EpisodeComboBoxList)
                {
                    if (row != currentRow && row.SelectedEpisodeItem?.Id == selected.Id)
                    {
                        MessageBox.Show("该集数已被其他下拉框选中，请选择不同集数。");
                        currentRow.SelectedEpisodeItem = null;
                        return;
                    }
                }
            }
            #region 数据加载模拟方法（实际应从数据库或服务获取）

            private List<EntryItem> LoadAllEntries()
            {
                return new List<EntryItem>
            {
                new(1, "作品A"),
                new(2, "作品B"),
                new(3, "作品C")
            };
            }

            private List<EpisodeItem> LoadAllEpisodes()
            {
                return new List<EpisodeItem>
            {
                new(1, 1, "第1集"),
                new(2, 1, "第2集"),
                new(3, 2, "第1集"),
                new(4, 2, "第2集"),
                new(5, 3, "第1集")
            };
            }

            #endregion

            public async void Dispose()
            {
                if (!_isDisposed)
                {
                    _autoSaveTimer.Stop();
                    _autoSaveTimer.Tick -= SaveFileAsync;
                    await SaveFileAsync(); // 最终保存一次
                    _isDisposed = true;
                }

                GC.SuppressFinalize(this);
            }
        }*/


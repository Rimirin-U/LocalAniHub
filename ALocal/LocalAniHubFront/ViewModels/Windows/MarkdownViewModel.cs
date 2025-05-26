using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalAniHubFront.Models;
using LocalAniHubFront.Services;
using LocalAniHubFront.Views.Windows;

namespace LocalAniHubFront.ViewModels.Windows
{
    public record struct EntryItem(int EntryId, string EntryName);
    public record struct EpisodeItem(int EntryId, string EntryName, int EpisodeId, int EpisodeNumber);

    public partial class EntryRowViewModel : ObservableObject
    {
        private readonly Func<EntryItem, object, bool> _isItemSelected;
        public event Action<object> RemoveRequested;

        [RelayCommand]
        private void RemoveSelf()
            => RemoveRequested?.Invoke(this);

        public override string? ToString()
        {
            return SelectedEntryItem is EntryItem entryItem
                ? entryItem.EntryName
                : string.Empty;
        }

        private EntryItem? _selectedEntryItem;
        public EntryItem? SelectedEntryItem
        {
            get => _selectedEntryItem;
            set
            {
                if (value.HasValue && _isItemSelected(value.Value, this))
                {
                    MessageBox.Show($"作品“{value.Value.EntryName}”已被其他下拉框选中，请选择不同作品。",
                        "重复选择", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetProperty(ref _selectedEntryItem, null);
                }
                else
                {
                    SetProperty(ref _selectedEntryItem, value);
                }
            }
        }

        public IReadOnlyList<EntryItem> EntryItemList { get; }

        public EntryRowViewModel(
            IReadOnlyList<EntryItem> entryItemList,
            Func<EntryItem, object, bool> isItemSelected)
        {
            EntryItemList = entryItemList;
            _isItemSelected = isItemSelected;
        }
    }

    public partial class EpisodeRowViewModel : ObservableObject
    {
        private readonly Func<EpisodeItem, object, bool> _isItemSelected;
        private readonly IReadOnlyList<EpisodeItem> _allEpisodes;
        public event Action<object> RemoveRequested;

        [RelayCommand]
        private void RemoveSelf()
            => RemoveRequested?.Invoke(this);

        private EntryItem? _selectedEntryItem;
        public EntryItem? SelectedEntryItem
        {
            get => _selectedEntryItem;
            set
            {
                if (SetProperty(ref _selectedEntryItem, value) && value.HasValue)
                {
                    var entryId = value.Value.EntryId;
                    EpisodeItemList = new ObservableCollection<EpisodeItem>(
                        _allEpisodes.Where(e => e.EntryId == entryId));
                }
                else if (!value.HasValue)
                {
                    EpisodeItemList.Clear();
                }
            }
        }

        private ObservableCollection<EpisodeItem> _episodeItemList = new();
        public ObservableCollection<EpisodeItem> EpisodeItemList
        {
            get => _episodeItemList;
            private set => SetProperty(ref _episodeItemList, value);
        }

        private EpisodeItem? _selectedEpisodeItem;
        public EpisodeItem? SelectedEpisodeItem
        {
            get => _selectedEpisodeItem;
            set
            {
                if (value.HasValue && _isItemSelected(value.Value, this))
                {
                    MessageBox.Show($"第 {value.Value.EpisodeNumber} 集已被其他下拉框选中，请选择不同集数。",
                        "重复选择", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetProperty(ref _selectedEpisodeItem, null);
                }
                else
                {
                    SetProperty(ref _selectedEpisodeItem, value);
                }
            }
        }

        public IReadOnlyList<EntryItem> EntryItemList { get; }

        public EpisodeRowViewModel(
            IReadOnlyList<EntryItem> allEntries,
            IReadOnlyList<EpisodeItem> allEpisodes,
            Func<EpisodeItem, object, bool> isItemSelected)
        {
            EntryItemList = allEntries;
            _allEpisodes = allEpisodes;
            _isItemSelected = isItemSelected;
        }
    }

    public partial class MarkdownViewModel : ObservableObject, IDisposable
    {
        private readonly int _noteId;
        private readonly NoteManager _noteManager;
        private CancellationTokenSource _autoSaveCts;
        private readonly SemaphoreSlim _saveLock = new(1, 1);

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

            SelectedTabIndex = openOp == MarkdownWindow_OpenOp.View ? 0 : 1;
            var note = _noteManager.FindById(noteId);
            if (note != null)
            {
                NoteTitle = note.NoteTitle;
                var noteService = new NoteService();
                MarkdownText = noteService.LoadNoteContent(note);
            }

            LoadAllEntries();
            LoadAllEpisodes();
            InitializeEntryComboBoxes(note);
            InitializeEpisodeComboBoxes(note);

            StartAutoSave();
        }

        private void LoadAllEntries()
        {
            var entryManager = new EntryManager();
            var entries = entryManager.Query(e => true);
            EntryItemList = entries.Select(e => new EntryItem(e.Id, e.TranslatedName)).ToList();
        }

        private void LoadAllEpisodes()
        {
            var entryManager = new EntryManager();
            var episodeManager = new EpisodeManager();
            var episodes = episodeManager.Query(e => true);
            EpisodeItemList = episodes
                .Where(e => e.EntryId != null)
                .Select(e =>
                {
                    var entry = entryManager.FindById(e.EntryId.Value);
                    return new EpisodeItem(
                        e.EntryId.Value,
                        entry?.TranslatedName ?? "未知作品",
                        e.Id,
                        e.EpisodeNumber);
                })
                .ToList();
        }

        private void InitializeEntryComboBoxes(Note? note)
        {
            if (note?.EntriesId != null)
            {
                foreach (var id in note.EntriesId)
                {
                    var item = EntryItemList.FirstOrDefault(e => e.EntryId == id);
                    var row = CreateNewEntryRow();
                    row.SelectedEntryItem = item;
                    EntryComboBoxList.Add(row);
                }
            }
            if (EntryComboBoxList.Count == 0)
                EntryComboBoxList.Add(CreateNewEntryRow());
        }

        private void InitializeEpisodeComboBoxes(Note? note)
        {
            if (note?.EpisodesId != null)
            {
                foreach (var id in note.EpisodesId)
                {
                    var item = EpisodeItemList.FirstOrDefault(e => e.EpisodeId == id);
                    var row = CreateNewEpisodeRow();
                    var entryItem = new EntryItem(item.EntryId, item.EntryName);
                    row.SelectedEntryItem = entryItem;
                    row.SelectedEpisodeItem = item;
                    EpisodeComboBoxList.Add(row);
                }
            }
            if (EpisodeComboBoxList.Count == 0)
                EpisodeComboBoxList.Add(CreateNewEpisodeRow());
        }

        private EntryRowViewModel CreateNewEntryRow()
        {
            var row = new EntryRowViewModel(EntryItemList, IsEntryItemSelected);
            row.RemoveRequested += sender => { if (sender is EntryRowViewModel r) EntryComboBoxList.Remove(r); };
            return row;
        }

        private EpisodeRowViewModel CreateNewEpisodeRow()
        {
            var row = new EpisodeRowViewModel(EntryItemList, EpisodeItemList, IsEpisodeItemSelected);
            row.RemoveRequested += sender => { if (sender is EpisodeRowViewModel r) EpisodeComboBoxList.Remove(r); };
            return row;
        }

        [RelayCommand]
        private void AddEntryComboBox()
            => EntryComboBoxList.Add(CreateNewEntryRow());

        [RelayCommand]
        private void AddEpisodeComboBox()
            => EpisodeComboBoxList.Add(CreateNewEpisodeRow());

        private bool IsEntryItemSelected(EntryItem item, object requester)
            => EntryComboBoxList
                .Where(vm => vm != requester)
                .Any(vm => vm.SelectedEntryItem.HasValue && vm.SelectedEntryItem.Value.EntryId == item.EntryId);

        private bool IsEpisodeItemSelected(EpisodeItem item, object requester)
            => EpisodeComboBoxList
                .Where(vm => vm != requester)
                .Any(vm => vm.SelectedEpisodeItem.HasValue && vm.SelectedEpisodeItem.Value.EpisodeId == item.EpisodeId);

        private void StartAutoSave()
        {
            _autoSaveCts = new CancellationTokenSource();
            _ = AutoSaveLoopAsync(_autoSaveCts.Token);
        }

        private void StopAutoSave()
            => _autoSaveCts?.Cancel();

        private async Task AutoSaveLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Application.Current.Dispatcher.InvokeAsync(SaveNoteAsync);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), ct);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task SaveNoteAsync()
        {
            // 快照 UI 数据
            var entryIds = new List<int>();
            var episodeIds = new List<int>();
            var title = string.Empty;
            var content = string.Empty;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                entryIds = EntryComboBoxList
                    .Where(vm => vm.SelectedEntryItem.HasValue)
                    .Select(vm => vm.SelectedEntryItem!.Value.EntryId)
                    .ToList();
                episodeIds = EpisodeComboBoxList
                    .Where(vm => vm.SelectedEpisodeItem.HasValue)
                    .Select(vm => vm.SelectedEpisodeItem!.Value.EpisodeId)
                    .ToList();
                title = NoteTitle;
                content = MarkdownText;
            });

            await _saveLock.WaitAsync();
            try
            {
                var note = _noteManager.FindById(_noteId);
                if (note != null)
                {
                    note.NoteTitle = title;
                    note.EntriesId = entryIds;
                    note.EpisodesId = episodeIds;
                    var noteService = new NoteService();
                    noteService.SaveNote(note, content);
                    _noteManager.Modify(note);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error));
            }
            finally
            {
                _saveLock.Release();
            }
        }


        public void Dispose()
        {
            StopAutoSave();
            // 最后一次保存
            Application.Current.Dispatcher.InvokeAsync(() => SaveNoteAsync());
            GC.SuppressFinalize(this);
        }
    }
}

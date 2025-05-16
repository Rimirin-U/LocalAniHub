using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalAniHubFront.Views.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace LocalAniHubFront.ViewModels.Windows
{
    public record struct EntryItem(int EntryId, string EntryName);
    public record struct EpisodeItem(int EntryId, string EntryName, int EpisodeId, int EpisodeNumber);
    public partial class EntryRowViewModel : ObservableObject
    {
        private readonly Func<EntryItem, object, bool> _isItemSelected;

        public EntryRowViewModel(Func<EntryItem, object, bool> isItemSelected)
        {
            _isItemSelected = isItemSelected;
        }

        [RelayCommand]
        private void RemoveSelf()
        {
            // 通知 MarkdownViewModel 删除当前项
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
                    MessageBox.Show($"“{value.Value.EntryName}” 已被其他下拉框选中，请选择不同作品。", "重复选择", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetProperty(ref _selectedEntryItem, null);
                }
                else
                {
                    SetProperty(ref _selectedEntryItem, value);
                }
            }
        }
    }
    public partial class EpisodeRowViewModel : ObservableObject
    {
        private readonly Func<EpisodeItem, object, bool> _isItemSelected;

        public EpisodeRowViewModel(Func<EpisodeItem, object, bool> isItemSelected)
        {
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
            set => SetProperty(ref _selectedEntryItem, value);
        }

        private EpisodeItem? _selectedEpisodeItem;
        public EpisodeItem? SelectedEpisodeItem
        {
            get => _selectedEpisodeItem;
            set
            {
                if (value.HasValue && _isItemSelected(value.Value, this))
                {
                    MessageBox.Show($"“{value.Value.EpisodeNumber}” 已被其他下拉框选中，请选择不同集数。", "重复选择", MessageBoxButton.OK, MessageBoxImage.Warning);
                    SetProperty(ref _selectedEpisodeItem, null);
                }
                else
                {
                    SetProperty(ref _selectedEpisodeItem, value);
                }
            }
        }
    }
    public partial class MarkdownViewModel : ObservableObject
    {
        private readonly int _noteId;
        private readonly string _filePath;
        private readonly NoteService _noteService;
        private readonly NoteManager _noteManager;
        private readonly List<EntryItem> _entryItems;
        private readonly List<EpisodeItem> _episodeItems;
        [ObservableProperty]
        private string _noteTitle = "新笔记";

        [ObservableProperty]
        private string _markdownText = "# 默认内容\n请开始编辑...";

        [ObservableProperty]
        private int _selectedTabIndex;

        public ObservableCollection<EntryRowViewModel> EntryComboBoxList { get; } = new();
        public ObservableCollection<EpisodeRowViewModel> EpisodeComboBoxList { get; } = new();
        public MarkdownViewModel(int noteId, MarkdownWindow_OpenOp openOp)
        {
            _noteId = noteId;

            SelectedTabIndex = openOp == MarkdownWindow_OpenOp.View ? 0 : 1;
            _noteManager = new NoteManager();
            _noteService = new NoteService(_noteManager);
            var note = _noteManager.FindById(noteId);
            if ( note != null)
            {
                NoteTitle = note.NoteTitle;
                MarkdownText = note.Content;
            }
            else
            {
                MarkdownText = string.Empty;
            }
            _entryItems = LoadAllEntries(_noteId);
            _episodeItems = LoadAllEpisodes(_noteId);

            /* var notesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Notes");
             if (!Directory.Exists(notesDirectory))
                 Directory.CreateDirectory(notesDirectory);*/
    
            _filePath = Path.Combine(_noteService.BaseDirectory, $"{note.NoteTitle}.md");//感觉这里存在问题
            //LoadNote();
            InitializeAutoSave();
            InitializeEntryComboBoxes();
            InitializeEpisodeComboBoxes();
        }
        /* private void LoadNote()
         {
             if (File.Exists(_filePath))
             {
                 MarkdownText = File.ReadAllText(_filePath);
                 NoteTitle = $"笔记 {_noteId}";
             }
         }*/
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
                File.WriteAllText(_filePath, MarkdownText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}");
            }
        }

        private List<EntryItem> LoadAllEntries(int noteId)
        {
            var noteManager = new NoteManager();
            var entryManager = new EntryManager();

            // Step 1: 查找当前 Note 对象
            var note = noteManager.FindById(noteId);
            if (note == null || note.EntriesId == null || !note.EntriesId.Any())
            {
                return new List<EntryItem>(); // 如果没有关联任何 Entry，返回空列表
            }

            // Step 2: 获取该 Note 关联的所有 Entry ID
            var entryIds = note.EntriesId;

            // Step 3: 查询这些 Entry ID 对应的 Entry 数据
            var entries = entryManager.Query(e => entryIds.Contains(e.Id));

            // Step 4: 转换为 EntryItem 列表
            return entries.Select(e => new EntryItem(e.Id, e.TranslatedName))
                          .ToList();
        }

        private List<EpisodeItem> LoadAllEpisodes(int noteId)
        {
            var noteManager = new NoteManager();
            var episodeManager = new EpisodeManager();

            // Step 1: 获取对应的 Note
            var note = noteManager.FindById(noteId);
            if (note == null || note.EpisodesId == null || !note.EpisodesId.Any())
            {
                return new List<EpisodeItem>(); // 如果没有关联任何 Episode，返回空列表
            }

            var episodeIds = note.EpisodesId;

            // Step 2: 查询这些 Episode ID 对应的剧集数据
            var episodes = episodeManager.Query(e => episodeIds.Contains(e.Id))
                                         .Where(e => e.Entry != null); // 确保有对应的作品信息

            // Step 3: 转换为 EpisodeItem 列表
            return episodes.Select(e => new EpisodeItem(
                             e.Entry!.Id,
                             e.Entry.TranslatedName,
                             e.Id,
                             e.EpisodeNumber))
                           .ToList();
        }

        private void InitializeEntryComboBoxes()
        {
            EntryComboBoxList.Add(CreateNewEntryRow());
        }

        private void InitializeEpisodeComboBoxes()
        {
            EpisodeComboBoxList.Add(CreateNewEpisodeRow());
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

        private EntryRowViewModel CreateNewEntryRow()
        {
            var row = new EntryRowViewModel(IsEntryItemSelected);
            row.RemoveRequested += item => EntryComboBoxList.Remove((EntryRowViewModel)item);
            return row;
        }

        private EpisodeRowViewModel CreateNewEpisodeRow()
        {
            var row = new EpisodeRowViewModel(IsEpisodeItemSelected);
            row.RemoveRequested += item => EpisodeComboBoxList.Remove((EpisodeRowViewModel)item);
            return row;
        }

        private bool IsEntryItemSelected(EntryItem item, object requester)
        {
            return EntryComboBoxList
                .Where(vm => vm != requester)
                .Any(vm => vm.SelectedEntryItem.HasValue &&
                           vm.SelectedEntryItem.Value.EntryId == item.EntryId);
        }

        private bool IsEpisodeItemSelected(EpisodeItem item, object requester)
        {
            return EpisodeComboBoxList
                .Where(vm => vm != requester)
                .Any(vm => vm.SelectedEpisodeItem.HasValue &&
                           vm.SelectedEpisodeItem.Value.EpisodeId == item.EpisodeId);
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
 

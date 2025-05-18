using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class EntryWindow_NoteManageViewModel : ObservableObject
    {
        private readonly NoteManager _noteManager = new NoteManager();
        private readonly EntryManager _entryManager = new EntryManager();
        private readonly ISnackbarService _snackbarService;
        private readonly INavigationService _navigationService;
        private Entry? _entry;

        [ObservableProperty]
        private string _subtitle = "笔记管理";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EntryTitle))]
        private string _selectedTitleMode = "Original";

        public ObservableCollection<NoteData> NoteData { get; } = new();

        public EntryWindow_NoteManageViewModel(
            ISnackbarService snackbarService,
            INavigationService navigationService)
        {
            _snackbarService = snackbarService;
            _navigationService = navigationService;
        }

        public void Initialize(int entryId)
        {
            LoadEntry(entryId);
            LoadTitleDisplayMode();
        }

        public Task OnNavigatedToAsync()
        {
            LoadNotes();
            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void LoadEntry(int entryId)
        {
            _entry = _entryManager.FindById(entryId);
        }

        private void LoadTitleDisplayMode()
        {
            var titleDisplayMode = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");
            SelectedTitleMode = int.TryParse(titleDisplayMode, out int mode) && mode == 1
                ? "Translated"
                : "Original";
        }

        private void LoadNotes()
        {
            if (_entry == null) return;

            try
            {
                NoteData.Clear();
                var notes = _noteManager.Query(NoteManager.ByEntriesId(new List<int> { _entry.Id }));

                foreach (var note in notes)
                {
                    NoteData.Add(new NoteData(note.Id, note.NoteTitle ?? "未命名笔记"));
                }
            }
            catch (Exception ex)
            {
                _snackbarService.Show("加载笔记失败", ex.Message);
            }
        }

        [RelayCommand]
        private async Task NewNote()
        {
            if (_entry == null) return;

            try
            {
                // 创建新笔记
                var newNote = new Note
                {
                    NoteTitle = $"新笔记-{DateTime.Now:yyyyMMdd-HHmmss}",
                    EntriesId = new List<int> { _entry.Id },
                    Content = $"# {_entry.OriginalName}\n\n## 第{DateTime.Now:yyyy-MM-dd}条笔记"
                };

                _noteManager.Add(newNote);

                // 打开编辑窗口
                _navigationService.NavigateTo(typeof(NoteEditPage), newNote.Id);

                // 刷新列表
                await Task.Delay(300); // 等待编辑窗口打开
                LoadNotes();

                _snackbarService.Show("操作成功", "已创建新笔记");
            }
            catch (Exception ex)
            {
                _snackbarService.Show("创建失败", ex.Message);
            }
        }

        [RelayCommand]
        private void NoteView(int noteId)
        {
            try
            {
                var note = _noteManager.FindById(noteId);
                if (note == null)
                {
                    _snackbarService.Show("错误", "未找到指定笔记");
                    return;
                }

                // 打开只读查看窗口
                _navigationService.NavigateTo(typeof(NoteViewPage), noteId, true);
            }
            catch (Exception ex)
            {
                _snackbarService.Show("打开失败", ex.Message);
            }
        }

        [RelayCommand]
        private void NoteDelete(int noteId)
        {
            try
            {
                var result = MessageBox.Show(
                    "确定要删除此笔记吗？此操作不可恢复！",
                    "确认删除",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _noteManager.RemoveById(noteId);
                    var note = NoteData.FirstOrDefault(n => n.NoteId == noteId);
                    if (note != null) NoteData.Remove(note);
                    _snackbarService.Show("操作成功", "笔记已删除");
                }
            }
            catch (Exception ex)
            {
                _snackbarService.Show("删除失败", ex.Message);
            }
        }

        public void AddResources(string filePath)
        {
            try
            {
                if (_entry == null || !File.Exists(filePath)) return;

                // 读取Markdown内容
                var content = File.ReadAllText(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                // 创建笔记并关联
                var newNote = new Note
                {
                    NoteTitle = fileName,
                    EntriesId = new List<int> { _entry.Id },
                    Content = content
                };

                _noteManager.Add(newNote);
                LoadNotes();

                _snackbarService.Show("导入成功", $"已导入笔记: {fileName}");
            }
            catch (Exception ex)
            {
                _snackbarService.Show("导入失败", ex.Message);
            }
        }

        public string EntryTitle => SelectedTitleMode switch
        {
            "Original" => _entry?.OriginalName ?? "",
            "Translated" => _entry?.TranslatedName ?? "",
            _ => _entry?.OriginalName ?? ""
        };
    }

    public record NoteData(int NoteId, string NoteName);
}
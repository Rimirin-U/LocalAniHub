using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalAniHubFront.Helpers;
using LocalAniHubFront.ViewModels.Windows;
using LocalAniHubFront.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class EntryWindow_NoteManageViewModel : ObservableObject
    {
        private readonly NoteManager _noteManager = new();
        private readonly NoteService _noteService = new();
        private readonly EntryManager _entryManager = new();
        private readonly Entry _entry;

        [ObservableProperty]
        private string _subtitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<NoteItem> _noteData = new();

        // 公开命令供XAML绑定
        public ICommand NoteViewCommand { get; }
        public ICommand NoteDeleteCommand { get; }

        public EntryWindow_NoteManageViewModel(int entryId)
        {
    //        // 测试数据（注释正式逻辑）
    //        _noteData = new ObservableCollection<NoteItem>
    //{
    //    new NoteItem { NoteId = 1, NoteName = "测试笔记1" },
    //    new NoteItem { NoteId = 2, NoteName = "测试笔记2" }
    //};
            _entry = _entryManager.FindById(entryId)
                ?? throw new ArgumentException($"未找到ID为{entryId}的条目");

            // 初始化命令
            NoteViewCommand = new RelayCommand<int>(NoteView);
            NoteDeleteCommand = new RelayCommand<int>(NoteDelete);

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            UpdateSubtitle();
            LoadNotes();
        }

        private void UpdateSubtitle()
        {
            // 从全局设置读取显示偏好 (0:原名 1:译名)
            var displaySetting = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");

            string displayName = displaySetting == "0"
                ? _entry.TranslatedName
                : _entry.OriginalName;

            Subtitle = $"{displayName} ";
        }

        private void LoadNotes()
        {
            NoteData.Clear();
            var notes = _noteManager.Query(NoteManager.ByEntriesId(new List<int> { _entry.Id }))
                                   .OrderByDescending(n => n.Id);

            foreach (var note in notes)
            {
                NoteData.Add(new NoteItem
                {
                    NoteId = note.Id,
                    NoteName = note.NoteTitle
                });
            }
        }

        public void AddResources(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("文件不存在", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string content = File.ReadAllText(filePath);
                string title = Path.GetFileNameWithoutExtension(filePath);

                if (_noteManager.Query(n => n.NoteTitle == title).Any())
                {
                    title = $"{title}_{DateTime.Now:yyyyMMddHHmmss}";
                }

                var newNote = new Note
                {
                    NoteTitle = title,
                    EntriesId = new List<int> { _entry.Id }
                };

                _noteManager.Add(newNote);
                _noteService.SaveNote(title, content);
                LoadNotes();

                MessageBox.Show($"成功导入笔记: {title}", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void NewNote()
        {
            var noteTitle = $"{_entry.OriginalName} 评价 {DateTime.Now:yyyyMMddHHmm}";
            var newNote = new Note
            {
                NoteTitle = noteTitle,
                EntriesId = new List<int> { _entry.Id }
            };

            _noteManager.Add(newNote);
            _noteService.SaveNote(noteTitle, "");
            LoadNotes();
            NoteView(newNote.Id); // 直接调用方法而不是通过命令
        }

        private void NoteView(int noteId)
        {
            var note = _noteManager.FindById(noteId);
            if (note != null)
            {
                string content = _noteService.LoadNoteContent(note);

                var markdownWindow = new MarkdownWindow(
                    noteId: note.Id,
                    openOp: MarkdownWindow_OpenOp.Edit)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Application.Current.MainWindow
                };

                var viewModel = (MarkdownViewModel)markdownWindow.DataContext;
                viewModel.MarkdownText = content;

                markdownWindow.ShowDialog();
            }
        }

        private void NoteDelete(int noteId)
        {
            var note = _noteManager.FindById(noteId);
            if (note != null)
            {
                var result = MessageBox.Show($"确定要删除笔记 '{note.NoteTitle}' 吗?",
                    "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _noteService.DeleteNoteFile(note);
                    _noteManager.RemoveById(note.Id); // 确保从管理器中也删除
                    LoadNotes();
                }
            }
        }
    }

    public class NoteItem
    {
        public int NoteId { get; set; }
        public string NoteName { get; set; } = string.Empty;
    }
}
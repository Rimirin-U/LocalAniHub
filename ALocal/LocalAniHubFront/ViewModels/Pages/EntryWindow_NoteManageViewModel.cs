using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BasicClassLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using LocalAniHubFront.Models;
using LocalAniHubFront.Views.Windows;


namespace LocalAniHubFront.ViewModels.Pages
{
    public partial class EntryWindow_NoteManageViewModel : ObservableObject, INavigationAware
    {
        // record struct 定义
        public readonly record struct NoteData(int NoteId, string NoteTitle)
        {
            // 添加显示用属性
            public string NoteName => NoteTitle;
           
        }
        // 服务实例
        private readonly NoteManager _noteManager = new NoteManager();
        private readonly EntryManager _entryManager = new EntryManager();
        private readonly NoteService _noteService = new NoteService();

        // 原始数据源
        private Entry? _entry;

        // 笔记数据源
        public ObservableCollection<NoteData> NotesData { get; } = new();

        // 标题显示模式相关属性
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SubTitle))]
        private string _selectedTitleMode;

        private bool _isInitialized = false;
        private int _entryId;

        public EntryWindow_NoteManageViewModel(int entryId)
        {
            _entryId = entryId;
        }

        public Task OnNavigatedToAsync()
        {
            if (!_isInitialized)
                InitializeViewModel();

            return Task.CompletedTask;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private void InitializeViewModel()
        {
            LoadEntry();
            LoadInitialState();
            LoadNotes();
            _isInitialized = true;
        }

        private void LoadEntry()
        {
            _entry = _entryManager.FindById(_entryId);
        }

        private void LoadInitialState()
        {
            // 加载标题显示模式
            var titleDisplayMode = GlobalSettingsService.Instance.GetValue("entryWindowMainTitle");
            SelectedTitleMode = int.TryParse(titleDisplayMode, out int mode) && mode == 1 ? "Translated" : "Original";

        }

        private void LoadNotes()
        {
            if (_entry == null) return;

            var notes = _noteManager.Query(n => n.EntriesId.Contains(_entry.Id));
            NotesData.Clear();

            foreach (var note in notes)
            {
                NotesData.Add(new NoteData(
                    NoteId: note.Id,
                    NoteTitle: note.NoteTitle
                ));
            }
        }

        // 计算属性：根据SelectedTitleMode返回原名或译名
        public string SubTitle => SelectedTitleMode switch
        {
            "Original" => _entry?.OriginalName ?? "",
            "Translated" => _entry?.TranslatedName ?? "",
            _ => _entry?.OriginalName ?? ""
        };

        [RelayCommand]
        private void NoteViewCommand(int noteId)
        {
            // 创建新窗口并传入 NoteId
            var window = new MarkdownWindow(noteId, MarkdownWindow_OpenOp.View);
            window.Show();
        }

        [RelayCommand]
        private void NoteDeleteCommand(int noteId)
        {
            var note = _noteManager.FindById(noteId);
            if (note != null)
            {
                _noteService.DeleteNoteFile(note); // 删除文件
                _noteManager.RemoveById(noteId); // 删除数据库记录

                var noteToRemove = NotesData.FirstOrDefault(n => n.NoteId == noteId);
                if (NotesData.Contains(noteToRemove))
                {
                    NotesData.Remove(noteToRemove);
                }
            }
        }

        [RelayCommand]
        private void NewNoteCommand()
        {
            // 创建新笔记并关联到当前条目
            var newNote = new Note
            {
                NoteTitle = $"新笔记-{DateTime.Now:yyyyMMdd-HHmmss}",
                EntriesId = new List<int> { _entryId }
            };

            _noteManager.Add(newNote);
            // 创建空内容文件
            _noteService.SaveNote(newNote, "# 新笔记\n\n在这里写下你的评价...");

            NotesData.Add(new NoteData(newNote.Id, newNote.NoteTitle));

            // 打开编辑窗口
            var window = new MarkdownWindow(newNote.Id, MarkdownWindow_OpenOp.View);
            window.Show();
        }

        [RelayCommand]
        private void NoteEditCommand(int noteId)
        {
            var note = _noteManager.FindById(noteId);
            if (note != null)
            {
                var window = new MarkdownWindow(noteId, MarkdownWindow_OpenOp.Edit);
                window.Closed += (s, e) => RefreshNotes();
                window.Show();
            }
        }

        private void RefreshNotes()
        {
            LoadEntry();
            LoadNotes();
        }

        public void AddResources(string filePath)
        {
            // 从Markdown文件导入笔记
            try
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                var content = System.IO.File.ReadAllText(filePath);


                var newNote = new Note
                {
                    NoteTitle = fileName,
                    EntriesId = new List<int> { _entryId }
                };

                _noteManager.Add(newNote);
                _noteService.SaveNote(newNote, content); // 保存内容到文件
                
                NotesData.Add(new NoteData(newNote.Id, newNote.NoteTitle));

                 // 可选：强制属性更改通知
                  OnPropertyChanged(nameof(NotesData));
            }
            catch (Exception ex)
            {
                // 处理异常，例如显示错误消息
                System.Diagnostics.Debug.WriteLine($"导入笔记失败: {ex.Message}");
            }
        }
        }
    }
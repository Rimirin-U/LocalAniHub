using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasicClassLibrary
{
    public class NoteService
    {
        private readonly string _notesDirectory;
        private readonly NoteManager _noteManager;
        public NoteService()
        {
            var globalBaseFolder = GlobalSettingsService.Instance.GetValue("globalBaseFolder");
            _notesDirectory = Path.Combine(globalBaseFolder, "Note");
            Directory.CreateDirectory(_notesDirectory);
            _noteManager = new NoteManager();
        }

        // 通过标题创建笔记文件并返回笔记对象
        public void SaveNote(string noteTitle, string content)
        {
            ValidateNoteTitle(noteTitle);
            if (content == null) throw new ArgumentNullException(nameof(content));

            // 创建新的笔记对象
            var note = new Note
            {
                NoteTitle = noteTitle
            };
            //_noteManager.Add(note); // 添加到数据库

            // 保存笔记内容
            File.WriteAllText(GetNoteFilePath(noteTitle), content);
        }

        // 通过Note对象更新笔记文件
        public void SaveNote(Note note, string content)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));
            ValidateNoteTitle(note.NoteTitle);
            if (content == null) throw new ArgumentNullException(nameof(content));

            File.WriteAllText(GetNoteFilePath(note.NoteTitle), content);
        }

        // 读取笔记内容
        public string LoadNoteContent(Note note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            string path = GetNoteFilePath(note.NoteTitle);
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        // 删除笔记文件
        public void DeleteNoteFile(Note note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            string path = GetNoteFilePath(note.NoteTitle);
            if (File.Exists(path)) File.Delete(path);
            _noteManager.RemoveById(note.Id); // 删除数据库中的笔记记录
        }

        private string GetNoteFilePath(string noteTitle)
            => Path.Combine(_notesDirectory, $"{noteTitle}.md");

        private void ValidateNoteTitle(string noteTitle)
        {
            if (string.IsNullOrWhiteSpace(noteTitle))
                throw new ArgumentException("笔记标题不能为空或空白");
        }
    }
}

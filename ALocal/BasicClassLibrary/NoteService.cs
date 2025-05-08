using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BasicClassLibrary
{
    public class NoteService
    {
        private readonly NoteManager NoteManager;
        private readonly string BaseDirectory;

        public NoteService(NoteManager noteManager)
        {
            this.NoteManager = noteManager;
            BaseDirectory = "/base/NoteService";
            Directory.CreateDirectory(BaseDirectory); // 确保服务文件夹存在
            //该方法会创建指定路径的目录，要是该目录已经存在，就不会再次创建。
            //如果指定路径中的父目录不存在，此方法会递归创建所有必要的父目录。
        }

        // 保存笔记为 Markdown 文件
        public void SaveNoteAsMarkdown(Note note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));
            if (string.IsNullOrWhiteSpace(note.NoteTitle)) throw new ArgumentException("NoteTitle cannot be null or empty.", nameof(note.NoteTitle));

            string filePath = Path.Combine(BaseDirectory, $"{note.NoteTitle}.md");
            File.WriteAllText(filePath, note.Content);
        }

        // 加载 Markdown 文件为 Note 对象
        public Note LoadNoteFromMarkdown(string noteTitle)
        {
            if (string.IsNullOrWhiteSpace(noteTitle)) throw new ArgumentException("NoteTitle cannot be null or empty.", nameof(noteTitle));

            string filePath = Path.Combine(BaseDirectory, $"{noteTitle}.md");
            if (!File.Exists(filePath)) throw new FileNotFoundException("The specified note file does not exist.", filePath);

            string content = File.ReadAllText(filePath);
            return new Note
            {
                NoteTitle = noteTitle,
                Content = content
            };
        }
    }
}

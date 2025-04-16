using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class NoteManager:Manager<Note>
    {
        //构造函数
        public NoteManager():base(new AppDbContext()) { }
        // 根据作品ID查找关联笔记
        public static Func<Note, bool> ByEntryId(int entryId) =>
            n => n.EntryId == entryId;

        // 根据剧集ID查找关联笔记
        public static Func<Note, bool> ByEpisodeId(int episodeId) =>
            n => n.EpisodeId == episodeId;

        // 根据关键字搜索笔记内容
        public static Func<Note, bool> ByKeyword(string keyword) =>
            n => n.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase);

        // 获取未关联任何作品或剧集的笔记
        public static readonly Func<Note, bool> OrphanNotes =
            n => n.EntryId == null && n.EpisodeId == null;
    }
    //public class NoteService
    //{
    //    private readonly List<Note> _notes = new List<Note>();
  
    //    // 创建新笔记
    //    public Note Create(string content, ICollection<int> entriesId = null, ICollection<int> episodesId = null)
    //    {
    //        if (string.IsNullOrWhiteSpace(content))
    //            throw new ArgumentException("笔记内容不能为空");

    //        var note = new Note
    //        {
    //            Content = content,
    //            EntriesId = entriesId?.ToList() ?? new List<int>(),
    //            EpisodesId = episodesId?.ToList() ?? new List<int>()
    //        };

    //        _notes.Add(note);
    //        return note;
    //    }

    //    // 根据ID获取笔记
    //    public Note GetById(int id)
    //    {
    //        return _notes.FirstOrDefault(n => n.Id == id);
    //    }
    //    // 获取所有笔记
    //    public List<Note> GetAll()
    //    {
    //        return _notes.ToList();
    //    }

    //    // 删除笔记
    //    public void Delete(int id)
    //    {
    //        var note = GetById(id);
    //        if (note == null)
    //        {
    //            throw new InvalidOperationException($"未找到ID为 {id} 的笔记");
    //        }
    //         _notes.Remove(note);
    //    }

    //    // 更新笔记内容
    //    public void UpdateContent(int id, string newContent)
    //    {
    //        var note = GetById(id);
    //        if (note == null || string.IsNullOrWhiteSpace(newContent))
    //        {
    //            throw new InvalidOperationException($"未找到ID为 {id} 的笔记 或者 新内容为空");
    //        }

    //        note.Content = newContent;
           
    //    }

    //    //添加关联
    //    // 为笔记添加作品关联
    //    public void AddEntry(int noteId, int entryId)
    //    {
    //        var note = GetById(noteId);
    //        if (note == null)
    //        {
    //            throw new InvalidOperationException($"未找到ID为 {noteId} 的笔记");
    //        }

    //            if (!note.EntriesId.Contains(entryId))
    //        {
    //            note.EntriesId.Add(entryId);
    //        }
           
    //    }
    //    // 为笔记添加剧集关联
    //    public void AddEpisode(int noteId, int episodeId)
    //    {
    //        var note = GetById(noteId);
    //        if (note == null)
    //        {
    //            throw new InvalidOperationException($"未找到ID为 {noteId} 的笔记");
    //        }

    //            if (!note.EpisodesId.Contains(episodeId))
    //        {
    //            note.EpisodesId.Add(episodeId);
    //        }
    //    }

    //    //移除关联
    //    // 从笔记移除作品关联
    //    public void RemoveEntry(int noteId, int entryId)
    //    {
    //        var note = GetById(noteId);
    //        if (note == null)
    //        {
    //            throw new InvalidOperationException($"未找到ID为 {noteId} 的笔记");
    //        }
    //        if (!note.EntriesId.Remove(entryId))
    //        {
    //            throw new InvalidOperationException($"笔记ID为 {noteId} 的笔记中不存在关联的作品ID {entryId}");
    //        }
            
    //    }
    //    // 从笔记移除剧集关联
    //    public void RemoveEpisode(int noteId, int episodeId)
    //    {
    //        var note = GetById(noteId);
    //        if (note == null)
    //        {
    //            throw new InvalidOperationException($"未找到ID为 {noteId} 的笔记");
    //        }

    //        if (!note.EpisodesId.Remove(episodeId))
    //        {
    //            throw new InvalidOperationException($"笔记ID为 {noteId} 的笔记中不存在关联的剧集ID {episodeId}");
    //        }
    //    }

    //    //查找笔记
    //    // 根据作品ID查找关联笔记
    //    public List<Note> FindByEntry(int entryId)
    //    {
    //        return _notes.Where(n => n.EntriesId.Contains(entryId)).ToList();
    //    }
    //    // 根据剧集ID查找关联笔记
    //    public List<Note> FindByEpisode(int episodeId)
    //    {
    //        return _notes.Where(n => n.EpisodesId.Contains(episodeId)).ToList();
    //    }

    //    // 根据关键字搜索笔记内容
    //    public List<Note> Search(string keyword)
    //    {
    //        if (string.IsNullOrWhiteSpace(keyword))
    //            return new List<Note>();

    //        return _notes
    //            .Where(n => n.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))//不区分大小写字母
    //            .ToList();
    //    }

    //    // 获取未关联任何作品或剧集的笔记
    //    public List<Note> GetOrphanNotes()
    //    {
    //        return _notes
    //            .Where(n => !n.EntriesId.Any() && !n.EpisodesId.Any())//!n.EntriesId.Any()表示当EntriesId集合为空时返回true
    //            .ToList();
    //    }
    //}
}

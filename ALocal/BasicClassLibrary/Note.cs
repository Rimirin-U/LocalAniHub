﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Note : IEntityWithId, IEntriesNavigation, IEpisodesNavigation
    {
        public Note()
        {
            this.NoteTitle = string.Empty;//笔记的名称
            this.EntriesId = new List<int>();
            this.Entries = new List<Entry>();
            this.EpisodesId = new List<int>();
            this.Episodes = new List<Episode>();
        }

        public int Id { get; set; }

        public string NoteTitle { get; set; } // 笔记名称
        public ICollection<int> EntriesId { get; set; }
        public ICollection<Entry> Entries { get; set; }
        public ICollection<int> EpisodesId { get; set; }
        public ICollection<Episode> Episodes { get; set; }

        public string Content { get; set; }
    }

    public partial class AppDbContext : DbContext
    {
        DbSet<Note> notes { get; set; }
    }
}

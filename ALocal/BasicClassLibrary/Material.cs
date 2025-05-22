﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class Material : IEntityWithId, IEntryNavigation
    {
        // Constructor
        public Material(int? entryId, string name, string kind, string path)
        {
            EntryId = entryId;
            // Entry = entry;
            this.Name = name;
            this.Kind = kind;
            this.Path = path;
        }

        public int Id { get; set; }

        // Navigation Properties
        public int? EntryId { get; set; }
        public Entry? Entry { get; set; }

        // Properties
        public string Name { get; set; }
        public string Kind { get; set; }
        public string Path { get; set; }
    }

    public partial class AppDbContext : DbContext
    {
        public DbSet<Material> Materials { get; set; }
    }
}

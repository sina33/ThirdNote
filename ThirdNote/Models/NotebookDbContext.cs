using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ThirdNote.Models
{
    public class NotebookDbContext : DbContext
    {
        public NotebookDbContext() 
        {
            Database.SetInitializer<DbContext>(new CreateDatabaseIfNotExists<DbContext>() );
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }
        public DbSet<Relapse> Relapses { get; set; }
        //public DbSet<Meta> Metas { get; set; }

    }
}
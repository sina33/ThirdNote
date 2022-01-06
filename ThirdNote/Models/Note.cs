using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ThirdNote.Models
{
    public enum Status
    {
        _ = 0,
        TO_DO = 1,
        IN_PROGRESS = 2,
        DONE = 3, 
        POSTPONE = 4,
        BACK_LOG = 5
    }

    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime WrittenDate { get; set; }
        public Boolean Markdown { get; set; }
        //[Display(Name ="PV")]
        public Boolean Hidden { get; set; }
        public Boolean Pin { get; set; }
        //public Boolean Actionable { get; set; }
        //public Status Status { get; set; }
        public int ViewCount { get; set; }
        //public virtual Meta Meta { get; set; }
        public virtual ICollection<NoteTag> NoteTags { get; set; }

        public Note()
        {
            this.CreatedDate = DateTime.Now;
        }
    }
}
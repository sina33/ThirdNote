using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ThirdNote.Models
{
    public class Tag
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public string Lable_fa { get; set; }
        public string Lable_en { get; set; }

        public virtual ICollection<NoteTag> NoteTags { get; set; }
    }


    public class NoteTag
    {
        public int ID { get; set; }
        //[Key, Column(Order = 1)]
        //public int TagId { get; set; }
        //[Key, Column(Order = 2)]
        //public int NoteId { get; set; }

        public int NoteID { get; set; }
        public Note Note { get; set; }

        public int TagID { get; set; }
        public Tag Tag { get; set; }

        public NoteTag(Note note, Tag tag)
        {
            this.Note = note;
            this.Tag = tag;
        }

        public NoteTag()
        {

        }
    }
}
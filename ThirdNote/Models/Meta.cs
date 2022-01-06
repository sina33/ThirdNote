using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThirdNote.Models
{

    public class Meta
    {
        public int Id { get; set; }
        public Boolean Markdown { get; set; }
        public Boolean Hidden { get; set; }
        public Boolean Pin { get; set; }
        public Boolean Actionable { get; set; }
        public Status Status { get; set; }
        
        public int NoteId { get; set; }
        [ForeignKey("NoteId")]
        public virtual Note Note { get; set; }
    }
}
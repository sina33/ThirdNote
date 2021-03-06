//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThirdNote.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Note
    {
        public Note()
        {
            this.Title = "";
            this.Tags = new HashSet<Tag>();
            this.Rates = new HashSet<Rate>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int MetaID { get; set; }
    
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
        public virtual Meta Meta { get; set; }
        public virtual Meta Meta1 { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cms.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Review
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string reviewer_id { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public string Evaluation { get; set; }
        public Nullable<int> Rating { get; set; }
        public Nullable<int> paper_id { get; set; }
        public Nullable<int> assign_id { get; set; }
        public string status { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Paper Paper { get; set; }
    }
}
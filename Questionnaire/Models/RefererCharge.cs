//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuestionnaireProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RefererCharge
    {
        public int Id { get; set; }
        public string NewClient_Id { get; set; }
        public string Referer_Id { get; set; }
        public Nullable<int> CountToPay { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
    }
}

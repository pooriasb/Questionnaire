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
    
    public partial class UsersRespons
    {
        public long Id { get; set; }
        public Nullable<int> Response_Id { get; set; }
        public string Question_Id { get; set; }
        public string Response { get; set; }
        public Nullable<bool> IsCorrect { get; set; }
        public string User_Id { get; set; }
    
        public virtual Respons Respons { get; set; }
    }
}

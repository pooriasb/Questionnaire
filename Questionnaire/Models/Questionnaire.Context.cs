﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QuestionnaireEntities : DbContext
    {
        public QuestionnaireEntities()
            : base("name=QuestionnaireEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AdminAnswer> AdminAnswers { get; set; }
        public virtual DbSet<AdminQuestion> AdminQuestions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<EX_AnatomicalRespnses> EX_AnatomicalRespnses { get; set; }
        public virtual DbSet<EX_MultipleChoiceAnswers> EX_MultipleChoiceAnswers { get; set; }
        public virtual DbSet<EX_MultipleChoiceResponses> EX_MultipleChoiceResponses { get; set; }
        public virtual DbSet<EX_QuestionerProfiles> EX_QuestionerProfiles { get; set; }
        public virtual DbSet<EX_ResponderProfiles> EX_ResponderProfiles { get; set; }
        public virtual DbSet<EX_VBAnswers> EX_VBAnswers { get; set; }
        public virtual DbSet<EX_VBResponses> EX_VBResponses { get; set; }
        public virtual DbSet<Fohsh> Fohshes { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<MoneyToReferer> MoneyToReferers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Option> Options { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<QuestionnaireCategory> QuestionnaireCategories { get; set; }
        public virtual DbSet<QuestionnaireRequest> QuestionnaireRequests { get; set; }
        public virtual DbSet<Questionnaire> Questionnaires { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<RefererCharge> RefererCharges { get; set; }
        public virtual DbSet<Respons> Responses { get; set; }
        public virtual DbSet<SpecialOffer> SpecialOffers { get; set; }
        public virtual DbSet<StudyField> StudyFields { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UploadedQuestionnaire> UploadedQuestionnaires { get; set; }
        public virtual DbSet<UsersRespons> UsersResponses { get; set; }
        public virtual DbSet<ResponderReport> ResponderReports { get; set; }
    }
}

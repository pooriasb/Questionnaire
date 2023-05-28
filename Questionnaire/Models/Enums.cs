namespace QuestionnaireProject.Models
{
    public class Enums
    {
        public enum QuestionType
        {
            BenchMarking = 0,
            Validation = 1,
            Main = 2,
            Anatomical = 3,
            MainWithPicture = 4,
            SiteBenchMarking = 5
        }

        public enum BenchMarkingQuestion
        {
            MaxAnswersNumber = 10,
            MinAnswersNumber = 2,
            MaxNumber = 10,
            MinNumber = 0,
        }

        public enum SiteBenchMarkingQuestion
        {
            MaxAnswersNumber = 10,
            MinAnswersNumber = 2,
            MaxNumber = 10,
            MinNumber = 0
        }

        public enum MainQuestion
        {
            MaxAnswersNumber = 10,
            MinAnswersNumber = 2,
            //MaxNumber = 10,
            //MinNumber = 0
        }

        public enum ValidationQuestion
        {
            MaxAnswersNumber = 10,
            MinAnswersNumber = 2,
            //MaxNumber = 10,
            //MinNumber = 0
        }

        public enum MediaType
        {
            NoMedia = 0,
            Picture = 1,
            Film = 2
        }

        public enum RequestStatus
        {
            Created = 0,
            Rejected = 1,
            Pending = 2,
            Accepted = 3,
            Paid = 4,
            Finished = 5,
            CompletelyRejected = 6
        }


        public enum NotificationType
        {
            Danger = 0,
            Success = 1,
            Info = 2,
            Primary = 3,
            Warning = 4            
        }

        public enum QuestionServiceMode
        {
            EditMode = 0,
            CreateMode = 1,
            DeleteMode = 2
        }

        public enum Special
        {
            Ladder = 0,
            Bold = 1,
            Color = 2,
            ExtraDays = 3
        }

        public enum PaymentType
        {
            AdminToResponder = 0,
            QuestionerToSiteForQuestionnaire = 1,
            QuestionerToSiteForSpecial = 2
        }

        public enum Sex
        {
            FeMale = 0,
            Male = 1,
            All = 2
        }

        public enum Questionnaire
        {
            MaxQuestionnairesNumberForEachUser = 10,

        }

        public enum AndroidCommands
        {
            Void = 0,
            Message = 1,
            Html = 2,
            NForceUpdate = 3,
            ForceUpdate = 4,
            LockApp = 5
        }
    }


}
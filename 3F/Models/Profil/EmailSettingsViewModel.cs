using _3F.BusinessEntities.Enum;
using System.Collections.Generic;

namespace _3F.Web.Models.Profil
{
    public class EmailSettingsViewModel : BaseViewModel
    {
        public EmailSettingsViewModel()
        {
            Title = "Emailová upozornění";
            Icon = "icon-warning-sign";
            MainCategories = new List<EmailCategory>();
            Categories = new List<EmailCategory>();
            MainCategoryIds = new int[] { };
            CategoryIds = new int[] { };
        }

        public bool SendMessagesToMail { get; set; }
        public bool SendMessagesFromAdminToMail { get; set; }
        public bool SendMayBeEventNotice { get; set; }
        public bool SendNewAlbumsToMail { get; set; }
        public bool SendNewSummaryToMail { get; set; }
        public bool Stay { get; set; }
        public List<EmailCategory> MainCategories { get; set; }
        public int[] MainCategoryIds { get; set; }
        public List<EmailCategory> Categories { get; set; }
        public int[] CategoryIds { get; set; }

    }

    public class EmailCategory
    {
        public int Id { get; set; }
        public bool IsAssigned { get; set; }
        public string Name { get; set; }
        public MainCategory MainCategory { get; set; }
    }
}
using _3F.BusinessEntities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3F.Web.Models.EventModels
{
    public class DuplicateEvent : BaseViewModel
    {
        [Required]
        public string Name { get; set; }
        public string HtmlName { get; set; }
        [Range(1,720)]
        public int Days { get; set; }
        public DateTime NewStartDate { get; set; }
        public string DuplicationType { get; set; }
    }

    public class ExternParticipant
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public bool IsActive { get; set; }
        public bool NeedConfirmation { get; set; }
        public string Status { get; set; }

        public ExternParticipant() { }
    }

    public class EventWithExternParticipant : BaseViewModel
    {
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public IEnumerable<ExternParticipant> ExternParticipants { get; set; }
        [Range(0, 10)]
        public int Count { get; set; }
    }

    public class OrganisatorMessage : BaseViewModel
    {
        public bool Prijdu { get; set; }
        public bool Mozna { get; set; }
        public bool Rezervace { get; set; }
        public bool Nahradnik { get; set; }
        public bool NepotvrzenaRezervace { get; set; }
        public bool PoTerminu { get; set; }
        public bool RezervacePropadla { get; set; }
        [Required]
        public string Message { get; set; }
        public string HtmlName { get; set; }
    }

    public class EventParticipantHistoryOverview : BaseViewModel
    {
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public bool IsFinished { get; set; }
        public int Id_Event { get; set; }
        public IEnumerable<string[]> ParticipantHistory { get; set; }
        public IEnumerable<Participant> LoginParticipants { get; set; }
        public bool IsAdministrator { get; set; }
    }

    public class EventInvitationsViewModel : BaseViewModel
    {
        public List<User> Users { get; set; }
        public string UserNames { get; set; }

        public bool IsOrganisator { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public bool IsAssigned { get; set; }
        public string HtmlName { get; set; }
        public string Name { get; set; }
        public MainCategory MainCategory { get; set; }
    }
}
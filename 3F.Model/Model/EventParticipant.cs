namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventParticipant")]
    public partial class EventParticipant : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_User { get; set; }

        public int Id_Event { get; set; }

        public int? Id_Payment { get; set; }

        public EventLoginEnum EventLoginStatus { get; set; }

        public DateTime Time { get; set; }

        public bool IsExternal { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Event Event { get; set; }

        public virtual Payment Payment { get; set; }
    }

    public enum EventLoginEnum
    {
        [Description("Přijdu")]
        Prijdu = 1,
        [Description("Možná dorazím")]
        Mozna = 2,
        [Description("Nepřijdu")]
        Neprijdu = 3,
        [Description("Náhradník")]
        Nahradnik = 4,
        [Description("Rezervace")]
        Rezervace = 5,
        [Description("Odhlášen po termínu")]
        PoTerminu = 6,
        [Description("Nevyjádřeno")]
        Nevyjadreno = 7,
        [Description("Rezervace propadla")]
        RezervacePropadla = 8,
        [Description("Omluven")]
        Omluven = 9,
        [Description("Neomluven")]
        Neomluven = 10,
        [Description("Nepotvrzená rezervace")]
        NepotvrzenaRezervace = 11,
        [Description("Vyřizuji")]
        Vyrizuji = 12,
    }
}

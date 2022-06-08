using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace _3F.Web.Definitions
{
    public enum RolesEnum
    {
        [Description("Účetní")]
        Accountant = 1,
        [Description("Tvůrce akcí")]
        EventCreator = 2,
        [Description("Administrátor")]
        Administrator = 3,
        [Description("Administrátor badmintonu")]
        BadmintonAdmin = 4,
        [Description("Hráč badmintonu")]
        BadmintonUser = 5,
        [Description("Tvůrce předplacených akcí")]
        CertifiedOrganisator = 6,
        [Description("Tvůrce soukromých akcí")]
        PrivateOrganisator = 7,
        [Description("Administrátor turistiky")]
        TouristAdmin = 8,
        [Description("Člen rady")]
        Council = 9,
        [Description("Člen dozorčí komise")]
        Supervisor = 10,
        [Description("Předseda")]
        Chief = 11,
        [Description("Schvalovatel běžných akcí")]
        EventConfirmator = 12,
    }

    public class Strings
    {
        public const string EventCreator = "EventCreator";
        public const string Administrator = "Administrator";
        public const string CertifiedOrganisator = "CertifiedOrganisator";
        public const string PrivateOrganisator = "PrivateOrganisator";
        public const string TouristAdmin = "TouristAdmin";
        public const string Council = "Council";
        public const string Chief = "Chief";
        public const string Supervisor = "Supervisor";
        public const string EventConfirmator = "EventConfirmator";

        public const string Organisation = "Společné aktivity z.s.";
        public const string System = "Systém";
        public const string DateFormat = "dd.MM.yyyy HH:mm:ss";
    }
}
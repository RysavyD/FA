namespace _3F.Web.Controllers.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [DataContract(Name = "Event")]
    public class EventApiModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Price { get; set; }

        [DataMember]
        public int ActionId { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public DateTime StopTime { get; set; }

        [DataMember]
        public IEnumerable<OrganisatorApiModel> Organisators { get; set; }
    }

    [DataContract(Name = "Organisator")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "There are only API models")]
    public class OrganisatorApiModel
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract(Name = "AccountancyInfo")]
    public class AccountancyInfoApiModel
    {
        [DataMember]
        public int PocetPredplacenychAkci { get; set; }

        [DataMember]
        public int PocetUcastniku { get; set; }

        [DataMember]
        public int OdhadovaneVynosy { get; set; }

        [DataMember]
        public int VRezervaci { get; set; }

        [DataMember]
        public int JizZaplaceno { get; set; }
    }
}
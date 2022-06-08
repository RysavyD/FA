namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Organisation")]
    public partial class Organisation : IPrimaryKey, IHtmlName
    {
        public Organisation()
        {
            OrganisationUser = new HashSet<OrganisationMember>();
        }

        public int Id { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }

        public string HtmlName { get; set; }

        public int ICO { get; set; }

        public string BankAccount { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public virtual ICollection<OrganisationMember> OrganisationUser { get; set; }

    }
}

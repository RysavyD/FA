namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OldPassword")]
    public partial class OldPassword : IPrimaryKey
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public byte[] PasswordHash { get; set; }

        [Required]
        [MaxLength(128)]
        public byte[] PasswordSalt { get; set; }

        public int Id_User { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}

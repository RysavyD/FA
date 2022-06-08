namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MessageRecipient")]
    public partial class MessageRecipient : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_Message { get; set; }

        public int Id_User { get; set; }

        public bool Unreaded { get; set; }

        public bool Visible { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Message Message { get; set; }
    }
}

namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Message")]
    public partial class Message : IPrimaryKey
    {
        public Message()
        {
            MessageRecipient = new HashSet<MessageRecipient>();
        }

        public int Id { get; set; }

        public int Id_Sender { get; set; }

        [Required]
        [StringLength(50)]
        public string Subject { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Time { get; set; }

        public bool Visible { get; set; }

        public int? Id_ReplyMessage { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual ICollection<MessageRecipient> MessageRecipient { get; set; }
    }
}

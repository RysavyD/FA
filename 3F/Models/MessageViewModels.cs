using _3F.Model.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3F.Web.Models
{
    public class MessagesViewModel : BaseViewModel
    {
        public string Url { get; set; }
    }

    public class MessageViewModel : BaseViewModel
    {
        public User Sender { get; set; }
        public List<Recipient> Recipients { get; set; }
        [StringLength(50)]
        public string Subject { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        public string Time { get; set; }
        public string Id { get; set; }
        public int? ReplyId { get; set; }
        public bool Visible { get; set; }
        public bool Unreaded { get; set; }

        public MessageViewModel()
        {
            Recipients = new List<Recipient>();
        }
    }

    public class Recipient : User
    {
        public bool Unreaded { get; set; }

        public Recipient()
            : base()
        { }

        public Recipient(AspNetUsers user, bool unreaded)
            : base(user)
        {
            this.Unreaded = unreaded;
        }
    }

    public class ReplyMessageViewModel : MessageViewModel
    {
        public List<MessageViewModel> RepliesMessages { get; set; }
        public string RecipientNames { get; set; }

        public ReplyMessageViewModel()
            : base()
        {
            RepliesMessages = new List<MessageViewModel>();
            Title = "Vytvoření zprávy";
        }
    }

    public class MessageDetailViewModel : EnumerableBaseViewModel<MessageViewModel>
    {
        public int Id { get; set; }
        public bool ReplyAllAllowed { get; set; }
    }
}
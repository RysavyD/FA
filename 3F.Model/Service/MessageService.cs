using System.Linq;
using _3F.Model.Email;
using _3F.Model.Email.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;

namespace _3F.Model.Service
{
    public interface IMessageService
    {
        void SendMessage(Message message);
    }

    public class MessageService : IMessageService
    {
        private IRepository repository;
        private IEmailSender emailSender;

        public MessageService(IRepository repository, IEmailSender emailSender)
        {
            this.repository = repository;
            this.emailSender = emailSender;
        }

        public void SendMessage(Message message)
        {
            message.Subject = message.Subject.TakeSafetely(50);
            repository.Add(message);

            var messageMailModel = new NewMessageMailModel()
            {
                Sender = message.AspNetUsers.UserName,
                Subject = message.Subject,
                Text = message.Text,
            };

            var emailAdresses = message.MessageRecipient
                .Select(mr => mr.AspNetUsers)
                .Where(us => us.Profiles.SendMessagesToMail)
                .Select(us => us.Email)
                .ToArray();

            if (emailAdresses.Length > 0)
                emailSender.SendEmail(EmailType.NewMessage, messageMailModel, emailAdresses);
        }
    }
}

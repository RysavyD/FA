using _3F.Log;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;

namespace _3F.Model.Email
{
    public class SmtpMailSender : BaseEmailSender
    {
        ILogger logger;
        public SmtpMailSender(ILogger logger)
        {
            this.logger = logger;
        }

        protected override void SendEmail(MailMessage message)
        {
            Task.Run(() => SendMessages(message));
        }

        private void SendMessages(MailMessage message)
        {
            try
            {
                int GoogleMailMaximumRecipients = 90;
                if (message.Bcc.Count > GoogleMailMaximumRecipients)
                {
                    var addresses = message.Bcc.Select(b => b.Address).ToArray();
                    var addressGroups = addresses
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / GoogleMailMaximumRecipients)
                        .Select(x => x.Select(v => v.Value)
                        .ToList());
                    message.Bcc.Clear();

                    foreach(var group in addressGroups)
                    {
                        var newMessage = new MailMessage()
                        {
                            Body = message.Body,
                            BodyEncoding = message.BodyEncoding,
                            BodyTransferEncoding = message.BodyTransferEncoding,
                            From = message.From,
                            IsBodyHtml = message.IsBodyHtml,
                            Sender = message.Sender,
                            Subject = message.Subject,
                            SubjectEncoding = message.SubjectEncoding,
                        };

                        newMessage.To.Add(message.To.First());
                        foreach (var address in group)
                            newMessage.Bcc.Add(address);

                        SendMail(newMessage);
                    }
                }
                else
                {
                    SendMail(message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Odeslání mailu s předmětem {0} se nezdařilo z důvodu: {1}", message.Subject, ex.Message), "SmtpMailSender.SendEmail");
                logger.LogException(ex, "SmtpMailSender.SendEmail");
            }
        }

        private void SendMail(MailMessage message)
        {
            using (SmtpClient sendClient = new SmtpClient(Properties.Settings.Default.SMTPServer, Properties.Settings.Default.SMTPPort)) //sifrovane spojeni SSL 587, jinak 25
            {
                sendClient.EnableSsl = Properties.Settings.Default.UseSSL;
                sendClient.UseDefaultCredentials = Properties.Settings.Default.UseDefaultCredentials;
                sendClient.Credentials = new NetworkCredential(Properties.Settings.Default.SMTPUserName, Properties.Settings.Default.SMTPPassword);

                sendClient.Send(message);
                logger.LogDebug(string.Format("Odeslání mailu s předmětem {0}", message.Subject), "SmtpMailSender.SendEmail");
            }
        }
    }
}

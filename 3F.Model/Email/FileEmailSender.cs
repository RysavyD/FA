using _3F.Log;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace _3F.Model.Email
{
    public class FileEmailSender : BaseEmailSender
    {
        private ILogger logger;

        public FileEmailSender(ILogger logger)
        {
            this.logger = logger;
        }

        protected override void SendEmail(System.Net.Mail.MailMessage message)
        {
            Task.Run(() =>
            {
                var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/Emails");
                var fileName = Info.CentralEuropeNow.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";

                var builder = new StringBuilder();
                builder.AppendLine("To:");
                builder.AppendLine(string.Join(",", message.To));
                builder.AppendLine();
                builder.AppendLine("CC:");
                builder.AppendLine(string.Join(",", message.CC));
                builder.AppendLine();
                builder.AppendLine("BCC:");
                builder.AppendLine(string.Join(",", message.Bcc));
                builder.AppendLine();
                builder.AppendLine(message.Subject);
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine(message.Body);

                File.AppendAllText(Path.Combine(path, fileName), builder.ToString());
                logger.LogDebug("Email byl uložen jako " + fileName, "FileEmailSender.SendEmail");
            });
        }
    }
}

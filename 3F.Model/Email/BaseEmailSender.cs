using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Xml.Linq;
using _3F.Model.Model;
using _3F.Model.Email.Model;
// ReSharper disable InconsistentNaming

namespace _3F.Model.Email
{
    public abstract class BaseEmailSender : IEmailSender
    {
        const string HASH_PageUrl = "#HASH_PageUrl";
        const string HASH_EventUrl = "#HASH_EventUrl";
        const string HASH_EventName = "#HASH_EventName";
        const string HASH_EventPerex = "#HASH_EventPerex";
        const string HASH_Organisators = "#HASH_Organisators";
        const string HASH_EventStartTime = "#HASH_EventStartTime";
        const string HASH_EventStopTime = "#HASH_EventStopTime";
        const string HASH_EventPlace = "#HASH_EventPlace";
        const string HASH_EventCapacity = "#HASH_EventCapacity";
        const string HASH_EventPrice = "#HASH_EventPrice";
        const string HASH_EventMeetTime = "#HASH_EventMeetTime";
        const string HASH_EventMeetPlace = "#HASH_EventMeetPlace";
        const string HASH_CallbackUrl = "#HASH_CallbackUrl";
        const string HASH_RegistrationUrl = "#HASH_RegistrationUrl";
        const string HASH_UserName = "#HASH_UserName";
        const string HASH_Email = "#HASH_Email";
        const string HASH_Footer = "#HASH_Footer";
        const string HASH_Logo = "#HASH_Logo";
        const string HASH_MessageSender = "#HASH_MessageSender";
        //const string HASH_MessageRecievers = "#HASH_MessageRecievers";
        const string HASH_MessageSubject = "#HASH_MessageSubject";
        const string HASH_MessageText = "#HASH_MessageText";
        //const string HASH_CasaUrl = "#HASH_CasaUrl";
        const string HASH_QRCode = "#HASH_QRCode";
        const string HASH_BankAccount = "#HASH_BankAccount";
        const string HASH_UserVariableSymbol = "#HASH_UserVariableSymbol";
        const string HASH_ReservationEndTime = "#HASH_ReservationEndTime";
        const string HASH_PhotoAlbumUrl = "#HASH_PhotoAlbumUrl";
        const string HASH_SummaryUrl = "#HASH_SummaryUrl";

        #region Footers and images
        string Footer = string.Format("<hr /><table align=\"center\" width=\"100%\">"+
                        "<tr><td align=\"center\">"+
                        "Toto je automaticky generovaný email. Neodpovídejte na něj prosím.<br />"+
                        "Email byl vygenerovaný automatickou službou ze stránek <a href=\"{0}\">{0}</a>"+
                        "</td></tr></table>", Properties.Settings.Default.WebUrl);

        string LogoImage = "<img style='height:100px' src=\"data:image/png;base64,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\" />";
        #endregion

        public void SendEmail(EmailType type, object data, params string[] addresses)
        {
            if (!Properties.Settings.Default.SendEmails)
                return;

            var message = CreateEmail(type, data, addresses.Distinct().ToArray());
            SendEmail(message);
        }

        protected virtual void SendEmail(MailMessage message)
        {
        }

        private string GetEmailTemplate(EmailType emailType)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/EmailTemplates.xml");
            if (path == null)
                return string.Empty;

            XDocument document = XDocument.Load(path);
            var element = document.Descendants("template").SingleOrDefault(e => e.FirstAttribute.Value == emailType.ToString());
            return (element == null) ? string.Empty : element.Value;
        }

        private MailMessage CreateEmail(EmailType type, object data, params string[] addresses)
        {
            var template = GetEmailTemplate(type);
            var message = new MailMessage();
            switch(type)
            {
                case EmailType.NewEvent:
                    NewEventEmail(message, template, data, addresses);
                    break;
                case EmailType.ConfirmNewUser:
                    ConfirmNewUserEmail(message, template, data, addresses);
                    break;
                case EmailType.ForgotPassword:
                    ForgotPasswordEmail(message, template, data, addresses);
                    break;
                case EmailType.NewMessage:
                    NewMessageEmail(message, template, data, addresses);
                    break;
                case EmailType.FreePlaceOnEvent:
                    FreePlaceOnEventEmail(message, template, data, addresses);
                    break;
                case EmailType.PaymentInstructions:
                    PaymentInstructions(message, template, data, addresses);
                    break;
                case EmailType.InfoAboutBreakReservation:
                    BreakReservation(message, template, data, addresses);
                    break;
                case EmailType.EventMayBeNotice:
                case EmailType.EventYesNotice:
                    MayBeEventNotice(message, template, data, addresses);
                    break;
                case EmailType.NewPhotoAlbum:
                    NewPhotoAlbumEmail(message, template, data, addresses);
                    break;
                case EmailType.NewEventSummary:
                    NewEventSummaryEmail(message, template, data, addresses);
                    break;
                case EmailType.ThanksForFirstEvent:
                    ThanksForFirstEvent(message, template, addresses);
                    break;
                case EmailType.NewSuggestedEvent:
                    NewSuggestedEventEmail(message, template, data, addresses);
                    break; 
            }
            message.From = new MailAddress(Properties.Settings.Default.EmailAddress);
            message.Body = message.Body.Replace(HASH_Footer, Footer);
            message.Body = message.Body.Replace(HASH_Logo, LogoImage);
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;

            return message;
        }

        private void NewEventEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            NewEventEmailModel evt = data as NewEventEmailModel;
            if (evt != null)
            {
                message.Subject = string.Format("Nová akce - {0}", evt.Name);

                template = template.Replace(HASH_PageUrl, Properties.Settings.Default.WebUrl);
                template = template.Replace(HASH_EventName, evt.Name);
                template = template.Replace(HASH_EventPerex, evt.Perex);
                template = template.Replace(HASH_EventStartTime, evt.StartTime.ToString("ddd dd.MM.yyyy HH:mm"));
                template = template.Replace(HASH_EventStopTime, evt.StopTime.ToString("ddd dd.MM.yyyy HH:mm"));
                template = template.Replace(HASH_EventCapacity,
                    (evt.Capacity == 0) ? "Neomezeno" : evt.Capacity.ToString());
                template = template.Replace(HASH_EventPrice, (evt.Price == 0) ? "Zdarma" : evt.Price.ToString());
                template = template.Replace(HASH_EventPlace, evt.Place);
                template = template.Replace(HASH_EventMeetTime, evt.MeetTime.ToString("ddd dd.MM.yyyy HH:mm"));
                template = template.Replace(HASH_EventMeetPlace, evt.MeetPlace);
                template = template.Replace(HASH_Organisators, evt.Organisators);
                template = template.Replace(HASH_EventUrl, string.Format("{0}/akce/detail/{1}", Properties.Settings.Default.WebUrl, evt.HtmlName));
            }

            message.Body = template;
            message.IsBodyHtml = true;
            message.To.Add(Properties.Settings.Default.EmailAddress);
            foreach(var cc in addresses)
                message.Bcc.Add(cc);
        }

        private void ConfirmNewUserEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            ConfirmUrlUserInformation model = data as ConfirmUrlUserInformation;
            message.Subject = "Potvrzení registrace";
            if (model != null)
            {
                template = template.Replace(HASH_RegistrationUrl, model.ConfirmUrl);
                template = template.Replace(HASH_Email, model.Email);
                template = template.Replace(HASH_UserName, model.UserName);
            }

            message.Body = template;
            message.IsBodyHtml = true;
            message.To.Add(addresses[0]);
        }

        private void ForgotPasswordEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            ConfirmUrlUserInformation model = data as ConfirmUrlUserInformation;
            message.Subject = "Reset zapomenutého hesla";
            if (model != null)
            {
                template = template.Replace(HASH_Email, model.Email);
                template = template.Replace(HASH_UserName, model.UserName);
                message.Body = template.Replace(HASH_CallbackUrl, model.ConfirmUrl);
            }

            message.IsBodyHtml = true;
            message.To.Add(addresses[0]);
        }

        private void NewMessageEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            NewMessageMailModel model = data as NewMessageMailModel;
            if (model != null)
            {
                message.Subject = string.Format("Nová zpráva - {0}", model.Subject);
                template = template.Replace(HASH_MessageSender, model.Sender);
                template = template.Replace(HASH_MessageSubject, model.Subject);
                message.Body = template.Replace(HASH_MessageText, model.Text);
            }

            message.IsBodyHtml = true;

            if (addresses.Length > 1)
            {
                message.To.Add(Properties.Settings.Default.EmailAddress);
                foreach (var bcc in addresses)
                    message.Bcc.Add(bcc);
            }
            else
                message.To.Add(addresses[0]);
        }

        private void FreePlaceOnEventEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            NewReservation model = data as NewReservation;
            message.Subject = "Volné místo na akci";
            
            if (model != null)
            {
                template = template.Replace(HASH_EventName, model.Name);
                template = template.Replace(HASH_EventUrl, string.Format("{0}/akce/detail/{1}", Properties.Settings.Default.WebUrl, model.HtmlName));
                message.Body = template.Replace(HASH_ReservationEndTime, model.EndReservationTime.ToString("dd.MM.yyyy HH:mm"));
            }

            message.IsBodyHtml = true;
            message.To.Add(addresses[0]);
        }

        private void PaymentInstructions(MailMessage message, string template, object data, params string[] addresses)
        {
            if (data is PaymentInstruction model)
            {
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        // https://api.paylibo.com/paylibo/generator/czech/image?accountNumber=222885&bankCode=5500&amount=250.00&currency=CZK&vs=333&message=FOND%20HUMANITY%20CCK
                        string url =
                            string.Format(
                                "https://api.paylibo.com/paylibo/generator/czech/image?accountNumber={0}&bankCode={1}&amount={2}&currency=CZK&vs={3}",
                                "2800132182",
                                "2010",
                                model.Price,
                                model.VariableSymbol);

                        var bytes = wc.DownloadData(new Uri(url));
                        var base64Text = Convert.ToBase64String(bytes);

                        template = template.Replace(HASH_QRCode, base64Text);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine("_3F.Model.Email.BaseEmailSender.PaymentInstructions");
                }
                finally
                {
                    template = template.Replace(HASH_EventName, model.Name);
                    template = template.Replace(HASH_EventPrice, model.Price.ToString());
                    template = template.Replace(HASH_BankAccount, "2800132182/2010");
                    template = template.Replace(HASH_UserVariableSymbol, model.VariableSymbol.ToString());
                    template = template.Replace(HASH_ReservationEndTime, model.EndReservationTime.ToString("dd.MM.yyyy HH:mm"));
                }

                message.Subject = "Nedostatek peněz na účtu";
                message.Body = template;
            }

            message.IsBodyHtml = true;
            message.To.Add(addresses[0]);
        }

        private void BreakReservation(MailMessage message, string template, object data, params string[] addresses)
        {
            NewReservation model = data as NewReservation;
            message.Subject = "Rezervace vypršela";

            if (model != null)
            {
                template = template.Replace(HASH_EventName, model.Name);
                template = template.Replace(HASH_EventUrl, string.Format("{0}/akce/detail/{1}", Properties.Settings.Default.WebUrl, model.HtmlName));
                message.Body = template;
            }

            message.IsBodyHtml = true;
            message.To.Add(addresses[0]);
        }

        private void MayBeEventNotice(MailMessage message, string template, object data, params string[] addresses)
        {
            EvenWithtParticipantsEmailModel model = data as EvenWithtParticipantsEmailModel;
            if (model != null)
            {
                message.Subject = string.Format("Upozornění na akci - {0}", model.Name);
                template = template.Replace(HASH_EventName, model.Name);
                template = template.Replace(HASH_EventPerex, model.Perex);
                message.Body = template.Replace(HASH_EventUrl, string.Format("{0}/akce/detail/{1}", Properties.Settings.Default.WebUrl, model.HtmlName));
            }

            message.IsBodyHtml = true;

            if (addresses.Length > 1)
            {
                message.To.Add(Properties.Settings.Default.EmailAddress);
                foreach (var bcc in addresses)
                    message.Bcc.Add(bcc);
            }
            else
            {
                if (addresses.Length > 0)
                    message.To.Add(addresses[0]);
            }
        }

        private void NewPhotoAlbumEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            PhotoAlbum album = data as PhotoAlbum;
            if (album != null)
            {
                message.Subject = "Nové album " + album.Event.Name;
                template = template.Replace(HASH_EventName, album.Event.Name);
                template = template.Replace(HASH_PhotoAlbumUrl, string.Format("{0}/Fotky/Album/{1}", Properties.Settings.Default.WebUrl, album.Id));
            }

            message.IsBodyHtml = true;
            message.Body = template;
            message.To.Add(Properties.Settings.Default.EmailAddress);
            foreach (var cc in addresses)
                message.Bcc.Add(cc);
        }

        private void NewEventSummaryEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            EventSummary summary = data as EventSummary;
            if (summary != null)
            {
                message.Subject = "Nové zápisky u akce " + summary.Event.Name;
                template = template.Replace(HASH_EventName, summary.Event.Name);
                template = template.Replace(HASH_SummaryUrl, string.Format("{0}/Zapis/Detail/{1}", Properties.Settings.Default.WebUrl, summary.Event.HtmlName));
            }

            message.IsBodyHtml = true;
            message.Body = template;
            message.To.Add(Properties.Settings.Default.EmailAddress);
            foreach (var cc in addresses)
                message.Bcc.Add(cc);
        }

        private void ThanksForFirstEvent(MailMessage message, string template, params string[] addresses)
        {
            message.Subject = "Děkujeme za účast";
            message.IsBodyHtml = true;
            message.Body = template;
            message.To.Add(addresses[0]);
        }

        private void NewSuggestedEventEmail(MailMessage message, string template, object data, params string[] addresses)
        {
            NewEventEmailModel evt = data as NewEventEmailModel;
            if (evt != null)
            {
                message.Subject = string.Format("Nová nápad na akci - {0}", evt.Name);

                template = template.Replace(HASH_PageUrl, Properties.Settings.Default.WebUrl);
                template = template.Replace(HASH_EventName, evt.Name);
                template = template.Replace(HASH_EventPerex, evt.Perex);
                template = template.Replace(HASH_EventStartTime, evt.StartTime.ToString("ddd dd.MM.yyyy HH:mm"));
                template = template.Replace(HASH_EventStopTime, evt.StopTime.ToString("ddd dd.MM.yyyy HH:mm"));
                template = template.Replace(HASH_EventCapacity,
                    (evt.Capacity == 0) ? "Neomezeno" : evt.Capacity.ToString());
                template = template.Replace(HASH_EventPrice, (evt.Price == 0) ? "Zdarma" : evt.Price.ToString());
                template = template.Replace(HASH_EventPlace, evt.Place);
                template = template.Replace(HASH_EventUrl, string.Format("{0}/akce/detail/{1}", Properties.Settings.Default.WebUrl, evt.HtmlName));
            }

            message.Body = template;
            message.IsBodyHtml = true;
            message.To.Add(Properties.Settings.Default.EmailAddress);
            foreach (var cc in addresses)
                message.Bcc.Add(cc);
        }
    }
}

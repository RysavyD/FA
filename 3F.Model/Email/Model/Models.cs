using System;

namespace _3F.Model.Email.Model
{
    public class BaseEmailModel
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class NewEventEmailModel 
    {
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string Perex { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public int Capacity { get; set; }
        public int Price { get; set; }
        public string Place { get; set; }
        public DateTime MeetTime { get; set; }
        public string MeetPlace { get; set; }
        public string Organisators { get; set; }
    }

    public class ConfirmUrlUserInformation 
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ConfirmUrl { get; set; }
    }

    public class NewMessageMailModel 
    {
        public string Subject { get; set; }
        public string Sender { get; set; }
        public string Text { get; set; }
    }

    public class NewReservation 
    {
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public DateTime EndReservationTime { get; set; }
    }

    public class PaymentInstruction 
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string BankAccount { get; set; }
        public int VariableSymbol { get; set; }
        public DateTime EndReservationTime { get; set; }
    }


    public class EvenWithtParticipantsEmailModel : BaseEmailModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string Perex { get; set; }
    }
}

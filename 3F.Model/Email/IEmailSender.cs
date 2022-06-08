namespace _3F.Model.Email
{
    public interface IEmailSender
    {
        void SendEmail(EmailType type, object data, params string[] addresses);
    }

    public enum EmailType
    {
        NewEvent,
        ConfirmNewUser,
        ForgotPassword,
        NewMessage,
        FreePlaceOnEvent,
        PaymentInstructions,
        InfoAboutBreakReservation,
        EventMayBeNotice,
        EventYesNotice,
        InfoFromAdmin,
        NewPhotoAlbum,
        NewEventSummary,
        ThanksForFirstEvent,
        NewSuggestedEvent,
    }
}

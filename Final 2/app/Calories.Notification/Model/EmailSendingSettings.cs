namespace Calories.Notification.Model
{
    public class EmailSendingSettings
    {
        public string ApiKey { get; set; }
        public string ConfirmationFrom { get; set; }
        public string InvitationFrom { get; set; }
        public string ConfirmationSubject { get; set; }
        public string InvitationSubject { get; set; }
        public string ConfirmationContent { get; set; }
        
        public string InviteUserHtmlTemplate { get; set; }
    }
}
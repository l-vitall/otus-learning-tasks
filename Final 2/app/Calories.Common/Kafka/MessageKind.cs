namespace Calories.Common.Kafka
{
    public class MessageKind
    {
        public const string UserCreated = "user-created";
        public const string NotificationCreated = "notification-created";
        //public const string EmailConfirmed = "notification-email-confirmed";
        public const string InvitationEmailRequest = "invitation-email";
        public const string ConfirmationEmailRequest = "confirmation-email";
        public const string OrderStatusUpdate = "order-status-update";
    }
}
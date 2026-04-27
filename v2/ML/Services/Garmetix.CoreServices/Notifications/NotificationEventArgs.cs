namespace Garmetix.Services.Notifications
{
    public class NotificationEventArgs : EventArgs
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
    }
}

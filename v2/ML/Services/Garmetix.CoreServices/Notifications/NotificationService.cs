using Bharat.ToolKits.Helpers;
using Garmetix.Data.Databases;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
//using Plugin.LocalNotification;


namespace Garmetix.Services.Notifications
{
    public partial class NotificationService
    {
        private static readonly bool SpeakerIsOn = StorageOps.GetPref("SpeakerIsOn", false);

        private static readonly INotificationManagerService notificationManager =
            Application.Current?.Windows[0]?.Page?.Handler?.MauiContext?.Services?.GetService<INotificationManagerService>()!;

        private static ApplicationDatabaseContext Database => DatabaseService.Instance.ApplicationDB;

        public static bool EnableLocalNotifications { get; set; } = false;

        /// <summary>
        /// Returns a list of unread notifications
        /// </summary>
        /// <returns></returns>
        public static Task<List<Notification>> GetUnreadNotifications() => Database.Notifications.Where(n => !n.IsRead).ToListAsync();

        /// <summary>
        /// Adds a notification
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Task AddNotification(string title, string message)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Timestamp = DateTime.Now,
                IsRead = false
            };
            Database.Notifications.Add(notification);
            return Database.SaveChangesAsync();
        }

        /// <summary>
        /// Marks the notification with the specified ID as read.
        /// </summary>
        /// <remarks>If no notification with the specified ID exists, no changes are made.</remarks>
        /// <param name="id">The unique identifier of the notification to mark as read.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when the changes are saved to the
        /// database.</returns>
        public static Task MarkAsRead(int id)
        {
            var notification = Database.Notifications.Where(c => c.Id == id).FirstOrDefault();
            if (notification != null)
            {
                notification.IsRead = true;
                Database.Notifications.Update(notification);
            }
            return Database.SaveChangesAsync();
        }

        /// <summary>
        /// Clears all notifications
        /// </summary>
        /// <returns></returns>
        public static Task ClearNotifications()
        {
            Database.Notifications.RemoveRange(Database.Notifications);
            return Database.SaveChangesAsync();
        }

        /// <summary>
        /// Sends a local notification
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public static void LocalNotification(string title, string msg) => notificationManager.SendNotification(title, msg, DateTime.Now.AddSeconds(10));

        /// <summary>
        /// Sends a local notification with a delay
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="notifyTime"></param>
        public static void LocalNotification(string title, string msg, DateTime notifyTime) => notificationManager.SendNotification(title, msg, notifyTime.AddSeconds(10));

        /// <summary>
        /// Sends a local notification  using the plugin.Notification Package
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void LocalPluginNotification(string title, string message)
        {
            if (EnableLocalNotifications == false) return;
            // Show the notification
            LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = 1001,
                Title = "Garmtix: " + title,
                Description = message,
                ReturningData = "TaskCompleted",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5) // Delayed notification
                }
            });
        }

        /// <summary>
        /// Sends a local notification with a handler for when the notification is received
        /// Not Implemented
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public static void LocalNotificationWithHandler(string title, string msg)
        {

            if (EnableLocalNotifications == false) return;
            notificationManager.SendNotification(title, msg, DateTime.Now);
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var eventData = (NotificationEventArgs)eventArgs;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Take required action in the app once the notification has been received.
                });
            };
        }
    }
}
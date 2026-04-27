namespace Garmetix.Services
{
    internal class MailService
    {
        public static async void SendTestMail()
        {
            if (Email.Default.IsComposeSupported)
            {

                string subject = "Hello friends!";
                string body = "It was great to see you last weekend.";
                string[] recipients = new[] { "john@contoso.com", "jane@contoso.com" };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = [.. recipients]
                };

                await Email.Default.ComposeAsync(message);
            }
        }
        public static async Task SendMailAsync(string subject, string body, List<string> recipients)
        {
            if (Email.Default.IsComposeSupported)
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = recipients
                };
                await Email.Default.ComposeAsync(message);
            }
        }
        public static async void TestMail()
        {
            if (Email.Default.IsComposeSupported)
            {

                string subject = "Hello friends!";
                string body = "It was great to see you last weekend. I've attached a photo of our adventures together.";
                string[] recipients = new[] { "john@contoso.com", "jane@contoso.com" };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = [.. recipients]
                };

                string picturePath = Path.Combine(FileSystem.CacheDirectory, "memories.jpg");

                message.Attachments.Add(new EmailAttachment(picturePath));

                await Email.Default.ComposeAsync(message);
            }
        }
        public async Task SendMailWithAttachmentAsync(string subject, string body, List<string> recipients, string attachmentPath)
        {
            if (Email.Default.IsComposeSupported)
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = recipients
                };
                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                {
                    message.Attachments.Add(new EmailAttachment(attachmentPath));
                }
                await Email.Default.ComposeAsync(message);
            }
        }
    }
}

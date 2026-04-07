

using Bharat.ToolKits.Helpers;

namespace Garmetix.ModuleService
{
    // All the code in this file is included in all platforms.
    public class ShareService
    {
        /// <summary>
        /// Share over email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<bool> ShareOverEmail(string email, string subject, string body, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                await Shell.Current.DisplayAlert("Input Needed", "Please enter an email address to share.", "OK");
                return false;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = [email]
                };

                message?.Attachments?.Add(new EmailAttachment(filePath));
                ServiceHelper.Current.GetService<IEmail>()?.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException)
            {
                await Shell.Current.DisplayAlert("Not Supported", "Email is not supported on this device.", "OK");
                return false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to send email: {ex.Message}", "OK");
                return false;
            }
        }
        /// <summary>
        /// ShareIt file the 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static async Task<bool> ShareIt(string subject, string filepath)
        {
            if (string.IsNullOrWhiteSpace(subject)) subject = "Sharing File";
            if (string.IsNullOrWhiteSpace(filepath)) return false;
            try
            {
                // Use the IShare interface to request sharing the file.
                // This will open the native OS share sheet.
                var _share = ServiceHelper.Current.GetService<IShare>();
                await _share!.RequestAsync(new ShareFileRequest
                {
                    Title = subject,
                    File = new ShareFile( filepath)
                });
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not share file: {ex.Message}", "OK");
                return false;
            }
        }


        /// <summary>
        /// Share or open file as pdf 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<bool> OpenAsPdf(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath) && !filePath.EndsWith(".pdf")) return false;
           return await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }

        public static Task<bool> ShareOverWhatsapp(string phonenumber, string subject, string filePath)
        {
            throw new NotImplementedException();
        }


        public static string GetBasePath()
        {
            return Path.Combine(FileSystem.CacheDirectory, "Bharat-Garmetix", Preferences.Get("CompanyName", "AadwikaFashion").Replace(" ", "_"));

        }
    }
}

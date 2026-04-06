namespace Bharat.ToolKits.Helpers
{
    public static class ShareHelper
    {
        //TODO: https://learn.microsoft.com/en-us/dotnet/architecture/maui/communicating-between-components?toc=%2Fdotnet%2Fmaui%2Ftoc.json&bc=%2Fdotnet%2Fmaui%2Fbreadcrumb%2Ftoc.json&view=net-maui-9.0
        //TODO:https://learn.microsoft.com/en-us/dotnet/architecture/maui/validation?toc=%2Fdotnet%2Fmaui%2Ftoc.json&bc=%2Fdotnet%2Fmaui%2Fbreadcrumb%2Ftoc.json&view=net-maui-9.0

        public static async Task<ImageSource?> TakeScreenshotAsync()
        {
            if (Screenshot.Default.IsCaptureSupported)
            {
                IScreenshotResult screen = await Screenshot.Default.CaptureAsync();

                Stream stream = await screen.OpenReadAsync();

                return ImageSource.FromStream(() => stream);
            }

            return null;
        }

        public static async Task ShareText(string title, string text)
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = title
            });
        }

        //TODO:https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/data/share?view=net-maui-9.0&tabs=android
        public static async Task ShareMultipleFiles(string title, List<string> files)
        {
            string file1 = Path.Combine(FileSystem.CacheDirectory, "Attachment1.txt");
            string file2 = Path.Combine(FileSystem.CacheDirectory, "Attachment2.txt");

            File.WriteAllText(file1, "Content 1");
            File.WriteAllText(file2, "Content 2");
            List<ShareFile> shareFiles = new List<ShareFile>();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    shareFiles.Add(new ShareFile(file));
                }
            }

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = title,
                Files = shareFiles
            });
        }

        public static async Task ShareFile(string title, string file)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(file)
            });
        }

        public static async Task ShareUri(string uri, IShare share)
        {
            await share.RequestAsync(new ShareTextRequest
            {
                Uri = uri,
                Title = "Share Web Link"
            });
        }
    }
}
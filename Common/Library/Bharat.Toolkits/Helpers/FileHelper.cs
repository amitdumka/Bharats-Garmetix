namespace Bharat.ToolKits.Helpers
{
    public class FileHelper
    {

        /// <summary>
        /// Reads the file from raw resources.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<Stream> ReadFromRaw(string filename)
        {
            var stream =
               await FileSystem.OpenAppPackageFileAsync(filename);
            return stream;
        }

        /// <summary>
        /// Picks and shows File Picker
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<FileResult?> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();
                        var image = ImageSource.FromStream(() => stream);
                    }
                }

                return result;
            }
            catch (Exception)
            {
                // The user canceled or something went wrong
            }

            return null;
        }

        public FilePickerFileType customFileType = new(
                  new Dictionary<DevicePlatform, IEnumerable<string>>
                  {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "application/comics" } },
                    { DevicePlatform.WinUI, new[] { ".cbr", ".cbz" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // or general UTType values
                  });

        public PickOptions options = new()
        {
            PickerTitle = "Please select a comic file",
            FileTypes = new FilePickerFileType(
                 new Dictionary<DevicePlatform, IEnumerable<string>>
                 {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "application/comics" } },
                    { DevicePlatform.WinUI, new[] { ".cbr", ".cbz" } },
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // or general UTType values
                 }),
        };

        /// <summary>
        /// Reads the text file. from raw resources.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<string> ReadTextFile(string filePath)
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(filePath);
            using StreamReader reader = new(fileStream);

            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// Converts the file to upper case.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="targetFileName"></param>
        /// <returns></returns>
        public static async Task ConvertFileToUpperCase(string sourceFile, string targetFileName)
        {
            // Read the source file
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(sourceFile);
            using StreamReader reader = new(fileStream);

            string content = await reader.ReadToEndAsync();

            // Transform file content to upper case text
            content = content.ToUpperInvariant();

            // Write the file content to the app data directory
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);

            using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
            using StreamWriter streamWriter = new(outputStream);

            await streamWriter.WriteAsync(content);
        }
    }
}
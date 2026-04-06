namespace Bharat.ToolKits.Files
{
    public class Files
    {
        public static async Task<string> ReadTextFile(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public static async Task WriteTextFile(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }
    }
}
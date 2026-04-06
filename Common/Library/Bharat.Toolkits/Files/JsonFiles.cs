namespace Bharat.ToolKits.Files
{
    public class JsonFiles
    {
        public static async Task<T?> ReadJsonFile<T>(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static async Task WriteJsonFile<T>(string path, T content)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(content);
            await File.WriteAllTextAsync(path, json);
        }

        public static async Task<List<T>?> ReadJsonFileToList<T>(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return System.Text.Json.JsonSerializer.Deserialize<List<T>>(json);
        }

        public static async Task WriteJsonFile<T>(string path, List<T> content)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(content);
            await File.WriteAllTextAsync(path, json);
        }
    }
}
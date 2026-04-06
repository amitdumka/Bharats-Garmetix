using System.Text.Json;

namespace Garmetix.Core.DataModels
{
    //TODO: Move to Toolkit or DataService 
    internal static class JsonHelper
    {
        public static string ToJson(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static void AppendToJsonFile<T>(string fileName, T data)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "DatabaseActivity", fileName);

            List<T> dataList;
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                dataList = JsonSerializer.Deserialize<List<T>>(jsonString) ?? [];
            }
            else
            {
                dataList = [];
            }

            dataList.Add(data);

            string updatedJsonString = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonString);
        }

        public static void AppendToJsonFile<T>(string fileName, List<T> data)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "DatabaseActivity", fileName);

            List<T> dataList;
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                dataList = JsonSerializer.Deserialize<List<T>>(jsonString) ?? [];
            }
            else
            {
                dataList = [];
            }

            dataList.AddRange(data);

            string updatedJsonString = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonString);
        }

        /// <summary>
        /// Update Database Activity to json file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void SaveOrUpdateJsonFile<T>(string fileName, T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data cannot be null.");
            }

            string filePath = Path.Combine(FileSystem.AppDataDirectory, "DatabaseActivity", fileName);

            List<Dictionary<string, object>> dataList;
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                dataList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString) ?? [];
            }
            else
            {
                dataList = [];
            }

            Dictionary<string, object> newData = new() { { "TableName", data.GetType().Name }, { "Data", data } };
            dataList.Add(newData);

            string updatedJsonString = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonString);
        }

        public static void SaveOrUpdateJsonFile<T>(string fileName, List<T> data)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "DatabaseActivity", fileName);

            List<Dictionary<string, object>> dataList;
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                dataList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString) ?? [];
            }
            else
            {
                dataList = [];
            }

            Dictionary<string, object> newData = new()
            {
            { "TableName", data.GetType().Name },
            { "Data", data }
        };

            dataList.Add(newData);

            string updatedJsonString = JsonSerializer.Serialize(dataList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonString);
        }

        /// <summary>
        /// Load Database Activty from JSON file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<T> LoadFromJsonFileAsList<T>(string fileName)
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "DatabaseActivity", fileName);
            if (!File.Exists(filePath))
            {
                return [];
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(jsonString) ?? [];
        }
    }
}
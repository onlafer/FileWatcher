using System.Text.Json;

namespace FileWatcher.Utilities
{
    class JsonHelper
    {
        public static string GetJsonString(string JsonFilePath)
        {
            if (!File.Exists(JsonFilePath))
            {
                Logger.Log(LogLevel.Error, text: $"Передан неверный путь к json файлу. Переданный путь: '{JsonFilePath}'");
                throw new FileNotFoundException();
            }

            try
            {
                string json = File.ReadAllText(JsonFilePath);
                return json;
            }
            catch (Exception)
            {
                Logger.Log(LogLevel.Error, text: $"Ошибка при чтении json файла. Переданный путь: '{JsonFilePath}'");
                throw;
            }
        }

        public static void WriteJsonString(string filePath, string json)
        {
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception)
            {
                Logger.Log(LogLevel.Error, text: $"Ошибка при записи json файла. Переданный путь: '{filePath}'");
                throw;
            }
        }

        // public static Dictionary<string, string> GetDictionary(string JsonFilePath)
        // {
        //     string json = GetJsonString(JsonFilePath);
        //     try
        //     {
        //         Dictionary<string, string> dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        //         return dictionary;
        //     }
        //     catch (Exception)
        //     {
        //         Logger.Log(LogLevel.Error, text: $"Ошибка при десериализации json файла. Переданный путь: '{JsonFilePath}'");
        //         throw;
        //     }
        // }
    }
}

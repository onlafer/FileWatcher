using System.Text.Json;

namespace FileWatcher.Utilities
{
    class JsonHelper
    {
        public static string GetJsonString(string JsonFilePath)
        {
            if (!File.Exists(JsonFilePath))
            {
                Logger.Log(LogLevel.FATAL, text: $"Передан неверный путь к json файлу. Переданный путь: '{JsonFilePath}'");
                throw new FileNotFoundException();
            }

            try
            {
                return File.ReadAllText(JsonFilePath);
            }
            catch (Exception)
            {
                Logger.Log(LogLevel.FATAL, text: $"Ошибка при чтении json файла. Переданный путь: '{JsonFilePath}'");
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
                Logger.Log(LogLevel.FATAL, text: $"Ошибка при записи json файла. Переданный путь: '{filePath}'");
                throw;
            }
        }

        public static T? ReadConfig<T>(string filePath) where T : class
        {
            try
            {
                string jsonString = JsonHelper.GetJsonString(filePath);
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<T>(jsonString, options);
            }
            catch (Exception)
            {
                Logger.Log(LogLevel.FATAL, text: $"Ошибка при сериализации json файла. Переданный путь: '{filePath}'");
                throw;
            }
        }
    }
}

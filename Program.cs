using FileWatcher.Utilities;
using FileWatcher.Config;
using System.Text.Json;

namespace FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Logger.
            Logger logger = new Logger("log.log");
            // Config config = JsonSerializer.Deserialize<Config>(JsonHelper.GetJsonString("Config\\config.json"));

            logger.Log(LogLevel.Warning, text: "Начало работы программы");
            logger.Log(LogLevel.Info, text: "Конец работы программы");
        }
    }
}
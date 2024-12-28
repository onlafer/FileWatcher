using FileWatcher.Config;
using FileWatcher.Services;
using FileWatcher.Utilities;

public class Program
{
    public static void Main(string[] args)
    {
        Logger logger = new Logger("Logs\\log.txt");
        ConfigManager? config = JsonHelper.ReadConfig<ConfigManager>("Config\\config.json");

        if (config != null)
        {
            logger.Log(new LogMessage(text: "Начало наблюдения за директориями."));
            FileWatcherService service = new FileWatcherService(config, logger);
            service.StartWatching();

            Console.WriteLine("Нажмите Enter для завершения наблюдения...");
            Console.ReadLine();

            service.StopWatching();
            logger.Log(new LogMessage(text: "Конец наблюдения за директориями."));
        }
        else
        {
            logger.Log(new LogMessage(LogLevel.FATAL, "Не удалось загрузить конфигурацию."));
        }
    }
}
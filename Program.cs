using System.Runtime.CompilerServices;
using FileWatcher.Config;
using FileWatcher.Services;
using FileWatcher.Utilities;

public class Program
{
    public static void Main(string[] args)
    {
        Logger logger = new Logger("Logs\\log.txt");
        ConfigManager? config = JsonHelper.ReadConfig<ConfigManager>("Config\\config.json");

        // logger.Log(new FatalLevel(), "Не удалось загрузить конфигурацию.");

        if (config != null)
        {
            logger.Log(new InfoLevel(), text: "Начало наблюдения за директориями.");
            FileWatcherService service = new FileWatcherService(config, logger);
            service.StartWatching();

            Console.WriteLine("Нажмите Enter для завершения наблюдения...");
            Console.ReadLine();

            service.StopWatching();
            logger.Log(new InfoLevel(), text: "Конец наблюдения за директориями.");
        }
        else
        {
            logger.Log(new FatalLevel(), "Не удалось загрузить конфигурацию.");
        }
    }
}
using System.Runtime.CompilerServices;

namespace FileWatcher.Utilities
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public class Logger
    {
        private ConsoleColor _originalConsoleColor = Console.ForegroundColor;

        public string LogFilePath { get; set; }
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
        public Dictionary<LogLevel, ConsoleColor> LogLevelColors { get; set; } = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.Debug, ConsoleColor.Cyan },
            { LogLevel.Info, ConsoleColor.Green },
            { LogLevel.Warning, ConsoleColor.Yellow },
            { LogLevel.Error, ConsoleColor.Red }
        };

        public Logger(string logFilePath = "")
        {
            LogFilePath = logFilePath;
        }

        public static void Log(
            LogLevel level = LogLevel.Info,
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0,
            string text = "",
            bool logToConsole = true,
            bool logToFile = false,
            string logFilePath = ""
        )
        {
            new Logger(logFilePath).Log(level, module, function, lineNumber, text, logToConsole, logToFile);
        }

        public void Log(
            LogLevel level = LogLevel.Info,
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0,
            string text = "",
            bool logToConsole = true,
            bool logToFile = true
        )
        {
            string datetime = DateTime.Now.ToString(DateTimeFormat);

            if (logToConsole)
            {
                LogToConsole(datetime, level, module, function, lineNumber, text);
            }

            if (logToFile)
            {
                LogToFile(datetime, level, module, function, lineNumber, text);
            }
        }

        private void LogToConsole(string datetime, LogLevel level, string module, string function, int lineNumber, string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(datetime);
            WriteSeparator();

            Console.ForegroundColor = LogLevelColors[level];
            Console.Write($"{level,-8}");
            WriteSeparator();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(module);
            WriteSeparator(":");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(function);
            WriteSeparator(":");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(lineNumber);
            WriteSeparator();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text);

            SetOriginalConsoleColor();
        }

        private void WriteSeparator(string sep = " | ")
        {
            SetOriginalConsoleColor();
            Console.Write(sep);
        }

        private void SetOriginalConsoleColor()
        {
            Console.ForegroundColor = _originalConsoleColor;
        }

        private void LogToFile(string datetime, LogLevel level, string module, string function, int lineNumber, string text)
        {
            if (!File.Exists(LogFilePath))
            {
                Logger.Log(LogLevel.Error, text: $"Передан неверный путь к лог файлу. Переданный путь: '{LogFilePath}'");
                throw new FileNotFoundException();
            }

            string logEntry = $"{datetime} | {level,-8} | {module}:{function}:{lineNumber} | {text}";
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(logEntry);
            }
        }
    }
}
using System.Runtime.CompilerServices;

namespace FileWatcher.Utilities
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        SUCCESS,
        WARNING,
        ERROR,
        FATAL
    }

    public class LogMessage
    {
        public LogLevel Level { get; }
        public string Text { get; }
        public string Module { get; }
        public string Function { get; }
        public int LineNumber { get; }

        public LogMessage(
            LogLevel level = LogLevel.INFO,
            string text = "",
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0
        )
        {
            Level = level;
            Text = text;
            Module = module;
            Function = function;
            LineNumber = lineNumber;
        }
    }

    public class Logger
    {
        private ConsoleColor _originalConsoleColor = Console.ForegroundColor;

        public string LogFilePath { get; set; }
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";
        public Dictionary<LogLevel, ConsoleColor> LogLevelColors { get; set; } = new Dictionary<LogLevel, ConsoleColor>
        {
            { LogLevel.DEBUG, ConsoleColor.White },
            { LogLevel.INFO, ConsoleColor.Green },
            { LogLevel.WARNING, ConsoleColor.Yellow },
            { LogLevel.ERROR, ConsoleColor.Red },
            { LogLevel.FATAL, ConsoleColor.DarkRed }
        };

        public Logger(string logFilePath = "")
        {
            LogFilePath = logFilePath;
        }

        public static void Log(
            LogLevel level = LogLevel.INFO,
            string text = "",
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0,
            bool logToConsole = true,
            bool logToFile = false,
            string logFilePath = ""
        )
        {
            new Logger(logFilePath).Log(new LogMessage(level, text, module, function, lineNumber), logToConsole, logToFile);
        }

        public void Log(
            LogMessage message,
            bool logToConsole = true,
            bool logToFile = true
        )
        {
            string datetime = DateTime.Now.ToString(DateTimeFormat);

            if (logToConsole)
            {
                LogToConsole(datetime, message);
            }

            if (logToFile)
            {
                LogToFile(datetime, message);
            }

            if (message.Level == LogLevel.FATAL)
            {
                throw new FatalLogException(message.Text);
            }
            if (message.Level == LogLevel.ERROR)
            {
                throw new ErrorLogException(message.Text);
            }
        }

        private void LogToConsole(string datetime, LogMessage message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(datetime);
            WriteSeparator();

            Console.ForegroundColor = LogLevelColors[message.Level];
            Console.Write($"{message.Level,-8}");
            WriteSeparator();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(message.Module);
            WriteSeparator(":");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(message.Function);
            WriteSeparator(":");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(message.LineNumber);
            WriteSeparator();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message.Text);

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

        private void LogToFile(string datetime, LogMessage message)
        {
            if (string.IsNullOrEmpty(LogFilePath))
            {
                Log(new LogMessage(LogLevel.WARNING, "Не передан путь к файлу лога. Логирование в файл не будет произведено"), logToConsole: true, logToFile: false);
                return;
            }

            if (!File.Exists(LogFilePath))
            {
                Log(new LogMessage(LogLevel.WARNING, $"Файл лога не найден. Создан новый файл по пути: {LogFilePath}"), logToConsole: true, logToFile: false);
                File.Create(LogFilePath).Close();
            }

            string logEntry = $"{datetime} | {message.Level,-8} | {message.Module}:{message.Function}:{message.LineNumber} | {message.Text}";
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(logEntry);
            }
        }
    }

    public class FatalLogException : Exception
    {
        public FatalLogException(string message) : base(message) { }
    }

    public class ErrorLogException : Exception
    {
        public ErrorLogException(string message) : base(message) { }
    }
}
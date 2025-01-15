using System.Runtime.CompilerServices;

namespace FileWatcher.Utilities
{
    public abstract class LogLevel
    {
        public abstract string Name { get; }
        public abstract ConsoleColor Color { get; }
        public virtual void HandleLog(string message, Exception? exception = null) { }
    }

    public class DebugLevel : LogLevel
    {
        public override string Name { get; } = "DEBUG";
        public override ConsoleColor Color { get; } = ConsoleColor.Cyan;
    }

    public class InfoLevel : LogLevel
    {
        public override string Name { get; } = "INFO";
        public override ConsoleColor Color { get; } = ConsoleColor.White;
    }

    public class SuccessLevel : LogLevel
    {
        public override string Name { get; } = "SUCCESS";
        public override ConsoleColor Color { get; } = ConsoleColor.Green;
    }

    public class WarningLevel : LogLevel
    {
        public override string Name { get; } = "WARNING";
        public override ConsoleColor Color { get; } = ConsoleColor.Yellow;
    }

    public class ErrorLevel : LogLevel
    {
        public override string Name { get; } = "ERROR";
        public override ConsoleColor Color { get; } = ConsoleColor.Red;
        public override void HandleLog(string message, Exception? exception = null)
        {
            if (exception == null)
            {
                throw new Exception($"Error: {message}");
            }

            throw new Exception($"Error: {message}", exception);
        }
    }

    public class FatalLevel : LogLevel
    {
        public override string Name { get; } = "FATAL";
        public override ConsoleColor Color { get; } = ConsoleColor.DarkRed;
        public override void HandleLog(string message, Exception? exception = null)
        {
            if (exception == null)
            {
                throw new Exception($"Fatal Error: {message}");
            }

            throw new Exception($"Fatal Error: {message}", exception);
        }
    }

    public class LogMessage
    {
        public DateTime DateTimeNow { get; }
        public LogLevel Level { get; }
        public string Text { get; }
        public string Module { get; }
        public string Function { get; }
        public int LineNumber { get; }
        public string Separator { get; set; }
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

        public LogMessage(LogLevel level, string text, string module, string function, int lineNumber, string separator)
        {
            DateTimeNow = DateTime.Now;
            Level = level;
            Text = text;
            Module = module;
            Function = function;
            LineNumber = lineNumber;
            Separator = separator;
        }

        public override string ToString()
        {
            return $"{DateTimeNow.ToString(DateTimeFormat)}{Separator}{Level.Name,-8}{Separator}{Module}:{Function}:{LineNumber}{Separator}{Text}";
        }
    }

    public class Logger
    {
        public string LogFilePath { get; set; }
        private ConsoleColor OriginalConsoleColor { get; }
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

        public Logger(string logFilePath = "")
        {
            LogFilePath = logFilePath;
            OriginalConsoleColor = Console.ForegroundColor;
        }

        public static void Log(
            LogLevel level,
            string text,
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0,
            bool logToConsole = true,
            bool logToFile = false,
            string logFilePath = "",
            bool enableHandler = true,
            string separator = " | ",
            Exception? exception = null
        )
        {
            new Logger(logFilePath).Log(level, text, module, function, lineNumber, logToConsole, logToFile, enableHandler, separator, exception);
        }

        public void Log(
            LogLevel level,
            string text,
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0,
            bool logToConsole = true,
            bool logToFile = true,
            bool enableHandler = true,
            string separator = " | ",
            Exception? exception = null
        )
        {
            var message = new LogMessage(level, text, module, function, lineNumber, separator);

            if (logToConsole)
            {
                LogToConsole(message);
            }

            if (logToFile)
            {
                LogToFile(message);
            }

            if (enableHandler)
            {
                level.HandleLog(text, exception);
            }
        }

        private void LogToConsole(LogMessage message)
        {
            const ConsoleColor DateTimeColor = ConsoleColor.Cyan;
            const ConsoleColor ModuleColor = ConsoleColor.Magenta;
            const ConsoleColor TextColor = ConsoleColor.Blue;

            Console.ForegroundColor = DateTimeColor;
            Console.Write(message.DateTimeNow.ToString(DateTimeFormat));
            WriteSeparator(message.Separator);

            Console.ForegroundColor = message.Level.Color;
            Console.Write($"{message.Level.Name,-8}");
            WriteSeparator(message.Separator);

            Console.ForegroundColor = ModuleColor;
            Console.Write($"{message.Module}");
            WriteSeparator(":");

            Console.ForegroundColor = ModuleColor;
            Console.Write($"{message.Function}");
            WriteSeparator(":");

            Console.ForegroundColor = ModuleColor;
            Console.Write($"{message.LineNumber}");
            WriteSeparator(message.Separator);

            Console.ForegroundColor = TextColor;
            Console.WriteLine(message.Text);

            Console.ForegroundColor = OriginalConsoleColor;
        }

        public void LogException(
            LogLevel level,
            Exception ex,
            [CallerFilePath] string module = "",
            [CallerMemberName] string function = "",
            [CallerLineNumber] int lineNumber = 0,
            bool rethrow = false
        )
        {
            this.Log(level, ex.Message, module: module, function: function, lineNumber: lineNumber, enableHandler: false, exception: ex);

            if (rethrow)
            {
                throw ex;
            }
        }

        private void WriteSeparator(string sep = " | ")
        {
            SetOriginalConsoleColor();
            Console.Write(sep);
        }

        private void SetOriginalConsoleColor()
        {
            Console.ForegroundColor = OriginalConsoleColor;
        }

        private void LogToFile(LogMessage message)
        {
            if (!File.Exists(LogFilePath))
            {
                Logger.Log(new FatalLevel(), text: $"Передан неверный путь к лог файлу. Переданный путь: '{LogFilePath}'");
            }

            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
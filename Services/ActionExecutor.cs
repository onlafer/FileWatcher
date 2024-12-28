using System.Diagnostics;
using FileWatcher.Config;
using FileWatcher.Utilities;

namespace FileWatcher.Services
{
    public class ActionExecutor
    {
        private readonly Logger _logger;

        public ActionExecutor(Logger logger)
        {
            _logger = logger;

        }

        public void Execute(FileAction action, string fullPath, string? newPath)
        {
            string newName = newPath != null ? Path.GetFileName(newPath) : "";

            Dictionary<string, string>placeHolders = new Dictionary<string, string>
            {
                { "%name", action.Name },
                { "%path", fullPath },
                { "%basename", Path.GetFileName(fullPath) },
                { "%newpath", newName },
                { "%newname", newPath ?? "" },
                { "%command", action.Command ?? "" },
                { "%args", action.Arguments ?? "" }
            };

            switch (action.Type)
            {
                case "log":
                    LogAction(action, placeHolders);
                    break;
                case "execute":
                    ExecuteCommand(action, placeHolders);
                    break;
                default:
                    _logger.Log(new WarningLevel(), $"Неизвестное действие: {action.Type}. Имя действия: {action.Name}");
                    break;
            }
        }

        public void LogAction(FileAction action, Dictionary<string, string> placeHolders)
        {
            if (string.IsNullOrEmpty(action.Message))
            {
                _logger.Log(new WarningLevel(), $"Передано пустое сообщение в action. Имя действия: {action.Name}");
                return;
            }

            string message = ReplacePlaceholders(action.Message, placeHolders);

            _logger.Log(new InfoLevel(), message);
        }

        public void ExecuteCommand(FileAction action, Dictionary<string, string> placeHolders)
        {
            if (string.IsNullOrEmpty(action.Command))
            {
                _logger.Log(new WarningLevel(), $"Если указан тип действия execute, обязательно должен присутствовать параметр command. Имя действия: {action.Name}");
                return;
            }
            
            string command = ReplacePlaceholders(action.Command, placeHolders);
            string arguments = ReplacePlaceholders(action.Arguments, placeHolders);


            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            try
            {
                if (action.Message != null)
                {
                    _logger.Log(new InfoLevel(), ReplacePlaceholders(action.Message, placeHolders));
                }
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                _logger.Log(new WarningLevel(), $"Неудалось запустить команду: '{command} {arguments}'. Имя действия: {action.Name}");
            }

        }

        private string ReplacePlaceholders(string message, Dictionary<string, string> placeHolders)
        {
            bool hasPlaceholders = true;

            while (hasPlaceholders)
            {
                hasPlaceholders = false;

                foreach (KeyValuePair<string, string> placeHolder in placeHolders)
                {
                    if (message.Contains(placeHolder.Key))
                    {
                        message = message.Replace(placeHolder.Key, placeHolder.Value);
                        hasPlaceholders = true;
                    }
                }
            }

            return message;
        }
    }
}
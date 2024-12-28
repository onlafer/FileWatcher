using FileWatcher.Config;
using FileWatcher.Utilities;
using System.Diagnostics;

namespace FileWatcher.Services
{
    public class FileWatcherService
    {
        private readonly ConfigManager _config;
        private readonly Logger _logger;
        private readonly ActionExecutor _actionExecutor;
        private readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();


        public FileWatcherService(ConfigManager config, Logger logger)
        {
            _config = config;
            _logger = logger;
            _actionExecutor = new ActionExecutor(logger);
        }

        public void StartWatching()
        {
            foreach (WatchedDirectory watchedDir in _config.WatchedDirectories)
            {
                StartWatcher(watchedDir);
            }
        }

        private void StartWatcher(WatchedDirectory watchedDir)
        {
            if (!Directory.Exists(watchedDir.Path))
            {
                _logger.Log(new WarningLevel(), text: $"Директория переданная в конфиг не существует. Переданный путь: {watchedDir.Path}");
                return;
            }
            string fullPath = watchedDir.Path;

            FileSystemWatcher watcher = new FileSystemWatcher(fullPath)
            {
                IncludeSubdirectories = watchedDir.IncludeSubDirectories,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            if (watchedDir.FileFilters != null)
                foreach (string filter in watchedDir.FileFilters)
                {
                    watcher.Filters.Add(filter);
                }

            watcher.Created += (sender, eventArgs) => HandleEvent(watchedDir, "created", eventArgs.FullPath);
            watcher.Changed += (sender, eventArgs) => HandleEvent(watchedDir, "changed", eventArgs.FullPath);
            watcher.Deleted += (sender, eventArgs) => HandleEvent(watchedDir, "deleted", eventArgs.FullPath);
            watcher.Renamed += (sender, eventArgs) => HandleEvent(watchedDir, "renamed", eventArgs.OldFullPath, newPath: eventArgs.FullPath);

            _watchers.Add(watcher);
            _logger.Log(new InfoLevel(), $"Наблюдение запущено для: {fullPath}");
        }

        private void HandleEvent(WatchedDirectory watchedDir, string eventType, string fullPath, string? newPath = null)
        {
            // Console.WriteLine($"Событие {eventType} обнаружено в {fullPath}"); Печать всех сообщений
            List<FileAction>? FileActions = GetFileActions(watchedDir, eventType);
            if (FileActions == null) return;

            foreach (FileAction action in FileActions)
            {
                _actionExecutor.Execute(action, fullPath, newPath);
            }
        }

        private List<FileAction>? GetFileActions(WatchedDirectory watchedDir, string eventType)
        {
            switch (eventType)
            {
                case "created":
                    return watchedDir.FileActions.Created;
                case "changed":
                    return watchedDir.FileActions.Changed;
                case "deleted":
                    return watchedDir.FileActions.Deleted;
                case "renamed":
                    return watchedDir.FileActions.Renamed;
                default:
                    _logger.Log(new WarningLevel(), text: $"Неизвестное событие: {eventType}. Задействованная директория: {watchedDir.Path}");
                    return null;
            }
        }

        public void StopWatching()
        {
            foreach (FileSystemWatcher watcher in _watchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }
    }
}
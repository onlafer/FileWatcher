namespace FileWatcher.Config
{
    public class ConfigManager
    {
        public required List<WatchedDirectory> WatchedDirectories { get; set; }
    }

    public class WatchedDirectory
    {
        public required string Path { get; set; }
        public required bool IncludeSubDirectories { get; set; }
        // public required bool IsWhitelist { get; set; } = true;
        public required List<string>? FileFilters { get; set; } = new List<string>();
        public required FileActions FileActions { get; set; }
    }

    public class FileActions
    {
        public List<FileAction> Created { get; set; } = new List<FileAction>();
        public List<FileAction> Changed { get; set; } = new List<FileAction>();
        public List<FileAction> Deleted { get; set; } = new List<FileAction>();
        public List<FileAction> Renamed { get; set; } = new List<FileAction>(); // Пока не работает
    }

    public class FileAction
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string Message { get; set; } = "";
        public string Command { get; set; } = "";
        public string Arguments { get; set; } = "";
    }
}
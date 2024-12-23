namespace FileWatcher.Config
{
    public class WatchedDirectory
    {
        public required string Path { get; set; }
        public required string Type { get; set; }
        public required bool IncludeSubDirectories { get; set; }
        public required bool IsWhitelist { get; set; }
        public required string[] FileFilters { get; set; }
        public required Actions Actions { get; set; }
    }

    public class Action
    {
        public required string Type { get; set; }
        public required string Message { get; set; }
        public required string Command { get; set; }
    }

    public class Actions
    {
        public required Action[] Created { get; set; }
    }

    public class Config
    {
        public required WatchedDirectory[] WatchedDirectories { get; set; }
        public required object GlobalSettings { get; set; }
    }
}
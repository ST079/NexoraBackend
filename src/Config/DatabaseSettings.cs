namespace NexoraBackend.Config
{
    public class DatabaseSettings
    {
        public const string SectionName = "Database";
        public string ConnectionString { get; set; } = null!;
        public int CommandTimeout { get; set; } = 30;
        public bool EnableSensitiveDataLogging { get; set; } = false;
        public bool EnableDetailedErrors { get; set; } = false;
    }
}
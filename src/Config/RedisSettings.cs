namespace NexoraBackend.Config;
public class RedisSettings
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = null!;
    public int DefaultExpiryMinutes { get; set; } = 60; 
}
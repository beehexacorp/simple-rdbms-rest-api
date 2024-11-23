using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;

public class ConnectionInfoViewModel
{
    public DbType DbType { get; set; }
    public string Database { get; set; } = null!;
    public string Host { get; set; } = null!;
    public string Port { get; set; } = null!;
    public string User { get; set; } = null!;
}
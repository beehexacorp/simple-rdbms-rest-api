using MessagePack;
using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;


[MessagePackObject]
public class ConnectionInfoViewModel
{
    [MessagePack.Key("id")]
    public Guid Id { get; set; }
    [MessagePack.Key("dbType")]
    public DbType DbType { get; set; }
    [MessagePack.Key("database")]
    public string Database { get; set; } = null!;
    [MessagePack.Key("host")]
    public string Host { get; set; } = null!;
    [MessagePack.Key("port")]
    public string Port { get; set; } = null!;
    [MessagePack.Key("user")]
    public string User { get; set; } = null!;
}
using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;

public class ConnectionInfoDTO
{
    public DbType DbType { get; set; }
    public byte[] ConnectionString { get; set; } = null!;

    public string? GetConnectionString()
    {
        return ConnectionString!.DecryptAES();
    }

}

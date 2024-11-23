using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;

public class ConnectionInfoDTO
{
    public DbType DbType { get; set; }
    public string ConnectionString { get; set; } = null!; // base64

    public string? GetConnectionString()
    {
        return Convert.FromBase64String(ConnectionString).DecryptAES();
    }

}

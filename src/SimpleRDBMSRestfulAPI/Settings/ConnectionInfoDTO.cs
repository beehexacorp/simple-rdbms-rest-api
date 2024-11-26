using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using System;

namespace SimpleRDBMSRestfulAPI.Settings;

public class ConnectionInfoDTO
{
    public Guid Id { get; set; }
    public DbType DbType { get; set; }
    public string ConnectionString { get; set; } = null!; // base64

    public string? GetConnectionString()
    {
        return Convert.FromBase64String(ConnectionString).DecryptAES();
    }

}

using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;

public interface IAppSettings
{
    public ConnectionInfoDTO? GetConnectionInfo();
    string GetConnectionString();
    Task SaveConnectionAsync(DbType dbType, string connectionString);
}

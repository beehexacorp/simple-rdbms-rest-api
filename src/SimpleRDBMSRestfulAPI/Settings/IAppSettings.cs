using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;

public interface IAppSettings
{
    public ConnectionInfoDTO? GetConnectionInfo(Guid connectionId);
    public IEnumerable<ConnectionInfoDTO> GetConnectionInfos();
    string GetConnectionString(Guid connectionId);
    Task<IEnumerable<ConnectionInfoDTO>> SaveConnectionAsync(DbType dbType, string connectionString);
}

using SimpleRDBMSRestfulAPI.Constants;

namespace SimpleRDBMSRestfulAPI.Settings;

public interface IAppSettings
{
    public ConnectionInfoDTO? GetConnectionInfo(Guid connectionId);
    public IEnumerable<ConnectionInfoDTO> GetConnectionInfos();
    public IEnumerable<ConnectionInfoDTO> GetAllMultitenantConnectionInfos();
    Task<string?> GetConnectionString(Guid connectionId);
    Task<ConnectionInfoDTO> SaveConnectionAsync(DbType dbType, string connectionString);
    string RunningMode { get; }
    string EncryptionKey { get; }
}

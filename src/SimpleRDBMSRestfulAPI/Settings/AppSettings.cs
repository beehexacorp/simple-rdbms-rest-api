using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleRDBMSRestfulAPI.Settings;

public class AppSettings : IAppSettings
{
    private readonly string _credentialsPath = Path.Join(Directory.GetCurrentDirectory(), "credentials.enc");
    private static IEnumerable<ConnectionInfoDTO> _connectionInfos = new List<ConnectionInfoDTO>();
    public ConnectionInfoDTO? GetConnectionInfo(Guid connectionId)
    {
        // TODO: create a database table (POSTGRESQL), and implement logics for user's connections registration and retrieval
        var connectionInfos = GetConnectionInfos();
        return connectionInfos?.FirstOrDefault(x => x.Id == connectionId);
    }

    public string GetConnectionString(Guid connectionId)
    {
        return GetConnectionInfo(connectionId)?.GetConnectionString()!;
    }

    public async Task<IEnumerable<ConnectionInfoDTO>> SaveConnectionAsync(DbType dbType, string connectionString)
    {
        var encryptedConnectionString = connectionString.EncryptAES();
        var connectionInfo = new ConnectionInfoDTO
        {
            Id = Guid.NewGuid(),
            DbType = dbType,
            ConnectionString = Convert.ToBase64String(encryptedConnectionString),
        };
        var connectionInfos = GetConnectionInfos() ?? new List<ConnectionInfoDTO>();
        _connectionInfos = connectionInfos.Append(connectionInfo);
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        File.WriteAllText(_credentialsPath, serializer.Serialize(_connectionInfos));
        await Task.CompletedTask;
        return _connectionInfos;
    }

    public IEnumerable<ConnectionInfoDTO> GetConnectionInfos()
    {
        if (_connectionInfos != null && _connectionInfos.Any())
        {
            return _connectionInfos;
        }

        if (!File.Exists(_credentialsPath))
        {
            return new List<ConnectionInfoDTO>();
        }

        var credentialContent = string.Empty;
        using (var fileStream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            byte[] fileBytes = new byte[fileStream.Length];

            // Read the data into the buffer
            fileStream.Read(fileBytes, 0, fileBytes.Length);

            // Optionally, convert bytes to string for demonstration
            credentialContent = System.Text.Encoding.UTF8.GetString(fileBytes);
        }

        if (string.IsNullOrWhiteSpace(credentialContent))
        {
            return new List<ConnectionInfoDTO>();
        }
        // Deserialize YAML to C# object
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _connectionInfos = deserializer.Deserialize<List<ConnectionInfoDTO>>(credentialContent);
        return _connectionInfos;
    }
}
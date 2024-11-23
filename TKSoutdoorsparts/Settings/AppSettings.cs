using SimpleRDBMSRestfulAPI.Constants;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleRDBMSRestfulAPI.Settings;
public class AppSettings : IAppSettings
{
    private readonly string _credentialsPath = Path.Combine(Directory.GetCurrentDirectory(), "credentials.enc");
    private static ConnectionInfoDTO? _connectionInfo = null;
    public ConnectionInfoDTO? GetConnectionInfo()
    {
        if (_connectionInfo != null)
        {
            return _connectionInfo;
        }

        if (!File.Exists(_credentialsPath))
        {
            return null;
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
            return null;
        }
        // Deserialize YAML to C# object
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _connectionInfo = deserializer.Deserialize<ConnectionInfoDTO?>(credentialContent);
        if (_connectionInfo?.ConnectionString == null)
        {
            return null;
        }
        return _connectionInfo;
    }

    public string GetConnectionString()
    {
        return GetConnectionInfo()?.GetConnectionString()!;
    }

    public async Task SaveConnectionAsync(DbType dbType, string connectionString)
    {
        if (File.Exists(_credentialsPath))
        {
            throw new Exception($@"The API has already authorized. Please ask the API owner to cleanup the credentials file first.");
        }

        var encryptedConnectionString = connectionString.EncryptAES();
        _connectionInfo = new ConnectionInfoDTO
        {
            DbType = dbType,
            ConnectionString = Convert.ToBase64String(encryptedConnectionString),
        };
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var credentialContent = serializer.Serialize(_connectionInfo);
        await File.WriteAllTextAsync(_credentialsPath, credentialContent);
    }
}
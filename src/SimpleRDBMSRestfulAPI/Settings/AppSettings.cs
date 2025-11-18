using hexasync.domain.managers;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using SimpleRDBMSRestfulAPI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using hexasync.infrastructure.dotnetenv;
using System.Threading;
using System.Threading.Tasks;
using hexasync.common;
using Microsoft.Extensions.Logging;
using hexasync.domain.profile.models;
using hexasync.domain.cluster_management.DTO;
using Microsoft.EntityFrameworkCore;

namespace SimpleRDBMSRestfulAPI.Settings;

// TODO: authenticate with HexaSync User. Treat this project as an External App, 
// so we also need custom connector in Gateway, Worker. 
// We also need to treat HexaSync SSO as a CONNECTOR.

public class AppSettings(IDbFactory dbFactory, IEnvReader envReader) : IAppSettings
{
    private readonly string _credentialsPath = Path.Join(Directory.GetCurrentDirectory(), "credentials.enc");
    private static IEnumerable<ConnectionInfoDTO> _connectionInfos = new List<ConnectionInfoDTO>();
    private readonly IEnvReader _envReader = envReader;
    private readonly IDbFactory _dbFactory = dbFactory;
    public string RunningMode => _envReader.Read("RUNNING_MODE") ?? "ON_PREM";
    public string EncryptionKey => _envReader.Read("RDBMS_API_SERVICE_SECRET");

    public ConnectionInfoDTO? GetConnectionInfo(Guid connectionId)
    {
        IEnumerable<ConnectionInfoDTO> connectionInfos =
            RunningMode == "MULTITENANT"
                ? GetAllMultitenantConnectionInfos()
                : GetConnectionInfos() ?? Enumerable.Empty<ConnectionInfoDTO>();

        return connectionInfos.FirstOrDefault(c => c.Id == connectionId);
    }

    public async Task<string?> GetConnectionString(Guid connectionId)
    {
        return GetConnectionInfo(connectionId)?.GetConnectionString()!;
    }

    public async Task<ConnectionInfoDTO> SaveConnectionAsync(DbType dbType, string connectionString)
    {
        var encryptedConnectionString = connectionString.EncryptAES();

        if (RunningMode == "MULTITENANT")
        {
            await using var db = _dbFactory.CreateDbContext<ApplicationDbContext>(DatabaseQueryType.DML_WRITE);
            var connectionInfo = new ConnectionInfoModel
            {

                Id = Guid.NewGuid(),
                DbType = dbType,
                ConnectionString = Convert.ToBase64String(encryptedConnectionString)
            };

            db.ConnectionInfo.Add(connectionInfo);
            await db.SaveChangesAsync();

            return new ConnectionInfoDTO
            {
                Id = connectionInfo.Id,
                DbType = connectionInfo.DbType,
                ConnectionString = connectionInfo.ConnectionString
            };
        }

        // Fallback for single-tenant mode (file storage)
        var connectionInfoDto = new ConnectionInfoDTO
        {
            Id = Guid.NewGuid(),
            DbType = dbType,
            ConnectionString = Convert.ToBase64String(encryptedConnectionString)
        };
        var connectionInfos = GetConnectionInfos() ?? new List<ConnectionInfoDTO>();
        _connectionInfos = connectionInfos.Append(connectionInfoDto);
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        File.WriteAllText(_credentialsPath, serializer.Serialize(_connectionInfos));
        await Task.CompletedTask;
        return connectionInfoDto;
    }

    public IEnumerable<ConnectionInfoDTO> GetAllMultitenantConnectionInfos()
    {
        // Tạo DbContext với quyền đọc
        using var db = _dbFactory.CreateDbContext<ApplicationDbContext>(DatabaseQueryType.DML_READ);
        var connectionInfos = db.ConnectionInfo.ToList();
        if (connectionInfos == null || !connectionInfos.Any())
        {
            return new List<ConnectionInfoDTO>();
        }

        return connectionInfos.Select(connInfo => new ConnectionInfoDTO
        {
            Id = connInfo.Id,
            DbType = connInfo.DbType,
            ConnectionString = connInfo.ConnectionString
        }).ToList();
    }

    // public ConnectionInfoDTO? GetMultitenantConnectionInfo(Guid connectionId)
    // {
    //     var db = _dbFactory.CreateDbContext<ApplicationDbContext>(DatabaseQueryType.DML_READ);

    //     var connInfo = db.ConnectionInfo
    //         .FirstOrDefault(x => x.Id == connectionId);

    //     if (connInfo == null)
    //     {
    //         throw new Exception($"ConnectionInfo not found for Id {connectionId}");
    //     }

    //     return new ConnectionInfoDTO
    //     {
    //         Id = connInfo.Id,
    //         DbType = connInfo.DbType,
    //         ConnectionString = connInfo.ConnectionString
    //     };
    // }

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
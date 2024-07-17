using hexasync.infrastructure.dotnetenv;

namespace TKSoutdoorsparts.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly IEnvReader _envReader;
        public AppSettings(IEnvReader envReader)
        {
            _envReader = envReader;
            EnvLoader.LoadEnvs();
        }
        public string ODBCConnectionString => _envReader.Read("ODBC_CONNECTION_STRING");

        public string NpgsqlConnectionString => _envReader.Read("POSTGRES_CONNECTION_STRING");

        public string SqlServerConnectionString => _envReader.Read("SQL_SERVER_CONNECTION_STRING");

        public string MySqlConnectionString => throw new NotImplementedException();
    }
}

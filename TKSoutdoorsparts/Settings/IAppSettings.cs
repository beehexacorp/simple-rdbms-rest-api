namespace TKSoutdoorsparts.Settings
{
    public interface IAppSettings
    {
        public string ODBCConnectionString { get; }
        public string NpgsqlConnectionString { get; }
        public string SqlServerConnectionString { get; }
        public string MySqlConnectionString { get; }
    }
}

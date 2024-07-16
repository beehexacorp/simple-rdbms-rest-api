using System.Data.Odbc;
using System.Data;
using TKSoutdoorsparts.Settings;
using Npgsql;
using DbType = TKSoutdoorsparts.Constants.DbType;
using System.Data.SqlClient;

namespace TKSoutdoorsparts.Factory
{
    public class ConnectionFactory :IConnectionFactory
    {

        private readonly IAppSettings _appSettings;
        public ConnectionFactory(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        public IDbConnection CreateConnection(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.ODBC:
                    return new OdbcConnection(_appSettings.ODBCConnectionString);
                case DbType.POSTGRES:
                    return new NpgsqlConnection(_appSettings.NpgsqlConnectionString);
                case DbType.SQL_SERVER:
                    return new SqlConnection(_appSettings.SqlServerConnectionString);
                default:
                    throw new NotImplementedException($"Dbtype {dbType} is not supported");
            }
        }
    }

}

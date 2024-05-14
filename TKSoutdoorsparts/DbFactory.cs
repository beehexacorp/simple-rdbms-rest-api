using System.Data.Odbc;
using hexasync.infrastructure.dotnetenv;

namespace TKSoutdoorsparts
{
    public class DbFactory : IDbFactory
    {
        private readonly IEnvReader _envReader;
        public DbFactory(IEnvReader envReader)
        {
            _envReader = envReader;
        }
        public virtual OdbcConnection CreateConnection()
        {
            // var connString = _envReader.Read("TKS_ODBC_CONNECTION_STRING");
            var connString = "DRIVER=SQL Anywhere 16;HOST=127.0.0.1:2638;DATABASE=enterprise;Trusted_Connection=Yes;Uid=EXT;Pwd=EXT";
            var connection =  new OdbcConnection(connString);
            connection.Open();
            return connection;
        }
    }
}

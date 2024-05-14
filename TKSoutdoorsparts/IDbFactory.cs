using System.Data.Odbc;

namespace TKSoutdoorsparts
{
    public interface IDbFactory
    {
        OdbcConnection CreateConnection();
    }
}

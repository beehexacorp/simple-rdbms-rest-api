using System.Data;
using TKSoutdoorsparts.Constants;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Factory
{
    public interface IConnectionFactory
    {
        public IDbConnection CreateConnection(DbType dbType);
    }
}

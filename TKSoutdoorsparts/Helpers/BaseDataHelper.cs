
using System.Data;
using TKSoutdoorsparts.Factory;
using Dapper;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public abstract class BaseDataHelper : IDataHelper
{
    protected readonly IConnectionFactory ConnectionFactory;
    public BaseDataHelper(IConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public abstract DbType DbType { get; }
    public virtual async Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object> @params)
    {
        using IDbConnection connection = ConnectionFactory.CreateConnection(DbType);
        var result = await connection.QueryAsync<dynamic>(query, @params);
        var data = result.Cast<IDictionary<string, object>>().ToList();
        return data;
    }
}


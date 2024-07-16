
using System.Data;
using TKSoutdoorsparts.Factory;
using Dapper;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers
{
    public class DataHelper : IDataHelper
    {
        private readonly IConnectionFactory _connectionFactory;
        public DataHelper(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object> @params, DbType dbType)
        {
            using IDbConnection connection = _connectionFactory.CreateConnection(dbType);
            var result = await connection.QueryAsync<dynamic>(query, @params);
            var data = result.Cast<IDictionary<string, object>>().ToList();
            return data;
        }

        /*public void GetDataSetFromAdapter(DataSet dataSet, string connectionString, string queryString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, connection);
                    connection.Open();
                    adapter.Fill(dataSet);
                    connection.Close();
            }
        }

        public void GetSchema(DataSet dataSet, string connectionString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                DataTable tables = connection.GetSchema("Tables");
                DataTable columns = connection.GetSchema("Columns");
                dataSet.Tables.Add(tables);
                dataSet.Tables.Add(columns);
                connection.Close();

            }
        }*/
    }
}

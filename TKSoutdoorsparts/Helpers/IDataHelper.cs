using TKSoutdoorsparts.Factory;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers
{
    public interface IDataHelper
    {
        /*      void GetDataSetFromAdapter(DataSet dataSet, string connectionString, string queryString);
              void GetSchema(DataSet dataSet, string connectionString);*/
        public Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object> @params, DbType dbType);
    }
}

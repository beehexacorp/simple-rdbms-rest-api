using System.Data;

namespace TKSoutdoorsparts.Helpers
{
    public interface IOdbcDataHelper
    {
        void GetDataSetFromAdapter(DataSet dataSet, string connectionString, string queryString);
        void GetSchema(DataSet dataSet, string connectionString);
    }
}

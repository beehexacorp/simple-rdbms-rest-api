using System.Data;
using System.Data.Odbc;

namespace TKSoutdoorsparts.Helpers
{
    public class OdbcDataHelper : IOdbcDataHelper
    {
        public void GetDataSetFromAdapter(DataSet dataSet, string connectionString, string queryString)
        {
            using (OdbcConnection connection =
               new OdbcConnection(connectionString))
            {
                OdbcDataAdapter adapter =
                    new OdbcDataAdapter(queryString, connection);

                // Open the connection and fill the DataSet.
                try
                {
                    connection.Open();
                    adapter.Fill(dataSet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void GetSchema(DataSet dataSet, string connectionString)
        {
            using (OdbcConnection connection =
               new OdbcConnection(connectionString))
            {
                // Open the connection and fill the DataSet.
                try
                {
                    connection.Open();
                    DataTable tables = connection.GetSchema("Tables");
                    DataTable columns = connection.GetSchema("Columns");
                    dataSet.Tables.Add(tables);
                    dataSet.Tables.Add(columns);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

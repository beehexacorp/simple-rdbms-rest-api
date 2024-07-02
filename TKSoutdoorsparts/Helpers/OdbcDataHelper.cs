﻿using System.Data;
using System.Data.Odbc;
using TKSoutdoorsparts.Controllers;

namespace TKSoutdoorsparts.Helpers
{
    public class OdbcDataHelper : IOdbcDataHelper
    {
        public void GetDataSetFromAdapter(DataSet dataSet, string connectionString, string queryString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    OdbcDataAdapter adapter = new OdbcDataAdapter(queryString, connection);
                    connection.Open();
                    adapter.Fill(dataSet);
                    connection.Close();
                }
                catch (Exception)
                {
                    throw;
                   
                }
            }
        }

        public void GetSchema(DataSet dataSet, string connectionString)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DataTable tables = connection.GetSchema("Tables");
                    DataTable columns = connection.GetSchema("Columns");
                    dataSet.Tables.Add(tables);
                    dataSet.Tables.Add(columns);
                    connection.Close();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
    }
}

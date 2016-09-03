using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryFeed
{
    public static class Library
    {
        public static void Execute(string command)
        {

            System.Data.SqlClient.SqlConnection sqlConnection1 =
           new System.Data.SqlClient.SqlConnection("data source=10.10.10.53;initial catalog=IFSReporting;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");

            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = command;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();
        }
    }
}
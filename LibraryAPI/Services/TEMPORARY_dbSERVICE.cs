
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace LibraryAPI.Services
{
    public class TEMPORARY_dbSERVICE
    {
        ConnectionFactory connectionFactory;
        public TEMPORARY_dbSERVICE()
        {
            connectionFactory = new ConnectionFactory();
        }

        public ActionResult<IEnumerable<string>> getAllNames()
        {
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    SELECT * FROM shite;
                    ";

                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        int id = reader.GetInt32(0);
                        string temp = reader.GetString(1);
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("ID: " + id + ", Name: " + temp);
                    }

                }
                return null;
            }
        }
    }
}
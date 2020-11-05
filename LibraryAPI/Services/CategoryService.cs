
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using LibraryAPI.Domains;
using Newtonsoft.Json;

namespace LibraryAPI.Services
{
    public class CategoryService
    {
        ConnectionFactory connectionFactory;
        public CategoryService()
        {
            connectionFactory = new ConnectionFactory();
        }

        public string getAll()
        {
            LinkedList<Category> categories = new LinkedList<Category>();
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    SELECT * FROM category;
                    ";
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        int id = reader.GetInt32(0);
                        string temp = reader.GetString(1);
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("ID: " + id + ", Name: " + temp);
                        categories.AddLast((new Category(id,temp)));
                    }
                cnn.Close();
                }
                return JsonConvert.SerializeObject(categories);
            }
        }
    }
}